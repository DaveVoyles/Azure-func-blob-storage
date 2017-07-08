using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types
using System.Diagnostics;
using System.IO;
using System;

namespace FunctionApp6
{
    public static class Function3
    {
        [FunctionName("HttpTriggerCSharp")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();

            // Set name to query string or body data
            name = name ?? data?.name;

            //ConnectToBlob(data.imgUrl);
            //ConnectToBlob();
            BlobStuff();

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }


        private static void BlobStuff()
        {
            AzureBlobManager abm = new AzureBlobManager();
                             abm.ContainerName = AzureBlobManager.ROOT_CONTAINER_NAME;
                             abm.DirectoryName = "TheBlob" + "/" + "PathYouWant" + "/";


            //Check if the Container Exists
            if (abm.DoesContainerExist(abm.ContainerName))
            {
                //If so, do this if you want to delete all the files already there.
                if (abm.DoesBlobDirectoryExist(abm.ContainerName, abm.DirectoryName))
                {
                    abm.DeleteBlobDirectory(abm.ContainerName, abm.DirectoryName);
                }
            }
            else
            {
                abm.CreateContainer(abm.ContainerName);
                Debug.WriteLine("Contrainer created! " + abm.ContainerName);
                //throw new Exception("The specified Azure Container \"" + abm.ContainerName + "\" doesn't exist.");
            }
        }





        public static void ConnectToBlob(/*string imgData*/)
        {
            var connString = "DefaultEndpointsProtocol=https;AccountName=dvgraphstore;AccountKey=PrVUPAQSKHMXJTvRiUq8dMomIau6TsJFKCEy+Ml45Ayd7JjNR90x8eFL7cyxqUKj1jFUxYv642wraPYc9k6zgw==;";

            Guid id = Guid.NewGuid();

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            //CloudBlobContainer container = blobClient.GetContainerReference("mycontainer-" + id);
            CloudBlobContainer container = blobClient.GetContainerReference("test-cont"+id);

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("myblob.jpg");

            // Set permissions
            container.SetPermissions(
                new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });

            // Get file name from the url
            //string fileName = Path.GetFileName(imgData);

            var imagePath = "http://snoopdogg.com/wp-content/themes/snoop_2014/assets/images/og-img.jpg";

            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(imagePath);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    blockBlob.UploadFromByteArrayAsync(data, 0, data.Length);

                    // Alternate way to upload images, via memory stream
                    //using (var yourImage = Image.FromStream(mem))
                    //{
                    //    //blockBlob.UploadFromStream(mem);
                    //}
                }

            }

        }

    }
}