using System.Data.Entity;

using NBAStatistics.Repositories.Contracts;

namespace NBAStatistics.Repositories
{
    public class StandingByDayRepository<StandingsByDay> : GenericRepository<StandingsByDay>, IRepository<StandingsByDay>
        where StandingsByDay : class
    {
        public StandingByDayRepository(DbContext context)
            : base(context)
        {
        }
    }
}
