using System.Collections.Generic;
using Data;
using Network;
using Network.Commands;
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
            SendCommandButton.onClick.AddListener(SendUnitUpdate);

            NetworkManager.Instance.OnUnitCommandDataReceived += HandleMessage;
        }

        void SendUnitUpdate()
        {
            List<UnitData> units = new List<UnitData>();
            units.Add(new UnitData(1, Random.Range(0,10), Random.Range(0,10), "Idle"));
            units.Add(new UnitData(2, Random.Range(0,10), Random.Range(0,10), "Moving"));
            SendUnitsUpdateCommand.Send(units);
        }
        
        void OnDestroy()
        {
            NetworkManager.Instance.OnUnitCommandDataReceived -= HandleMessage;
        }
        
        void HandleMessage(UnitCommandData data)
        {
            ReceiveMessageText.text =
                $"Command:\nUnit {data.unitId} → {data.targetUnitId}";
        }
    }
}
