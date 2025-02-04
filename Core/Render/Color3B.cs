namespace GMLib;

/// <summary>
/// GameMaker Color type, doesn't contain alpha channel.
/// When cast to Raylib color, it will read the environment's default rendering color from RenderingState class.
/// </summary>
public readonly struct Color3B(byte r, byte g, byte b)
{
    public readonly byte R = r; 
    public readonly byte G = g;
    public readonly byte B = b;

    public byte Hue => (byte)(Raylib.ColorToHSV(this).X / 360f * 255f);
    public byte Saturation => (byte)(Raylib.ColorToHSV(this).Y * 255f);
    public byte Value => (byte)(Raylib.ColorToHSV(this).Z * 255f);
    
    public Color3B(Color color) : this(color.R, color.G, color.B) { }
    
    public static implicit operator Color(Color3B color) =>
        new Color(color.R, color.G, color.B, GML.Graphics.Color.A);
}