using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class LeaveBalance
    {
        [Key]
        public Guid BalanceId { get; set; } = Guid.NewGuid();

        // The number of leave days used by the employee.
        public int UsedDays { get; set; }

        // The number of remaining leave days for the employee.
        public int RemainingDays { get; set; }

        // Foreign Key to the Employee table.
        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }

        // Foreign Key to the LeaveType table.
        [ForeignKey("LeaveType")]
        public Guid LeaveTypeId { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}
