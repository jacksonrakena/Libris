using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Libris
{
    internal class LibrisMinecraftServerStarter : IHostedService
    {
        private readonly LibrisMinecraftServer _backgroundService;

        public LibrisMinecraftServerStarter(LibrisMinecraftServer backgroundService)
        {
            _backgroundService = backgroundService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _backgroundService.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _backgroundService.StopAsync(cancellationToken);
        }
    }
}
