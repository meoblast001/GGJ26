using Newtonsoft.Json;
namespace Network.Messages
{
    public class SendUnitCommandMessage : INetworkMessage
    {
        public int unitId;
        public int targetUnitId;
        
        public string Type
        {
            get
            {
                return "SendUnitCommand";
            }
        }
        
        public string Payload 
        {
            get
            {
                var data = new
                {
                    unitId,
                    targetUnitId
                };
                return JsonConvert.SerializeObject(data);
            }
        }

        public SendUnitCommandMessage(int unitId, int targetUnitId)
        {
            this.unitId = unitId;
            this.targetUnitId = targetUnitId;
        }
    }
}
