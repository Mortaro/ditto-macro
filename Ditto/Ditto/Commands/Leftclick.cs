using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditto.Commands
{
    class Leftclick
    {

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public static void Execute(Macro macro, string[] arguments)
        {
            System.Diagnostics.Debug.WriteLine("LEFTCLICK!" + arguments.Length.ToString());
            if (arguments.Length == 3 && macro.Running)
            {
                int x = Int32.Parse(arguments[1]);
                int y = Int32.Parse(arguments[2]);
                int lParam = x | y << 16;
                foreach (IntPtr window in macro.Windows)
                {
                    SendMessage(window, 513, 0, lParam);
                    SendMessage(window, 514, 0, lParam);
                }
            }
        }

    }
}
