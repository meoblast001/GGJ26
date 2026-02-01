using System;
using System.Net;
using System.Net.Sockets;
using Data;
using Network.Commands;
using Newtonsoft.Json;
using Unity.Netcode;
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
                
                networkManager.CustomMessagingManager.RegisterNamedMessageHandler("BroadcastUnitsUpdateCommand", HandleBroadcastUnitsUpdateCommandReceived);
            }
        }
        
        void HandleSendUnitCommandReceived(ulong senderClientId, FastBufferReader reader)
        {
            reader.ReadValueSafe(out string messagePayload);
            Debug.Log($"Received SendUnitCommand: {messagePayload}");
            OnUnitCommandDataReceived?.Invoke(JsonConvert.DeserializeObject<UnitCommandData>(messagePayload));
        }
        
        void HandleBroadcastUnitsUpdateCommandReceived(ulong senderClientId, FastBufferReader reader)
        {
            reader.ReadValueSafe(out string messagePayload);
            Debug.Log($"Received BroadcastUnitsUpdateCommand: {messagePayload}");
            OnUnitsUpdateDataReceived?.Invoke(JsonConvert.DeserializeObject<UnitsUpdateData>(messagePayload));
        }

        void OnDestroy()
        {
            if (networkManager != null)
            {
                if (networkManager.CustomMessagingManager != null)
                {
                    networkManager.CustomMessagingManager.UnregisterNamedMessageHandler("BroadcastUnitsUpdateCommand");
                    networkManager.CustomMessagingManager.UnregisterNamedMessageHandler("SendUnitCommand");
                }
                networkManager.OnClientConnectedCallback -= HandleClientConnected;
            }
        }
        void HandleClientConnected(ulong clientId)
        {
            if (isServer == false && clientId == networkManager.LocalClientId || isServer)
            {
                Debug.Log($"This is Server ({isServer}) Client Connected.");

                if (isServer == false)
                {
                    networkManager.CustomMessagingManager.RegisterNamedMessageHandler("SendUnitCommand", HandleSendUnitCommandReceived);
                }
                
                OnClientConnected?.Invoke();
            }
        }
        
        private void StartServer()
        {
            networkManager.StartServer();
            Debug.Log($"Server started. Ip: {GetLocalIPv4()}");
        }
        
        
        string GetLocalIPv4()
        {
            string localIP = "unknown";
            foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }

        public void StartClient(string serverIp)
        {
            var transport = networkManager.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = serverIp;
            Debug.Log($"Client Connecting to {serverIp}");
            networkManager.StartClient();
        }
    }
}
