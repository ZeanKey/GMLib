namespace GMLib;

public static partial class GML
{
    public static readonly AssetManager Asset = new();
    
    public static IHandle AssetGetIndex(string name)
    {
        return Asset.TryFindAssetHandle(name, out var handle) ? handle : default;
    }

    public static Type AssetGetType(string name)
    {
        return Asset.TryFindAsset(name, out var asset) ? asset!.GetType() : typeof(Asset.Unknown);
    }
    
    public static Type AssetGetType(Asset asset) => asset.GetType();
    
    public static Handle<TAsset>[] AssetGetIds<TAsset>() where TAsset : Asset, IHandleable<TAsset>
    {
        return Asset[typeof(TAsset)].Handles.Cast<Handle<TAsset>>().ToArray();
    }
}

public abstract partial class Asset
{
    public class Unknown : IHandleable<Unknown>
    {
        public static WeakReference ResolveHandle(Handle<Unknown> handle)
        {
            throw new NotImplementedException();
        }
    }
}
