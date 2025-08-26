// File: Services/AuthService.cs - Complete and Latest

using AutoMapper;
using Common; // Assuming ServiceResponse is defined here
using Data.Interfaces; // For IUnitOfWork
using DTO; // For RegisterDto, LoginDto, ForgotPasswordDto, ResetPasswordDto, UserDto
using Entity; // For User entity
using Microsoft.EntityFrameworkCore; // For OrderByDescending, FirstOrDefaultAsync
using Microsoft.Extensions.Configuration; // For JWT configuration
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces; // For IAuthService
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography; // For HMACSHA512, RandomNumberGenerator
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        private const int STARTING_USER_ID = 120000; // Define your starting ID for new users

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _unitOfWork.Users.GetAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                return false; // User with this email already exists
            }

            CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // --- Custom User ID Generation Logic ---
            // Find the maximum existing UserId and increment it.
            // If no users exist, start from STARTING_USER_ID.
            int nextUserId = STARTING_USER_ID;
            var lastUser = await _unitOfWork.Users.Query().OrderByDescending(u => u.UserId).FirstOrDefaultAsync();
            if (lastUser != null)
            {
                nextUserId = Math.Max(STARTING_USER_ID, lastUser.UserId + 1);
            }
            // --- End Custom User ID Generation Logic ---

            var newUser = new User
            {
                UserId = nextUserId, // ⬅️ CRUCIAL: Assign the generated User ID
                UserName = registerDto.UserName,
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow,
                //CreatedBy = nextUserId // Assuming the user creating themselves is the creator
            };

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.CompleteAsync();

            return true; // Registration successful
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            var userEnt = await _unitOfWork.Users.GetAsync(u => u.Email == loginDto.Email);

            if (userEnt == null)
            {
                return "Invalid credentials"; // User not found by email
            }

            if (!VerifyPasswordHash(loginDto.Password, userEnt.PasswordHash, userEnt.PasswordSalt))
            {
                return "Invalid credentials"; // Password does not match
            }

            return CreateJwtToken(userEnt);
        }

        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == forgotPasswordDto.Email);

            if (user == null)
            {
                return null; // Return null to prevent email enumeration attacks.
            }

            string resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.PasswordResetToken = resetToken;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return resetToken;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _unitOfWork.Users.GetAsync(u =>
                u.Email == resetPasswordDto.Email &&
                u.PasswordResetToken == resetPasswordDto.Token);

            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
            {
                return false; // Invalid token, email mismatch, or expired token
            }

            CreatePasswordHash(resetPasswordDto.NewPassword, out byte[] newHash, out byte[] newSalt);
            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;

            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return true; // Password successfully reset
        }

        public async Task<UserDto> GetUserByEmailAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == loginDto.Email);
            return _mapper.Map<UserDto>(user);
        }

        public async Task<UserDto> GetUserByEmailAsync(RegisterDto registerDto)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == registerDto.Email);
            return _mapper.Map<UserDto>(user);
        }

        // --- Password Hashing Helper Method ---
        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        // --- Password Verification Helper Method ---
        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (storedHash == null || storedSalt == null || password == null)
            {
                return false;
            }

            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }

        // --- JWT Token Creation Helper Method ---
        private string CreateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // UserId (int) converted to string for claim
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7), // Token valid for 7 days
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
 