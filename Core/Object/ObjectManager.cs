namespace GMLib;

public class ObjectManager : HandleManager<IObject>, IObjectManager
{
    internal Dictionary<Handle<IObject>, IObject> MergedCollection
    {
        get
        {
            var collection = Contents
                .ToDictionary(objectPair => objectPair.Key, objectPair => objectPair.Value);
            foreach (var objectPair in SubManagers.SelectMany(subManager => subManager.MergedCollection))
                collection.Add(objectPair.Key, objectPair.Value);
            return collection;
        }
    }
    Dictionary<Handle<IObject>, IObject> IObjectManager.MergedCollection => MergedCollection;
    
    internal List<IObjectManager> SubManagers { get; } = [];
    List<IObjectManager> IObjectManager.SubManagers => SubManagers;
    
    public virtual bool TryFindObject(Handle<IObject>? id, out WeakReference? inst)
    {
        if (id is { } vid)
        {
            if (Contents.TryGetValue(vid, out var value))
            {
                inst = new WeakReference(value);
                return true;
            }
            foreach (var submanager in SubManagers)
                if (submanager.TryFindObject(vid, out inst)) return true;
        }
        inst = null;
        return false;
    }
    
    public virtual void EventPerform(ObjectEventType type, params object?[] eventArgs)
    {
        foreach (var inst in Contents.Values)
        {
            inst.EventPerform(type, eventArgs);
        }
        foreach (var submanager in SubManagers)
            submanager.EventPerform(type, eventArgs);
    }
    
    public virtual Handle<IObject>? Add(IObject gMObject)
    {
        if (Contents.ContainsValue(gMObject))
            return null;
        
        return Create(gMObject);
    }
    
    public virtual bool Remove(Handle<IObject>? id)
    {
        if (id is not { } vid)
            return false;
        
        if (Contents.Remove(vid))
            return true;
        
        foreach (var subManager in SubManagers)
            if (subManager.Remove(id))
                return true;
        
        return false;
    }
    
    public virtual bool Contains(Handle<IObject>? id)
    {
        if (id is not { } vid)
            return false;
        
        if (Contents.ContainsKey(vid))
            return true;
        
        foreach (var subManager in SubManagers)
            if (subManager.Contains(id))
                return true;
        
        return false;
    }
}