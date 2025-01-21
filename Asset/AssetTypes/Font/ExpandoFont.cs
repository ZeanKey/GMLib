namespace GMLib;

public sealed class ExpandoFont : IDisposable, IFont
{
    private int FontIndex;
    private readonly byte[] Data;

    private readonly List<DynamicFont> Fonts = [];
    public int Size { get; }
    
    private bool _disposedValue;
    
    ~ExpandoFont() => Dispose(false);
    
    public ExpandoFont(string path, int size)
    {
        if (Path.GetExtension(path) != FontFileType.TrueType) throw new Exception();
        
        Size = size;
        int byteRead = 0;
        unsafe
        {
            var nativeData = Raylib.LoadFileData(path, ref byteRead);
            Data = new ReadOnlySpan<byte>(nativeData, byteRead).ToArray();
            Raylib.UnloadFileData(nativeData);
        }
        
        var native = Raylib.LoadFontEx(path, size, null, 0); // this font is released inside the ctor of managed font
        Fonts.Add(new DynamicFont(native, size, out _));
    }
    
    public ExpandoFont(string path, int size, string range)
    {
        var type = Path.GetExtension(path);
        if (type != FontFileType.TrueType && type != FontFileType.OpenType) throw new Exception();
        
        Size = size;
        var byteRead = 0;
        unsafe
        {
            var nativeData = Raylib.LoadFileData(path, ref byteRead);
            Data = new ReadOnlySpan<byte>(nativeData, byteRead).ToArray();
            Raylib.UnloadFileData(nativeData);
        }
        var count = 0;
        var codepoints = Raylib.LoadCodepoints(range, ref count);
        
        var native = Raylib.LoadFontEx(path, size, codepoints, count);
        Fonts.Add(new DynamicFont(native, size, out var exceed));
        while (exceed.Length != 0)
        {
            FontIndex++;
            native = Raylib.LoadFontFromMemory(type, Data, Size, exceed, exceed.Length);
            Fonts.Add(new DynamicFont(native, Size, out exceed));
        }
    }
    
    public void AddGlyphsFromString(string range)
    {
        var fontManaged = Fonts[FontIndex];
        if (fontManaged.AddGlyphsFromString(range, Data, out var exceed)) return;
        while (exceed.Length != 0)
        {
            FontIndex++;
            var native = Raylib.LoadFontFromMemory(FontFileType.TrueType, Data, Size, exceed, exceed.Length);
            Fonts.Add(new DynamicFont(native, Size, out exceed));
        }
    }

    public Font GetFont(int sample)
    {
        foreach (var managedFont in Fonts)
        {
            if (managedFont.GlyphIndices.Contains(sample)) return managedFont.Font;
        }
        AddGlyphsFromString(sample.ToString());
        return Fonts[FontIndex].Font;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue) return;

        if (disposing)
        {
            foreach (var managedFont in Fonts) managedFont.Dispose();
        }
        
        _disposedValue = true;
    }

    public int SDFSpread => Fonts[0].SDFSpread;
    public Font Native => Fonts[0].Native;
    public Texture2D Texture => Fonts[0].Texture;
    public IReadOnlyDictionary<int, IFontGlyph> Glyphs => Fonts[0].Glyphs;
}