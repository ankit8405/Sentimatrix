namespace SentimatrixAPI.Models
{
    public class GroqRequest
    {
        public string Model { get; set; } = "llama2-70b-4096";
        public List<Message> Messages { get; set; } = new();
        public double Temperature { get; set; } = 0.7;
    }

    public class Message
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
