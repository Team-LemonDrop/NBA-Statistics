using NBAStatistics.Data.Repositories.Contracts;
using NBAStatistics.Models;

namespace NBAStatistics.Reports.Contracts
{
    public interface IJsonReportService
    {
        void CreatePointsPerGameReport(IRepository<PlayersSeasons> playedSeasonsDataSource, int seasonStartYear, int seasonEndYear);
    }
}
