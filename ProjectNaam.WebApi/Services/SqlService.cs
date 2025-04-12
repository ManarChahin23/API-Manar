using System.Data;
using Microsoft.Data.SqlClient;

namespace ProjectNaam.WebApi.Services
{
    public class SqlService
    {
        private readonly string _connectionString;
        public SqlService(string connectionString)
        {
            _connectionString = connectionString;
        }
        public virtual IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
