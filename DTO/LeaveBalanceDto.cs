using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class LeaveBalanceDto
    {
        public Guid BalanceId { get; set; }
        public int UsedDays { get; set; }
        public int RemainingDays { get; set; }
        public Guid EmpId { get; set; }
        public Guid LeaveTypeId { get; set; }

        
    }
}
