using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Libris.Utilities
{
    public static class Converters
    {
        public static int ReadVariableInteger(ArraySegment<byte> set, out ArraySegment<byte> remainder)
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
            remainder = new ArraySegment<byte>(set.Array, set.Offset + numberOfBytesRead, set.Count - numberOfBytesRead);
            return result;
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
            var output = new byte[5];
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
            return output;
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

        public static long ReadLong(byte[] set)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(set);
            return BitConverter.ToInt64(set, 0);
        }

        public static string ReadUtf8String(ArraySegment<byte> set, out ArraySegment<byte> remainder)
        {
            var lengthOfString = ReadVariableInteger(set, out ArraySegment<byte> data);
            var stringBytes = new byte[lengthOfString];

            Buffer.BlockCopy(data.Array, data.Offset, stringBytes, 0, lengthOfString);

            var utf8string = Encoding.UTF8.GetString(stringBytes);

            remainder = new ArraySegment<byte>(data.Array, data.Offset + lengthOfString, data.Count - lengthOfString);

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

        public static ushort ReadUnsignedShort(ArraySegment<byte> set, out byte[] remainder)
        {
            remainder = new ArraySegment<byte>(set.Array, set.Offset + 2, set.Count - 2).ToArray();
            return BitConverter.ToUInt16(new byte[2] { set[1], set[0] });
        }

        public static byte ReadByte(ArraySegment<byte> set, out byte[] remainder)
        {
            remainder = new ArraySegment<byte>(set.Array, set.Offset + 1, set.Count - 1).ToArray();
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
