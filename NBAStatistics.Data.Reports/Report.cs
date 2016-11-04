using NBAStatistics.Data.Reports.Contracts;
using NBAStatistics.Data.Repositories.Contracts;

namespace NBAStatistics.Data.Reports
{
    public abstract class Report<T> : IReport<T> where T : class
    {
        public Report(IRepository<T> dataSource)
        {
            this.DataSource = dataSource;
        }

        protected IRepository<T> DataSource { get; }

        public abstract void Generate(string fileName, string savePath);
    }
}
