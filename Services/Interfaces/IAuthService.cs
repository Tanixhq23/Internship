using Common;
using DTO;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<bool>> RegisterAsync(RegisterDto registerDto);
        Task<ServiceResponse<string>> LoginAsync(LoginDto loginDto);
        Task<ServiceResponse<UserDto>> GetUserByIdAsync(Guid id);
    }
}