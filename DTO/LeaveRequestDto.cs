using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class LeaveRequestDto
    {
        public Guid RequestId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; }
        public Guid LeaveTypeId { get; set; }
        public string Status { get; set; }
        public Guid EmpId { get; set; }
        public Guid? ApproverId { get; set; }

    }
}
