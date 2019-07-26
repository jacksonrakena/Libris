using Libris.EventArgs;
using Libris.Models;
using Libris.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Libris.Net
{
    internal class LibrisTcpServer
    {
        public delegate Task PacketReceivedHandler(PacketReceivedEventArgs packetReceived);
        public event PacketReceivedHandler PacketReceived;

        private readonly TcpListener _tcpListener;
        private readonly LibrisMinecraftServer _minecraftServer;

        private ConnectionState _state = ConnectionState.NotConnected;

        public LibrisTcpServer(LibrisMinecraftServer server)
        {
            _tcpListener = new TcpListener(IPAddress.Any, 25565);
            _tcpListener.Start();
            _minecraftServer = server;
        }

        public async Task StartListeningAsync()
        {
            while (true)
            {
                var client = await _tcpListener.AcceptTcpClientAsync();
                byte[] buffer = new byte[1024];
                var stream = client.GetStream();
                stream.Read(buffer, 0, buffer.Length);

                var packetLength = Converters.ReadVariableInteger(buffer, out byte[] remainder);
                var packetId = Converters.ReadByte(remainder, out byte[] r0);

                Console.WriteLine($"Received packet 0x{packetId:x2} with length of {packetLength} bytes. Current state: {_state}");

                PacketReceived?.Invoke(new PacketReceivedEventArgs(client, packetId, r0));

                switch (packetId)
                {
                    case 0x00: // handshake
                        Console.WriteLine($"[Packet Switcher] Received 0x00 packet, current connection state is {_state}");
                        var protocolVersion = Converters.ReadVariableInteger(r0, out byte[] serverAddr);
                        var serverAddress = Converters.ReadUtf8String(serverAddr, out byte[] serverP);
                        var serverPort = Converters.ReadUnsignedShort(serverP, out byte[] nextStateR);
                        var curStateR = Converters.ReadVariableInteger(nextStateR, out byte[] empty);
                        var state = curStateR == 1 ? ConnectionState.Status : ConnectionState.Login;
                        Console.WriteLine($"[Handshaking] Protocol version: {protocolVersion} | Server address: {serverAddress} | Port: {serverPort} | State requested: {state}");

                        if (state == ConnectionState.Status)
                        {
                            var requestPacketLength = Converters.ReadVariableInteger(empty, out byte[] id);
                            var requestPacketId = Converters.ReadByte(id, out byte[] empty1);
                            Console.WriteLine("Received status request with " + requestPacketLength + $" bytes and 0x{requestPacketId:x2} as it's ID.");
                            var d1 = JsonConvert.SerializeObject(
                            new ServerListPingResponse(new ServerListPingResponseVersion(LibrisMinecraftServer.ServerVersion, LibrisMinecraftServer.ProtocolVersion),
                            new ServerListPingResponsePlayerList(_minecraftServer.MaximumPlayers, 0, new List<int> { }),
                            new ServerListPingResponseDescription(_minecraftServer.Description))
                            );
                            new ClientboundPacket(0x00, d1).WriteToStream(stream);
                            Console.WriteLine($"[Status] Wrote status data to output stream");
                            client.Close();
                        }

                        if (state == ConnectionState.Login)
                        {
                            var requestPacketLength = Converters.ReadVariableInteger(empty, out byte[] id);
                            var requestPacketId = Converters.ReadByte(id, out byte[] empty1);
                            var username = Converters.ReadUtf8String(empty1, out byte[] empty2);
                            Console.WriteLine("Received login start from user " + username + " with " + requestPacketLength + $" bytes and 0x{requestPacketId:x2} as it's ID.");

                            // send 0x02 LOGIN SUCCESS

                            var data = Converters.WriteUtf8String("abdc8af6-70ab-4930-ab47-c6fc4e618155").Concat(Converters.WriteUtf8String("best_jessica")).ToArray();
                            new ClientboundPacket(0x02, data).WriteToStream(stream);
                            Console.WriteLine($"[Login] Wrote Login Success to stream");

                            // send 0x25 JOIN GAME
                        }
                        break;
                    default:
                        Console.WriteLine("Received unknown packet with ID 0x" + packetId.ToString("x2"));
                        break;
                }
            }
        }
    }
}
