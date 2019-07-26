using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Packets.Clientbound
{
    public class SpawnPositionPacket : ClientboundPacket
    {
        public SpawnPositionPacket(int x, int y, int z)
        {
            Id = 0x49;
            var locationInt = ((x & 0x3FFFFFF) << 38) | ((z & 0x3FFFFFF) << 12) | (y & 0xFFF);
            Data = Converters.GetUInt64Bytes((ulong) locationInt);
        }
    }
}
