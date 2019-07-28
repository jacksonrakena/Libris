using Libris.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Libris.Net.Clientbound
{
    internal interface IClientboundPacket
    {
        public void WriteToStream(NetworkStream stream);
    }
}
