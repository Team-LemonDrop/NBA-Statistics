using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

using NBAStatistics.Data.Repositories.Contracts;

namespace NBAStatistics.Data.Repositories
{
    public class EfRepository<T> : IEfRepository<T> where T : class
    {
        private readonly DbContext dbContext;
        private readonly IDbSet<T> dbSet;

        public DbContext Context
        {
            get
            {
                return this.dbContext;
            }
        }

        public EfRepository(DbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext), "Entity Framework database context cannot be null!");
            }

            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return this.dbSet;
        }

        public void Add(T value)
        {
            var entry = AttachIfDetached(value);
            entry.State = EntityState.Added;
        }

        public void Delete(T entity)
        {
            var entry = AttachIfDetached(entity);
            entry.State = EntityState.Deleted;
        }

        private DbEntityEntry AttachIfDetached(T entity)
        {
            var entry = this.dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.dbSet.Attach(entity);
            }

            return entry;
        }
    }
}
