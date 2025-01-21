namespace GMLib;

public unsafe class NativeFontGlyph(Font font, GlyphInfo glyphInfo, int glyphIndex) : IFontGlyph
{
    public Rectangle Rec { get; } = font.Recs[glyphIndex];
    public int X => (int)Rec.X;
    public int Y => (int)Rec.Y;
    public int Width => (int)Rec.Width;
    public int Height => (int)Rec.Height;
    public int Character => glyphInfo.Value;
    public int Shift => glyphInfo.AdvanceX;
    public Vector2 Offset => new(glyphInfo.OffsetX, glyphInfo.OffsetY);
}