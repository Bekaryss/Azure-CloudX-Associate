using Microsoft.Azure.Cosmos;
using Microsoft.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrderItemsReserver
{
    public class db
    {
        /// The Azure Cosmos DB endpoint for running this GetStarted sample.
        private string EndpointUrl = Environment.GetEnvironmentVariable("https://cosmodbbeka.documents.azure.com:443/");

        /// The primary key for the Azure DocumentDB account.
        private string PrimaryKey = Environment.GetEnvironmentVariable("MaeHV01BHBTTudEk7uwe1uzxv0Tb2N5vwMNAxMTuYTVdwqTyMjyMFL0pV7OTrxWrINqOCps9vtDF6qxEoytkfw==");

        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container container;

        // The name of the database and container we will create
        private string databaseId = "OrderDatabase";
        private string containerId = "OrderContainer";

        public async Task Create(Order order)
        {
            this.cosmosClient = new CosmosClient("AccountEndpoint=https://cosmodbbeka.documents.azure.com:443/;AccountKey=MaeHV01BHBTTudEk7uwe1uzxv0Tb2N5vwMNAxMTuYTVdwqTyMjyMFL0pV7OTrxWrINqOCps9vtDF6qxEoytkfw==;");
            await this.CreateDatabaseAsync();
            await this.CreateContainerAsync();

            //ADD THIS PART TO YOUR CODE
            await this.AddItemsToContainerAsync(order);
        }

        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            Console.WriteLine("Created Database: {0}\n", this.database.Id);
        }

        private async Task CreateContainerAsync()
        {
            // Create a new container
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId, "/BuyerId");
            Console.WriteLine("Created Container: {0}\n", this.container.Id);
        }

        private async Task AddItemsToContainerAsync(Order order)
        {
            try
            {
                // Read the item to see if it exists.
                ItemResponse<Order> orderResponce = await this.container.ReadItemAsync<Order>(order.id, new PartitionKey(order.BuyerId));
                Console.WriteLine("Item in database with id: {0} already exists\n", order.id);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Create an item in the container representing the Andersen family. Note we provide the value of the partition key for this item, which is "Andersen"
                ItemResponse<Order> andersenFamilyResponse = await this.container.CreateItemAsync<Order>(order, new PartitionKey(order.BuyerId));

                // Note that after creating the item, we can access the body of the item with the Resource property off the ItemResponse. We can also access the RequestCharge property to see the amount of RUs consumed on this request.
                Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", order.id, andersenFamilyResponse.RequestCharge);
            }
        }
    }
}
