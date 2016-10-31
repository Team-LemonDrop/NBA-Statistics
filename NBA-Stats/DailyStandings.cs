using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBA_Stats
{
    public class DailyStandings
    {
        [JsonProperty("resource")]
        public string Resource { get; set; }

        [JsonProperty("parameters")]
        public Parameters Parameters { get; set; }

        [JsonProperty("resultsets")]
        public Resultset[] ResultSets { get; set; }
    }

    public class Parameters
    {
        [JsonProperty("gamedate")]
        public string GameDate { get; set; }

        [JsonProperty("leagueid")]
        public string LeagueId { get; set; }

        [JsonProperty("dayoffset")]
        public string DayOffset { get; set; }
    }

    public class Resultset
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("headers")]
        public string[] Headers { get; set; }

        [JsonProperty("rowset")]
        public object[][] rowSet { get; set; }
    }
}
