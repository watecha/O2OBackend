using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient; // 使用 Microsoft.Data.SqlClient

namespace O2OBackend.Infrastructure.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        // 提供一個方法來創建新的資料庫連接
        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}