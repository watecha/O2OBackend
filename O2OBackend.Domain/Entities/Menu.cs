namespace O2OBackend.Domain.Entities
{
    public class Menu
    {
        public int Id { get; set; }
        public int? ParentId { get; set; } // Nullable for root menus
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; } // Icon name for frontend
        public string? Path { get; set; } // Frontend route path
        public string? ComponentPath { get; set; } // Frontend component path (if dynamic loading)
        public int SortOrder { get; set; }
        public string? PermissionCode { get; set; } // Related permission code, null if no specific permission needed
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}