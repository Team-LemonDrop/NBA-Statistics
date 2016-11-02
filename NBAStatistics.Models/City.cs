using System.Collections.Generic;

namespace NBAStatistics.Models
{
    public class City
    {
        private ICollection<Player> players;
        private ICollection<School> schools;
        private ICollection<Team> teams;

        public City()
        {
            this.players = new HashSet<Player>();
            this.schools = new HashSet<School>();
            this.teams = new HashSet<Team>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public int? CountryId { get; set; }

        public virtual Country Country { get; set; }

        public virtual ICollection<Player> Players
        {
            get { return this.players; }
            set { this.players = value; }
        }

        public virtual ICollection<School> Schools
        {
            get { return this.schools; }
            set { this.schools = value; }
        }

        public virtual ICollection<Team> Teams
        {
            get { return this.teams; }
            set { this.teams = value; }
        }
    }
}
