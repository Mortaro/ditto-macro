using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditto.Commands
{
    class KeyUnless
    {

        public static void Execute(Macro macro, string[] arguments)
        {
            if (arguments.Length >= 5 && macro.Running && macro.Windows.Count > 0)
            {
                int x = Int32.Parse(arguments[1]);
                int y = Int32.Parse(arguments[2]);
                string[] strArray = arguments[3].Split('.');
                int r = int.Parse(strArray[0]);
                int g = int.Parse(strArray[1]);
                int b = int.Parse(strArray[2]);
                string key = arguments[arguments.Length - 1];
                int tolerance = arguments.Length >= 6 ? Int32.Parse(arguments[4]) : 0;
                var current = Pixel.GetPixel(macro.Windows[0], x, y);
                bool matches = Math.Abs(current.R - r) <= tolerance
                    && Math.Abs(current.G - g) <= tolerance
                    && Math.Abs(current.B - b) <= tolerance;
                if (!matches)
                {
                    Keypress.Execute(macro, new string[] { "keypress", key });
                }
            }
        }

    }
}
