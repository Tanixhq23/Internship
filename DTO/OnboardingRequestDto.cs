// File: DTO/OnboardingRequestDto.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class OnboardingRequestDto
    {
        [Required(ErrorMessage = "Full Name is required.")]
        [StringLength(255, ErrorMessage = "Full Name cannot exceed 255 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Personal Email is required for communication.")]
        [EmailAddress(ErrorMessage = "Invalid personal email address format.")]
        [StringLength(255, ErrorMessage = "Personal Email cannot exceed 255 characters.")]
        public string PersonalEmail { get; set; } = string.Empty; // Employee's personal email for onboarding communication

        [Required(ErrorMessage = "Job Role is required.")]
        [StringLength(100, ErrorMessage = "Job Role cannot exceed 100 characters.")]
        public string JobRole { get; set; } = string.Empty;

        [Required(ErrorMessage = "Hire Date is required.")]
        public DateTime HireDate { get; set; }

        // Optional: Department ID, Manager ID, Shift ID (if these are set at onboarding)
        public int? DeptId { get; set; }
        public int? ManagerId { get; set; }
        public int? ShiftId { get; set; }
    }
}
