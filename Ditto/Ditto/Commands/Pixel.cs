using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Ditto.Commands
{
    class Pixel
    {

        [System.Runtime.InteropServices.DllImportAttribute("gdi32.dll")]
        private static extern int BitBlt(
          IntPtr hdcDest,     // handle to destination DC (device context)
          int nXDest,         // x-coord of destination upper-left corner
          int nYDest,         // y-coord of destination upper-left corner
          int nWidth,         // width of destination rectangle
          int nHeight,        // height of destination rectangle
          IntPtr hdcSrc,      // handle to source DC
          int nXSrc,          // x-coordinate of source upper-left corner
          int nYSrc,          // y-coordinate of source upper-left corner
          System.Int32 dwRop  // raster operation code
        );

        public static void Execute(Macro macro, string[] arguments)
        {
            if (arguments.Length == 3 && macro.Running)
            { 
                int x = Int32.Parse(arguments[1]);
                int y = Int32.Parse(arguments[2]);
                foreach (IntPtr window in macro.Windows)
                {
                    var original = GetPixel(window, x, y);
                    bool changed = false;
                    while (!changed && macro.Running)
                    {
                        string[] args = { "wait", "1s" };
                        Ditto.Commands.Wait.Execute(macro, args);
                        var current = GetPixel(window, x, y);
                        changed = current.R != original.R || current.G != original.G || current.B != original.B;
                    }
                }
            }
        }

        public static Color GetPixel(IntPtr window, int x, int y)
        {
            using (Bitmap screenPixel = new Bitmap(1, 1))
            {
                using (Graphics gdest = Graphics.FromImage(screenPixel))
                {
                    using (Graphics gsrc = Graphics.FromHwnd(window))
                    {
                        IntPtr hsrcdc = gsrc.GetHdc();
                        IntPtr hdc = gdest.GetHdc();
                        BitBlt(hdc, 0, 0, 1, 1, hsrcdc, x, y, (int)CopyPixelOperation.SourceCopy);
                        gdest.ReleaseHdc();
                        gsrc.ReleaseHdc();
                    }
                }
                return screenPixel.GetPixel(0, 0);
            }
        }

    }
}
