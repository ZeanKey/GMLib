using System.Collections;

namespace GMLib;

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