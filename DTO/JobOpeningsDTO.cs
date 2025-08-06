using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class JobOpeningsDto
    {
        public Guid JobOpeningId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Guid DeptId { get; set; }
        public int Qty { get; set; }
    }
}
