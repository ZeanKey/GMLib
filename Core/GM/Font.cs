namespace GMLib.GM;

public static class Font
{
    public static bool Exists(this Handle<GMLib.Asset.Font> ind)
        => GML.Font.Contains(ind);

    public static string GetFontName(this Handle<GMLib.Asset.Font> ind)
        => ind.Target?.FontName ?? "";
    
    public static int GetFirst(this Handle<GMLib.Asset.Font> ind)
        => ind.Target?.First ?? -1;
    
    public static int GetLast(this Handle<GMLib.Asset.Font> ind)
        => ind.Target?.Last ?? -1;
    
    public static bool GetItalic(this Handle<GMLib.Asset.Font> ind)
        => ind.Target?.Italic ?? false;
    
    public static bool GetBold(this Handle<GMLib.Asset.Font> ind)
        => ind.Target?.Bold ?? false;
    
    public static int GetSize(this Handle<GMLib.Asset.Font> ind)
        => ind.Target?.Size ?? 0;

    public static FontInfo GetInfo(this Handle<GMLib.Asset.Font> ind)
        => ind.Target?.Info ?? default;

    public static Texture2D GetTexture(this Handle<GMLib.Asset.Font> ind)
        => ind.Target?.Texture ?? default;

    public static UVs GetUVs(this Handle<GMLib.Asset.Font> ind)
        => ind.Target?.UVs ?? default;

    public static IFontGlyph CacheGlyph(this Handle<GMLib.Asset.Font> ind, int glyphIndex)
    {
        return ind.Target?.CacheGlyph(glyphIndex) ?? default(FontGlyph);
    }
    
    public static Handle<GMLib.Asset.Font> Add(string name, int size, bool bold, bool italic, int first, int last)
    {
        var resource = new ExpandoFont(name, size);
        var range = "";
        for (var i = first; i <= last; i++)
            range += ((char)i).ToString();
        
        resource.AddGlyphsFromString(range);
        
        var font = new GMLib.Asset.Font(resource)
        {
            FontName = name,
            Italic = italic,
            Bold = bold,
            First = first,
            Last = last
        };
        
        return GML.Font.Register(name, font);
    }
    
    public static Handle<GMLib.Asset.Font> Add(string name, int size, string range)
    {
        var resource = new ExpandoFont(name, size);
        resource.AddGlyphsFromString(range);
        
        var font = new GMLib.Asset.Font(resource)
        {
            FontName = name,
            Italic = false,
            Bold = false,
            First = -1,
            Last = -1
        };
        
        return GML.Font.Register(name, font);
    }

    public static Handle<GMLib.Asset.Font> AddSprite(Handle<GMLib.Asset.Sprite> spr, int first, bool prop, int sep)
    {
        var sprite = spr.Target ?? throw new Exception();
      
        var range = "";
        for (var i = 0; i < sprite.Number; i++)
        {
            range += (char)i;
        }

        return AddSpriteExt(spr, range, prop, sep);
    }
    
    public static Handle<GMLib.Asset.Font> AddSpriteExt(Handle<GMLib.Asset.Sprite> spr, string stringMap, bool prop, int sep)
    {
        var sprite = spr.Target ?? throw new Exception();

        var resource = new ExpandoFont(stringMap, sprite.Frames.Select(frame => frame.Image).ToArray());
        
        var font = new GMLib.Asset.Font(resource)
        {
            FontName = "undefined",
            Italic = false,
            Bold = false,
            First = -1,
            Last = -1
        };
        
        return GML.Font.Register(GML.Font.NextName(), font);
    }

    public static void Delete(this Handle<GMLib.Asset.Font> ind)
    {
        ind.Target?.Dispose();
        GML.Font.Remove(ind);
    }
        
    public static Vector2 TexturePageSize
    {
        get => DynamicFont.TextureSize;
        set => DynamicFont.TextureSize = value;
    }
}