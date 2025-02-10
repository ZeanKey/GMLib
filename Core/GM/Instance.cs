namespace GMLib.GM;

public static class Instance
{
    public static bool Exists(this Handle<IObject>? id)
    {
        return GML.Instance.Contains(id);
    }
    
    public static void Destroy(this Handle<IObject>? id)
    {
        if (id is null) return;
        if (id.Exists())
        {
            id.Destroy();
        }
        GML.Instance.Remove(id);
    }
    
    public static Handle<IObject> CreateDepth(Vector2 position, int depth, GMLib.Asset.Object obj, IDictionary<string, object?>? varStruct = null)
    {
        return (Handle<IObject>)GML.Instance.Add(
            new GMLib.Instance(obj)
            {
                Self = varStruct as Dictionary<string, object?> ?? [],
                XStart = position.X,
                YStart = position.Y,
                Position = position,
                Depth = depth
            })!;
    }
}