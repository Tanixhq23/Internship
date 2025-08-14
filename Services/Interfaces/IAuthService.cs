using Common;
using DTO;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<UserDto> GetUserByEmailAsync(LoginDto loginDto);
        Task<UserDto> GetUserByEmailAsync(RegisterDto registerDto);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}