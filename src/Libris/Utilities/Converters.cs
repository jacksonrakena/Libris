using Libris.Net.Clientbound;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Libris.Utilities
{
    internal static class Converters
    {
        public static void WritePacket(this BinaryWriter writer, ClientboundPacket packet)
        {
            writer.WriteVariableInteger(packet.Data.Length + 1);
            writer.Write(packet.Id);
            writer.Write(packet.Data.Span);
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

        public static T Read<T>(this NetworkStream stream, bool isBigEndian = true) where T: struct
        {
            T result = default;
            Span<byte> span = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref result, 1));
            stream.Read(span);
            if (isBigEndian && BitConverter.IsLittleEndian) span.Reverse();
            return result;
        }

        public static int ReadVarInt(this NetworkStream stream)
        {
            var numberOfBytesRead = 0;
            var result = 0;

            byte current;

            do
            {
                current = (byte) stream.ReadByte();
                var value = current & 0b01111111;
                result |= value << (7 * numberOfBytesRead);

                numberOfBytesRead++;
                if (numberOfBytesRead > 5)
                    throw new InvalidOperationException("Data is too large to be a variable integer.");
            }
            while ((current & 0b10000000) != 0);
            return result;
        }

        public static string ReadString(this NetworkStream stream)
        {
            var length = stream.ReadVarInt();
            Span<byte> stringBytes = stackalloc byte[length];
            stream.Read(stringBytes);
            return Encoding.UTF8.GetString(stringBytes);
        }

        public static void GetVarIntBytes(int value, Span<byte> destination, out int bytesWritten)
        {
            Span<byte> buffer = stackalloc byte[5]; // the maximum size for a varint is 5
            var cindex = 0;
            do
            {
                var temp = (byte) (value & 0b01111111);
                value >>= 7;
                if (value != 0)
                {
                    temp |= 0b10000000;
                }
                buffer[cindex] = temp;
                cindex++;
            } while (value != 0);
            buffer.Slice(0, cindex).CopyTo(destination);
            bytesWritten = cindex;
        }

        public static bool TryGetVarIntBytes(int value, Span<byte> destination)
        {
            Span<byte> buffer = stackalloc byte[5];
            var cindex = 0;
            do
            {
                byte temp = (byte) (value & 0b01111111);
                value >>= 7;
                if (value != 0)
                {
                    temp |= 0b10000000;
                }
                buffer[cindex] = temp;
                cindex++;
            } while (value != 0);
            return buffer.Slice(0, cindex).TryCopyTo(destination);
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

        public static void GetStringBytes(string data, Span<byte> destination)
        {
            Span<byte> intBytes = stackalloc byte[5];
            GetVarIntBytes(data.Length, intBytes, out int intBytesLength);

            var r = intBytes.Slice(0, intBytesLength).TryCopyTo(destination);
            Encoding.UTF8.GetBytes(data, destination.Slice(intBytesLength));
        }

        public static byte[] GetStringBytes(string data)
        {
            Span<byte> intBytes = stackalloc byte[5];
            GetVarIntBytes(data.Length, intBytes, out int intBytesLength);

            var destination = new byte[intBytesLength + Encoding.UTF8.GetByteCount(data)];

            intBytes.Slice(0, intBytesLength).CopyTo(destination);
            Encoding.UTF8.GetBytes(data, destination);

            return destination;
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
