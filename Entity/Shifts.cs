using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Shifts
    {
        [Key]
        public Guid ShiftId { get; set; } = Guid.NewGuid();

        // A descriptive name for the shift (e.g., "Morning", "Evening").
        [Required]
        [StringLength(255)]
        public string ShiftName { get; set; }

        // The scheduled start time of the shift.
        public TimeSpan StartTime { get; set; }

        // The scheduled end time of the shift.
        public TimeSpan EndTime { get; set; }

        // Foreign Key to the Employee table, for an employee assigned to this shift.
        // This could represent the default shift for an employee.
        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}
