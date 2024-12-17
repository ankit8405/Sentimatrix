using System;

namespace SentimatrixAPI.Models
{
    public class ProcessedEmail
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public int SentimentScore { get; set; }
        public string Response { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }

        public ProcessedEmail()
        {
            ProcessedAt = DateTime.UtcNow;
        }
    }
}
