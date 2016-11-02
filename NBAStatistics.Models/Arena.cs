using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NBAStatistics.Models
{
    public class Arena
    {
        [ForeignKey("Team")]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        public int? Capacity { get; set; }

        public virtual Team Team { get; set; }
    }
}
