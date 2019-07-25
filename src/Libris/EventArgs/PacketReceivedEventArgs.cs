using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Libris.EventArgs
{
    internal class PacketReceivedEventArgs
    {
        internal int PacketId { get; }

        internal byte[] Data { get; }

        internal TcpClient Sender { get; }

        internal PacketReceivedEventArgs(TcpClient sender, int packetId, byte[] data)
        {
            PacketId = packetId;
            Data = data;
            Sender = sender;
        }
    }
}
