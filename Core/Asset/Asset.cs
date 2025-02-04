namespace GMLib;

partial class GML
{
    public static readonly AssetManager Asset = new();
    
    public static IHandle? AssetGetIndex(string name)
    {
        return Asset.TryFindAssetHandle(name, out var handle) ? handle : null;
    }

    public static Type AssetGetType(string name)
    {
        return Asset.TryFindAsset(name, out var asset) ? asset!.GetType() : typeof(Asset.Unknown);
    }
    
    public static Type AssetGetType(Asset asset) => asset.GetType();

    public static Handle<TAsset>[] TagGetAssetIds<TAsset>(params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        return Asset[typeof(TAsset)].Handles
            .Cast<Handle<TAsset>>()
            .Where(handle =>
            {
                return (WeakReference?)handle is { Target: TAsset asset } && 
                       tags.All(tag => asset.Tags.Contains(tag));
            })
            .ToArray();
    }
    
    public static string[] TagGetAssets<TAsset>(params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        return ((AssetManager<TAsset>)Asset[typeof(TAsset)])!.Assets
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
    
    public static Handle<TAsset>[] AssetGetIds<TAsset>() where TAsset : Asset, IHandleable<TAsset>
    {
        return Asset[typeof(TAsset)].Handles.Cast<Handle<TAsset>>().ToArray();
    }

    public static string[] AssetGetTags<TAsset>(string name) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(name, out var reference))
        {
            return reference!.Target is TAsset asset ? asset.Tags.ToArray() : [];
        }
        return [];
    }
    
    public static string[] AssetGetTags<TAsset>(Handle<TAsset> index) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(index, out var reference))
        {
            return reference!.Target is TAsset asset ? asset.Tags.ToArray() : [];
        }
        return [];
    }
    
    public static bool AssetAddTags<TAsset>(string name, params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(name, out var reference) && reference!.Target is TAsset asset)
        {
            asset.Tags.AddRange(tags);
            return true;
        }
        return false;
    }
    
    public static bool AssetAddTags<TAsset>(Handle<TAsset> index, params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(index, out var reference) && reference!.Target is TAsset asset)
        {
             asset.Tags.AddRange(tags);
             return true;
        }
        return false;
    }
    
    public static bool AssetRemoveTags<TAsset>(string name, params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(name, out var reference) && reference!.Target is TAsset asset)
        {
            asset.Tags.RemoveAll(tags.Contains);
            return true;
        }
        return false;
    }
    
    public static bool AssetRemoveTags<TAsset>(Handle<TAsset> index, params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(index, out var reference) && reference!.Target is TAsset asset)
        {
            asset.Tags.RemoveAll(tags.Contains);
            return true;
        }
        return false;
    }
    
    public static bool AssetHasTags<TAsset>(string name, params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(name, out var reference) && reference!.Target is TAsset asset)
        {
            return asset.Tags.All(tags.Contains);
        }
        return false;
    }
    
    public static bool AssetHasTags<TAsset>(Handle<TAsset> index, params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(index, out var reference) && reference!.Target is TAsset asset)
        {
            return asset.Tags.All(tags.Contains);
        }
        return false;
    }
    
    public static bool AssetHasAnyTag<TAsset>(string name, params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(name, out var reference) && reference!.Target is TAsset asset)
        {
            return asset.Tags.Count > 0;
        }
        return false;
    }
    
    public static bool AssetHasAnyTag<TAsset>(Handle<TAsset> index, params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(index, out var reference) && reference!.Target is TAsset asset)
        {
            return asset.Tags.Count > 0;
        }
        return false;
    }
    
    public static bool AssetClearTag<TAsset>(string name, params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(name, out var reference) && reference!.Target is TAsset asset)
        {
             asset.Tags.Clear();
             return true;
        }
        return false;
    }
    
    public static bool AssetClearTag<TAsset>(Handle<TAsset> index, params string[] tags) where TAsset : Asset, IHandleable<TAsset>
    {
        if (Asset[typeof(TAsset)].TryFindAsset(index, out var reference) && reference!.Target is TAsset asset)
        {
            asset.Tags.Clear();
            return true;
        }
        return false;
    }
}

public abstract partial class Asset
{
    public List<string> Tags { get; } = [];
}

partial class Asset
{
    public class Unknown : IHandleable<Unknown>
    {
        public static WeakReference? ResolveHandle(Handle<Unknown> handle)
        {
            throw new NotImplementedException();
        }
    }
}
