using System.Data.Entity;

using NBAStatistics.Repositories.Contracts;

namespace NBAStatistics.Repositories
{
    public class TeamsRepository<Team> : GenericRepository<Team>, IRepository<Team>
        where Team : class
    {
        public TeamsRepository(DbContext context)
            : base(context)
        {
        }
    }
}
