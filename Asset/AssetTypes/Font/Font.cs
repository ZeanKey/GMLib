#define DYNAMIC_FONT_ENABLED

namespace GMLib;

public partial class GML
{
    public static AssetManager<Asset.Font> Font => AssetManager<Asset.Font>.Default;
    
    public static bool FontExists(Handle<Asset.Font> ind)
    {
        return Font.Contains(ind);
    }

    public static int FontGetSize(Handle<Asset.Font> ind)
    {
        return Font.TryFindAsset(ind, out var font) ? 
            (font!.Target as Asset.Font)?.Size ?? -1 : -1;
    }

    public static FontInfo FontGetInfo(Handle<Asset.Font> ind)
    {
        return Font.TryFindAsset(ind, out var font) ? 
            (font!.Target as Asset.Font)?.Info ?? default : default;
    }

    public static Texture2D FontGetTexture(Handle<Asset.Font> ind)
    {
        return Font.TryFindAsset(ind, out var font) ? 
            (font!.Target as Asset.Font)?.Texture ?? default : default;
    }

    public static UVs FontGetUVs(Handle<Asset.Font> ind)
    {
        return Font.TryFindAsset(ind, out var font) ? 
            (font!.Target as Asset.Font)?.UVs ?? default : default;
    }

    public static IFontGlyph FontCacheGlyph(Handle<Asset.Font> ind, int glyphIndex)
    {
        return Font.TryFindAsset(ind, out var font) ? 
            (font!.Target as Asset.Font)?.CacheGlyph(glyphIndex) ?? default(FontGlyph) : default(FontGlyph);
    }

    public static Handle<Asset.Font> FontAdd(string name, int size, int first, int last)
    {
        // TODO 实现独立的基于first -> last的FontAdd
        var resource = new ExpandoFont(name, size);
        var range = "";
        for (var i = first; i <= last; i++)
            range += ((char)i).ToString();
        
        resource.AddGlyphsFromString(range);
        
        return Font.Register(name, new Asset.Font(resource))!;
    }
    
    public static Handle<Asset.Font> FontAdd(string name, int size, string range)
    {
        var resource = new ExpandoFont(name, size);
        resource.AddGlyphsFromString(range);
        
        return Font.Register(name, new Asset.Font(resource))!;
    }

    public static void FontDelete(Handle<Asset.Font> ind)
    {
        if (!Font.TryFindAsset(ind, out var font)) return;
        (font!.Target as Asset.Font)?.Dispose();
        Font.Remove(ind);
    }

    public static int FontTexturePageSize
    {
        get => DynamicFont.TextureWidth;
        set
        {
            DynamicFont.TextureWidth = value;
            DynamicFont.TextureHeight = value;
        }
    }
}

public partial class Asset
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Font : Asset, IHandleable<Font>, IFont, IDisposable
    {
        private readonly IFont Resource;
        
        public Raylib_cs.Font Native => Resource.Native;
        
        public int Size => Resource.Size;
        public int SDFSpread => Resource.SDFSpread;
        public UVs UVs => new(0, 0, 1, 1); // TODO
        public FontInfo Info => new FontInfo() {
            Size = Resource.Size,
            SDFSpread = Resource.SDFSpread,
            Glyphs = Glyphs,
            Name = "",
            Texture = Texture
        };
        public Texture2D Texture => Resource.Texture;
        public IReadOnlyDictionary<int, IFontGlyph> Glyphs => Resource.Glyphs;
        
        internal Font(IFont font)
        {
            Resource = font;
        }

        public IFontGlyph CacheGlyph(int glyphIndex)
        {
            if (Resource is ExpandoFont expandoFont)
            {
                expandoFont.AddGlyphsFromString(((char)glyphIndex).ToString());
                return expandoFont.Glyphs[glyphIndex];
            }
            return default(FontGlyph);
        }

        public static WeakReference ResolveHandle(Handle<Font> handle)
        {
            throw new NotImplementedException();
        }

        #region Disposable
        private bool DisposedValue;
        
        ~Font() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (DisposedValue) return;
            if (disposing && Resource is IDisposable disposable) 
                disposable.Dispose();
            DisposedValue = true;
        }
        #endregion
    }
}