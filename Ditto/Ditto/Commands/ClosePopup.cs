using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Ditto.Commands
{
    class ClosePopup
    {
        [DllImport("gdi32.dll")]
        private static extern int BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
            IntPtr hdcSrc, int nXSrc, int nYSrc, Int32 dwRop);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private const int WM_KEYDOWN = 0x100, WM_KEYUP = 0x101;
        private const int WM_MOUSEMOVE = 0x200, WM_LBUTTONDOWN = 0x201, WM_LBUTTONUP = 0x202;
        private const int MK_LBUTTON = 1, VK_ESCAPE = 27;

        private const int FRAME_Y = 575, FRAME_X0 = 800, FRAME_WIDTH = 320;
        private const int FRAME_GOLD_MINIMUM = 200;
        private const int OK_Y = 556, OK_X0 = 900, OK_WIDTH = 115;
        private const int OK_RED_MINIMUM = 80;
        private const int OK_CLICK_X = 959, OK_CLICK_Y = 558;
        private const int CLOSE_CLICK_X = 1088, CLOSE_CLICK_Y = 412;
        private const int SETTLE_MS = 700;

        public static void Execute(Macro macro, string[] arguments)
        {
            if (!macro.Running) return;
            foreach (IntPtr window in macro.Windows)
            {
                if (HasDialog(window)) Dismiss(macro, window);
            }
        }

        private static bool HasDialog(IntPtr window)
        {
            try
            {
                int gold = 0;
                using (Bitmap frame = Grab(window, FRAME_X0, FRAME_Y, FRAME_WIDTH))
                    for (int x = 0; x < frame.Width; x++)
                        if (IsFrameGold(frame.GetPixel(x, 0))) gold++;
                if (gold < FRAME_GOLD_MINIMUM) return false;

                int red = 0;
                using (Bitmap button = Grab(window, OK_X0, OK_Y, OK_WIDTH))
                    for (int x = 0; x < button.Width; x++)
                        if (IsButtonRed(button.GetPixel(x, 0))) red++;
                return red >= OK_RED_MINIMUM;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static void Dismiss(Macro macro, IntPtr window)
        {
            Click(window, OK_CLICK_X, OK_CLICK_Y);
            if (Gone(macro, window)) return;

            Click(window, CLOSE_CLICK_X, CLOSE_CLICK_Y);
            if (Gone(macro, window)) return;

            SendMessage(window, WM_KEYDOWN, VK_ESCAPE, 0x10001);
            SendMessage(window, WM_KEYUP, VK_ESCAPE, unchecked((int)0xC0010001));
            if (Gone(macro, window)) return;

            PostMessage(window, WM_KEYDOWN, VK_ESCAPE, 0x10001);
            PostMessage(window, WM_KEYUP, VK_ESCAPE, unchecked((int)0xC0010001));
        }

        private static bool Gone(Macro macro, IntPtr window)
        {
            Wait.Execute(macro, new string[] { "wait", SETTLE_MS.ToString() });
            return !HasDialog(window);
        }

        private static void Click(IntPtr window, int x, int y)
        {
            int lParam = x | y << 16;
            SendMessage(window, WM_MOUSEMOVE, 0, lParam);
            SendMessage(window, WM_LBUTTONDOWN, MK_LBUTTON, lParam);
            SendMessage(window, WM_LBUTTONUP, 0, lParam);
        }

        private static bool IsFrameGold(Color c)
        {
            return c.R > 120 && c.G > 95 && c.B < 110 && (c.R - c.B) > 45 && Math.Abs(c.R - c.G) < 45;
        }

        private static bool IsButtonRed(Color c)
        {
            return c.R > 70 && c.R - c.G > 25 && c.R - c.B > 25;
        }

        private static Bitmap Grab(IntPtr window, int x, int y, int width)
        {
            Bitmap strip = new Bitmap(width, 1);
            using (Graphics destination = Graphics.FromImage(strip))
            using (Graphics source = Graphics.FromHwnd(window))
            {
                IntPtr sourceDc = source.GetHdc();
                IntPtr destinationDc = destination.GetHdc();
                BitBlt(destinationDc, 0, 0, width, 1, sourceDc, x, y, (int)CopyPixelOperation.SourceCopy);
                destination.ReleaseHdc();
                source.ReleaseHdc();
            }
            return strip;
        }

    }
}
