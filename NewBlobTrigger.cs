using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace constoso.blobtrigger
{
    public class NewBlobTrigger
    {


        public class CFDI
        {
            public string Name { get; set; }
            public string Date { get; set; }
            public string Path { get; set; }
        }

        static HttpClient client = new HttpClient();

        [FunctionName("NewBlobTrigger")]
        public void Run([BlobTrigger("sftphome/{name}", Connection = "videlasftpdemo_STORAGE")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            CFDI product = new CFDI
            {
                Name = name.Split("/")[1],
                Date = name.Split("/")[0],
                Path = "https://videlasftpdemo.blob.core.windows.net/sftphome/"+name
            };

            //Código para invocar API de actualización de base de datos

            log.LogInformation("Actualizando CFDI en informix : \n CFDI:{Name} \n Path: {Path} \n Date: {Date}", product.Name, product.Path, product.Date);

        }
    }
}
