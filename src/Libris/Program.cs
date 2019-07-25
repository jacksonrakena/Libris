using Libris.Net;
using System;
using System.Threading.Tasks;

namespace Libris
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        public static async Task MainAsync()
        {
            var server = new LibrisMinecraftServer();
            await server.StartAsync();
            Console.WriteLine("Listening.");
            await Task.Delay(-1);
        }
    }
}
