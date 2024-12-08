using System;

namespace SentimatrixAPI.Data
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string EmailsCollectionName { get; set; } = string.Empty;
    }
}
