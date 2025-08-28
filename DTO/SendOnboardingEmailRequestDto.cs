// File: DTO/SendOnboardingEmailRequestDto.cs

using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class SendOnboardingEmailRequestDto
    {
        [Required(ErrorMessage = "Employee ID is required.")]
        public int EmpId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Office Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid office email address format.")]
        public string OfficeEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Temporary Password is required.")]
        public string TemporaryPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Personal Email for sending is required.")]
        [EmailAddress(ErrorMessage = "Invalid personal email address format.")]
        public string PersonalEmail { get; set; } = string.Empty; // The target personal email to send credentials to

        public string FullName { get; set; } = string.Empty; // Employee's full name for email personalization
    }
}
