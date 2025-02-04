namespace GMLib;

/// <summary>
/// The alignment of the text.
/// </summary>
/// <param name="halign"></param>
/// <param name="valign"></param>
public struct FontAlignment(FontHAlignment halign, FontVAlignment valign)
{
    public static readonly FontAlignment Default = new(FontHAlignment.Left, FontVAlignment.Top);
    public FontHAlignment HAlignment = halign;
    public FontVAlignment VAlignment = valign;

    public static explicit operator Vector2(FontAlignment align)
    {
        var result = Vector2.Zero;
        result.X = align.HAlignment switch
        {
            FontHAlignment.Left => 0f,
            FontHAlignment.Center => 0.5f,
            FontHAlignment.Right => 1f,
            _ => result.X
        };
        result.Y = align.VAlignment switch
        {
            FontVAlignment.Top => 0f,
            FontVAlignment.Middle => 0.5f,
            FontVAlignment.Bottom => 1f,
            _ => result.Y
        };
        return result;
    }
}