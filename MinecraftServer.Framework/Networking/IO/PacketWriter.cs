using System.IO;

namespace MinecraftServer.Framework.Networking.IO
{
    public class PacketWriter : BinaryWriter
    {
        private static MemoryStream stream;
        public PacketWriter() : base(stream = new MemoryStream()) { }
        public MemoryStream GetStream() => stream;
        
        public void WriteBool(bool data) => base.Write(data);
        public void WriteInt8(sbyte data) => base.Write(data);
        public void WriteInt16(short data) => base.Write(data);
        public void WriteInt32(int data) => base.Write(data);
        public void WriteInt64(long data) => base.Write(data);
        public void WriteUInt8(byte data) => base.Write(data);
        public void WriteUInt16(ushort data) => base.Write(data);
        public void WriteUInt32(uint data) => base.Write(data);
        public void WriteUInt64(ulong data) => base.Write(data);
        public void WriteFloat(float data) => base.Write(data);
        public void WriteDouble(double data) => base.Write(data);

        public void WriteVarInt(int data)
        {
            do
            {
                byte temp = (byte) (data & 0b01111111);

                data = (int) ((uint) data >> 7);
                if (data != 0)
                    temp |= 0b10000000;
                
                WriteInt32(temp);
            } 
            while (data != 0);
        }
    }
}