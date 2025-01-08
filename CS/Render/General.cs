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
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="alpha"></param>
    public static void DrawSetAlpha(float alpha)
    {
        GML.GraphicManager.Color = GML.GraphicManager.Color with { A = (byte)(255 * alpha) };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="alpha"></param>
    public static void DrawSetAlpha(byte alpha)
    {
        GML.GraphicManager.Color = GML.GraphicManager.Color with { A = alpha };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public static void DrawSetColor(Color3B color)
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
    public static void DrawSetColor(Color color)
    {
        GML.GraphicManager.Color = color;
    }

}

 /// <summary>
/// The honrizontal alignment property.
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
public struct FontAlignment
{
    public static readonly FontAlignment Default = new(FontHAlignment.Left, FontVAlignment.Top);
    public FontHAlignment HAlignment;
    public FontVAlignment VAlignment;

    public FontAlignment(FontHAlignment halign, FontVAlignment valign)
    {
        HAlignment = halign;
        VAlignment = valign;
    }

    public static explicit operator Vector2(FontAlignment align)
    {
        Vector2 result = Vector2.Zero;
        switch (align.HAlignment)
        {
            case FontHAlignment.Left:
            result.X = 0f;
            break;
            case FontHAlignment.Center:
            result.X = 0.5f;
            break;
            case FontHAlignment.Right:
            result.X = 1f;
            break;
        }
        switch (align.VAlignment)
        {
            case FontVAlignment.Top:
            result.X = 0f;
            break;
            case FontVAlignment.Middle:
            result.X = 0.5f;
            break;
            case FontVAlignment.Bottom:
            result.X = 1f;
            break;
        }
        return result;
    }
}

/// <summary>
/// 
/// </summary>
public struct Transform2D
{
    public Vector2 Position;
    public Vector2 Scale;
    public float Rotation;
    public Transform2D(Vector2 position, Vector2 scale, float rotation)
    {
        Position = position;
        Scale = scale;
        Rotation = rotation;
    }
}

/// <summary>
/// GameMaker Color type, doesn't contain alpha channel.
/// When cast to Raylib color, it will read the environment's default rendering color from RenderingState class.
/// </summary>
public struct Color3B
{
    public byte R; 
    public byte G;
    public byte B;
    public byte? A;

    public Color3B(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }
    public Color3B(byte r, byte g, byte b, byte a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    public Color3B(Color color)
    {
        R = color.R;
        G = color.G;
        B = color.B;
    }

    public static implicit operator Color(Color3B color)
    {
        var alpha = color.A ?? GML.GraphicManager.Color.A;
        return new Color(color.R, color.G, color.B, alpha);
    }
}

/// <summary>
/// This static class works as a global GameMaker rendering state.
/// Editing fields under this class will affect drawing effects after.
/// </summary>
public partial class GraphicManager
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
    public Asset.Font Font { get; set; } = Asset.Font.Default;
    public Color Color { get; set; } = Color.White;
}