using MinecraftServer.Framework.Networking.IO;

namespace MinecraftServer.Framework.Networking.Message
{
    public interface IWritable
    {
        void Write(PacketWriter writer);
    }
}