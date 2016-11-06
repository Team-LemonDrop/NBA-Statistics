using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;
using NBAStatistics.Data.Repositories.SQLServer;
using NBAStatistics.Data.Contracts.SQLServer;
using NBAStatistics.Models;
using System.Globalization;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace NBAStatistics.Data.ImportIntoSqlServer
{
    public class ImportIntoSqlServer
    {
        const string User = "miro";
        const string Pass = "1qazcde3";
        const string DbHost = "ds029565.mlab.com";
        const int DbPort = 29565;
        const string DbName = "appharbor_5cwg75nh";

        public static async Task Import()
        {
            var credentials = MongoCredential.CreateCredential(DbName, User, Pass);
            var settings = new MongoClientSettings
            {
                Server = new MongoServerAddress(DbHost, DbPort),
                Credentials = new List<MongoCredential> { credentials }
            };

            var client = new MongoClient(settings);
            var mongoDb = client.GetDatabase(DbName);

            var dbContext = new NBAStatisticsDbContext();

            await Task.Run(async () =>
            {
                var mongoSeasons = mongoDb.GetCollection<NBAStatistics.Data.FillMongoDB.Models.Season>("Seasons").AsQueryable().ToList();

                // Load all teams from the database into the dbContext 
                dbContext.Teams.Load();

                foreach (var season in mongoSeasons)
                {
                    foreach (var team in season.Teams)
                    {
                        //var teamInDb = dbContext.Teams.Local
                        //    .FirstOrDefault(t => t.TeamId == team.TeamId);

                        var teamInDb = dbContext.Teams.Local
                            .SingleOrDefault(t => t.TeamId == team.TeamId); // runs in memory

                        if (teamInDb == null)
                        {
                            dbContext.Teams.Add(new Team
                            {
                                TeamId = team.TeamId,
                                Name = team.Name
                            });
                        }
                    }
                }

                await dbContext.SaveChangesAsync();

                var mongoPlayers = mongoDb.GetCollection<NBAStatistics.Data.FillMongoDB.Models.Player>("Players").AsQueryable().ToList();

                // Load all players from the database into the dbContext 
                dbContext.Players.Load();
                dbContext.Teams.Load();

                foreach (var player in mongoPlayers)
                {
                    var playerNameParts = player.PlayerName.Split(new char[] { ' ' });

                    // throws an exception if there is more than 1 element in the sequence
                    var playerInDb = dbContext.Players.Local
                            .SingleOrDefault(p => p.AdditionalInfo.PlayerId == player.PlayerId); // runs in memory

                    if (playerInDb == null)
                    {
                        dbContext.Players.Add(new Player
                        {
                            FirstName = playerNameParts.Length > 0 ? playerNameParts[0] : "",
                            LastName = playerNameParts.Length > 1 ? playerNameParts[1] : "",
                            AdditionalInfo = new PlayerInfo
                            {
                                PlayerId = player.PlayerId,
                                Birthday = DateTime.ParseExact(player.BirthDate, "MMM dd, yyyy", CultureInfo.InvariantCulture),
                                Height = ConvertHeightFromFeetsInchesToCentimeters(player.Height),
                                Weight = PoundsToKilogram(player.Weight)
                            },
                            School = string.IsNullOrEmpty(player.School) ? null : new School
                            {
                                Name = player.School
                            },
                            Position = player.Position,
                            TeamId = dbContext.Teams.Local
                                .Single(t => t.TeamId == player.TeamId)
                                .Id
                        });
                    }
                }

                await dbContext.SaveChangesAsync();
            });
        }

        /// <summary>
        /// Convert between feet/inches and cm
        /// 
        /// source: http://sg.answers.yahoo.com/question/index?qid=20070403095853AA3FKJZ
        /// </summary>
        /// <param name="sHeight"></param>
        /// <returns></returns>
        public static double ConvertHeightFromFeetsInchesToCentimeters(string sHeight)
        {
            var feets = int.Parse(sHeight.Split(new char[] { '-' })[0]);
            var inches = int.Parse(sHeight.Split(new char[] { '-' })[1]);

            var totalInches = (feets * 12) + inches;
            var centimeters = totalInches * 2.54;

            return Math.Round(centimeters, 2);
        }

        public static double PoundsToKilogram(string sPounds)
        {
            return Math.Round(int.Parse(sPounds) / 2.20462262);
        }
    }
}
