using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();

        // The user's unique username.
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string? UserName { get; set; }

        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string? FullName { get; set; } = string.Empty;

        // The user's email address, with validation.
        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; } = string.Empty;

        // The hashed password and salt.
        [Required(ErrorMessage = "Password hash is required.")]
        public byte[]? PasswordHash { get; set; }

        [Required(ErrorMessage = "Password salt is required.")]
        public byte[]? PasswordSalt { get; set; }

        // --- New fields for Password Reset functionality ---
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        // ---------------------------------------------------

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