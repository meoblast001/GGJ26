using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
namespace Network
{
    public class NetworkClient
    {
        public event Action OnConnected;
        public event Action<NetMessage> OnMessageReceived;

        readonly int udpPort;
        readonly string broadcastName;

        TcpClient client;
        NetworkStream stream;

        public NetworkClient(int udpPort, string broadcastName)
        {
            this.udpPort = udpPort;
            this.broadcastName = broadcastName;
        }

        public async Task DiscoverAndConnect()
        {
            using var udp = new UdpClient(udpPort);

            while (true)
            {
                var result = await udp.ReceiveAsync();
                string msg = Encoding.UTF8.GetString(result.Buffer);

                if (!msg.StartsWith(broadcastName))
                    continue;

                string[] parts = msg.Split('|');
                int tcpPort = int.Parse(parts[1]);
                string ip = result.RemoteEndPoint.Address.ToString();

                await Connect(ip, tcpPort);
                break;
            }
        }

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
