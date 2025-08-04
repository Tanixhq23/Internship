using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class LeaveRequest
    {
        [Key]
        public Guid RequestId { get; set; } = Guid.NewGuid();

        // The start date of the requested leave.
        public DateTime StartDate { get; set; }

        // The end date of the requested leave.
        public DateTime EndDate { get; set; }

        // The reason provided by the employee for the leave request.
        public string Reason { get; set; }

        // Foreign Key to the LeaveType table.
        [ForeignKey("LeaveType")]
        public Guid LeaveTypeId { get; set; }

        // The current status of the leave request (e.g., Pending, Approved, Denied).
        public string Status { get; set; }

        // Foreign Key to the Employee table (the employee who requested the leave).
        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }

        // Foreign Key to the Employee table (the manager who approves the leave).
        [ForeignKey("Approver")]
        public Guid? ApproverId { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}
