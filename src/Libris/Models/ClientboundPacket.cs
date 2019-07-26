using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Libris.Models
{
    internal class ClientboundPacket
    {
        public byte PacketId { get; }
        public byte[] Data { get; }

        public byte[] Pack()
        {
            var length = Converters.WriteVariableInteger(1 + Data.Length);
            return length.Append(PacketId).Concat(Data).ToArray();
        }

        public void WriteToStream(NetworkStream stream)
        {
            var p = Pack();
            stream.Write(p, 0, p.Length);
        }

        internal ClientboundPacket(byte packetId, byte[] data)
        {
            PacketId = packetId;
            Data = data;
        }

        internal ClientboundPacket(byte packetId, string data)
        {
            PacketId = packetId;
            Data = Converters.WriteUtf8String(data);
        }
    }
}
