using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class RefferalDto
    {
        public Guid ReferralId { get; set; }
        public DateTime ReferralDate { get; set; }
        public string? Status { get; set; }
        public Guid CandidateId { get; set; }
        public Guid EmpId { get; set; }
        public Guid JobOpeningId { get; set; }
    }
}
