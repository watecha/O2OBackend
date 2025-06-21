namespace O2OBackend.Domain.Entities
{
    public class Permission
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty; // Unique code, e.g., "User.View"
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}