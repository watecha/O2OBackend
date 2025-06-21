using System.ComponentModel.DataAnnotations;

namespace O2OBackend.Application.DTOs.Menu
{
    public class MenuCreateDto
    {
        public int? ParentId { get; set; }

        [Required(ErrorMessage = "Menu name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Menu name must be between 2 and 50 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Icon name cannot exceed 50 characters.")]
        public string? Icon { get; set; }

        [StringLength(255, ErrorMessage = "Path cannot exceed 255 characters.")]
        public string? Path { get; set; }

        [StringLength(255, ErrorMessage = "Component path cannot exceed 255 characters.")]
        public string? ComponentPath { get; set; }

        [Required(ErrorMessage = "Sort order is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Sort order must be a non-negative integer.")]
        public int SortOrder { get; set; }

        [StringLength(100, ErrorMessage = "Permission code cannot exceed 100 characters.")]
        public string? PermissionCode { get; set; } // Related permission code, null if no specific permission needed
    }
}