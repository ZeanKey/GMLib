using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;
using static GMLib.RaylibExtension;

namespace GMLib;

public static partial class GML
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    public static void draw_set_font(FontAsset font)
    {
        GraphicManager.Font = font;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="halign"></param>
    public static void draw_set_halign(FontHAlignment halign)
    {
        GraphicManager.HAlignment = halign;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="valign"></param>
    public static void draw_set_valign(FontVAlignment valign)
    {
        GraphicManager.VAlignment = valign;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static FontAsset draw_get_font()
    {
        return GraphicManager.Font;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static FontHAlignment draw_get_halign()
    {
        return GraphicManager.HAlignment;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static FontVAlignment draw_get_valign()
    {
        return GraphicManager.VAlignment;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="text"></param>
    public static void draw_text(float x, float y, string text)
    {
        DrawText(new Vector2(x, y), text);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="text"></param>
    /// <param name="sep"></param>
    /// <param name="w"></param>
    public static void draw_text_ext(float x, float y, string text, float sep, float w)
    {
        DrawTextExt(new Vector2(x, y), text, sep, w);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="text"></param>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <param name="c3"></param>
    /// <param name="c4"></param>
    /// <param name="alpha"></param>
    public static void draw_text_colour(float x, float y, string text, Color3B c1, Color3B c2, Color3B c3, Color3B c4, float alpha)
    {
        byte alphaub = (byte)((int)alpha * 255);
        DrawTextColour
        (
            new Vector2(x, y), text,
            new Color(c1.R, c1.G, c1.B, alphaub),
            new Color(c2.R, c2.G, c2.B, alphaub),
            new Color(c3.R, c3.G, c3.B, alphaub),
            new Color(c4.R, c4.G, c4.B, alphaub)
        );
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="text"></param>
    /// <param name="xscale"></param>
    /// <param name="yscale"></param>
    /// <param name="angle"></param>
    public static void draw_text_transformed(float x, float y, string text, float xscale, float yscale, float angle)
    {
        DrawTextTransformed(new Vector2(x, y), text, new Vector2(xscale, yscale), angle);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="text"></param>
    /// <param name="sep"></param>
    /// <param name="w"></param>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <param name="c3"></param>
    /// <param name="c4"></param>
    /// <param name="alpha"></param>
    public static void draw_text_ext_colour(float x, float y, string text, float sep, float w, Color3B c1, Color3B c2, Color3B c3, Color3B c4, float alpha)
    {
        byte alphaub = (byte)((int)alpha * 255);
        DrawTextExtColour
        (
            new Vector2(x, y), text, sep, w,
            new Color(c1.R, c1.G, c1.B, alphaub),
            new Color(c2.R, c2.G, c2.B, alphaub),
            new Color(c3.R, c3.G, c3.B, alphaub),
            new Color(c4.R, c4.G, c4.B, alphaub)
        );
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="text"></param>
    /// <param name="sep"></param>
    /// <param name="w"></param>
    /// <param name="xscale"></param>
    /// <param name="yscale"></param>
    /// <param name="angle"></param>
    public static void draw_text_ext_transformed(float x, float y, string text, float sep, float w, float xscale, float yscale, float angle)
    {
        DrawTextExtTransformed(new Vector2(x, y), text, sep, w, new Vector2(xscale, yscale), angle);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="text"></param>
    /// <param name="sep"></param>
    /// <param name="w"></param>
    /// <param name="xscale"></param>
    /// <param name="yscale"></param>
    /// <param name="angle"></param>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <param name="c3"></param>
    /// <param name="c4"></param>
    /// <param name="alpha"></param>
    public static void draw_text_ext_transformed_colour(float x, float y, string text, float sep, float w, float xscale, float yscale, float angle, Color3B c1, Color3B c2, Color3B c3, Color3B c4, float alpha)
    {
        byte alphaub = (byte)((int)alpha * 255);
        DrawTextExtTransformedColour
        (
            new Vector2(x, y), text, sep, w, new Vector2(xscale, yscale), angle, 
            new Color(c1.R, c1.G, c1.B, alphaub),
            new Color(c2.R, c2.G, c2.B, alphaub),
            new Color(c3.R, c3.G, c3.B, alphaub),
            new Color(c4.R, c4.G, c4.B, alphaub)
        );
    }
}