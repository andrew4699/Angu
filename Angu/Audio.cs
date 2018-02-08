using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Angu
{
    class Audio
    {
        private static bool speakers;

        private static int DEVICE_SPEAKER;
        private static int DEVICE_HEADPHONES;

        public static void swapDevice()
        {
            int device;

            if (speakers)
            {
                device = DEVICE_HEADPHONES;
                Notification.create("Headphones");
            }
            else
            {
                device = DEVICE_SPEAKER;
                Notification.create("Speakers");
            }

            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Users\\Andrew4699\\Desktop\\Angu\\audio_device.exe",
                    Arguments = device.ToString(),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            speakers = !speakers;
        }

        public static void scanDevices()
        {
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "C:\\Users\\Andrew4699\\Desktop\\Angu\\audio_device.exe",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            process.Start();

            int count = -1;

            while (!process.StandardOutput.EndOfStream)
            {
                string deviceName = process.StandardOutput.ReadLine();

                if (deviceName.Contains("Speakers") && deviceName.Contains("High Definition Audio Device"))
                {
                    if (count < 0)
                        speakers = true;
                    else
                        DEVICE_SPEAKER = count;
                }
                else if (deviceName.Contains("Logitech G933 Gaming Headset"))
                {
                    if (count < 0)
                        speakers = false;
                    else
                        DEVICE_HEADPHONES = count;
                }

                count++;
            }
        }
    }
}
