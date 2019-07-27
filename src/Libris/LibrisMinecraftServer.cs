using Libris.Models;
using Libris.Net;
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
    public class LibrisMinecraftServer
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
        public string Description { get; set; } = "A server running on Libris.";

        /// <summary>
        ///     The server's favicon, shown in Minecraft clients next to the server's name.
        /// </summary>
        public ServerFavicon Favicon { get; set; }

        private readonly LibrisTcpServer _tcp;

        public LibrisMinecraftServer()
        {
            _tcp = new LibrisTcpServer(this);
        }

        public async Task StartAsync(CancellationToken? cancellationToken = null)
        {
            if (File.Exists("favicon.png"))
            {
                using var faviconImage = Image.Load("favicon.png");
                if (faviconImage.Height > 64) throw new InvalidOperationException("The provided favicon.png is more than 64 pixels high.");
                if (faviconImage.Width > 64) throw new InvalidOperationException("The provided favicon.png is more than 64 pixels wide.");
                Favicon = ServerFavicon.FromBase64String(faviconImage.ToBase64String(PngFormat.Instance));

                Console.WriteLine($"[Startup] Loaded server favicon ({faviconImage.Height}x{faviconImage.Width}) from file favicon.png");
            }

            _ = _tcp.StartAsync(cancellationToken);
        }
    }
}
