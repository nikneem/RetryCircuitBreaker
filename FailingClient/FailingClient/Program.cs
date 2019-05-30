using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FailingClient
{
    class Program
    {

        private static HttpClient client;

        static async Task Main(string[] args)
        {
            client = new HttpClient();
            Console.WriteLine("Press ESC to stop");
            do
            {
                while (!Console.KeyAvailable)
                {
                    Console.Write("Requesting...");
                    var response = await client.GetAsync("https://localhost:5001/api/values");
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
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
        }
    }
}
