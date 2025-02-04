namespace GMLib;

/// <summary>
///
/// </summary>
public interface IFontGlyph
{
    public Rectangle Rec { get; }
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public int Character { get; }
    public int Shift { get; }
    public Vector2 Offset { get; }
}