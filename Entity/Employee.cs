// File: Entity/Employee.cs (Updated to include Status property)

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // For custom or sequential EmpId
        public int EmpId { get; set; } // Unique identifier for the employee (e.g., EMP-120000)

        [Required]
        [StringLength(255)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string PersonalEmail { get; set; } = string.Empty; // Employee's personal email (for onboarding)

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string OfficeEmail { get; set; } = string.Empty; // The employee's official company email

        [Required]
        [StringLength(100)]
        public string JobRole { get; set; } = string.Empty;

        [Required]
        public DateTime HireDate { get; set; }

        [ForeignKey("User")] // Link to the User entity for authentication
        public int UserId { get; set; } // Link to the User entity

        // ⬅️ Add this new Status property
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Onboarding"; // e.g., "Onboarding", "Active", "On Leave", "Terminated"

        // Optional: Other fields
        public int? DeptId { get; set; } // Foreign Key to Department
        public int? ManagerId { get; set; } // Foreign Key to another Employee (manager)
        public int? ShiftId { get; set; } // Foreign Key to Shift

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedByUserId { get; set; } // HR's User ID who onboarded
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }
    }
}
