using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Angu
{
    class KeyProcessor
    {
        private const string VOLUME_KEY = "F12";
        public static bool volumeKeyDown = false;

        private const int DOUBLE_TAP_TIME = 200;
        private static Dictionary<string, Timer> keyPressTime = new Dictionary<string, Timer>();

        public static void initialize()
        {
            for (int i = 6; i < 11; i++)
            {
                string index = "F" + i;
                
                Timer keyTimer = new Timer();
                keyTimer.Interval = DOUBLE_TAP_TIME;
                keyTimer.Tick += delegate
                {
                    Program.anguMain.onGlobalKeyPress(index, false);
                    keyPressTime[index].Stop();
                };

                keyPressTime.Add(index, keyTimer);
            }
        }

        public static void onGlobalKeyPress(string key, bool down)
        {
            if (!down)
            {
                if(key == VOLUME_KEY)
                {
                    volumeKeyDown = false;
                }
                else if (keyPressTime.ContainsKey(key)) // F6 - F10
                {
                    if (keyPressTime[key].Enabled) // Double tap
                    {
                        keyPressTime[key].Enabled = false;
                        Program.anguMain.onGlobalKeyPress(key, true);
                    }
                    else
                    {
                        keyPressTime[key].Start();
                    }
                }
            }
            else if(key == VOLUME_KEY)
            {
                volumeKeyDown = true;
            }
        }

        private static int getTime()
        {
            return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
