using System;
using System.Collections.Generic;

namespace NBAStatistics.Models
{
    public class Season
    {
        private ICollection<PlayerSeasonPointsPerGame> players;

        public Season()
        {
            this.players = new HashSet<PlayerSeasonPointsPerGame>();
        }

        public int Id { get; set; }

        public string SeasonId { get; set; }

        //public ushort StartYear { get; set; }

        //public ushort EndYear { get; set; }

        public virtual ICollection<PlayerSeasonPointsPerGame> Players
        {
            get { return this.players; }
            set { this.players = value; }
        }
    }
}
