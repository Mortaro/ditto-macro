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
            if (arguments.Length == 4 && macro.Running)
            { 
                int x = Int32.Parse(arguments[1]);
                int y = Int32.Parse(arguments[2]);
                string[] strArray = arguments[3].Split('.');
                int r = int.Parse(strArray[0]);
                int g = int.Parse(strArray[1]);
                int b = int.Parse(strArray[2]);
                foreach (IntPtr window in macro.Windows)
                {
                    bool changed = false;
                    while (!changed && macro.Running)
                    {
                        string[] args = { "wait", "1s" };
                        Ditto.Commands.Wait.Execute(macro, args);
                        var current = GetPixel(window, x, y);
                        changed = (int)current.R != r || (int)current.G != g || (int)current.B != b;
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
                    try {
                        using (Graphics gsrc = Graphics.FromHwnd(window))
                        {
                            IntPtr hsrcdc = gsrc.GetHdc();
                            IntPtr hdc = gdest.GetHdc();
                            BitBlt(hdc, 0, 0, 1, 1, hsrcdc, x, y, (int)CopyPixelOperation.SourceCopy);
                            gdest.ReleaseHdc();
                            gsrc.ReleaseHdc();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                return screenPixel.GetPixel(0, 0);
            }
        }

    }
}
