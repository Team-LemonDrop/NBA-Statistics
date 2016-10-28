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

namespace NBA_Stats
{
    public partial class NBAStatsForm : Form
    {
        private const string SHOOTING_ADDRESS = "http://stats.nba.com/js/data/sportvu/shootingData.js";

        private const string GAMES = "http://stats.nba.com/stats/shotchartdetail?Season=2013-14&SeasonType=Regular+Season&LeagueID=00&TeamID=1610612743&PlayerID=0&GameID=0021300605&Outcome=&Location=&Month=0&SeasonSegment=&DateFrom=&DateTo=&OpponentTeamID=0&VsConference=&VsDivision=&Position=&RookieYear=&GameSegment=&Period=0&LastNGames=0&ContextFilter=&ContextMeasure=FG_PCT&display-mode=performance&zone-mode=zone&zoneOverlays=false&zoneDetails=false&viewShots=true";

        private static XmlDocument xmlDoc;
        private static JObject jsonObj;
        //private static Rootobject rootObj;

        public NBAStatsForm()
        {
            InitializeComponent();
        }

        private async void btnGenerateZipFile_Click(object sender, EventArgs e)
        {
            //await GetJsonObjFromNetworkFileAsync(GAMES, Encoding.ASCII);

            //Task t = GetJsonObjFromNetworkFileAsync(GAMES, Encoding.ASCII);

            //t.Wait();




            var dbContext = new NBAStatisticsEntities();

            dbContext.SaveChanges();
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

        private async Task GetJsonObjFromNetworkFileAsync(string uriString, Encoding encoding)
        {
            try
            {
                var webClient = new WebClient() { Encoding = encoding };

                using (Stream stream = await webClient.OpenReadTaskAsync(new Uri(uriString, UriKind.Absolute)))
                {
                    //jsonObj = GetJsonObject(stream);
                    //jsonObj = stream.CreateFromJsonStream<JObject>(Encoding.UTF8);
                    //rootObj = stream.CreateFromJsonStream<Rootobject>(Encoding.UTF8);

                    //// Just loop
                    //long ctr = 0;
                    //for (ctr = 0; ctr <= 10000000000; ctr++)
                    //{ }

                    //MessageBox.Show("Finished");
                }
            }
            catch (Exception)
            {
                jsonObj = null;
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
    }
}
