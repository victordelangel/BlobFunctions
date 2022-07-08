using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;
using Azure.Storage.Blobs;

namespace contoso.movefilestoblob
{
    public static class HTTPTriggerFilesToBlob
    {
        [FunctionName("HTTPTriggerFilesToBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"]; 

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

                AzureStorageAccountBlob(log);

            return new OkObjectResult(responseMessage);
        }

        private static void AzureStorageAccountBlob(ILogger log)
        {
            //Colocar connection String
            string connectionString = "";
            //share que vamos a buscar
            //En este caso corresponde al folder compartido por Azure Files
            string sharename ="demoshare";
            //Directorio compartido por Azure Files
            string directory = "demodirectory";
            
            //Blob destino
            string containerName = "sftphome";
            
            log.LogInformation("Obteniendo referencia al archivo");
            //Obtener referencia al archivo
            ShareClient share = new ShareClient(connectionString, sharename);
            ShareDirectoryClient directoryClient = share.GetDirectoryClient(directory);
           
            //Crea referencia al blob destino
            BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);

            foreach(ShareFileItem item in directoryClient.GetFilesAndDirectories()){

                log.LogInformation("encuentra archivo o directorio: "+item.Name);

                BlobClient blob = containerClient.GetBlobClient(item.Name);

                log.LogInformation("Subiendo archivo {item.Name} al blob", item.Name);

                Stream file = directoryClient.GetFileClient(item.Name).OpenRead();

                blob.Upload(file);


                log.LogInformation("Borrando archivo {item.Name} del directorio {directoryClient.Name}", item.Name, directoryClient.Name);
                directoryClient.DeleteFile(item.Name);

                blob.SetAccessTier(Azure.Storage.Blobs.Models.AccessTier.Archive );


            }

            

            
            

            

            



        }
    }
}
