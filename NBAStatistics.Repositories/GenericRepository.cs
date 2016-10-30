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
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "Must provide a db context!");
            }

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
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (this.Context.Entry(entity).State == EntityState.Detached)
            {
                this.Context.Set<TEntity>().Attach(entity);
            }

            this.Context.Set<TEntity>().Remove(entity);
            this.Context.SaveChanges();
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

        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var entry = this.Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.Context.Set<TEntity>().Attach(entity);
                entry.State = EntityState.Modified;
            }

            this.Context.SaveChanges();
        }
    }
}
