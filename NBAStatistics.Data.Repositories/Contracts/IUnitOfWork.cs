using System;
using System.Threading.Tasks;

namespace NBAStatistics.Data.Repositories.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        Task CommitAsync();
    }
}
