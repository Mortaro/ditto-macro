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
            
            // Look for boxes containing purple text #DD55EE (RGB: 221, 85, 238)
            Color purpleTextColor = Color.FromArgb(221, 85, 238);
            int purpleTolerance = 15; // Tolerance for purple text detection
            int minPurplePixels = 5; // Minimum purple pixels to consider it valid text
            
            foreach (IntPtr window in macro.Windows)
            {
                Rectangle? box = FindBoxWithPurpleText(window, purpleTextColor, purpleTolerance, minPurplePixels);
                if (box.HasValue)
                {
                    // Click at the center of the found box
                    int centerX = box.Value.X + box.Value.Width / 2;
                    int centerY = box.Value.Y + box.Value.Height / 2;
                    
                    string[] args = new string[3]
                    {
                        "leftclick",
                        centerX.ToString(),
                        centerY.ToString(),
                    };
                    Leftclick.Execute(macro, args);
                    System.Windows.Forms.Cursor.Position = new System.Drawing.Point(centerX, centerY);
                }
            }
        }

        public static Rectangle? FindBoxWithPurpleText(
            IntPtr hwnd,
            Color purpleTextColor,
            int purpleTolerance,
            int minPurplePixels)
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
                    var pixelHelper = new PixelHelper(buffer, data.Stride, purpleTextColor, purpleTolerance);
                    
                    return SearchForPurpleTextBox(w, h, cx, cy, pixelHelper, minPurplePixels);
                }
                finally
                {
                    bmp.UnlockBits(data);
                }
            }
        }

        private static Rectangle? SearchForPurpleTextBox(
            int width,
            int height,
            int centerX,
            int centerY,
            PixelHelper pixelHelper,
            int minPurplePixels)
        {
            var queue = new Queue<(int x, int y)>();
            var scanned = new bool[width, height];
            var componentProcessed = new bool[width, height];

            // Start BFS from center
            int startX = Math.Min(Math.Max(0, centerX), width - 1);
            int startY = Math.Min(Math.Max(0, centerY), height - 1);
            queue.Enqueue((startX, startY));
            scanned[startX, startY] = true;

            while (queue.Count > 0)
            {
                var (x, y) = queue.Dequeue();

                // If we find a purple pixel, check if it's part of a valid name box
                if (pixelHelper.IsColorMatch(x, y) && !componentProcessed[x, y])
                {
                    Rectangle? box = ProcessPurpleTextRegion(x, y, width, height, pixelHelper, componentProcessed, minPurplePixels);
                    if (box.HasValue)
                        return box;
                }

                EnqueueNeighbors(queue, scanned, x, y, width, height);
            }

            return null;
        }

        private static Rectangle? ProcessPurpleTextRegion(
            int startX,
            int startY,
            int width,
            int height,
            PixelHelper pixelHelper,
            bool[,] componentProcessed,
            int minPurplePixels)
        {
            // Find all purple pixels in this region
            var purplePixels = FloodFillPurpleRegion(startX, startY, width, height, pixelHelper, componentProcessed);
            
            if (purplePixels.Count < minPurplePixels)
                return null;

            // Calculate bounding box of purple text
            int minX = int.MaxValue, maxX = int.MinValue;
            int minY = int.MaxValue, maxY = int.MinValue;
            
            foreach (var (x, y) in purplePixels)
            {
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
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

        private static List<(int x, int y)> FloodFillPurpleRegion(
            int startX,
            int startY,
            int width,
            int height,
            PixelHelper pixelHelper,
            bool[,] componentProcessed)
        {
            var stack = new Stack<(int x, int y)>();
            var purplePixels = new List<(int x, int y)>();
            
            stack.Push((startX, startY));
            componentProcessed[startX, startY] = true;

            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();
                purplePixels.Add((x, y));

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

            return purplePixels;
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
