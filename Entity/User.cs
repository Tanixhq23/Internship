using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();

        // The user's unique username.
        [Required]
        [StringLength(255)]
        public string UserName { get; set; }

        public string FullName { get; set; } = string.Empty;


        // The hashed password.
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public byte[] PasswordHash { get; set; }
        [Required]
        public byte[] PasswordSalt { get; set; }

        // Foreign Key to the Role table.
        [ForeignKey("Role")]
        public Guid? RoleId { get; set; }

        // Foreign Key linking the user account to a specific employee.
        [ForeignKey("Employee")]
        public Guid? EmpId { get; set; }

        // Audit fields for tracking creation and updates.
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}
