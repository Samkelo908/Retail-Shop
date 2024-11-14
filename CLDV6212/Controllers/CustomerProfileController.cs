using CLDV6212.Models;
using CLDV6212.Models.CLDV6212.Models;
using CLDV6212.Services;
using Microsoft.AspNetCore.Mvc;

namespace CLDV6212.Controllers
{
    // Controller for managing customer profiles
    public class CustomerProfileController : Controller
    {
        private readonly BlobService _blobService; // Manages image storage
        private readonly TableStorageServices _tableStorageServices; // Manages customer profile storage

        public CustomerProfileController(BlobService blobService, TableStorageServices tableStorageServices)
        {
            _blobService = blobService;
            _tableStorageServices = tableStorageServices;
        }

        // Display list of customer profiles
        public async Task<IActionResult> Index()
        {
            var customers = await _tableStorageServices.GetAllCustomerProfilesAsync();
            return View(customers);
        }

        // Add a new customer profile with an optional image
        [HttpPost]
        public async Task<IActionResult> AddCustomerProfile(CustomerProfile profile, IFormFile file)
        {
            if (file != null)
            {
                using var stream = file.OpenReadStream();
                profile.ImageUrl = await _blobService.UploadAsync(stream, file.FileName); // Upload image
            }

            if (ModelState.IsValid)
            {
                profile.PartitionKey = "CustomerPartition";
                profile.RowKey = Guid.NewGuid().ToString();

                // Assign a unique Customer_Id
                var existingProfiles = await _tableStorageServices.GetAllCustomerProfilesAsync();
                profile.Customer_Id = existingProfiles.Any() ? existingProfiles.Max(c => c.Customer_Id) + 1 : 1;

                await _tableStorageServices.AddCustomerProfileAsync(profile); // Save profile
                return RedirectToAction("Index");
            }

            return View(profile);
        }

        // Delete a customer profile and associated image
        [HttpPost]
        public async Task<IActionResult> DeleteCustomerProfile(string partitionKey, string rowKey, CustomerProfile profile)
        {
            if (profile != null && !string.IsNullOrEmpty(profile.ImageUrl))
            {
                await _blobService.DeleteBlobAsync(profile.ImageUrl); // Delete image from Blob
            }

            await _tableStorageServices.DeleteCustomerProfileAsync(partitionKey, rowKey); // Delete profile
            return RedirectToAction("Index");
        }

        // Display the form to add a new customer profile
        [HttpGet]
        public IActionResult AddCustomerProfile()
        {
            return View();
        }
    }
}
