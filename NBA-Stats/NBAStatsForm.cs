using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

using System.Collections;
using System.Data.OleDb;
using NBA_Stats.ConnectionProviders;
using System.Globalization;
using System.IO.Compression;
using NBAStatistics.Models.Models.Json;
using NBAStatistics.Data.FillMongoDB.Models;
using MongoDB.Bson;
using NBAStatistics.Data.FillMongoDB;
using NBAStatistics.Data;
using System.Xml.Linq;
using System.Xml.XPath;
using NBAStatistics.Data.ImportIntoSqlServer;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using NBAStatistics.Data.MySQL;
using NBAStatistics.Data.MySQL.Models;
using NBAStatistics.Data.Repositories.SQLServer;
using NBAStatistics.Data.Repositories;
using NBAStatistics.DataImporters;
using SeasonMongo = NBAStatistics.Data.FillMongoDB.Models.Season;
using PlayerMongo = NBAStatistics.Data.FillMongoDB.Models.Player;
using NBAStatistics.Reports;

namespace NBA_Stats
{
    public partial class NBAStatsForm : Form
    {
        private const string SHOOTING_ADDRESS = "http://stats.nba.com/js/data/sportvu/shootingData.js";
        private const string GAMES = "http://stats.nba.com/stats/shotchartdetail?Season=2013-14&SeasonType=Regular+Season&LeagueID=00&TeamID=1610612743&PlayerID=0&GameID=0021300605&Outcome=&Location=&Month=0&SeasonSegment=&DateFrom=&DateTo=&OpponentTeamID=0&VsConference=&VsDivision=&Position=&RookieYear=&GameSegment=&Period=0&LastNGames=0&ContextFilter=&ContextMeasure=FG_PCT&display-mode=performance&zone-mode=zone&zoneOverlays=false&zoneDetails=false&viewShots=true";

        private static XmlDocument xmlDoc;

        private readonly string ExeDirectory;

        public NBAStatsForm()
        {
            InitializeComponent();

            this.ExeDirectory = AppDomain.CurrentDomain.BaseDirectory;

            var dbContext = new NBAStatisticsDbContext();

            if (dbContext.Conferences.Count() == 0)
            {
                dbContext.Conferences.Add(new NBAStatistics.Models.Conference
                {
                    Name = "East"
                });
                dbContext.Conferences.Add(new NBAStatistics.Models.Conference
                {
                    Name = "West"
                });
            }

            dbContext.SaveChanges();
        }

        private async void btnGenerateZipFile_Click(object sender, EventArgs e)
        {
            this.btnGenerateZipFile.Enabled = false;

            const string DailyStandingsUri = "http://stats.nba.com/stats/scoreboard?DayOffset=0&LeagueID=00&gameDate=";

            DateTime date = DateTime.Now;

            try
            {
                string directoryWithReports = $"{this.ExeDirectory}Reports\\";
                string zipPath = $"{this.ExeDirectory}reports.zip";

                if (Directory.Exists(directoryWithReports))
                {
                    Directory.Delete(directoryWithReports, true);
                }

                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                // If the directory already exists, this method does not create a new directory
                DirectoryInfo di = Directory.CreateDirectory(directoryWithReports);

                var options = new Dictionary<string, string>();
                options["Referer"] = "http://stats.nba.com/scores/";

                var tasks = new List<Task<DailyStandings>>();

                int numberOfFiles = 10;
                for (int i = 0; i < numberOfFiles; i++)
                {
                    date = date.AddDays(-1);

                    string uriString = DailyStandingsUri + date.ToString("MM-dd-yyyy").Replace("-", "%2F");

                    // random delay to simulate human requests and prevent blocking of 
                    // our IP address from server
                    int milisecondsToDelay = RandomProvider.Instance.Next(0, numberOfFiles * 50);

                    tasks.Add(GetJsonObjFromNetworkFileAsync<DailyStandings>(uriString, Encoding.UTF8, options, milisecondsToDelay));
                }

                await Task.Run(async () =>
                {
                    foreach (var dailyStandings in await Task.WhenAll(tasks))
                    {
                        if (dailyStandings == null)
                        {
                            MessageBox.Show("Daily Standings url does not response with JSON file.");
                            return;
                        }

                        DateTime gameDate = DateTime.ParseExact(
                                dailyStandings.Parameters.GameDate,
                                "MM/dd/yyyy",
                                CultureInfo.InvariantCulture);

                        string directoryPath = directoryWithReports + gameDate.ToString("dd-MMM-yyyy");
                        Directory.CreateDirectory(directoryPath);

                        IEnumerable<string> reportsNames = new string[] {
                            "EastConfStandingsByDay",
                            "WestConfStandingsByDay" };

                        foreach (var reportName in reportsNames)
                        {
                            var xlsPath = $"{directoryPath}\\{reportName}.xls";
                            var connectionString = OleDbConnectionProvider.GetConnectionString(xlsPath, false);

                            using (var oleDbConnection = new OleDbConnection(connectionString))
                            {
                                oleDbConnection.Open();
                                var sheetNames = GetSheetNames(oleDbConnection);
                                if (sheetNames.Count() != 2)
                                {
                                    SetSheetNames(reportName, oleDbConnection);
                                }

                                foreach (var resultSet in dailyStandings.ResultSets)
                                {
                                    foreach (var row in resultSet.RowSet)
                                    {
                                        if (resultSet.Name == reportName)
                                        {
                                            var teamId = (int)(long)row[0];
                                            var leagueId = (string)row[1];
                                            var seasonId = int.Parse((string)row[2]);
                                            var standingsDate = DateTime.ParseExact((string)row[3], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                                            var conference = (string)row[4];
                                            var team = (string)row[5];
                                            var games = (byte)(long)row[6];
                                            var wins = (byte)(long)row[7];
                                            var losses = (byte)(long)row[8];
                                            var winningsPercentage = (float)(double)row[9];
                                            var homeRecord = (string)row[10];
                                            var roadRecord = (string)row[11];

                                            var cmd = new OleDbCommand(
                                                $"INSERT INTO [{resultSet.Name}] VALUES (@TEAM_ID, @LEAGUE_ID, @SEASON_ID, @STANDINGSDATE, @CONFERENCE, @TEAM, @G, @W, @L, @W_PCT, @HOME_RECORD, @ROAD_RECORD)",
                                                oleDbConnection);

                                            cmd.Parameters.AddWithValue("@TEAM_ID", teamId);
                                            cmd.Parameters.AddWithValue("@LEAGUE_ID", leagueId);
                                            cmd.Parameters.AddWithValue("@SEASON_ID", seasonId);
                                            cmd.Parameters.AddWithValue("@STANDINGSDATE", standingsDate);
                                            cmd.Parameters.AddWithValue("@CONFERENCE", conference);
                                            cmd.Parameters.AddWithValue("@TEAM", team);
                                            cmd.Parameters.AddWithValue("@G", games);
                                            cmd.Parameters.AddWithValue("@W", wins);
                                            cmd.Parameters.AddWithValue("@L", losses);
                                            cmd.Parameters.AddWithValue("@W_PCT", winningsPercentage);
                                            cmd.Parameters.AddWithValue("@HOME_RECORD", homeRecord);
                                            cmd.Parameters.AddWithValue("@ROAD_RECORD", roadRecord);

                                            cmd.ExecuteNonQuery();
                                        }
                                    }
                                }

                                oleDbConnection.Close();
                            }

                            // Force clean up to release file handles
                            // source: http://stackoverflow.com/questions/2225087/the-process-cannot-access-the-file-because-it-is-being-used-by-another-process
                            GC.Collect();
                        }
                    }
                });

                ZipFile.CreateFromDirectory(directoryWithReports, zipPath);

                Directory.Delete(directoryWithReports, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.btnGenerateZipFile.Enabled = true;
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

        private static bool SetSheetNames(string sheetName, OleDbConnection oleDbConnection)
        {
            string createTableScript = $"CREATE TABLE {sheetName} ([TEAM_ID] int, [LEAGUE_ID] varchar(2), [SEASON_ID] int, [STANDINGSDATE] date, [CONFERENCE] varchar(10), [TEAM] nvarchar(50), [G] tinyint, [W] tinyint, [L] tinyint, [W_PCT] float, [HOME_RECORD] varchar(5), [ROAD_RECORD] varchar(5))";
            using (var cmd = new OleDbCommand(createTableScript, oleDbConnection))
            {
                cmd.ExecuteNonQuery();
            }

            DataTable sheets = oleDbConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (sheets.Rows.Count == 2)
            {
                return true;
            }

            return false;
        }

        private static async Task GetXmlDocFromNetworkFileAsync(string uriString, Encoding encoding)
        {
            try
            {
                var webClient = new WebClient() { Encoding = encoding };

                using (Stream stream = await webClient.OpenReadTaskAsync(new Uri(uriString, UriKind.Absolute)))
                {
                    xmlDoc = GetXml(stream);
                }
            }
            catch (Exception)
            {
                xmlDoc = null;
            }
        }

        private async Task<T> GetJsonObjFromNetworkFileAsync<T>(
            string uriString,
            Encoding encoding,
            IEnumerable options,
            int milisecondsToDelay)
        {
            try
            {
                var address = new Uri(uriString, UriKind.Absolute);

                var webClient = new WebClient();
                webClient.Encoding = encoding;

                foreach (KeyValuePair<string, string> kvp in options)
                {
                    webClient.Headers.Add(kvp.Key, kvp.Value);
                }

                // delay to simulate human requests and prevent blocking of 
                // our IP address from server
                await Task.Delay(milisecondsToDelay);

                using (Stream stream = await webClient.OpenReadTaskAsync(address))
                {
                    return stream.CreateFromJsonStream<T>(Encoding.UTF8);
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static XmlDocument GetXml(Stream stream)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);

            return xmlDoc;
        }

        public static JObject GetJsonObject(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                //string tmp = sr.ReadToEnd();
                return (JObject)serializer.Deserialize(jsonTextReader);
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private async void btnFillMongoDb_Click(object sender, EventArgs e)
        {
            this.btnFillMongoDb.Enabled = false;

            const string TeamUri = "http://stats.nba.com/stats/commonteamroster?";
            await Task.Run(async () =>
            {
                try
                {
                    // === Move === //
                    const string User = "miro";
                    const string Pass = "1qazcde3";
                    const string DbHost = "ds029565.mlab.com";
                    const int DbPort = 29565;
                    const string DbName = "appharbor_5cwg75nh";

                    var credentials = MongoCredential.CreateCredential(DbName, User, Pass);
                    var settings = new MongoClientSettings
                    {
                        Server = new MongoServerAddress(DbHost, DbPort),
                        Credentials = new List<MongoCredential> { credentials }
                    };

                    var client = new MongoClient(settings);
                    var db = client.GetDatabase(DbName);
                    // === Move === //

                    var xmlDoc = XDocument.Load("../../teams-by-season.xml");

                    var seasonIdsFromXml = xmlDoc.XPathSelectElements("/seasons/season")
                        .Select(el => el.Attribute("id").Value);

                    var seasons = new HashSet<Season>();
                    var seasonsToAddInMongoDb = new HashSet<Season>();

                    // get just season Id's to reduce network traffic
                    var collectionSeasons = db.GetCollection<Season>("Seasons");
                    var fieldsSeasons = Builders<Season>.Projection.Include(s => s.SeasonId);

                    var seasonIdsInMongoDb = collectionSeasons.Find(s => true)
                        .Project<Season>(fieldsSeasons)
                        .ToList()
                        .Select(s => s.SeasonId);

                    foreach (var seasonId in seasonIdsFromXml)
                    {
                        var teams = xmlDoc.XPathSelectElements($"/seasons/season[@id='{seasonId}']/teams/team")
                            .Select(el => new Team(
                                int.Parse(el.Attribute("id").Value),
                                el.Element("name").Value,
                                el.Attribute("abbreviation").Value,
                                el.Element("logo-src").Value,
                                el.Element("headcoach").Value,
                                int.Parse(el.Attribute("founded").Value),
                                el.Element("arena").Value,
                                el.Attribute("city").Value,
                                el.Attribute("country").Value))
                            .ToList();

                        seasons.Add(new Season(seasonId, teams));

                        if (!seasonIdsInMongoDb.Contains(seasonId))
                        {
                            seasonsToAddInMongoDb.Add(new Season(seasonId, teams));
                        }
                    }

                    if (seasons.Count == 0)
                    {
                        this.btnFillMongoDb.Enabled = true;

                        // MongoDB already contains this seasons 
                        return;
                    }

                    if (seasonsToAddInMongoDb.Count > 0)
                    {
                        await FillMongoDB.FillDatabase(
                            seasonsToAddInMongoDb.Select(x => x.ToBsonDocument<Season>()), "Seasons");
                    }

                    foreach (var seasonId in seasonIdsFromXml)
                    {
                        var players = new HashSet<Player>();
                        var coaches = new HashSet<Coach>();

                        var teams = seasons.First(x => x.SeasonId == seasonId).Teams;
                        var options = new Dictionary<string, string>();

                        var tasks = new HashSet<Task<TeamInfo>>();

                        foreach (var team in teams)
                        {
                            string uriString = $"{TeamUri}TeamID={team.TeamId}&Season={seasonId}";

                            // random delay to simulate human requests and prevent blocking of 
                            // our IP address from server
                            int milisecondsToDelay = RandomProvider.Instance.Next(0, teams.Count() * 10);

                            tasks.Add(GetJsonObjFromNetworkFileAsync<TeamInfo>(uriString, Encoding.UTF8, options, milisecondsToDelay));
                        }

                        await Task.Run(async () =>
                        {
                            // get just player Id's to reduce network traffic
                            var collectionPlayers = db.GetCollection<Player>("Players");
                            //var conditionPlayers = Builders<Player>.Filter.Eq(p => p.TeamId, currentTeamId);
                            var fieldsPlayers = Builders<Player>.Projection.Include(p => p.PlayerId);

                            var playerIdsInMongoDb = collectionPlayers.Find(p => true)//.Find(conditionPlayers)
                                .Project<Player>(fieldsPlayers)
                                .ToList()
                                .Select(p => p.PlayerId);

                            // get just coach Id's to reduce network traffic
                            var collectionCoaches = db.GetCollection<Coach>("Coaches");
                            //var conditionCoaches = Builders<Coach>.Filter.Eq(c => c.TeamId, currentTeamId);
                            var fieldsCoaches = Builders<Coach>.Projection.Include(c => c.CoachId);

                            var coachIdsInMongoDb = collectionCoaches.Find(c => true)//conditionCoaches)
                                .Project<Coach>(fieldsCoaches)
                                .ToList()
                                .Select(c => c.CoachId);

                            foreach (var teamInfo in await Task.WhenAll(tasks))
                            {
                                if (teamInfo == null)
                                {
                                    MessageBox.Show("TeamInfo url does not response with JSON file.");
                                    return;
                                }

                                // we need this when use conditionPlayers and conditionCoaches
                                //var currentTeamId = (int)(long)teamInfo.ResultSets[0].RowSet[0][0];

                                foreach (var resultSet in teamInfo.ResultSets)
                                {
                                    if (resultSet.Name == "CommonTeamRoster")
                                    {
                                        foreach (var row in resultSet.RowSet) // players
                                        {
                                            var playerId = (int)(long)row[12];

                                            if (!playerIdsInMongoDb.Contains(playerId))
                                            {
                                                string uriString = $"http://stats.nba.com/stats/commonplayerinfo?PlayerID={playerId}";

                                                // random delay to simulate human requests and prevent blocking of 
                                                // our IP address from server
                                                int milisecondsToDelay = RandomProvider.Instance.Next(0, 20);

                                                var playerInfo = await GetJsonObjFromNetworkFileAsync<PlayerInfo>(uriString, Encoding.UTF8, options, milisecondsToDelay);

                                                var personId = playerInfo.ResultSets[0].RowSet[0][0];
                                                var firstName = playerInfo.ResultSets[0].RowSet[0][1];
                                                var lastName = playerInfo.ResultSets[0].RowSet[0][2];
                                                var displayFirstLastName = playerInfo.ResultSets[0].RowSet[0][3];
                                                var displayLastCommaFirstName = playerInfo.ResultSets[0].RowSet[0][4];
                                                var displayFiLastName = playerInfo.ResultSets[0].RowSet[0][5];
                                                DateTime birthDate = (DateTime)playerInfo.ResultSets[0].RowSet[0][6];
                                                var school = playerInfo.ResultSets[0].RowSet[0][7];
                                                var country = playerInfo.ResultSets[0].RowSet[0][8];
                                                var lastAffiliation = playerInfo.ResultSets[0].RowSet[0][9];
                                                var height = playerInfo.ResultSets[0].RowSet[0][10];
                                                var weight = playerInfo.ResultSets[0].RowSet[0][11];
                                                var seasonExp = playerInfo.ResultSets[0].RowSet[0][12];
                                                var jersey = playerInfo.ResultSets[0].RowSet[0][13];
                                                var position = playerInfo.ResultSets[0].RowSet[0][14];
                                                var rosterStatus = playerInfo.ResultSets[0].RowSet[0][15];
                                                var teamId = playerInfo.ResultSets[0].RowSet[0][16];
                                                var teamName = playerInfo.ResultSets[0].RowSet[0][17];
                                                var teamAbbreviation = playerInfo.ResultSets[0].RowSet[0][18];
                                                var teamCode = playerInfo.ResultSets[0].RowSet[0][19];
                                                var teamCity = playerInfo.ResultSets[0].RowSet[0][20];
                                                var playerCode = playerInfo.ResultSets[0].RowSet[0][21];
                                                var fromYear = playerInfo.ResultSets[0].RowSet[0][22];
                                                var toYear = playerInfo.ResultSets[0].RowSet[0][23];
                                                var dLeagueFlag = playerInfo.ResultSets[0].RowSet[0][24];
                                                var gamesPlayedFlag = playerInfo.ResultSets[0].RowSet[0][25];

                                                // fill pointsPerGame with fake data
                                                var seasonPointsPerGame = new Dictionary<string, double>();
                                                seasonPointsPerGame[seasonId] = (double)RandomProvider.Instance.Next(0, 5000) / 100;
                                                seasonPointsPerGame["2015-16"] = (double)RandomProvider.Instance.Next(0, 5000) / 100;

                                                players.Add(new Player(
                                                    (int)(long)personId,
                                                    (string)firstName,
                                                    (string)lastName,
                                                    (string)displayFirstLastName,
                                                    (string)displayLastCommaFirstName,
                                                    (string)displayFiLastName,
                                                    (DateTime)birthDate,
                                                    (string)school,
                                                    (string)country,
                                                    (string)lastAffiliation,
                                                    (string)height,
                                                    (string)weight,
                                                    (int)(long)seasonExp,
                                                    (string)jersey,
                                                    (string)position,
                                                    (string)rosterStatus,
                                                    (int)(long)teamId,
                                                    (string)teamName,
                                                    (string)teamAbbreviation,
                                                    (string)teamCode,
                                                    (string)teamCity,
                                                    (string)playerCode,
                                                    (int)(long)fromYear,
                                                    (int)(long)toYear,
                                                    (string)dLeagueFlag,
                                                    (string)gamesPlayedFlag,
                                                    seasonPointsPerGame
                                                ));
                                            }
                                        }
                                    }
                                    else if (resultSet.Name == "Coaches")
                                    {
                                        foreach (var row in resultSet.RowSet)
                                        {
                                            var coachId = row[2];

                                            if (!coachIdsInMongoDb.Contains((string)coachId))
                                            {
                                                var teamId = row[0];
                                                var season = row[1];
                                                //var coachId = row[2];
                                                var firstName = row[3];
                                                var lastName = row[4];
                                                var coachName = row[5];
                                                var coachCode = row[6];
                                                var isAssistant = row[7];
                                                var coachType = row[8];
                                                var school = row[9];
                                                var sortSequence = row[10];

                                                coaches.Add(new Coach(
                                                    (int)(long)teamId,
                                                    (string)season,
                                                    (string)coachId,
                                                    (string)firstName,
                                                    (string)lastName,
                                                    (string)coachName,
                                                    (string)coachCode,
                                                    (int)(double)isAssistant,
                                                    (string)coachType,
                                                    (string)school,
                                                    (int?)(double?)sortSequence
                                                ));
                                            }
                                        }
                                    }
                                }
                            }

                            await FillMongoDB.FillDatabase(
                                players.Select(x => x.ToBsonDocument()), "Players");

                            await FillMongoDB.FillDatabase(
                                coaches.Select(x => x.ToBsonDocument()), "Coaches");
                        });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });

            this.btnFillMongoDb.Enabled = true;
        }

        private async void btnImportDataIntoSqlServer_Click(object sender, EventArgs e)
        {
            //this.btnImportDataIntoSqlServer.Enabled = false;

            //const string User = "miro";
            //const string Pass = "1qazcde3";
            //const string DbHost = "ds029565.mlab.com";
            //const int DbPort = 29565;
            //const string DbName = "appharbor_5cwg75nh";

            //var credentials = MongoCredential.CreateCredential(DbName, User, Pass);
            //var settings = new MongoClientSettings
            //{
            //    Server = new MongoServerAddress(DbHost, DbPort),
            //    Credentials = new List<MongoCredential> { credentials }
            //};

            //var client = new MongoClient(settings);
            //var mongoDb = client.GetDatabase(DbName);

            try
            {
                await ImportIntoSqlServer.ImportFromMongoDB();

                string zipPath = $"{this.ExeDirectory}reports.zip";

                await ImportIntoSqlServer.ImportFromZipFile(zipPath);

                // Adding teams
                //var seasonsSourceRepository = new MongoRepository<SeasonMongo>(mongoDb);
                //var dbContext = new NBAStatisticsDbContext();
                //var teamsRepository = new EfRepository<NBAStatistics.Models.Team>(dbContext);
                //var teamsUnitOfWork = new EfUnitOfWork(dbContext);
                //var teamsImporter = new TeamsImporter(seasonsSourceRepository, teamsRepository, teamsUnitOfWork);

                //await teamsImporter.Import();

                //// Adding players
                //var playersSourceRepository = new MongoRepository<PlayerMongo>(mongoDb);
                //dbContext = new NBAStatisticsDbContext();
                //var playersRepository = new EfRepository<NBAStatistics.Models.Player>(dbContext);
                //teamsRepository = new EfRepository<NBAStatistics.Models.Team>(dbContext);
                //var playersUnitOfWork = new EfUnitOfWork(dbContext);
                //var playersImporter = new PlayersImporter(playersSourceRepository, playersRepository, teamsRepository, playersUnitOfWork);

                //await playersImporter.Import();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.btnImportDataIntoSqlServer.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var nbaStastsContext = new NBAStatisticsDbContext();
            var teamsRepository = new EfRepository<NBAStatistics.Models.StandingsByDay>(nbaStastsContext);
            var pdfService = new PdfReportService(teamsRepository);
            pdfService.GeneratePdf();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var jsonHandler = new JsonHandler();
            var reportService = new JsonReportService(jsonHandler);
            //reportService.CreatePointsPerGameReport()
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var xmlHandler = new PlayedGamesXmlHandler();
            var reportService = new XmlReportService(xmlHandler);
            var nbaStatsContext = new NBAStatisticsDbContext();
            var dailyStandingsRepository = new EfRepository<NBAStatistics.Models.StandingsByDay>(nbaStatsContext);
            reportService.CreatePlayedGamesReport(dailyStandingsRepository);
        }
    }
}
