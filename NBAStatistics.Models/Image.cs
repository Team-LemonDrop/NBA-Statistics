using System.ComponentModel.DataAnnotations;

namespace NBAStatistics.Models
{
    public class Image
    {
        public int Id { get; set; }

        [Required]
        public byte[] Content { get; set; }

        [Required]
        [MaxLength(10)]
        public string Extension { get; set; }
    }
}
