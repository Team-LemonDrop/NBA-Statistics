using NBAStatistics.Data.Contracts.Base;
using NBAStatistics.Models;

namespace NBAStatistics.Reports.Contracts
{
    public interface IXmlReportService
    {
        void CreatePlayedGamesReport(IRepository<StandingsByDay> dataSource);
    }
}
