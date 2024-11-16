using CLDV6212.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace CLDV6212.Controllers
{
    public class UserController : Controller
    {
        // Fields for HttpClient, Logger, and connection strings
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserController> _logger;
        private readonly string _functionUrl = "";
        string connectionString = "";

        // Constructor for injecting dependencies
        public UserController(HttpClient httpClient, ILogger<UserController> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        // Fetch and display all users from the database
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Getting all users.");
            var users = new List<User>();

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Users"; //Glynn Rudman.2024.
                using (var command = new SqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        users.Add(new User
                        {
                            Id = Convert.ToInt32(reader["Id"]), //Glynn Rudman.2024.
                            Name = reader["Name"].ToString(),
                            Surname = reader["Surname"].ToString(),
                            Email = reader["Email"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Address = reader["Address"].ToString(),
                            ImageUrl = reader["ImageUrl"].ToString()
                        });
                    }
                }
            }

            return View(users);
        }

        // Display form to add a new user
        public IActionResult AddUser()
        {
            return View();
        }

        // Add a new user with an image upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUser(User model, IFormFile userImage)
        {
            try //W3schools.com.2000
            {
                // Check for a valid image
                if (userImage == null || userImage.Length == 0)
                {
                    TempData["ErrorMessage"] = "Please upload a user image.";
                    return View(model);
                }

                // Upload image to Blob storage and get its URL
                var imageUrl = await UploadImageToBlobAsync(userImage);
                model.ImageUrl = imageUrl;

                // Send user data to Azure Function
                var response = await _httpClient.PostAsJsonAsync(_functionUrl, model);

                // Check if the user was successfully added
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "User added successfully!";
                    _logger.LogInformation("User added successfully.");
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add user. Please try again.";
                    _logger.LogError($"Failed to add user. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                _logger.LogError($"Exception occurred: {ex.Message}");
            }

            return View(model);
        }

        // Display login form
        public IActionResult Login()
        {
            return View();
        }

        // Authenticate user login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(User model)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Users WHERE Email = @Email AND Password = @Password"; //Glynn Rudman.2024
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@Password", model.Password);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        // If valid user, set up authentication
                        if (await reader.ReadAsync()) //W3schools.com.2000
                        {
                            var role = reader["Staff"].ToString().ToLower() == "yes" ? "Admin" : "User";

                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Name, model.Email),
                                new Claim(ClaimTypes.Role, role)
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                            return RedirectToAction("Index", "Home");
                        }
                        else //W3schools.com.2000
                        {
                            TempData["ErrorMessage"] = "Invalid email or password.";
                        }
                    }
                }
            }

            return View(model);
        }

        // Logout the user
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Delete a user by ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string query = "DELETE FROM Users WHERE Id = @Id";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            TempData["SuccessMessage"] = "User deleted successfully!";
                            _logger.LogInformation($"User with ID {id} deleted successfully.");
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "User not found.";
                            _logger.LogWarning($"User with ID {id} not found.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
                _logger.LogError($"Exception occurred while deleting user with ID {id}: {ex.Message}");
            }

            return RedirectToAction("Index");
        }

        // Upload image to Blob storage and return URL
        private async Task<string> UploadImageToBlobAsync(IFormFile file)
        {
            var connectionString = "";
            var blobContainerName = "";
            var blobClient = new BlobContainerClient(connectionString, blobContainerName);
            await blobClient.CreateIfNotExistsAsync();

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
