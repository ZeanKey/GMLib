namespace GMLib;

public abstract class Asset<T> where T : IHandleable<T>
{
    public readonly string Name;

    public Asset(string name)
    {
        Name = name;
    }
}

public class AssetManager<T> where T : Asset<T>, IHandleable<T> 
{
    protected ushort _currentAssetIndex;
    private readonly Dictionary<Handle<T>, T> AssetHandleToResourceMap;
    private readonly Dictionary<string, Handle<T>> AssetNameToHandleMap;
    
    public AssetManager()
    {
        _currentAssetIndex = 0;
        AssetHandleToResourceMap = [];
        AssetNameToHandleMap = [];
    }
    
    private Handle<T> NextHandle()
    {
        return new Handle<T>(_currentAssetIndex ++);
    }
        
    public Handle<T>? Register(T asset)
    {
        if (AssetHandleToResourceMap.ContainsValue(asset))
        {
            return null;
        }

        Handle<T> handle = NextHandle();
        AssetHandleToResourceMap.Add(handle, asset);
        AssetNameToHandleMap.Add(asset.Name, handle);

        return handle;
    }
    public bool Override(T asset)
    {
        if (!AssetNameToHandleMap.TryGetValue(asset.Name, out Handle<T> handle)) return false;
        AssetHandleToResourceMap[handle] = asset;
        return true;
    }
    public bool Remove(Handle<T> handle)
    {
        return AssetHandleToResourceMap.Remove(handle);
    }
    public bool Exists(Handle<T> handle)
    {
        return AssetHandleToResourceMap.ContainsKey(handle);
    }
    public bool TryFindAsset(Handle<T> handle, out T? asset)
    {
        return AssetHandleToResourceMap.TryGetValue(handle, out asset);
    }
    public bool TryFindAssetHandle(string name, out Handle<T> handle)
    {
        return AssetNameToHandleMap.TryGetValue(name, out handle);
    }
}
