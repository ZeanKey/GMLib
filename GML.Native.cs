namespace GMLib;

public static partial class GMLNative
{
    public static void show_debug_message(params object[] args)
    {
        foreach (var arg in args)
        {
            Console.WriteLine(arg ?? "undefined");
        }
    }
}