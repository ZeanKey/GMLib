#define DYNAMIC_FONT_ENABLED

using System.Collections;
using System.Numerics;
using System.Runtime.InteropServices;
using static Raylib_cs.Raylib;

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

public interface IFont
{
    public int Size { get; }
    public int SDFSpread { get; }
    public Font Native { get; }
    public Texture2D Texture { get; }
    public IReadOnlyDictionary<int, IFontGlyph> Glyphs { get; }
}

public interface IFontGlyph
{
    public Rectangle Rec { get; }
    public int X { get; }
    public int Y { get; }
    public int Width { get; }
    public int Height { get; }
    public int Character { get; }
    public int Shift { get; }
    public Vector2 Offset { get; }
}

#region Native Raylib Font Decorator
public unsafe class NativeFont(Font font) : IFont, IDisposable
{
    private bool _disposedValue;
    ~NativeFont() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        UnloadFont(font);
        _disposedValue = true;
    }

    public int Size => font.BaseSize;
    public int SDFSpread => font.GlyphPadding;
    public Font Native => font;
    public Texture2D Texture => font.Texture;
    public IReadOnlyDictionary<int, IFontGlyph> Glyphs => new NativeFontGlyphCollection(font, font.Glyphs, font.GlyphCount);
}

public unsafe class NativeFontGlyph(Font font, GlyphInfo glyphInfo, int glyphIndex) : IFontGlyph
{
    public Rectangle Rec { get; } = font.Recs[glyphIndex];
    public int X => (int)Rec.X;
    public int Y => (int)Rec.Y;
    public int Width => (int)Rec.Width;
    public int Height => (int)Rec.Height;
    public int Character => glyphInfo.Value;
    public int Shift => glyphInfo.AdvanceX;
    public Vector2 Offset => new(glyphInfo.OffsetX, glyphInfo.OffsetY);
}

public unsafe class NativeFontGlyphCollection(Font font, GlyphInfo* glyphs, int glyphCount) : IReadOnlyDictionary<int, IFontGlyph>
{
    public IEnumerator<KeyValuePair<int, IFontGlyph>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => glyphCount;
    public bool ContainsKey(int key)
    {
        return TryGetValue(key, out _);
    }

    public bool TryGetValue(int key, out IFontGlyph value)
    {
        var ret = false;
        var index = 0;
        var fallbackIndex = 0;      // Get index of fallback glyph '?'

        // Look for character index in the unordered charset
        for (var i = 0; i < glyphCount; i++)
        {
            if (glyphs[i].Value == 63) fallbackIndex = i;
            if (glyphs[i].Value != key) continue;
            index = i;
            break;
        }

        if (index == 0 && glyphs[0].Value != key)
            index = fallbackIndex;
        else ret = true;

        value = new NativeFontGlyph(font, glyphs[index], index);
        return ret;
    }

    public IFontGlyph this[int key]
    {
        get
        { 
            TryGetValue(key, out var value);
            return value;
        }
    }

    public IEnumerable<int> Keys { get; }
    public IEnumerable<IFontGlyph> Values { get; }
}
#endregion

public struct FontInfo
{
    public int Size;
    public int SDFSpread;
    public string Name;
    public Texture2D Texture;
    public IReadOnlyDictionary<int, IFontGlyph> Glyphs;
}

public struct FontGlyph : IFontGlyph
{
    public Rectangle Rec { get; set; }
    
    #region Rec
    public int X => (int)Rec.X;
    public int Y => (int)Rec.Y;
    public int Width => (int)Rec.Width;
    public int Height => (int)Rec.Height;

    #endregion
    
    public int Character { get; set; }
    public int Shift { get; set; }
    public Vector2 Offset { get; set; }
    
}

public static class FontFileType
{
    public const string TrueType = ".ttf";
    public const string OpenType = ".otf";
}

#region Dynamic Font
public sealed class DynamicFont : IDisposable, IFont
{
    private bool _disposedValue;
    ~DynamicFont() => Dispose(false);

    public static int TextureWidth { get; internal set; } = 1024;
    public static int TextureHeight { get; internal set;  } = 1024;
    public static readonly Vector2 TextureSize = new(TextureWidth, TextureHeight);

    private Image GlyphImage = new()
    {
        Width = (int)TextureSize.X,
        Height = (int)TextureSize.Y,
        Format = PixelFormat.UncompressedGrayAlpha,
        Mipmaps = 1
    };
    
    private bool IsTextureLoaded;
    
    private GlyphInfo[] GlyphInfos;
    private Rectangle[] Recs;
    private int GlyphCount;
    private int Capacity;
    private Font NativeFont;

    public int Size { get; }
    public int SDFSpread => NativeFont.GlyphPadding;
    public Font Native => NativeFont;
    public Texture2D Texture => NativeFont.Texture;
    public unsafe IReadOnlyDictionary<int, IFontGlyph> Glyphs => new NativeFontGlyphCollection(NativeFont, NativeFont.Glyphs, NativeFont.GlyphCount);
    
    public Font Font => NativeFont;
    public readonly HashSet<int> GlyphIndices = [];

    private readonly FixedContainerRowPacker RectPacker = new(TextureWidth, TextureHeight);
    
    private GCHandle GlyphsHandle;
    private GCHandle GlyphDataHandle;
    private GCHandle RectsHandle;

    public DynamicFont(Font nativeFont, int size, out int[] exceed)
    {
        NativeFont = nativeFont;
        this.Size = size;

        unsafe
        {
            var nativeGlyphs = new ReadOnlySpan<GlyphInfo>(NativeFont.Glyphs, NativeFont.GlyphCount).ToArray();
            
            MemFree(NativeFont.Glyphs);
            MemFree(NativeFont.Recs);
            UnloadTexture(NativeFont.Texture);
            // The glyph images is reserved for further use, and will be released at Unload.
            // So here don't use the UnloadFontData

            GlyphInfos = [];
            var glyphData = new byte[TextureWidth * TextureHeight * 2];
            Recs = [];
        
            GlyphsHandle = GCHandle.Alloc(GlyphInfos, GCHandleType.Pinned);
            GlyphDataHandle = GCHandle.Alloc(glyphData, GCHandleType.Pinned);
            RectsHandle = GCHandle.Alloc(Recs, GCHandleType.Pinned);

            GlyphCount = 0;
            Capacity = 0;
            
            fixed (byte* glyphDataPtr = glyphData)
                GlyphImage.Data = glyphDataPtr;

            fixed (GlyphInfo* nativeGlyphsPtr = nativeGlyphs)
                AddGlyphs(nativeGlyphsPtr, nativeGlyphs.Length, out exceed);
        }
    }
    
    private bool AddGlyphImage(ref Image glyphImage, ref Rectangle glyphRec)
    {
        const int Padding = 4;
        
        Rectangle srcRec = new(0, 0, glyphImage.Width, glyphImage.Height);
        Vector2 dest;

        if (RectPacker.TryPack(new Rectangle(Vector2.Zero, srcRec.Size + Vector2.One * Padding * 2), out var position))
            dest = (Vector2)position!;
        else return false;

        glyphRec.Position = dest + new Vector2(Padding, Padding); ;
        glyphRec.Size = new Vector2(glyphImage.Width, glyphImage.Height);

        if (glyphImage.Format == PixelFormat.UncompressedGrayscale)
        { 
            unsafe
            {
                var dataGrayAlpha = (byte*)MemAlloc((uint)(glyphImage.Width * glyphImage.Height * sizeof(byte) * 2)); // Two channels

                for (int i = 0, k = 0; i < glyphImage.Width * glyphImage.Height; i++, k += 2)
                {
                    dataGrayAlpha[k] = 255;
                    dataGrayAlpha[k + 1] = ((byte*)glyphImage.Data)[i];
                }

                MemFree(glyphImage.Data);
                glyphImage.Data = dataGrayAlpha;
                glyphImage.Format = PixelFormat.UncompressedGrayAlpha;
            }
        }

        ImageDraw(ref GlyphImage, glyphImage, srcRec, glyphRec with { Position = dest }, Color.White);
        return true;
    }
    
    public bool AddGlyph(GlyphInfo glyphInfo)
    {
        if (GlyphIndices.Contains(glyphInfo.Value))
        {
            UnloadImage(glyphInfo.Image);
            return false;
        }
        
        Rectangle rect = new();
        if (!AddGlyphImage(ref glyphInfo.Image, ref rect))
            return false;
        
        ResizeForSize(1);
        GlyphInfos[GlyphCount] = glyphInfo;
        Recs[GlyphCount] = rect;
        NativeFont.GlyphCount = ++GlyphCount;
            
        GlyphIndices.Add(glyphInfo.Value);

        if (IsTextureLoaded)
            UnloadTexture(NativeFont.Texture);
        else IsTextureLoaded = true;
        NativeFont.Texture = LoadTextureFromImage(GlyphImage);
        return true;
    }
    
    public unsafe bool AddGlyphs(GlyphInfo* glyphInfos, int count, out int[] exceed)
    {
        var unloaded = new GlyphInfo[count];
        var unloadedCount = 0;
        for (var i = 0; i < count; i++)
            if (!GlyphIndices.Contains(glyphInfos[i].Value))
            {
                unloaded[unloadedCount++] = glyphInfos[i];
            }
            else UnloadImage(glyphInfos[i].Image);
        
        var glyphsExceedList = new List<int>(unloadedCount);
        
        Rectangle[] recs = new Rectangle[unloadedCount];
        var loadedCount = 0;
        var isExceeded = false;
        for (var i = 0; i < unloadedCount; i++)
            if (isExceeded)
            {
                UnloadImage(unloaded[i].Image);
                glyphsExceedList.Add(unloaded[i].Value);
            }
            else if (!AddGlyphImage(ref unloaded[i].Image, ref recs[i]))
            {
                isExceeded = true;
            }
            else
            {
                GlyphIndices.Add(unloaded[i].Value);
                loadedCount++;
            }
        
        exceed = glyphsExceedList.ToArray();
        if (loadedCount == 0) return true;
        
        ResizeForSize(loadedCount);
        
        for (var i = 0; i < loadedCount; i++)
        {
            GlyphInfos[GlyphCount] = unloaded[i];
            Recs[GlyphCount] = recs[i];
            NativeFont.GlyphCount = ++GlyphCount;
        }
        
        if (IsTextureLoaded)
            UnloadTexture(NativeFont.Texture);
        else IsTextureLoaded = true;
        NativeFont.Texture = LoadTextureFromImage(GlyphImage);
        return !isExceeded;
    }
    
    public bool AddGlyphsFromString(string range, byte[] fontData, out int[] exceed)
    {
        var result = false;
        var codepointCount = 0;
        var codepoints = LoadCodepoints(range, ref codepointCount);
        unsafe
        {
            fixed (int* codepointsPtr = codepoints)
            {
                fixed (byte* glyphInfos = fontData)
                {
                    var glyphData = LoadFontData(glyphInfos, fontData.Length, Size, codepointsPtr, codepointCount,
                        FontType.Default);
                    result = AddGlyphs(glyphData, codepointCount, out exceed);
                    MemFree(glyphData); // theoretically, all the image data is either managed or unloaded inside
                }
            }
        }
        return result;
    }

    public unsafe void ResizeForSize(int size)
    {
        if (Capacity >= GlyphCount + size) return;
        
        GlyphsHandle.Free();
        RectsHandle.Free();

        if (Capacity < 128) 
            Capacity = 128; 
        while (Capacity < GlyphCount + size) Capacity *= 2;

        Array.Resize(ref GlyphInfos, Capacity);
        Array.Resize(ref Recs, Capacity);
            
        GlyphsHandle = GCHandle.Alloc(GlyphInfos, GCHandleType.Pinned);
        RectsHandle = GCHandle.Alloc(Recs, GCHandleType.Pinned);

        fixed (GlyphInfo* glyphs = this.GlyphInfos)
            NativeFont.Glyphs = glyphs;
        fixed (Rectangle* rects = Recs)
            NativeFont.Recs = rects;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        
        for (var i = 0; i < GlyphCount; i++)
            UnloadImage(GlyphInfos[i].Image);
        UnloadTexture(NativeFont.Texture);
        GlyphsHandle.Free();
        RectsHandle.Free();
        GlyphDataHandle.Free();
        
        _disposedValue = true;
    }

    public static explicit operator Font(DynamicFont value)
    {
        return value.Font;
    }
    
    private class FixedContainerRowPacker(int containerWidth, int containerHeight)
    {
        private int _currentX = 0;
        private int _currentY = 0;
        private int _currentRowHeight = 0;

        // Attempts to pack a rectangle; returns true if successful
        public bool TryPack(Rectangle rect, out Vector2? position)
        {
            position = null;
            if (rect.Width > containerWidth || rect.Height > containerHeight)
            {
                // Rectangle is too large to fit in the container
                return false;
            }

            // Check if it fits in the current row
            if (_currentX + rect.Width > containerWidth)
            {
                // Move to the next row
                _currentX = 0;
                _currentY += _currentRowHeight;
                _currentRowHeight = 0;

                // Check if there's enough space for a new row
                if (_currentY + rect.Height > containerHeight)
                {
                    return false; // No more space in the container
                }
            }
            
            position = new Vector2(_currentX, _currentY);
            
            _currentX += (int)rect.Width;
            _currentRowHeight = Math.Max(_currentRowHeight, (int)rect.Height);
            
            return true;
        }
    }
}

public sealed class ExpandoFont : IDisposable, IFont
{
    private int FontIndex;
    private readonly byte[] Data;

    private readonly List<DynamicFont> Fonts = [];
    public int Size { get; }
    
    private bool _disposedValue;
    
    ~ExpandoFont() => Dispose(false);
    
    public ExpandoFont(string path, int size)
    {
        if (Path.GetExtension(path) != FontFileType.TrueType) throw new Exception();
        
        Size = size;
        int byteRead = 0;
        unsafe
        {
            var nativeData = LoadFileData(path, ref byteRead);
            Data = new ReadOnlySpan<byte>(nativeData, byteRead).ToArray();
            UnloadFileData(nativeData);
        }
        
        var native = LoadFontEx(path, size, null, 0); // this font is released inside the ctor of managed font
        Fonts.Add(new DynamicFont(native, size, out _));
    }
    
    public ExpandoFont(string path, int size, string range)
    {
        var type = Path.GetExtension(path);
        if (type != FontFileType.TrueType && type != FontFileType.OpenType) throw new Exception();
        
        Size = size;
        var byteRead = 0;
        unsafe
        {
            var nativeData = LoadFileData(path, ref byteRead);
            Data = new ReadOnlySpan<byte>(nativeData, byteRead).ToArray();
            UnloadFileData(nativeData);
        }
        var count = 0;
        var codepoints = LoadCodepoints(range, ref count);
        
        var native = LoadFontEx(path, size, codepoints, count);
        Fonts.Add(new DynamicFont(native, size, out var exceed));
        while (exceed.Length != 0)
        {
            FontIndex++;
            native = LoadFontFromMemory(type, Data, Size, exceed, exceed.Length);
            Fonts.Add(new DynamicFont(native, Size, out exceed));
        }
    }
    
    public void AddGlyphsFromString(string range)
    {
        var fontManaged = Fonts[FontIndex];
        if (fontManaged.AddGlyphsFromString(range, Data, out var exceed)) return;
        while (exceed.Length != 0)
        {
            FontIndex++;
            var native = LoadFontFromMemory(FontFileType.TrueType, Data, Size, exceed, exceed.Length);
            Fonts.Add(new DynamicFont(native, Size, out exceed));
        }
    }

    public Font GetFont(int sample)
    {
        foreach (var managedFont in Fonts)
        {
            if (managedFont.GlyphIndices.Contains(sample)) return managedFont.Font;
        }
        AddGlyphsFromString(sample.ToString());
        return Fonts[FontIndex].Font;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue) return;

        if (disposing)
        {
            foreach (var managedFont in Fonts) managedFont.Dispose();
        }
        
        _disposedValue = true;
    }

    public int SDFSpread => Fonts[0].SDFSpread;
    public Font Native => Fonts[0].Native;
    public Texture2D Texture => Fonts[0].Texture;
    public IReadOnlyDictionary<int, IFontGlyph> Glyphs => Fonts[0].Glyphs;
}
#endregion

public partial class Asset
{
    /// <summary>
    /// 
    /// </summary>
    [AssetClassOf(AssetType.Font)]
    public sealed class Font : Asset, IHandleable<Font>, IFont, IDisposable
    {
        private readonly IFont _resource;
        
        public Raylib_cs.Font Native => _resource.Native;
        
        public int Size => _resource.Size;
        public int SDFSpread => _resource.SDFSpread;
        public UVs UVs => new(0, 0, 1, 1); // TODO
        public FontInfo Info => new FontInfo() {
            Size = _resource.Size,
            SDFSpread = _resource.SDFSpread,
            Glyphs = Glyphs,
            Name = Name,
            Texture = Texture
        };
        public Texture2D Texture => _resource.Texture;
        public IReadOnlyDictionary<int, IFontGlyph> Glyphs => _resource.Glyphs;
        
        internal Font(string name, IFont font) : base(name)
        {
            _resource = font;
        }

        public IFontGlyph CacheGlyph(int glyphIndex)
        {
            if (_resource is ExpandoFont expandoFont)
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
        private bool _disposedValue;
        ~Font() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing && _resource is IDisposable disposable) 
                disposable.Dispose();
            _disposedValue = true;
        }
        #endregion
    }
}