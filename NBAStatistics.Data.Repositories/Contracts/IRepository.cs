using System.Collections.Generic;
using System.Data.Entity;

namespace NBAStatistics.Data.Repositories.Contracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();

        void Add(TEntity value);

        void Delete(TEntity value);
    }
}
