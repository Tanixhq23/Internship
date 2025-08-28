// File: DTO/EmployeeCredentialsResponseDto.cs

using System;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class EmployeeCredentialsResponseDto
    {
        public int EmpId { get; set; }
        public int UserId { get; set; }

        [EmailAddress(ErrorMessage = "Invalid office email address format.")]
        public string OfficeEmail { get; set; } = string.Empty; // The generated office email for the employee

        public string TemporaryPassword { get; set; } = string.Empty; // The auto-generated temporary password
        public string FullName { get; set; } = string.Empty; // Employee's full name for context
        public string PersonalEmail { get; set; } = string.Empty; // The personal email provided during onboarding
    }
}
