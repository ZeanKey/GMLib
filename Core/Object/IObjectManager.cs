namespace GMLib;

public interface IObjectManager
{
    internal Dictionary<Handle<IObject>, IObject> MergedCollection { get; }
    
    internal List<IObjectManager> SubManagers { get; }

    public bool TryFindObject(Handle<IObject>? id, out WeakReference? inst);

    public void EventPerform(ObjectEventType type, params object?[] eventArgs);

    public Handle<IObject>? Add(IObject gMObject);

    public bool Remove(Handle<IObject>? id);

    public bool Contains(Handle<IObject>? id);
}