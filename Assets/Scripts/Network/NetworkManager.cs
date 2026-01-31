using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        public bool isServer = true;
        public string serverIP = "127.0.0.1";
        public int port = 5000;

        public Action<NetMessage> OnMessageReceived;

        TcpListener server;
        List<TcpClient> connectedClients = new();

        TcpClient client;
        NetworkStream clientStream;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        async void Start()
        {
            if (isServer)
                await StartServer();
            else
                await StartClient();
        }

        // ================= SERVER =================

        async Task StartServer()
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Debug.Log($"[Server] Listening on port {port}");

            while (true)
            {
                var tcpClient = await server.AcceptTcpClientAsync();
                connectedClients.Add(tcpClient);
                Debug.Log("[Server] Client connected");

                _ = HandleClient(tcpClient);
            }
        }

        async Task HandleClient(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();

            try
            {
                while (tcpClient.Connected)
                {
                    NetMessage msg = await ReadMessage(stream);
                    Debug.Log($"[Server] Received: {msg.type} {msg.payload}");
                    OnMessageReceived?.Invoke(msg);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Server] Client disconnected: " + e.Message);
            }
            finally
            {
                connectedClients.Remove(tcpClient);
                tcpClient.Close();
            }
        }

        public async void SendToAllClients(NetMessage message)
        {
            byte[] packet = BuildPacket(message);

            foreach (var c in connectedClients.ToArray())
            {
                if (!c.Connected)
                {
                    connectedClients.Remove(c);
                    continue;
                }

                try
                {
                    await c.GetStream().WriteAsync(packet, 0, packet.Length);
                    await c.GetStream().FlushAsync();
                    Debug.Log($"[Server] Sent: {message.type}");
                }
                catch
                {
                    connectedClients.Remove(c);
                }
            }
        }

        // ================= CLIENT =================

        async Task StartClient()
        {
            client = new TcpClient();
            await client.ConnectAsync(serverIP, port);
            clientStream = client.GetStream();
            Debug.Log("[Client] Connected to server");

            _ = ReceiveLoop();
        }

        async Task ReceiveLoop()
        {
            try
            {
                while (client.Connected)
                {
                    NetMessage msg = await ReadMessage(clientStream);
                    Debug.Log($"[Client] Received: {msg.type} {msg.payload}");
                    OnMessageReceived?.Invoke(msg);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("[Client] Disconnected: " + e.Message);
            }
        }

        public async void SendToServer(NetMessage message)
        {
            if (client == null || !client.Connected) return;

            byte[] packet = BuildPacket(message);
            await clientStream.WriteAsync(packet, 0, packet.Length);
            await clientStream.FlushAsync();

            Debug.Log($"[Client] Sent: {message.type}");
        }

        // ================= SHARED =================

        byte[] BuildPacket(NetMessage message)
        {
            string json = JsonConvert.SerializeObject(message);
            byte[] payload = Encoding.UTF8.GetBytes(json);

            byte[] length = BitConverter.GetBytes(payload.Length);
            byte[] packet = new byte[length.Length + payload.Length];

            Buffer.BlockCopy(length, 0, packet, 0, 4);
            Buffer.BlockCopy(payload, 0, packet, 4, payload.Length);

            return packet;
        }

        async Task<NetMessage> ReadMessage(NetworkStream stream)
        {
            byte[] lengthBuffer = new byte[4];
            await ReadExact(stream, lengthBuffer, 4);

            int length = BitConverter.ToInt32(lengthBuffer, 0);
            byte[] data = new byte[length];
            await ReadExact(stream, data, length);

            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<NetMessage>(json);
        }

        async Task ReadExact(NetworkStream stream, byte[] buffer, int size)
        {
            int read = 0;
            while (read < size)
            {
                int r = await stream.ReadAsync(buffer, read, size - read);
                if (r == 0) throw new Exception("Disconnected");
                read += r;
            }
        }
    }
}
