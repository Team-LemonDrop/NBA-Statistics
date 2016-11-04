using System;

using NBAStatistics.Data.Contracts;

namespace NBAStatistics.Data
{
    public class NBAStatisticsData : INBAStatisticsData
    {
        private readonly INBAStatisticsContext dbContext;

        public NBAStatisticsData(INBAStatisticsContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext), "Database context cannot be null!");
            }

            this.dbContext = dbContext;
        }

        public void Commit()
        {
            this.dbContext.SaveChanges();
        }
    }
}
