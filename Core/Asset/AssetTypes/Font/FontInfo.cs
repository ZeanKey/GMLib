namespace GMLib;

public struct FontInfo
{
    public int Size;
    public int SDFSpread;
    public string Name;
    public Texture2D Texture;
    public IReadOnlyDictionary<int, IFontGlyph> Glyphs;
}