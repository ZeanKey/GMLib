using System.Collections;
using System.Reflection;

namespace GMLib;

public static partial class GML
{
    public static AssetManager AssetManager = new();
    
    public static Handle<Asset> AssetGetIndex(string name)
    {
        if (AssetManager.TryFindAssetHandle(name, out var handle))
        {
            return handle;
        }
        return default;
    }

    public static Type AssetGetType(string name)
    {
        if (AssetManager.TryFindAssetHandle(name, out var handle))
        {
            return handle.Prefix.ToType();
        }
        return typeof(Asset.Unknown);
    }

    public static Handle<Asset>[] AssetGetIds(Asset.Type type)
    {
        return AssetManager[(byte)type].Handles;
    }
}

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class AssetClassOf : Attribute
{
    public Asset.Type Type { get; init; }
    public AssetClassOf(Asset.Type type)
    {
        Type = type;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract partial class Asset : IHandleable<Asset>
{
    public enum Type : byte
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

    [AssetClassOf(Type.Unknown)]
    public class Unknown
    {
        
    }

    public static Asset? ResolveHandle(Handle<Asset> handle)
    {
        throw new NotImplementedException();
    }
}

public abstract partial class Asset
{
    internal readonly static Dictionary<byte, System.Type> AssetPrefixToTypeMap = [];
    internal readonly static Dictionary<System.Type, byte> TypeToAssetPrefixMap = [];
    
    static Asset()
    {
        var members = typeof(Asset).GetNestedTypes()
            .Where(p => Attribute.IsDefined(p, typeof(AssetClassOf)));
        foreach (var member in members)
        {
            var value = (byte)(member.GetCustomAttribute(typeof(AssetClassOf))! as AssetClassOf)!.Type;
            AssetPrefixToTypeMap.Add
            (
                value, member
            );
            TypeToAssetPrefixMap.Add
            (
                member, value
            );
        }
    }
}

public abstract partial class Asset
{
    public readonly string Name;
    
    public Asset(string name)
    {
        Name = name;
    }
}

public static class AssetExtension
{
    internal static Type ToType(this byte type) => Asset.AssetPrefixToTypeMap[type];
    internal static byte ToAssetPreifx(this Type type) => Asset.TypeToAssetPrefixMap[type];
}

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
/// <typeparam name="T"></typeparam>
public class AssetManager<T> : IAssetManager where T : Asset, IHandleable<T>
{
    protected byte assetIndexCounter;
    protected readonly Dictionary<Handle<T>, T> AssetHandleToResourceMap = [];
    protected readonly Dictionary<string, Handle<T>> AssetNameToHandleMap = [];

    public int Counter => assetIndexCounter;
    public static readonly byte Prefix = typeof(T).ToAssetPreifx();

    internal static AssetManager<T> DefaultManager = new();

    static AssetManager()
    {
        AssetManager.PrefixToManagerMap[Prefix] = DefaultManager;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual Handle<T> NextHandle()
    {
        return new Handle<T>(
            (ushort)((Prefix << 8) + assetIndexCounter ++)
        );
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    public virtual Handle<T>? Register(T asset)
    {
        if (AssetHandleToResourceMap.ContainsValue(asset))
            return null;

        Handle<T> handle = NextHandle();
        AssetHandleToResourceMap.Add(handle, asset);
        AssetNameToHandleMap.Add(asset.Name, handle);

        return handle;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    public virtual bool Override(T asset)
    {
        if (!AssetNameToHandleMap.TryGetValue(asset.Name, out Handle<T> handle))
            return false;

        AssetHandleToResourceMap[handle] = asset;
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public virtual bool Remove(Handle<T> handle)
    {
        return AssetHandleToResourceMap.Remove(handle);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <returns></returns>
    public virtual bool Exists(Handle<T> handle)
    {
        return AssetHandleToResourceMap.ContainsKey(handle);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public virtual bool TryFindAsset(Handle<T> handle, out T? asset)
    {
        return AssetHandleToResourceMap.TryGetValue(handle, out asset);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="asset"></param>
    /// <returns></returns>
    public virtual bool TryFindAsset(string name, out T? asset)
    {
        asset = null;
        return TryFindAssetHandle(name, out Handle<T> handle) && AssetHandleToResourceMap.TryGetValue(handle, out asset);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="handle"></param>
    /// <returns></returns>
    public virtual bool TryFindAssetHandle(string name, out Handle<T> handle)
    {
        return AssetNameToHandleMap.TryGetValue(name, out handle);
    }

#region IAssetManager
    public Handle<Asset>[] Handles => AssetHandleToResourceMap.Keys
        .Select<Handle<T>, Handle<Asset>>((handle) => new(handle)).ToArray();

    public bool Remove(Handle<Asset> handle)
    {
        if (handle.Prefix != Prefix) return false;
        return Remove(new Handle<T>(handle));
    }

    public bool Exists(Handle<Asset> handle)
    {
        if (handle.Prefix != Prefix) return false;
        return Exists(new Handle<T>(handle));
    }

    public bool TryFindAsset(Handle<Asset> handle, out Asset? asset)
    {
        if (handle.Prefix != Prefix)
        {
            asset = null;
            return false;
        }
        var result = TryFindAsset(new Handle<T>(handle), out var assetT);
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
        if (TryFindAssetHandle(name, out Handle<T> handleT))
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
public class AssetManager : IAssetManager
{
    public Handle<Asset>[] Handles { 
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

    internal static Dictionary<byte, IAssetManager> PrefixToManagerMap = [];
    
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
