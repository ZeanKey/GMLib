namespace GMLib;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IHandleable<T> where T : class?, IHandleable<T>
{
    public static abstract WeakReference? ResolveHandle(Handle<T> handle);
}