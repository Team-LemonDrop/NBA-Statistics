using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using System.Windows.Forms;
using System.IO.Compression;
using System.IO;
using System.Data.OleDb;
using System.Data;

namespace NBAStatistics.Data.ImportIntoSqlServer
{
    public class ImportIntoSqlServer
    {
        const string User = "miro";
        const string Pass = "1qazcde3";
        const string DbHost = "ds029565.mlab.com";
        const int DbPort = 29565;
        const string DbName = "appharbor_5cwg75nh";

        public static async Task ImportFromZipFile(string zipPath)
        {
            var dbContext = new NBAStatisticsDbContext();

            await Task.Run(() =>
            {
                try
                {
                    var directoryPath = zipPath.Substring(0, zipPath.LastIndexOf('\\'));
                    string directoryWithReports = $"{directoryPath}\\Reports\\";

                    if (Directory.Exists(directoryWithReports))
                    {
                        Directory.Delete(directoryWithReports, true);
                    }

                    // If the directory already exists, this method does not create a new directory
                    DirectoryInfo di = Directory.CreateDirectory(directoryWithReports);

                    using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                    {
                        foreach (ZipArchiveEntry entry in archive.Entries)
                        {
                            var directoryName = entry.FullName.Substring(0, 11);
                            var dateOfTheReport = DateTime.ParseExact(
                                directoryName,
                                "dd-MMM-yyyy",
                                CultureInfo.InvariantCulture);

                            var xlsFileName = entry.FullName.Substring(12);
                            var xlsPath = $"{directoryWithReports}{directoryName}_{xlsFileName}";
                            entry.ExtractToFile(xlsPath);

                            var connectionString = GetConnectionString(xlsPath, false);

                            using (var oleDbConnection = new OleDbConnection(connectionString))
                            {
                                oleDbConnection.Open();
                                var sheetNames = GetSheetNames(oleDbConnection);

                                var oleDbCommand =
                                    new OleDbCommand("SELECT * FROM [" + sheetNames.First() + "]", oleDbConnection);

                                using (var oleDbAdapter = new OleDbDataAdapter(oleDbCommand))
                                {
                                    var dataSet = new DataSet();
                                    oleDbAdapter.Fill(dataSet);

                                    // Load into the dbContext                                    
                                    dbContext.StandingsByDays.Load();
                                    dbContext.Teams.Load();
                                    dbContext.Conferences.Load();

                                    using (var reader = dataSet.CreateDataReader())
                                    {
                                        while (reader.Read())
                                        {
                                            var teamId = (int)(double)reader["TEAM_ID"];           // TEAM_ID
                                            var leagueId = reader["LEAGUE_ID"];                    // LEAGUE_ID
                                            var seasonId = (int)(double)reader["SEASON_ID"];       // SEASON_ID
                                            var standingsDate = (DateTime)reader["STANDINGSDATE"]; // STANDINGSDATE
                                            var conference = (string)reader["CONFERENCE"];         // CONFERENCE
                                            var team = (string)reader["TEAM"];                     // TEAM
                                            var games = (byte)(double)reader["G"];                 // G
                                            var wins = (byte)(double)reader["W"];                  // W
                                            var losses = (byte)(double)reader["L"];                // L
                                            var winningsPercentage = (double)reader["W_PCT"];      // W_PCT
                                            var homeRecord = (string)reader["HOME_RECORD"];        // HOME_RECORD
                                            var roadRecord = (string)reader["ROAD_RECORD"];        // ROAD_RECORD

                                            var standingsByDayInDb = dbContext.StandingsByDays.Local
                                                .SingleOrDefault(sbd => (sbd.TeamId == teamId) && (sbd.Date == standingsDate)); // runs in memory

                                            // for tests only
                                            //var t1 = dbContext.Teams.Local
                                            //            .SingleOrDefault(t => t.TeamId == teamId)
                                            //            .Id;
                                            //var t2 = seasonId;
                                            //var t3 = standingsDate;
                                            //var t4 = dbContext.Conferences.Local
                                            //            .SingleOrDefault(c => c.Name == conference)
                                            //            .Id;
                                            //var t5 = games;
                                            //var t6 = wins;
                                            //var t7 = losses;
                                            //var t8 = winningsPercentage;
                                            //var t9 = (byte)homeRecord.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)
                                            //    .Select(int.Parse)
                                            //    .ToArray()[0];
                                            //var t10 = (byte)roadRecord.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)
                                            //    .Select(int.Parse)
                                            //    .ToArray()[0];

                                            if (standingsByDayInDb == null)
                                            {
                                                dbContext.StandingsByDays.Add(new StandingsByDay
                                                {
                                                    TeamId = dbContext.Teams.Local
                                                        .SingleOrDefault(t => t.TeamId == teamId)
                                                        .Id,
                                                    SeasonId = seasonId,
                                                    Date = standingsDate,
                                                    ConferenceId = dbContext.Conferences.Local
                                                        .SingleOrDefault(c => c.Name == conference)
                                                        .Id,
                                                    Games = games,
                                                    Wins = wins,
                                                    Loses = losses,
                                                    SuccessRate = Math.Round(winningsPercentage, 2),
                                                    HomeRecord = (byte)homeRecord.Split(new char[] { '-'}, StringSplitOptions.RemoveEmptyEntries)                                                        
                                                        .Select(int.Parse)
                                                        .ToArray()[0],
                                                    RoadRecord = (byte)roadRecord.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries)
                                                        .Select(int.Parse)
                                                        .ToArray()[0]
                                                });
                                            }
                                        }
                                    }
                                }
                            }

                            // Force clean up to release file handles
                            // source: http://stackoverflow.com/questions/2225087/the-process-cannot-access-the-file-because-it-is-being-used-by-another-process
                            GC.Collect();
                        }
                    }

                    dbContext.SaveChanges();

                    Directory.Delete(directoryWithReports, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        public static async Task ImportFromMongoDB()
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
                        var playersSeasons = new List<PlayerSeasonPointsPerGame>();
                        foreach (var sppg in player.SeasonPointsPerGame)
                        {
                            var season = sppg.Key;
                            var pointsPerGame = sppg.Value;

                            playersSeasons.Add(new PlayerSeasonPointsPerGame
                            {
                                PlayerId = player.PlayerId,
                                SeasonId = season,
                                PointsPerGame = pointsPerGame
                            });
                        }

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

        public static string GetConnectionString(string filePath, bool xlsx)
        {
            Dictionary<string, string> props;

            if (xlsx)
            {
                // XLSX - Excel 2007, 2010, 2012, 2013
                props = new Dictionary<string, string>
                {
                    ["Provider"] = "Microsoft.ACE.OLEDB.12.0",
                    ["Data Source"] = filePath,
                    ["Extended Properties"] = "\"Excel 12.0 XML\""
                };
            }
            else
            {
                // XLS - Excel 2003 and Older
                props = new Dictionary<string, string>
                {
                    ["Provider"] = "Microsoft.Jet.OLEDB.4.0",
                    ["Data Source"] = filePath,
                    ["Extended Properties"] = "\"Excel 8.0;HDR=YES;\""
                };
            }

            var sb = new StringBuilder();
            foreach (var prop in props)
            {
                sb.Append(prop.Key);
                sb.Append('=');
                sb.Append(prop.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }

        private static IEnumerable<string> GetSheetNames(OleDbConnection oleDbConnection)
        {
            DataTable sheets = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            ICollection<string> documentSheets = new List<string>();

            foreach (DataRow row in sheets.Rows)
            {
                documentSheets.Add(row["TABLE_NAME"].ToString());
            }

            return documentSheets;
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
