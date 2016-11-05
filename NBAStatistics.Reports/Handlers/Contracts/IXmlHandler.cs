using System.Collections.Generic;

namespace NBAStatistics.Reports.Handlers.Contracts
{
    public interface IXmlHandler<T> where T : class
    {
        void Serialize(IEnumerable<T> values, string rootPath);
    }
}
