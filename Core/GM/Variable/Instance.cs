namespace GMLib.GM.Variable;

public static class Instance
{
    public static bool Exists(this Handle<IObject> instanceId, string name)
    {
        if (GML.Object.TryFindObject(instanceId, out var reference))
        {
            if (reference!.Target is GMLib.Instance instance)
            {
                return instance.Self?.ContainsKey(name) ?? false;
            }
        }
        
        return false;
    }
    
    public static string[] GetNames(this Handle<IObject> instanceId)
    {
        if (GML.Object.TryFindObject(instanceId, out var reference))
        {
            if (reference!.Target is GMLib.Instance instance)
            {
                return instance.Self?.Keys.ToArray() ?? [];
            }
        }
        return [];
    }

    public static int GetCount(this Handle<IObject> instanceId)
    {
        if (GML.Object.TryFindObject(instanceId, out var reference))
        {
            if (reference!.Target is GMLib.Instance instance)
            {
                return instance.Self?.Keys.Count ?? -1;
            }
        }
        return -1;
    }

    public static object? Get(this Handle<IObject> instanceId, string name)
    {
        if (GML.Object.TryFindObject(instanceId, out var reference))
        {
            if (reference!.Target is GMLib.Instance instance)
            {
                return instance.Self?[name];
            }
        }
        return null;
    }

    public static void Set(this Handle<IObject> instanceId, string name, object? val)
    {
        if (!GML.Object.TryFindObject(instanceId, out var reference))
            return;
        if (reference!.Target is not GMLib.Instance instance) 
            return;
        if (instance.Self is { } self)
            self[name] = val;
    }
}