namespace TokensApi.Utils;

public static class Clients
{
    /// <summary>
    /// Collection of valid clients
    /// </summary>
    public static readonly IEnumerable<string> ClientArray = new[]
    {
        UnspecifiedClient,
        BackendClient
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
    public const string UnspecifiedClient = "unspecified";
    /// <summary>
    /// General client for backend services
    /// </summary>
    public const string BackendClient = "backend";
}
