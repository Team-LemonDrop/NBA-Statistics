using System.Data.Entity;

namespace NBAStatistics.Data.Repositories.Contracts
{
    public interface IEfRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        DbContext Context { get; }
    }
}
