using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Libris.Packets.Clientbound
{
    public abstract class ClientboundPacket
    {
        public byte Id { get; protected set; }
        public byte[] Data { get; protected set; }

        public byte[] Pack()
        {
            var length = Converters.WriteVariableInteger(1 + Data.Length);
            return length.Append(Id).Concat(Data).ToArray();
        }

        internal ClientboundPacket(byte packetId, byte[] data)
        {
            Id = packetId;
            Data = data;
        }

        internal ClientboundPacket(byte packetId, string data)
        {
            Id = packetId;
            Data = Converters.WriteUtf8String(data);
        }

        internal ClientboundPacket()
        {

        }
    }
}
