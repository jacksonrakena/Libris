using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Packets.Serverbound
{
    public class ServerboundPacket
    {
        public byte Id { get; }
        public int Length { get; }
        public byte[] Data { get; }

        public ServerboundPacket(byte[] data)
        {
            Length = Converters.ReadVariableInteger(data, out var remainder);
            Id = Converters.ReadByte(remainder, out byte[] r0);
            Data = r0;
        }

        public ServerboundPacket(ArraySegment<byte> data)
        {
            Length = Converters.ReadVariableInteger(data, out var remainder);
            Id = Converters.ReadByte(remainder, out byte[] r0);
            Data = r0;
        }
    }
}
