using System;

using Xunit;

namespace TokensApi.Tests;

public class AuthenticationTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(12L)]
    public void Authentication_MustReturnNull_WhenTokenLifetimeIsInvalid(long? tokenLifetime)
    {
        var settings = GetValidAuthenticationSettings();
        settings.TokenLifetime = tokenLifetime;

        var config = settings.TryBuild();

        Assert.Null(config);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("-----BEGIN PRIVATE KEY-----MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC85J4d4HFf882V2bTLsHY25V5GcKTrQc9nn+QYk55K8OcdTAnJ0LZWVuLU/etnLOkiUtOvhqfHN2hLK7tCHYTqPObuQkVQt/j3u0YmylJDX0KIizfeB9dXdWiBHV/Ta3X2LiuqzwTI9iU2hnWH5w7QXkoDaYOYSIvsjIxa2VbrQBttHUs/ypI0HgkgR2UKdMJpRv018eGziPyUPxX7pTDMbcNdDgJZCUI18kd9M6RQc3cdC3t60zy9hecY8AgXLXuDRDBSL8If4850CJlM/R4dZ005qGn5P/SW2qgyYHRiuIJ2in0nhfnqb3P8iDyl2v4lH1IkM1sF/3kT2Q2we1PNAgMBAAECggEAANX6JSOzHLuVc05NUIFtZHzLWABzml7mLg01Ey7ECozaWPTXLj9wLx2fT1X4TrKSLYa0TRiJcYY7PenpoDdFOUwfsXo1mE/YGDCJ9O38QzQOtU/4ZVEfkNGCLuAUZnkZQoedxdm4qbL9IyqhJoLNO/rO1QkIfgoEJH3SuSwxCEV0ZMpPIMC3s4+1hJa9PPIjrOE1Q/MaOEZuX9b2eZZTfp4NwW2qLTdeaos/QlGKlhbjt/4PvNjzpj8zsevR76f4qSIeqxgqh8KrIvZypCwKtydssUg72CLRtNM0+XyvIi0RXmrWK3CFjY/npfulsUCKyLj5zA4UVbR43fcf3t4RuQKBgQDzlzjQzNFRJtRlarVyNZ2W0HjCLZfPVTnD0UbxEYw1SRz6jmM5r75V4cqMPkUSEGHrY5EcTK9gIyRl6LbrZ3Og05Zrp9kDEmo60xMcO1logfgyDCmJIDKotPrSkQiP1ejlknLdBAj3FO2Ff+DKVZdm/SOoiVsopcynOT+Wy/33HwKBgQDGhA7sRTThd701+/a8YN2vHHCHsYRLahfq1vmhx9h4CCDJQ7P1yXkmqvZ4iiSA68lUNwYV/dzNa7nU8Rexc8ey3qillGp/jxCejCO92Io1bywm6ZaZ5pclWuOK3awW/aBYfYLNzF4fAh/Ccq7Lj52ZkiBHl2wn+RYRBr6vZWzzkwKBgQDXI2Zgs6TjQaIxeE+M8WGfw7wD8/Syf5knI3y+iTsGDO71NDDa0CcQ/vy/ZHfofmOdXDDbh7cU97S79Q8pVhM/peCicHowRmFgVu/37UsIDFJDEY7Vt4RGkKE+vC2Nq+WobPHZ1ih84RTu5YSJ75JquYR24+mJOvZHcz6AVuaVDQKBgQCHf8oj0VU39KAMwg/3IvMNvI6+wBc51o++tr+rgpopy/p9Bh5GzR9JmnYjbr9d1BMWzXv09NOKz1YHmAyBDjMnzz0zz+slaESCw8r9oVktAdYUNWqbX76ZO8GGnqoBA1s4K2tmB3HTHoMJGXhf74Y2NXydASlM24MJWh474DIOywKBgApGr5OW2G1dv1Kv/AL8CTY0AnBIJKIetdftFCmj8UptmTfjXO/VEjlCHBXVjs79A/zq9xeBk7Q3bBr3ZbqTrW9NO3YXu/HqP4nBSv8bPTw6EZuOUMJCM2bYZzxjMP8dsXfpBttYgkipAPRxHBVTdAefU2LuQ7q4DOKRz-----END PRIVATE KEY-----")]
    public void Authentication_MustReturnNull_WhenPrivateKeyIsInvalid(string? privateKey)
    {
        var settings = GetValidAuthenticationSettings();
        settings.PrivateKey = privateKey;

        var config = settings.TryBuild();

        Assert.Null(config);
    }

    [Fact]
    public void Authentication_MustReturnNull_WhenProfileArrayIsNull()
    {
        var settings = GetValidAuthenticationSettings();
        settings.ProfileSettings = null;

        var config = settings.TryBuild();

        Assert.Null(config);
    }

    [Fact]
    public void Authentication_MustReturnNull_WhenProfileArrayIsEmpty()
    {
        var settings = GetValidAuthenticationSettings();
        settings.ProfileSettings = Array.Empty<ProfileSetting>(); 

        var config = settings.TryBuild();

        Assert.Null(config);
    }

    [Fact]
    public void Authentication_MustReturnNull_WhenProfileArrayConstainsDuplicate()
    {
        var settings = GetValidAuthenticationSettings();
        settings.ProfileSettings = new ProfileSetting[]
        {
            ProfileTests.GetValidProfileSetting(),
            ProfileTests.GetValidProfileSetting()
        };

        var config = settings.TryBuild();

        Assert.Null(config);
    }

    [Fact]
    public void Authentication_MustNotReturnNull_WhenSettingsAreValid()
    {
        var settings = GetValidAuthenticationSettings();

        var config = settings.TryBuild();

        Assert.NotNull(config);
    }

    public static AuthenticationSettings GetValidAuthenticationSettings()
    {
        return new AuthenticationSettings()
        {
            TokenLifetime = 180,
            PrivateKey = "-----BEGIN PRIVATE KEY-----MIIEvgIBADANBgkqhkiG9w0BAQEFAASCBKgwggSkAgEAAoIBAQC85J4d4HFf882V2bTLsHY25V5GcKTrQc9nn+QYk55K8OcdTAnJ0LZWVuLU/etnLOkiUtOvhqfHN2hLK7tCHYTqPObuQkVQt/j3u0YmylJDX0KIizfeB9dXdWiBHV/Ta3X2LiuqzwTI9iU2hnWH5w7QXkoDaYOYSIvsjIxa2VbrQBttHUs/ypI0HgkgR2UKdMJpRv018eGziPyUPxX7pTDMbcNdDgJZCUI18kd9M6RQc3cdC3t60zy9hecY8AgXLXuDRDBSL8If4850CJlM/R4dZ005qGn5P/SW2qgyYHRiuIJ2in0nhfnqb3P8iDyl2v4lH1IkM1sF/3kT2Q2we1PNAgMBAAECggEAANX6JSOzHLuVc05NUIFtZHzLWABzml7mLg01Ey7ECozaWPTXLj9wLx2fT1X4TrKSLYa0TRiJcYY7PenpoDdFOUwfsXo1mE/YGDCJ9O38QzQOtU/4ZVEfkNGCLuAUZnkZQoedxdm4qbL9IyqhJoLNO/rO1QkIfgoEJH3SuSwxCEV0ZMpPIMC3s4+1hJa9PPIjrOE1Q/MaOEZuX9b2eZZTfp4NwW2qLTdeaos/QlGKlhbjt/4PvNjzpj8zsevR76f4qSIeqxgqh8KrIvZypCwKtydssUg72CLRtNM0+XyvIi0RXmrWK3CFjY/npfulsUCKyLj5zA4UVbR43fcf3t4RuQKBgQDzlzjQzNFRJtRlarVyNZ2W0HjCLZfPVTnD0UbxEYw1SRz6jmM5r75V4cqMPkUSEGHrY5EcTK9gIyRl6LbrZ3Og05Zrp9kDEmo60xMcO1logfgyDCmJIDKotPrSkQiP1ejlknLdBAj3FO2Ff+DKVZdm/SOoiVsopcynOT+Wy/33HwKBgQDGhA7sRTThd701+/a8YN2vHHCHsYRLahfq1vmhx9h4CCDJQ7P1yXkmqvZ4iiSA68lUNwYV/dzNa7nU8Rexc8ey3qillGp/jxCejCO92Io1bywm6ZaZ5pclWuOK3awW/aBYfYLNzF4fAh/Ccq7Lj52ZkiBHl2wn+RYRBr6vZWzzkwKBgQDXI2Zgs6TjQaIxeE+M8WGfw7wD8/Syf5knI3y+iTsGDO71NDDa0CcQ/vy/ZHfofmOdXDDbh7cU97S79Q8pVhM/peCicHowRmFgVu/37UsIDFJDEY7Vt4RGkKE+vC2Nq+WobPHZ1ih84RTu5YSJ75JquYR24+mJOvZHcz6AVuaVDQKBgQCHf8oj0VU39KAMwg/3IvMNvI6+wBc51o++tr+rgpopy/p9Bh5GzR9JmnYjbr9d1BMWzXv09NOKz1YHmAyBDjMnzz0zz+slaESCw8r9oVktAdYUNWqbX76ZO8GGnqoBA1s4K2tmB3HTHoMJGXhf74Y2NXydASlM24MJWh474DIOywKBgApGr5OW2G1dv1Kv/AL8CTY0AnBIJKIetdftFCmj8UptmTfjXO/VEjlCHBXVjs79A/zq9xeBk7Q3bBr3ZbqTrW9NO3YXu/HqP4nBSv8bPTw6EZuOUMJCM2bYZzxjMP8dsXfpBttYgkipAPRxHBVTdAefU2LuQ7q4DOKRz/wqU6HK-----END PRIVATE KEY-----",
            ProfileSettings = new ProfileSetting[] { ProfileTests.GetValidProfileSetting() }
        };
    }
}