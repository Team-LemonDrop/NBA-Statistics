using System;

using NBAStatistics.Reports.Handlers.Contracts;
using Newtonsoft.Json;

namespace NBAStatistics.Reports
{
    public class JsonHandler : IJsonHandler
    {
        public T Deserialize<T>(string json) where T : class
        {
            if (string.IsNullOrEmpty(json))
            {
                throw new ArgumentException("Should provide a json object to perform desirialization!");
            }

            T value = JsonConvert.DeserializeObject<T>(json);

            return value;
        }

        public string Serialize<T>(T value) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException("Serializing null object is invalid!");
            }

            string json = JsonConvert.SerializeObject(value, Formatting.Indented);

            return json;
        }
    }
}
