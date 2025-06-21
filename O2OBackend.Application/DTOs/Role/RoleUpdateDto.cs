using System.ComponentModel.DataAnnotations;

namespace O2OBackend.Application.DTOs.Role
{
    public class RoleUpdateDto
    {
        [Required(ErrorMessage = "Role ID is required.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Role name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}