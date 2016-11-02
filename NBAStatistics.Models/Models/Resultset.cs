using Newtonsoft.Json;

namespace NBAStatistics.Models.Models
{
    public class Resultset
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("headers")]
        public string[] Headers { get; set; }

        [JsonProperty("rowset")]
        public object[][] RowSet { get; set; }
    }
}
