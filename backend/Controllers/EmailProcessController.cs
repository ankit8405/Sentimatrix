using Microsoft.AspNetCore.Mvc;
using SentimatrixAPI.Models;
using System.Web;
using HtmlAgilityPack;
using SentimatrixAPI.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using SentimatrixAPI.Hubs;
using System.IO;

namespace SentimatrixAPI.Controllers
{
    /// <summary>
    /// Controller for processing emails received from TruBot
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class EmailProcessController : ControllerBase
    {
        private readonly ILogger<EmailProcessController> _logger;
        private readonly GroqService _groqService;
        private readonly IHubContext<TicketHub> _hubContext;
        private readonly EmailService _emailService;
        private const string SERIOUS_EMAILS_PATH = "serious_emails.json";
        private const string POSITIVE_EMAILS_PATH = "positive_emails.json";

        public EmailProcessController(
            ILogger<EmailProcessController> logger,
            GroqService groqService,
            IHubContext<TicketHub> hubContext,
            EmailService emailService)
        {
            _logger = logger;
            _groqService = groqService;
            _hubContext = hubContext;
            _emailService = emailService;
        }

        /// <summary>
        /// Process an email received from TruBot
        /// </summary>
        /// <param name="emailData">The email data</param>
        /// <returns>Processing result with status and message</returns>
        /// <response code="200">Returns the processing result when successful</response>
        /// <response code="500">If there's an error processing the email</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmailProcessResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(EmailProcessResponse))]
        public async Task<IActionResult> ProcessEmail([FromForm] EmailData emailData)
        {
            try
            {
                if (emailData == null)
                {
                    return BadRequest(new EmailProcessResponse
                    {
                        Status = "Error",
                        Message = "Email data is required",
                        Data = null
                    });
                }

                _logger.LogInformation($"Processing email from: {emailData.SenderEmail}");

                var plainTextBody = ConvertHtmlToPlainText(emailData.Body ?? string.Empty);
                
                // Analyze sentiment using Groq
                int sentimentScore = await _groqService.AnalyzeSentiment(plainTextBody);
                string response = _groqService.GenerateResponse(sentimentScore, plainTextBody);

                // Create email document for MongoDB
                var email = new Email
                {
                    Body = plainTextBody,
                    Score = sentimentScore,
                    Sender = emailData.SenderEmail ?? string.Empty,
                    Receiver = emailData.ReceiverEmail ?? string.Empty,
                    Type = sentimentScore <= 60 ? "positive" : "negative",
                    Time = DateTime.UtcNow
                };

                // Store in MongoDB
                await _emailService.CreateAsync(email);

                // Store in local JSON files for backward compatibility
                await StoreEmail(new ProcessedEmail
                {
                    Subject = emailData.Subject ?? string.Empty,
                    Body = plainTextBody,
                    SenderEmail = emailData.SenderEmail ?? string.Empty,
                    SentimentScore = sentimentScore,
                    Response = response,
                    ProcessedAt = DateTime.UtcNow
                });

                // If sentiment score is high (negative), notify all connected clients
                if (sentimentScore > 60)
                {
                    await _hubContext.Clients.All.SendAsync("ReceiveSeriousTicket", email);
                }

                return Ok(new EmailProcessResponse
                {
                    Status = "Success",
                    Message = response,
                    Data = new EmailResponseData 
                    {
                        Subject = emailData.Subject,
                        Body = plainTextBody,
                        SenderEmail = emailData.SenderEmail,
                        SentimentScore = sentimentScore,
                        Response = response
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing email: {ex.Message}");
                _logger.LogError($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new EmailProcessResponse
                {
                    Status = "Error",
                    Message = "Failed to process email",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Get serious tickets
        /// </summary>
        /// <returns>List of serious tickets</returns>
        /// <response code="200">Returns the list of serious tickets when successful</response>
        /// <response code="500">If there's an error retrieving the tickets</response>
        [HttpGet("serious-tickets")]
        public async Task<IActionResult> GetSeriousTickets()
        {
            try
            {
                // Get negative emails from MongoDB
                var tickets = await _emailService.GetNegativeEmailsAsync();
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving serious tickets: {ex.Message}");
                return StatusCode(500, new { message = "Error retrieving tickets" });
            }
        }

        private string ConvertHtmlToPlainText(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Remove script and style elements
            var nodes = doc.DocumentNode.SelectNodes("//script|//style");
            if (nodes != null)
            {
                foreach (var node in nodes)
                {
                    node.Remove();
                }
            }

            string plainText = doc.DocumentNode.InnerText;
            plainText = HttpUtility.HtmlDecode(plainText);
            plainText = plainText.Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            while (plainText.Contains("  ")) plainText = plainText.Replace("  ", " ");

            return plainText.Trim();
        }

        private async Task StoreEmail(ProcessedEmail email)
        {
            string filePath = email.SentimentScore > 60 ? SERIOUS_EMAILS_PATH : POSITIVE_EMAILS_PATH;
            
            List<ProcessedEmail> emails = new List<ProcessedEmail>();
            if (System.IO.File.Exists(filePath))
            {
                string jsonContent = await System.IO.File.ReadAllTextAsync(filePath);
                emails = JsonConvert.DeserializeObject<List<ProcessedEmail>>(jsonContent) ?? new List<ProcessedEmail>();
            }

            emails.Add(email);
            string updatedJson = JsonConvert.SerializeObject(emails, Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(filePath, updatedJson);
        }
    }
}
