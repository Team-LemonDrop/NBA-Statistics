using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAStatistics.Models
{
    public class Player
    {
        private ICollection<PlayersSeasons> playedSeasons;

        public Player()
        {
            this.playedSeasons = new HashSet<PlayersSeasons>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [NotMapped]
        public string FirstLastName => $"{this.FirstName} {this.LastName}";

        [NotMapped]
        public string LastCommaFirstName => $"{this.LastName}, {this.FirstName}";

        public PlayerInfo AdditionalInfo { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }

        public int SchoolId { get; set; }

        public virtual School School { get; set; }

        public string Position { get; set; }

        public bool? IsActive { get; set; }

        public int? TeamId { get; set; }

        public virtual Team Team { get; set; }

        public virtual ICollection<PlayersSeasons> PlayedSeasons
        {
            get { return this.playedSeasons; }
            set { this.playedSeasons = value; }
        }
    }
}
