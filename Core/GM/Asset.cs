namespace GMLib.GM;

public static class Asset
{
    public static IHandle? GetIndex(string name)
    {
        return GML.Asset.TryFindAssetHandle(name, out var handle) ? handle : null;
    }

    public static Type GetType(string name)
    {
        return GML.Asset.TryFindAsset(name, out var asset) ? asset!.GetType() : typeof(GMLib.Asset.Unknown);
    }

    public static Type GetType(this GMLib.Asset asset)
        => asset.GetType();
    
    public static string GetName<TAsset>(this Handle<TAsset> index) where TAsset : GMLib.Asset, IHandleable<TAsset>
        => Asset<TAsset>.GetName(index);

    public static string[] GetTags<TAsset>(this Handle<TAsset> index) where TAsset : GMLib.Asset, IHandleable<TAsset>
        => Asset<TAsset>.GetTags(index);
    
    public static bool AddTags<TAsset>(this Handle<TAsset> index, params string[] tags) where TAsset : GMLib.Asset, IHandleable<TAsset>
        => Asset<TAsset>.AddTags(index, tags);
    
    public static bool RemoveTags<TAsset>(Handle<TAsset> index, params string[] tags) where TAsset : GMLib.Asset, IHandleable<TAsset>
        => Asset<TAsset>.RemoveTags(index, tags);
    
    public static bool HasTags<TAsset>(this Handle<TAsset> index, params string[] tags) where TAsset : GMLib.Asset, IHandleable<TAsset>
        => Asset<TAsset>.HasTags(index, tags);
    
    public static bool HasAnyTag<TAsset>(this Handle<TAsset> index, params string[] tags) where TAsset : GMLib.Asset, IHandleable<TAsset>
        => Asset<TAsset>.HasAnyTag(index, tags);
    
    public static bool ClearTag<TAsset>(this Handle<TAsset> index, params string[] tags) where TAsset : GMLib.Asset, IHandleable<TAsset>
        => Asset<TAsset>.ClearTag(index, tags);
}

public static class Asset<TAsset> where TAsset : GMLib.Asset, IHandleable<TAsset>
{
    public static string GetName(Handle<TAsset> index)
    {
        return GML.Asset.GetManager<TAsset>().Assets.FirstOrDefault(pair => pair.Value == index).Key;
    }
    
    public static Handle<TAsset>[] TagGetAssetIds(params string[] tags)
    {
        return GML.Asset[typeof(TAsset)].Handles
            .Cast<Handle<TAsset>>()
            .Where(handle =>
            {
                return (WeakReference?)handle is { Target: TAsset asset } && 
                       tags.All(tag => asset.Tags.Contains(tag));
            })
            .ToArray();
    }
    
    public static string[] TagGetAssets(params string[] tags)
    {
        return ((AssetManager<TAsset>)GML.Asset[typeof(TAsset)]).Assets
            .Where(pair =>
            {
                if (((WeakReference)pair.Value)!.Target is TAsset asset)
                {
                    return tags.All(tag => asset.Tags.Contains(tag));                    
                }
                return false;
            })
            .Select(pair => pair.Key)
            .ToArray();
    }
    
    public static Handle<TAsset>[] GetIds()
    {
        return GML.Asset[typeof(TAsset)].Handles
            .Cast<Handle<TAsset>>()
            .ToArray();
    }

    public static string[] GetTags(string name)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(name, out var reference))
        {
            return reference!.Target is TAsset asset ? asset.Tags.ToArray() : [];
        }
        return [];
    }
    
    public static string[] GetTags(Handle<TAsset> index)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(index, out var reference))
        {
            return reference!.Target is TAsset asset ? asset.Tags.ToArray() : [];
        }
        return [];
    }
    
    public static bool AddTags(string name, params string[] tags)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(name, out var reference) && reference!.Target is TAsset asset)
        {
            asset.Tags.AddRange(tags);
            return true;
        }
        return false;
    }
    
    public static bool AddTags(Handle<TAsset> index, params string[] tags)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(index, out var reference) && reference!.Target is TAsset asset)
        {
             asset.Tags.AddRange(tags);
             return true;
        }
        return false;
    }
    
    public static bool RemoveTags(string name, params string[] tags)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(name, out var reference) && reference!.Target is TAsset asset)
        {
            asset.Tags.RemoveAll(tags.Contains);
            return true;
        }
        return false;
    }
    
    public static bool RemoveTags(Handle<TAsset> index, params string[] tags)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(index, out var reference) && reference!.Target is TAsset asset)
        {
            asset.Tags.RemoveAll(tags.Contains);
            return true;
        }
        return false;
    }
    
    public static bool HasTags(string name, params string[] tags)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(name, out var reference) && reference!.Target is TAsset asset)
        {
            return asset.Tags.All(tags.Contains);
        }
        return false;
    }
    
    public static bool HasTags(Handle<TAsset> index, params string[] tags)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(index, out var reference) && reference!.Target is TAsset asset)
        {
            return asset.Tags.All(tags.Contains);
        }
        return false;
    }
    
    public static bool HasAnyTag(string name, params string[] tags)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(name, out var reference) && reference!.Target is TAsset asset)
        {
            return asset.Tags.Count > 0;
        }
        return false;
    }
    
    public static bool HasAnyTag(Handle<TAsset> index, params string[] tags)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(index, out var reference) && reference!.Target is TAsset asset)
        {
            return asset.Tags.Count > 0;
        }
        return false;
    }
    
    public static bool ClearTag(string name, params string[] tags)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(name, out var reference) && reference!.Target is TAsset asset)
        {
             asset.Tags.Clear();
             return true;
        }
        return false;
    }
    
    public static bool ClearTag(Handle<TAsset> index, params string[] tags)
    {
        if (GML.Asset[typeof(TAsset)].TryFindAsset(index, out var reference) && reference!.Target is TAsset asset)
        {
            asset.Tags.Clear();
            return true;
        }
        return false;
    }
}