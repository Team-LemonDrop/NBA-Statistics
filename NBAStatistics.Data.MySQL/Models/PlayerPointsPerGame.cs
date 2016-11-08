namespace NBAStatistics.Data.MySQL.Models
{
    public class PlayerPointsPerGame
    {
        public int Id { get; set; }

        public int PlayerId { get; set; }

        public string PlayerName { get; set; }

        public string TeamName { get; set; }

        public double PointsPerGame { get; set; }
    }
}
