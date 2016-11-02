namespace NBAStatistics.Data.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Contracts;

    public class GenericRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly INBAStatisticsContext dbContext;

        public GenericRepository(INBAStatisticsContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext), "Database context cannot be null!");
            }

            this.dbContext = dbContext;
        }

        public void Add(IEnumerable<TEntity> entries)
        {
            throw new NotImplementedException();
        }

        public void Add(TEntity entry)
        {
            throw new NotImplementedException();
        }

        public void Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public TEntity GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}
