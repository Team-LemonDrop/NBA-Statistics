using System.Data;
using System.Data.SqlClient;

namespace NBA_Stats.ConnectionProviders
{
    public class SqlServerConectionProvider : ConnectionProvider
    {
        protected override IDbConnection GetConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
