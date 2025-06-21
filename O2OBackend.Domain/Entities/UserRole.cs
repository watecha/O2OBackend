namespace O2OBackend.Domain.Entities
{
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; }

        // 您可能需要導航屬性，但 Dapper 通常不會自動加載它們
        // public User? User { get; set; }
        // public Role? Role { get; set; }
    }
}