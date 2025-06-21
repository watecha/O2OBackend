using Dapper;
using O2OBackend.Domain.Entities;
using O2OBackend.Domain.Repositories;
using System.Collections.Generic;
using System.Linq; // for Distinct()
using System.Threading.Tasks;
using O2OBackend.Infrastructure.Data;

namespace O2OBackend.Infrastructure.Data.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DapperContext _context;

        public PermissionRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Permission?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Permissions WHERE Id = @Id AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Permission>(sql, new { Id = id });
            }
        }

        public async Task<Permission?> GetByCodeAsync(string code)
        {
            var sql = "SELECT * FROM Permissions WHERE Code = @Code AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Permission>(sql, new { Code = code });
            }
        }

        public async Task<IEnumerable<Permission>> GetAllAsync()
        {
            var sql = "SELECT * FROM Permissions WHERE IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Permission>(sql);
            }
        }

        public async Task<int> AddAsync(Permission permission)
        {
            var sql = @"
                INSERT INTO Permissions (Code, Name, Description, CreatedAt, UpdatedAt)
                VALUES (@Code, @Name, @Description, GETDATE(), GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);";
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(sql, permission);
            }
        }

        public async Task<bool> UpdateAsync(Permission permission)
        {
            var sql = @"
                UPDATE Permissions SET
                    Code = @Code,
                    Name = @Name,
                    Description = @Description,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, permission);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sql = "UPDATE Permissions SET IsActive = 0, UpdatedAt = GETDATE() WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByUserIdAsync(int userId)
        {
            var sql = @"
                SELECT DISTINCT p.*
                FROM Permissions p
                JOIN RolePermissions rp ON p.Id = rp.PermissionId
                JOIN UserRoles ur ON rp.RoleId = ur.RoleId
                WHERE ur.UserId = @UserId AND p.IsActive = 1;";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Permission>(sql, new { UserId = userId });
            }
        }
    }
}