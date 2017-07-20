using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace SampQueryService
{
    public class SampQueryClient
    {
        public int ReceiveTimeout { get; set; } = 2000;
        public int SendTimeout { get; set; } = 2000;
        //public int ListeningPort { get; set; } = 22387;

        public Task<T> SendQueryAsync<T>(string ip, int port) where T : IQueryDataResult, new()
        {
            IPAddress cleanIP;

            var isValidIP = IPAddress.TryParse(ip, out cleanIP);
            if (!isValidIP) throw new FormatException("String ip is not in a valid fromat.");

            return SendQueryAsync<T>(cleanIP, port);
        }

        public Task<T> SendQueryAsync<T>(IPAddress ip, int port) where T : IQueryDataResult, new()
        {
            
            return SendQueryAsync<T>(new IPEndPoint(ip, port));
        }

        public async Task<T> SendQueryAsync<T>(IPEndPoint ipEnd) where T : IQueryDataResult, new()
        {
            var query = new SampQuery(ipEnd);
            var obj = new T();

            var receivedPacketsTask = query.ReceiveAsync();
            var sendQueryTask = await query.SendAsync(obj.GetOpCode());
            //await Task.WhenAll(receivedPacketsTask, sendQueryTask);

            var rPackets = await receivedPacketsTask;

            var result = obj.Deserialize(rPackets);
            if (result == false) obj = default(T);
            return obj;
        }

        public Task SendRconQueryAsync(IPAddress ip, int port, string password, string command)
        {
            return SendRconQueryAsync(new IPEndPoint(ip, port), password, command);    
        }
        
        public async Task SendRconQueryAsync(IPEndPoint ipEnd, string password, string command)
        {
            var query = new SampQuery(ipEnd);

            var receivedPacketsTask = query.ReceiveAsync();
            await query.SendRconAsync(password, command);
            var rPackets = await receivedPacketsTask;
        }
        /*
        public async Task<int> PingAsync(IPAddress ip, int port)
        {
            var query = new SampQuery(new IPEndPoint(ip, port));
            var pingWatch = new Stopwatch();
            pingWatch.Start();

            var receivedPacketsTask = query.ReceiveAsync();
            var sendQueryTask = await query.SendAsync('p');
            await receivedPacketsTask;

            pingWatch.Stop();
            return (int)pingWatch.ElapsedMilliseconds;
        }

        public Task<int> PingAsync(string ip, int port)
        {
            IPAddress cleanIP;

            var isValidIP = IPAddress.TryParse(ip, out cleanIP);
            if (!isValidIP) throw new FormatException("String ip is not in a valid fromat.");

            return PingAsync(cleanIP, port);
        }
        */
    }
}
