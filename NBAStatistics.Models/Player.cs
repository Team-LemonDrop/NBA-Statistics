using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAStatistics.Models
{
    public class Player
    {
        private ICollection<PlayerSeasonPointsPerGame> playedSeasons;

        public Player()
        {
            this.playedSeasons = new HashSet<PlayerSeasonPointsPerGame>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [NotMapped]
        public string FirstLastName => $"{this.FirstName} {this.LastName}";

        [NotMapped]
        public string LastCommaFirstName => $"{this.LastName}, {this.FirstName}";

        public PlayerInfo AdditionalInfo { get; set; }

        public int? SchoolId { get; set; }

        public virtual School School { get; set; }

        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        public string Position { get; set; }

        public string RosterStatus { get; set; }

        public int TeamId { get; set; }

        public virtual Team Team { get; set; }

        public virtual ICollection<PlayerSeasonPointsPerGame> PlayedSeasons
        {
            get { return this.playedSeasons; }
            set { this.playedSeasons = value; }
        }
    }
}
