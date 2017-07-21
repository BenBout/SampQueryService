[license]: https://tldrlegal.com/l/mit

# SAMP Asynchronous Query Service [![License](http://img.shields.io/badge/license-MIT-lightgrey.svg?style=flat)][License]

A simple client service to query SA-MP servers. www.sa-mp.com.

## Supported platforms
 * all .NET compat with STANDARD 1.4  https://docs.microsoft.com/fr-fr/dotnet/standard/library
 
## Sample
```csharp
using System;
using System.Threading.Tasks;
using SampQueryService;
using SampQueryService.QueryResult;
using System.Net;
using System.Linq;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var serverIP = IPAddress.Parse("151.80.94.179");
            int port = 7777;

            var sampQuery = new SampQueryClient();
            var playerList = await sampQuery.SendQueryAsync<PlayerList>(serverIP, port);

            var filteredPlayerList = playerList.Players
                .Where(p => p.Level > 5)
                .OrderByDescending(p => p.Level);

            foreach (var player in filteredPlayerList)
                Console.WriteLine($"ID: {player.ID} Username: {player.UserName} Ping: {player.ping}");
            
            await Task.Delay(-1);
        }
    }
}
```
