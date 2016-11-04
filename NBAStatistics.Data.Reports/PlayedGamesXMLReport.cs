using System;
using System.Linq;
using System.Text;
using System.Xml;
using NBAStatistics.Data.Repositories.Contracts;
using NBAStatistics.Models;

namespace NBAStatistics.Data.Reports
{
    public class PlayedGamesXMLReport : Report<StandingsByDay>
    {
        public PlayedGamesXMLReport(IRepository<StandingsByDay> dataSource) : base(dataSource)
        {
        }

        public override void Generate(string fileName, string savePath)
        {
            var fileEncoding = Encoding.GetEncoding("windows-1251");
            using (var xmlWriter = new XmlTextWriter(fileName, fileEncoding))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.IndentChar = '\t';
                xmlWriter.Indentation = 1;

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("teams");

                var gamesStatistics = this.DataSource.GetAll().GroupBy(x => x.Team.Name);
                foreach (var team in gamesStatistics)
                {
                    xmlWriter.WriteStartElement("team");
                    string teamName = team.Key;
                    xmlWriter.WriteAttributeString("name", teamName);

                    foreach (var game in team)
                    {
                        WriteGameData(xmlWriter, game.Date, game.SuccessRate);
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndDocument();
            }
        }

        private void WriteGameData(XmlTextWriter xmlWriter, DateTime date, double rating)
        {
            xmlWriter.WriteStartElement("game");
            xmlWriter.WriteAttributeString("date", date.ToShortDateString());
            xmlWriter.WriteAttributeString("success-rate", rating.ToString());
            xmlWriter.WriteEndElement();
        }
    }
}
