using System;
using System.Collections.Generic;

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

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string DisplayFirstLastName { get; set; }

        public string DisplayLastCommaFirstName { get; set; }

        public DateTime? Birthday { get; set; }

        public int CityId { get; set; }

        public virtual City City { get; set; }

        public int SchoolId { get; set; }

        public virtual School School { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        public string Position { get; set; }

        public int? TeamId { get; set; }

        public bool? IsActive { get; set; }

        public virtual Team Team { get; set; }

        public virtual ICollection<Team> Teams
        {
            get { return this.teams; }
            set { this.teams = value; }
        }
    }
}
