using CLDV6212.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CLDV6212.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            

            return View();
        }

        public IActionResult Privacy()
        {
            var mediaUrl = "https://abcretailclvd.blob.core.windows.net/video/1092730069-preview.mp4";
            var model = new Media { MediaUrl = mediaUrl };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}