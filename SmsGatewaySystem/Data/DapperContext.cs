using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace SmsGatewaySystem.Data
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _connectionStringSms;
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection");
            _connectionStringSms = _configuration.GetConnectionString("SmsConnectionString");
        }

        public IDbConnection CreateConnection()
            => new OracleConnection(_connectionString);

        public IDbConnection CreateSmsDbConnection()
            => new NpgsqlConnection(_connectionStringSms);

    }
}
