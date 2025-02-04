namespace GMLib;

public static class Vector2Helper
{
    public static float Angle(this Vector2 vector)
    {
        return MathF.Atan2(vector.Y, vector.X);
    }
    
    public static Vector2 Normalized(this Vector2 vector)
    {
        return vector / vector.Length();
    }
    
    public static Vector2 FromAngle(float angle)
    {
        return new Vector2(float.Cos(angle), float.Sin(angle));
    }
}