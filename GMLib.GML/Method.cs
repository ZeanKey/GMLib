using System.Dynamic;

namespace GMLib;

public static partial class GML
{
    public static readonly ScriptAssetManager FunctionManager = new();
}

public class ScriptAssetManager : AssetManager<Script>
{
    public string NextName()
    {
        return $"__anonymous_{_currentAssetIndex.ToString()}";
    }
}

public class Script : Asset<Script>, IHandleable<Script>
{
    public delegate object? Function(object? sender, params object?[]? arguments);
    public Function Body;
    public bool IsConstructor;

    public Script(Function body, string name) : base(name)
    {
        Body = body;
        IsConstructor = false;
    }
    public Script(Function body) : base(GML.FunctionManager.NextName())
    {
        Body = body;
        IsConstructor = false;
    }
    public Script(Action<object?, object?[]?> body, string name) : base(name)
    {
        Body = new((object? sender, object?[]? arguments) => { body(sender, arguments); return null;});
        IsConstructor = false;
    }
    public Script(Action<object?, object?[]?> body) : base(GML.FunctionManager.NextName())
    {
        Body = new((object? sender, object?[]? arguments) => { body(sender, arguments); return null;});
        IsConstructor = false;
    }

    public static Script? ResolveHandle(Handle<Script> handle)
    {
        GML.FunctionManager.TryFindAsset(handle, out Script? asset);
        return asset;
    }
}

public class Method : DynamicObject
{
    public string? Name;
    public object? Context;
    public Script.Function Function;

    public Method(object? context, Script.Function function)
    {
        Context = context;
        Function = function;
    }
    public Method(object? context, Script function)
    {
        Context = context;
        Function = function.Body;
    }
    public Method(object? context, Method method)
    {
        Context = context;
        Function = method.Function;
    }

    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        result = Function(Context, args);
        return true;
    }

    public object? Invoke(params object?[]? args)
    {
        return Function(Context, args);
    }
}