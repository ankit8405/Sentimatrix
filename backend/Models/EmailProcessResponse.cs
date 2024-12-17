using System.ComponentModel.DataAnnotations;

namespace SentimatrixAPI.Models
{
    /// <summary>
    /// Response model for email processing
    /// </summary>
    public class EmailProcessResponse
    {
        /// <summary>
        /// Status of the email processing (success/error)
        /// </summary>
        [Required]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Detailed message about the processing result
        /// </summary>
        [Required]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// The processed email data (if successful)
        /// </summary>
        public EmailResponseData Data { get; set; } = new();
    }

    public class EmailResponseData
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public int SentimentScore { get; set; }
        public string Response { get; set; } = string.Empty;
    }
}
