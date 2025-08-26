// File: Services/Interfaces/IAttendanceService.cs

using DTO; // For AttendanceRecordDto, DailyAttendanceSummaryDto
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IAttendanceService
    {
        // For Punch In
        Task<AttendanceRecordDto> PunchInAsync(int userId, string? location);

        // For Punch Out
        Task<AttendanceRecordDto> PunchOutAsync(int userId, string? location);

        // Optional: Get daily attendance summary for a specific user
        Task<DailyAttendanceSummaryDto> GetDailyAttendanceSummaryAsync(int userId, DateTime date);

        // Optional: Get daily attendance summary for all users (for Admin/HR)
        Task<List<DailyAttendanceSummaryDto>> GetAllUsersDailyAttendanceSummaryAsync(DateTime date);

        // Optional: Get all attendance records for a user (e.g., historical data)
        Task<List<AttendanceRecordDto>> GetUserAttendanceRecordsAsync(int userId, DateTime? startDate, DateTime? endDate);

        // Optional: Handle missing punch-outs (e.g., nightly job or manual fix)
        Task<bool> HandleMissingPunchOutsAsync();
    }
}
