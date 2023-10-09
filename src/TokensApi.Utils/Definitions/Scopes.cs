namespace TokensApi.Utils;

public static class Scopes
{
    /// <summary>
    /// Collection of valid scopes
    /// </summary>
    public static readonly IEnumerable<string> ScopeArray = new[]
    {
        UnspecifiedScope,
        BackendScope
    };

    private static readonly ISet<string> ScopeSet = new HashSet<string>(ScopeArray);

    /// <summary>
    /// Method for checking that a string is a valid scope.
    /// </summary>
    /// <param name="arg">The scope in question.</param>
    /// <returns>A flag indicating whether the given 'arg' is a valid scope.</returns>
    public static bool ScopeExists(string arg) => ScopeSet.Contains(arg);

    // constants

    /// <summary>
    /// Unspecified (default) scope
    /// </summary>
    public const string UnspecifiedScope = "unspecified";
    /// <summary>
    /// General scope for backend services
    /// </summary>
    public const string BackendScope = "backend";
}