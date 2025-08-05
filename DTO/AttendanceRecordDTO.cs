using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AttendanceRecordDto
    {
        public Guid RecordId { get; set; }
        public DateTime Date { get; set; }
        public DateTime PunchInTime { get; set; }
        public DateTime? PunchOutTime { get; set; }
        public Guid EmpId { get; set; }
        public Guid? ShiftId { get; set; }
    }
}
