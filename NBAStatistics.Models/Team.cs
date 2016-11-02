using System;
using System.Collections.Generic;

namespace NBAStatistics.Models
{
    public class Team
    {
        private ICollection<StandingsByDay> standingsByDays;
        private ICollection<Player> players;
        private ICollection<Player> oldPlayers;

        public Team()
        {
            this.players = new HashSet<Player>();
            this.standingsByDays = new HashSet<StandingsByDay>();
            this.oldPlayers = new HashSet<Player>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public DateTime Founded { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }

        public int? ArenaId { get; set; }

        public virtual Arena Arena { get; set; }

        public int HeadCoachId { get; set; }

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

        public virtual ICollection<Player> OldPlayers
        {
            get { return this.oldPlayers; }
            set { this.oldPlayers = value; }
        }
    }
}
