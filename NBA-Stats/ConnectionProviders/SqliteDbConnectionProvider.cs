using System.Data;
using System.Data.SQLite;

namespace NBA_Stats.ConnectionProviders
{
    public class SqliteDbConnectionProvider : ConnectionProvider
    {
        protected override IDbConnection GetConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString);
        }
    }
}