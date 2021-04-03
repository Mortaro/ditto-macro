using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ditto.Commands
{
    class Stop
    {

        public static void Execute(Macro macro, string[] arguments)
        {
            if (arguments.Length >= 2 && macro.Running)
            {
                if (Regex.IsMatch(arguments[1], @"^\d+$"))
                {
                    macro.BeginInvoke(new MethodInvoker(delegate ()
                    {
                        int index = Int32.Parse(arguments[1]);
                        if (index >= 0 && macro.Launcher.Macros.Count > index)
                        {
                            macro.Launcher.Macros[index].Stop();
                        }
                    }));
                }
                else if(macro.Launcher.NetworkMode.Checked)
                {
                    if (arguments.Length == 3)
                    {
                        macro.Launcher.Socket.Send(macro.Launcher.Password() + " stop " + arguments[1] + " " + arguments[2]);
                    }
                    else
                    {
                        macro.Launcher.Socket.Send(macro.Launcher.Password() + " stop " + arguments[1]);
                    }
                }
            }
            else if (arguments.Length == 1)
            {
                macro.BeginInvoke(new MethodInvoker(delegate ()
                {
                    macro.Stop();
                }));
            }
        }

    }
}
