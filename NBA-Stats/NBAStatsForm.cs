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

using NBAStatistics.Data.Models;
using System.Collections;
using System.Data.OleDb;
using NBA_Stats.ConnectionProviders;
using System.Globalization;
using System.IO.Compression;

namespace NBA_Stats
{
    public partial class NBAStatsForm : Form
    {
        private const string SHOOTING_ADDRESS = "http://stats.nba.com/js/data/sportvu/shootingData.js";
        private const string GAMES = "http://stats.nba.com/stats/shotchartdetail?Season=2013-14&SeasonType=Regular+Season&LeagueID=00&TeamID=1610612743&PlayerID=0&GameID=0021300605&Outcome=&Location=&Month=0&SeasonSegment=&DateFrom=&DateTo=&OpponentTeamID=0&VsConference=&VsDivision=&Position=&RookieYear=&GameSegment=&Period=0&LastNGames=0&ContextFilter=&ContextMeasure=FG_PCT&display-mode=performance&zone-mode=zone&zoneOverlays=false&zoneDetails=false&viewShots=true";

        private static XmlDocument xmlDoc;
        private static DailyStandings dailyStandings;
        //private static Rootobject rootObj;

        private readonly string ExeDirectory;

        public NBAStatsForm()
        {
            InitializeComponent();

            this.ExeDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        private async void btnGenerateZipFile_Click(object sender, EventArgs e)
        {
            const string dailyStandingsUri = "http://stats.nba.com/stats/scoreboard?DayOffset=0&LeagueID=00&gameDate=";

            DateTime date = DateTime.Now;

            try
            {
                string directoryWithReportsPath = $"{this.ExeDirectory}Reports";
                string zipPath = $"{this.ExeDirectory}reports.zip";

                if (Directory.Exists(directoryWithReportsPath))
                {
                    Directory.Delete(directoryWithReportsPath, true);
                }

                if (File.Exists(zipPath))
                {
                    File.Delete(zipPath);
                }

                // If the directory already exists, this method does not create a new directory
                DirectoryInfo di = Directory.CreateDirectory(directoryWithReportsPath);

                var options = new Dictionary<string, string>();
                options["Referer"] = "http://stats.nba.com/scores/";

                await Task.Run(async () =>
                {
                    // get DailyStandings for last 10 days
                    for (int i = 0; i < 10; i++)
                    {
                        date = date.AddDays(-1);

                        string uriString = dailyStandingsUri + date.ToString("MM-dd-yyyy").Replace("-", "%2F");

                        await GetJsonObjFromNetworkFileAsync(uriString, Encoding.UTF8, options);

                        if (dailyStandings == null)
                        {
                            MessageBox.Show("Daily Standings url does not response with JSON file.");
                            return;
                        }

                        DateTime gameDate = DateTime.ParseExact(
                            dailyStandings.Parameters.GameDate,
                            "MM/dd/yyyy",
                            CultureInfo.InvariantCulture);

                        string directoryPath = this.ExeDirectory + "Reports\\" + gameDate.ToString("dd-MMM-yyyy");
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
                                    if (resultSet.Name == reportName)
                                    {
                                        foreach (var row in resultSet.rowSet)
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
                                            var homeRecord = (string)row[10]; //byte.Parse(((string)row[10]).Split(new char[] { '-' })[0]);
                                            var roadRecord = (string)row[11]; //byte.Parse(((string)row[11]).Split(new char[] { '-' })[0]);

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
                            }
                        }
                    }
                });

                ZipFile.CreateFromDirectory(directoryWithReportsPath, zipPath);

                // delete Reports directory
                di.Delete(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        private async Task GetJsonObjFromNetworkFileAsync(
            string uriString,
            Encoding encoding,
            IEnumerable options)
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

                using (Stream stream = await webClient.OpenReadTaskAsync(address))
                {
                    await Task.Run(() =>
                    {
                        //jsonObject = GetJsonObject(stream);
                        dailyStandings = stream.CreateFromJsonStream<DailyStandings>(Encoding.UTF8);
                        //rootObj = stream.CreateFromJsonStream<Rootobject>(Encoding.UTF8);
                    });
                }
            }
            catch (Exception)
            {
                dailyStandings = null;
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

        private void btnFillMongoDb_Click(object sender, EventArgs e)
        {

        }

        private void btnImportZipDataToSqlServer_Click(object sender, EventArgs e)
        {

        }
    }
}
