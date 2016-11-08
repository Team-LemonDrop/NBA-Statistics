using System;

using NBAStatistics.Data.Repositories.Contracts;
using NBAStatistics.DataImporters.Contracts;

namespace NBAStatistics.DataImporters
{
    public abstract class DataImporter<T> : IDataImporter<T> where T : class
    {
        public DataImporter(IRepository<T> source, IRepository<T> storage)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Cannot use null for data source!");
            }

            if (storage == null)
            {
                throw new ArgumentNullException(nameof(source), "Cannot use null for data storage!");
            }

            this.Source = source;
            this.Storage = storage;
        }

        protected IRepository<T> Source { get; set; }

        protected IRepository<T> Storage { get; set; }

        public abstract void Import();
    }
}
