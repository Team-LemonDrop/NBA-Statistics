using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAStatistics.Models
{
    [ComplexType]
    public class PlayerInfo
    {
        [Column("PlayerId-NBA")]
        public int PlayerId { get; set; }

        [Column("Birthday")]
        public DateTime? Birthday { get; set; }

        [Column("Height")]
        public double? Height { get; set; }

        [Column("Weight")]
        public double? Weight { get; set; }
    }
}