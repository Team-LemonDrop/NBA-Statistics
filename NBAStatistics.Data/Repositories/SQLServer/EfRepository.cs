using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

using NBAStatistics.Data.Contracts.Base;
using NBAStatistics.Data.Contracts.SQLServer;

namespace NBAStatistics.Data.Repositories.SQLServer
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly INBAStatisticsDbContext DbContext;
        private readonly IDbSet<TEntity> DbSet;

        public EfRepository(INBAStatisticsDbContext dbContext)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext), "Database context cannot be null!");
            }

            this.DbContext = dbContext;
            this.DbSet = this.DbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> All
        {
            get { return this.DbSet; }
        }


        public TEntity GetById(object id)
        {
            return this.DbSet.Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return this.GetAll(null);
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> filterExpression)
        {
            return this.GetAll<object>(filterExpression, null);
        }

        public IEnumerable<TEntity> GetAll<T1>(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, T1>> sortExpression)
        {
            return this.GetAll<T1, TEntity>(filterExpression, sortExpression, null);
        }

        public IEnumerable<T2> GetAll<T1, T2>(Expression<Func<TEntity, bool>> filterExpression, Expression<Func<TEntity, T1>> sortExpression, Expression<Func<TEntity, T2>> selectExpression)
        {
            IQueryable<TEntity> result = this.DbSet;

            if (filterExpression != null)
            {
                result = result.Where(filterExpression);
            }

            if (sortExpression != null)
            {
                result = result.OrderBy(sortExpression);
            }

            if (selectExpression != null)
            {
                return result.Select(selectExpression).ToList();
            }
            else
            {
                return result.OfType<T2>().ToList();
            }
        }

        //public IStudentSystemDbContext Context { get; set; }

        // might be useful in unit testing but I'm not sure now
        //protected IDbSet<T> DbSet { get; set; }

        public void Add(TEntity entity)
        {
            var entry = AttachIfDetached(entity);
            entry.State = EntityState.Added;
        }

        public void Update(TEntity entity)
        {
            var entry = AttachIfDetached(entity);
            entry.State = EntityState.Modified;
        }

        public void Delete(TEntity entity)
        {
            var entry = AttachIfDetached(entity);
            entry.State = EntityState.Deleted;
        }

        private DbEntityEntry AttachIfDetached(TEntity entity)
        {
            var entry = this.DbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                this.DbSet.Attach(entity);
            }

            return entry;
        }
    }
}
