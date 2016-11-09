namespace NBAStatistics.Reports.Pocos
{
    public class TeamSuccessRatePoco
    {
        public string TeamName { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int Games { get; set; }

        public double SuccessRate { get; set; }

        public byte HomeRecord { get; set; }
    }
}
