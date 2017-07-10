using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.IO;
using System;

namespace FunctionApp
{
    public static class BlobFunc
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

            UploadBlob(name);

            return name == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
        }


        /// <summary>
        /// Uploads content to blob storage
        /// </summary>
        /// <param name="name">Parsed filepath from URL in web request. Will be used to generate a name for the file stored in Blob.</param>
        private static void UploadBlob(string name)
        {
            string newName = GenerateNameFromFile(name);

            AzureBlobManager abm = new AzureBlobManager();
            abm.ContainerName = AzureBlobManager.ROOT_CONTAINER_NAME;
            abm.DirectoryName = "TheBlob" + "/" + "PathYouWant" + "/";

            //Check if the Container Exists. If it does.....
            // TODO: Consider creating a new contanier for each day
            if (abm.DoesContainerExist(abm.ContainerName))
            {
                UploadToBlobViaMemStream(name, newName, abm);
            }
            else
            {
                UploadToBlobViaMemStream(name, newName, abm);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the file from the POST request</param>
        /// <param name="newName">Returned name with current date pre-pended</param>
        /// <param name="abm">Reference to Azure Blob Manager</param>
        private static void UploadToBlobViaMemStream(string name, string newName, AzureBlobManager abm)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(name);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    abm.CreateContainer(abm.ContainerName);
                    abm.PutBlob(abm.ContainerName, newName, data);
                }
            }
        }


        /// <summary>
        ///  Grab file name from file & append the date
        /// </summary>
        /// <param name="name">Name from the file POSTed</param>
        private static string GenerateNameFromFile(string name)
        {
            // Get file name from the url
            string fileName = Path.GetFileName(name);

            string currentTime = DateTime.Now.ToString("yyyy-dd-M-HH-");
            string newName = currentTime + fileName;

            return newName;
        }
    }
}