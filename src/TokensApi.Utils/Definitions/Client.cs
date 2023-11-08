namespace TokensApi.Utils;

public static class Client
{
    /// <summary>
    /// Collection of valid clients
    /// </summary>
    public static readonly IEnumerable<string> ClientArray = new[]
    {
        UnspecifiedClient,
        GatewayClient
    };

    private static readonly ISet<string> ClientSet = new HashSet<string>(ClientArray);

    /// <summary>
    /// Method for checking that a string is a valid client.
    /// </summary>
    /// <param name="arg">The client in question.</param>
    /// <returns>A flag indicating whether the given 'arg' is a valid client.</returns>
    public static bool ClientExists(string role) => ClientSet.Contains(role);

    
    /// <summary>
    /// Unspecified (default) client
    /// </summary>
    public const string UnspecifiedClient = "00000000-0000-0000-0000-000000000000";
    /// <summary>
    /// Identifier for gateway client
    /// </summary>
    public const string GatewayClient = "f77eca20-5020-4164-a93c-12565dc71b30";
}
