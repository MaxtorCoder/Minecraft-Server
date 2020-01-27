using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MinecraftServer.Framework.Logging;
using MinecraftServer.Framework.Networking.Message.Attributes;

namespace MinecraftServer.Framework.Networking.Message
{
    public delegate void MessageHandlerDelegate(Session session, IReadable message);
    
    public static class MessageManager
    {
        private delegate IReadable MessageFactoryDelegate();

        private static ImmutableDictionary<Opcodes, MessageFactoryDelegate> ClientMessageStructures;
        private static ImmutableDictionary<Opcodes, MessageHandlerDelegate> ClientMessageHandlers;
        private static ImmutableDictionary<Type, Opcodes> ServerMessageStructures;
        
        public static void Initialize()
        {
            InitializeMessages();
            InitializeMessageHandlers();
        }

        private static void InitializeMessages()
        {
            var messageStructures = new Dictionary<Opcodes, MessageFactoryDelegate>();
            var messageOpcodes = new Dictionary<Type, Opcodes>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Concat(Assembly.GetEntryAssembly().GetTypes()))
            {
                var attribute = type.GetCustomAttribute<MessageAttribute>();
                if (attribute == null)
                    continue;

                if (typeof(IReadable).IsAssignableFrom(type))
                {
                    var @new = Expression.New(type.GetConstructor((Type.EmptyTypes)));
                    messageStructures.Add(attribute.Opcode, Expression.Lambda<MessageFactoryDelegate>(@new).Compile());
                }
                else if (typeof(IWritable).IsAssignableFrom(type))
                    messageOpcodes.Add(type, attribute.Opcode);
            }

            ClientMessageStructures = messageStructures.ToImmutableDictionary();
            ServerMessageStructures = messageOpcodes.ToImmutableDictionary();
            
            Log.Print(LogType.Server, $"Initialized {ClientMessageStructures.Count} client {(ClientMessageStructures.Count == 1 ? "message" : "messages")}.");
            Log.Print(LogType.Server, $"Initialized {ServerMessageStructures.Count} server {(ServerMessageStructures.Count == 1 ? "message" : "messages")}.");
        }

        private static void InitializeMessageHandlers()
        {
            var handlers = new Dictionary<Opcodes, MessageHandlerDelegate>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes()
                .Concat(Assembly.GetEntryAssembly().GetTypes()))
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.DeclaringType != type)
                        continue;

                    var attribute = method.GetCustomAttribute<MessageHandlerAttribute>();
                    if (attribute == null)
                        continue;

                    var sessionParameter = Expression.Parameter(typeof(Session));
                    var messageParameter = Expression.Parameter(typeof(IReadable));

                    var parameterInfo = method.GetParameters();

                    if (method.IsStatic)
                    {
                        #region Debug

                        Debug.Assert(parameterInfo.Length == 2);
                        Debug.Assert(typeof(Session).IsAssignableFrom(parameterInfo[0].ParameterType));
                        Debug.Assert(typeof(IReadable).IsAssignableFrom(parameterInfo[1].ParameterType));

                        #endregion

                        var call = Expression.Call(method,
                            Expression.Convert(sessionParameter, parameterInfo[0].ParameterType),
                            Expression.Convert(messageParameter, parameterInfo[1].ParameterType));

                        var lambda =
                            Expression.Lambda<MessageHandlerDelegate>(call, sessionParameter, messageParameter);

                        handlers.Add(attribute.Opcode, lambda.Compile());
                    }
                    else
                    {
                        #region Debug

                        Debug.Assert(parameterInfo.Length == 1);
                        Debug.Assert(typeof(Session).IsAssignableFrom(type));
                        Debug.Assert(typeof(IReadable).IsAssignableFrom(parameterInfo[0].ParameterType));

                        #endregion

                        var call = Expression.Call(
                            Expression.Convert(sessionParameter, type),
                            method,
                            Expression.Convert(messageParameter, parameterInfo[0].ParameterType));

                        var lambda =
                            Expression.Lambda<MessageHandlerDelegate>(call, sessionParameter, messageParameter);

                        handlers.Add(attribute.Opcode, lambda.Compile());
                    }
                }
            }
            ClientMessageHandlers = handlers.ToImmutableDictionary();
            
            Log.Print(LogType.Server, $"Initialized {ClientMessageHandlers.Count} {(ClientMessageHandlers.Count == 1 ? "handler" : "handlers")}.");
        }
        
        public static IReadable GetMessage(Opcodes opcode)
        {
            return ClientMessageStructures.TryGetValue(opcode, out MessageFactoryDelegate factory)
                ? factory.Invoke() : null;
        }

        public static bool GetOpcodeTypes(IWritable message, out Opcodes opcode)
        {
            return ServerMessageStructures.TryGetValue(message.GetType(), out opcode);
        }

        public static MessageHandlerDelegate GetMessageHandler(Opcodes opcode)
        {
            return ClientMessageHandlers.TryGetValue(opcode, out MessageHandlerDelegate handler)
                ? handler : null;
        }
    }
}