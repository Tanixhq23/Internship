using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class AttendanceRecordDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }      
        public DateTime PunchInTime { get; set; }
        public DateTime? PunchOutTime { get; set; }
        public string? Status { get; set; }
        public string? PunchInLocation { get; set; }
        public string? PunchOutLocation { get; set; }
        public TimeSpan? TotalHours { get; set; }        
    }
}
