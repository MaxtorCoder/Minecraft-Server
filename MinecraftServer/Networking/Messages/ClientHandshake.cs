using MinecraftServer.Framework.Networking;
using MinecraftServer.Framework.Networking.IO;
using MinecraftServer.Framework.Networking.Message;
using MinecraftServer.Framework.Networking.Message.Attributes;

namespace MinecraftServer.Networking.Messages
{
    [Message(Opcodes.ClientHandshake)]
    public class ClientHandshake : IReadable
    {
        public int ProtocolVersion { get; set; }
        public string ServerAddress { get; set; }
        public ushort ServerPort { get; set; }
        public int NextState { get; set; }
        
        public void Read(PacketReader reader)
        {
            ProtocolVersion   = reader.ReadVarInt();
            ServerAddress     = reader.ReadPrefixedString();
            ServerPort        = reader.ReadUInt16();
            NextState         = reader.ReadVarInt();
        }
    }
}