using MinecraftServer.Framework.Networking.IO;
 
 namespace MinecraftServer.Framework.Networking.Message
 {
     public interface IReadable
     {
         void Read(PacketReader reader);
     }
 }