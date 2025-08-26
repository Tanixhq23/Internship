// File: PeopleStrong_API/Controllers/AttendanceController.cs

using Microsoft.AspNetCore.Mvc;
using DTO;
using Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Authorization; // Comment out this using statement
// using Microsoft.AspNetCore.Authentication.JwtBearer; // Comment out this using statement
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
// [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // ⬅️ TEMPORARILY COMMENT THIS OUT
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly ILogger<AttendanceController> _logger;

    public AttendanceController(IAttendanceService attendanceService, ILogger<AttendanceController> logger)
    {
        _attendanceService = attendanceService;
        _logger = logger;
    }

    [HttpPost("punch-in")]
    // [Authorize] // ⬅️ If specific actions also have it, comment them out too
    public async Task<IActionResult> PunchIn([FromBody] AttendancePunchInDto request)
    {
        // You'll need to mock a userId or pass it in the request body for testing
        // For temporary testing without auth, you might hardcode a userId
        int userId = 120000; // ⬅️ TEMPORARY: Hardcode a UserId for unauthenticated testing

        try
        {
            int effectiveUserId = request.UserId ?? userId; // Still respect request.UserId if provided

            var record = await _attendanceService.PunchInAsync(effectiveUserId, request.Location);

            if (record == null)
            {
                return BadRequest("Cannot punch in. You might already have an active punch-in record.");
            }
            return Ok(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PunchIn for UserId: {UserId}", userId);
            return StatusCode(500, "An error occurred while processing your punch-in.");
        }
    }

    [HttpPost("punch-out")]
    // [Authorize] // ⬅️ If specific actions also have it, comment them out too
    public async Task<IActionResult> PunchOut([FromBody] AttendancePunchOutDto request)
    {
        int userId = 120000; // ⬅️ TEMPORARY: Hardcode a UserId for unauthenticated testing

        try
        {
            int effectiveUserId = request.UserId ?? userId;

            var record = await _attendanceService.PunchOutAsync(effectiveUserId, request.Location);

            if (record == null)
            {
                return BadRequest("Cannot punch out. No active punch-in record found to close.");
            }
            return Ok(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during PunchOut for UserId: {UserId}", userId);
            return StatusCode(500, "An error occurred while processing your punch-out.");
        }
    }

    [HttpGet("my-daily-attendance")]
    // [Authorize] // ⬅️ If specific actions also have it, comment them out too
    public async Task<IActionResult> GetMyDailyAttendance()
    {
        int userId = 120000; // ⬅️ TEMPORARY: Hardcode a UserId for unauthenticated testing

        try
        {
            var summary = await _attendanceService.GetDailyAttendanceSummaryAsync(userId, DateTime.Today);
            if (summary == null)
            {
                return NotFound($"No attendance records found for user {userId} today.");
            }
            return Ok(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving daily attendance for UserId: {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving daily attendance.");
        }
    }

    // For this endpoint, you might want to keep some form of authorization for roles
    [HttpGet("daily-attendance/{date}")]
    // [Authorize(Roles = "Admin, HR", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // ⬅️ TEMPORARILY COMMENT THIS OUT OR ADJUST
    public async Task<IActionResult> GetDailyAttendanceByDate(DateTime date)
    {
        try
        {
            var summaryList = await _attendanceService.GetAllUsersDailyAttendanceSummaryAsync(date.Date);
            if (summaryList == null || !summaryList.Any())
            {
                return NotFound($"No attendance records found for {date.ToShortDateString()}.");
            }
            return Ok(summaryList);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users' daily attendance for date: {Date}", date.ToShortDateString());
            return StatusCode(500, "An error occurred while retrieving daily attendance.");
        }
    }
}
