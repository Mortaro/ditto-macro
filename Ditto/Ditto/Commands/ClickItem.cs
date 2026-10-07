using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Ditto.Commands
{
    class ClickItem
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref ClickMonster.Rect rectangle);

        private const int DEFAULT_RADIUS = 220;
        private const int DEFAULT_TOLERANCE = 30;
        private const int BLOB_MINIMUM = 6;
        private const int BLOB_MAXIMUM = 400;

        private static readonly Color[] DEFAULT_COLORS =
        {
            Color.FromArgb(255, 255, 222),
            Color.FromArgb(230, 207, 145)
        };

        public static void Execute(Macro macro, string[] arguments)
        {
            if (!macro.Running)
            {
                return;
            }

            int radius = DEFAULT_RADIUS;
            int tolerance = DEFAULT_TOLERANCE;
            bool radiusRead = false;
            var colors = new List<Color>();
            bool debug = false;

            for (int i = 1; i < arguments.Length; i++)
            {
                string arg = arguments[i];
                if (arg == "debug")
                {
                    debug = true;
                }
                else if (arg.Contains("."))
                {
                    string[] channels = arg.Split('.');
                    if (channels.Length == 3)
                    {
                        colors.Add(Color.FromArgb(int.Parse(channels[0]), int.Parse(channels[1]), int.Parse(channels[2])));
                    }
                }
                else if (int.TryParse(arg, out int value))
                {
                    if (!radiusRead) { radius = value; radiusRead = true; }
                    else { tolerance = value; }
                }
            }

            if (colors.Count == 0)
            {
                colors.AddRange(DEFAULT_COLORS);
            }

            foreach (IntPtr window in macro.Windows)
            {
                var rect = new ClickMonster.Rect();
                GetWindowRect(window, ref rect);
                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;
                if (width <= 0 || height <= 0)
                {
                    continue;
                }

                int cx = width / 2;
                int cy = height / 2;

                using (Bitmap screen = ClickMonster.GetScreenshot(window, width, height, rect.Left, rect.Top))
                {
                    var target = Find(screen, cx, cy, radius, tolerance, colors, debug);
                    if (target.HasValue)
                    {
                        string[] click = { "leftclick", target.Value.X.ToString(), target.Value.Y.ToString() };
                        Leftclick.Execute(macro, click);
                    }
                }
            }
        }

        private static Point? Find(Bitmap screen, int cx, int cy, int radius, int tolerance, List<Color> colors, bool debug)
        {
            int x0 = Math.Max(0, cx - radius);
            int y0 = Math.Max(0, cy - radius);
            int x1 = Math.Min(screen.Width - 1, cx + radius);
            int y1 = Math.Min(screen.Height - 1, cy + radius);
            if (x1 <= x0 || y1 <= y0)
            {
                return null;
            }

            var area = new Rectangle(x0, y0, x1 - x0, y1 - y0);
            BitmapData data = screen.LockBits(area, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            int stride = data.Stride;
            var buffer = new byte[Math.Abs(stride) * area.Height];
            Marshal.Copy(data.Scan0, buffer, 0, buffer.Length);
            screen.UnlockBits(data);

            var seen = new bool[area.Width, area.Height];
            Point? best = null;
            double bestDistance = double.MaxValue;
            int candidates = 0;

            for (int y = 0; y < area.Height; y++)
            {
                for (int x = 0; x < area.Width; x++)
                {
                    if (seen[x, y] || !Matches(buffer, stride, x, y, colors, tolerance))
                    {
                        continue;
                    }

                    int size = 0;
                    long sumX = 0, sumY = 0;
                    var queue = new Queue<Point>();
                    queue.Enqueue(new Point(x, y));
                    seen[x, y] = true;

                    while (queue.Count > 0)
                    {
                        Point p = queue.Dequeue();
                        size++;
                        sumX += p.X;
                        sumY += p.Y;
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            for (int dx = -1; dx <= 1; dx++)
                            {
                                int nx = p.X + dx, ny = p.Y + dy;
                                if (nx < 0 || ny < 0 || nx >= area.Width || ny >= area.Height || seen[nx, ny])
                                {
                                    continue;
                                }
                                if (Matches(buffer, stride, nx, ny, colors, tolerance))
                                {
                                    seen[nx, ny] = true;
                                    queue.Enqueue(new Point(nx, ny));
                                }
                            }
                        }
                    }

                    if (size < BLOB_MINIMUM || size > BLOB_MAXIMUM)
                    {
                        continue;
                    }

                    candidates++;
                    int screenX = area.X + (int)(sumX / size);
                    int screenY = area.Y + (int)(sumY / size);
                    double dist = Math.Sqrt(Math.Pow(screenX - cx, 2) + Math.Pow(screenY - cy, 2));
                    if (dist > radius)
                    {
                        continue;
                    }
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        best = new Point(screenX, screenY);
                    }
                }
            }

            if (debug)
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\ditto-clickitem.txt";
                string line = DateTime.Now.ToString("HH:mm:ss") + "  candidates=" + candidates
                    + "  chosen=" + (best.HasValue ? best.Value.X + "," + best.Value.Y + " dist=" + (int)bestDistance : "none")
                    + Environment.NewLine;
                try
                {
                    System.IO.File.AppendAllText(path, line);
                }
                catch (Exception failure) when (failure is System.IO.IOException || failure is UnauthorizedAccessException)
                {
                }
            }

            return best;
        }

        private static bool Matches(byte[] buffer, int stride, int x, int y, List<Color> colors, int tolerance)
        {
            int idx = y * stride + x * 4;
            if (idx + 2 >= buffer.Length)
            {
                return false;
            }
            byte b = buffer[idx];
            byte g = buffer[idx + 1];
            byte r = buffer[idx + 2];
            foreach (Color color in colors)
            {
                if (Math.Abs(r - color.R) <= tolerance
                    && Math.Abs(g - color.G) <= tolerance
                    && Math.Abs(b - color.B) <= tolerance)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
