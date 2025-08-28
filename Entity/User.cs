// File: Entity/User.cs (Updated to ensure nullable int types)

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entity
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [StringLength(50)]
        public string? UserName { get; set; }

        [StringLength(100)]
        public string? FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; } = new byte[0];

        [Required]
        public byte[] PasswordSalt { get; set; } = new byte[0];

        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ⬅️ Ensure CreatedBy is nullable if it can ever be NULL in DB for any record
        // If every user *must* have been created by another user, keep it int.
        // But if, for example, the first user has no creator, it should be int?.
        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
        // ⬅️ CRITICAL FIX: UpdatedBy is almost always nullable, so it should be int?
        public int? UpdatedBy { get; set; }

        // ⬅️ CRITICAL FIX: EmpId is a foreign key and is nullable, so it MUST be int?
        [ForeignKey("Employee")]
        public int? EmpId { get; set; }

        // ⬅️ CRITICAL FIX: RoleId is a foreign key and likely nullable, so it MUST be int?
        public int? RoleId { get; set; }
    }
}
