using System;
using System.Collections.Generic;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TokensApi.Utils;

using Xunit;

namespace TokensApi.Tests;

public class ExtensionsTests
{
    [Fact]
    public void AddAuthConfig_MustNotThrowException_WhenSettingsAreValid()
    {
        var services = new ServiceCollection();
        var validAuthSetting = GetValidAuthSetting();
        var settings = new [] { validAuthSetting };
        var memCollection = GetInMemoryCollection(TokensApiExtensions.AuthSettings, settings);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(memCollection)
            .Build();

        services.AddAuthConfig(config);

        // assert
        // test is passing
        // due to no exception
    }

    [Fact]
    public void AddAuthConfig_MustThrowException_WhenSettingsAreMissing()
    {
        var services = new ServiceCollection();
        var memCollection = Array.Empty<KeyValuePair<string, string>>();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(memCollection)
            .Build();

        Assert.Throws<InvalidOperationException>(() => services.AddAuthConfig(config));
    }

    [Theory]
    [InlineData(null)]
    [InlineData(12L)]
    public void AddAuthConfig_MustThrowException_WhenTokenLifetimeIsInvalid(long? tokenLifetime)
    {
        var services = new ServiceCollection();
        var authSetting = GetValidAuthSetting();
        authSetting.TokenLifetime = tokenLifetime;
        var settings = new [] { authSetting };
        var memCollection = GetInMemoryCollection(TokensApiExtensions.AuthSettings, settings);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(memCollection)
            .Build();

        Assert.Throws<InvalidOperationException>(() => services.AddAuthConfig(config));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  \n ")]
    [InlineData("-----BEGIN PRIVATE KEY-----MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC85J4d4HFf882V2bTLsHY25V5GcKTrQc9nn+QYk55K8OcdTAnJ0LZWVuLU/etnLOkiUtOvhqfHN2hLK7tCHYTqPObuQkVQt/j3u0YmylJDX0KIizfeB9dXdWiBHV/Ta3X2LiuqzwTI9iU2hnWH5w7QXkoDaYOYSIvsjIxa2VbrQBttHUs/ypI0HgkgR2UKdMJpRv018eGziPyUPxX7pTDMbcNdDgJZCUI18kd9M6RQc3cdC3t60zy9hecY8AgXLXuDRDBSL8If4850CJlM/R4dZ005qGn5P/SW2qgyYHRiuIJ2in0nhfnqb3P8iDyl2v4lH1IkM1sF/3kT2Q2we1PNAgMBAAECggEAANX6JSOzHLuVc05NUIFtZHzLWABzml7mLg01Ey7ECozaWPTXLj9wLx2fT1X4TrKSLYa0TRiJcYY7PenpoDdFOUwfsXo1mE/YGDCJ9O38QzQOtU/4ZVEfkNGCLuAUZnkZQoedxdm4qbL9IyqhJoLNO/rO1QkIfgoEJH3SuSwxCEV0ZMpPIMC3s4+1hJa9PPIjrOE1Q/MaOEZuX9b2eZZTfp4NwW2qLTdeaos/QlGKlhbjt/4PvNjzpj8zsevR76f4qSIeqxgqh8KrIvZypCwKtydssUg72CLRtNM0+XyvIi0RXmrWK3CFjY/npfulsUCKyLj5zA4UVbR43fcf3t4RuQKBgQDzlzjQzNFRJtRlarVyNZ2W0HjCLZfPVTnD0UbxEYw1SRz6jmM5r75V4cqMPkUSEGHrY5EcTK9gIyRl6LbrZ3Og05Zrp9kDEmo60xMcO1logfgyDCmJIDKotPrSkQiP1ejlknLdBAj3FO2Ff+DKVZdm/SOoiVsopcynOT+Wy/33HwKBgQDGhA7sRTThd701+/a8YN2vHHCHsYRLahfq1vmhx9h4CCDJQ7P1yXkmqvZ4iiSA68lUNwYV/dzNa7nU8Rexc8ey3qillGp/jxCejCO92Io1bywm6ZaZ5pclWuOK3awW/aBYfYLNzF4fAh/Ccq7Lj52ZkiBHl2wn+RYRBr6vZWzzkwKBgQDXI2Zgs6TjQaIxeE+M8WGfw7wD8/Syf5knI3y+iTsGDO71NDDa0CcQ/vy/ZHfofmOdXDDbh7cU97S79Q8pVhM/peCicHowRmFgVu/37UsIDFJDEY7Vt4RGkKE+vC2Nq+WobPHZ1ih84RTu5YSJ75JquYR24+mJOvZHcz6AVuaVDQKBgQCHf8oj0VU39KAMwg/3IvMNvI6+wBc51o++tr+rgpopy/p9Bh5GzR9JmnYjbr9d1BMWzXv09NOKz1YHmAyBDjMnzz0zz+slaESCw8r9oVktAdYUNWqbX76ZO8GGnqoBA1s4K2tmB3HTHoMJGXhf74Y2NXydASlM24MJWh474DIOywKBgApGr5OW2G1dv1Kv/AL8CTY0AnBIJKIetdftFCmj8UptmTfjXO/VEjlCHBXVjs79A/zq9xeBk7Q3bBr3ZbqTrW9NO3YXu/HqP4nBSv8bPTw6EZuOUMJCM2bYZzxjMP8dsXfpBttYgkipAPRxHBVTdAefU2LuQ7q4DOKRz-----END PRIVATE KEY-----")]
    public void AddAuthConfig_MustThrowException_WhenPrivateKeyIsInvalid(string? privateKey)
    {
        var services = new ServiceCollection();
        var authSetting = GetValidAuthSetting();
        authSetting.PrivateKey = privateKey;
        var settings = new [] { authSetting };
        var memCollection = GetInMemoryCollection(TokensApiExtensions.AuthSettings, settings);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(memCollection)
            .Build();

        Assert.Throws<InvalidOperationException>(() => services.AddAuthConfig(config));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  \n ")]
    public void AddAuthConfig_MustThrowException_WhenClientIdIsInvalid(string? clientId)
    {
        var services = new ServiceCollection();
        var authSetting = GetValidAuthSetting();
        authSetting.ClientId = clientId;
        var settings = new [] { authSetting };
        var memCollection = GetInMemoryCollection(TokensApiExtensions.AuthSettings, settings);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(memCollection)
            .Build();

        Assert.Throws<InvalidOperationException>(() => services.AddAuthConfig(config));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  \n ")]
    public void AddAuthConfig_MustThrowException_WhenClienSecretIsInvalid(string? clientId)
    {
        var services = new ServiceCollection();
        var authSetting = GetValidAuthSetting();
        authSetting.ClientId = clientId;
        var settings = new [] { authSetting };
        var memCollection = GetInMemoryCollection(TokensApiExtensions.AuthSettings, settings);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(memCollection)
            .Build();

        Assert.Throws<InvalidOperationException>(() => services.AddAuthConfig(config));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  \n ")]
    public void AddAuthConfig_MustThrowException_WhenScopesAreInvalid(string? scope)
    {
        var services = new ServiceCollection();
        var authSetting = GetValidAuthSetting();
        authSetting.Scopes = new string?[] { scope };
        var settings = new [] { authSetting };
        var memCollection = GetInMemoryCollection(TokensApiExtensions.AuthSettings, settings);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(memCollection)
            .Build();

        Assert.Throws<InvalidOperationException>(() => services.AddAuthConfig(config));
    }

    [Fact]
    public void AddAuthConfig_MustThrowException_WhenScopesAreMissing()
    {
        var services = new ServiceCollection();
        var authSetting = GetValidAuthSetting();
        authSetting.Scopes = new string[] { };
        var settings = new [] { authSetting };
        var memCollection = GetInMemoryCollection(TokensApiExtensions.AuthSettings, settings);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(memCollection)
            .Build();

        Assert.Throws<InvalidOperationException>(() => services.AddAuthConfig(config));
    }

    [Fact]
    public void AddAuthConfig_MustThrowException_WhenDuplicateSettingIsFound()
    {
        var services = new ServiceCollection();
        var settings = new [] { GetValidAuthSetting(), GetValidAuthSetting() };
        var memCollection = GetInMemoryCollection(TokensApiExtensions.AuthSettings, settings);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(memCollection)
            .Build();

        Assert.Throws<InvalidOperationException>(() => services.AddAuthConfig(config));
    }

    private static IEnumerable<KeyValuePair<string,string>> GetInMemoryCollection(string key, AuthSetting[] settings)
    {
        var list = new List<KeyValuePair<string,string>>();

        for (var i = 0; i < settings.Length; i++)
        {
            var setting = settings[i];
            if (setting.TokenLifetime is not null)
            {
                list.Add(new KeyValuePair<string, string>($"{key}:{i}:TokenLifetime", setting.TokenLifetime.Value.ToString()));
            }
            if (setting.PrivateKey is not null)
            {
                list.Add(new KeyValuePair<string, string>($"{key}:{i}:PrivateKey", setting.PrivateKey));
            }
            if (setting.ClientId is not null)
            {
                list.Add(new KeyValuePair<string, string>($"{key}:{i}:ClientId", setting.ClientId));
            }
            if (setting.ClientSecret is not null)
            {
                list.Add(new KeyValuePair<string, string>($"{key}:{i}:ClientSecret", setting.ClientSecret));
            }
            if (setting.Scopes is null || setting.Scopes.Length == 0)
                continue;
            
            var scopes = setting.Scopes;

            for (var k = 0; k < scopes.Length; k++)
            {
                var scope = scopes[k];
                if (scope is not null)
                {
                    list.Add(new KeyValuePair<string, string>($"{key}:{i}:Scopes:{k}", scope));
                }
            }
        }

        return list;
    }

    public static AuthSetting GetValidAuthSetting()
    {
        return new AuthSetting()
        {
            TokenLifetime = 180,
            PrivateKey = "-----BEGIN PRIVATE KEY-----MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC85J4d4HFf882V2bTLsHY25V5GcKTrQc9nn+QYk55K8OcdTAnJ0LZWVuLU/etnLOkiUtOvhqfHN2hLK7tCHYTqPObuQkVQt/j3u0YmylJDX0KIizfeB9dXdWiBHV/Ta3X2LiuqzwTI9iU2hnWH5w7QXkoDaYOYSIvsjIxa2VbrQBttHUs/ypI0HgkgR2UKdMJpRv018eGziPyUPxX7pTDMbcNdDgJZCUI18kd9M6RQc3cdC3t60zy9hecY8AgXLXuDRDBSL8If4850CJlM/R4dZ005qGn5P/SW2qgyYHRiuIJ2in0nhfnqb3P8iDyl2v4lH1IkM1sF/3kT2Q2we1PNAgMBAAECggEAANX6JSOzHLuVc05NUIFtZHzLWABzml7mLg01Ey7ECozaWPTXLj9wLx2fT1X4TrKSLYa0TRiJcYY7PenpoDdFOUwfsXo1mE/YGDCJ9O38QzQOtU/4ZVEfkNGCLuAUZnkZQoedxdm4qbL9IyqhJoLNO/rO1QkIfgoEJH3SuSwxCEV0ZMpPIMC3s4+1hJa9PPIjrOE1Q/MaOEZuX9b2eZZTfp4NwW2qLTdeaos/QlGKlhbjt/4PvNjzpj8zsevR76f4qSIeqxgqh8KrIvZypCwKtydssUg72CLRtNM0+XyvIi0RXmrWK3CFjY/npfulsUCKyLj5zA4UVbR43fcf3t4RuQKBgQDzlzjQzNFRJtRlarVyNZ2W0HjCLZfPVTnD0UbxEYw1SRz6jmM5r75V4cqMPkUSEGHrY5EcTK9gIyRl6LbrZ3Og05Zrp9kDEmo60xMcO1logfgyDCmJIDKotPrSkQiP1ejlknLdBAj3FO2Ff+DKVZdm/SOoiVsopcynOT+Wy/33HwKBgQDGhA7sRTThd701+/a8YN2vHHCHsYRLahfq1vmhx9h4CCDJQ7P1yXkmqvZ4iiSA68lUNwYV/dzNa7nU8Rexc8ey3qillGp/jxCejCO92Io1bywm6ZaZ5pclWuOK3awW/aBYfYLNzF4fAh/Ccq7Lj52ZkiBHl2wn+RYRBr6vZWzzkwKBgQDXI2Zgs6TjQaIxeE+M8WGfw7wD8/Syf5knI3y+iTsGDO71NDDa0CcQ/vy/ZHfofmOdXDDbh7cU97S79Q8pVhM/peCicHowRmFgVu/37UsIDFJDEY7Vt4RGkKE+vC2Nq+WobPHZ1ih84RTu5YSJ75JquYR24+mJOvZHcz6AVuaVDQKBgQCHf8oj0VU39KAMwg/3IvMNvI6+wBc51o++tr+rgpopy/p9Bh5GzR9JmnYjbr9d1BMWzXv09NOKz1YHmAyBDjMnzz0zz+slaESCw8r9oVktAdYUNWqbX76ZO8GGnqoBA1s4K2tmB3HTHoMJGXhf74Y2NXydASlM24MJWh474DIOywKBgApGr5OW2G1dv1Kv/AL8CTY0AnBIJKIetdftFCmj8UptmTfjXO/VEjlCHBXVjs79A/zq9xeBk7Q3bBr3ZbqTrW9NO3YXu/HqP4nBSv8bPTw6EZuOUMJCM2bYZzxjMP8dsXfpBttYgkipAPRxHBVTdAefU2LuQ7q4DOKRz/wqU6HK-----END PRIVATE KEY-----",
            ClientId = "backend",
            ClientSecret = "secret",
            Scopes = new string[] { Scope.BackendScope }
        };
    }
}