using System;
using System.Collections.Generic;

namespace NBAStatistics.Repositories.Contracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity GetById(int id);

        IEnumerable<TEntity> GetAll();

        IEnumerable<TEntity> Find(Func<TEntity, bool> predicate);

        void Add(TEntity entity);

        void Delete(TEntity entity);
    }
}
