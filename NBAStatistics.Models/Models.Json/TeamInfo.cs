using Newtonsoft.Json;

namespace NBAStatistics.Models.Models.Json
{
    public class TeamInfo
    {
        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("parameters")]
        public TeamInfoParameter Parameters { get; set; }

        [JsonProperty("resultsets")]
        public Resultset[] ResultSets { get; set; }
    }
}
