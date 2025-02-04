namespace GMLib;

/// <summary>
/// 
/// </summary>
public interface IAssetManager
{
    public IHandle[] Handles { get; }
    
    public abstract bool TryFindAsset(IHandle handle, out WeakReference? assetRef);

    public abstract bool TryFindAsset(string name, out WeakReference? assetRef);
    
    public abstract bool TryFindAssetHandle(string name, out IHandle? handle);
}
