using System;
using System.Data;
using NBA_Stats.ConnectionProviders.Contracts;
using System.Data.SQLite;

namespace NBA_Stats.ConnectionProviders
{
    public class SqliteDbConnectionProvider : IConnectionProvider
    {
        public IDbConnection CreateConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("sqllite");
            }

            var connection = new SQLiteConnection(connectionString);

            return connection;
        }
    }
}