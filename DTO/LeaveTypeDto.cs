using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class LeaveTypeDto
    {
        public Guid LeaveTypeId { get; set; }
        public string TypeName { get; set; }
        public string Description { get; set; }
        public int DefaultDaysPerYear { get; set; }

    }
}
