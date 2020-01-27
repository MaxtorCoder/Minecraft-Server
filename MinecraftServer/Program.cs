using System;
using MinecraftServer.Framework.Logging;
using MinecraftServer.Framework.Networking.Message;
using MinecraftServer.Framework.Update;
using MinecraftServer.Networking;

namespace MinecraftServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server("127.0.0.1", 25565);
            Log.Print(LogType.Server, "Server successfully started!");
            
            MessageManager.Initialize();
            UpdateManager.Initialize(lastTick =>
            {
                server.Update(lastTick);
            });
        }
    }
}