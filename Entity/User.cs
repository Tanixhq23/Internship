using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string? UserName { get; set; }

        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string? FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password hash is required.")]
        public byte[]? PasswordHash { get; set; }

        [Required(ErrorMessage = "Password salt is required.")]
        public byte[]? PasswordSalt { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        [ForeignKey("Role")]
        public Guid? RoleId { get; set; }
        [ForeignKey("Employee")]
        public Guid? EmpId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public Guid? UpdatedBy { get; set; }
    }
}