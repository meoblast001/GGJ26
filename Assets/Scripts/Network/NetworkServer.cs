using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Network
{
    public class NetworkServer
    {
        public event Action<TcpClient> OnClientConnected;
        public event Action<NetMessage> OnMessageReceived;

        readonly int tcpPort;
        readonly string broadcastName;

        TcpListener listener;
        readonly List<TcpClient> clients = new();

        public NetworkServer(int tcpPort, string broadcastName)
        {
            this.tcpPort = tcpPort;
            this.broadcastName = broadcastName;
        }

        public async Task Start()
        {
            listener = new TcpListener(IPAddress.Any, tcpPort);
            listener.Start();
            
            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                clients.Add(client);
                OnClientConnected?.Invoke(client);
                _ = HandleClient(client);
            }
        }

        async Task HandleClient(TcpClient client)
        {
            var stream = client.GetStream();

            try
            {
                while (client.Connected)
                {
                    var msg = await NetworkCommon.ReadMessage(stream);
                    OnMessageReceived?.Invoke(msg);
                }
            }
            catch { }
            finally
            {
                clients.Remove(client);
                client.Close();
            }
        }

        public async void SendToAll(NetMessage msg)
        {
            byte[] packet = NetworkCommon.BuildPacket(msg);

            foreach (var c in clients.ToArray())
            {
                try
                {
                    await c.GetStream().WriteAsync(packet, 0, packet.Length);
                }
                catch
                {
                    clients.Remove(c);
                }
            }
        }
    }
}
