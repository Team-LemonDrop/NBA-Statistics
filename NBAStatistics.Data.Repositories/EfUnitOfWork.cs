using System;
using System.Data.Entity;
using System.Threading.Tasks;

using NBAStatistics.Data.Repositories.Contracts;

namespace NBAStatistics.Data.Repositories
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private readonly DbContext context;

        public EfUnitOfWork(DbContext context)
        {
            this.context = context;
        }

        public Task CommitAsync()
        {
            return this.context.SaveChangesAsync();
        }

        public void Dispose()
        {
        }
    }
}
