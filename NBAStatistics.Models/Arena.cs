namespace NBAStatistics.Models
{
    public class Arena
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int? Capacity { get; set; }

        public virtual Team Team { get; set; }
    }
}
