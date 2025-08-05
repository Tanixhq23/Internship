using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class LifeCycleTaskDto
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string Type { get; set; } // "Onboard" or "Offboard"
        public Guid EmpId { get; set; }
        public DateTime? CompletionDate { get; set; }
    }
}
