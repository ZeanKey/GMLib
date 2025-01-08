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
    public static void DrawSetFont(Asset.Font font)
    {
        GraphicManager.Font = font;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="halign"></param>
    public static void DrawSetHalign(FontHAlignment halign)
    {
        GraphicManager.HAlignment = halign;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="valign"></param>
    public static void DrawSetValign(FontVAlignment valign)
    {
        GraphicManager.VAlignment = valign;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static Asset.Font DrawGetFont()
    {
        return GraphicManager.Font!;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static FontHAlignment DrawGetHalign()
    {
        return GraphicManager.HAlignment;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static FontVAlignment DrawGetValign()
    {
        return GraphicManager.VAlignment;
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
        DrawTextExtTransformedColour(position, text, sep, w, scale, angle, GraphicManager.Color, GraphicManager.Color, GraphicManager.Color, GraphicManager.Color);
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
        var font = GraphicManager.Font.Font;
        if (sep < 0)
            sep = 0;
        else
            sep -= font.BaseSize * scale.Y;
        DrawTextProExt(font, text, position, Vector2.Zero, angle, scale, (Vector2)GraphicManager.Alignment, sep, 0, c1, c2, c3, c4);
    }
}

/// <summary>
/// 
/// </summary>
public static class FontFileType
{
    public const string TrueType = ".ttf";
    public const string OpenType = ".otf";
}

/// <summary>
/// 
/// </summary>
public class ExpandoFont
{
    private readonly byte[] Data;
    public Font Font;
    public string RangeSample;
    public readonly int Size;
    public readonly string Type;

    public unsafe ExpandoFont(string filename, string type, int size)
    {
        int fileSize = 0;
        byte* fileData = LoadFileData(filename, ref fileSize);
        Data = new ReadOnlySpan<byte>(fileData, fileSize).ToArray();
        Type = type;
        Size = size;
        RangeSample = "";
        UnloadFileData(fileData);
        int codepointCount = 0;
        int[] codepoints = LoadCodepoints(RangeSample, ref codepointCount);
        Font = LoadFontFromMemory(Type, Data, Size, codepoints, codepointCount);
    }
    public unsafe ExpandoFont(string filename, string type, int size, string sample)
    {
        int fileSize = 0;
        byte* fileData = LoadFileData(filename, ref fileSize);
        Data = new ReadOnlySpan<byte>(fileData, fileSize).ToArray();
        Type = type;
        Size = size;
        RangeSample = sample;
        UnloadFileData(fileData);
        int codepointCount = 0;
        int[] codepoints = LoadCodepoints(RangeSample, ref codepointCount);
        Font = LoadFontFromMemory(Type, Data, Size, codepoints, codepointCount);
    }

    public void Reload()
    {
        UnloadFont(Font);
        int codepointCount = 0;
        int[] codepoints = LoadCodepoints(RangeSample, ref codepointCount);
        //read font based on code points
        Font = LoadFontFromMemory(Type, Data, Size, codepoints, codepointCount);
    }
    public bool TryExpand(char sample)
    {
        if (!RangeSample.Contains(sample))
        {
            RangeSample += sample;
            Reload();
            return true;
        }
        return false;
    }
    public bool TryExpand(string sample)
    {
        bool result = false;
        foreach (char sampleChar in sample)
        {
            if (!RangeSample.Contains(sampleChar))
            {
                RangeSample += sampleChar;
                result = true;
            }
        }
        if (result)
        {
            Reload();
        }
        return result;
    }
}

public abstract partial class Asset
{
    /// <summary>
    /// 
    /// </summary>
    [AssetClassOf(Asset.Type.Font)]
    public sealed class Font : ExpandoFont, IHandleable<Font>
    {
        public static readonly Font Default = new("assets/fntSimSun.ttf", FontFileType.TrueType, 10);
        
        public unsafe Font(string filename, string type, int size) : base(filename, type, size)
        {

        }
        public unsafe Font(string filename, string type, int size, string sample) : base(filename, type, size, sample)
        {

        }

        public static Font ResolveHandle(Handle<Font> handle)
        {
            throw new NotImplementedException();
        }
    }
}