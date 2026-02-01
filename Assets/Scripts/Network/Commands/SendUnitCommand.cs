using Data;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
namespace Network.Commands
{
    public static class SendUnitCommand
    {
        public static void Send(int unitId)
        {
            var data = new UnitCommandData
            {
                unitId = unitId,
            };
            
            using var writer = new FastBufferWriter(128, Allocator.Temp);
            writer.WriteValueSafe(JsonConvert.SerializeObject(data));

            Debug.Log($"Sending SendUnitCommand: {JsonConvert.SerializeObject(data)}");
            NetworkController.Instance.networkManager.CustomMessagingManager
                .SendNamedMessage("SendUnitCommand", NetworkManager.ServerClientId, writer);
        }
    }
}
