
using System.Numerics;
using GMLib.GM;

namespace GMLib;

public static partial class GML
{
    public static void DrawSelf(Handle<IObject> self)
        => self.DrawSelf();

    public static void DrawSprite(Handle<Asset.Sprite> sprite, int subimg, Vector2 position)
        => sprite.Draw(subimg, position);

    public static void DrawSpriteExt(Handle<Asset.Sprite> sprite, int subimg, Vector2 position, Vector2 scale, float rot, Color colour)
        => sprite.DrawExt(subimg, position, scale, rot, colour);

    public static void DrawSpriteGeneral(Handle<Asset.Sprite> sprite, int subimg, Rectangle source, Vector2 position, Vector2 scale, float rot, Color c1, Color c2, Color c3, Color c4)
        => sprite.DrawGeneral(subimg, source, position, scale, rot, c1, c2, c3, c4);

    public static void DrawSpritePart(Handle<Asset.Sprite> sprite, int subimg, Rectangle source, Vector2 position)
        => sprite.DrawPart(subimg, source, position);

    public static void DrawSpritePartExt(Handle<Asset.Sprite> sprite, int subimg, Rectangle source, Vector2 position, Vector2 scale, Color colour)
        => sprite.DrawPartExt(subimg, source, position, scale, colour);

    public static void DrawSpriteStretched(Handle<Asset.Sprite> sprite, int subimg, Rectangle dest)
        => sprite.DrawStretched(subimg, dest);

    public static void DrawSpriteStretchedExt(Handle<Asset.Sprite> sprite, int subimg, Rectangle dest, Color colour)
        => sprite.DrawStretchedExt(subimg, dest, colour);

    public static void DrawSpritePos(Handle<Asset.Sprite> sprite, int subimg, Vector2 pos1, Vector2 pos2, Vector2 pos3, Vector2 pos4, byte alpha)
        => sprite.DrawPos(subimg, pos1, pos2, pos3, pos4, alpha);

    public static void DrawSpriteTiled(Handle<Asset.Sprite> sprite, int subimg, Vector2 position)
        => sprite.DrawTiled(subimg, position);

    public static void DrawSpriteTiledExt(Handle<Asset.Sprite> sprite, int subimg, Vector2 position, Vector2 scale, Color colour)
        => sprite.DrawTiledExt(subimg, position, scale, colour);
}