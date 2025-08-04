using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class LeaveType
    {
        [Key]
        public Guid LeaveTypeId { get; set; } = Guid.NewGuid();

        // The name of the leave type (e.g., "Vacation", "Sick Leave").
        [Required]
        [StringLength(255)]
        public string TypeName { get; set; }

        // A description of the leave type.
        public string Description { get; set; }

        // The number of days an employee is entitled to for this leave type per year.
        public int DefaultDaysPerYear { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}
