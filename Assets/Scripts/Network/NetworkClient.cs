using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Network
{
    public class NetworkClient
    {
        public event Action OnConnected;
        public event Action<NetMessage> OnMessageReceived;

        TcpClient client;
        NetworkStream stream;
        

        public async Task Connect(string ip, int port)
        {
            client = new TcpClient();
            await client.ConnectAsync(ip, port);

            stream = client.GetStream();
            OnConnected?.Invoke();

            _ = ReceiveLoop();
        }

        async Task ReceiveLoop()
        {
            try
            {
                while (client.Connected)
                {
                    var msg = await NetworkCommon.ReadMessage(stream);
                    OnMessageReceived?.Invoke(msg);
                }
            }
            catch { }
        }

        public async void Send(NetMessage msg)
        {
            if (client == null || !client.Connected)
                return;

            byte[] packet = NetworkCommon.BuildPacket(msg);
            await stream.WriteAsync(packet, 0, packet.Length);
        }
    }
}
