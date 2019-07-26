using Libris.Packets.Clientbound;
using Libris.Packets.Serverbound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Libris.Utilities
{
    public static class Converters
    {
        public static int ReadVariableInteger(byte[] set, out byte[] remainder)
        {
            var numberOfBytesRead = 0;
            var result = 0;

            byte current;
            int currentIndex = 0;

            do
            {
                current = set[currentIndex];
                var value = current & 0b01111111;
                result |= value << (7 * numberOfBytesRead);

                currentIndex++;
                numberOfBytesRead++;
                if (numberOfBytesRead > 5)
                {
                    // too big
                    throw new InvalidOperationException("Data is too big to be a variable integer.");
                }
            }
            while ((current & 0b10000000) != 0);
            remainder = new ArraySegment<byte>(set, numberOfBytesRead, set.Length - numberOfBytesRead).ToArray();
            return result;
        }

        public static int ReadVariableInteger(this BinaryReader reader)
        {
            var numberOfBytesRead = 0;
            var result = 0;

            byte current;

            do
            {
                current = reader.ReadByte();
                var value = current & 0b01111111;
                result |= value << (7 * numberOfBytesRead);

                numberOfBytesRead++;
                if (numberOfBytesRead > 5)
                    throw new InvalidOperationException("Data is too large to be a variable integer.");
            }
            while ((current & 0b10000000) != 0);
            return result;
        }
        public static Task WritePacketAsync(this BinaryWriter writer, ClientboundPacket packet)
        {
            writer.WriteVariableInteger(packet.Data.Length + 1);
            writer.Write(packet.Id);
            writer.Write(packet.Data);
            return writer.BaseStream.FlushAsync();
        }


        public static byte[] WriteInteger(int value)
        {
            /* if(value > 0)
            {
                value = (value & (int.MaxValue >> 1));
                value = ~value + 1;
            }*/
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] WriteVariableInteger(int value)
        {
            List<byte> output = new List<byte>();
            int cindex = 0;
            do
            {
                byte temp = (byte) (value & 0b01111111);
                value >>= 7;
                if (value != 0)
                {
                    temp |= 0b10000000;
                }
                output.Add(temp);
            } while (value != 0);
            return output.ToArray();
        }

        public static void WriteVariableInteger(this BinaryWriter writer, int value)
        {
            do
            {
                byte temp = (byte) (value & 0b01111111);
                value >>= 7;
                if (value != 0)
                {
                    temp |= 0b10000000;
                }
                writer.Write(temp);
            } while (value != 0);
        }

        public static byte[] WriteUnsignedLong(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] WriteLong(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        public static ushort ReadUInt16BigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(2);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes);
        }

        /*public static long ReadLong(byte[] set)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(set);
            return BitConverter.ToInt64(set, 0);
        }*/

        public static string ReadUtf8String(byte[] set, out byte[] remainder)
        {
            var length = ReadVariableInteger(set, out byte[] data);
            var bytesRemainder = new byte[length];
            Buffer.BlockCopy(data, 0, bytesRemainder, 0, length);
            var utf8string = Encoding.UTF8.GetString(bytesRemainder, 0, length);

            var remainder0 = new byte[data.Length - length];
            Buffer.BlockCopy(data, length, remainder0, 0, data.Length - length);
            remainder = remainder0;

            return utf8string;
        }

        public static byte WriteBoolean(bool value)
        {
            return value ? (byte) 0x01 : (byte) 0x00;
        }

        public static byte[] WriteUtf8String(string data)
        {
            var length = WriteVariableInteger(data.Length);
            var str = Encoding.UTF8.GetBytes(data);
            return length.Concat(str).ToArray();
        }

        public static ushort ReadUnsignedShort(byte[] set, out byte[] remainder)
        {
            remainder = new ArraySegment<byte>(set, 2, set.Length - 2).ToArray();
            return BitConverter.ToUInt16(new byte[2] { set[1], set[0] });
        }

        public static byte ReadByte(byte[] set, out byte[] remainder)
        {
            remainder = new ArraySegment<byte>(set, 1, set.Length - 1).ToArray();
            return set[0];
        }

        public static byte[] WriteFloat(float f)
        {
            var bytes = BitConverter.GetBytes(f);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] WriteDouble(double d)
        {
            var bytes = BitConverter.GetBytes(d);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
    }
}
