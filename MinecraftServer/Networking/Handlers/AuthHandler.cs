using MinecraftServer.Framework.Logging;
using MinecraftServer.Framework.Networking;
using MinecraftServer.Framework.Networking.Message.Attributes;
using MinecraftServer.Networking.Messages;

namespace MinecraftServer.Networking.Handlers
{
    public static class AuthHandler
    {
        [MessageHandler(Opcodes.ClientHandshake)]
        public static void HandleHandshake(Session session, ClientHandshake handshake)
        {
            Log.Print(LogType.Debug, $"Protocol Version: {handshake.ProtocolVersion}");
            Log.Print(LogType.Debug, $"Server Address: {handshake.ServerAddress}");
            Log.Print(LogType.Debug, $"Server Port: {handshake.ServerPort}");
            Log.Print(LogType.Debug, $"Next State: {handshake.NextState}");
        }
    }
}