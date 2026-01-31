using Data;
using Network;
using Network.Commands;
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
            NetworkManager.Instance.OnUnitsUpdateDataReceived += HandleUnitsUpdateReceived;
        }
        
        void SendUnitCommand()
        {
            SendUnitCommandCommand.Send(Random.Range(0,10), Random.Range(0,10));
        }
        
        void OnDestroy()
        {
            NetworkManager.Instance.OnUnitsUpdateDataReceived -= HandleUnitsUpdateReceived;
        }
        
        void HandleUnitsUpdateReceived(UnitsUpdateData data)
        {
            string text = $"Units: {data.units.Count}\n";
            foreach (var u in data.units)
            {
                text += $"{u.id} ({u.x},{u.y}) {u.state}\n";
            }
            ReceiveMessageText.text = text;
        }
    }
}
