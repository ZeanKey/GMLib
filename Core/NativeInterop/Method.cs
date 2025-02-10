using System.Dynamic;

namespace GMLib;

partial class GML
{
    public static Method Method(object structRefOrInstanceId, Asset.Script func)
        => new Method(structRefOrInstanceId, func);
    
    public static Method Method(object structRefOrInstanceId, Method func)
        => new Method(structRefOrInstanceId, func);

    public static object? MethodGetSelf(Method method)
        => GM.Method.GetSelf(method);

    public static Asset.Script.Function MethodGetIndex(Method method)
        => GM.Method.GetIndex(method);

    public static object? MethodCall(Method method, params object?[]? arrayArgs)
        => GM.Method.Call(method, arrayArgs);
}

/// <summary>
/// 
/// </summary>
public sealed class Method : DynamicObject
{
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