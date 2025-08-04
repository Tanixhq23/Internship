using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Refferals
    {
        [Key]
        public Guid ReferralId { get; set; } = Guid.NewGuid();

        // The date the referral was submitted.
        public DateTime ReferralDate { get; set; }

        // The current status of the referral (e.g., Submitted, Interviewing, Hired).
        public string Status { get; set; }

        // Foreign Key to the Candidate table.
        [ForeignKey("Candidate")]
        public Guid CandidateId { get; set; }

        // Foreign Key to the Employee table (the referrer).
        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }

        // Foreign Key to the JobOpening table.
        [ForeignKey("JobOpening")]
        public Guid JobOpeningId { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}
