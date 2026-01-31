using Data;
using Newtonsoft.Json;
namespace Network.Commands
{
    public static class SendUnitCommand
    {
        public static void Send(int unitId, int targetUnitId)
        {
            var data = new UnitCommandData
            {
                unitId = unitId,
                targetUnitId = targetUnitId
            };

            var msg = new NetMessage
            {
                type = "SendUnitCommand",
                payload = JsonConvert.SerializeObject(data)
            };

            NetworkManager.Instance.SendToServer(msg);
        }
    }
}
