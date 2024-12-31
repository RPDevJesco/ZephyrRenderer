namespace ZephyrRenderer.UI
{
    public static class FontRenderer
    {
        private static readonly Dictionary<char, bool[,]> CharacterBitmaps = new()
        {
            ['A'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, true, true, true, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true }
            },
            ['B'] = new bool[,] {
                { true, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, true, true, true, false }
            },
            ['C'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            ['D'] = new bool[,] {
                { true, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, true, true, true, false }
            },
            ['E'] = new bool[,] {
                { true, true, true, true, true },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, true, true, true, false },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, true, true, true, true }
            },
            ['F'] = new bool[,] {
                { true, true, true, true, true },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, true, true, true, false },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, false, false, false, false }
            },
            ['G'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, false },
                { true, false, true, true, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            ['H'] = new bool[,] {
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, true, true, true, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true }
            },
            ['I'] = new bool[,] {
                { false, true, true, true, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, true, true, true, false }
            },
            ['L'] = new bool[,] {
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, true, true, true, true }
            },
            ['M'] = new bool[,] {
                { true, false, false, false, true },
                { true, true, false, true, true },
                { true, false, true, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true }
            },
            ['N'] = new bool[,] {
                { true, false, false, false, true },
                { true, true, false, false, true },
                { true, false, true, false, true },
                { true, false, false, true, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true }
            },
            ['O'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            ['P'] = new bool[,] {
                { true, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, true, true, true, false },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, false, false, false, false }
            },
            ['R'] = new bool[,] {
                { true, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, true, true, true, false },
                { true, false, true, false, false },
                { true, false, false, true, false },
                { true, false, false, false, true }
            },
            ['S'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, false },
                { false, true, true, true, false },
                { false, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            ['T'] = new bool[,] {
                { true, true, true, true, true },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false }
            },
            ['U'] = new bool[,] {
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            ['V'] = new bool[,] {
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { false, true, false, true, false },
                { false, false, true, false, false }
            },
            ['W'] = new bool[,] {
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { true, false, true, false, true },
                { true, false, true, false, true },
                { true, true, false, true, true },
                { true, false, false, false, true }
            },
            ['X'] = new bool[,] {
                { true, false, false, false, true },
                { true, false, false, false, true },
                { false, true, false, true, false },
                { false, false, true, false, false },
                { false, true, false, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true }
            },
            ['Y'] = new bool[,] {
                { true, false, false, false, true },
                { true, false, false, false, true },
                { false, true, false, true, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false }
            },
            ['Z'] = new bool[,] {
                { true, true, true, true, true },
                { false, false, false, false, true },
                { false, false, false, true, false },
                { false, false, true, false, false },
                { false, true, false, false, false },
                { true, false, false, false, false },
                { true, true, true, true, true }
            },
            ['0'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, true, true },
                { true, false, true, false, true },
                { true, true, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            ['1'] = new bool[,] {
                { false, false, true, false, false },
                { false, true, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, false, true, false, false },
                { false, true, true, true, false }
            },
            ['2'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, true },
                { false, false, false, false, true },
                { false, false, false, true, false },
                { false, false, true, false, false },
                { false, true, false, false, false },
                { true, true, true, true, true }
            },
            ['3'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, true },
                { false, false, false, false, true },
                { false, false, true, true, false },
                { false, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            ['4'] = new bool[,] {
                { false, false, false, true, false },
                { false, false, true, true, false },
                { false, true, false, true, false },
                { true, false, false, true, false },
                { true, true, true, true, true },
                { false, false, false, true, false },
                { false, false, false, true, false }
            },
            ['5'] = new bool[,] {
                { true, true, true, true, true },
                { true, false, false, false, false },
                { true, true, true, true, false },
                { false, false, false, false, true },
                { false, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            ['6'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, false },
                { true, false, false, false, false },
                { true, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            ['7'] = new bool[,] {
                { true, true, true, true, true },
                { false, false, false, false, true },
                { false, false, false, true, false },
                { false, false, true, false, false },
                { false, true, false, false, false },
                { false, true, false, false, false },
                { false, true, false, false, false }
            },
            ['8'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            ['9'] = new bool[,] {
                { false, true, true, true, false },
                { true, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, true },
                { false, false, false, false, true },
                { true, false, false, false, true },
                { false, true, true, true, false }
            },
            [' '] = new bool[,] {
                { false, false, false, false, false },
                { false, false, false, false, false },
                { false, false, false, false, false },
                { false, false, false, false, false },
                { false, false, false, false, false },
                { false, false, false, false, false },
                { false, false, false, false, false }
            }
        };

        public static void DrawText(Framebuffer framebuffer, string text, int x, int y, Color color)
        {
            int currentX = x;
            foreach (char c in text.ToUpper())
            {
                if (CharacterBitmaps.TryGetValue(c, out bool[,] bitmap))
                {
                    DrawCharacter(framebuffer, bitmap, currentX, y, color);
                }
                currentX += 6; // 5 pixels wide + 1 pixel spacing
            }
        }

        private static void DrawCharacter(Framebuffer framebuffer, bool[,] bitmap, int x, int y, Color color)
        {
            int height = bitmap.GetLength(0);
            int width = bitmap.GetLength(1);

            for (int py = 0; py < height; py++)
            {
                for (int px = 0; px < width; px++)
                {
                    if (bitmap[py, px])
                    {
                        framebuffer.SetPixel(x + px, y + py, color);
                    }
                }
            }
        }

        public static int MeasureText(string text)
        {
            return text.Length * 6; // 5 pixels wide + 1 pixel spacing
        }
    }
}