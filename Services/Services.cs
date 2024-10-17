using Dapper;
using LogManagement.Data;
using LogManagement.Services.Interfaces;
using MySqlConnector;

namespace LogManagement.Services
{
    public class Services : IServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _connectionString;

        public Services(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _context = context;
        }

        public int GetAutoIncrement(string table)
        {
            int sequencial = 0;
            using (var sqlConnection = new MySqlConnection(_connectionString))
            {
                string sql = $"SELECT AUTO_INCREMENT FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = '{table}'";
                sequencial = sqlConnection.QuerySingleOrDefault<int>(sql);
            }
            return sequencial;
        }
    }
}