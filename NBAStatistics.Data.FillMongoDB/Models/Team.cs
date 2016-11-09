using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NBAStatistics.Data.FillMongoDB.Models
{
    public class Team : IEntity
    {
        public Team(
            int teamId,
            string name,
            string abbreviation,
            string logoSrc,
            string headCoach,
            int founded,
            string arena,
            string city,
            string country)
        {
            this.TeamId = teamId;
            this.Name = name;
            this.Abbreviation = abbreviation;
            this.LogoSrc = logoSrc;
            this.HeadCoach = headCoach;
            this.Founded = founded;
            this.Arena = arena;
            this.City = city;
            this.Country = country;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int TeamId { get; private set; }

        public string Name { get; private set; }

        public string Abbreviation { get; private set; }

        public string LogoSrc { get; private set; }

        public string HeadCoach { get; private set; }

        [BsonRepresentation(BsonType.Int32)]
        public int Founded { get; private set; }

        public string Arena { get; private set; }

        public string City { get; private set; }

        public string Country { get; private set; }
    }
}
