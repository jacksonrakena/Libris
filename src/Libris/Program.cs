using Libris.Net;
using Libris.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
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
            new HostBuilder()
                .ConfigureServices((hostBuilderContext, serviceCollection) =>
                {
                    serviceCollection.AddSingleton<LibrisMinecraftServer>();
                    serviceCollection.AddHostedService<LibrisMinecraftServerStarter>();
                    serviceCollection.AddSingleton<LibrisTcpServer>();
                    serviceCollection.AddTransient<LibrisTcpConnection>();
                    serviceCollection.AddSingleton(hostBuilderContext.Configuration);
                })
                .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
                {
                    configurationBuilder.AddJsonFile("libris.json", false, true);
                })
                .ConfigureLogging((hostBuilderContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(hostBuilderContext.Configuration.GetSection("Logging"));
                    loggingBuilder.AddConsole();
                })
                .RunConsoleAsync()
                .GetAwaiter().GetResult();
        }
    }
}
