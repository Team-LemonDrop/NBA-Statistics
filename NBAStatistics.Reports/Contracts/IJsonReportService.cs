using NBAStatistics.Data.Contracts.Base;
using NBAStatistics.Models;

namespace NBAStatistics.Reports.Contracts
{
    public interface IJsonReportService
    {
        void CreatePointsPerGameReport(IRepository<PlayerSeasonPointsPerGame> playedSeasonsDataSource);
    }
}
