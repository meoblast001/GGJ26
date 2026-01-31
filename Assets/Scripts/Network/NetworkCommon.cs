using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Network
{
    public static class NetworkCommon
    {
        public static byte[] BuildPacket(NetMessage message)
        {
            string json = JsonConvert.SerializeObject(message);
            byte[] payload = Encoding.UTF8.GetBytes(json);

            byte[] length = BitConverter.GetBytes(payload.Length);
            byte[] packet = new byte[4 + payload.Length];

            Buffer.BlockCopy(length, 0, packet, 0, 4);
            Buffer.BlockCopy(payload, 0, packet, 4, payload.Length);

            return packet;
        }

        public static async Task<NetMessage> ReadMessage(NetworkStream stream)
        {
            byte[] lengthBuffer = new byte[4];
            await ReadExact(stream, lengthBuffer, 4);

            int length = BitConverter.ToInt32(lengthBuffer, 0);
            byte[] data = new byte[length];

            await ReadExact(stream, data, length);

            return JsonConvert.DeserializeObject<NetMessage>(
                Encoding.UTF8.GetString(data)
                );
        }

        private static async Task ReadExact(NetworkStream stream, byte[] buffer, int size)
        {
            int read = 0;
            while (read < size)
            {
                int r = await stream.ReadAsync(buffer, read, size - read);
                if (r == 0) throw new Exception("Disconnected");
                read += r;
            }
        }
    }
}
