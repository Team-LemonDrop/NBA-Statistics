using System.IO;
using System.Linq;

using NBAStatistics.Data.Contracts.Base;
using NBAStatistics.Models;
using NBAStatistics.Reports.Contracts;
using NBAStatistics.Reports.Handlers.Contracts;
using NBAStatistics.Reports.Pocos;

namespace NBAStatistics.Reports
{
    public class JsonReportService : IJsonReportService
    {
        private const string SaveDirectory = "../../Files/Json-Reports/";

        private readonly IJsonHandler jsonHandler;

        public JsonReportService(IJsonHandler jsonHandler)
        {
            this.jsonHandler = jsonHandler;
        }

        public void CreatePointsPerGameReport(IRepository<PlayerSeasonPointsPerGame> playerSeasonsDataSource)
        {
            var statistic = playerSeasonsDataSource
                .Find(x => x.SeasonId == seasonId)
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
