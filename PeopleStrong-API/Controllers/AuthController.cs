// File: PeopleStrong_API.Controllers/AuthController.cs - Complete and Latest

using AutoMapper;
using Common; // Assuming ServiceResponse is defined here
using Common.Email;
using DTO; // Assuming RegisterDto, LoginDto, ForgotPasswordDto, ResetPasswordDto are here
using Entity; // For FrontendSettings, WelcomeRequest (assuming they are in Entity or Common)
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Interfaces; // For IAuthService, IMailService
using System; // Required for Uri.EscapeDataString
using System.Security.Claims;
using System.Threading.Tasks;

namespace PeopleStrong_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly FrontendSettings _frontendSettings;

        public AuthController(
            IAuthService authService,
            IMapper mapper,
            ILogger<AuthController> logger,
            IMailService mailService,
            IOptions<FrontendSettings> frontendSettings)
        {
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
            _mailService = mailService;
            _frontendSettings = frontendSettings.Value;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<bool>>> Register([FromBody] RegisterDto registerdto)
        {
            var response = new ServiceResponse<bool>();
            var isSuccess = await _authService.RegisterAsync(registerdto);

            if (!isSuccess)
            {
                response.Response(false, "User with this email already exists.", false);
                return BadRequest(response);
            }

            var emailSent = await _mailService.SendWelcomeEmailAsync(new WelcomeRequest
            {
                ToEmail = registerdto.Email,
                UserName = registerdto.UserName
            });

            if (!emailSent)
            {
                _logger.LogWarning("Failed to send welcome email for registered user {Email}. See MailService logs for details.", registerdto.Email);
            }

            response.Response(true, "User registered successfully. A welcome email has been sent (or attempted).", true);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<object>>> Login([FromBody] LoginDto loginDto)
        {
            var response = new ServiceResponse<object>();

            var token = await _authService.LoginAsync(loginDto);

            if (token == "Invalid credentials")
            {
                response.Response(false, "Invalid email or password. Please try again.", null);
                return Unauthorized(response);
            }

            // Success: Token is now a string. Wrap it in an anonymous object as { token: '...' }
            response.Response(true, "Login successful.", new { token = token });
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ServiceResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var response = new ServiceResponse<bool>();
            var resetToken = await _authService.ForgotPasswordAsync(forgotPasswordDto);

            if (!string.IsNullOrEmpty(resetToken))
            {
                string resetLink = $"{_frontendSettings.ResetPasswordUrl}?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(forgotPasswordDto.Email)}";

                string subject = "Password Reset Request";
                string body = $"Hello,<br/><br/>" +
                              $"We received a password reset request for your account associated with {forgotPasswordDto.Email}. <br/><br/>" +
                              $"Please click the following link to reset your password:<br/><br/>" +
                              $"<a href='{resetLink}'>Reset Your Password</a><br/><br/>" +
                              $"This link will expire soon. If you did not request a password reset, please ignore this email.<br/><br/>" +
                              $"Regards,<br/>Your App Team";

                var emailSent = await _mailService.SendEmailAsync(new MailRequest { ToEmail = forgotPasswordDto.Email, Subject = subject, Body = body });

                if (!emailSent)
                {
                    _logger.LogWarning("Failed to send password reset email to {Email}. See MailService logs for details.", forgotPasswordDto.Email);
                }
            }

            response.Response(true, "If your email is in our system, you will receive a password reset link.", true);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ServiceResponse<bool>>> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var isSuccess = await _authService.ResetPasswordAsync(resetPasswordDto);
            var response = new ServiceResponse<bool>();

            if (!isSuccess)
            {
                response.Response(false, "Invalid token or email, or token has expired.", false);
                return BadRequest(response);
            }

            string subject = "Your password has been changed";
            string body = $"Hello,<br/><br/>" +
                          $"Your password for the account associated with {resetPasswordDto.Email} has been successfully updated.<br/><br/>" +
                          $"If you did not make this change, please contact support immediately.<br/><br/>" +
                          $"Regards,<br/>Your App Team";

            var emailSent = await _mailService.SendEmailAsync(new MailRequest { ToEmail = resetPasswordDto.Email, Subject = subject, Body = body });

            if (!emailSent)
            {
                _logger.LogWarning("Failed to send password changed confirmation email to {Email}. See MailService logs for details.", resetPasswordDto.Email);
            }

            response.Response(true, "Password has been successfully reset.", true);
            return Ok(response);
        }

        [HttpGet("protected")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ProtectedEndpoint()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                _logger.LogWarning("ProtectedEndpoint access failed: User ID not found in token or invalid format.");
                return Unauthorized("User ID not found in token or invalid format.");
            }
            var userName = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { Message = $"Welcome, {userName}! Your User ID is {userId}." });
        }
    }
}
