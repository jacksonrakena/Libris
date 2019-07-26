using Libris.Models;
using Libris.Net;
using System;
using System.Collections.Generic;
using System.Text;
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
        public ServerFavicon Favicon { get; set; } = ServerFavicon.FromFile("favicon.png");

        private readonly LibrisTcpServer _tcp;

        public LibrisMinecraftServer()
        {
            _tcp = new LibrisTcpServer(this);
        }

        public async Task StartAsync()
        {
            _ = Task.Run(_tcp.StartListeningAsync);
        }
    }
}
