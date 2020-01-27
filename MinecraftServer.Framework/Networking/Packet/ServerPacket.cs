using MinecraftServer.Framework.Networking.IO;
using MinecraftServer.Framework.Networking.Message;

namespace MinecraftServer.Framework.Networking.Packet
{
    public class ServerPacket : GamePacket
    {
        public ServerPacket(Opcodes opcode, IWritable message)
        {
            using (var writer = new PacketWriter())
            {
                writer.WriteVarInt((int)opcode);
                message.Write(writer);

                Data = writer.GetStream().ToArray();
                Size = (int) writer.GetStream().Length;
            }

            Opcode = opcode;
        }
    }
}