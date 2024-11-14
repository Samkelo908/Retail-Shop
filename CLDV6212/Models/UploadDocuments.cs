using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CLDV6212.Models
{
    public class UploadDocuments
    {
        [Required]
        public IFormFile ProductDocument { get; set; }
    }
}
