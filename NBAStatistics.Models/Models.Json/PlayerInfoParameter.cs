using Newtonsoft.Json;

namespace NBAStatistics.Models.Models.Json
{
    public class PlayerInfoParameter
    {
        [JsonProperty("playerid")]
        public int PlayerID { get; set; }

        [JsonProperty("leagueid")]
        public object LeagueID { get; set; }
    }
}
