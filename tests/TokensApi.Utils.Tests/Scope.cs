using System.Linq;
using System.Collections.Generic;

using Xunit;

namespace TokensApi.Utils.Tests;

public class ScopeTests
{
    [Fact]
    public void ScopeDefinitions_MustNotContainInvalidStrings()
    {
        var scopes = Scope.ScopeArray;

        var invalidScopes = scopes.Where(scope => string.IsNullOrWhiteSpace(scope));

        // must be empty
        Assert.Empty(invalidScopes);
    }

    [Fact]
    public void ScopeDefinitions_MustNotContainDuplicates()
    {
        var scopes = Scope.ScopeArray;

        // aggregate duplicates
        var cache = new Dictionary<string, int>();
        foreach (var scope in scopes)
        {
            if (cache.ContainsKey(scope))
            {
                cache[scope]++;
            }
            else
            {
                cache[scope] = 1;
            }
        }
        var duplicates = cache.Where(x => x.Value > 1).Select(x => x.Key);

        // must be empty
        Assert.Empty(duplicates);
    }
}