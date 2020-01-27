using System;

namespace MinecraftServer.Framework.Networking.Message.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MessageAttribute : Attribute
    {
        public Opcodes Opcode { get; }

        public MessageAttribute(Opcodes opcode) => Opcode = opcode;
    }
}