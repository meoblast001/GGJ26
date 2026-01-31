using System.Collections.Generic;
using Data;
using Network;
using Network.Messages;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Debug
{
    public class DebugConsoleView : MonoBehaviour
    {
        public Button SendCommandButton;
        public TextMeshProUGUI ReceiveMessageText;
    
        void Start()
        {
            SendCommandButton.onClick.AddListener(SendDebugMessage);

            NetworkManager.Instance.OnMessageReceived += HandleMessage;
        }
        void HandleMessage(INetworkMessage message)
        {
            switch (message.Type)
            {
                case "SendUnitCommand":
                    var payload = JsonConvert.DeserializeObject<SendUnitCommandMessage>(message.Payload);

                    ReceiveMessageText.text = $"Command Received:\n" +
                                              $"Unit ID: {payload.unitId}\n" +
                                              $"Target Unit ID: {payload.targetUnitId}";
                    break;
                default:
                    ReceiveMessageText.text = $"Unknown message type: {message.Type}\nPayload: {message.Payload}";
                    break;
            }
        }

        void SendDebugMessage()
        {
            List<UnitData> units = new List<UnitData>();
            units.Add(new UnitData(1, Random.Range(0,10), Random.Range(0,10), "Idle"));
            units.Add(new UnitData(2, Random.Range(0,10), Random.Range(0,10), "Moving"));
            NetworkManager.Instance.SendToAllClients(new UnitUpdateMessage(units));
        }

        void OnDestroy()
        {
            NetworkManager.Instance.OnMessageReceived -= HandleMessage;
        }
    }
}
