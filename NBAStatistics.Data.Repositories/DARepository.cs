using System;
using System.Collections.Generic;

using Telerik.OpenAccess;

using NBAStatistics.Data.Repositories.Contracts;
using System.Data.Entity;

namespace NBAStatistics.Data.Repositories
{
    public class DARepository<T> : IRepository<T> where T : class
    {
        private readonly OpenAccessContext dbContext;

        public DARepository(OpenAccessContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext), "OpenAccess database context cannot be null!");
            }

            this.dbContext = dbContext;
        }
        
        public void Add(T value)
        {
            this.dbContext.Add(value);
        }

        public void Delete(T value)
        {
            this.dbContext.Delete(value);
        }

        public IEnumerable<T> GetAll()
        {
            return this.dbContext.GetAll<T>();
        }
    }
}
