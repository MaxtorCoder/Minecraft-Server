namespace MinecraftServer.Framework.Networking.Packet
{
    public abstract class GamePacket
    {
        // Packet Id Length is 2 bytes. Literally what the fuck minecraft.
        public const int HeaderSize = 2;
        
        public int Size { get; set; }
        public Opcodes Opcode { get; set; }
        
        public byte[] Data { get; set; }
    }
}