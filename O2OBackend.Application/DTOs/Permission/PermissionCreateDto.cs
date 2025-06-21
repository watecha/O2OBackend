using System.ComponentModel.DataAnnotations;

namespace O2OBackend.Application.DTOs.Permission
{
    public class PermissionCreateDto
    {
        [Required(ErrorMessage = "Permission code is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Permission code must be between 3 and 100 characters.")]
        public string Code { get; set; } = string.Empty; // e.g., "User.View"

        [Required(ErrorMessage = "Permission name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Permission name must be between 3 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Description cannot exceed 255 characters.")]
        public string? Description { get; set; }
    }
}