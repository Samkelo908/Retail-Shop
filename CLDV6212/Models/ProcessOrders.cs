using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace CLDV6212.Models
{
    public class ProcessOrders
    {
        public int Processing_Id { get; set; }
        public int Id { get; set; }  // User Id
        public int ProductID { get; set; }  // Product Id
        public DateTime Process_Date { get; set; }
        public string Process_Location { get; set; }
        public string Status { get; set; }

       
        public User User { get; set; }
        public Product Product { get; set; }
    }
}