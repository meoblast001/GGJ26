namespace Network
{
    public interface INetworkMessage
    {
        public string Type { get; }
        public string Payload { get; }
    }
}