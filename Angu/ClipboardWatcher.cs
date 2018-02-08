using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angu
{
    class ClipboardWatcher
    {
        public static IntPtr nextClipboardViewer;
        public static bool firstSet = true;
        public static bool skipNext = false;

        public static void wndProc(ref System.Windows.Forms.Message m)
        {
            // defined in winuser.h
            const int WM_DRAWCLIPBOARD = 0x308;
            const int WM_CHANGECBCHAIN = 0x030D;

            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    {
                        if (firstSet)
                        {
                            firstSet = false;
                        }
                        else if (skipNext)
                        {
                            skipNext = false;
                        }
                        else
                        {
                            Program.anguMain.onClipboardChange();
                        }

                        Angu.SendMessage(ClipboardWatcher.nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;
                    }

                case WM_CHANGECBCHAIN:
                    {
                        if (m.WParam == ClipboardWatcher.nextClipboardViewer)
                            ClipboardWatcher.nextClipboardViewer = m.LParam;
                        else
                            Angu.SendMessage(ClipboardWatcher.nextClipboardViewer, m.Msg, m.WParam, m.LParam);
                        break;
                    }

                default:
                    {
                        Program.anguMain.processWndProc(ref m);
                        break;
                    }
            }
        }
    }
}
