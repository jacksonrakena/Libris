using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Libris.Net.Clientbound
{
    internal class SpawnPositionPacket : IClientboundPacket
    {
        private ulong _location;

        public SpawnPositionPacket(int x, int y, int z)
        {
            var locationInt = ((x & 0x3FFFFFF) << 38) | ((z & 0x3FFFFFF) << 12) | (y & 0xFFF);
            _location = (ulong) locationInt;
        }

        public void WriteToStream(NetworkStream stream)
        {

        }
    }
}
