using AutoMapper;
using Common;
using Data.Interfaces;
using DTO;
using Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


 
namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<bool>> RegisterAsync(RegisterDto registerDto)
        {
            var response = new ServiceResponse<bool>();

            var existingUser = await _unitOfWork.Users.GetAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                response.Success = false;
                response.Message = "User with this email already exists.";
                return response;
            }

            CreatePasswordHash(registerDto.Password, out byte[] hash, out byte[] salt);
            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            response.Data = true;
            response.Message = "User registered successfully!";
            return response;
        }

        public async Task<ServiceResponse<string>> LoginAsync(LoginDto loginDto)
        {
            var response = new ServiceResponse<string>();

            var user = await _unitOfWork.Users.GetAsync(u => u.Email == loginDto.Email);
            if (user == null)
            {
                response.Success = false;
                response.Message = "Invalid credentials.";
                return response;
            }

            if (!VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                response.Success = false;
                response.Message = "Invalid credentials.";
                return response;
            }

            response.Data = CreateJwtToken(user);
            response.Message = "Login successful!";
            return response;
        }

        // Corrected to accept a Guid ID
        public async Task<ServiceResponse<UserDto>> GetUserByIdAsync(Guid id)
        {
            var response = new ServiceResponse<UserDto>();
            // Assuming your User entity has a UserId property of type Guid
            var user = await _unitOfWork.Users.GetAsync(u => u.UserId == id);

            if (user == null)
            {
                response.Success = false;
                response.Message = "User not found.";
                return response;
            }

            response.Data = _mapper.Map<UserDto>(user);
            response.Message = "User fetched successfully.";
            return response;
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        private string CreateJwtToken(User user)
        {
            var claims = new[]
            {
                // Corrected to handle a Guid UserId
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public Task<ServiceResponse<UserDto>> GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}