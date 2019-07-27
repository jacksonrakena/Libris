using Libris.Models;
using Libris.Packets.Clientbound;
using Libris.Utilities;
using Microsoft.Extensions.DependencyInjection;
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
        private readonly IServiceProvider _services;

        private CancellationTokenSource _cts;

        public LibrisTcpServer(IServiceProvider services)
        {
            _tcpListener = new TcpListener(IPAddress.Any, 25565);
            _services = services;
            _tcpListener.Start();
        }

        public async Task StartAsync()
        {
            _cts = new CancellationTokenSource();
            while (!_cts.IsCancellationRequested)
            {
                var client = await _tcpListener.AcceptTcpClientAsync();

                var conn = _services.GetRequiredService<LibrisTcpConnection>();
                _ = conn.HandleAsync(client);
            }
        }

        public Task StopAsync()
        {
            _cts.Cancel();
            return Task.CompletedTask;
        }
    }
}
