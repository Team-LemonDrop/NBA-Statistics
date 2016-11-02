namespace NBAStatistics.Models
{
    public class Image
    {
        public int Id { get; set; }

        public byte[] Content { get; set; }

        public string FileName { get; set; }

        public string Type { get; set; }
    }
}
