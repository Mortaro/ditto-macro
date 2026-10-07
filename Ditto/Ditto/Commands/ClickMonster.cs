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
        private static extern bool GetClientRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, uint nFlags);

        private const uint PW_RENDERFULLCONTENT = 0x00000002;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int VK_MENU = 0x12; // Alt key

        public static void Execute(Macro macro, string[] arguments)
        {
            if (arguments.Length < 2 || !macro.Running) return;

            // Determine click type from argument (left or right)
            string clickType = arguments[1].ToLower();
            if (clickType != "left" && clickType != "right") return;

            int radius = 0;
            int timeout = 0;
            int tolerance = 0;
            bool radiusSet = false;
            bool debug = false;
            bool background = false;
            var customColors = new List<Color>();
            for (int i = 2; i < arguments.Length; i++)
            {
                string argument = arguments[i];
                if (argument == "debug")
                {
                    debug = true;
                }
                else if (argument == "bg")
                {
                    background = true;
                }
                else if (argument.Contains('.'))
                {
                    string[] channels = argument.Split('.');
                    if (channels.Length == 3)
                    {
                        customColors.Add(Color.FromArgb(
                            int.Parse(channels[0]),
                            int.Parse(channels[1]),
                            int.Parse(channels[2])
                        ));
                    }
                }
                else if (Int32.TryParse(argument, out int value))
                {
                    if (!radiusSet)
                    {
                        radius = value;
                        radiusSet = true;
                    }
                    else
                    {
                        tolerance = value;
                    }
                }
                else
                {
                    timeout = Wait.GetMiliseconds(argument);
                }
            }

            // Monster name boxes can have text in multiple colors
            var textColors = customColors.Count > 0 ? customColors : new List<Color>
            {
                Color.FromArgb(221, 85, 238),   // #DD55EE - Purple (much higher)
                Color.FromArgb(255, 0, 0),      // #FF0000 - Red (higher)
                Color.FromArgb(255, 187, 51),   // #FFBB33 - Orange (a bit higher)
                Color.FromArgb(238, 238, 0)     // #EEEE00 - Yellow (slightly higher)
            };

            var timer = System.Diagnostics.Stopwatch.StartNew();
            foreach (IntPtr window in macro.Windows)
            {
                // Keep trying until we find a pixel or the macro stops
                while (macro.Running)
                {
                    if (timeout > 0 && timer.ElapsedMilliseconds > timeout) return;

                    List<string> debugLog = debug ? new List<string>() : null;
                    Point? clickPoint = FindClosestColoredPixelRow(window, textColors, radius, tolerance, debugLog, background);
                    if (debugLog != null)
                    {
                        WriteDebugReport(debugLog, clickPoint);
                        debug = false;
                    }
                    
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
            List<Color> textColors,
            int radius = 0,
            int tolerance = 0,
            List<string> debugLog = null,
            bool background = false)
        {
            Rect rectangle = new Rect();
            GetWindowRect(hwnd, ref rectangle);
            int width = rectangle.Right - rectangle.Left;
            int height = rectangle.Bottom - rectangle.Top;
            
            using (Bitmap bmp = GetScreenshot(hwnd, width, height, rectangle.Left, rectangle.Top, background))
            {
                if (bmp.PixelFormat != PixelFormat.Format32bppArgb)
                    throw new ArgumentException("Bitmap must be Format32bppArgb");

                int w = bmp.Width, h = bmp.Height;
                int cx = w / 2, cy = h / 2;
                const int UI_TOP_OFFSET = 60; // Ignore top 60px where UI is located
                const int MAX_GAP = 50; // Maximum gap between pixels before stopping
                const int MIN_NAME_WIDTH = 25;
                const int MAX_NAME_HEIGHT = 18;
                const int MIN_NAME_HEIGHT = 6;
                const int MAX_BRIGHT_GAP_PERCENT = 40;

                if (debugLog != null)
                {
                    try
                    {
                        string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                        bmp.Save(System.IO.Path.Combine(desktop, "ditto-debug.png"));
                    }
                    catch { }
                    var colorNames = new List<string>();
                    foreach (var c in textColors) colorNames.Add(c.R + "." + c.G + "." + c.B);
                    debugLog.Add("window " + w + "x" + h + " center " + cx + "," + cy + " radius " + radius + " tolerance " + tolerance + " colors " + string.Join(" ", colorNames));
                }

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
                            if (IsColorMatch(buffer, data.Stride, x, y, targetColor, tolerance))
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
                                if (IsColorMatch(buffer, data.Stride, checkX, y, matchedColor.Value, tolerance))
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
                                if (IsColorMatch(buffer, data.Stride, checkX, y, matchedColor.Value, tolerance))
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

                            int rowWidth = maxX - minX;

                            int glyphTop = y;
                            while (glyphTop - 1 >= 0 && IsColorMatch(buffer, data.Stride, x, glyphTop - 1, matchedColor.Value, tolerance)) glyphTop--;
                            int glyphBottom = y;
                            while (glyphBottom + 1 < h && IsColorMatch(buffer, data.Stride, x, glyphBottom + 1, matchedColor.Value, tolerance)) glyphBottom++;
                            int textHeight = glyphBottom - glyphTop + 1;

                            int gapPixels = 0, brightGapPixels = 0;
                            for (int checkX = minX; checkX <= maxX; checkX++)
                            {
                                if (IsColorMatch(buffer, data.Stride, checkX, y, matchedColor.Value, tolerance)) continue;
                                if (IsColorMatch(buffer, data.Stride, checkX, y, matchedColor.Value, tolerance * 2)) continue;
                                gapPixels++;
                                if (!IsDarkPixel(buffer, data.Stride, checkX, y)) brightGapPixels++;
                            }
                            int brightPercent = gapPixels == 0 ? 0 : (brightGapPixels * 100 / gapPixels);
                            bool onDarkBackdrop = brightPercent <= MAX_BRIGHT_GAP_PERCENT;

                            bool isPlayerNameplate = HasNearbyTextRow(buffer, data.Stride, w, h, minX, maxX, glyphTop, glyphBottom, matchedColor.Value, tolerance);

                            bool accepted = rowWidth >= MIN_NAME_WIDTH && textHeight >= MIN_NAME_HEIGHT && textHeight <= MAX_NAME_HEIGHT && onDarkBackdrop && !isPlayerNameplate;

                            if (debugLog != null && debugLog.Count < 60)
                            {
                                debugLog.Add("(" + x + "," + y + ") color " + matchedColor.Value.R + "." + matchedColor.Value.G + "." + matchedColor.Value.B
                                    + " width " + rowWidth + " height " + textHeight + " brightGap " + brightPercent + "% twoLines " + isPlayerNameplate
                                    + (accepted ? " => ACCEPTED" : " => rejected"));
                            }

                            if (accepted)
                            {
                                return new Point(centerX, y);
                            }

                            for (int vy = Math.Max(0, glyphTop); vy <= Math.Min(h - 1, glyphBottom); vy++)
                            {
                                for (int vx = Math.Max(0, minX); vx <= Math.Min(w - 1, maxX); vx++)
                                {
                                    visited[vx, vy] = true;
                                }
                            }
                        }
                        
                        // Enqueue neighbors in spiral pattern
                        var neighbors = new[] { (x, y - 1), (x + 1, y), (x, y + 1), (x - 1, y) };
                        foreach (var (nx, ny) in neighbors)
                        {
                            if (nx >= 0 && nx < w && ny >= 0 && ny < h && !visited[nx, ny])
                            {
                                int dx = nx - cx;
                                int dy = ny - cy;
                                if (radius > 0 && (dx * dx) + (dy * dy) > radius * radius) continue;
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

        private static void WriteDebugReport(List<string> log, Point? result)
        {
            try
            {
                log.Add(result.HasValue ? "RESULT: click at " + result.Value.X + "," + result.Value.Y : "RESULT: no candidate accepted");
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                System.IO.File.WriteAllLines(System.IO.Path.Combine(desktop, "ditto-debug.txt"), log);
            }
            catch { }
        }

        private static bool HasNearbyTextRow(byte[] buffer, int stride, int w, int h, int minX, int maxX, int glyphTop, int glyphBottom, Color color, int tolerance)
        {
            const int SEARCH_DISTANCE = 24;
            const int MIN_COMPANION_MATCHES = 5;
            for (int checkY = Math.Max(0, glyphTop - SEARCH_DISTANCE); checkY <= glyphTop - 4; checkY++)
            {
                if (CountRowMatches(buffer, stride, w, minX, maxX, checkY, color, tolerance) >= MIN_COMPANION_MATCHES) return true;
            }
            for (int checkY = glyphBottom + 4; checkY <= Math.Min(h - 1, glyphBottom + SEARCH_DISTANCE); checkY++)
            {
                if (CountRowMatches(buffer, stride, w, minX, maxX, checkY, color, tolerance) >= MIN_COMPANION_MATCHES) return true;
            }
            return false;
        }

        private static int CountRowMatches(byte[] buffer, int stride, int w, int minX, int maxX, int y, Color color, int tolerance)
        {
            int matches = 0;
            for (int x = Math.Max(0, minX); x <= Math.Min(w - 1, maxX); x++)
            {
                if (IsColorMatch(buffer, stride, x, y, color, tolerance)) matches++;
            }
            return matches;
        }

        private static bool IsDarkPixel(byte[] buffer, int stride, int x, int y)
        {
            int idx = y * stride + x * 4;
            int brightness = (buffer[idx] + buffer[idx + 1] + buffer[idx + 2]) / 3;
            return brightness < 160;
        }

        private static bool IsColorMatch(byte[] buffer, int stride, int x, int y, Color targetColor, int tolerance = 0)
        {
            int idx = y * stride + x * 4;
            byte b = buffer[idx];
            byte g = buffer[idx + 1];
            byte r = buffer[idx + 2];

            return Math.Abs(r - targetColor.R) <= tolerance
                && Math.Abs(g - targetColor.G) <= tolerance
                && Math.Abs(b - targetColor.B) <= tolerance;
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

        public static Bitmap GetScreenshot(IntPtr window, int width, int height, int left, int top, bool background = false)
        {
            if (width <= 0 || height <= 0)
            {
                return new Bitmap(1, 1, PixelFormat.Format32bppArgb);
            }
            Rect client = new Rect();
            if (GetClientRect(window, ref client) && client.Right > 0 && client.Bottom > 0)
            {
                width = client.Right;
                height = client.Bottom;
            }

            Bitmap screenshot = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage((Image) screenshot))
            {
                bool captured = false;
                if (background)
                {
                    IntPtr hdc = graphics.GetHdc();
                    captured = PrintWindow(window, hdc, PW_RENDERFULLCONTENT);
                    graphics.ReleaseHdc(hdc);
                }
                if (!captured)
                {
                    try
                    {
                        using (Graphics source = Graphics.FromHwnd(window))
                        {
                            IntPtr hsrc = source.GetHdc();
                            IntPtr hdst = graphics.GetHdc();
                            captured = BitBlt(hdst, 0, 0, width, height, hsrc, 0, 0, (int) CopyPixelOperation.SourceCopy) != 0;
                            graphics.ReleaseHdc();
                            source.ReleaseHdc();
                        }
                    }
                    catch (Exception)
                    {
                        captured = false;
                    }
                }
                if (!captured)
                {
                    graphics.CopyFromScreen(left, top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
                }
            }
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
