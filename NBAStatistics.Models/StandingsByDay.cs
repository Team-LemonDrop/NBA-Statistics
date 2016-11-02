using System;

namespace NBAStatistics.Models
{
    public class StandingsByDay
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int SeasonId { get; set; }

        public byte Games { get; set; }

        public byte Wins { get; set; }

        public byte Loses { get; set; }

        public double SuccessRate { get; set; }

        public byte HomeRecord { get; set; }

        public byte RoadRecord { get; set; }

        public int ConferenceId { get; set; }

        public virtual Conference Conference { get; set; }

        public int TeamId { get; set; }

        public virtual Team Team { get; set; }
    }
}
