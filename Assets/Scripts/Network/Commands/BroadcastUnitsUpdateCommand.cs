using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
namespace Network.Commands
{
    public static class BroadcastUnitsUpdateCommand
    {
        public static void Send(List<UnitData> units)
        {
            var data = new UnitsUpdateData
            {
                units = units
            };
            
            using var writer = new FastBufferWriter(128, Allocator.Temp);
            writer.WriteValueSafe(JsonConvert.SerializeObject(data));

            Debug.Log($"Sending BroadcastUnitsUpdateCommand: {JsonConvert.SerializeObject(data)}");
            NetworkController.Instance.networkManager.CustomMessagingManager.SendNamedMessageToAll("BroadcastUnitsUpdateCommand", writer);
        }
    }
}
