using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ditto.Commands
{
    class Wait
    {

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public static void Execute(Macro macro, string[] arguments)
        {
            System.Diagnostics.Debug.WriteLine("WAITING");
            if (arguments.Length == 2 && macro.Running)
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                int miliseconds = GetMiliseconds(arguments[1]);
                while (timer.Elapsed < TimeSpan.FromMilliseconds((double)miliseconds) && macro.Running)
                {
                    Thread.Sleep(100);
                    macro.BeginInvoke(new MethodInvoker(delegate ()
                    {
                        int remainder = miliseconds - (int)timer.ElapsedMilliseconds;
                        if (remainder < 0) remainder = 0;
                        TimeSpan time = TimeSpan.FromMilliseconds((double)remainder);
                        string mask;
                        if (remainder > 60000)
                        {
                            mask = string.Format("{0:D2}m {1:D2}s", time.Minutes, time.Seconds);
                        }
                        else if (remainder > 1000)
                        {
                            mask = string.Format("{0:D2}s {1:D3}ms", time.Seconds, time.Milliseconds);
                        }
                        else
                        {
                            mask = string.Format("{0:D3}ms", time.Milliseconds);
                        }
                        macro.SetStartButtonText(mask.ToString());
                    }));
                }
                macro.BeginInvoke(new MethodInvoker(delegate ()
                {
                    macro.SetStartButtonText("Start");
                }));
                timer.Stop();
            }
        }

        public static int GetMiliseconds(string Time)
        {
            if (Time.Contains("s"))
            {
                Time = Time.Replace("s", "");
                return int.Parse(Time) * 1000;
            }
            if (Time.Contains("m"))
            {
                Time = Time.Replace("m", "");
                return int.Parse(Time) * 60000;
            }
            return int.Parse(Time);
        }

    }
}
