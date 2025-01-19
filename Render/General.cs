using System.Numerics;

namespace GMLib;

public static partial class GML
{   
    public static GraphicManager GraphicManager = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static Matrix4x4 MatrixBuild(Vector3 position, Vector3 rotation, Vector3 scale)
    {
        Matrix4x4 mat = Matrix4x4.Identity;
        mat *= Raymath.MatrixScale(scale.X, scale.Y, scale.Z);
        mat *= Raymath.MatrixRotateXYZ(rotation);
        mat *= Raymath.MatrixTranslate(position.X, position.Y, position.Z);
        return mat;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <param name="scale"></param>
    /// <returns></returns>
    public static Matrix4x4 MatrixBuild(Vector2 position, float rotation, Vector2 scale)
    {
        Matrix4x4 mat = Matrix4x4.Identity;
        mat *= Raymath.MatrixScale(scale.X, scale.Y, 1);
        mat *= Raymath.MatrixRotateZ(rotation);
        mat *= Raymath.MatrixTranslate(position.X, position.Y, 0);
        return mat;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static Matrix4x4 MatrixBuild(Transform2D transform)
    {
        return MatrixBuild(transform.Position, transform.Rotation, transform.Scale);
    }
    
    public static byte ColorGetRed(Color3B color) => color.R;
    public static byte ColorGetGreen(Color3B color) => color.G;
    public static byte ColorGetBlue(Color3B color) => color.B; 
    public static byte ColorGetHue(Color3B color) => color.Hue; 
    public static byte ColorGetSaturation(Color3B color) => color.Saturation; 
    public static byte ColorGetValue(Color3B color) => color.Value; 
    
    public static Color3B MakeColourHSV(byte h, byte s, byte v)
    {
        return new Color3B(Raylib.ColorFromHSV(h / 255f * 360f, s / 255f, v / 255f));
    }
    
    public static Color3B MakeColourRGB(byte r, byte g, byte b)
    {
        return new Color3B(r, g, b);
    }
    
    public static Color3B MergeColour(Color3B col1, Color3B col2, float amount)
    {
        var invAmount = 1.0f - amount;
        return new Color3B(
            (byte)(col1.R * invAmount + col2.R * amount), 
            (byte)(col1.G * invAmount + col2.G * amount), 
            (byte)(col1.B * invAmount + col2.B * amount));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public static void DrawClear(Color3B color)
    {
        Raylib.ClearBackground(new Color(color.R, color.G, color.B, (byte)255));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="alpha"></param>
    public static void DrawClearAlpha(Color3B color, float alpha)
    {
        Raylib.ClearBackground(new Color(color.R, color.G, color.B, (byte)(255 * alpha)));
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public static void DrawClearAlpha(Color color)
    {
        Raylib.ClearBackground(color);
    }

    public static float DrawGetAlpha()
    {
        return GraphicManager.Alpha;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="alpha"></param>
    public static void DrawSetAlpha(float alpha)
    {
        GraphicManager.Color = GraphicManager.Color with { A = (byte)(255 * alpha) };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="alpha"></param>
    public static void DrawSetAlpha(byte alpha)
    {
        GML.GraphicManager.Color = GML.GraphicManager.Color with { A = alpha };
    }

    public static Color3B DrawGetColour()
    {
        return GraphicManager.Colour;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public static void DrawSetColour(Color3B color)
    {
        GML.GraphicManager.Color = GML.GraphicManager.Color with
        {
            R = color.R,
            G = color.G,
            B = color.B
        };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public static void DrawSetColour(Color color)
    {
        GML.GraphicManager.Color = color;
    }

}

/// <summary>
/// The horizontal alignment property.
/// </summary>
public enum FontHAlignment
{
    Left, Center, Right
}

/// <summary>
/// The vertical alignment property.
/// </summary>
public enum FontVAlignment
{
    Top, Middle, Bottom
}

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

/// <summary>
/// 
/// </summary>
public struct Transform2D(Vector2 position, Vector2 scale, float rotation)
{
    public Vector2 Position = position;
    public Vector2 Scale = scale;
    public float Rotation = rotation;
}

/// <summary>
/// GameMaker Color type, doesn't contain alpha channel.
/// When cast to Raylib color, it will read the environment's default rendering color from RenderingState class.
/// </summary>
public struct Color3B(byte r, byte g, byte b)
{
    public readonly byte R = r; 
    public readonly byte G = g;
    public readonly byte B = b;

    public byte Hue => (byte)(Raylib.ColorToHSV(this).X / 360f * 255f);
    public byte Saturation => (byte)(Raylib.ColorToHSV(this).Y * 255f);
    public byte Value => (byte)(Raylib.ColorToHSV(this).Z * 255f);


    public Color3B(Color color) : this(color.R, color.G, color.B) { }

    public static implicit operator Color(Color3B color) =>
        new Color(color.R, color.G, color.B, GML.GraphicManager.Color.A);
}

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
    
    public Asset.Font Font { get; set; }
    
    public Color Color { get; set; } = Color.White;
    public Color3B Colour
    {
        get => new(Color);
        set => Color = value;
    } // GameMaker style colour (RGB channels only)
    public float Alpha
    {
        get => GML.GraphicManager.Color.A;
        set => GML.GraphicManager.Color = GML.GraphicManager.Color with { A = (byte)(255 * value) };
    } // GameMaker style alpha (A channel only)
}