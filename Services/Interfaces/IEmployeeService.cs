// File: Services/Interfaces/IEmployeeService.cs

using DTO; // For OnboardingRequestDto, EmployeeCredentialsResponseDto, SendOnboardingEmailRequestDto
using System.Threading.Tasks;
using Common; // For ServiceResponse (if you plan to use it for generic API responses)

namespace Services.Interfaces
{
    public interface IEmployeeService
    {
        
        Task<ServiceResponse<EmployeeCredentialsResponseDto>> OnboardEmployeeAndGenerateCredentialsAsync(OnboardingRequestDto onboardingRequest, int hrUserId);

        
        Task<ServiceResponse<bool>> SendEmployeeCredentialsEmailAsync(SendOnboardingEmailRequestDto request);
    }
}
