using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ditto.Commands
{
    class Cursor
    {

        public static void Execute(Macro macro, string[] arguments)
        {
            int x = Int32.Parse(arguments[1]);
            int y = Int32.Parse(arguments[2]);
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(x, y);
        }

    }
}
