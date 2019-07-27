using Libris.Models;
using Libris.Packets.Clientbound;
using Libris.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Libris.Net
{
    public class LibrisTcpServer
    {
        private readonly TcpListener _tcpListener;
        internal readonly LibrisMinecraftServer libris;

        internal Dictionary<int, LibrisTcpConnection> Connections = new Dictionary<int, LibrisTcpConnection>();
        private int nextKey = 0;


        public LibrisTcpServer(LibrisMinecraftServer server)
        {
            _tcpListener = new TcpListener(IPAddress.Any, 25565);
            _tcpListener.Start();
            libris = server;
        }

        public async Task StartAsync(CancellationToken? cancellationToken = null)
        {
            cancellationToken ??= CancellationToken.None;
            while (!cancellationToken.Value.IsCancellationRequested)
            {
                var client = await _tcpListener.AcceptTcpClientAsync();

                var conn = CreateConnection(client);
                _ = conn.HandleAsync();
            }
        }

        public LibrisTcpConnection CreateConnection(TcpClient client)
        {
            var conn = new LibrisTcpConnection(client, this, nextKey);
            Connections[nextKey] = conn;
            Console.WriteLine($"[TCP Server] Created connection " + nextKey);
            nextKey++;
            return conn;
        }

        public bool RemoveConnection(int connectionId)
        {
            var success = Connections.Remove(connectionId);
            Console.WriteLine($"[TCP Server] {(success ? "Removed" : "Failed to remove" )} connection " + connectionId);
            return success;
        }
    }
}
