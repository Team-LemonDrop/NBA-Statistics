using System.Data;

namespace NBA_Stats.ConnectionProviders.Contracts
{
    public interface IConnectionProvider
    {
        IDbConnection CreateConnection(string connectionString);
    }
}
