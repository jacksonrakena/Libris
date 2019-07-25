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
        public int PacketId { get; }
        public byte[] Data { get; }

        public byte[] Pack()
        {
            var packetId = Converters.WriteVariableInteger(PacketId);
            var length = Converters.WriteVariableInteger(packetId.Length + Data.Length);
            return length.Concat(packetId).Concat(Data).ToArray();
        }

        public void WriteToStream(NetworkStream stream)
        {
            var p = Pack();
            stream.Write(p, 0, p.Length);
        }

        internal ClientboundPacket(int packetId, byte[] data)
        {
            PacketId = packetId;
            Data = data;
        }

        internal ClientboundPacket(int packetId, string data)
        {
            PacketId = packetId;
            Data = Converters.WriteUtf8String(data);
        }
    }
}
