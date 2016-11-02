using System.ComponentModel.DataAnnotations;

namespace NBAStatistics.Models
{
    public class Arena
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public int? Capacity { get; set; }

        public virtual Team Team { get; set; }
    }
}
