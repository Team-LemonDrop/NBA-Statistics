using System.Data.Entity;

using NBAStatistics.Repositories.Contracts;

namespace NBAStatistics.Repositories
{
    public class PlayersRepository<Player> : GenericRepository<Player>, IRepository<Player>
        where Player : class
    {
        public PlayersRepository(DbContext context)
            : base(context)
        {
        }
    }
}
