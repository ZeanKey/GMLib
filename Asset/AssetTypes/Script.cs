using System.Dynamic;

namespace GMLib;

public partial class GML
{
    public static AssetManager<Asset.Script> Script => AssetManager<Asset.Script>.Default;
    
    public static bool ScriptExists(Handle<Asset.Script> handle) => Script.Contains(handle);
    
    public static string ScriptGetName(Handle<Asset.Script> handle)
    {
        return Script.TryFindAsset(handle, out var scriptRef) ? (scriptRef!.Target as Asset.Script)?.Name ?? " " : " ";
    }
}

internal static class AssetScriptManagerExtension
{
    internal static string NextName(this AssetManager<Asset.Script> manager)
    {
        return $"__anonymous_{manager.Counter}";
    }
}

public partial class Asset
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
    }
}

/// <summary>
/// 
/// </summary>
public sealed class Method : DynamicObject
{
    public string? Name;
    public object? Context;
    public Asset.Script.Function Function;

    public Method(object? context, Asset.Script.Function function)
    {
        Context = context;
        Function = function;
    }
    
    public Method(object? context, Asset.Script function)
    {
        Context = context;
        Function = function.Body;
    }
    
    public Method(object? context, Method method)
    {
        Context = context;
        Function = method.Function;
    }
    
    public object? Invoke(params object?[]? args)
    {
        return Function(Context, args);
    }
    
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        result = Function(Context, args);
        return true;
    }

}