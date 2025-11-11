using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

namespace Ditto.Commands
{
    class LeftClickMonster
    {
        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static void Execute(Macro macro, string[] arguments)
        {
            if (arguments.Length != 2 || !macro.Running) return;
            int r, g, b;
            if (arguments[1] == "purple") 
            {
                r = 221;
                g = 85;
                b = 238;
            } 
            else
            {
                r = 0;
                g = 0;
                b = 0;
            }
            foreach (IntPtr window in macro.Windows)
            {
                Coord coord = FindCoordinateForColor(macro, window, r, g, b);
                if (coord.X > 0) {
                    string[] args = new string[3]
                    {
                        "leftclick",
                        (coord.X + 30).ToString(),
                        (coord.Y + 30).ToString(),
                    };
                    Rightclick.Execute(macro, args);
                }
            }
        }

        public static Coord FindCoordinateForColor(Macro macro, IntPtr window, int r, int g, int b)
        { 
            Rect rectangle = new Rect();
            Coord coord = new Coord();
            GetWindowRect(window, ref rectangle);
            int width = rectangle.Right - rectangle.Left;
            int height = rectangle.Bottom - rectangle.Top;
            Bitmap screenshot = GetScreenshot(window, width, height, rectangle.Left, rectangle.Top);
            while (macro.Running)
            {
                for (int y = 200; y < height; y += 10)
                {
                    int x = 0;
                    while (x < width && macro.Running)
                    {
                        if (screenshot.GetPixel(x, y).R == r && screenshot.GetPixel(x, y).G == g && screenshot.GetPixel(x, y).B == b)
                        {
                            coord.X = x;
                            coord.Y = y;
                            return coord;
                        }
                        else {
                            ++x;
                        }
                    }
                }
            }
            return coord;
        }

        public static Bitmap GetScreenshot(IntPtr window, int width, int height, int left, int top)
        {
            if (width <= 0 || height <= 0) 
            {
                return new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            }
            Bitmap screenshot = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage((Image) screenshot))
            graphics.CopyFromScreen(left, top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            return screenshot;
        }

        public struct Rect
        {
            public int Left { get; set; }

            public int Top { get; set; }

            public int Right { get; set; }

            public int Bottom { get; set; }
        }

        public struct Coord
        {
            public int X { get; set; }
            public int Y { get; set; }
        }
    }
}
