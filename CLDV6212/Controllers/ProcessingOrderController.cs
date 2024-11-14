using CLDV6212.Models;
using CLDV6212.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CLDV6212.Controllers
{
    public class ProcessingOrderController : Controller
    {
        private readonly string _connectionString = "Data Source=abcretailpart3.database.windows.net;Initial Catalog=abcRetailer; User ID =ST10141464; Password=SamRock28";


        public async Task<IActionResult> Index()
        {
            var processOrders = await GetProcessOrdersAsync();
            return View(processOrders);
        }


        // GET: Register
        public async Task<IActionResult> Register()
        {
            // Fetch users and products using the methods that query the database
            var users = await GetUsersAsync();
            var products = await GetProductsAsync();

            // Populate ViewBag with data
            ViewBag.Product = new SelectList(products, "Product_Id", "Product_Name");
            ViewBag.User = new SelectList(users, "Id", "Name");

            return View();
        }

        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(ProcessOrders processOrder)
        {
            // No validation check
            processOrder.Process_Date = DateTime.SpecifyKind(processOrder.Process_Date, DateTimeKind.Utc);
            processOrder.Status = "Pending";

            await AddProcessOrderAsync(processOrder);

            // Redirect after successfully saving the process order
            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> DeleteProcessProfile(int processingId)
        {
            await DeleteProcessOrderAsync(processingId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int processing_Id, string Status) 
        {
            await UpdateProcessOrderStatusAsync(processing_Id, Status);
            return RedirectToAction("Index");
        }


        private async Task<List<User>> GetUsersAsync()
        {
            var users = new List<User>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT Id, Name FROM Users", connection);
                connection.Open();
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
            return users;
        }

        private async Task<List<Product>> GetProductsAsync()
        {
            var products = new List<Product>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT ProductID, Product_Name FROM Product", connection);
                connection.Open();
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    products.Add(new Product
                    {
                        Product_Id = reader.GetInt32(0),
                        Product_Name = reader.GetString(1)
                    });
                }
            }
            return products;
        }


        private async Task<List<ProcessOrders>> GetProcessOrdersAsync()
        {
            var processOrders = new List<ProcessOrders>();
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT Processing_Id, Id, ProductID, Process_Date, Process_Location, Status FROM ProcessOrders", connection);
                connection.Open();
                var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    processOrders.Add(new ProcessOrders
                    {
                        Processing_Id = reader.GetInt32(0),
                        Id = reader.GetInt32(1),
                        ProductID = reader.GetInt32(2),
                        Process_Date = reader.GetDateTime(3),
                        Process_Location = reader.GetString(4),
                        Status = reader.GetString(5)
                    });
                }
            }
            return processOrders;
        }

        private async Task AddProcessOrderAsync(ProcessOrders processOrder)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO ProcessOrders (Id, ProductID, Process_Date, Process_Location, Status) VALUES (@Id, @ProductID, @Process_Date, @Process_Location, @Status)", connection);
                command.Parameters.AddWithValue("@Id", processOrder.Id);
                command.Parameters.AddWithValue("@ProductID", processOrder.ProductID);
                command.Parameters.AddWithValue("@Process_Date", processOrder.Process_Date);
                command.Parameters.AddWithValue("@Process_Location", processOrder.Process_Location);
                command.Parameters.AddWithValue("@Status", processOrder.Status);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task DeleteProcessOrderAsync(int processingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM ProcessOrders WHERE Processing_Id = @Processing_Id", connection);
                command.Parameters.AddWithValue("@Processing_Id", processingId);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateProcessOrderStatusAsync(int processing_Id, string Status)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var command = new SqlCommand("UPDATE ProcessOrders SET Status = @Status WHERE Processing_Id = @Processing_Id", connection);

                
                command.Parameters.AddWithValue("@Processing_Id", processing_Id);
                command.Parameters.AddWithValue("@Status", Status);

                await command.ExecuteNonQueryAsync();
            }
        }


    }

}





// W3schools.com.ASP tutorial.Available at: https://www.w3schools.com/asp/default.ASP (Accessed: August 30, 2024).


//Concepts, P. R. O. (no date) Part 49. Dropdown using SelectListItem in .NET core MVC. | group | multi select | disable item |. Youtube. Available at: https://www.youtube.com/watch?v=QHh39YnufJY (Accessed:28 August 2024)