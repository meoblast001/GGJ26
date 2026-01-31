using System;
using Data;
using Newtonsoft.Json;
using UnityEngine;

namespace Network
{
    public class NetworkManager : MonoBehaviour
    {
        public Action<UnitCommandData> OnUnitCommandDataReceived;
        public Action<UnitsUpdateData> OnUnitsUpdateDataReceived;

        public bool isServer;
        public int tcpPort = 5000;

        NetworkServer server;
        NetworkClient client;
        
        public static NetworkManager Instance { get; private set; }
        
        public NetworkServer NetworkServer => server;
        public NetworkClient NetworkClient => client;

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

        void Start()
        {
            if (isServer)
            {
                StartServer();
            }
            else
            {
                StartClient();
            }
        }

        async void StartServer()
        {
            server = new NetworkServer(tcpPort);

            server.OnMessageReceived += HandleMessage;
            server.OnClientConnected += _ => Debug.Log("Client connected");

            await server.Start();
        }

        void StartClient()
        {
            client = new NetworkClient();

            client.OnConnected += () => Debug.Log("Connected to server");
            client.OnMessageReceived += HandleMessage;
        }

        void HandleMessage(NetMessage message)
        {
            switch (message.type)
            {
                case "SendUnitCommand":
                    OnUnitCommandDataReceived?.Invoke(
                        JsonConvert.DeserializeObject<UnitCommandData>(message.payload)
                        );
                    break;

                case "UnitUpdate":
                    OnUnitsUpdateDataReceived?.Invoke(
                        JsonConvert.DeserializeObject<UnitsUpdateData>(message.payload)
                        );
                    break;
            }
        }
    }
}

