// File: Services/EmployeeService.cs

using AutoMapper;
using Common; // For ServiceResponse
using Common.Email;
using Data.Interfaces; // For IUnitOfWork
using DTO; // For OnboardingRequestDto, EmployeeCredentialsResponseDto, SendOnboardingEmailRequestDto
using Entity; // For User, Employee, MailRequest, FrontendSettings
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // For accessing appsettings.json
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; // For IOptions<FrontendSettings>
using Services.Interfaces; // For IEmployeeService, IAuthService, IMailService
using System;
using System.Security.Cryptography; // For RandomNumberGenerator, HMACSHA512
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IMailService _mailService;
        private readonly FrontendSettings _frontendSettings;

        // Using private const for password generation for consistency
        private const int TEMPORARY_PASSWORD_LENGTH = 10;
        private const string TEMPORARY_PASSWORD_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";

        // Define starting IDs for sequential generation (if not using database auto-increment)
        private const int STARTING_USER_ID = 120000;
        private const int STARTING_EMP_ID = 10000;

        public EmployeeService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<EmployeeService> logger,
            IMailService mailService,
            IOptions<FrontendSettings> frontendSettings)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
            _mailService = mailService;
            _frontendSettings = frontendSettings.Value;
        }

        /// <summary>
        /// Generates a random temporary password.
        /// </summary>
        private string GenerateTemporaryPassword()
        {
            var password = new char[TEMPORARY_PASSWORD_LENGTH];
            var rng = new Random(); // Consider using RandomNumberGenerator for cryptographically secure random numbers
            for (int i = 0; i < TEMPORARY_PASSWORD_LENGTH; i++)
            {
                password[i] = TEMPORARY_PASSWORD_CHARS[rng.Next(TEMPORARY_PASSWORD_CHARS.Length)];
            }
            return new string(password);
        }

        /// <summary>
        /// Hashes a password and generates a salt.
        /// </summary>
        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public async Task<ServiceResponse<EmployeeCredentialsResponseDto>> OnboardEmployeeAndGenerateCredentialsAsync(OnboardingRequestDto onboardingRequest, int hrUserId)
        {
            var response = new ServiceResponse<EmployeeCredentialsResponseDto>();

            // 1. Generate unique EmpId and UserId
            int nextEmpId = await _unitOfWork.Employees.GetNextEmpIdAsync(); // Uses EmployeeRepository's method

            // Re-use logic from AuthService for UserId generation for consistency
            int nextUserId = STARTING_USER_ID;
            var lastUser = await _unitOfWork.Users.Query().OrderByDescending(u => u.UserId).FirstOrDefaultAsync();
            if (lastUser != null)
            {
                nextUserId = Math.Max(STARTING_USER_ID, lastUser.UserId + 1);
            }

            // 2. Generate Office Email (e.g., firstname.lastname@yourcompany.com)
            // A more robust solution might handle duplicates (e.g., adding numbers)
            string officeEmail = $"{onboardingRequest.FullName.Replace(" ", "").ToLower()}@peoplestrong.com";
            // Check for existing user with this office email
            var existingUserWithOfficeEmail = await _unitOfWork.Users.GetAsync(u => u.Email == officeEmail);
            if (existingUserWithOfficeEmail != null)
            {
                // Handle duplicate office email, e.g., append a number or throw a more specific error
                response.Response(false, "Generated office email already exists. Please choose a different name or manually adjust.", null);
                _logger.LogWarning("Onboarding failed: Generated office email '{OfficeEmail}' already exists.", officeEmail);
                return response;
            }


            // 3. Generate Temporary Password
            string temporaryPassword = GenerateTemporaryPassword();
            CreatePasswordHash(temporaryPassword, out byte[] passwordHash, out byte[] passwordSalt);

            // 4. Create User Account (for login)
            var newUser = new User
            {
                UserId = nextUserId,
                UserName = onboardingRequest.FullName, // Or generated from OfficeEmail
                FullName = onboardingRequest.FullName,
                Email = officeEmail, // Office email for login
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CreatedAt = DateTime.UtcNow,
                //CreatedBy = UserId, // HR's UserId who onboarded
                //EmpId = nextEmpId // Link User to Employee
            };
            await _unitOfWork.Users.AddAsync(newUser);

            // 5. Create Employee Record (for HR details)
            var newEmployee = new Employee
            {
                EmpId = nextEmpId, // Use the generated employee ID
                UserId = nextUserId, // Link Employee to User
                FullName = onboardingRequest.FullName,
                PersonalEmail = onboardingRequest.PersonalEmail, // Store personal email in employee record
                OfficeEmail = officeEmail, // Store office email in employee record
                JobRole = onboardingRequest.JobRole,
                HireDate = onboardingRequest.HireDate,
                DeptId = onboardingRequest.DeptId,
                ManagerId = onboardingRequest.ManagerId,
                ShiftId = onboardingRequest.ShiftId,
                Status = "Onboarding", // Initial status
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = hrUserId // HR's UserId who onboarded
            };
            await _unitOfWork.Employees.AddAsync(newEmployee);

            await _unitOfWork.CompleteAsync(); // Save both User and Employee in one transaction

            _logger.LogInformation("New employee onboarded: EmpId {EmpId}, UserId {UserId}, OfficeEmail {OfficeEmail}.",
                nextEmpId, nextUserId, officeEmail);

            // Return generated credentials to HR for display
            response.Response(true, "Employee onboarded and credentials generated successfully.", new EmployeeCredentialsResponseDto
            {
                EmpId = nextEmpId,
                UserId = nextUserId,
                OfficeEmail = officeEmail,
                TemporaryPassword = temporaryPassword,
                FullName = onboardingRequest.FullName,
                PersonalEmail = onboardingRequest.PersonalEmail // Pass personal email back for the email sending step
            });
            return response;
        }

        public async Task<ServiceResponse<bool>> SendEmployeeCredentialsEmailAsync(SendOnboardingEmailRequestDto request)
        {
            var response = new ServiceResponse<bool>();

            // Retrieve login URL from FrontendSettings
            string loginLink = _frontendSettings.LoginUrl;

            string subject = "Welcome to PeopleStrong! Your Account Credentials";
            string body = $"Hello {request.FullName},<br/><br/>" +
                          $"Welcome to PeopleStrong! Your new employee account has been created.<br/><br/>" +
                          $"Here are your credentials:<br/>" +
                          $"<b>Office Email:</b> {request.OfficeEmail}<br/>" +
                          $"<b>Temporary Password:</b> {request.TemporaryPassword}<br/><br/>" +
                          $"Please use these credentials to log in to our system:<br/>" +
                          $"<a href='{loginLink}'>Click here to login</a><br/><br/>" +
                          $"For security reasons, we recommend changing your password after your first login.<br/><br/>" +
                          $"Regards,<br/>The PeopleStrong Team";

            var mailRequest = new MailRequest
            {
                ToEmail = request.PersonalEmail, // Send to the employee's personal email
                Subject = subject,
                Body = body
            };

            var emailSent = await _mailService.SendEmailAsync(mailRequest);

            if (emailSent)
            {
                response.Response(true, "Employee credentials email sent successfully.", true);
                _logger.LogInformation("Employee credentials email sent to personal email {PersonalEmail} for EmpId {EmpId}.",
                    request.PersonalEmail, request.EmpId);
            }
            else
            {
                response.Response(false, "Failed to send employee credentials email.", false);
                _logger.LogError("Failed to send employee credentials email to personal email {PersonalEmail} for EmpId {EmpId}.",
                    request.PersonalEmail, request.EmpId);
            }
            return response;
        }
    }
}
