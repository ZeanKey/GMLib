
using System.Numerics;

namespace GMLib;

public static partial class GML
{
    public static AssetManager<Asset.Sprite> Sprite => AssetManager<Asset.Sprite>.Default;
    
    public static void DrawSpriteOf()
    {

    }

    public static void DrawSprite(Asset.Sprite sprite, int subimg, Vector2 position)
    {
        sprite.Draw(subimg, position);
    }

    public static void DrawSpriteExt(Asset.Sprite sprite, int subimg, Vector2 position, Vector2 scale, float rot, Color colour)
    {
        sprite.Draw(subimg, position, scale, rot, colour);
    }

    public static void DrawSpriteGeneral(Asset.Sprite sprite, int subimg, Rectangle source, Vector2 position, Vector2 scale, float rot, Color c1, Color c2, Color c3, Color c4)
    {
        sprite.Draw(subimg, source, new(position, scale * sprite.Size), rot, c1, c2, c3, c4);
    }

    public static void DrawSpritePart(Asset.Sprite sprite, int subimg, Rectangle source, Vector2 position)
    {
        sprite.Draw(subimg, source, position);   
    }

    public static void DrawSpritePartExt(Asset.Sprite sprite, int subimg, Rectangle source, Vector2 position, Vector2 scale, Color colour)
    {
        sprite.Draw(subimg, source, position, scale, colour);
    }

    public static void DrawSpriteStretched(Asset.Sprite sprite, int subimg, Rectangle dest)
    {
        sprite.Draw(subimg, dest);
    }

    public static void DrawSpriteStretchedExt(Asset.Sprite sprite, int subimg, Rectangle dest, Color colour)
    {
        sprite.Draw(subimg, dest, colour);
    }

    // TODO: DrawSpritePos
    public static void DrawSpritePos(Asset.Sprite sprite, int subimg, Vector2 pos1, Vector2 pos2, Vector2 pos3, Vector2 pos4, byte alpha)
    {
        throw new NotImplementedException();
    }

    // TODO: DrawSpriteTiled
    public static void DrawSpriteTiled(Asset.Sprite sprite, int subimg, Vector2 position)
    {
        throw new NotImplementedException();
    }

    // TODO: DrawSpriteTiledExt
    public static void DrawSpriteTiledExt(Asset.Sprite sprite, int subimg, Vector2 position, Vector2 scale, Color colour)
    {
        throw new NotImplementedException();
    }
}