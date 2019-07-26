using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

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

        public static string ReadUtf8String(byte[] set, out byte[] remainder)
        {
            var length = ReadVariableInteger(set, out byte[] data);
            remainder = new ArraySegment<byte>(data, length, data.Length - length).ToArray();
            return Encoding.UTF8.GetString(data, 0, length);
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
    }
}
