using System.ComponentModel.DataAnnotations;

namespace O2OBackend.Application.DTOs.User
{
    public class UserUpdateDto
    {
        [Required(ErrorMessage = "User ID is required.")]
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; } = string.Empty;

        // 密碼更新應該是獨立的，或在這裡標記為可選
        // [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        // public string? Password { get; set; } // 如果允許部分更新密碼

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string? FullName { get; set; }

        public bool IsActive { get; set; }
    }
}