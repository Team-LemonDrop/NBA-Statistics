using Newtonsoft.Json;

namespace NBAStatistics.Reports.Pocos
{
    public class PointsPerGameReportPoco
    {
        [JsonProperty("player-id")]
        public int PlayerId { get; set; }

        [JsonProperty("player-name")]
        public string PlayerName { get; set; }

        [JsonProperty("team-name")]
        public string TeamName { get; set; }

        [JsonProperty("points-per-game")]
        public double PointPerGame { get; set; }
    }
}
