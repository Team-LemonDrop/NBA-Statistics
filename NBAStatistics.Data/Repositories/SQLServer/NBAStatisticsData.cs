using System;

using NBAStatistics.Data.Contracts.SQLServer;

namespace NBAStatistics.Data.Repositories.SQLServer
{
    public class NBAStatisticsData : INBAStatisticsData
    {
        private readonly INBAStatisticsDbContext DbContext;

        public NBAStatisticsData(INBAStatisticsDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext), "Database context cannot be null!");
            }

            this.DbContext = dbContext;
        }

        public void Commit()
        {
            this.DbContext.SaveChanges();
        }
    }
}
