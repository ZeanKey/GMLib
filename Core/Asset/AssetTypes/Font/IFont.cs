namespace GMLib;

/// <summary>
/// 
/// </summary>
public interface IFont
{
    public int Size { get; }
    public int SDFSpread { get; }
    public Font Native { get; }
    public Texture2D Texture { get; }
    public IReadOnlyDictionary<int, IFontGlyph> Glyphs { get; }
}
