namespace O2OBackend.Domain.Entities
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime CreatedAt { get; set; }

        // 您可能需要導航屬性
        // public Role? Role { get; set; }
        // public Permission? Permission { get; set; }
    }
}