using DTO;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
    }
}
