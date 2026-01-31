using Data;
using Network;
using Network.Commands;
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
            SendCommandButton.onClick.AddListener(SendUnitCommand);
            NetworkManager.Instance.OnMessageReceived += HandleMessage;
        }
        
        void SendUnitCommand()
        {
            SendUnitCommandCommand.Send(Random.Range(0,10), Random.Range(0,10));
        }
        
        void OnDestroy()
        {
            NetworkManager.Instance.OnMessageReceived -= HandleMessage;
        }
        
        void HandleMessage(NetMessage message)
        {
            switch (message.type)
            {
                case "UnitUpdate":
                {
                    var data = JsonConvert.DeserializeObject<UnitUpdateData>(message.payload);

                    string text = $"Units: {data.units.Count}\n";
                    foreach (var u in data.units)
                    {
                        text += $"{u.id} ({u.x},{u.y}) {u.state}\n";
                    }
                    ReceiveMessageText.text = text;
                    break;
                }
            }
        }
    }
}
