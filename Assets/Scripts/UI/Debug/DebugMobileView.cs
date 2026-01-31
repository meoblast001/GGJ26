using Network;
using Network.Messages;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Debug
{
    public class DebugMobileView : MonoBehaviour
    {
        public Button SendCommandButton;
        public TextMeshProUGUI ReceiveMessageText;
    
        void Start()
        {
            SendCommandButton.onClick.AddListener(SendDebugMessage);
            NetworkManager.Instance.OnMessageReceived += HandleMessage;
        }

        void SendDebugMessage()
        {
            NetworkManager.Instance.SendToServer(new SendUnitCommandMessage(Random.Range(0,10), Random.Range(0,10)));
        }
        
        void HandleMessage(INetworkMessage message)
        {
            switch (message.Type)
            {
                case "UnitUpdate":
                    var payload = JsonConvert.DeserializeObject<UnitUpdateMessage>(message.Payload);

                    string displayText = $"Received Unit Update ({payload.units.Count} units):\n";
                    foreach (var unit in payload.units)
                    {
                        displayText += $"ID: {unit.id} | Pos: ({unit.x:F1}, {unit.y:F1}) | State: {unit.state}\n";
                    }

                    ReceiveMessageText.text = displayText;

                    break;
                
                default:
                    ReceiveMessageText.text = $"Unknown message type: {message.Type}\nPayload: {message.Payload}";
                    break;
            }
        }
    }
}
