using Newtonsoft.Json;

namespace NBAStatistics.Models.Models.Json
{
    public class DailyStandingsParameters
    {
        [JsonProperty("gamedate")]
        public string GameDate { get; set; }

        [JsonProperty("leagueid")]
        public string LeagueId { get; set; }

        [JsonProperty("dayoffset")]
        public string DayOffset { get; set; }
    }
}
