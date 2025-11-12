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
    class ClickMonster
    {
        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_MENU = 0x12; // Alt key

        public static void Execute(Macro macro, string[] arguments)
        {
            if (arguments.Length < 2 || !macro.Running) return;
            
            // Determine click type from argument (left or right)
            string clickType = arguments[1].ToLower();
            if (clickType != "left" && clickType != "right") return;
            
            // Monster name boxes can have text in multiple colors
            var textColors = new List<Color>
            {
                Color.FromArgb(221, 85, 238),  // #DD55EE - Purple
                Color.FromArgb(255, 0, 0),      // #FF0000 - Red
                Color.FromArgb(238, 238, 0),    // #EEEE00 - Yellow
                Color.FromArgb(255, 187, 51)    // #FFBB33 - Orange
            };
            
            foreach (IntPtr window in macro.Windows)
            {
                // Keep trying until we find a pixel or the macro stops
                while (macro.Running)
                {
                    Point? clickPoint = FindClosestColoredPixelRow(window, textColors);
                    
                    if (clickPoint.HasValue)
                    {
                        int clickX = clickPoint.Value.X;
                        int clickY = clickPoint.Value.Y + 30;
                        
                        // Move cursor to the position for debugging
                        //System.Windows.Forms.Cursor.Position = new System.Drawing.Point(clickX, clickY);
                        System.Threading.Thread.Sleep(10); // Small delay to see cursor position
                        
                        string[] args = new string[3]
                        {
                            clickType == "left" ? "leftclick" : "rightclick",
                            clickX.ToString(),
                            clickY.ToString(),
                        };
                        
                        if (clickType == "left")
                        {
                            Leftclick.Execute(macro, args);
                        }
                        else
                        {
                            Rightclick.Execute(macro, args);
                        }
                        
                        break; // Exit the retry loop after successful click
                    }
                    
                    // Small delay before retrying to avoid excessive CPU usage
                    System.Threading.Thread.Sleep(50);
                }
            }
        }

        public static Point? FindClosestColoredPixelRow(
            IntPtr hwnd,
            List<Color> textColors)
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
                const int UI_TOP_OFFSET = 60; // Ignore top 60px where UI is located
                const int MAX_GAP = 50; // Maximum gap between pixels before stopping

                var data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                try
                {
                    byte[] buffer = CreatePixelBuffer(data, h);
                    
                    // Find closest colored pixel to center (using spiral/BFS search)
                    var queue = new Queue<(int x, int y)>();
                    var visited = new bool[w, h];
                    
                    queue.Enqueue((cx, Math.Max(cy, UI_TOP_OFFSET)));
                    visited[cx, Math.Max(cy, UI_TOP_OFFSET)] = true;
                    
                    while (queue.Count > 0)
                    {
                        var (x, y) = queue.Dequeue();
                        
                        if (y < UI_TOP_OFFSET)
                            continue;
                        
                        // Check if this pixel matches any of the target colors
                        Color? matchedColor = null;
                        foreach (var targetColor in textColors)
                        {
                            if (IsColorMatch(buffer, data.Stride, x, y, targetColor))
                            {
                                matchedColor = targetColor;
                                break;
                            }
                        }
                        
                        if (matchedColor.HasValue)
                        {
                            // Found a colored pixel! Now find all pixels in the same row
                            List<int> rowPixelXs = new List<int>();
                            rowPixelXs.Add(x);
                            
                            // Search left from found pixel
                            int lastX = x;
                            for (int checkX = x - 1; checkX >= 0; checkX--)
                            {
                                if (IsColorMatch(buffer, data.Stride, checkX, y, matchedColor.Value))
                                {
                                    rowPixelXs.Add(checkX);
                                    lastX = checkX;
                                }
                                else if (checkX < lastX - MAX_GAP)
                                {
                                    // Gap is too large, stop searching left
                                    break;
                                }
                            }
                            
                            // Search right from found pixel
                            lastX = x;
                            for (int checkX = x + 1; checkX < w; checkX++)
                            {
                                if (IsColorMatch(buffer, data.Stride, checkX, y, matchedColor.Value))
                                {
                                    rowPixelXs.Add(checkX);
                                    lastX = checkX;
                                }
                                else if (checkX > lastX + MAX_GAP)
                                {
                                    // Gap is too large, stop searching right
                                    break;
                                }
                            }
                            
                            // Find center of all found pixels in this row
                            int minX = rowPixelXs.Min();
                            int maxX = rowPixelXs.Max();
                            int centerX = (minX + maxX) / 2;
                            
                            return new Point(centerX, y);
                        }
                        
                        // Enqueue neighbors in spiral pattern
                        var neighbors = new[] { (x, y - 1), (x + 1, y), (x, y + 1), (x - 1, y) };
                        foreach (var (nx, ny) in neighbors)
                        {
                            if (nx >= 0 && nx < w && ny >= 0 && ny < h && !visited[nx, ny])
                            {
                                visited[nx, ny] = true;
                                queue.Enqueue((nx, ny));
                            }
                        }
                    }
                    
                    return null; // No colored pixel found
                }
                finally
                {
                    bmp.UnlockBits(data);
                }
            }
        }

        private static bool IsColorMatch(byte[] buffer, int stride, int x, int y, Color targetColor)
        {
            int idx = y * stride + x * 4;
            byte b = buffer[idx];
            byte g = buffer[idx + 1];
            byte r = buffer[idx + 2];
            
            return r == targetColor.R && g == targetColor.G && b == targetColor.B;
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
