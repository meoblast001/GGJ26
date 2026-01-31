using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {
        public bool isServer = true;
        public string serverIP = "127.0.0.1";
        public int port = 5000;
        public Action<INetworkMessage> OnMessageReceived;

        TcpListener server;
        TcpClient client;
        NetworkStream stream;
        
        async void Start()
        {
            try
            {
                if (isServer)
                {
                    await StartServer();
                }
                else
                {
                    await ConnectToServer();
                }
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }
        
        async Task StartServer()
        {
            server = new TcpListener(IPAddress.Any, port);
            server.Start();
            Debug.Log("Server started on port " + port);

            while (true)
            {
                var tcpClient = await server.AcceptTcpClientAsync();
                Debug.Log("Client connected");
                _ = HandleClient(tcpClient);
            }
        }
        
        async Task HandleClient(TcpClient tcpClient)
        {
            var netStream = tcpClient.GetStream();
            byte[] buffer = new byte[4096];

            while (tcpClient.Connected)
            {
                int bytesRead = 0;
                try
                {
                    bytesRead = await netStream.ReadAsync(buffer, 0, buffer.Length);
                }
                catch
                {
                    break;
                }

                if (bytesRead == 0)
                {
                    break;
                }

                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var msg = JsonConvert.DeserializeObject<INetworkMessage>(json);
                OnMessageReceived?.Invoke(msg);
            }

            tcpClient.Close();
        }
        
        async Task ConnectToServer()
        {
            client = new TcpClient();
            Debug.Log("Attempting to server");
            await client.ConnectAsync(serverIP, port);
            stream = client.GetStream();
            Debug.Log("Connected to server");

            _ = ReceiveLoop();
        }
        
        public async void SendToServer(INetworkMessage message)
        {
            if (client.Connected)
            {
                string json = JsonConvert.SerializeObject(message);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                await stream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
        
        async Task ReceiveLoop()
        {
            byte[] buffer = new byte[4096];
            while (client.Connected)
            {
                int bytesRead = 0;
                try { bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); } 
                catch { break; }

                if (bytesRead == 0) break;

                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var msg = JsonConvert.DeserializeObject<INetworkMessage>(json);
                OnMessageReceived?.Invoke(msg);
            }
        }
    }
}
