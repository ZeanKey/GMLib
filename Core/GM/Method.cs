namespace GMLib.GM;

public static class Method
{
    public static object? GetSelf(this GMLib.Method method)
        => method.Context;

    public static GMLib.Asset.Script.Function GetIndex(this GMLib.Method method)
        => method.Function;
    
    public static object? Call(this GMLib.Method method, params object?[]? arrayArgs)
        => method.Invoke(arrayArgs);
}