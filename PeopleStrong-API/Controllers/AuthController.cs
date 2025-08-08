using AutoMapper;
using Common;
using DTO;
using Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace PeopleStrong_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AuthController(IAuthService authService, IMapper mapper)
        {
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ServiceResponse<bool>>> Register([FromBody] RegisterDto registerdto)
        {
            var isSuccess = await _authService.RegisterAsync(registerdto);
            var response = new ServiceResponse<bool>();

            if (!isSuccess)
            {
                response.Response(false, "User with this email already exists.", false);
                return BadRequest(response);
            }

            response.Response(true, "User registered successfully.", true);
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

        [HttpGet("protected")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ProtectedEndpoint()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { Message = $"Welcome, {userName}!" });
        }
    }
}