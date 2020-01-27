using System.Net;
using System.Net.Sockets;
using System.Threading;

using MinecraftServer.Framework.Networking;
using MinecraftServer.Framework.Update;

namespace MinecraftServer.Networking
{
    public class Server : IUpdate
    {
        private TcpListener ServerListener;
        private Session Session;

        public Server(string IP, int Port)
        {
            ServerListener = new TcpListener(IPAddress.Parse(IP), Port);
            ServerListener.Start();

            new Thread(Start).Start();
        }
        
        public void Start()
        {
            while (true)
            {
                while (ServerListener.Pending())
                {
                    Session = new Session(ServerListener.AcceptSocket());
                }
            }
        }

        public void Update(double lastTick) => Session?.Update(lastTick);
    }
}