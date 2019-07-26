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
            for (var i = 0; i < 255; i++)
            {
                var ba = Converters.WriteVariableInteger(i);
                var hex = new StringBuilder(ba.Length * 2);
                foreach (byte b in ba)
                    hex.AppendFormat("0x{0:x2} ", b);
                Console.WriteLine($"Normal number {i} = {hex.ToString()}");
            }

            var server = new LibrisMinecraftServer();
            await server.StartAsync();
            Console.WriteLine("Listening.");
            await Task.Delay(-1);
        }
    }
}
