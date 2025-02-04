namespace GMLib;

/// <summary>
/// This static class works as a global GameMaker rendering state.
/// Editing fields under this class will affect drawing effects after.
/// </summary>
public class GraphicManager
{
    public FontAlignment Alignment { get; set; } = FontAlignment.Default;
    public FontHAlignment HAlignment
    {
        get => Alignment.HAlignment;
        set => Alignment = Alignment with { HAlignment = value };
    }
    public FontVAlignment VAlignment
    {
        get => Alignment.VAlignment;
        set => Alignment = Alignment with { VAlignment = value };
    }
    
    public Handle<Asset.Font> Font { get; set; }
    
    public Color Color { get; set; } = Color.White;
    public Color3B Colour
    {
        get => new(Color);
        set => Color = value;
    } // GameMaker style colour (RGB channels only)
    public float Alpha
    {
        get => GML.Graphics.Color.A;
        set => GML.Graphics.Color = GML.Graphics.Color with { A = (byte)(255 * value) };
    } // GameMaker style alpha (A channel only)
}