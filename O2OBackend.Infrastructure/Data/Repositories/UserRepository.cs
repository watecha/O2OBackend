using Dapper;
using O2OBackend.Domain.Entities; // 假設 User 實體在這裡
using O2OBackend.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace O2OBackend.Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var sql = "SELECT * FROM Users WHERE Id = @Id AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Id = id });
            }
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var sql = "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QuerySingleOrDefaultAsync<User>(sql, new { Username = username });
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var sql = "SELECT * FROM Users WHERE IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<User>(sql);
            }
        }

        public async Task<int> AddAsync(User user)
        {
            var sql = @"
                INSERT INTO Users (Username, PasswordHash, Email, FullName, CreatedAt, UpdatedAt)
                VALUES (@Username, @PasswordHash, @Email, @FullName, GETDATE(), GETDATE());
                SELECT CAST(SCOPE_IDENTITY() as int);"; // 返回新增記錄的Id
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteScalarAsync<int>(sql, user);
            }
        }

        public async Task<bool> UpdateAsync(User user)
        {
            var sql = @"
                UPDATE Users SET
                    Username = @Username,
                    PasswordHash = @PasswordHash,
                    Email = @Email,
                    FullName = @FullName,
                    UpdatedAt = GETDATE()
                WHERE Id = @Id AND IsActive = 1";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, user);
                return affectedRows > 0;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            // 實作軟刪除
            var sql = "UPDATE Users SET IsActive = 0, UpdatedAt = GETDATE() WHERE Id = @Id";
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
                return affectedRows > 0;
            }
            // 如果是硬刪除，則使用：
            // var sql = "DELETE FROM Users WHERE Id = @Id";
        }
    }
}