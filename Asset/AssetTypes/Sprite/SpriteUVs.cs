namespace GMLib;

/// <summary>
/// 
/// </summary>
/// <param name="Position"></param>
/// <param name="Size"></param>
/// <param name="Offset"></param>
/// <param name="NormalizationRatio"></param>
public record struct SpriteUVs
(
    Vector2 Position,
    Vector2 Size,
    Vector2 Offset,
    Vector2 NormalizationRatio
);