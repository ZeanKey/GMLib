using System.Numerics;
using static GMLib.RaylibExtensions;

namespace GMLib;

public static partial class GML
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="font"></param>
    public static void DrawSetFont(Handle<Asset.Font> font)
    {
        Graphics.Font = font;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="halign"></param>
    public static void DrawSetHalign(FontHAlignment halign)
    {
        Graphics.HAlignment = halign;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="valign"></param>
    public static void DrawSetValign(FontVAlignment valign)
    {
        Graphics.VAlignment = valign;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Handle<Asset.Font> DrawGetFont()
    {
        return Graphics.Font!;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static FontHAlignment DrawGetHalign()
    {
        return Graphics.HAlignment;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static FontVAlignment DrawGetValign()
    {
        return Graphics.VAlignment;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    public static void DrawText(Vector2 position, string text)
    {
        DrawTextExt(position, text, -1, -1);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="sep"></param>
    /// <param name="w"></param>
    public static void DrawTextExt(Vector2 position, string text, float sep, float w)
    {
        DrawTextExtTransformed(position, text, sep, w, Vector2.One, 0);
    }
     
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <param name="c3"></param>
    /// <param name="c4"></param>
    public static void DrawTextColour(Vector2 position, string text, Color c1, Color c2, Color c3, Color c4)
    {
        DrawTextExtColour(position, text, -1, -1, c1, c2, c3, c4);
    }
  
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="scale"></param>
    /// <param name="angle"></param>
    public static void DrawTextTransformed(Vector2 position, string text, Vector2 scale, float angle)
    {
        DrawTextExtTransformed(position, text, -1, -1, scale, angle);
    }
   
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="sep"></param>
    /// <param name="w"></param>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <param name="c3"></param>
    /// <param name="c4"></param>
    public static void DrawTextExtColour(Vector2 position, string text, float sep, float w, Color c1, Color c2, Color c3, Color c4)
    {
        DrawTextExtTransformedColour(position, text, sep, w, Vector2.One, 0, c1, c2, c3, c4);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="sep"></param>
    /// <param name="w"></param>
    /// <param name="scale"></param>
    /// <param name="angle"></param>
    public static void DrawTextExtTransformed(Vector2 position, string text, float sep, float w, Vector2 scale, float angle)
    {
        DrawTextExtTransformedColour(position, text, sep, w, scale, angle, Graphics.Color, Graphics.Color, Graphics.Color, Graphics.Color);
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="text"></param>
    /// <param name="sep"></param>
    /// <param name="w"></param>
    /// <param name="scale"></param>
    /// <param name="angle"></param>
    /// <param name="c1"></param>
    /// <param name="c2"></param>
    /// <param name="c3"></param>
    /// <param name="c4"></param>
    public static void DrawTextExtTransformedColour(Vector2 position, string text, float sep, float w, Vector2 scale, float angle, Color c1, Color c2, Color c3, Color c4)
    {
        var font = Graphics.Font;
        if (!Font.TryFindAsset(font, out var fontRef)) 
            throw new Exception("Font not found");
        var fontAsset = fontRef!.Target as Asset.Font ?? throw new Exception("Font not found");
        
        if (sep < 0)
            sep = 0;
        else
            sep -= fontAsset.Size * scale.Y;
        
        DrawTextProGeneral(fontAsset, text, position, Vector2.Zero, angle, scale, (Vector2)Graphics.Alignment, sep, 0, c1, c2, c3, c4);
    }
}