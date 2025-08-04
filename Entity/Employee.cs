using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public class Employee
    {
        [Key]
        public Guid EmpId { get; set; } = Guid.NewGuid();

        // Employee's full name.
        [Required]
        [StringLength(255)]
        public string FullName { get; set; }

        // Employee's email address, which can be used as a unique identifier.
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        // Employee's phone number.
        [Phone]
        public string Phone { get; set; }

        // The job title or role of the employee.
        public string JobRole { get; set; }

        // The date the employee was hired.
        public DateTime HireDate { get; set; }

        // Foreign Key to the Department table.
        [ForeignKey("Department")]
        public Guid? DeptId { get; set; }

        // Foreign Key to the Employee table for the employee's manager.
        [ForeignKey("Manager")]
        public Guid? ManagerId { get; set; }

        // Foreign Key to a hypothetical Shift table (not in provided schema but referenced).
        [ForeignKey("Shift")]
        public Guid? ShiftId { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }

    }
}
