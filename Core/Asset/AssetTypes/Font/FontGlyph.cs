namespace GMLib;

public struct FontGlyph : IFontGlyph
{
    public Rectangle Rec { get; set; }
    
    #region Rec
    public int X => (int)Rec.X;
    public int Y => (int)Rec.Y;
    public int Width => (int)Rec.Width;
    public int Height => (int)Rec.Height;

    #endregion
    
    public int Character { get; set; }
    public int Shift { get; set; }
    public Vector2 Offset { get; set; }
    
}