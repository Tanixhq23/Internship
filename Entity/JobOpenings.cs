using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class JobOpenings
    {
        [Key]
        public Guid JobOpeningId { get; set; } = Guid.NewGuid();

        // The title of the job opening.
        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        // A description of the job opening.
        public string Description { get; set; }

        // Foreign Key to the Department table.
        [ForeignKey("Department")]
        public Guid DeptId { get; set; }

        // The number of open positions for this job.
        public int Qty { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}
