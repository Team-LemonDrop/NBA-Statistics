using System.Threading.Tasks;

namespace NBAStatistics.Data.Repositories.Contracts
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
