namespace O2OBackend.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string? Email { get; set; } // 可以為 null
        public string? FullName { get; set; } // 可以為 null
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // 您可以在這裡添加導航屬性，但對於 Dapper，它們通常是「虛擬的」或在服務層通過多個查詢來實現
        // public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}