using static Raylib_cs.Raylib;

namespace GMLib;

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
        var fileSize = 0;
        var fileData = LoadFileData(filename, ref fileSize);
        Data = new ReadOnlySpan<byte>(fileData, fileSize).ToArray();
        Type = type;
        Size = size;
        RangeSample = "";
        UnloadFileData(fileData);
        var codepointCount = 0;
        var codepoints = LoadCodepoints(RangeSample, ref codepointCount);
        Font = LoadFontFromMemory(Type, Data, Size, codepoints, codepointCount);
    }
    
    public unsafe ExpandoFont(string filename, string type, int size, string sample)
    {
        var fileSize = 0;
        var fileData = LoadFileData(filename, ref fileSize);
        Data = new ReadOnlySpan<byte>(fileData, fileSize).ToArray();
        Type = type;
        Size = size;
        RangeSample = sample;
        UnloadFileData(fileData);
        var codepointCount = 0;
        var codepoints = LoadCodepoints(RangeSample, ref codepointCount);
        Font = LoadFontFromMemory(Type, Data, Size, codepoints, codepointCount);
    }

    public void Reload()
    {
        UnloadFont(Font);
        var codepointCount = 0;
        var codepoints = LoadCodepoints(RangeSample, ref codepointCount);
        //read font based on code points
        Font = LoadFontFromMemory(Type, Data, Size, codepoints, codepointCount);
    }
    
    public bool TryExpand(char sample)
    {
        if (RangeSample.Contains(sample)) return false;
        RangeSample += sample;
        Reload();
        return true;
    }
    
    public bool TryExpand(string sample)
    {
        var result = false;
        foreach (var sampleChar in sample.Where(sampleChar => !RangeSample.Contains(sampleChar)))
        {
            RangeSample += sampleChar;
            result = true;
        }
        if (result)
            Reload();
        return result;
    }
}

public abstract partial class Asset
{
    /// <summary>
    /// 
    /// </summary>
    [AssetClassOf(AssetType.Font)]
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