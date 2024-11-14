using CLDV6212.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Azure.Storage.Blobs;
using System.IO;
using System;
using System.Data.SqlClient;

namespace CLDV6212.Controllers
{
    public class ProductController : Controller
    {
        
        private readonly HttpClient _httpClient;
        // Logger to log information and errors
        private readonly ILogger<ProductController> _logger;
        
        private readonly string _functionUrl = "https://abcretailerfunction.azurewebsites.net/api/AddProduct?code=6yJRxDO2RKyEPN3AFALk6_UKhjnlCUTQ788qQLyPUlZ7AzFuMkT8cg%3D%3D";
        // Connection string for SQL database
        string connectionString = "Data Source=abcretailpart3.database.windows.net;Initial Catalog=abcRetailer; User ID=ST10141464; Password=SamRock28";

        // Constructor initializes HttpClient and logger
        public ProductController(HttpClient httpClient, ILogger<ProductController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // GET: Index action to fetch and display all products
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Getting all products.");
            var products = new List<Product>();

            // Connect to SQL database to fetch all products
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Product"; //Glynn Rudman.2024
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    // Loop through each product and add it to the list
                    while (await reader.ReadAsync())
                    {
                        products.Add(new Product
                        {
                            Product_Name = reader["Product_Name"].ToString(), //Glynn Rudman.2024
                            Description = reader["Description"].ToString(),
                            Price = Convert.ToDecimal(reader["Price"]),
                            CategoryName = reader["CategoryName"].ToString(),
                            AvailabilityStatus = reader["AvailabilityStatus"].ToString(),
                            ImageUrl = reader["ImageUrl"].ToString()
                        });
                    }
                }
            }

            // Return the product list to the view
            return View(products);
        }

        // GET: Render the AddProduct form view
        public IActionResult AddProduct()
        {
            return View();
        }

        // POST: Handle form submission for adding a new product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct(Product model, IFormFile productImage)
        {
            try
            {
                // Check if an image file is uploaded
                if (productImage == null || productImage.Length == 0)
                {
                    TempData["ErrorMessage"] = "Please upload a product image.";
                    return View(model);
                }

                // Upload image to Blob Storage and get the image URL
                var imageUrl = await UploadImageToBlobAsync(productImage);
                model.ImageUrl = imageUrl;

                // Send product data to the Azure Function
                var response = await _httpClient.PostAsJsonAsync(_functionUrl, model);

                // Check if the product was added successfully
                if (response.IsSuccessStatusCode) //W3schools.com.2000
                {
                    TempData["SuccessMessage"] = "Product added successfully!";
                    _logger.LogInformation("Product added successfully.");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add product. Please try again.";
                    _logger.LogError($"Failed to add product. Status code: {response.StatusCode}");
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return View(model);
        }

        // DELETE: Remove a product by its name from the database and Blob Storage
        public async Task<IActionResult> DeleteProduct(string productName)
        {
            try
            {
                // Delete the product record from SQL database
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Product WHERE Product_Name = @ProductName"; //Glynn Rudman.2024
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ProductName", productName);
                        var rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            _logger.LogInformation($"Product '{productName}' deleted from SQL database.");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Product not found.";
                            return RedirectToAction("Index");
                        }
                    }
                }

                // Define Blob Storage connection details
                var blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=abcretailclvd;AccountKey=YMXj4YnPxNcKH7ddYBXMiEZXiybKEMbldwLLHhJMd83EUr44qtzDldYMPU56cn7B1w4pi7TsdZNT+AStHdLkNA==;EndpointSuffix=core.windows.net";
                var blobContainerName = "images1";

                // Delete the image file from Blob Storage
                var blobClient = new BlobContainerClient(blobConnectionString, blobContainerName);
                var blob = blobClient.GetBlobClient(productName);
                await blob.DeleteIfExistsAsync();

                TempData["SuccessMessage"] = "Product deleted successfully!";
            }
            catch (Exception ex) //W3schools.com.2000
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                _logger.LogError($"Exception occurred while deleting product: {ex.Message}");
            }

            return RedirectToAction("Index");
        }

        // Helper function to upload image to Blob Storage and get its URL
        private async Task<string> UploadImageToBlobAsync(IFormFile file)
        {
            // Define Blob Storage connection string and container name
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=abcretailclvd;AccountKey=YMXj4YnPxNcKH7ddYBXMiEZXiybKEMbldwLLHhJMd83EUr44qtzDldYMPU56cn7B1w4pi7TsdZNT+AStHdLkNA==;EndpointSuffix=core.windows.net";
            var blobContainerName = "images1";
            var blobClient = new BlobContainerClient(connectionString, blobContainerName);

            // Create the container if it doesn't exist
            await blobClient.CreateIfNotExistsAsync();

            // Upload the file and return its URL
            var blob = blobClient.GetBlobClient(file.FileName);
            await blob.UploadAsync(file.OpenReadStream(), overwrite: true);

            return blob.Uri.ToString();
        }
    }
}

//W3schools.com.2000.SQL tutorial .Available at: https://www.w3schools.com/sql/ (Accessed:  7 November 2024).
//W3schools.com.2000. C# if ... Else. Available at: https://www.w3schools.com/cs/cs_conditions.php (Accessed:10 November 2024).
//W3schools.com. 2000. C# exceptions (try..Catch). Available at: https://www.w3schools.com/cs/cs_exceptions.php (Accessed: 10 November 2024).
//Glynn Rudman.2024.BCA2 CLDV Part 2 Workshop.May 2024.Available at: https://youtu.be/I_tiFJ-nlfE?si=NFAGPv9rajwA2cqX (Accessed:8 November 2024).
