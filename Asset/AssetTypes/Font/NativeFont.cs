namespace GMLib;

public unsafe class NativeFont(Font font) : IFont, IDisposable
{
    private bool _disposedValue;
    ~NativeFont() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        Raylib.UnloadFont(font);
        _disposedValue = true;
    }

    public int Size => font.BaseSize;
    public int SDFSpread => font.GlyphPadding;
    public Font Native => font;
    public Texture2D Texture => font.Texture;
    public IReadOnlyDictionary<int, IFontGlyph> Glyphs => new NativeFontGlyphCollection(font, font.Glyphs, font.GlyphCount);
}