using Microsoft.AspNetCore.Mvc;
using SentimatrixAPI.Models;
using SentimatrixAPI.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace SentimatrixAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly GroqService _groqService;

        public EmailController(EmailService emailService, GroqService groqService)
        {
            _emailService = emailService;
            _groqService = groqService;
        }

        [HttpPost("process")]
        public async Task<ActionResult<Email>> ProcessEmail([FromBody] EmailData emailData)
        {
            try
            {
                if (string.IsNullOrEmpty(emailData.Body))
                {
                    return BadRequest("Email body cannot be empty");
                }

                // Analyze sentiment using Groq
                var sentimentScore = await _groqService.AnalyzeSentiment(emailData.Body);
                
                // Create new email document
                var email = new Email
                {
                    Body = emailData.Body,
                    Score = sentimentScore,
                    Sender = emailData.SenderEmail ?? "",
                    Receiver = emailData.ReceiverEmail ?? "",
                    Type = sentimentScore <= 50 ? "positive" : "negative",
                    Time = DateTime.UtcNow
                };

                // Save to MongoDB
                await _emailService.CreateAsync(email);

                return Ok(email);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<Email>>> GetAllEmails()
        {
            var emails = await _emailService.GetAsync();
            return Ok(emails);
        }

        [HttpGet("positive")]
        public async Task<ActionResult<List<Email>>> GetPositiveEmails()
        {
            var emails = await _emailService.GetPositiveEmailsAsync();
            return Ok(emails);
        }

        [HttpGet("negative")]
        public async Task<ActionResult<List<Email>>> GetNegativeEmails()
        {
            var emails = await _emailService.GetNegativeEmailsAsync();
            return Ok(emails);
        }

        [HttpGet("sender/{email}")]
        public async Task<ActionResult<List<Email>>> GetEmailsBySender(string email)
        {
            var emails = await _emailService.GetEmailsBySenderAsync(email);
            return Ok(emails);
        }
    }
}
