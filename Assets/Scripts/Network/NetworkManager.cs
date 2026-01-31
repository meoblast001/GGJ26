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

        // --- Server fields ---
        private TcpListener serverListener;
        private List<TcpClient> connectedClients = new List<TcpClient>();

        // --- Client fields ---
        private TcpClient tcpClient;
        private NetworkStream clientStream;

        public Action<INetworkMessage> OnMessageReceived;

        private void Awake()
        {
            if (Instance != null && Instance != this)
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
                await ConnectToServer();
        }

        #region Server
        async Task StartServer()
        {
            serverListener = new TcpListener(IPAddress.Any, port);
            serverListener.Start();
            Debug.Log("Server started on port " + port);

            while (true)
            {
                TcpClient newClient = await serverListener.AcceptTcpClientAsync();
                Debug.Log("Client connected: " + newClient.Client.RemoteEndPoint);
                connectedClients.Add(newClient);
                _ = HandleClient(newClient);
            }
        }

        async Task HandleClient(TcpClient tcpClient)
        {
            NetworkStream stream = tcpClient.GetStream();
            byte[] buffer = new byte[4096];

            try
            {
                while (tcpClient.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break; // client disconnected

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var msg = JsonConvert.DeserializeObject<INetworkMessage>(json);

                    OnMessageReceived?.Invoke(msg);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Client disconnected: " + e.Message);
            }
            finally
            {
                connectedClients.Remove(tcpClient);
                tcpClient.Close();
            }
        }

        public async void SendToAllClients(INetworkMessage message)
        {
            string json = JsonConvert.SerializeObject(message);
            byte[] bytes = Encoding.UTF8.GetBytes(json);

            for (int i = connectedClients.Count - 1; i >= 0; i--)
            {
                TcpClient client = connectedClients[i];
                if (!client.Connected)
                {
                    connectedClients.RemoveAt(i);
                    continue;
                }
                try
                {
                    await client.GetStream().WriteAsync(bytes, 0, bytes.Length);
                }
                catch
                {
                    connectedClients.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Client
        async Task ConnectToServer()
        {
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(serverIP, port);
            clientStream = tcpClient.GetStream();
            Debug.Log("Connected to server");

            _ = ReceiveLoop();
        }

        async Task ReceiveLoop()
        {
            byte[] buffer = new byte[4096];
            try
            {
                while (tcpClient.Connected)
                {
                    int bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var msg = JsonConvert.DeserializeObject<INetworkMessage>(json);
                    OnMessageReceived?.Invoke(msg);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning("Disconnected from server: " + e.Message);
            }
        }

        public async void SendToServer(INetworkMessage message)
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                string json = JsonConvert.SerializeObject(message);
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                await clientStream.WriteAsync(bytes, 0, bytes.Length);
            }
        }
        #endregion
    }
}
