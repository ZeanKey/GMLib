using System;
using System.Numerics;

namespace GMLib;

public static unsafe class RaylibExtension
{
    const float DEG2RAD = MathF.PI / 180f;

    public static int TextLineSpacing { get; set; } = 2;

    public static void DrawTextExExt(Font font, sbyte * text, Vector2 position, Vector2 scale, Vector2 orientation, float sep, float spacing, Color c1, Color c2, Color c3, Color c4)
    {
        if (font.Texture.Id == 0) font = Raylib.GetFontDefault();  // Security check in case of not valid font

        int size = (int)Raylib.TextLength(text);    // Total size in bytes of the text, scanned by codepoints in loop

        float textOffsetY = 0;          // Offset between lines (on linebreak '\n')
        float textOffsetX = 0.0f;       // Offset X to next character to draw

        Vector2 fontSize = scale * font.BaseSize;
        Vector2 scaleFactor = scale;         // Character quad scaling factor

        int lineIndex;
        List<float> alignmentOffsetX = [0f];
        float alignmentOffsetY = 0;

        lineIndex = 0;
        for (int i = 0; i < size;)
        {
            // Get next codepoint from byte string and glyph index in font
            int codepointByteCount = 0;
            int codepoint = Raylib.GetCodepointNext(&text[i], &codepointByteCount);
            int index = Raylib.GetGlyphIndex(font, codepoint);

            if (codepoint == '\n')
            {
                // NOTE: Line spacing is a global variable, use SetTextLineSpacing() to setup
                alignmentOffsetY -= (fontSize.Y + sep) * orientation.Y;
                alignmentOffsetX.Add(0);
                lineIndex ++;
            }
            else
            {
                if (font.Glyphs[index].AdvanceX == 0)
                {
                    alignmentOffsetX[lineIndex] -= (font.Recs[index].Width * scaleFactor.X + spacing) * orientation.X;
                }
                else
                {
                    alignmentOffsetX[lineIndex] -= (font.Glyphs[index].AdvanceX * scaleFactor.X + spacing) * orientation.X;
                }
            }
            i += codepointByteCount;
        }

        lineIndex = 0;
        for (int i = 0; i < size;)
        {
            // Get next codepoint from byte string and glyph index in font
            int codepointByteCount = 0;
            int codepoint = Raylib.GetCodepointNext(&text[i], &codepointByteCount);
            int index = Raylib.GetGlyphIndex(font, codepoint);

            if (codepoint == '\n')
            {
                // NOTE: Line spacing is a global variable, use SetTextLineSpacing() to setup
                textOffsetY += fontSize.Y + sep;
                textOffsetX = 0.0f;
                lineIndex ++;
            }
            else
            {
                if ((codepoint != ' ') && (codepoint != '\t'))
                {
                    DrawTextCodepointExt(font, codepoint, new Vector2(position.X + textOffsetX + alignmentOffsetX[lineIndex], position.Y + textOffsetY + alignmentOffsetY), scale, c1, c2, c3, c4);
                }

                if (font.Glyphs[index].AdvanceX == 0) textOffsetX += font.Recs[index].Width * scaleFactor.X + spacing;
                else textOffsetX += font.Glyphs[index].AdvanceX * scaleFactor.X + spacing;
            }

            i += codepointByteCount;   // Move text bytes counter to next codepoint
        }
    }
    
    public static void DrawTextExExt(Font font, string text, Vector2 position, Vector2 scale, Vector2 orientation, float sep, float spacing, Color c1, Color c2, Color c3, Color c4)
    {
        using var str1 = text.ToUtf8Buffer();
        DrawTextExExt(font, str1.AsPointer(), position, scale, orientation, sep, spacing, c1, c2, c3, c4);
    }

    public static void DrawTextProExt(Font font, sbyte * text, Vector2 position, Vector2 origin, float rotation, Vector2 scale, Vector2 orientation, float sep, float spacing, Color c1, Color c2, Color c3, Color c4)
    {
        Rlgl.PushMatrix();

            Rlgl.Translatef(position.X, position.Y, 0.0f);
            Rlgl.Rotatef(rotation, 0.0f, 0.0f, 1.0f);
            Rlgl.Translatef(-origin.X, -origin.Y, 0.0f);

            DrawTextExExt(font, text, new Vector2(0.0f, 0.0f), scale, orientation, sep, spacing, c1, c2, c3, c4);

        Rlgl.PopMatrix();
    }
    
    public static void DrawTextProExt(Font font, string text, Vector2 position, Vector2 origin, float rotation, Vector2 scale, Vector2 orientation, float sep, float spacing, Color c1, Color c2, Color c3, Color c4)
    {
        using var str1 = text.ToUtf8Buffer();
        DrawTextProExt(font, str1.AsPointer(), position, origin, rotation, scale, orientation, sep, spacing, c1, c2, c3, c4);
    }

    public static void DrawTextCodepointExt(Font font, int codepoint, Vector2 position, Vector2 scale, Color c1, Color c2, Color c3, Color c4)
    {
        // Character index position in sprite font
        // NOTE: In case a codepoint is not available in the font, index returned points to '?'
        int index = Raylib.GetGlyphIndex(font, codepoint);
        Vector2 scaleFactor = scale;     // Character quad scaling factor

        // Character destination rectangle on screen
        // NOTE: We consider glyphPadding on drawing
        Rectangle dstRec = new
        (
            position.X + font.Glyphs[index].OffsetX*scaleFactor.X - (float)font.GlyphPadding*scaleFactor.X,
            position.Y + font.Glyphs[index].OffsetY*scaleFactor.Y - (float)font.GlyphPadding*scaleFactor.Y,
            (font.Recs[index].Width + 2.0f*font.GlyphPadding)*scaleFactor.X,
            (font.Recs[index].Height + 2.0f*font.GlyphPadding)*scaleFactor.Y
        );

        // Character source rectangle from font texture atlas
        // NOTE: We consider chars padding when drawing, it could be required for outline/glow shader effects
        Rectangle srcRec = new
        (
            font.Recs[index].X - (float)font.GlyphPadding, font.Recs[index].Y - (float)font.GlyphPadding,
            font.Recs[index].Width + 2.0f*font.GlyphPadding, font.Recs[index].Height + 2.0f*font.GlyphPadding
        );

        // Draw the character texture on the screen
        DrawTexturePro(font.Texture, srcRec, dstRec, new Vector2(0, 0), 0.0f, c1, c2, c3, c4);
    }
    
    #region Expando
    public static void DrawTextExExpando(ExpandoFont font, sbyte * text, Vector2 position, Vector2 scale, Vector2 orientation, float sep, float spacing, Color c1, Color c2, Color c3, Color c4)
    {
        int size = (int)Raylib.TextLength(text);    // Total size in bytes of the text, scanned by codepoints in loop

        float textOffsetY = 0;          // Offset between lines (on linebreak '\n')
        float textOffsetX = 0.0f;       // Offset X to next character to draw

        Vector2 fontSize = scale * font.Size;
        Vector2 scaleFactor = scale;         // Character quad scaling factor

        int lineIndex;
        List<float> alignmentOffsetX = [0f];
        float alignmentOffsetY = 0;

        lineIndex = 0;
        for (int i = 0; i < size;)
        {
            // Get next codepoint from byte string and glyph index in font
            int codepointByteCount = 0;
            int codepoint = Raylib.GetCodepointNext(&text[i], &codepointByteCount);
            var native = font.GetFont(codepoint);
            int index = Raylib.GetGlyphIndex(native, codepoint);

            if (codepoint == '\n')
            {
                // NOTE: Line spacing is a global variable, use SetTextLineSpacing() to setup
                alignmentOffsetY -= (fontSize.Y + sep) * orientation.Y;
                alignmentOffsetX.Add(0);
                lineIndex ++;
            }
            else
            {
                if (native.Glyphs[index].AdvanceX == 0)
                {
                    alignmentOffsetX[lineIndex] -= (native.Recs[index].Width * scaleFactor.X + spacing) * orientation.X;
                }
                else
                {
                    alignmentOffsetX[lineIndex] -= (native.Glyphs[index].AdvanceX * scaleFactor.X + spacing) * orientation.X;
                }
            }
            i += codepointByteCount;
        }

        lineIndex = 0;
        for (int i = 0; i < size;)
        {
            // Get next codepoint from byte string and glyph index in font
            int codepointByteCount = 0;
            int codepoint = Raylib.GetCodepointNext(&text[i], &codepointByteCount);
            var native = font.GetFont(codepoint);
            int index = Raylib.GetGlyphIndex(native, codepoint);

            if (codepoint == '\n')
            {
                // NOTE: Line spacing is a global variable, use SetTextLineSpacing() to setup
                textOffsetY += fontSize.Y + sep;
                textOffsetX = 0.0f;
                lineIndex ++;
            }
            else
            {
                if ((codepoint != ' ') && (codepoint != '\t'))
                {
                    DrawTextCodepointExt(native, codepoint, new Vector2(position.X + textOffsetX + alignmentOffsetX[lineIndex], position.Y + textOffsetY + alignmentOffsetY), scale, c1, c2, c3, c4);
                }

                if (native.Glyphs[index].AdvanceX == 0) textOffsetX += native.Recs[index].Width * scaleFactor.X + spacing;
                else textOffsetX += native.Glyphs[index].AdvanceX * scaleFactor.X + spacing;
            }

            i += codepointByteCount;   // Move text bytes counter to next codepoint
        }
    }
        
    public static void DrawTextProExpando(ExpandoFont font, sbyte * text, Vector2 position, Vector2 origin, float rotation, Vector2 scale, Vector2 orientation, float sep, float spacing, Color c1, Color c2, Color c3, Color c4)
    {
        Rlgl.PushMatrix();

        Rlgl.Translatef(position.X, position.Y, 0.0f);
        Rlgl.Rotatef(rotation, 0.0f, 0.0f, 1.0f);
        Rlgl.Translatef(-origin.X, -origin.Y, 0.0f);

        DrawTextExExpando(font, text, new Vector2(0.0f, 0.0f), scale, orientation, sep, spacing, c1, c2, c3, c4);

        Rlgl.PopMatrix();
    }

    public static void DrawTextProExpando(ExpandoFont font, string text, Vector2 position, Vector2 origin, float rotation, Vector2 scale, Vector2 orientation, float sep, float spacing, Color c1, Color c2, Color c3, Color c4)
    {
        using var str1 = text.ToUtf8Buffer();
        DrawTextProExpando(font, str1.AsPointer(), position, origin, rotation, scale, orientation, sep, spacing, c1, c2, c3, c4);
    }
    #endregion

    #region IFont
    public static void DrawTextProGeneral(IFont font, string text, Vector2 position, Vector2 origin, float rotation, Vector2 scale, Vector2 orientation, float sep, float spacing, Color c1, Color c2, Color c3, Color c4)
    {
        Rlgl.PushMatrix();

        Rlgl.Translatef(position.X, position.Y, 0.0f);
        Rlgl.Rotatef(rotation, 0.0f, 0.0f, 1.0f);
        Rlgl.Translatef(-origin.X, -origin.Y, 0.0f);

        DrawTextExGeneral(font, text, new Vector2(0.0f, 0.0f), scale, orientation, sep, spacing, c1, c2, c3, c4);

        Rlgl.PopMatrix();
    }
    
    public static void DrawTextExGeneral(IFont font, string text, Vector2 position, Vector2 scale, Vector2 orientation, float sep, float spacing, Color c1, Color c2, Color c3, Color c4)
    {
        var size = text.Length;

        var textOffsetY = 0f;
        var textOffsetX = 0f;

        var fontSize = scale * font.Size;
        var line = 0;

        List<float> alignmentOffsetX = [0f];
        var alignmentOffsetY = 0f;

        for (var i = 0; i < size; i++)
        {
            // Get next codepoint from byte string and glyph index in font
            var codepoint = (int)text[i];
            var glyph = font.Glyphs[codepoint];

            if (codepoint == '\n')
            {
                // NOTE: Line spacing is a global variable, use SetTextLineSpacing() to setup
                alignmentOffsetY -= (fontSize.Y + sep) * orientation.Y;
                alignmentOffsetX.Add(0);
                line ++;
                continue;
            }
            
            alignmentOffsetX[line] -= ((glyph.Shift == 0? glyph.Width : glyph.Shift) * scale.X + spacing) * orientation.X;
        }

        line = 0;
        for (var i = 0; i < size; i++)
        {
            var codepoint = (int)text[i];
            var glyph = font.Glyphs[codepoint];

            if (codepoint == '\n')
            {
                // NOTE: Line spacing is a global variable, use SetTextLineSpacing() to setup
                textOffsetY += fontSize.Y + sep;
                textOffsetX = 0.0f;
                line ++;
                continue;
            }

            if (codepoint != ' ' && codepoint != '\t')
                DrawTextCodepointGeneral(font, codepoint, new Vector2(position.X + textOffsetX + alignmentOffsetX[line], position.Y + textOffsetY + alignmentOffsetY), scale, c1, c2, c3, c4);

            textOffsetX += (glyph.Shift == 0 ? glyph.Width : glyph.Shift) * scale.X + spacing;
        }
    }
    
    public static void DrawTextCodepointGeneral(IFont font, int codepoint, Vector2 position, Vector2 scale, Color c1, Color c2, Color c3, Color c4)
    {
        Vector2 scaleFactor = scale;     // Character quad scaling factor
        var glyph = font.Glyphs[codepoint];
        
        // Character destination rectangle on screen
        // NOTE: We consider glyphPadding on drawing
        Rectangle dstRec = new(
            position + glyph.Offset * scaleFactor - font.SDFSpread * scaleFactor,
            (glyph.Rec.Size + Vector2.One * 2f * font.SDFSpread) * scaleFactor);

        // Character source rectangle from font texture atlas
        // NOTE: We consider chars padding when drawing, it could be required for outline/glow shader effects
        Rectangle srcRec = new(
            glyph.Rec.Position - font.SDFSpread * Vector2.One, 
            glyph.Rec.Size + font.SDFSpread * Vector2.One * 2f);

        // Draw the character texture on the screen
        DrawTexturePro(font.Texture, srcRec, dstRec, new Vector2(0, 0), 0.0f, c1, c2, c3, c4);
    }
    #endregion
    
    public static void DrawTexturePro(Texture2D texture, Rectangle source, Rectangle dest, Vector2 origin, float rotation, Color c1, Color c2, Color c3, Color c4)
    {
        // Check if texture is valid
        if (texture.Id > 0)
        {
            float width = (float)texture.Width * MathF.Sign(dest.Width);
            float height = (float)texture.Height * MathF.Sign(dest.Height);

            bool flipX = false;

            if (source.Width < 0) { flipX = true; source.Width *= -1; }
            if (source.Height < 0) source.Y -= source.Height;

            if (dest.Width < 0) dest.Width *= -1;
            if (dest.Height < 0) dest.Height *= -1;

            Vector2 topLeft = Vector2.Zero;
            Vector2 topRight = Vector2.Zero;
            Vector2 bottomLeft = Vector2.Zero;
            Vector2 bottomRight = Vector2.Zero;

            // Only calculate rotation if needed
            if (rotation == 0.0f)
            {
                float x = dest.X - origin.X;
                float y = dest.Y - origin.Y;
                topLeft = new Vector2(x, y);
                topRight = new Vector2(x + dest.Width, y);
                bottomLeft = new Vector2(x, y + dest.Height);
                bottomRight = new Vector2(x + dest.Width, y + dest.Height);
            }
            else
            {
                float sinRotation = MathF.Sin(rotation * DEG2RAD);
                float cosRotation = MathF.Cos(rotation * DEG2RAD);
                float x = dest.X;
                float y = dest.Y;
                float dx = -origin.X;
                float dy = -origin.Y;

                topLeft.X = x + dx*cosRotation - dy*sinRotation;
                topLeft.Y = y + dx*sinRotation + dy*cosRotation;

                topRight.X = x + (dx + dest.Width)*cosRotation - dy*sinRotation;
                topRight.Y = y + (dx + dest.Width)*sinRotation + dy*cosRotation;

                bottomLeft.X = x + dx*cosRotation - (dy + dest.Height)*sinRotation;
                bottomLeft.Y = y + dx*sinRotation + (dy + dest.Height)*cosRotation;

                bottomRight.X = x + (dx + dest.Width)*cosRotation - (dy + dest.Height)*sinRotation;
                bottomRight.Y = y + (dx + dest.Width)*sinRotation + (dy + dest.Height)*cosRotation;
            }

            Rlgl.SetTexture(texture.Id);
            Rlgl.Begin(DrawMode.Quads);
                Rlgl.Normal3f(0.0f, 0.0f, 1.0f);                          // Normal vector pointing towards viewer
                // Top-left corner for texture and quad
                Rlgl.Color4ub(c1.R, c1.G, c1.B, c1.A);
                if (flipX) Rlgl.TexCoord2f((source.X + source.Width)/width, source.Y/height);
                else Rlgl.TexCoord2f(source.X/width, source.Y/height);
                Rlgl.Vertex2f(topLeft.X, topLeft.Y);
                

                // Bottom-left corner for texture and quad
                Rlgl.Color4ub(c2.R, c2.G, c2.B, c2.A);
                if (flipX) Rlgl.TexCoord2f((source.X + source.Width)/width, (source.Y + source.Height)/height);
                else Rlgl.TexCoord2f(source.X/width, (source.Y + source.Height)/height);
                Rlgl.Vertex2f(bottomLeft.X, bottomLeft.Y);
                

                // Bottom-right corner for texture and quad
                Rlgl.Color4ub(c3.R, c3.G, c3.B, c3.A);
                if (flipX) Rlgl.TexCoord2f(source.X/width, (source.Y + source.Height)/height);
                else Rlgl.TexCoord2f((source.X + source.Width)/width, (source.Y + source.Height)/height);
                Rlgl.Vertex2f(bottomRight.X, bottomRight.Y);
                

                // Top-right corner for texture and quad
                Rlgl.Color4ub(c4.R, c4.G, c4.B, c4.A);
                if (flipX) Rlgl.TexCoord2f(source.X/width, source.Y/height);
                else Rlgl.TexCoord2f((source.X + source.Width)/width, source.Y/height);
                Rlgl.Vertex2f(topRight.X, topRight.Y);
                

            Rlgl.End();
            Rlgl.SetTexture(0);
        }
    }
}
