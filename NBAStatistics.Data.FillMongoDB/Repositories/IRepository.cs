using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBAStatistics.Data.FillMongoDB.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    //using MongDB.Driver.Demos.Models;
    using Models;

    public interface IRepository<T> where T : IEntity
    {
        Task Add(T value);

        Task<IQueryable<T>> All();

        Task Delete(object id);

        Task Delete(T obj);
    }
}
