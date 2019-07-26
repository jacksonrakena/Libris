using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Packets.Clientbound
{
    public class ServerListPingLatencyPacket : ClientboundPacket
    {
        public ServerListPingLatencyPacket(long confirmation)
        {
            Id = 0x01;
            Data = Converters.GetInt64Bytes(confirmation);
        }
    }
}
