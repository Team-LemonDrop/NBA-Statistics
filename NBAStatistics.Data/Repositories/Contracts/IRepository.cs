using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NBAStatistics.Data.Repositories.Contracts
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        TEntity GetById(int id);

        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entry);

        void Add(IEnumerable<TEntity> entries);

        void Update(TEntity entity);

        void Delete(TEntity entity);
    }
}
