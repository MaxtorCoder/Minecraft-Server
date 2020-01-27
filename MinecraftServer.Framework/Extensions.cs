using System;
using System.IO;
using System.Text;

namespace MinecraftServer.Framework
{
    public static class Extensions
    {
        public static string DeserializePacket(this byte[] data)
        {
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < data.Length; i++)
            {
                var dataString = data[i].ToString("X").ToString();

                if (i == 16 || i == 32 || i == 48)
                    stringBuilder.Append("\n");

                if (dataString.Length == 1)
                    stringBuilder.Append("0");

                stringBuilder.Append(dataString + " ");
            }
            
            return stringBuilder.ToString();
        }
        
        public static uint Remaining(this Stream stream)
        {
            if (stream.Length < stream.Position)
                throw new InvalidOperationException();

            return (uint)(stream.Length - stream.Position);
        }
    }
}