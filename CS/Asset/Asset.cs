using System.Collections;
using System.Reflection;

namespace GMLib;

public static partial class GML
{
    public static AssetManager Asset = new();
    
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
internal class AssetClassOf(AssetType assetType) : Attribute
{
    internal AssetType AssetType { get; init; } = assetType;
}

public enum AssetType : byte
{
    Unknown,
    Object,
    Sprite, 
    Sound,
    Room,
    Tiles,
    Path,
    Script,
    Font,
    Timeline,
    Shader,
    AnimationCurve,
    Sequence,
    ParticleSystem
}

/// <summary>
/// 
/// </summary>
public abstract partial class Asset : IHandleable<Asset>
{
    /// <summary>
    /// 
    /// </summary>
    [AssetClassOf(AssetType.Unknown)]
    public class Unknown
    {
        
    }

    public static Asset? ResolveHandle(Handle<Asset> handle)
    {
        GML.Asset.TryFindAsset(handle, out var result);
        return result;
    }
}

public abstract partial class Asset
{
    internal static readonly Dictionary<byte, System.Type> AssetPrefixToTypeMap = [];
    internal static readonly Dictionary<System.Type, byte> TypeToAssetPrefixMap = [];
    
    static Asset()
    {
        var members = typeof(Asset).GetNestedTypes()
            .Where(p => Attribute.IsDefined(p, typeof(AssetClassOf)));
        foreach (var member in members)
        {
            var value = (byte)(member.GetCustomAttribute(typeof(AssetClassOf))! as AssetClassOf)!.AssetType;
            AssetPrefixToTypeMap
                .Add(value, member);
            TypeToAssetPrefixMap
                .Add(member, value);
        }
    }
}

public abstract partial class Asset(string name)
{
    public readonly string Name = name;
}

internal static class AssetExtension
{
    internal static Type ToType(this byte type) => Asset.AssetPrefixToTypeMap[type];
    internal static byte ToAssetPrefix(this Type type) => Asset.TypeToAssetPrefixMap[type];
}

#region Managed Asset * Abandoned
/*
/// <summary>
/// 
/// </summary>
public abstract class ManagedAsset : Asset, IHandleable<ManagedAsset>
{
    internal ManagedHandle<ManagedAsset> Handle;
    protected Action LoadHandler;
    
    internal ManagedAsset(string name, params object?[] args) : base(name)
    {
        Handle = new(new()) { IsValid = false };
        LoadHandler = () => Load(args);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    protected abstract void Load(params object?[] args);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    internal virtual void Load()
    {
       LoadHandler();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    internal virtual void Release(params object?[] args)
    {
        Handle.IsValid = false;
    }

    public static ManagedAsset? ResolveHandle(Handle<ManagedAsset> handle)
    {
        throw new NotImplementedException();
    }
}
*/
#endregion

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

/// <summary>
/// 
/// </summary>
/// <typeparam name="TAsset"></typeparam>
public sealed class AssetManager<TAsset> : IAssetManager where TAsset : Asset, IHandleable<TAsset>
{
    private byte AssetIndexCounter;
    private readonly Dictionary<Handle<TAsset>, TAsset> AssetHandleToResourceMap = []; // Get Asset object from Asset Handle
    private readonly Dictionary<string, Handle<TAsset>> AssetNameToHandleMap = []; // Get Asset Handle from Asset name

    public int Counter => AssetIndexCounter;
    public static readonly byte Prefix = typeof(TAsset).ToAssetPrefix();

    public static AssetManager<TAsset> Default { get; internal set; } = new();

    static AssetManager()
    {
        AssetManager.PrefixToManagerMap[Prefix] = Default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private Handle<TAsset> NextHandle() => 
        new Handle<TAsset>((ushort)((Prefix << 8) + AssetIndexCounter ++));
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    public Handle<TAsset>? Register(TAsset asset)
    {
        if (AssetHandleToResourceMap.ContainsValue(asset))
            return null;

        var handle = NextHandle();
        AssetHandleToResourceMap
            .Add(handle, asset);
        AssetNameToHandleMap
            .Add(asset.Name, handle);

        return handle;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    public bool Override(TAsset asset)
    {
        if (!AssetNameToHandleMap.TryGetValue(asset.Name, out var handle))
            return false;

        AssetHandleToResourceMap[handle] = asset;
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public bool Remove(Handle<TAsset> handle)
    {
        return AssetHandleToResourceMap.Remove(handle);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public bool Exists(Handle<TAsset> handle)
    {
        return AssetHandleToResourceMap.ContainsKey(handle);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public bool TryFindAsset(Handle<TAsset> handle, out TAsset? asset)
    {
        return AssetHandleToResourceMap.TryGetValue(handle, out asset);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public bool TryFindAsset(string name, out TAsset? asset)
    {
        asset = null;
        return TryFindAssetHandle(name, out Handle<TAsset> handle) && AssetHandleToResourceMap.TryGetValue(handle, out asset);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="handle"></param>
    /// <returns></returns>
    public bool TryFindAssetHandle(string name, out Handle<TAsset> handle)
    {
        return AssetNameToHandleMap.TryGetValue(name, out handle);
    }

#region IAssetManager
    public Handle<Asset>[] Handles => AssetHandleToResourceMap.Keys
        .Select<Handle<TAsset>, Handle<Asset>>((handle) => new(handle)).ToArray();

    public bool Remove(Handle<Asset> handle)
    {
        if (handle.Prefix != Prefix) return false;
        return Remove(new Handle<TAsset>(handle));
    }

    public bool Exists(Handle<Asset> handle)
    {
        if (handle.Prefix != Prefix) return false;
        return Exists(new Handle<TAsset>(handle));
    }

    public bool TryFindAsset(Handle<Asset> handle, out Asset? asset)
    {
        if (handle.Prefix != Prefix)
        {
            asset = null;
            return false;
        }
        var result = TryFindAsset(new Handle<TAsset>(handle), out var assetT);
        asset = assetT;
        return result;
    }

    public bool TryFindAsset(string name, out Asset? asset)
    {
        if (TryFindAssetHandle(name, out Handle<Asset> handle))
        {
            return TryFindAsset(handle, out asset);
        }
        asset = null;
        return false;
    }

    public bool TryFindAssetHandle(string name, out Handle<Asset> handle)
    {
        if (TryFindAssetHandle(name, out Handle<TAsset> handleT))
        {
            handle = new(handleT);
            return true;
        }
        handle = default;
        return false;
    }
#endregion
}

/// <summary>
/// 
/// </summary>
public sealed class AssetManager : IAssetManager
{
    public Handle<Asset>[] Handles
    { 
        get
        {
            List<Handle<Asset>> result = [];
            foreach (var manager in PrefixToManagerMap.Values)
            {
                result.AddRange(manager.Handles);
            }
            return result.ToArray();
        }
    }

    internal static readonly Dictionary<byte, IAssetManager> PrefixToManagerMap = [];
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    internal IAssetManager this[byte prefix] => PrefixToManagerMap[prefix];

    /// <summary>
    /// 
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    [Obsolete]
    public Handle<Asset>? Register(Asset asset)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public bool Remove(Handle<Asset> handle)
    {
        return this[handle.Prefix].Remove(handle);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public bool Exists(Handle<Asset> handle)
    {
        return this[handle.Prefix].Exists(handle);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public bool TryFindAsset(Handle<Asset> handle, out Asset? asset)
    {
        return this[handle.Prefix].TryFindAsset(handle, out asset);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public bool TryFindAsset(string name, out Asset? asset)
    {
        foreach (var manager in PrefixToManagerMap.Values)
        {
            if (manager.TryFindAssetHandle(name, out var handle))
            {
                return manager.TryFindAsset(handle, out asset);
            }
        }
        asset = null;
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="handle"></param>
    /// <returns></returns>
    public bool TryFindAssetHandle(string name, out Handle<Asset> handle)
    {
        foreach (var manager in PrefixToManagerMap.Values)
        {
            if (manager.TryFindAssetHandle(name, out handle))
            {
                return true;
            }
        }
        handle = default;
        return false;
    }
}
