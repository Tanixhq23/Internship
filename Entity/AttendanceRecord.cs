// File: Entity/AttendanceRecord.cs - Verified and Corrected

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class AttendanceRecord
    {
        [Key]
        public int Id { get; set; } // Unique identifier for the attendance record

        [Required]
        public int UserId { get; set; } // ⬅️ THIS IS THE CRITICAL MISSING COLUMN
        // public virtual User User { get; set; } // Navigation property to the User entity

        [Required]
        public DateTime PunchInTime { get; set; } // Timestamp when the user punched in

        public DateTime? PunchOutTime { get; set; } // Nullable timestamp when the user punched out

        [StringLength(50)]
        public string? PunchInLocation { get; set; } // Optional: Store location info (e.g., IP address, GPS coords)

        [StringLength(50)]
        public string? PunchOutLocation { get; set; } // Optional: Store location info for punch out

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "PunchedIn"; // Default status upon creation'

        // Foreign Key to the Employee table. If EmpId is intended as an int FK, ensure it matches your Employee.Id type.
        [ForeignKey("Employee")]
        public int EmpId { get; set; } // ⬅️ Assuming this should be int for consistency

        // Foreign Key to the Shifts table. If ShiftId is intended as an int FK, ensure it matches your Shift.Id type.
        [ForeignKey("Shift")]
        public int ShiftId { get; set; } // ⬅️ Assuming this should be int for consistency

        // Audit fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int CreatedByUserId { get; set; } // Who performed the punch-in (if different from UserId)
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }
    }
}
