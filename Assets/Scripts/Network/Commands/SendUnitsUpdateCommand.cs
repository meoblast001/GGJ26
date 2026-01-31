using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
namespace Network.Commands
{
    public static class SendUnitsUpdateCommand
    {
        public static void Send(List<UnitData> units)
        {
            var data = new UnitsUpdateData
            {
                units = units
            };

            var msg = new NetMessage
            {
                type = "UnitUpdate",
                payload = JsonConvert.SerializeObject(data)
            };

            NetworkManager.Instance.SendToAllClients(msg);
        }
    }
}
