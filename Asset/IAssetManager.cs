namespace GMLib;

/// <summary>
/// 
/// </summary>
public interface IAssetManager
{
    /// <summary>
    /// 
    /// </summary>
    public Handle<Asset>[] Handles { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public abstract bool Remove(Handle<Asset> handle);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public abstract bool Exists(Handle<Asset> handle);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public abstract bool TryFindAsset(Handle<Asset> handle, out Asset? asset);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public abstract bool TryFindAsset(string name, out Asset? asset);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="handle"></param>
    /// <returns></returns>
    public abstract bool TryFindAssetHandle(string name, out Handle<Asset> handle);

}
