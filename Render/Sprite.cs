
using System.Numerics;

namespace GMLib;

public static partial class GML
{
    public static void DrawSpriteOf()
    {

    }

    public static void DrawSprite(Handle<Asset.Sprite> sprite, int subimg, Vector2 position)
    {
        (((WeakReference?)sprite)?.Target as Asset.Sprite)?.Draw(subimg, position);
    }

    public static void DrawSpriteExt(Handle<Asset.Sprite> sprite, int subimg, Vector2 position, Vector2 scale, float rot, Color colour)
    {
        (((WeakReference?)sprite)?.Target as Asset.Sprite)?.Draw(subimg, position, scale, rot, colour);
    }

    public static void DrawSpriteGeneral(Handle<Asset.Sprite> sprite, int subimg, Rectangle source, Vector2 position, Vector2 scale, float rot, Color c1, Color c2, Color c3, Color c4)
    {
        var spr = ((WeakReference?)sprite)?.Target as Asset.Sprite;
        spr?.Draw(subimg, source, new(position, scale * spr.Size), rot, c1, c2, c3, c4);
    }

    public static void DrawSpritePart(Handle<Asset.Sprite> sprite, int subimg, Rectangle source, Vector2 position)
    {
        (((WeakReference?)sprite)?.Target as Asset.Sprite)?.Draw(subimg, source, position);   
    }

    public static void DrawSpritePartExt(Handle<Asset.Sprite> sprite, int subimg, Rectangle source, Vector2 position, Vector2 scale, Color colour)
    {
        (((WeakReference?)sprite)?.Target as Asset.Sprite)?.Draw(subimg, source, position, scale, colour);
    }

    public static void DrawSpriteStretched(Handle<Asset.Sprite> sprite, int subimg, Rectangle dest)
    {
        (((WeakReference?)sprite)?.Target as Asset.Sprite)?.Draw(subimg, dest);
    }

    public static void DrawSpriteStretchedExt(Handle<Asset.Sprite> sprite, int subimg, Rectangle dest, Color colour)
    {
        (((WeakReference?)sprite)?.Target as Asset.Sprite)?.Draw(subimg, dest, colour);
    }

    // TODO: DrawSpritePos
    public static void DrawSpritePos(Handle<Asset.Sprite> sprite, int subimg, Vector2 pos1, Vector2 pos2, Vector2 pos3, Vector2 pos4, byte alpha)
    {
        throw new NotImplementedException();
    }

    // TODO: DrawSpriteTiled
    public static void DrawSpriteTiled(Handle<Asset.Sprite> sprite, int subimg, Vector2 position)
    {
        throw new NotImplementedException();
    }

    // TODO: DrawSpriteTiledExt
    public static void DrawSpriteTiledExt(Handle<Asset.Sprite> sprite, int subimg, Vector2 position, Vector2 scale, Color colour)
    {
        throw new NotImplementedException();
    }
}