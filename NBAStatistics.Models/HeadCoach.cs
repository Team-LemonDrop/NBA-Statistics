namespace NBAStatistics.Models
{
    public class HeadCoach
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public virtual Team Team { get; set; }
    }
}
