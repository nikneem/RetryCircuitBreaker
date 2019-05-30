using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Polly.CircuitBreaker;

namespace ResilientClient
{
    class Program
    {

        private static HttpClient client;

        static async Task Main(string[] args)
        {
            client = new HttpClient();
            Console.WriteLine("Press ESC to stop");


            var retryPolicy =  Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.BadRequest)
                .WaitAndRetryAsync(3, i =>TimeSpan.FromSeconds(3), (result, i) => { Console.WriteLine("Retrying..."); });

            var circuitBreakerPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.BadRequest)
                .CircuitBreakerAsync(6, TimeSpan.FromMinutes(1));

            do
            {
                while (!Console.KeyAvailable)
                {
                    try
                    {
                        Console.Write("Requesting...");
                        var response = await retryPolicy.WrapAsync(circuitBreakerPolicy)
                            .ExecuteAsync(() =>  client.GetAsync("https://localhost:5001/api/values"));
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            Console.WriteLine(content);
                        }
                        else
                        {
                            Console.WriteLine("FAILED");
                        }
                    }
                    catch (BrokenCircuitException )
                    {
                        Console.WriteLine("FAILED -> Broken Circuit");
                        await Task.Delay(3000);
                    }
                }
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);


        }

    }

}
