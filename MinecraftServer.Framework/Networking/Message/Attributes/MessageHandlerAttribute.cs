using System;

namespace MinecraftServer.Framework.Networking.Message.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageHandlerAttribute : Attribute
    {
        public Opcodes Opcode { get; }

        public MessageHandlerAttribute(Opcodes opcode) => Opcode = opcode;
    }
}