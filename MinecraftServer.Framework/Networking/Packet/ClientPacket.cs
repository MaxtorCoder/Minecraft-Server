using MinecraftServer.Framework.Networking.IO;

namespace MinecraftServer.Framework.Networking.Packet
{
    public class ClientPacket : GamePacket
    {
        public ClientPacket(byte[] data)
        {
            using (var reader = new PacketReader(data))
            {
                Size     = reader.ReadVarInt();
                Opcode   = (Opcodes)reader.ReadVarInt();
                
                // Read the remaining bytes as data.
                Data = reader.ReadBytes(Size);
            }
        }
    }
}