using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Net.Clientbound
{
    internal class SpawnPositionPacket : ClientboundPacket
    {
        public SpawnPositionPacket(int x, int y, int z)
        {
            Id = OutboundPackets.SpawnPositionPacketId;
            var locationInt = ((x & 0x3FFFFFF) << 38) | ((z & 0x3FFFFFF) << 12) | (y & 0xFFF);
            Data = Converters.GetUInt64Bytes((ulong) locationInt);
        }
    }
}
