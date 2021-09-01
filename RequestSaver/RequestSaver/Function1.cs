using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace RequestSaver
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task Run([ServiceBusTrigger("requests", Connection = "AzureWebJobsSB")]string myQueueItem, ILogger log, ExecutionContext context)
        {
            CreateContainerIfNotExists(log, context);
            try
            {
                CloudStorageAccount storageAccount = GetCloudStorageAccount(context);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("dummy-messages");
                string randomStr = Guid.NewGuid().ToString();
                CloudBlockBlob blob = container.GetBlockBlobReference(randomStr);
                using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(myQueueItem)))
                {
                    throw new Exception();
                    await blob.UploadFromStreamAsync(ms);
                }

                log.LogInformation($"Bolb {randomStr} is uploaded to container {container.Name}");
                await blob.SetPropertiesAsync();
            }catch
            {
                var smtpClient = new SmtpClient("smtp.mail.ru")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("bekarys_kuralbai@mail.ru", "Beka4464696!"),
                    EnableSsl = true,
                };

                smtpClient.Send("bekarys_kuralbai@mail.ru", "bekarys.kuralbay@gmail.com", "badrequest", myQueueItem);
            }

            log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        }

        private static void CreateContainerIfNotExists(ILogger logger, ExecutionContext executionContext)
        {
            CloudStorageAccount storageAccount = GetCloudStorageAccount(executionContext);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            string[] containers = new string[] { "dummy-messages" };
            foreach (var item in containers)
            {
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(item);
                blobContainer.CreateIfNotExistsAsync();
            }
        }

        private static CloudStorageAccount GetCloudStorageAccount(ExecutionContext executionContext)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(executionContext.FunctionAppDirectory)
                            .AddJsonFile("local.settings.json", true, true)
                            .AddEnvironmentVariables().Build();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config["AzureWebJobsStorage"]);
            return storageAccount;
        }
    }
}
