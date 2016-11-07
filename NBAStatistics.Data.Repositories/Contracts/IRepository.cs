using System.Collections.Generic;

namespace NBAStatistics.Data.Repositories.Contracts
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();

        void Add(TEntity value);

        void Update(TEntity value);

        void Delete(TEntity value);
    }
}
