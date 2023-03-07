using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Identity;
using System.Net.Http.Headers;
using System.Net.Http;

namespace Func2funcwithSysMI
{
    public static class FuncA
    {
        [FunctionName("FuncA")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function in funcA processed a request.");
            

            var url = "https://funcb.azurewebsites.net/api/httptrigger";
            var creds = new DefaultAzureCredential();
            var token = await creds.GetTokenAsync(new Azure.Core.TokenRequestContext(new[] { "63831325-718b-4044-99ed-e65f5057de71" })); //Client Id from the Azure AD protected FuncB

            log.LogInformation("Token obtained" + token.Token);

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
                var result = await client.GetAsync(url);
                var funcBresponseMessage = await result.Content.ReadAsStringAsync();

                return new OkObjectResult(funcBresponseMessage);
            }

        }
    }
}
