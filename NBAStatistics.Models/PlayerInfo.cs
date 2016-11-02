using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAStatistics.Models
{
    public class PlayerInfo
    {
        [Column("Birthday")]
        public DateTime? Birthday { get; set; }

        [Column("Height")]
        public double Height { get; set; }

        [Column("Weight")]
        public double Weight { get; set; }

        [Column("CityId")]
        public int CityId { get; set; }

        public virtual City City { get; set; }

        [Column("SchoolId")]
        public int SchoolId { get; set; }

        public virtual School School { get; set; }
    }
}
