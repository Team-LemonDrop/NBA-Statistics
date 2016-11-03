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
                    int secondsToDelay = RandomProvider.Instance.Next(0, numberOfFiles);

                    tasks.Add(GetJsonObjFromNetworkFileAsync<DailyStandings>(uriString, Encoding.UTF8, options, secondsToDelay));
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
                            // http://stackoverflow.com/questions/2225087/the-process-cannot-access-the-file-because-it-is-being-used-by-another-process
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
            int secondsToDelay)
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
                await Task.Delay(secondsToDelay * 1000);

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

            try
            {
                uint team_Id = 1610612741;
                var seasonStr = "2016-17";

                var options = new Dictionary<string, string>();

                var tasks = new List<Task<TeamInfo>>();

                int numberOfTeams = 30;
                for (int i = 0; i < numberOfTeams; i++)
                {
                    string uriString = $"{TeamUri}TeamID={team_Id}&Season={seasonStr}";

                    // random delay to simulate human requests and prevent blocking of 
                    // our IP address from server
                    int secondsToDelay = RandomProvider.Instance.Next(0, numberOfTeams);

                    tasks.Add(GetJsonObjFromNetworkFileAsync<TeamInfo>(uriString, Encoding.UTF8, options, secondsToDelay / 3));
                }

                var players = new List<BsonDocument>();

                await Task.Run(async () =>
                    {
                        foreach (var teamInfo in await Task.WhenAll(tasks))
                        {
                            if (teamInfo == null)
                            {
                                MessageBox.Show("TeamInfo url does not response with JSON file.");
                                return;
                            }

                            foreach (var resultSet in teamInfo.ResultSets)
                            {
                                if (resultSet.Name == "CommonTeamRoster")
                                {
                                    foreach (var row in resultSet.RowSet)
                                    {
                                        var teamId = (int)(long)row[0];
                                        var season = (string)row[1];
                                        var leagueId = (string)row[2];
                                        var playerName = (string)row[3];
                                        var num = (string)row[4];
                                        var position = (string)row[5];
                                        var height = (string)row[6];
                                        var weight = (string)row[7];
                                        var birthDate = (string)row[8];
                                        var age = (double)row[9];
                                        var exp = (string)row[10];
                                        var school = (string)row[11];
                                        var playerId = (int)(long)row[12];

                                        players.Add(new NBAStatistics.Data.FillMongoDB.Models.Player(
                                            teamId,
                                            season,
                                            leagueId,
                                            playerName,
                                            num,
                                            position,
                                            height,
                                            weight,
                                            birthDate,
                                            age,
                                            exp,
                                            school,
                                            playerId
                                        ).ToBsonDocument());
                                    }
                                }
                            }
                        }
                    });

                await FillMongoDB.FillPlayersCollection(players);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.btnFillMongoDb.Enabled = true;
        }

        private void btnImportZipDataToSqlServer_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var dbContext = new NBAStatisticsContext();
                MessageBox.Show(dbContext.Players.Count().ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ex " + ex.Message);
            }
        }
    }
}
