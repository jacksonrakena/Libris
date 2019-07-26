using Libris.Packets.Serverbound;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Libris.EventArgs
{
    internal class PacketReceivedEventArgs
    {
        internal ServerboundPacket Packet { get; }
        internal TcpClient Sender { get; }

        internal PacketReceivedEventArgs(TcpClient sender, ServerboundPacket packet)
        {
            Packet = packet;
            Sender = sender;
        }
    }
}
