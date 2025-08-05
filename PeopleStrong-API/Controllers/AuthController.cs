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
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            try
            {
                // Optional: Manual Mapping if AutoMapper causes issues
                var user = _mapper.Map<User>(dto); // Ensure mapping is configured
                var token = await _authService.RegisterAsync(dto); // If RegisterAsync needs RegisterDto
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAsync(dto);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }

        [HttpGet("protected")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult ProtectedEndpoint()
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            return Ok(new { Message = $"Welcome, {userName}!" });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<UserDto>>> GetUserById(int id)
        {
            var response = await _userService.GetUserByIdAsync(id);

            if (!response.Success)
                return NotFound(response);

            return Ok(response);
        }
    }
}
