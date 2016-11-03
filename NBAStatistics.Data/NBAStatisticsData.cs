using System;

using NBAStatistics.Data.Contracts;

namespace NBAStatistics.Data
{
    public class NBAStatisticsData : INBAStatisticsData
    {
        private readonly INBAStatisticsContext DbContext;

        public NBAStatisticsData(INBAStatisticsContext dbContext)
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
