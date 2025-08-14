// In Services/AuthService.cs (or wherever your IAuthService is implemented)

using AutoMapper;
using Common;
using Data.Interfaces; // Assuming IUnitOfWork is defined here
using DTO; // Assuming RegisterDto, LoginDto, ForgotPasswordDto, ResetPasswordDto, UserDto are here
using Entity; // Assuming your User entity is here
using Microsoft.EntityFrameworkCore; // For EF Core methods like .AnyAsync, but _unitOfWork abstracts this
using Microsoft.Extensions.Configuration; // For JWT configuration
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces; // Assuming IAuthService is here
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class AuthService : IAuthService // Your class implementing IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration; // For accessing appsettings.json for Jwt:Key
        private readonly IMapper _mapper; // For mapping User entity to UserDto

        // Constructor to inject IUnitOfWork, IConfiguration, and IMapper
        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _mapper = mapper;
        }

        // --- RegisterAsync Method ---
        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user with this email already exists using Unit of Work's GetAsync
            var existingUser = await _unitOfWork.Users.GetAsync(u => u.Email == registerDto.Email);
            if (existingUser != null)
            {
                return false; // User with this email already exists
            }

            // Create password hash and salt
            CreatePasswordHash(registerDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            // Create new user entity
            var newUser = new User
            {
                UserName = registerDto.UserName,
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow, // Assuming CreatedAt is part of your User model
                // Other properties like EmpId, RoleId, CreatedBy should be handled based on your application's logic
            };

            // Add the new user via Unit of Work and complete the transaction
            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.CompleteAsync(); // Save changes to the database

            return true; // Registration successful
        }

        // --- LoginAsync Method ---
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            // Retrieve user by email using Unit of Work
            var userEnt = await _unitOfWork.Users.GetAsync(u => u.Email == loginDto.Email);

            if (userEnt == null || !VerifyPasswordHash(loginDto.Password, userEnt.PasswordHash, userEnt.PasswordSalt))
            {
                return "Invalid credentials"; // User not found or password does not match
            }

            // Generate and return a JWT token upon successful login
            return CreateJwtToken(userEnt);
        }

        // --- ForgotPasswordAsync Method ---
        public async Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            // 1. Find the user by email using Unit of Work
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == forgotPasswordDto.Email);

            if (user == null)
            {
                // IMPORTANT: Do NOT indicate if the email was not found to prevent email enumeration attacks.
                // Return null so the controller can send a generic message.
                return null;
            }

            // 2. Generate a secure, unique password reset token
            // Using Base64String for better URL compatibility compared to HexString for tokens
            string resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)); // Generates a 64-byte random token, Base64 encoded

            // 3. Store the token and its expiry in the user's database record
            user.PasswordResetToken = resetToken;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour from now

            // 4. Update the user via Unit of Work and complete the transaction
            _unitOfWork.Users.Update(user); // Assuming Update method marks entity as modified
            await _unitOfWork.CompleteAsync(); // Save changes to the database

            // 5. Return the generated token string
            return resetToken;
        }

        // --- ResetPasswordAsync Method ---
        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            // Find the user by email AND token, and check if the token is still valid/not expired
            var user = await _unitOfWork.Users.GetAsync(u =>
                u.Email == resetPasswordDto.Email &&
                u.PasswordResetToken == resetPasswordDto.Token); // GetAsync with predicate

            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
            {
                return false; // Invalid token, email mismatch, or expired token
            }

            // Hash the new password and update user record
            CreatePasswordHash(resetPasswordDto.NewPassword, out byte[] newHash, out byte[] newSalt);
            user.PasswordHash = newHash;
            user.PasswordSalt = newSalt;

            // Clear the token and its expiry after successful reset
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            // Update the user via Unit of Work and complete the transaction
            _unitOfWork.Users.Update(user);
            await _unitOfWork.CompleteAsync();

            return true; // Password successfully reset
        }

        // --- GetUserByEmailAsync methods (used internally or exposed for mapping DTOs) ---
        public async Task<UserDto> GetUserByEmailAsync(LoginDto loginDto)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == loginDto.Email);
            return _mapper.Map<UserDto>(user); // Maps User entity to UserDto
        }

        public async Task<UserDto> GetUserByEmailAsync(RegisterDto registerDto)
        {
            var user = await _unitOfWork.Users.GetAsync(u => u.Email == registerDto.Email);
            return _mapper.Map<UserDto>(user); // Maps User entity to UserDto
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
            // Add null checks for storedHash/Salt for robustness
            if (storedHash == null || storedSalt == null)
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
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Assuming UserId is the ID property
                new Claim(ClaimTypes.Name, user.UserName ?? user.Email),
                new Claim(ClaimTypes.Email, user.Email),
            };

            // Retrieve JWT key from configuration
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
