using System.Collections.Generic;
using Data;
using Newtonsoft.Json;
namespace Network.Messages
{
    public class UnitUpdateMessage : INetworkMessage
    {
        public List<UnitData> units;

        public string Type => "UnitUpdate";

        public string Payload
        {
            get
            {
                return JsonConvert.SerializeObject(units);
            }
        }

        public UnitUpdateMessage(List<UnitData> units)
        {
            this.units = units;
        }
    }
}
