using Azure;
using Azure.Data.Tables;

namespace CLDV6212.Models
{
    public class User : ITableEntity
    {
        public int Id { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; } // Changed to string
        public string Address { get; set; }
        public string ImageUrl { get; set; } // Path of the uploaded image
        public string Password { get; set; }
        public bool IsStaff { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}
