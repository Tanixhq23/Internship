using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class DailyAttendanceSummaryDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan? TotalDuration { get; set; } // Sum of all punched-in durations for the day
        public List<AttendanceRecordDto> Records { get; set; } = new List<AttendanceRecordDto>();
        public bool HasMissingPunchOut { get; set; } // Indicates if there's an open punch-in
    }
}
