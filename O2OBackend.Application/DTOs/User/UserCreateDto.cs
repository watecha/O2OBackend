using System.ComponentModel.DataAnnotations;

namespace O2OBackend.Application.DTOs.User
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; } = string.Empty; // 這裡接收明文密碼，後續會在服務層進行雜湊

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string? Email { get; set; }

        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string? FullName { get; set; }
    }
}