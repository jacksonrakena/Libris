using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Libris.Packets.Clientbound
{
    internal abstract class ClientboundPacket
    {
        public byte Id { get; protected set; }
        public byte[] Data { get; protected set; }
    }
}
