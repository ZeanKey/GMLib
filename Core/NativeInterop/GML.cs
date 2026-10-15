namespace GMLib.NativeInterop;

public partial class GML;

partial class GML
{
    public bool variable_instance_exists(Handle<IObject> instance_id, string name)
    {
        
    }

    public string[] variable_instance_get_names(Handle<IObject> instance_id)
    {
        
    }

    public float variable_instance_get_names_count(Handle<IObject> instance_id)
    {
        
    }

    public object? variable_instance_get(Handle<IObject> instance_id, string name)
    {
        
    }

    public void variable_instance_set(Handle<IObject> instance_id, string name, object val)
    {
        
    }

    public bool variable_global_exists(string name)
    {
        
    }

    public object? variable_global_get(string name)
    {
        
    }

    public void variable_global_set(string name, object val)
    {
        
    }

    public string nameof(object name)
    {
        
    }

    public Method method(object struct_ref_or_instance_id, object func)
    {
        
    }
}

partial class GML
{
    public static void show_debug_message(params object[] args)
    {
        foreach (var arg in args)
        {
            Console.WriteLine(arg ?? "undefined");
        }
    }
}

partial class GML
{
    public static void draw_set_font(Handle<Asset.Font> font)
    {
        GMLib.GML.Graphics.Font = font;
    }
    
    public static void draw_set_halign(FontHAlignment halign)
    {
        GMLib.GML.Graphics.HAlignment = halign;
    }
    
    public static void draw_set_valign(FontVAlignment valign)
    {
        GMLib.GML.Graphics.VAlignment = valign;
    }
    
    public static Handle<Asset.Font> draw_get_font()
    {
        return GMLib.GML.Graphics.Font;
    }
    
    public static FontHAlignment draw_get_halign()
    {
        return GMLib.GML.Graphics.HAlignment;
    }
    
    public static FontVAlignment draw_get_valign()
    {
        return GMLib.GML.Graphics.VAlignment;
    }
    
    public static void draw_text(float x, float y, string text)
    {
        GMLib.GML.DrawText(new Vector2(x, y), text);
    }

    public static void draw_text_ext(float x, float y, string text, float sep, float w)
    {
        GMLib.GML. DrawTextExt(new Vector2(x, y), text, sep, w);
    }
    
    public static void draw_text_colour(float x, float y, string text, Color3B c1, Color3B c2, Color3B c3, Color3B c4, float alpha)
    {
        var alphaUByte = (byte)(alpha * 255);
        GMLib.GML.DrawTextColour
        (
            new Vector2(x, y), text,
            new Color(c1.R, c1.G, c1.B, alphaUByte),
            new Color(c2.R, c2.G, c2.B, alphaUByte),
            new Color(c3.R, c3.G, c3.B, alphaUByte),
            new Color(c4.R, c4.G, c4.B, alphaUByte)
        );
    }
    
    public static void draw_text_transformed(float x, float y, string text, float xscale, float yscale, float angle)
    {
        GMLib.GML.DrawTextTransformed(new Vector2(x, y), text, new Vector2(xscale, yscale), angle);
    }
    
    public static void draw_text_ext_colour(float x, float y, string text, float sep, float w, Color3B c1, Color3B c2, Color3B c3, Color3B c4, float alpha)
    {
        var alphaUByte = (byte)(alpha * 255);
        GMLib.GML.DrawTextExtColour
        (
            new Vector2(x, y), text, sep, w,
            new Color(c1.R, c1.G, c1.B, alphaUByte),
            new Color(c2.R, c2.G, c2.B, alphaUByte),
            new Color(c3.R, c3.G, c3.B, alphaUByte),
            new Color(c4.R, c4.G, c4.B, alphaUByte)
        );
    }
    
    public static void draw_text_ext_transformed(float x, float y, string text, float sep, float w, float xscale, float yscale, float angle)
    {
        GMLib.GML.DrawTextExtTransformed(new Vector2(x, y), text, sep, w, new Vector2(xscale, yscale), angle);
    }
    
    public static void draw_text_ext_transformed_colour(float x, float y, string text, float sep, float w, float xscale, float yscale, float angle, Color3B c1, Color3B c2, Color3B c3, Color3B c4, float alpha)
    {
        var alphaUByte = (byte)(alpha * 255);
        GMLib.GML.DrawTextExtTransformedColour
        (
            new Vector2(x, y), text, sep, w, new Vector2(xscale, yscale), angle, 
            new Color(c1.R, c1.G, c1.B, alphaUByte),
            new Color(c2.R, c2.G, c2.B, alphaUByte),
            new Color(c3.R, c3.G, c3.B, alphaUByte),
            new Color(c4.R, c4.G, c4.B, alphaUByte)
        );
    }
}