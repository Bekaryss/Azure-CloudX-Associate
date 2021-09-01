using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using Microsoft.Models;

namespace OrderItemsReserver
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string buyerId = req.Query["BuyerId"];
            string date = req.Query["OrderDate"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<Order>(requestBody);
            buyerId = buyerId ?? data?.BuyerId;
            date = date ?? data.OrderDate.DateTime.ToString();
            date = date.Replace('/', '_');
            string name = buyerId + "" + date;
            var id = new Guid().ToString();
            var db = new db();
            await db.Create(data);

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

    }
}
