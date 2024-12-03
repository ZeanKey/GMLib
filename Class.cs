global using Raylib_cs;

namespace GMLib;

public static partial class GML
{
    public static void show_debug_message(params object[] args)
    {
        foreach (var arg in args)
        {
            Console.WriteLine(arg is null ? "undefined" : arg);
        }
    }
}