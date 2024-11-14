using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace CLDV6212.Models
{
    namespace CLDV6212.Models
    {
        public class CustomerProfile : ITableEntity
        {
            [Key]
            public int Customer_Id { get; set; }

            public string? Customer_Name { get; set; }
            public string Customer_Surname { get; set; }
            public string Customer_Email { get; set; }

            public int Customer_Phone { get; set; }

            public string? Description { get; set; }

            public string? ImageUrl { get; set; }

            public string? Address { get; set; }

            public string? PartitionKey { get; set; }

            public string? RowKey { get; set; }

            public ETag ETag { get; set; }

            public DateTimeOffset? Timestamp { get; set; }
            public string Phone { get; set; } // Changed to string
            
            public string Password { get; set; }
            public bool IsStaff { get; set; }
        }
    }
}