namespace SentimatrixAPI.Models
{
    public class SentimentData
    {
        public string? Period { get; set; } = string.Empty; // Initialized to avoid nullability warning
        public double AverageScore { get; set; }
        public int Count { get; set; }
    }
}
