using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditto.Commands
{
    class Rightclick
    {

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public static void Execute(Macro macro, string[] arguments)
        {
            if (arguments.Length == 3 && macro.Running)
            {
                int x = Int32.Parse(arguments[1]);
                int y = Int32.Parse(arguments[2]);
                int lParam = x | y << 16;
                SendMessage(macro.Window, 516, 0, lParam);
                SendMessage(macro.Window, 517, 0, lParam);
            }
        }

    }
}
