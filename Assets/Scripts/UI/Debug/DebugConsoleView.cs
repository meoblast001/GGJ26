using System.Collections.Generic;
using Data;
using Network;
using Network.Commands;
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
            SendCommandButton.onClick.AddListener(SendUnitUpdate);

            NetworkManager.Instance.OnMessageReceived += HandleMessage;
        }

        void SendUnitUpdate()
        {
            List<UnitData> units = new List<UnitData>();
            units.Add(new UnitData(1, Random.Range(0,10), Random.Range(0,10), "Idle"));
            units.Add(new UnitData(2, Random.Range(0,10), Random.Range(0,10), "Moving"));
            SendUnitUpdateCommand.Send(units);
        }
        
        void OnDestroy()
        {
            NetworkManager.Instance.OnMessageReceived -= HandleMessage;
        }
        
        void HandleMessage(NetMessage message)
        {
            switch (message.type)
            {
                case "SendUnitCommand":
                {
                    var data = JsonConvert.DeserializeObject<UnitCommandData>(message.payload);

                    ReceiveMessageText.text =
                        $"Command:\nUnit {data.unitId} → {data.targetUnitId}";
                    break;
                }
            }
        }
    }
}
