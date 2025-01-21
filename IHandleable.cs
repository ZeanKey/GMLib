namespace GMLib;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleable<T> where T : IHandleable<T>
{
    public static abstract T? ResolveHandle(Handle<T> handle);
}