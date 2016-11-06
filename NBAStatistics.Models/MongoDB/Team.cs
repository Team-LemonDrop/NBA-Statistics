using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NBAStatistics.Data.FillMongoDB.Models
{
    public class Team : IEntity
    {
        public Team(
            int teamId,
            string name)
        {
            this.TeamId = teamId;
            this.Name = name;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int TeamId { get; private set; }

        public string Name { get; private set; }
    }
}
