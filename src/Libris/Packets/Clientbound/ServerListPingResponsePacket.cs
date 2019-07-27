using Libris.Models;
using Libris.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Libris.Packets.Clientbound
{
    public class ServerListPingResponsePacket : ClientboundPacket
    {
        public ServerListPingResponsePacket(string serverVersion, int protocolVersion, int currentPlayers, int maximumPlayers,
            List<PlayerListSampleEntry> onlinePlayerSample, ChatText serverDescription, string faviconString = null)
        {
            Id = OutboundPackets.ServerListPingResponsePacketId;
            var stringSerialized = JsonConvert.SerializeObject(
                new ServerListPingResponse(
                        new ServerListPingResponseVersion(serverVersion, protocolVersion),
                        new ServerListPingResponsePlayerList(maximumPlayers, currentPlayers, onlinePlayerSample),
                        serverDescription,
                        faviconString
                    )
                );
            Data = Converters.GetStringBytes(stringSerialized);
        }

        internal class ServerListPingResponse
        {
            [JsonProperty("version")]
            public ServerListPingResponseVersion Version { get; }

            [JsonProperty("players")]
            public ServerListPingResponsePlayerList Players { get; }

            [JsonProperty("description")]
            public ChatText Description { get; }

            [JsonProperty("favicon", NullValueHandling = NullValueHandling.Ignore)]
            public string FaviconString { get; }

            public ServerListPingResponse(ServerListPingResponseVersion version, ServerListPingResponsePlayerList players,
                ChatText description, string faviconString)
            {
                Version = version;
                Players = players;
                Description = description;
                FaviconString = faviconString;
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
            public List<PlayerListSampleEntry> OnlinePlayerSample { get; }

            public ServerListPingResponsePlayerList(int maxPlayers, int onlinePlayers, List<PlayerListSampleEntry> sample)
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
