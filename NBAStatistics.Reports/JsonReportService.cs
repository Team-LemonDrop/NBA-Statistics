using System.IO;
using System.Linq;

using NBAStatistics.Data.Contracts.Base;
using NBAStatistics.Models;
using NBAStatistics.Reports.Handlers.Contracts;
using NBAStatistics.Reports.Pocos;

namespace NBAStatistics.Reports
{
    public class JsonReportService
    {
        private const string SaveDirectory = "../../Files/Json-Reports/";

        private readonly IJsonHandler jsonHandler;

        public JsonReportService(IJsonHandler jsonHandler)
        {
            this.jsonHandler = jsonHandler;
        }

        public void CreatePointsPerGameReport(IRepository<PlayersSeasons> playedSeasonsDataSource, int seasonStartYear, int seasonEndYear)
        {
            var statistic = playedSeasonsDataSource
                .Find(x => x.Season.StartYear == seasonStartYear &&
                x.Season.EndYear == seasonEndYear)
                .Select(x => new PointsPerGameReportPoco
                {
                    PlayerId = x.PlayerId,
                    PlayerName = x.Player.FirstLastName,
                    TeamName = x.Player.Team.Name,
                    PointPerGame = x.PointsPerGame
                });

            foreach (var info in statistic)
            {
                string json = this.jsonHandler.Serialize(info);
                string fileName = info.PlayerId + ".json";
                File.WriteAllText(SaveDirectory + fileName, json);
            }
        }
    }
}
