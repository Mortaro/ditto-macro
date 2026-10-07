using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditto.Commands
{
    class Keypress
    {

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        private const int VK_CONTROL = 0x11;
        private const int VK_MENU = 0x12;
        private const int MODIFIER_POLL_MS = 20;
        private const int MODIFIER_SETTLE_MS = 40;

        private static void WaitPhysicalModifiersReleased(Macro macro)
        {
            bool wasHeld = false;
            while (macro.Running && (
                (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0 ||
                (GetAsyncKeyState(VK_MENU) & 0x8000) != 0
            ))
            {
                wasHeld = true;
                System.Threading.Thread.Sleep(MODIFIER_POLL_MS);
            }
            if (wasHeld)
            {
                System.Threading.Thread.Sleep(MODIFIER_SETTLE_MS);
            }
        }

        public static void Execute(Macro macro, string[] arguments)
        {
            if (arguments.Length == 2 && macro.Running)
            {
                int lParam = 500 | 500 << 16;
                foreach (IntPtr window in macro.Windows)
                {
                    WaitPhysicalModifiersReleased(macro);
                    SendMessage(window, 522, 0, lParam);
                    if (arguments[1] == "1")
                    {
                        SendMessage(window, 256, (IntPtr)49, (IntPtr)257);
                    }
                    else if (arguments[1] == "2")
                    {
                        SendMessage(window, 256, (IntPtr)50, (IntPtr)257);
                    }
                    else if (arguments[1] == "3")
                    {
                        SendMessage(window, 256, (IntPtr)51, (IntPtr)257);
                    }
                    else if (arguments[1] == "4")
                    {
                        SendMessage(window, 256, (IntPtr)52, (IntPtr)257);
                    }
                    else if (arguments[1] == "5")
                    {
                        SendMessage(window, 256, (IntPtr)53, (IntPtr)257);
                    }
                    else if (arguments[1] == "6")
                    {
                        SendMessage(window, 256, (IntPtr)54, (IntPtr)257);
                    }
                    else if (arguments[1] == "7")
                    {
                        SendMessage(window, 256, (IntPtr)55, (IntPtr)257);
                    }
                    else if (arguments[1] == "8")
                    {
                        SendMessage(window, 256, (IntPtr)56, (IntPtr)257);
                    }
                    else if (arguments[1] == "9")
                    {
                        SendMessage(window, 256, (IntPtr)57, (IntPtr)257);
                    }
                    else if (arguments[1] == "0")
                    {
                        SendMessage(window, 256, (IntPtr)48, (IntPtr)257);
                    }
                    else if (arguments[1] == "f1")
                    {
                        SendMessage(window, 256, (IntPtr)112, (IntPtr)257);
                    }
                    else if (arguments[1] == "f2")
                    {
                        SendMessage(window, 256, (IntPtr)113, (IntPtr)257);
                    }
                    else if (arguments[1] == "f3")
                    {
                        SendMessage(window, 256, (IntPtr)114, (IntPtr)257);
                    }
                    else if (arguments[1] == "f4")
                    {
                        SendMessage(window, 256, (IntPtr)115, (IntPtr)257);
                    }
                    else if (arguments[1] == "f5")
                    {
                        SendMessage(window, 256, (IntPtr)116, (IntPtr)257);
                    }
                    else if (arguments[1] == "f6")
                    {
                        SendMessage(window, 256, (IntPtr)117, (IntPtr)257);
                    }
                    else if (arguments[1] == "f7")
                    {
                        SendMessage(window, 256, (IntPtr)118, (IntPtr)257);
                    }
                    else if (arguments[1] == "f8")
                    {
                        SendMessage(window, 256, (IntPtr)119, (IntPtr)257);
                    }
                    else if (arguments[1] == "f9")
                    {
                        SendMessage(window, 256, (IntPtr)120, (IntPtr)257);
                    }
                    else if (arguments[1] == "f10")
                    {
                        SendMessage(window, 256, (IntPtr)121, (IntPtr)257);
                    }
                    else if (arguments[1] == "f11")
                    {
                        SendMessage(window, 256, (IntPtr)128, (IntPtr)257);
                    }
                    else if (arguments[1] == "f12")
                    {
                        SendMessage(window, 256, (IntPtr)129, (IntPtr)257);
                    }
                    else if (arguments[1] == "tab")
                    {
                        SendMessage(window, 256, (IntPtr)9, (IntPtr)257);
                    }
                    else if (arguments[1] == "enter")
                    {
                        SendMessage(window, 256, (IntPtr)13, (IntPtr)257);
                    }
                    else if (arguments[1] == "numlock")
                    {
                        SendMessage(window, 256, (IntPtr)144, (IntPtr)257);
                    }
                    else if (arguments[1] == "esc")
                    {
                        SendMessage(window, 256, (IntPtr)27, (IntPtr)257);
                    }
                    else if (arguments[1].Length == 1 && Char.IsLetter(arguments[1][0]))
                    {
                        SendMessage(window, 256, (IntPtr)Char.ToUpperInvariant(arguments[1][0]), (IntPtr)257);
                    }
                }
            }
        }

    }
}
