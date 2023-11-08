using System.Linq;
using Xunit;

using System.Collections.Generic;

namespace TokensApi.Utils.Tests;

public class ClientTests
{
    [Fact]
    public void ClientDefinitions_MustNotContainInvalidStrings()
    {
        var clients = Client.ClientArray;

        var invalidClients = clients.Where(client => string.IsNullOrWhiteSpace(client));

        // must be empty
        Assert.Empty(invalidClients);
    }

    [Fact]
    public void ClientDefinitions_MustNotContainDuplicates()
    {
        var clients = Client.ClientArray;

        // aggregate duplicates
        var cache = new Dictionary<string, int>();
        foreach (var client in clients)
        {
            if (cache.ContainsKey(client))
            {
                cache[client]++;
            }
            else
            {
                cache[client] = 1;
            }
        }
        var duplicates = cache.Where(x => x.Value > 1).Select(x => x.Key);

        // must be empty
        Assert.Empty(duplicates);
    }
}