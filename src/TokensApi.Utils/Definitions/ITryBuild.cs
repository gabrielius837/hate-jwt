namespace TokensApi.Utils;

public interface ITryBuild<T> where T : class
{
    /// <summary>
    /// Tries to build <see cref="T"/> where it's a class
    /// </summary>
    /// <returns>Valid <see cref="T"/> or null</returns>
    T? TryBuild();
}