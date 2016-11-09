using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace NBAStatistics.Data.FillMongoDB.Models
{
    public class Season : IEntity
    {
        public Season(
            string seasonId,
            IEnumerable<Team> teams)
        {
            this.SeasonId = seasonId;
            this.Teams = teams;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        public string SeasonId { get; private set; }

        public IEnumerable<Team> Teams { get; private set; }
    }
}
