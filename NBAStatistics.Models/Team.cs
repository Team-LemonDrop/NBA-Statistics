using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAStatistics.Models
{
    public class Team
    {
        private ICollection<StandingsByDay> standingsByDays;
        private ICollection<Player> players;

        public Team()
        {
            this.players = new HashSet<Player>();
            this.standingsByDays = new HashSet<StandingsByDay>();
        }

        public int Id { get; set; }

        public int TeamId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string Abbreviation { get; set; }

        public DateTime? Founded { get; set; }

        public int? CityId { get; set; }

        public virtual City City { get; set; }

        public virtual Arena Arena { get; set; }

        public virtual HeadCoach HeadCoach { get; set; }

        public Image Logo { get; set; }

        public virtual ICollection<Player> Players
        {
            get { return this.players; }
            set { this.players = value; }
        }

        public virtual ICollection<StandingsByDay> StandingsByDays
        {
            get { return this.standingsByDays; }
            set { this.standingsByDays = value; }
        }
    }
}