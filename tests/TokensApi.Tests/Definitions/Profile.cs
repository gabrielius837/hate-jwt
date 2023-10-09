using Xunit;
using TokensApi.Utils;
using System.Linq;
using System;

namespace TokensApi.Tests;

public class ProfileTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   \t\n  ")]
    public void Profile_MustReturnNull_WhenCliendIdIsInvalid(string? clientId)
    {
        var profileSetting = GetValidProfileSetting();
        profileSetting.ClientId = clientId;

        var config = profileSetting.TryBuild();

        Assert.Null(config);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   \t\n  ")]
    public void Profile_MustReturnNull_WhenCliendSecretIsInvalid(string? clientSecret)
    {
        var profileSetting = GetValidProfileSetting();
        profileSetting.ClientSecret = clientSecret;

        var config = profileSetting.TryBuild();

        Assert.Null(config);
    }

    [Fact]
    public void Profile_MustReturnNull_WhenScopeArrayIsNull()
    {
        var profileSetting = GetValidProfileSetting();
        profileSetting.Scopes = null;

        var config = profileSetting.TryBuild();

        Assert.Null(config);
    }

    [Fact]
    public void Profile_MustReturnNull_WhenScopeArrayIsEmpty()
    {
        var profileSetting = GetValidProfileSetting();
        profileSetting.Scopes = Array.Empty<string>();

        var config = profileSetting.TryBuild();

        Assert.Null(config);
    }

    [Fact]
    public void Profile_MustReturnNull_WhenScopeArrayConstainsInvalidScope()
    {
        var profileSetting = GetValidProfileSetting();
        var invalidScope = "invalid-scope";
        profileSetting.Scopes = new string[] { invalidScope };

        var invalid = !Scopes.ScopeExists(invalidScope);
        var config = profileSetting.TryBuild();

        Assert.True(invalid);
        Assert.Null(config);
    }

    [Fact]
    public void Profile_MustNotReturnNull_WhenSettingsAreValid()
    {
        var profileSetting = GetValidProfileSetting();

        var config = profileSetting.TryBuild();

        Assert.NotNull(config);
    }

    public static ProfileSetting GetValidProfileSetting()
    {
        return new ProfileSetting()
        {
            ClientId = Clients.UnspecifiedClient,
            ClientSecret = "some-secret",
            Scopes = Scopes.ScopeArray.ToArray()
        };
    }
}