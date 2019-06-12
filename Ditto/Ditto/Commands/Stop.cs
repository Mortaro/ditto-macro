using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ditto.Commands
{
    class Stop
    {

        public static void Execute(Macro macro, string[] arguments)
        {
            if (arguments.Length == 2 && macro.Running)
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
            if (arguments.Length == 1)
            {
                macro.BeginInvoke(new MethodInvoker(delegate ()
                {
                    macro.Stop();
                }));
            }
        }

    }
}
