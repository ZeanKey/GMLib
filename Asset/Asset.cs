namespace GMLib;

public static partial class GML
{
    public static readonly AssetManager Asset = new();
    
    public static Handle<Asset> AssetGetIndex(string name)
    {
        return Asset.TryFindAssetHandle(name, out var handle) ? handle : default;
    }

    public static Type AssetGetType(string name)
    {
        return Asset.TryFindAssetHandle(name, out var handle) ? handle.Prefix.ToType() : typeof(Asset.Unknown);
    }

    public static Handle<Asset>[] AssetGetIds(AssetType assetType)
    {
        return Asset[(byte)assetType].Handles;
    }
}

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class RegisterAssetType(AssetType assetType) : Attribute
{
    internal AssetType AssetType { get; init; } = assetType;
}

/// <summary>
/// 
/// </summary>
public abstract partial class Asset : IHandleable<Asset>
{
    /// <summary>
    /// 
    /// </summary>
    [RegisterAssetType(AssetType.Unknown)]
    public class Unknown;

    public static Asset? ResolveHandle(Handle<Asset> handle)
    {
        GML.Asset.TryFindAsset(handle, out var result);
        return result;
    }
}

public abstract partial class Asset(string name)
{
    public readonly string Name = name;
}

