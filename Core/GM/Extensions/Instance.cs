namespace GMLib.GM.Extensions;

public static class Instance
{
    public static bool VariableExists(this Handle<IObject> instanceId, string name)
    {
        if (GML.Instance.TryFindObject(instanceId, out var reference))
        {
            if (reference!.Target is GMLib.Instance instance)
            {
                return instance.Self?.ContainsKey(name) ?? false;
            }
        }
        
        return false;
    }
    
    public static string[] GetVariableNames(this Handle<IObject> instanceId)
    {
        if (GML.Instance.TryFindObject(instanceId, out var reference))
        {
            if (reference!.Target is GMLib.Instance instance)
            {
                return instance.Self?.Keys.ToArray() ?? [];
            }
        }
        return [];
    }

    public static int GetVariableNamesCount(this Handle<IObject> instanceId)
    {
        if (GML.Instance.TryFindObject(instanceId, out var reference))
        {
            if (reference!.Target is GMLib.Instance instance)
            {
                return instance.Self?.Keys.Count ?? -1;
            }
        }
        return -1;
    }

    public static object? GetVariable(this Handle<IObject> instanceId, string name)
    {
        if (GML.Instance.TryFindObject(instanceId, out var reference))
        {
            if (reference!.Target is GMLib.Instance instance)
            {
                return instance.Self?[name];
            }
        }
        return null;
    }

    public static void SetVariable(this Handle<IObject> instanceId, string name, object? val)
    {
        if (!GML.Instance.TryFindObject(instanceId, out var reference))
            return;
        if (reference!.Target is not GMLib.Instance instance) 
            return;
        if (instance.Self is { } self)
            self[name] = val;
    }
}
