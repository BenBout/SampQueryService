using System;
using System.Net;
using System.Threading.Tasks;
using SampQueryService.QueryResult;

namespace SampQueryService
{
    /// <summary>
    /// Client class to send async query to sa-mp servers.
    /// </summary>
    public class SampQueryClient
    {
        /// <summary>
        /// Send an asynchronous query to the sa-mp server.
        /// Type param must be an inherits of SampQueryResult 
        /// return depend from it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException">String ip is not in a valid format.</exception>
        public Task<T> SendQueryAsync<T>(string ip, int port) where T : SampQueryResult, new()
        {
            IPAddress cleanIP;

            var isValidIP = IPAddress.TryParse(ip, out cleanIP);
            if (!isValidIP) throw new FormatException("String ip is not in a valid format.");

            return SendQueryAsync<T>(cleanIP, port);
        }

        /// <summary>
        /// Send an asynchronous query to the sa-mp server.
        /// Type param must be an inherits of SampQueryResult 
        /// return depend from it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        public Task<T> SendQueryAsync<T>(IPAddress ip, int port) where T : SampQueryResult, new()
        {

            return SendQueryAsync<T>(new IPEndPoint(ip, port));
        }

        /// <summary>
        /// Send an asynchronous query to the sa-mp server.
        /// Type param must be an inherits of SampQueryResult 
        /// return depend from it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ipEnd">An IPendpoint instance.</param>
        /// <returns></returns>
        public async Task<T> SendQueryAsync<T>(IPEndPoint ipEnd) where T : SampQueryResult, new()
        {
            var query = new SampQuery(ipEnd);
            var obj = new T();

            var receivedPacketsTask = query.ReceiveAsync();
            var sendQueryTask = query.SendAsync(obj.OpCode);

            await Task.WhenAll(receivedPacketsTask, sendQueryTask);
            var rPackets = receivedPacketsTask.Result;

            if (rPackets != null)
            {
                obj.IsCompleted = true;
                obj.Deserialize(rPackets);
            }

            return obj;
        }

        /// <summary>
        /// Send an asynchronous rcon query to the sa-mp server.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="password">The password.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public Task SendRconQueryAsync(IPAddress ip, int port, string password, string command)
        {
            return SendRconQueryAsync(new IPEndPoint(ip, port), password, command);
        }

        /// <summary>
        /// Send an asynchronous rcon query to the sa-mp server.
        /// </summary>
        /// <param name="ipEnd">The ip end.</param>
        /// <param name="password">The password.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public async Task SendRconQueryAsync(IPEndPoint ipEnd, string password, string command)
        {
            var query = new SampQuery(ipEnd);

            var receivedPacketsTask = query.ReceiveAsync();
            await query.SendRconAsync(password, command);
            //var rPackets = await receivedPacketsTask;
        }
    }
}
