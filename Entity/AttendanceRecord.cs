using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public  class AttendanceRecord
    {
        [Key]
        public Guid RecordId { get; set; } = Guid.NewGuid();

        // The date of the attendance record.
        public DateTime Date { get; set; }

        // The time the employee punched in.
        public DateTime PunchInTime { get; set; }

        // The time the employee punched out. Nullable if the employee hasn't punched out yet.
        public DateTime? PunchOutTime { get; set; }

        // Foreign Key to the Employee table.
        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }

        // Foreign Key to the Shifts table.
        [ForeignKey("Shift")]
        public Guid? ShiftId { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}
