using Newtonsoft.Json;

namespace NBAStatistics.Models.Models.Json
{
    public class PlayerInfo
    {
        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("parameters")]
        public PlayerInfoParameter[] Parameters { get; set; }

        [JsonProperty("resultsets")]
        public Resultset[] ResultSets { get; set; }
    }
}
