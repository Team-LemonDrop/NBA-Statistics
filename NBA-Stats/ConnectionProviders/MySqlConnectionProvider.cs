using System.Data;
using MySql.Data.MySqlClient;

namespace NBA_Stats.ConnectionProviders
{
    public class MySqlConnectionProvider : ConnectionProvider
    {
        protected override IDbConnection GetConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
