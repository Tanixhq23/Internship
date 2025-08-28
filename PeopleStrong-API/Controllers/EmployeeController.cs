// File: PeopleStrong_API/Controllers/EmployeeController.cs

using DTO; // For OnboardingRequestDto, SendOnboardingEmailRequestDto, EmployeeCredentialsResponseDto
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Services.Interfaces; // For IEmployeeService
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Common; // For ServiceResponse

namespace PeopleStrong_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Base route: /api/Employee
    // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // ⬅️ TEMPORARILY COMMENT THIS OUT
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService;
            _logger = logger;
        }

        [HttpPost("onboard-employee")]
        // [Authorize(Roles = "HR, Admin")] // ⬅️ If specific actions also have it, comment them out too
        public async Task<ActionResult<ServiceResponse<EmployeeCredentialsResponseDto>>> OnboardEmployee([FromBody] OnboardingRequestDto request)
        {
            var response = new ServiceResponse<EmployeeCredentialsResponseDto>();

            if (!ModelState.IsValid)
            {
                response.Response(false, "Invalid onboarding request data.", null);
                return BadRequest(response);
            }

            // Since Authorize is commented out, User.FindFirst will be null.
            // For temporary testing, you might need to hardcode a hrUserId or make it optional in service.
            // int hrUserId = 1; // ⬅️ TEMPORARY: You might hardcode an ID for testing if needed by the service

            // To prevent NRE, we can use a default or handle null carefully
            int hrUserId = 0; // Default to 0 if not authenticated. Service should handle this.
            var hrUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (hrUserIdClaim != null && int.TryParse(hrUserIdClaim.Value, out int authenticatedHrUserId))
            {
                hrUserId = authenticatedHrUserId;
            }


            try
            {
                var onboardingResult = await _employeeService.OnboardEmployeeAndGenerateCredentialsAsync(request, hrUserId);

                if (onboardingResult.Success && onboardingResult.Data != null)
                {
                    _logger.LogInformation("Employee onboarded successfully by HR User {HrUserId}. EmpId: {EmpId}, UserId: {UserId}.",
                        hrUserId, onboardingResult.Data.EmpId, onboardingResult.Data.UserId);
                    return Ok(onboardingResult);
                }
                else
                {
                    _logger.LogError("Employee onboarding failed for HR User {HrUserId}: {Message}", hrUserId, onboardingResult.Message);
                    return BadRequest(onboardingResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during employee onboarding by HR User {HrUserId}.", hrUserId);
                response.Response(false, "An unexpected error occurred during employee onboarding.", null);
                return StatusCode(500, response);
            }
        }

        [HttpPost("send-credentials-email")]
        // [Authorize(Roles = "HR, Admin")] // ⬅️ If specific actions also have it, comment them out too
        public async Task<ActionResult<ServiceResponse<bool>>> SendCredentialsEmail([FromBody] SendOnboardingEmailRequestDto request)
        {
            var response = new ServiceResponse<bool>();

            if (!ModelState.IsValid)
            {
                response.Response(false, "Invalid email request data.", false);
                return BadRequest(response);
            }

            int hrUserId = 0; // Default to 0 if not authenticated.
            var hrUserIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (hrUserIdClaim != null && int.TryParse(hrUserIdClaim.Value, out int authenticatedHrUserId))
            {
                hrUserId = authenticatedHrUserId;
            }

            try
            {
                var emailResult = await _employeeService.SendEmployeeCredentialsEmailAsync(request);

                if (emailResult.Success)
                {
                    _logger.LogInformation("Employee credentials email sent successfully by HR User {HrUserId} to {PersonalEmail} for EmpId {EmpId}.",
                        hrUserId, request.PersonalEmail, request.EmpId);
                    return Ok(emailResult);
                }
                else
                {
                    _logger.LogError("Failed to send employee credentials email by HR User {HrUserId} to {PersonalEmail} for EmpId {EmpId}: {Message}",
                        hrUserId, request.PersonalEmail, request.EmpId, emailResult.Message);
                    return BadRequest(emailResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending employee credentials email by HR User {HrUserId} for EmpId {EmpId}.",
                    hrUserId, request.EmpId);
                response.Response(false, "An unexpected error occurred while sending credentials email.", false);
                return StatusCode(500, response);
            }
        }
    }
}
