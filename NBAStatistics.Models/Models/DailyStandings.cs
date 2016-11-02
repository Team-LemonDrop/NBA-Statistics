using Newtonsoft.Json;

namespace NBAStatistics.Models.Models
{
    public class DailyStandings
    {
        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("parameters")]
        public DailyStandingsParameters Parameters { get; set; }

        [JsonProperty("resultsets")]
        public Resultset[] ResultSets { get; set; }
    }
}
