using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class LifeCycleTask
    {
        [Key]
        public Guid TaskId { get; set; } = Guid.NewGuid();

        // The name of the task (e.g., "Set up workstation", "Return company laptop").
        [Required]
        [StringLength(255)]
        public string TaskName { get; set; }

        // A detailed description of the task.
        public string Description { get; set; }

        // The due date for the task.
        public DateTime DueDate { get; set; }

        // A flag to indicate if the task has been completed.
        public bool IsCompleted { get; set; }

        // The type of task, either "Onboard" or "Offboard".
        public string Type { get; set; }

        // Foreign Key to the Employee table to link the task to a specific employee.
        [ForeignKey("Employee")]
        public Guid EmpId { get; set; }

        // The date the task was completed. Nullable if not yet completed.
        public DateTime? CompletionDate { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}
