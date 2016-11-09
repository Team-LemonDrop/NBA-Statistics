using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using NBAStatistics.Data.Repositories.SQLServer;
using NBAStatistics.Models;

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

                var nullTeamInDb = dbContext.Teams.Local
                    .SingleOrDefault(t => t.TeamId == 0); // runs in memory

                if (nullTeamInDb == null)
                {
                    // add fake team for players without team
                    dbContext.Teams.Add(new Team
                    {
                        TeamId = 0,
                        Name = "NoName",
                        Abbreviation = "",
                        Founded = 0,
                        City = new City
                        {
                            Name = "NoName",
                            Country = new Country
                            {
                                Name = "NoName"
                            }
                        },
                        Arena = new Arena
                        {
                            Name = "NoName"
                        },
                        HeadCoach = new HeadCoach
                        {
                            Name = "NoName"
                        }
                    });
                }

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
                                Name = team.Name,
                                Abbreviation = team.Abbreviation,
                                Founded = team.Founded,
                                City = new City
                                {
                                    Name = team.City,
                                    Country = new Country
                                    {
                                        Name = team.Country
                                    }
                                },
                                Arena = new Arena
                                {
                                    Name = team.Arena
                                },
                                HeadCoach = new HeadCoach
                                {
                                    Name = team.HeadCoach
                                }
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
                    // throws an exception if there is more than 1 element in the sequence
                    var playerInDb = dbContext.Players.Local
                            .SingleOrDefault(p => p.AdditionalInfo.PlayerId == player.PlayerId); // runs in memory

                    if (playerInDb == null)
                    {
                        dbContext.Players.Add(new Player
                        {
                            FirstName = player.FirstName,
                            LastName = player.LastName,
                            AdditionalInfo = new PlayerInfo
                            {
                                PlayerId = player.PlayerId,
                                Birthday = player.BirthDate,
                                Height = string.IsNullOrEmpty(player.Height) ? null : ConvertHeightFromFeetsInchesToCentimeters(player.Height),
                                Weight = string.IsNullOrEmpty(player.Weight) ? null : PoundsToKilogram(player.Weight)
                            },
                            School = string.IsNullOrEmpty(player.School) ? null : new School
                            {
                                Name = player.School
                            },
                            Country = string.IsNullOrEmpty(player.Country) ?
                                new Country { Name = "NoName" } :
                                    string.IsNullOrEmpty(player.Country.Trim()) ?
                                        new Country { Name = "NoName" } :
                                        new Country { Name = player.Country },
                            Position = player.Position,
                            RosterStatus = player.RosterStatus,
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
        public static double? ConvertHeightFromFeetsInchesToCentimeters(string sHeight)
        {
            if (string.IsNullOrEmpty(sHeight))
            {
                return null;
            }

            var feets = int.Parse(sHeight.Split(new char[] { '-' })[0]);
            var inches = int.Parse(sHeight.Split(new char[] { '-' })[1]);

            var totalInches = (feets * 12) + inches;
            var centimeters = totalInches * 2.54;

            return Math.Round(centimeters, 1);
        }

        public static double? PoundsToKilogram(string sPounds)
        {
            if (string.IsNullOrEmpty(sPounds))
            {
                return null;
            }

            return Math.Round(int.Parse(sPounds) / 2.20462262);
        }
    }
}
