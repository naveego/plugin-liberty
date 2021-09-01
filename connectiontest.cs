using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

namespace  PluginLiberty
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // https://devapi.libertysoftware.com/LibertyAPI.json

            var client = new TransactionExportHandlerClient();

            await client.OpenAsync();

            var results = await client.getTransactionRecordsAsync("aunalytics", "Mh-4Ae5-Nk(J", DateTime.Today.AddDays(-1), DateTime.Today);

            var transactionRecords = results.Body.getTransactionRecords;

 //           foreach (var transactionRecord in transactionRecords)
 //           {
 //               Console.WriteLine(JsonConvert.SerializeObject(transactionRecord));
 //           }
    Console.WriteLine(JsonConvert.SerializeObject(transactionRecords.First(), Formatting.Indented));

            await client.CloseAsync();
        }
    }
}