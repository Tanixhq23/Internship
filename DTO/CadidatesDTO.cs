using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class CandidatesDto
    {
        public Guid CandidateId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ResumeUrl { get; set; }
    }
}
