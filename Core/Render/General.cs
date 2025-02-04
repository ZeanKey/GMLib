using System.Numerics;

namespace GMLib;

public static partial class GML
{   
    public static readonly GraphicManager Graphics = new();

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
        return Graphics.Alpha;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="alpha"></param>
    public static void DrawSetAlpha(float alpha)
    {
        Graphics.Color = Graphics.Color with { A = (byte)(255 * alpha) };
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="alpha"></param>
    public static void DrawSetAlpha(byte alpha)
    {
        GML.Graphics.Color = GML.Graphics.Color with { A = alpha };
    }

    public static Color3B DrawGetColour()
    {
        return Graphics.Colour;
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    public static void DrawSetColour(Color3B color)
    {
        GML.Graphics.Color = GML.Graphics.Color with
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
        GML.Graphics.Color = color;
    }

}