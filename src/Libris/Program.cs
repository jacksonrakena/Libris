using Libris.Net;
using Libris.Utilities;
using System;
using System.Linq;
using System.Text;
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
            Console.WriteLine("Now listening for connections on port 25565 on all addresses.");
            await Task.Delay(-1);
        }
    }
}
