using System;

namespace WpfSidebarApp.Models
{
    public class Email
    {
        public string Id { get; set; }
        public string Body { get; set; }
        public int Score { get; set; }
        public string SenderEmail { get; set; }
        public string ReceiverEmail { get; set; }
        public string Type { get; set; }
        public DateTime Time { get; set; }
    }
} 