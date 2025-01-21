using System.Runtime.InteropServices;

namespace GMLib;

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
            
            Raylib.MemFree(NativeFont.Glyphs);
            Raylib.MemFree(NativeFont.Recs);
            Raylib.UnloadTexture(NativeFont.Texture);
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
                var dataGrayAlpha = (byte*)Raylib.MemAlloc((uint)(glyphImage.Width * glyphImage.Height * sizeof(byte) * 2)); // Two channels

                for (int i = 0, k = 0; i < glyphImage.Width * glyphImage.Height; i++, k += 2)
                {
                    dataGrayAlpha[k] = 255;
                    dataGrayAlpha[k + 1] = ((byte*)glyphImage.Data)[i];
                }

                Raylib.MemFree(glyphImage.Data);
                glyphImage.Data = dataGrayAlpha;
                glyphImage.Format = PixelFormat.UncompressedGrayAlpha;
            }
        }

        Raylib.ImageDraw(ref GlyphImage, glyphImage, srcRec, glyphRec with { Position = dest }, Color.White);
        return true;
    }
    
    public bool AddGlyph(GlyphInfo glyphInfo)
    {
        if (GlyphIndices.Contains(glyphInfo.Value))
        {
            Raylib.UnloadImage(glyphInfo.Image);
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
            Raylib.UnloadTexture(NativeFont.Texture);
        else IsTextureLoaded = true;
        NativeFont.Texture = Raylib.LoadTextureFromImage(GlyphImage);
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
            else Raylib.UnloadImage(glyphInfos[i].Image);
        
        var glyphsExceedList = new List<int>(unloadedCount);
        
        Rectangle[] recs = new Rectangle[unloadedCount];
        var loadedCount = 0;
        var isExceeded = false;
        for (var i = 0; i < unloadedCount; i++)
            if (isExceeded)
            {
                Raylib.UnloadImage(unloaded[i].Image);
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
            Raylib.UnloadTexture(NativeFont.Texture);
        else IsTextureLoaded = true;
        NativeFont.Texture = Raylib.LoadTextureFromImage(GlyphImage);
        return !isExceeded;
    }
    
    public bool AddGlyphsFromString(string range, byte[] fontData, out int[] exceed)
    {
        var result = false;
        var codepointCount = 0;
        var codepoints = Raylib.LoadCodepoints(range, ref codepointCount);
        unsafe
        {
            fixed (int* codepointsPtr = codepoints)
            {
                fixed (byte* glyphInfos = fontData)
                {
                    var glyphData = Raylib.LoadFontData(glyphInfos, fontData.Length, Size, codepointsPtr, codepointCount,
                        FontType.Default);
                    result = AddGlyphs(glyphData, codepointCount, out exceed);
                    Raylib.MemFree(glyphData); // theoretically, all the image data is either managed or unloaded inside
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
            Raylib.UnloadImage(GlyphInfos[i].Image);
        Raylib.UnloadTexture(NativeFont.Texture);
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