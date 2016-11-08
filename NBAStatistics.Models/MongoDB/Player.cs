using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System;
using System.Collections.Generic;

namespace NBAStatistics.Data.FillMongoDB.Models
{
    public class Player : IEntity
    {
        public Player(
            int playerId,
            string firstName,
            string lastName,
            string displayFirstLastName,
            string displayLastCommaFirstName,
            string displayFiLastName,
            DateTime birthDate,
            string school,
            string country,
            string lastAffiliation,
            string height,
            string weight,
            int seasonExp,
            string jersey,
            string position,
            string rosterStatus,
            int teamId,
            string teamName,
            string teamAbbreviation,
            string teamCode,
            string teamCity,
            string playerCode,
            int fromYear,
            int toYear,
            string dLeagueFlag,
            string gamesPlayedFlag,
            Dictionary<string, double> seasonPointsPerGame
            )
        {
            this.PlayerId = playerId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DisplayFirstLastName = displayFirstLastName;
            this.DisplayLastCommaFirstName = displayLastCommaFirstName;
            this.DisplayFiLastName = displayFiLastName;
            this.BirthDate = birthDate;
            this.School = school;
            this.Country = country;
            this.LastAffiliation = lastAffiliation;
            this.Height = height;
            this.Weight = weight;
            this.SeasonExp = seasonExp;
            this.Jersey = jersey;
            this.Position = position;
            this.RosterStatus = rosterStatus;
            this.TeamId = teamId;
            this.TeamName = teamName;
            this.TeamAbbreviation = teamAbbreviation;
            this.TeamCode = teamCode;
            this.TeamCity = teamCity;
            this.PlayerCode = playerCode;
            this.FromYear = fromYear;
            this.ToYear = toYear;
            this.DLeagueFlag = dLeagueFlag;
            this.GamesPlayedFlag = gamesPlayedFlag;
            this.SeasonPointsPerGame = seasonPointsPerGame;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.Int32)]
        public int PlayerId { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public string DisplayFirstLastName { get; private set; }

        public string DisplayLastCommaFirstName { get; private set; }

        public string DisplayFiLastName { get; private set; }

        public DateTime BirthDate { get; private set; }

        public string School { get; private set; }

        public string Country { get; private set; }

        public string LastAffiliation { get; private set; }

        public string Height { get; private set; }

        public string Weight { get; private set; }

        [BsonRepresentation(BsonType.Int32)]
        public int SeasonExp { get; private set; }

        public string Jersey { get; private set; }

        public string Position { get; private set; }

        public string RosterStatus { get; private set; }

        [BsonRepresentation(BsonType.Int32)]
        public int TeamId { get; private set; }

        public string TeamName { get; private set; }

        public string TeamAbbreviation { get; private set; }

        public string TeamCode { get; private set; }

        public string TeamCity { get; private set; }

        public string PlayerCode { get; private set; }

        [BsonRepresentation(BsonType.Int32)]
        public int FromYear { get; private set; }

        [BsonRepresentation(BsonType.Int32)]
        public int ToYear { get; private set; }

        public string DLeagueFlag { get; private set; }

        public string GamesPlayedFlag { get; private set; }

        [BsonDictionaryOptions(DictionaryRepresentation.ArrayOfDocuments)]
        public Dictionary<string, double> SeasonPointsPerGame { get; set; }
    }
}
