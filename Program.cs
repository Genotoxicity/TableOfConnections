using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KSP.E3.TableOfConnections
{
    class Program
    {
        // All WPF applications should execute on a single-threaded apartment (STA) thread
        [STAThread]
        public static void Main()
        {
            Script script = new Script();
            new Application().Run(script.UI);
        }
    }
}
