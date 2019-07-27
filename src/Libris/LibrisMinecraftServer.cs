using Libris.Models;
using Libris.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Libris
{
    public class LibrisMinecraftServer : IHostedService
    {
        public const string ServerVersion = "1.14.4";
        public const int ProtocolVersion = 498;
        public const int NewConnectionBufferSize = 1024;

        /// <summary>
        ///     The maximum number of players allowed to join the server simultaneously.
        /// </summary>
        public int MaximumPlayers { get; set; } = 50;

        /// <summary>
        ///     The description, or "message of the day", as distributed to clients.
        /// </summary>
        public ChatText Description { get; set; } = new ChatText("A")
                    .SetColour(MinecraftColour.White)
                    .AddSubcomponent(new ChatText(" Libris ").SetBold(true).SetColour(MinecraftColour.Cyan))
                    .AddSubcomponent(new ChatText("Server").SetColour(MinecraftColour.White));

        /// <summary>
        ///     The server's favicon, shown in Minecraft clients next to the server's name.
        /// </summary>
        public ServerFavicon Favicon { get; set; }

        private readonly LibrisTcpServer _tcp;
        private readonly ILogger<LibrisMinecraftServer> _logger;
        private readonly IConfiguration _serverConfig;

        public LibrisMinecraftServer(ILogger<LibrisMinecraftServer> logger, LibrisTcpServer tcp, IConfiguration configuration)
        {
            _tcp = tcp;
            _logger = logger;
            _serverConfig = configuration.GetSection("Server");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (File.Exists("favicon.png"))
            {
                using var faviconImage = Image.Load("favicon.png");
                if (faviconImage.Height > 64) throw new InvalidOperationException("The provided favicon.png is more than 64 pixels high.");
                if (faviconImage.Width > 64) throw new InvalidOperationException("The provided favicon.png is more than 64 pixels wide.");
                Favicon = ServerFavicon.FromBase64String(faviconImage.ToBase64String(PngFormat.Instance));

                _logger.LogInformation($"Loaded server favicon ({faviconImage.Height}x{faviconImage.Width}) from file favicon.png");
            }
            else
            {
                _logger.LogWarning($"No favicon found. To enable favicons, save a 64x64 file called \"favicon.png\" into the server's directory.");
            }

            var maxPlayers = _serverConfig.GetValue<int?>("MaximumPlayers");
            var description = _serverConfig.GetSection("Description").Get<ChatText>();

            if (description == null)
                _logger.LogWarning("No message of the day found. To set it, set Server.Description to your desired message.");
            else Description = description;

            if (maxPlayers == null)
                _logger.LogWarning("No maximum player count configured (defaulting to 50). To set it, set Server.MaximumPlayers to your desired maximum player count.");
            else MaximumPlayers = maxPlayers.Value;

            
            _ = _tcp.StartAsync();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _tcp.StopAsync();
        }
    }
}
