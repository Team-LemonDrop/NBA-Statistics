using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBA_Stats
{
    public static class JsonHelpers
    {
        public static T CreateFromJsonStream<T>(this Stream stream, Encoding encoding)
        {
            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return JsonSerializer.Create().Deserialize<T>(jsonTextReader);
            }
        }

        public static T CreateFromJsonStream<T>(this Stream stream)
        {
            return CreateFromJsonStream<T>(stream, Encoding.Default);
        }

        public static T CreateFromJsonString<T>(this String json, Encoding encoding)
        {
            using (var stream = new MemoryStream(encoding.GetBytes(json)))
            {
                return CreateFromJsonStream<T>(stream, encoding);
            }
        }

        public static T CreateFromJsonString<T>(this String json)
        {
            return CreateFromJsonString<T>(json, Encoding.Default);
        }

        public static T CreateFromJsonFile<T>(this String fileName, Encoding encoding)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                return CreateFromJsonStream<T>(fileStream, encoding);
            }
        }

        public static T CreateFromJsonFile<T>(this String fileName)
        {
            return CreateFromJsonFile<T>(fileName, Encoding.Default);
        }
    }
}
