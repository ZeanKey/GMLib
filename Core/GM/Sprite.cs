using Rectangle = Raylib_cs.Rectangle;

namespace GMLib.GM;

public static partial class Sprite;

partial class Sprite
{
    public static bool Exists(this Handle<GMLib.Asset.Sprite> index)
        => GML.Sprite.Contains(index);

    public static int GetNumber(this Handle<GMLib.Asset.Sprite> index)
        => index.Target?.Number ?? 0;
    
    public static float GetSpeed(this Handle<GMLib.Asset.Sprite> index)
        => index.Target?.Speed ?? 0;
    
    public static SpriteSpeedType GetSpeedType(this Handle<GMLib.Asset.Sprite> index)
        => index.Target?.SpeedType ?? SpriteSpeedType.Unknown;
    
    public static int GetWidth(this Handle<GMLib.Asset.Sprite> index)
        => (int)(index.Target?.Size.X ?? 0);
    
    public static int GetHeight(this Handle<GMLib.Asset.Sprite> index)
        => (int)(index.Target?.Size.Y ?? 0);
    
    public static Vector2 GetSize(this Handle<GMLib.Asset.Sprite> index)
        => index.Target?.Size ?? Vector2.Zero;
    
    public static int GetXOffset(this Handle<GMLib.Asset.Sprite> index)
        => (int)(index.Target?.Offset.X ?? 0);
    
    public static int GetYOffset(this Handle<GMLib.Asset.Sprite> index)
        => (int)(index.Target?.Offset.Y ?? 0);
    
    public static Vector2 GetOffset(this Handle<GMLib.Asset.Sprite> index)
        => index.Target?.Offset ?? Vector2.Zero;
}

partial class Sprite
{
    public static Handle<GMLib.Asset.Sprite> Add(string fName, int imgNumb, bool removeBack, bool smooth, Vector2 orig)
    {
        var sprite = GMLib.Asset.Sprite.Create(fName, imgNumb, orig);
        return GML.Sprite.Register(fName, sprite);
    }
    
    public static async Task<Handle<GMLib.Asset.Sprite>> AddExt(string fName, int imgNumb, bool removeBack, bool smooth, Vector2 orig)
    {
        var sprite = await GMLib.Asset.Sprite.CreateAsync(fName, imgNumb, orig);
        return GML.Sprite.Register(fName, sprite);
    }

    public static void Delete(this Handle<GMLib.Asset.Sprite> index)
    {
        GML.Sprite.Remove(index);
        index.Target?.Dispose();
    }

    public static void Replace(this Handle<GMLib.Asset.Sprite> index, string fName, int imgNumb, bool removeBack, bool smooth, Vector2 orig)
    {
        var sprite = GMLib.Asset.Sprite.Create(fName, imgNumb, orig);
        GML.Sprite.SetTarget(index, sprite);
    }
    
}

partial class Sprite
{
    public static void DrawSelf(this Handle<IObject> self)
    {

    }

    public static void Draw(this Handle<GMLib.Asset.Sprite> sprite, int subimg, Vector2 position)
    {
        sprite.Target?.Draw(subimg, position);
    }

    public static void DrawExt(this Handle<GMLib.Asset.Sprite> sprite, int subimg, Vector2 position, Vector2 scale, float rot, Color colour)
    {
        sprite.Target?.Draw(subimg, position, scale, rot, colour);
    }

    public static void DrawGeneral(this Handle<GMLib.Asset.Sprite> sprite, int subimg, Rectangle source, Vector2 position, Vector2 scale, float rot, Color c1, Color c2, Color c3, Color c4)
    {
        sprite.Target?.Draw(subimg, source, new Rectangle(position, scale * sprite.Target.Size), rot, c1, c2, c3, c4);
    }

    public static void DrawPart(this Handle<GMLib.Asset.Sprite> sprite, int subimg, Rectangle source, Vector2 position)
    {
        sprite.Target?.Draw(subimg, source, position);   
    }

    public static void DrawPartExt(this Handle<GMLib.Asset.Sprite> sprite, int subimg, Rectangle source, Vector2 position, Vector2 scale, Color colour)
    {
        sprite.Target?.Draw(subimg, source, position, scale, colour);
    }

    public static void DrawStretched(this Handle<GMLib.Asset.Sprite> sprite, int subimg, Rectangle dest)
    {
        sprite.Target?.Draw(subimg, dest);
    }

    public static void DrawStretchedExt(this Handle<GMLib.Asset.Sprite> sprite, int subimg, Rectangle dest, Color colour)
    {
        sprite.Target?.Draw(subimg, dest, colour);
    }

    public static void DrawPos(this Handle<GMLib.Asset.Sprite> sprite, int subimg, Vector2 pos1, Vector2 pos2, Vector2 pos3, Vector2 pos4, byte alpha)
    {
        throw new NotImplementedException();
    }

    public static void DrawTiled(this Handle<GMLib.Asset.Sprite> sprite, int subimg, Vector2 position)
    {
        throw new NotImplementedException();
    }

    public static void DrawTiledExt(this Handle<GMLib.Asset.Sprite> sprite, int subimg, Vector2 position, Vector2 scale, Color colour)
    {
        throw new NotImplementedException();
    }
}