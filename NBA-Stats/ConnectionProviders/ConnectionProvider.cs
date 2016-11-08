using System;
using System.Data;

using NBA_Stats.ConnectionProviders.Contracts;

namespace NBA_Stats.ConnectionProviders
{
    public abstract class ConnectionProvider : IConnectionProvider
    {
        private const string NoConnectionString = "";

        public IDbConnection CreateConnection(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString), "Cannot create a database connection when the connection string is null!");
            }

            if (connectionString == NoConnectionString)
            {
                throw new ArgumentException("Cannot create a database connection when connection string is empty!", nameof(connectionString));
            }

            IDbConnection dbConnection = this.GetConnection(connectionString);
            return dbConnection;
        }

        protected abstract IDbConnection GetConnection(string connectionString);
    }
}
