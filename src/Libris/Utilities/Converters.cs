using Libris.Packets.Clientbound;
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
        public static ushort ReadUInt16BigEndian(this BinaryReader reader)
        {
            var bytes = reader.ReadBytes(2);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return BitConverter.ToUInt16(bytes);
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

        public static void WritePacket(this BinaryWriter writer, ClientboundPacket packet)
        {
            writer.WriteVariableInteger(packet.Data.Length + 1);
            writer.Write(packet.Id);
            writer.Write(packet.Data);
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

        public static byte[] GetIntBytes(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetVarIntBytes(int value)
        {
            var output = new List<byte>(5);
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

        public static byte[] GetUInt64Bytes(ulong value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetInt64Bytes(long value)
        {
            var bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        public static byte GetBoolBytes(bool value)
        {
            return value ? (byte) 0x01 : (byte) 0x00;
        }

        public static byte[] GetStringBytes(string data)
        {
            var length = GetVarIntBytes(data.Length);
            var str = Encoding.UTF8.GetBytes(data);
            return length.Concat(str).ToArray();
        }

        public static byte[] GetFloatBytes(float f)
        {
            var bytes = BitConverter.GetBytes(f);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }

        public static byte[] GetDoubleBytes(double d)
        {
            var bytes = BitConverter.GetBytes(d);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return bytes;
        }
    }
}
