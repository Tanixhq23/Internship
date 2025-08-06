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
            var response = await _authService.RegisterAsync(dto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.RegisterAsync(dto);
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

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ServiceResponse<UserDto>>> GetUserById(Guid id)
        {
            var response = await _authService.GetUserByIdAsync(id);
            if (response == null)
            {
                return NotFound(new ServiceResponse<UserDto> { Success = false, Message = "User not found." });
            }

            if (!response.Success)
            {
                return NotFound(response);
            }
        }
    }
}