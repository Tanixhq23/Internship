// File: Services/AttendanceService.cs (Updated for Email on Punch Out)

using AutoMapper;
using Data.Interfaces; // For IUnitOfWork, IAttendanceRepository, IUserRepository
using DTO; // For AttendanceRecordDto, DailyAttendanceSummaryDto, etc.
using Entity; // For AttendanceRecord, User, MailRequest
using Microsoft.Extensions.Logging;
using Services.Interfaces; // For IAttendanceService, IMailService
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AttendanceService> _logger;
        private readonly IMailService _mailService; // ⬅️ Inject IMailService here

        public AttendanceService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AttendanceService> logger,
            IMailService mailService) // ⬅️ Add IMailService to constructor
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _mailService = mailService; // ⬅️ Assign injected service
        }

        public async Task<AttendanceRecordDto> PunchInAsync(int userId, string? location)
        {
            var activeRecord = await _unitOfWork.AttendanceRecords.GetActivePunchInForUserAsync(userId);

            if (activeRecord != null)
            {
                _logger.LogWarning("PunchIn attempt for UserId {UserId} failed: Already has an active punch-in record (ID: {RecordId}).", userId, activeRecord.Id);
                return null;
            }

            var newRecord = new AttendanceRecord
            {
                UserId = userId,
                PunchInTime = DateTime.UtcNow,
                PunchInLocation = location,
                Status = "PunchedIn",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = userId
            };

            await _unitOfWork.AttendanceRecords.AddAsync(newRecord);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("UserId {UserId} successfully punched in (Record ID: {RecordId}).", userId, newRecord.Id);
            return _mapper.Map<AttendanceRecordDto>(newRecord);
        }

        public async Task<AttendanceRecordDto> PunchOutAsync(int userId, string? location)
        {
            var activeRecord = await _unitOfWork.AttendanceRecords.GetActivePunchInForUserAsync(userId);

            if (activeRecord == null)
            {
                _logger.LogWarning("PunchOut attempt for UserId {UserId} failed: No active punch-in record found.", userId);
                return null;
            }

            activeRecord.PunchOutTime = DateTime.UtcNow;
            activeRecord.PunchOutLocation = location;
            activeRecord.Status = "PunchedOut";
            activeRecord.UpdatedAt = DateTime.UtcNow;
            activeRecord.UpdatedByUserId = userId;

            _unitOfWork.AttendanceRecords.Update(activeRecord);
            await _unitOfWork.CompleteAsync(); // Save to database before sending email

            _logger.LogInformation("UserId {UserId} successfully punched out (Record ID: {RecordId}).", userId, activeRecord.Id);

            // ⬅️ NEW: Send attendance summary email after successful punch-out
            var user = await _unitOfWork.Users.GetAsync(u => u.UserId == userId);
            if (user != null)
            {
                var dailySummary = await GetDailyAttendanceSummaryAsync(userId, DateTime.Today); // Get summary for today

                if (dailySummary != null)
                {
                    string subject = "Your Daily Attendance Summary";
                    string emailBody = $"Hello {user.UserName},<br/><br/>" +
                                       $"Here is your attendance summary for {DateTime.Today.ToShortDateString()}:<br/><br/>";

                    foreach (var recordDto in dailySummary.Records.OrderBy(r => r.PunchInTime))
                    {
                        emailBody += $"&bull; Punched In: {recordDto.PunchInTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}<br/>";
                        if (recordDto.PunchOutTime.HasValue)
                        {
                            emailBody += $"&bull; Punched Out: {recordDto.PunchOutTime.Value.ToLocalTime():yyyy-MM-dd HH:mm:ss}<br/>";
                        }
                        else
                        {
                            emailBody += $"&bull; Status: {recordDto.Status}<br/>";
                        }
                    }

                    if (dailySummary.TotalDuration.HasValue)
                    {
                        emailBody += $"<br/><b>Total Hours Worked: {dailySummary.TotalDuration.Value.TotalHours:F2} hours</b><br/><br/>";
                    }

                    if (dailySummary.HasMissingPunchOut)
                    {
                        emailBody += "<i>Note: You have a missing punch-out record. Total hours might be estimated.</i><br/><br/>";
                    }

                    emailBody += "Regards,<br/>People Strong";

                    var mailRequest = new MailRequest
                    {
                        ToEmail = user.Email,
                        Subject = subject,
                        Body = emailBody
                    };

                    var emailSent = await _mailService.SendEmailAsync(mailRequest);
                    if (!emailSent)
                    {
                        _logger.LogWarning("Failed to send attendance summary email to {Email} for UserId {UserId}.", user.Email, userId);
                    }
                    else
                    {
                        _logger.LogInformation("Attendance summary email sent to {Email} for UserId {UserId}.", user.Email, userId);
                    }
                }
                else
                {
                    _logger.LogWarning("No daily attendance summary found to send email for UserId {UserId} after punch-out.", userId);
                }
            }
            else
            {
                _logger.LogWarning("User not found (UserId {UserId}) to send attendance summary email.", userId);
            }

            return _mapper.Map<AttendanceRecordDto>(activeRecord);
        }

        public async Task<DailyAttendanceSummaryDto> GetDailyAttendanceSummaryAsync(int userId, DateTime date)
        {
            var records = await _unitOfWork.AttendanceRecords.GetRecordsForUserAndDayAsync(userId, date);

            if (!records.Any())
            {
                return null;
            }

            var summary = new DailyAttendanceSummaryDto
            {
                UserId = userId,
                Date = date.Date,
                Records = _mapper.Map<List<AttendanceRecordDto>>(records).ToList()
            };

            TimeSpan totalDuration = TimeSpan.Zero;
            bool hasMissingPunchOut = false;

            foreach (var record in records)
            {
                if (record.PunchOutTime.HasValue)
                {
                    totalDuration += (record.PunchOutTime.Value - record.PunchInTime);
                }
                else
                {
                    if (record.Status == "PunchedIn")
                    {
                        hasMissingPunchOut = true;
                        totalDuration += (DateTime.UtcNow - record.PunchInTime);
                    }
                }
            }

            summary.TotalDuration = totalDuration;
            summary.HasMissingPunchOut = hasMissingPunchOut;

            var user = await _unitOfWork.Users.GetAsync(u => u.UserId == userId);
            if (user != null)
            {
                summary.UserName = user.UserName;
                foreach (var recDto in summary.Records)
                {
                    recDto.UserName = user.UserName;
                    if (recDto.PunchOutTime.HasValue)
                    {
                        recDto.TotalHours = recDto.PunchOutTime.Value - recDto.PunchInTime;
                    }
                    else if (recDto.Status == "PunchedIn")
                    {
                        recDto.TotalHours = DateTime.UtcNow - recDto.PunchInTime;
                    }
                }
            }

            return summary;
        }

        public async Task<List<DailyAttendanceSummaryDto>> GetAllUsersDailyAttendanceSummaryAsync(DateTime date)
        {
            var allDailyRecords = await _unitOfWork.AttendanceRecords.GetAllRecordsForDayAsync(date);

            var userIds = allDailyRecords.Select(r => r.UserId).Distinct().ToList();
            var allUsers = await _unitOfWork.Users.GetAllAsync(u => userIds.Contains(u.UserId));

            var summaryList = new List<DailyAttendanceSummaryDto>();

            foreach (var userId in userIds)
            {
                var userRecordsForDay = allDailyRecords.Where(r => r.UserId == userId).ToList();

                var summary = new DailyAttendanceSummaryDto
                {
                    UserId = userId,
                    Date = date.Date,
                    Records = _mapper.Map<List<AttendanceRecordDto>>(userRecordsForDay).ToList()
                };

                TimeSpan totalDuration = TimeSpan.Zero;
                bool hasMissingPunchOut = false;

                foreach (var record in userRecordsForDay)
                {
                    if (record.PunchOutTime.HasValue)
                    {
                        totalDuration += (record.PunchOutTime.Value - record.PunchInTime);
                    }
                    else
                    {
                        if (record.Status == "PunchedIn")
                        {
                            hasMissingPunchOut = true;
                            totalDuration += (DateTime.UtcNow - record.PunchInTime);
                        }
                    }
                }

                summary.TotalDuration = totalDuration;
                summary.HasMissingPunchOut = hasMissingPunchOut;

                var user = allUsers.FirstOrDefault(u => u.UserId == userId);
                if (user != null)
                {
                    summary.UserName = user.UserName;
                    foreach (var recDto in summary.Records)
                    {
                        recDto.UserName = user.UserName;
                        if (recDto.PunchOutTime.HasValue)
                        {
                            recDto.TotalHours = recDto.PunchOutTime.Value - recDto.PunchInTime;
                        }
                        else if (recDto.Status == "PunchedIn")
                        {
                            recDto.TotalHours = DateTime.UtcNow - recDto.PunchInTime;
                        }
                    }
                }
                summaryList.Add(summary);
            }
            return summaryList;
        }

        public async Task<List<AttendanceRecordDto>> GetUserAttendanceRecordsAsync(int userId, DateTime? startDate, DateTime? endDate)
        {
            var records = await _unitOfWork.AttendanceRecords.GetAllAsync(ar =>
                ar.UserId == userId &&
                (!startDate.HasValue || ar.PunchInTime >= startDate.Value.Date) &&
                (!endDate.HasValue || ar.PunchInTime < endDate.Value.Date.AddDays(1))
            );

            var recordDtos = _mapper.Map<List<AttendanceRecordDto>>(records.OrderByDescending(ar => ar.PunchInTime)).ToList();

            var user = await _unitOfWork.Users.GetAsync(u => u.UserId == userId);
            if (user != null)
            {
                foreach (var recDto in recordDtos)
                {
                    recDto.UserName = user.UserName;
                    if (recDto.PunchOutTime.HasValue)
                    {
                        recDto.TotalHours = recDto.PunchOutTime.Value - recDto.PunchInTime;
                    }
                    else if (recDto.Status == "PunchedIn")
                    {
                        recDto.TotalHours = DateTime.UtcNow - recDto.PunchInTime;
                    }
                }
            }
            return recordDtos;
        }

        public async Task<bool> HandleMissingPunchOutsAsync()
        {
            var oldActiveRecords = await _unitOfWork.AttendanceRecords.GetAllAsync(
                ar => ar.PunchOutTime == null && ar.Status == "PunchedIn" &&
                      ar.PunchInTime < DateTime.UtcNow.AddDays(-1)
            );

            if (!oldActiveRecords.Any())
            {
                _logger.LogInformation("No old missing punch-outs found to process.");
                return true;
            }

            foreach (var record in oldActiveRecords)
            {
                record.PunchOutTime = record.PunchInTime.Date.AddHours(17);
                record.Status = "MissedPunchOut";
                record.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.AttendanceRecords.Update(record);
                _logger.LogWarning("Marked missing punch-out for UserId {UserId}, Record ID {RecordId}.", record.UserId, record.Id);
            }

            await _unitOfWork.CompleteAsync();
            _logger.LogInformation("Processed {Count} missing punch-out records.", oldActiveRecords.Count());
            return true;
        }
    }
}
