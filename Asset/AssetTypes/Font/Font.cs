#define DYNAMIC_FONT_ENABLED

namespace GMLib;

public partial class GML
{
    public static AssetManager<Asset.Font> FontManager => AssetManager<Asset.Font>.Default;
    
    public static bool FontExists(Handle<Asset.Font> ind)
    {
        return FontManager.Exists(ind);
    }

    public static string FontGetName(Handle<Asset.Font> ind)
    {
        return FontManager.TryFindAsset(ind, out var font) ? font!.Name : "";
    }

    public static int FontGetSize(Handle<Asset.Font> ind)
    {
        return FontManager.TryFindAsset(ind, out var font) ? font!.Size : -1;
    }

    public static FontInfo FontGetInfo(Handle<Asset.Font> ind)
    {
        return FontManager.TryFindAsset(ind, out var font) ? font!.Info : default;
    }

    public static Texture2D FontGetTexture(Handle<Asset.Font> ind)
    {
        return FontManager.TryFindAsset(ind, out var font) ? font!.Texture : default;
    }

    public static UVs FontGetUVs(Handle<Asset.Font> ind)
    {
        return FontManager.TryFindAsset(ind, out var font) ? font!.UVs : default;
    }

    public static IFontGlyph FontCacheGlyph(Handle<Asset.Font> ind, int glyphIndex)
    {
        return FontManager.TryFindAsset(ind, out var font) ? font!.CacheGlyph(glyphIndex) : default(FontGlyph);
    }

    public static Handle<Asset.Font> FontAdd(string name, int size, int first, int last)
    {
        // TODO 实现独立的基于first -> last的FontAdd
        var resource = new ExpandoFont(name, size);
        var range = "";
        for (var i = first; i <= last; i++)
            range += ((char)i).ToString();
        
        resource.AddGlyphsFromString(range);
        
        return (Handle<Asset.Font>)FontManager.Register(
            new Asset.Font(
                $"ExternalFont_{FontManager.Counter}", resource))!;
    }
    
    public static Handle<Asset.Font> FontAdd(string name, int size, string range)
    {
        var resource = new ExpandoFont(name, size);
        resource.AddGlyphsFromString(range);
        
        return (Handle<Asset.Font>)FontManager.Register(
            new Asset.Font(
                $"ExternalFont_{FontManager.Counter}", resource))!;
    }

    public static void FontDelete(Handle<Asset.Font> ind)
    {
        if (!FontManager.TryFindAsset(ind, out var font)) return;
        font!.Dispose();
        FontManager.Remove(ind);
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
    [RegisterAssetType(AssetType.Font)]
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
            Name = Name,
            Texture = Texture
        };
        public Texture2D Texture => Resource.Texture;
        public IReadOnlyDictionary<int, IFontGlyph> Glyphs => Resource.Glyphs;
        
        internal Font(string name, IFont font) : base(name)
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
        
        public static Font ResolveHandle(Handle<Font> handle)
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