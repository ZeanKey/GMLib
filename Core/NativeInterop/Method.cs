using System.Dynamic;

namespace GMLib;

partial class GML
{
    public static Method Method(object structRefOrInstanceId, Asset.Script func)
    {
        return new Method(structRefOrInstanceId, func);
    }
    
    public static Method Method(object structRefOrInstanceId, Method func)
    {
        return new Method(structRefOrInstanceId, func);
    }

    public static object? MethodGetSelf(Method method)
    {
        return method.Self;
    }

    public static Asset.Script.Function MethodGetIndex(Method method)
    {
        return method.Function;
    }

    public static object? MethodCall(Method method, params object?[]? arrayArgs)
    {
        return method.Invoke(arrayArgs);
    }
}

/// <summary>
/// 
/// </summary>
public sealed class Method : DynamicObject
{
    public object? Self;
    public Asset.Script.Function Function;

    public Method(object? self, Asset.Script.Function function)
    {
        Self = self;
        Function = function;
    }
    
    public Method(object? self, Asset.Script function)
    {
        Self = self;
        Function = function.Body;
    }
    
    public Method(object? self, Method method)
    {
        Self = self;
        Function = method.Function;
    }
    
    public object? Invoke(params object?[]? args)
    {
        return Function(Self, args);
    }
    
    public override bool TryInvoke(InvokeBinder binder, object?[]? args, out object? result)
    {
        result = Function(Self, args);
        return true;
    }
}