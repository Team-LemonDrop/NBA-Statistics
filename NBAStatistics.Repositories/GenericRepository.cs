namespace NBAStatistics.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    using Contracts;

    public class GenericRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly DbContext Context;

        public GenericRepository(DbContext context)
        {
            this.Context = context;
        }

        public void Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            this.Context.Set<TEntity>().Add(entity);
            this.Context.SaveChanges();
        }

        public void Delete(TEntity entity)
        {

        }

        public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            var result = this.Context.Set<TEntity>()
                .Where(predicate)
                .ToList();
            return result;
        }

        public IEnumerable<TEntity> GetAll()
        {
            var result = this.Context.Set<TEntity>().ToList();
            return result;
        }

        public TEntity GetById(int id)
        {
            var result = this.Context.Set<TEntity>().Find(id);
            return result;
        }
    }
}
