using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBA_Stats
{
    // name: EastConfStandingsByDay or WestConfStandingsByDay
    public class StandingsByDay
    {
        // headers: "TEAM_ID"
        [JsonProperty("team_id")]
        public int TeamId { get; set; }

        // headers: "LEAGUE_ID"
        [JsonProperty("league_id")]
        public string LeagueId { get; set; }

        // headers: "SEASON_ID"
        [JsonProperty("season_id")]
        public string SeasonId { get; set; }

        // headers: "STANDINGSDATE"
        [JsonProperty("standingsdate")]
        public DateTime StandingsDate { get; set; }

        // headers: "CONFERENCE"
        [JsonProperty("conference")]
        public string Conference { get; set; }

        // headers: "TEAM"
        [JsonProperty("team")]
        public string Team { get; set; }

        // headers: "G"
        [JsonProperty("games")]
        public int Games { get; set; }

        // headers: "W"
        [JsonProperty("wins")]
        public int Wins { get; set; }

        // headers: "L"
        [JsonProperty("losses")]
        public int Losses { get; set; }

        // headers: "W_PCT"
        [JsonProperty("w_pct")]
        public int WinningPercentage { get; set; }

        // headers: "HOME_RECORD"
        [JsonProperty("home_record")]
        public string HomeRecord { get; set; }

        // headers: "ROAD_RECORD"
        [JsonProperty("road_record")] 
        public string RoadRecord { get; set; }
    }
}
