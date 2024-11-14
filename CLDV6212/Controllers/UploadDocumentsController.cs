using CLDV6212.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace CLDV6212.Controllers
{
    public class UploadDocumentsController : Controller
    {
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Index(UploadDocuments model)
        {
            if (ModelState.IsValid && model.ProductDocument != null)
            {
                await SaveFile(model.ProductDocument);
                TempData["Message"] = "File uploaded successfully!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Please upload a valid file.";
            return View(model);
        }

        private async Task SaveFile(IFormFile file)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }
    }
}
