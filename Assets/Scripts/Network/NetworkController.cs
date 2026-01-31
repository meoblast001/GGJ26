using System;
using Data;
using Newtonsoft.Json;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
namespace Network
{
    public class NetworkController : MonoBehaviour
    {
        public Action OnClientConnected;

        public Action<UnitCommandData> OnUnitCommandDataReceived;
        public Action<UnitsUpdateData> OnUnitsUpdateDataReceived;
        
        public Unity.Netcode.NetworkManager networkManager;
        public bool isServer;

        public static NetworkController Instance { get; private set; }
        
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
        
        public void Initialize()
        {
            networkManager.OnClientConnectedCallback += HandleClientConnected;

            if (isServer)
            {
                StartServer();
            }
        }

        void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.OnClientConnectedCallback -= HandleClientConnected;
            }
        }
        void HandleServerConnected(ulong clientId)
        {
            if (isServer ==  false && clientId == networkManager.LocalClientId || isServer)
            {
                OnClientConnected?.Invoke();
            }
        }

        void HandleClientConnected(ulong obj)
        {
            OnClientConnected?.Invoke();
        }
        
        public void StartServer()
        {
            networkManager.StartServer();
        }

        public void StartClient(string serverIp)
        {
            var transport = networkManager.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = serverIp;
            networkManager.StartClient();
        }
        
        void HandleMessage(NetMessage message)
        {
            switch (message.type)
            {
                case "SendUnitCommand":
                    OnUnitCommandDataReceived?.Invoke(JsonConvert.DeserializeObject<UnitCommandData>(message.payload));
                    break;

                case "UnitUpdate":
                    OnUnitsUpdateDataReceived?.Invoke(JsonConvert.DeserializeObject<UnitsUpdateData>(message.payload));
                    break;
            }
        }
    }
}
