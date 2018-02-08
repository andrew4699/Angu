using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Angu
{
    static class Program
    {
        public static Angu anguMain;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!Debugger.IsAttached)
            {
                KeyboardHook.preInit();
                MouseHook.preInit();
            }

            anguMain = new Angu();
            Application.Run(anguMain);

            if (!Debugger.IsAttached)
            {
                KeyboardHook.postInit();
                MouseHook.postInit();
            }
        }
    }
}
