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
            if (arguments.Length < 1 || !macro.Running) return;
            
            // Monster name boxes can have text in multiple colors
            var textColors = new List<Color>
            {
                Color.FromArgb(221, 85, 238),  // #DD55EE - Purple
                Color.FromArgb(255, 0, 0),      // #FF0000 - Red
                Color.FromArgb(238, 238, 0),    // #EEEE00 - Yellow
                Color.FromArgb(255, 187, 51)    // #FFBB33 - Orange
            };
            int colorTolerance = 15; // Tolerance for text color detection
            int minTextPixels = 5; // Minimum text pixels to consider it valid
            
            foreach (IntPtr window in macro.Windows)
            {
                // Keep trying until we find a box or the macro stops
                while (macro.Running)
                {
                    Rectangle? box = FindBoxWithColoredText(macro, window, textColors, colorTolerance, minTextPixels);
                    if (box.HasValue)
                    {
                        // Click at the center of the found box
                        int centerX = box.Value.X + box.Value.Width / 2;
                        int centerY = box.Value.Y + box.Value.Height / 2;
                        
                        string[] args = new string[3]
                        {
                            "leftclick",
                            centerX.ToString(),
                            (centerY + 30).ToString(),
                        };
                        Leftclick.Execute(macro, args);
                        //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(centerX, centerY + 30);
                        break; // Exit the retry loop after successful click
                    }
                    
                    // Small delay before retrying to avoid excessive CPU usage
                    System.Threading.Thread.Sleep(50);
                }
            }
        }

        public static Rectangle? FindBoxWithColoredText(
            Macro macro,
            IntPtr hwnd,
            List<Color> textColors,
            int colorTolerance,
            int minTextPixels)
        {
            Rect rectangle = new Rect();
            GetWindowRect(hwnd, ref rectangle);
            int width = rectangle.Right - rectangle.Left;
            int height = rectangle.Bottom - rectangle.Top;
            
            using (Bitmap bmp = GetScreenshot(hwnd, width, height, rectangle.Left, rectangle.Top))
            {
                if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
                    throw new ArgumentException("Bitmap must be Format32bppArgb");

                int w = bmp.Width, h = bmp.Height;
                int cx = w / 2, cy = h / 2;

                var data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    byte[] buffer = CreatePixelBuffer(data, h);
                    
                    // Try to find a box with any of the text colors
                    foreach (var textColor in textColors)
                    {
                        if (!macro.Running) return null; // Check if macro is still running
                        
                        var pixelHelper = new PixelHelper(buffer, data.Stride, textColor, colorTolerance);
                        Rectangle? box = SearchForColoredTextBox(macro, w, h, cx, cy, pixelHelper, minTextPixels);
                        if (box.HasValue)
                            return box;
                    }
                    
                    return null;
                }
                finally
                {
                    bmp.UnlockBits(data);
                }
            }
        }

        private static Rectangle? SearchForColoredTextBox(
            Macro macro,
            int width,
            int height,
            int centerX,
            int centerY,
            PixelHelper pixelHelper,
            int minTextPixels)
        {
            const int UI_TOP_OFFSET = 60; // Ignore top 60px where UI is located
            
            var queue = new Queue<(int x, int y)>();
            var scanned = new bool[width, height];
            var componentProcessed = new bool[width, height];

            // Start BFS from center, but below UI area
            int startX = Math.Min(Math.Max(0, centerX), width - 1);
            int startY = Math.Min(Math.Max(UI_TOP_OFFSET, centerY), height - 1);
            queue.Enqueue((startX, startY));
            scanned[startX, startY] = true;

            while (queue.Count > 0 && macro.Running)
            {
                var (x, y) = queue.Dequeue();

                // Skip pixels in the UI area
                if (y < UI_TOP_OFFSET)
                    continue;

                // If we find a colored text pixel, check if it's part of a valid name box
                if (pixelHelper.IsColorMatch(x, y) && !componentProcessed[x, y])
                {
                    Rectangle? box = ProcessColoredTextRegion(x, y, width, height, pixelHelper, componentProcessed, minTextPixels);
                    if (box.HasValue)
                        return box;
                }

                EnqueueNeighbors(queue, scanned, x, y, width, height);
            }

            return null;
        }

        private static Rectangle? ProcessColoredTextRegion(
            int startX,
            int startY,
            int width,
            int height,
            PixelHelper pixelHelper,
            bool[,] componentProcessed,
            int minTextPixels)
        {
            // Find all colored text pixels in this region (first word)
            var textPixels = FloodFillTextRegion(startX, startY, width, height, pixelHelper, componentProcessed);
            
            if (textPixels.Count < minTextPixels)
                return null;

            // Calculate initial bounding box of first word
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            
            foreach (var (x, y) in textPixels)
            {
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }

            // Look for additional words nearby (within reasonable horizontal distance)
            // This handles multi-word monster names like "demon vulgar monster"
            int searchRadius = 100; // pixels to search horizontally for more words
            int wordGapTolerance = 20; // max vertical difference for words on same line
            
            for (int y = Math.Max(0, minY - wordGapTolerance); y <= Math.Min(height - 1, maxY + wordGapTolerance); y++)
            {
                for (int x = Math.Max(0, minX - searchRadius); x <= Math.Min(width - 1, maxX + searchRadius); x++)
                {
                    if (!componentProcessed[x, y] && pixelHelper.IsColorMatch(x, y))
                    {
                        var additionalPixels = FloodFillTextRegion(x, y, width, height, pixelHelper, componentProcessed);
                        foreach (var (px, py) in additionalPixels)
                        {
                            textPixels.Add((px, py));
                            if (px < minX) minX = px;
                            if (px > maxX) maxX = px;
                            if (py < minY) minY = py;
                            if (py > maxY) maxY = py;
                        }
                    }
                }
            }

            // Expand the box to include the surrounding black background
            // Monster name boxes typically have padding around the text
            int padding = 15;
            minX = Math.Max(0, minX - padding);
            maxX = Math.Min(width - 1, maxX + padding);
            minY = Math.Max(0, minY - padding);
            maxY = Math.Min(height - 1, maxY + padding);

            // Verify the surrounding area is mostly dark (black/dark shades)
            if (IsSurroundingAreaDark(minX, maxX, minY, maxY, pixelHelper))
            {
                int boxWidth = maxX - minX + 1;
                int boxHeight = maxY - minY + 1;
                return new Rectangle(minX, minY, boxWidth, boxHeight);
            }

            return null;
        }

        private static List<(int x, int y)> FloodFillTextRegion(
            int startX,
            int startY,
            int width,
            int height,
            PixelHelper pixelHelper,
            bool[,] componentProcessed)
        {
            var stack = new Stack<(int x, int y)>();
            var textPixels = new List<(int x, int y)>();
            
            stack.Push((startX, startY));
            componentProcessed[startX, startY] = true;

            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();
                textPixels.Add((x, y));

                // Check 8-neighbors for text connectivity
                var neighbors = new[] { 
                    (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1),
                    (x - 1, y - 1), (x + 1, y - 1), (x - 1, y + 1), (x + 1, y + 1)
                };
                
                foreach (var (nx, ny) in neighbors)
                {
                    if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        continue;
                    if (componentProcessed[nx, ny])
                        continue;
                    if (pixelHelper.IsColorMatch(nx, ny))
                    {
                        componentProcessed[nx, ny] = true;
                        stack.Push((nx, ny));
                    }
                }
            }

            return textPixels;
        }

        private static bool IsSurroundingAreaDark(
            int minX,
            int maxX,
            int minY,
            int maxY,
            PixelHelper pixelHelper)
        {
            int totalPixels = 0;
            int darkPixels = 0;
            int darkThreshold = 60; // Pixels with all RGB values below this are considered dark

            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    totalPixels++;
                    var (b, g, r, a) = pixelHelper.GetPixel(x, y);
                    
                    // Check if pixel is dark (low RGB values)
                    if (r < darkThreshold && g < darkThreshold && b < darkThreshold)
                    {
                        darkPixels++;
                    }
                }
            }

            // At least 60% of the box should be dark
            double darkRatio = (double)darkPixels / totalPixels;
            return darkRatio >= 0.6;
        }

        private static byte[] CreatePixelBuffer(BitmapData data, int height)
        {
            int stride = data.Stride;
            IntPtr scan0 = data.Scan0;
            int bytes = Math.Abs(stride) * height;
            byte[] buffer = new byte[bytes];
            Marshal.Copy(scan0, buffer, 0, bytes);
            return buffer;
        }

        private static void EnqueueNeighbors(
            Queue<(int x, int y)> queue, 
            bool[,] scanned, 
            int x, 
            int y, 
            int width, 
            int height)
        {
            var directions = new[] { (x, y - 1), (x + 1, y), (x, y + 1), (x - 1, y) };
            foreach (var (nx, ny) in directions)
            {
                if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                    continue;
                if (scanned[nx, ny])
                    continue;
                    
                scanned[nx, ny] = true;
                queue.Enqueue((nx, ny));
            }
        }

        private class PixelHelper
        {
            private readonly byte[] buffer;
            private readonly int stride;
            private readonly Color targetColor;
            private readonly int tolerance;

            public PixelHelper(byte[] buffer, int stride, Color targetColor, int tolerance)
            {
                this.buffer = buffer;
                this.stride = stride;
                this.targetColor = targetColor;
                this.tolerance = tolerance;
            }

            public (byte b, byte g, byte r, byte a) GetPixel(int x, int y)
            {
                int idx = y * stride + x * 4;
                return (buffer[idx], buffer[idx + 1], buffer[idx + 2], buffer[idx + 3]);
            }

            public int GetPixelArgb(int x, int y)
            {
                var (b, g, r, a) = GetPixel(x, y);
                return (a << 24) | (r << 16) | (g << 8) | b;
            }

            public bool IsColorMatch(int x, int y)
            {
                var (b, g, r, a) = GetPixel(x, y);
                return ChannelClose(r, targetColor.R, tolerance)
                    && ChannelClose(g, targetColor.G, tolerance)
                    && ChannelClose(b, targetColor.B, tolerance);
            }

            private bool ChannelClose(int v1, int v2, int tol) => Math.Abs(v1 - v2) <= tol;
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
