using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SentimatrixAPI.Models
{
    public class EmailData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("body")]
        public string Body { get; set; } = string.Empty;

        [BsonElement("subject")]
        public string Subject { get; set; } = string.Empty;

        [BsonElement("sender")]
        public string SenderEmail { get; set; } = string.Empty;

        [BsonElement("receiver")]
        public string ReceiverEmail { get; set; } = string.Empty;

        [BsonElement("score")]
        public int Score { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } = string.Empty;

        [BsonElement("time")]
        public DateTime ReceivedDate { get; set; } = DateTime.UtcNow;
    }
}