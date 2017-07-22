using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace SampQueryService
{
    internal class SampQuery
    {
        private UdpClient _client;
        private IPEndPoint _ipEndP;

        public SampQuery(IPEndPoint endPoint)
        {
            _client = new UdpClient(endPoint.Port);
            _ipEndP = endPoint;
        }

        // TODO: refactor sendasync & rconasync
        public async Task<bool> SendAsync(char opCode)
        {
            byte[] datagram;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    var ip = _ipEndP.Address.GetAddressBytes();

                    writer.Write("SAMP".ToCharArray());             // byte 0 - 3
                    foreach (var pack in ip) writer.Write(pack);    // byte 4 - 7
                    writer.Write((ushort)_ipEndP.Port);             // byte 8 - 9
                    writer.Write(opCode);                           // byte 10
                }
                datagram = stream.ToArray();
            }

            var result = await _client.SendAsync(datagram, datagram.Length, _ipEndP);
            
            if (result != 11) return false; // corrupted paquets
            return true;
        }

        public async Task SendRconAsync(string password, string command)
        {
            byte[] datagram;
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    var ip = _ipEndP.Address.GetAddressBytes();

                    writer.Write("SAMP".ToCharArray());             // byte 0 - 3
                    foreach (var pack in ip) writer.Write(pack);    // byte 4 - 7
                    writer.Write((ushort)_ipEndP.Port);             // byte 8 - 9
                    writer.Write('x');                              // byte 10

                    writer.Write((ushort) password.Length);
                    writer.Write(password.ToCharArray());

                    writer.Write((ushort)command.Length);
                    writer.Write(command.ToCharArray());
                }
                datagram = stream.ToArray();
            }

            var result = await _client.SendAsync(datagram, datagram.Length, _ipEndP);
        }

        public async Task<byte[]> ReceiveAsync()
        {
            var receiveTask = _client.ReceiveAsync();
            var timeoutTask = TimeOutAsync();
            await Task.WhenAny(receiveTask, timeoutTask);
            _client.Client.Dispose();

            if (!receiveTask.IsCompleted) // Timeout
                return null;

            var packets = receiveTask.Result;
            byte[] cleanPackets;

            if (packets.Buffer.Length > 11)
            {
                // clean packets (without packets sended to the server)
                cleanPackets = new byte[packets.Buffer.Length - 11];
                int cleanPacketsCount = 0;
                for (int i = 11; i < packets.Buffer.Length; i++)
                {
                    cleanPackets[cleanPacketsCount] = packets.Buffer[i];
                    cleanPacketsCount += 1;
                }
            }
            else
            {
                cleanPackets = null;
            }

            return cleanPackets;
        }

        private Task TimeOutAsync() => Task.Delay(1500);
    }
}