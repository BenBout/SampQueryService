[license]: https://tldrlegal.com/l/mit

# SAMP Asynchronous Query Service [![License](http://img.shields.io/badge/license-MIT-lightgrey.svg?style=flat)][License]

A simple client service to query SA-MP servers. www.sa-mp.com.

## Supported platforms
 * all .NET compat with STANDARD 1.4  https://docs.microsoft.com/fr-fr/dotnet/standard/library
 * .NET CORE 1.1.
 * if you want a .net framework <= 46, build the projet  yourself with the correct target framework or contact me.

## Nuget
 * https://www.nuget.org/packages/SampQueryService/1.0.1
 
## Installing
 * To install Pastebin.cs, run the following command in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console)

    PM> Install-Package SampQueryService

## Sample
```csharp
static async Task SimpleQuery()
{
    var serverIP = IPAddress.Parse("151.80.94.179");
    int port = 7777;

    var sampQuery = new SampQueryClient();
    var playerList = await sampQuery.SendQueryAsync<PlayerList>(serverIP, port);


    // IsCompletedProperty returns false when 
    // query timeout or playerlist is upper 100.
    // http://wiki.sa-mp.com/wiki/Query_Mechanism
    if (playerList.IsCompleted)
    {
        var filteredPlayerList = playerList.Players
        .Where(p => p.Level > 5)
        .OrderByDescending(p => p.Level);

        foreach (var player in filteredPlayerList)
            Console.WriteLine($"ID: {player.ID} Username: {player.UserName} Ping: {player.ping}");
    }
}

static async Task MultipleQueriesWithSameQueryResultType()
{
    var ipEndList = new List<IPEndPoint>(); // we suppose a list of 10 ipendpoint.
    var serverInfoList = await  new SampQueryClient().SendQueryAsync<ServerInfo>(ipEndList);

    foreach (var server in serverInfoList)
    {
        if(server.IsCompleted)
            Console.WriteLine($"Server name: {server.HostName}");
    }
}
```
