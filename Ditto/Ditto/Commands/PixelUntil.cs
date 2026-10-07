using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditto.Commands
{
    class PixelUntil
    {

        public static void Execute(Macro macro, string[] arguments)
        {
            if (arguments.Length >= 4 && macro.Running)
            {
                int x = Int32.Parse(arguments[1]);
                int y = Int32.Parse(arguments[2]);
                string[] strArray = arguments[3].Split('.');
                int r = int.Parse(strArray[0]);
                int g = int.Parse(strArray[1]);
                int b = int.Parse(strArray[2]);
                int tolerance = 0;
                int timeout = 0;
                for (int i = 4; i < arguments.Length; i++)
                {
                    if (Int32.TryParse(arguments[i], out int value))
                    {
                        tolerance = value;
                    }
                    else
                    {
                        timeout = Wait.GetMiliseconds(arguments[i]);
                    }
                }
                foreach (IntPtr window in macro.Windows)
                {
                    var timer = Stopwatch.StartNew();
                    bool matched = false;
                    while (!matched && macro.Running)
                    {
                        if (timeout > 0 && timer.ElapsedMilliseconds > timeout) break;
                        var current = Pixel.GetPixel(window, x, y);
                        matched = Math.Abs(current.R - r) <= tolerance
                            && Math.Abs(current.G - g) <= tolerance
                            && Math.Abs(current.B - b) <= tolerance;
                        if (!matched)
                        {
                            string[] args = { "wait", Pixel.POLL_MS.ToString() };
                            Ditto.Commands.Wait.Execute(macro, args);
                        }
                    }
                }
            }
        }

    }
}
