using System.Threading.Tasks;

namespace NBAStatistics.DataImporters.Contracts
{
    public interface IDataImporter
    {
        Task Import();
    }
}
