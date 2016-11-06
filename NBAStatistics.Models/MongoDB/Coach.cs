using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NBAStatistics.Data.FillMongoDB.Models
{
    public class Coach : IEntity
    {
        public Coach(
            int teamId,
            string season,
            string coachId,
            string firstName,
            string lastName,
            string coachName,
            string coachCode,
            int isAssistant,
            string coachType,
            string school,
            int? sortSequence
            )
        {
            this.TeamId = teamId;
            this.Season = season;
            this.CoachId = coachId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.CoachName = coachName;
            this.CoachCode = coachCode;
            this.IsAssistant = isAssistant;
            this.CoachType = coachType;
            this.School = school;
            this.SortSequence = sortSequence;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int TeamId { get; private set; }

        public string Season { get; private set; }

        public string CoachId { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string CoachName { get; private set; }

        public string CoachCode { get; private set; }

        [BsonRepresentation(BsonType.Int32)]
        public int IsAssistant { get; private set; }

        public string CoachType { get; private set; }

        public string School { get; private set; }

        public int? SortSequence { get; private set; }
    }
}
