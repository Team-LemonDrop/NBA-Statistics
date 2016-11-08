using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAStatistics.Models
{
    public class City
    {
        private ICollection<School> schools;
        private ICollection<Team> teams;

        public City()
        {
            this.schools = new HashSet<School>();
            this.teams = new HashSet<Team>();
        }

        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("City")]
        public string Name { get; set; }

        public int CountryId { get; set; }

        public virtual Country Country { get; set; }

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
