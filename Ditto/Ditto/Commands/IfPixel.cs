using System;

namespace Ditto.Commands
{
    class IfPixel
    {
        public static bool TryMatches(Macro macro, string[] arguments, out bool matches)
        {
            matches = false;
            if (arguments.Length < 4)
            {
                return false;
            }

            string character = null;
            int tolerance = 0;
            for (int i = 4; i < arguments.Length; i++)
            {
                string argument = arguments[i];
                if (argument.StartsWith("@"))
                {
                    character = argument;
                }
                else
                {
                    Int32.TryParse(argument, out tolerance);
                }
            }

            IntPtr window;
            if (!TryResolveWindow(macro, character, out window))
            {
                return false;
            }

            int x, y;
            if (!Int32.TryParse(arguments[1], out x) || !Int32.TryParse(arguments[2], out y))
            {
                return false;
            }

            string[] channels = arguments[3].Split('.');
            if (channels.Length != 3)
            {
                return false;
            }
            int r, g, b;
            if (!Int32.TryParse(channels[0], out r)
                || !Int32.TryParse(channels[1], out g)
                || !Int32.TryParse(channels[2], out b))
            {
                return false;
            }

            var current = Pixel.GetPixel(window, x, y);
            matches = Math.Abs(current.R - r) <= tolerance
                && Math.Abs(current.G - g) <= tolerance
                && Math.Abs(current.B - b) <= tolerance;
            return true;
        }

        private static bool TryResolveWindow(Macro macro, string character, out IntPtr window)
        {
            window = IntPtr.Zero;
            try
            {
                if (character == null)
                {
                    if (macro.Windows.Count == 0)
                    {
                        return false;
                    }
                    window = macro.Windows[0];
                    return true;
                }
                foreach (var other in macro.Launcher.Macros)
                {
                    if (other.Character == character && other.Windows.Count > 0)
                    {
                        window = other.Windows[0];
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return false;
        }

        public static int IndexOfEndif(string[] lines, int index)
        {
            int depth = 0;
            for (int i = index + 1; i < lines.Length; i++)
            {
                string head = lines[i].Trim().Split(' ')[0];
                if (head == "ifpixel" || head == "ifnotpixel")
                {
                    depth++;
                }
                else if (head == "endif")
                {
                    if (depth == 0)
                    {
                        return i;
                    }
                    depth--;
                }
            }
            return lines.Length;
        }
    }
}
