using AutoMapper;
using Common;
using Common.Email;
using DTO; // Assuming RegisterDto, LoginDto, ForgotPasswordDto, ResetPasswordDto are here
using Entity; // For FrontendSettings, WelcomeRequest (if needed directly here)
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options; // For IOptions<FrontendSettings>
using Services.Interfaces; // For IAuthService
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
// Make sure to add using for MailRequest and IMailService if they are in different namespaces
// using PeopleStrong_API.Services; // Adjust if your IMailService is in this namespace

namespace PeopleStrong_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService; // Your existing authentication service
        private readonly IMapper _mapper;
        private readonly IMailService _mailService; // ⬅️ **Inject IMailService**
        private readonly FrontendSettings _frontendSettings; // ⬅️ **Inject FrontendSettings**

        public AuthController(
            IAuthService authService,
            IMapper mapper,
            ILogger<AuthController> logger,
            IMailService mailService, // ⬅️ **Add IMailService to constructor**
            IOptions<FrontendSettings> frontendSettings) // ⬅️ **Add IOptions<FrontendSettings> to constructor**
        {
            _authService = authService;
            _mapper = mapper;
            _logger = logger;
            _mailService = mailService; // ⬅️ **Assign injected service**
            _frontendSettings = frontendSettings.Value; // ⬅️ **Get settings value**
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

            // ⬅️ **Trigger Welcome Email after successful registration**
            try
            {
                // Assuming RegisterDto has an Email property and UserName property (or you derive it)
                // You might need to retrieve the created user from authService or pass more data.
                // For this example, assuming RegisterDto has ToEmail and UserName:
                var welcomeRequest = new WelcomeRequest
                {
                    ToEmail = registerdto.Email, // Get email from registration DTO
                    UserName = registerdto.UserName // Get username from registration DTO
                };
                await _mailService.SendWelcomeEmailAsync(welcomeRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", registerdto.Email);
                // Optionally, you might still return success to the user to avoid revealing email issues
                // but log the error internally.
            }

            response.Response(true, "User registered successfully. A welcome email has been sent.", true);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ServiceResponse<string>>> Login([FromBody] LoginDto logindto)
        {
            var token = await _authService.LoginAsync(logindto);
            var response = new ServiceResponse<string>();

            if (token == "Invalid credentials")
            {
                response.Response(false, "Invalid email or password.", null);
                return Unauthorized(response);
            }

            response.Response(true, "Login successful.", token);
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ServiceResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var response = new ServiceResponse<bool>();
            // The ForgotPasswordAsync in IAuthService should handle token generation
            // and saving it to the user.
            var resetToken = await _authService.ForgotPasswordAsync(forgotPasswordDto); // ⬅️ IAuthService should return the generated token or null/empty if email not found

            // IMPORTANT: Always return a generic success message to prevent email enumeration attacks.
            // Only proceed with sending email if a token was successfully generated (meaning email was found).
            if (!string.IsNullOrEmpty(resetToken))
            {
                try
                {
                    // Replace the problematic line in ForgotPassword with the following:

                    string resetLink = $"http://127.0.0.1:5500/index.html?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(forgotPasswordDto.Email)}";
                    
                    string subject = "Password Reset Request";
                    string body = $"Hello,<br/><br/>" +
                                  $"We received a password reset request for your account associated with {forgotPasswordDto.Email}. " +
                                  $"Please click the following link to reset your password:<br/><br/>" +
                                  $"<a href='{resetLink}'>Reset Your Password</a><br/><br/>" +
                                  $"This link will expire soon. If you did not request a password reset, please ignore this email.<br/><br/>" +
                                  $"Regards,<br/>Your App Team";

                    await _mailService.SendEmailAsync(new MailRequest { ToEmail = forgotPasswordDto.Email, Subject = subject, Body = body });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send password reset email to {Email}", forgotPasswordDto.Email);
                    // Continue to return generic success to avoid leaking information
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

            // ⬅️ **Optionally, send a password changed confirmation email**
            try
            {
                string subject = "Your password has been changed";
                string body = $"Hello,<br/><br/>" +
                              $"Your password for the account associated with {resetPasswordDto.Email} has been successfully updated.<br/><br/>" +
                              $"If you did not make this change, please contact support immediately.<br/><br/>" +
                              $"Regards,<br/>Your App Team";
                await _mailService.SendEmailAsync(new MailRequest { ToEmail = resetPasswordDto.Email, Subject = subject, Body = body });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password changed confirmation email to {Email}", resetPasswordDto.Email);
            }

            response.Response(true, "Password has been successfully reset.", true);
            return Ok(response);
        }

        [HttpGet("protected")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ProtectedEndpoint()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { Message = $"Welcome, {userName}!" });
        }
    }
}