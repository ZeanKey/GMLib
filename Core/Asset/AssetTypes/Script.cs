using System.Dynamic;

namespace GMLib;

public partial class GML
{
    public static AssetManager<Asset.Script> Script => Asset.GetManager<Asset.Script>();
    
    public static bool ScriptExists(Handle<Asset.Script> handle) => Script.Contains(handle);
    
    public static string ScriptGetName(Handle<Asset.Script> handle)
    {
        return Script.TryFindAsset(handle, out var scriptRef) ? (scriptRef!.Target as Asset.Script)?.Name ?? " " : " ";
    }
}

internal static class ScriptManagerExtensions
{
    internal static string NextName(this AssetManager<Asset.Script> manager)
    {
        return $"__anonymous_{manager.Counter}";
    }
}

partial class Asset
{
    public sealed class Script : Asset, IHandleable<Script>
    {
        public delegate object? Function(object? sender, params object?[]? arguments);
        public readonly string Name;
        public readonly Function Body;
        public readonly bool IsConstructor;

        public Script(Function body, string name)
        {
            Name = name;
            Body = body;
            IsConstructor = false;
        }
        
        public Script(Function body)
        {
            Name = GML.Script.NextName();
            Body = body;
            IsConstructor = false;
        }
        
        public Script(Action<object?, object?[]?> body, string name)
        {
            Name = name;
            Body = (sender, arguments) => { body(sender, arguments); return null;};
            IsConstructor = false;
        }
        
        public Script(Action<object?, object?[]?> body)
        {
            Name = GML.Script.NextName();
            Body = (sender, arguments) => { body(sender, arguments); return null;};
            IsConstructor = false;
        }

        public static WeakReference? ResolveHandle(Handle<Script> handle)
        {
            GML.Script.TryFindAsset(handle, out var asset);
            return asset;
        }

        static Script()
        {
            AssetManager.Managers[typeof(Script)] = new AssetManager<Script>();
        }
    }
}
