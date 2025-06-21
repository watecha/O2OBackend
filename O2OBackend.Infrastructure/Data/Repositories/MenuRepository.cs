using Dapper;
using O2OBackend.Domain.Entities;
using O2OBackend.Domain.Repositories;
using System.Collections.Generic;
using System.Linq; // for Distinct()
using System.Threading.Tasks;
using O2OBackend.Infrastructure.Data;

namespace O2OBackend.Infrastructure.Data.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly DapperContext _context;

        public MenuRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<Menu?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Menus WHERE Id = @Id AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<Menu>(sql, new { Id = id });
            }
        }

        public async Task<IEnumerable<Menu>> GetAllAsync()
        {
            var sql = "SELECT * FROM Menus WHERE IsActive = 1 ORDER BY SortOrder ASC";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<Menu>(sql);
            }
        }

        public async Task<int> AddAsync(Menu menu)
        {
            var sql = @"
                INSERT INTO Menus (ParentId, Name, Icon, Path, ComponentPath, SortOrder, PermissionCode, CreatedAt, UpdatedAt)
                VALUES (@ParentId, @Name, @Icon, @Path, @ComponentPath, @SortOrder, @PermissionCode, GETDATE(), GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);";
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(sql, menu);
            }
        }

        public async Task<bool> UpdateAsync(Menu menu)
        {
            var sql = @"
                UPDATE Menus SET
                    ParentId = @ParentId,
                    Name = @Name,
                    Icon = @Icon,
                    Path = @Path,
                    ComponentPath = @ComponentPath,
                    SortOrder = @SortOrder,
                    PermissionCode = @PermissionCode,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, menu);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // 軟刪除，同時也考慮軟刪除所有子選單 (可選，根據需求決定是否級聯軟刪除)
            var sql = @"
                WITH MenuHierarchy AS (
                    SELECT Id FROM Menus WHERE Id = @Id
                    UNION ALL
                    SELECT m.Id FROM Menus m JOIN MenuHierarchy mh ON m.ParentId = mh.Id
                )
                UPDATE Menus SET IsActive = 0, UpdatedAt = GETDATE() WHERE Id IN (SELECT Id FROM MenuHierarchy);";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
        }

        public async Task<IEnumerable<Menu>> GetAccessibleMenusByPermissionCodesAsync(IEnumerable<string> permissionCodes)
        {
            // 如果沒有傳入任何權限碼，則只返回沒有 PermissionCode 的選單
            if (permissionCodes == null || !permissionCodes.Any())
            {
                var sqlNoPerm = "SELECT * FROM Menus WHERE PermissionCode IS NULL AND IsActive = 1 ORDER BY SortOrder ASC";
                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<Menu>(sqlNoPerm);
                }
            }

            // 查詢所有選單，包括需要特定權限的選單和不需要權限的選單
            // 使用 DISTINCT 避免重複
            var sql = @"
                SELECT DISTINCT m.*
                FROM Menus m
                WHERE m.IsActive = 1
                AND (m.PermissionCode IS NULL OR m.PermissionCode IN @PermissionCodes)
                ORDER BY m.SortOrder ASC;";
            using (var connection = _context.CreateConnection())
            {
                // Dapper 允許將 IEnumerable<string> 直接作為參數傳遞給 IN 子句
                return await connection.QueryAsync<Menu>(sql, new { PermissionCodes = permissionCodes });
            }
        }
    }
}