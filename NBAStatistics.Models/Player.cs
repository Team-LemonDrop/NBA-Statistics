using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAStatistics.Models
{
    public partial class Player
    {
        private ICollection<Team> teams;

        public Player()
        {
            this.teams = new HashSet<Team>();
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

        public string Position { get; set; }

        public bool? IsActive { get; set; }

        public int? TeamId { get; set; }

        public virtual Team Team { get; set; }

        public virtual ICollection<Team> Teams
        {
            get { return this.teams; }
            set { this.teams = value; }
        }
    }
}
