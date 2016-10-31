using MySql.Data.MySqlClient;
using NBA_Stats.ConnectionProviders.Contracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBA_Stats.ConnectionProviders
{
    public class MySqlConnectionProvider : IConnectionProvider
    {
        public IDbConnection CreateConnection(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            var connection = new MySqlConnection(connectionString);

            return connection;
        }
    }
}
