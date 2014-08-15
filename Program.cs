using System;
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
