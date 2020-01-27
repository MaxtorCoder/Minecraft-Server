using System;
using System.IO;
using System.Text;

namespace MinecraftServer.Framework.Networking.IO
{
    public class PacketReader : BinaryReader
    {
        private static MemoryStream stream;
        public PacketReader(byte[] data) : base(stream = new MemoryStream(data)) { }
        
        public uint BytesRemaining => stream?.Remaining() ?? 0u;
        
        public bool ReadBool() => base.ReadBoolean();
        public sbyte ReadInt8() => base.ReadSByte();
        public byte ReadUInt8() => base.ReadByte();
        public float ReadFloat() => base.ReadSingle();

        public int ReadVarInt()
        {
            int numRead = 0, result = 0, read = 0;
            
            do
            {
                read = ReadUInt8();
                if (read == -1)
                    return 0;

                int value = ((byte) read & 0b0111111);
                result |= (value << (7 * numRead));

                numRead++;
                if (numRead > 5)
                    throw new Exception("VarInt is too large!!!!!");
            }
            while (((byte) read & 0b10000000) != 0);

            return result;
        }

        public string ReadPrefixedString()
        {
            // Read the string length
            var stringLength = ReadVarInt();

            // Return the string
            return Encoding.UTF8.GetString(ReadBytes(stringLength));
        }
    }
}