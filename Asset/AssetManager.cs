namespace GMLib;

public sealed class AssetManager<TAsset> : HandleManager<TAsset>, IAssetManager where TAsset : Asset, IHandleable<TAsset>
{
    private readonly Dictionary<string, Handle<TAsset>> Assets = [];
    
    public new static AssetManager<TAsset> Default { get; } = new();

    static AssetManager()
    {
        AssetManager.Managers[typeof(TAsset)] = Default;
    }
    
    public Handle<TAsset> Register(string name, TAsset asset)
    {
        var handle = Create(asset);
        Assets[name] = handle;
        return handle;
    }
    
    public bool TryFindAsset(Handle<TAsset> handle, out WeakReference? assetRef)
    {
        var ret = Contents.TryGetValue(handle, out var asset);
        assetRef = ret ? new WeakReference(asset!) : null;
        return ret;
    }
    
    public bool TryFindAsset(string name, out WeakReference? assetRef)
    {
        assetRef = null;
        if (!TryFindAssetHandle(name, out Handle<TAsset> handle) ||
            !Contents.TryGetValue(handle, out var asset)) return false;
        assetRef = new WeakReference(asset);
        return true;
    }
    
    public bool TryFindAssetHandle(string name, out Handle<TAsset> handle)
    {
        return Assets.TryGetValue(name, out handle);
    }

    public IHandle[] Handles => Contents.Keys.Cast<IHandle>().ToArray();

    public bool TryFindAsset(IHandle handle, out WeakReference? assetRef)
    {
        var ret = Contents.TryGetValue(new Handle<TAsset>(handle), out var asset);
        assetRef = ret ? new WeakReference(asset!) : null;
        return ret;
    }

    public bool TryFindAssetHandle(string name, out IHandle handle)
    {
        if (TryFindAssetHandle(name, out Handle<TAsset> handleT))
        {
            handle = handleT;
            return true;
        }
        handle = default(Handle<Asset.Unknown>);
        return false;
    }
}

public sealed class AssetManager : IAssetManager
{
    public IHandle[] Handles
    { 
        get
        {
            List<IHandle> result = [];
            foreach (var manager in Managers.Values)
            {
                result.AddRange(manager.Handles);
            }
            return result.ToArray();
        }
    }

    internal static readonly Dictionary<Type, IAssetManager> Managers = [];
    internal IAssetManager this[Type type] => Managers[type];

    internal void RegisterManager(Type type, IAssetManager manager)
    {
        Managers[type] = manager;
    }

    public bool Contains<TAsset>(IHandle handle) where TAsset : Asset, IHandleable<TAsset>
    {
        return ((AssetManager<TAsset>)this[typeof(TAsset)]).Contains(new Handle<TAsset>(handle));
    }
    
    public bool TryFindAsset<TAsset>(IHandle handle, out WeakReference? assetRef) where TAsset : Asset, IHandleable<TAsset>
    {
        return ((AssetManager<TAsset>)this[typeof(TAsset)]).TryFindAsset(handle, out assetRef);
    }
    
    public bool TryFindAsset<TAsset>(string name, out WeakReference? assetRef) where TAsset : Asset, IHandleable<TAsset>
    { 
        return ((AssetManager<TAsset>)this[typeof(TAsset)]).TryFindAsset(name, out assetRef);
    }
    
    public bool TryFindAssetHandle<TAsset>(string name, out Handle<TAsset> handle) where TAsset : Asset, IHandleable<TAsset>
    {
        return ((AssetManager<TAsset>)this[typeof(TAsset)]).TryFindAssetHandle(name, out handle);
    }
    
    public bool TryFindAsset(IHandle handle, out WeakReference? assetRef)
    {
        throw new NotImplementedException();
    }

    public bool TryFindAsset(string name, out WeakReference? assetRef)
    {
        throw new NotImplementedException();
    }

    public bool TryFindAssetHandle(string name, out IHandle? handle)
    {
        throw new NotImplementedException();
    }
}
