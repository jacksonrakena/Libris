using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Packets.Clientbound
{
    public class ServerListPingResponsePacket : ClientboundPacket
    {
        public ServerListPingResponsePacket(string serverVersion, int protocolVersion, int currentPlayers, int maximumPlayers,
            List<string> onlinePlayerSample, string serverDescription) : base(0x00, JsonConvert.SerializeObject(
                new ServerListPingResponse(
                        new ServerListPingResponseVersion(serverVersion, protocolVersion),
                        new ServerListPingResponsePlayerList(maximumPlayers, currentPlayers, onlinePlayerSample),
                        new ServerListPingResponseDescription(serverDescription)
                    )
                ))
        {
        }

        internal class ServerListPingResponse
        {
            [JsonProperty("version")]
            public ServerListPingResponseVersion Version { get; }

            [JsonProperty("players")]
            public ServerListPingResponsePlayerList Players { get; }

            [JsonProperty("description")]
            public ServerListPingResponseDescription Description { get; }

            public ServerListPingResponse(ServerListPingResponseVersion version, ServerListPingResponsePlayerList players,
                ServerListPingResponseDescription description)
            {
                Version = version;
                Players = players;
                Description = description;
            }
        }

        internal class ServerListPingResponseVersion
        {
            [JsonProperty("name")]
            public string ServerVersion { get; }
            [JsonProperty("protocol")]
            public int ProtocolVersion { get; }

            public ServerListPingResponseVersion(string serverVersion, int protocolVersion)
            {
                ServerVersion = serverVersion;
                ProtocolVersion = protocolVersion;
            }
        }

        internal class ServerListPingResponsePlayerList
        {
            [JsonProperty("max")]
            public int MaximumPlayers { get; }

            [JsonProperty("online")]
            public int OnlinePlayers { get; }

            [JsonProperty("sample")]
            public List<string> OnlinePlayerSample { get; }

            public ServerListPingResponsePlayerList(int maxPlayers, int onlinePlayers, List<string> sample)
            {
                MaximumPlayers = maxPlayers;
                OnlinePlayers = onlinePlayers;
                OnlinePlayerSample = sample;
            }
        }

        // todo: replace with chat model
        internal class ServerListPingResponseDescription
        {
            [JsonProperty("text")]
            public string Text { get; }

            public ServerListPingResponseDescription(string text)
            {
                Text = text;
            }
        }
    }
}
