using System.Collections.Generic;
using System.Linq;

using NBAStatistics.Data.Repositories.Contracts;
using NBAStatistics.Models;
using NBAStatistics.Reports.Contracts;
using NBAStatistics.Reports.Pocos;

namespace NBAStatistics.Reports
{
    public class XmlReportService : IXmlReportService
    {
        private const string SaveDirectory = "../../../Files/Xml-Reports/";

        private readonly PlayedGamesXmlHandler xmlHandler;

        public XmlReportService(PlayedGamesXmlHandler xmlHandler)
        {
            this.xmlHandler = xmlHandler;
        }

        public void CreatePlayedGamesReport(IEfRepository<StandingsByDay> dataSource)
        {
            var dailyStandings = dataSource
                .GetAll()
                .GroupBy(x => x.Team.Name);

            var standingsPocos = new List<TeamDailyStandingsPoco>();
            foreach (var info in dailyStandings)
            {
                var standing = new TeamDailyStandingsPoco { TeamName = info.Key };
                var gamesPlayed = new List<GamePoco>();

                foreach (var game in info)
                {
                    var gameInfo = new GamePoco
                    {
                        Wins = game.Wins,
                        Loses = game.Loses,
                        PlayedOn = game.Date
                    };

                    gamesPlayed.Add(gameInfo);
                }

                standing.Games = gamesPlayed.OrderBy(x => x.PlayedOn);

                standingsPocos.Add(standing);
            }

            var dailyStandingsSortedByTeamName = standingsPocos.OrderBy(x => x.TeamName);
            this.xmlHandler.Serialize(dailyStandingsSortedByTeamName, SaveDirectory + "daily-standings.xml");
        }
    }
}
