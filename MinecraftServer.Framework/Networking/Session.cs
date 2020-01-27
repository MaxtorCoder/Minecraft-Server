using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using MinecraftServer.Framework.Logging;
using MinecraftServer.Framework.Networking.IO;
using MinecraftServer.Framework.Networking.Message;
using MinecraftServer.Framework.Networking.Packet;
using MinecraftServer.Framework.Update;

namespace MinecraftServer.Framework.Networking
{
    public class Session : IUpdate
    {
        public bool Disconnected;
        public bool RequestDisconnect;
        public Heartbeat Heartbeat = new Heartbeat();
        
        private Socket Socket;
        private byte[] buffer = new byte[4096];
        
        private ConcurrentQueue<ClientPacket> IncomingPackets   = new ConcurrentQueue<ClientPacket>();
        private Queue<ServerPacket> OutgoingPackets             = new Queue<ServerPacket>();
        
        public Session(Socket socket)
        {
            if (Socket != null)
                throw new InvalidOperationException();
            
            Socket = socket;
            Log.Print(LogType.Network, "New client connected", GetRemoteEndpoint());
            
            Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);
        }

        private void ReceiveDataCallback(IAsyncResult ar)
        {
            try
            {
                var length = Socket.EndReceive(ar);
                if (length == 0)
                    return;

                var data = new byte[length];
                Buffer.BlockCopy(buffer, 0, data, 0, data.Length);
                OnData(data);

                Socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveDataCallback, null);
            }
            catch (Exception ex)
            {
                RequestDisconnect = true;
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void OnData(byte[] data)
        {
            var packet = new ClientPacket(data);
            IncomingPackets.Enqueue(packet);
        }
        
        public virtual void Update(double lastTick)
        {
            if (!RequestDisconnect)
            {
                Heartbeat.Update(lastTick);
                if (Heartbeat.Flatline)
                    RequestDisconnect = true;
            }
            else if (!Disconnected)
                OnDisconnect();
            
            while (IncomingPackets.TryDequeue(out var packet))
                HandlePacket(packet);
        }

        private void HandlePacket(ClientPacket packet)
        {
            var message = MessageManager.GetMessage(packet.Opcode);
            if (message == null)
            {
                Log.Print(LogType.Error, $"Received unknown packet {(uint)packet.Opcode} (0x{(uint)packet.Opcode:X4}, Size: {packet.Size})", GetRemoteEndpoint());
                return;
            }

            var handler = MessageManager.GetMessageHandler(packet.Opcode);
            if (handler == null)
            {
                Log.Print(LogType.Error, $"Received packet without handler {packet.Opcode} (0x{(uint)packet.Opcode:X4}, Size: {packet.Size})", GetRemoteEndpoint());
                return;
            }
            
            Log.Print(LogType.Network, $"Received packet {packet.Opcode} (0x{(uint)packet.Opcode:X4}, Size: {packet.Size})", GetRemoteEndpoint());
            Heartbeat.OnHeartBeat();

            using (var reader = new PacketReader(packet.Data))
            {
                message.Read(reader);
                if (reader.BytesRemaining > 1)
                    Log.Print(LogType.Warning, $"Could not read entire packet {packet.Opcode} (0x{(uint)packet.Opcode:X4}, Remaining: {reader.BytesRemaining})", GetRemoteEndpoint());

                try
                {
                    handler.Invoke(this, message);
                }
                catch (Exception ex)
                {
                    Log.Print(LogType.Error, ex.ToString());
                }
            }
        }

        private void SendRaw(byte[] data)
        {
            try
            {
                Socket.Send(data, 0, data.Length, SocketFlags.None);
            }
            catch
            {
                RequestDisconnect = true;
            }
        }

        private void OnDisconnect()
        {
            Log.Print(LogType.Network, "Client disconnected", GetRemoteEndpoint());
            
            Disconnected = true; 
            Socket.Close();
            
            IncomingPackets.Clear();
            OutgoingPackets.Clear();
        }

        private string GetRemoteEndpoint() => Socket.RemoteEndPoint.ToString();
    }
}