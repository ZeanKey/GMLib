namespace GMLib;

public class HandleManager<T> where T : class?, IHandleable<T>
{
    protected byte HandleIndexCounter;
    protected readonly Dictionary<Handle<T>, T> Contents = [];

    public int Counter => HandleIndexCounter;
    
    public static readonly HandleManager<T> Default = new();
    
    protected Handle<T> Next() => new(HandleIndexCounter ++);
    
    public Handle<T> Create(T handleable)
    {
        var handle = Next();
        Contents.Add(handle, handleable);
        return handle;
    }
    
    public void SetTarget(Handle<T> handle, T handleable)
    {
        Contents[handle] = handleable;
    }
    
    public bool Remove(Handle<T> handle)
    {
        return Contents.Remove(handle);
    }
    
    public bool Contains(Handle<T> handle)
    {
        return Contents.ContainsKey(handle);
    }
    
    public bool ContainsTarget(T handleable)
    {
        return Contents.ContainsValue(handleable);
    }
    
    public bool TryGetTarget(Handle<T> handle, out WeakReference<T>? target)
    {
        var ret = Contents.TryGetValue(handle, out var resource);
        target = ret ? new WeakReference<T>(resource!) : null;
        return ret;
    }
}