using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using NBAStatistics.Reports.Handlers.Contracts;
using NBAStatistics.Reports.Pocos;

namespace NBAStatistics.Reports
{
    public class PlayedGamesXmlHandler : IXmlHandler<TeamDailyStandingsPoco>
    {
        public void Serialize(IEnumerable<TeamDailyStandingsPoco> values, string rootPath)
        {
            var fileEncoding = Encoding.GetEncoding("windows-1251");
            using (var xmlWriter = new XmlTextWriter(rootPath, fileEncoding))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.IndentChar = '\t';
                xmlWriter.Indentation = 1;

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("teams");

                foreach (var team in values)
                {
                    xmlWriter.WriteStartElement("team");
                    string teamName = team.TeamName;
                    xmlWriter.WriteAttributeString("name", teamName);

                    var games = team.Games;
                    foreach (var game in games)
                    {
                        WriteGameData(xmlWriter, game.PlayedOn, game.Wins + game.Loses);
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndDocument();
            }
        }

        private void WriteGameData(XmlTextWriter xmlWriter, DateTime date, int games)
        {
            xmlWriter.WriteStartElement("summary");
            xmlWriter.WriteAttributeString("date", date.ToShortDateString());
            xmlWriter.WriteAttributeString("games", games.ToString());
            xmlWriter.WriteEndElement();
        }
    }
}
