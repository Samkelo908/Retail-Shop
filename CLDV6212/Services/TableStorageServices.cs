using Azure.Data.Tables;
using Azure;
using CLDV6212.Models.CLDV6212.Models;
using CLDV6212.Models;

namespace CLDV6212.Services
{
    public class TableStorageServices
    {
        private readonly TableClient _customerTableClient;
        private readonly TableClient _productTableClient;
        private readonly TableClient _userTableClient;
        private readonly TableClient _processOrderTableClient;



        public TableStorageServices(string connectionString, string tableName)
        {
            _customerTableClient = new TableClient(connectionString, "CustomerProfile");
            _productTableClient = new TableClient(connectionString, "Product");
            _processOrderTableClient = new TableClient(connectionString, "Processing");
            var serviceClient = new TableServiceClient(connectionString);
            _userTableClient = serviceClient.GetTableClient(tableName);
            _userTableClient.CreateIfNotExists();
        }



        public async Task<List<CustomerProfile>> GetAllCustomerProfilesAsync()
        {
            var customers = new List<CustomerProfile>();
            await foreach (var customer in _customerTableClient.QueryAsync<CustomerProfile>())
            {
                customers.Add(customer);
            }
            return customers;
        }

        public async Task AddCustomerProfileAsync(CustomerProfile profile)
        {
            if (string.IsNullOrEmpty(profile.PartitionKey) || string.IsNullOrEmpty(profile.RowKey))
            {
                throw new ArgumentException("Partition key must be set.");
            }

            try
            {
                await _customerTableClient.AddEntityAsync(profile);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException("Error adding entity to table storage", ex);
            }
        }

        public async Task DeleteCustomerProfileAsync(string partitionKey, string rowKey)
        {
            await _customerTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

     

        public async Task AddProductAsync(Product product)
        {
            if (string.IsNullOrEmpty(product.PartitionKey) || string.IsNullOrEmpty(product.RowKey))
            {
                throw new ArgumentException("Partition key and row key must be set.");
            }

            try
            {
                await _productTableClient.AddEntityAsync(product);
            }
            catch (RequestFailedException ex)
            {
                throw new InvalidOperationException("Error adding entity to table storage", ex);
            }
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            await foreach (var product in _productTableClient.QueryAsync<Product>())
            {
                products.Add(product);
            }
            return products;
        }

        public async Task DeleteProductAsync(string partitionKey, string rowKey)
        {
            await _productTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }

        //public async Task<List<ProcessOrders>> GetAllProcessOrdersAsync()
        //{
        //    var processOrders = new List<ProcessOrders>();
        //    await foreach (var order in _processOrderTableClient.QueryAsync<ProcessOrders>())
        //    {
        //        processOrders.Add(order);
        //    }
        //    return processOrders;
        //}

        //public async Task AddProcessOrderAsync(ProcessOrders processOrder)
        //{
        //    if (string.IsNullOrEmpty(processOrder.PartitionKey) || string.IsNullOrEmpty(processOrder.RowKey))
        //    {
        //        throw new ArgumentException("Partition key and row key must be set.");
        //    }

        //    try
        //    {
        //        await _processOrderTableClient.AddEntityAsync(processOrder);
        //    }
        //    catch (RequestFailedException ex)
        //    {
        //        throw new InvalidOperationException("Error adding entity to table storage", ex);
        //    }
        //}

        public async Task DeleteProcessOrderAsync(string partitionKey, string rowKey)
        {
            await _processOrderTableClient.DeleteEntityAsync(partitionKey, rowKey);
        }
        public async Task SignUpAsync(User user)
        {
            try
            {
                await _userTableClient.AddEntityAsync(user);
            }
            catch (Exception ex)
            {
                
            }
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            try
            {
                
                var query = _userTableClient.QueryAsync<User>(u => u.Email == email && u.Password == password);

               
                var users = new List<User>();


                await foreach (var user in query)
                {
                    users.Add(user);
                }

                return users.FirstOrDefault();
            }
            catch (Exception ex)
            {
                
                return null;
            }
        }
        //public async Task<ProcessOrders> GetProcessOrderAsync(string partitionKey, string rowKey)
        //{
        //    return await _processOrderTableClient.GetEntityAsync<ProcessOrders>(partitionKey, rowKey);
        //}

        //public async Task UpdateProcessOrderAsync(ProcessOrders processOrder)
        //{
        //    await _processOrderTableClient.UpdateEntityAsync(processOrder, processOrder.ETag, TableUpdateMode.Replace);
        ////}
    }
}

