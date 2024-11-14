using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace CLDV6212.Models
{
    public class Product : ITableEntity
    {
        public int Product_Id { get; set; }
        public string Product_Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public string AvailabilityStatus { get; set; }
    
    public string? Customer_Name { get; set; }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }
}

