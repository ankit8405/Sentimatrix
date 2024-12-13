namespace SentimatrixAPI.Models
{
    public class DashboardStats
    {
        public int TotalEmails { get; set; }
        public int PositiveEmails { get; set; }
        public int NegativeEmails { get; set; }
        public double AverageSentimentScore { get; set; }
    }
}
