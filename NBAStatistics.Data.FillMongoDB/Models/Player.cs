using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NBAStatistics.Data.FillMongoDB.Models
{
    public class Player : IEntity
    {
        public Player(
            int teamId,
            string season,
            string leagueId,
            string playerName,
            string num,
            string position,
            string height,
            string weight,
            string birthDate,
            int age,
            string exp,
            string school,
            int playerId)
        {
            this.TeamId = teamId;
            this.Season = season;
            this.LeagueId = leagueId;
            this.PlayerName = playerName;
            this.Num = num;
            this.Position = position;
            this.Height = height;
            this.Weight = weight;
            this.BirthDate = birthDate;
            this.Age = age;
            this.Exp = exp;
            this.School = school;
            this.PlayerId = playerId;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int TeamId { get; private set; }

        public string Season { get; private set; }

        public string LeagueId { get; private set; }

        public string PlayerName { get; private set; }

        public string Num { get; private set; }

        public string Position { get; private set; }

        public string Height { get; private set; }

        public string Weight { get; private set; }

        public string BirthDate { get; private set; }

        [BsonRepresentation(BsonType.Int32)]
        public int Age { get; private set; }

        public string Exp { get; private set; }

        public string School { get; private set; }

        [BsonRepresentation(BsonType.Int32)]
        public int PlayerId { get; private set; }
    }
}
