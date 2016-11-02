using System.ComponentModel.DataAnnotations;

namespace NBAStatistics.Models
{
    public class HeadCoach
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public virtual Team Team { get; set; }
    }
}
