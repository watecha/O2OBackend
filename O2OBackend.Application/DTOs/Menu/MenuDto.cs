namespace O2OBackend.Application.DTOs.Menu
{
    public class MenuDto
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? Path { get; set; }
        public string? ComponentPath { get; set; }
        public int SortOrder { get; set; }
        public string? PermissionCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}