using Dapper;
using O2OBackend.Domain.Entities;
using O2OBackend.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using O2OBackend.Infrastructure.Data; // 確保引用 DapperContext

namespace O2OBackend.Infrastructure.Data.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DapperContext _context;

        public RoleRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Roles WHERE Id = @Id AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Role>(sql, new { Id = id });
            }
        }

        public async Task<Role?> GetByNameAsync(string name)
        {
            var sql = "SELECT * FROM Roles WHERE Name = @Name AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Role>(sql, new { Name = name });
            }
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            var sql = "SELECT * FROM Roles WHERE IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Role>(sql);
            }
        }

        public async Task<int> AddAsync(Role role)
        {
            var sql = @"
                INSERT INTO Roles (Name, Description, CreatedAt, UpdatedAt)
                VALUES (@Name, @Description, GETDATE(), GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);";
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(sql, role);
            }
        }

        public async Task<bool> UpdateAsync(Role role)
        {
            var sql = @"
                UPDATE Roles SET
                    Name = @Name,
                    Description = @Description,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, role);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "UPDATE Roles SET IsActive = 0, UpdatedAt = GETDATE() WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            var sql = @"
                IF NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId)
                BEGIN
                    INSERT INTO UserRoles (UserId, RoleId, CreatedAt)
                    VALUES (@UserId, @RoleId, GETDATE());
                END;";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, new { UserId = userId, RoleId = roleId });
                return affectedRows > 0;
            }
        }

        public async Task<bool> RemoveRoleFromUserAsync(int userId, int roleId)
        {
            var sql = "DELETE FROM UserRoles WHERE UserId = @UserId AND RoleId = @RoleId";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, new { UserId = userId, RoleId = roleId });
                return affectedRows > 0;
            }
        }

        public async Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            var sql = @"
                IF NOT EXISTS (SELECT 1 FROM RolePermissions WHERE RoleId = @RoleId AND PermissionId = @PermissionId)
                BEGIN
                    INSERT INTO RolePermissions (RoleId, PermissionId, CreatedAt)
                    VALUES (@RoleId, @PermissionId, GETDATE());
                END;";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, new { RoleId = roleId, PermissionId = permissionId });
                return affectedRows > 0;
            }
        }

        public async Task<bool> RemovePermissionFromRoleAsync(int roleId, int permissionId)
        {
            var sql = "DELETE FROM RolePermissions WHERE RoleId = @RoleId AND PermissionId = @PermissionId";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, new { RoleId = roleId, PermissionId = permissionId });
                return affectedRows > 0;
            }
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId)
        {
            var sql = @"
                SELECT p.*
                FROM Permissions p
                JOIN RolePermissions rp ON p.Id = rp.PermissionId
                WHERE rp.RoleId = @RoleId AND p.IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Permission>(sql, new { RoleId = roleId });
            }
        }

        public async Task<IEnumerable<User>> GetUsersByRoleIdAsync(int roleId)
        {
            var sql = @"
                SELECT u.*
                FROM Users u
                JOIN UserRoles ur ON u.Id = ur.UserId
                WHERE ur.RoleId = @RoleId AND u.IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<User>(sql, new { RoleId = roleId });
            }
        }
    }
}