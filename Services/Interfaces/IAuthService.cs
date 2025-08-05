using Common;
using DTO;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<ServiceResponse<UserDto>> GetUserByIdAsync(int id);
    }
}