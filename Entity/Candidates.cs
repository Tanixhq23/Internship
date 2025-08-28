using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Candidates
    {
        [Key]
        public Guid CandidateId { get; set; } = Guid.NewGuid();

        // The full name of the candidate.
        [Required]
        [StringLength(255)]
        public string? FullName { get; set; }

        // The candidate's email address.
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        // A URL to the candidate's resume.
        public string? ResumeUrl { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}
