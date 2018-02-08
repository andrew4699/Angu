using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Angu
{
    public static class LED
    {
        public const int LOGI_DEVICETYPE_MONOCHROME_ORD = 0;
        public const int LOGI_DEVICETYPE_RGB_ORD = 1;
        public const int LOGI_DEVICETYPE_PERKEY_RGB_ORD = 2;

        public const int LOGI_DEVICETYPE_MONOCHROME = (1 << LOGI_DEVICETYPE_MONOCHROME_ORD);
        public const int LOGI_DEVICETYPE_RGB = (1 << LOGI_DEVICETYPE_RGB_ORD);
        public const int LOGI_DEVICETYPE_PERKEY_RGB = (1 << LOGI_DEVICETYPE_PERKEY_RGB_ORD);

        public const int LOGI_DEVICETYPE_ALL = (LOGI_DEVICETYPE_MONOCHROME | LOGI_DEVICETYPE_RGB | LOGI_DEVICETYPE_PERKEY_RGB);

        [DllImport("C:/Users/Andrew4699/Desktop/LED/Lib/LogitechLedEnginesWrapper/x86/LogitechLedEnginesWrapper.dll", EntryPoint = "LogiLedInit", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool Init();

        [DllImport("C:/Users/Andrew4699/Desktop/LED/Lib/LogitechLedEnginesWrapper/x86/LogitechLedEnginesWrapper.dll", EntryPoint = "LogiLedShutdown", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern void Shutdown();

        [DllImport("C:/Users/Andrew4699/Desktop/LED/Lib/LogitechLedEnginesWrapper/x86/LogitechLedEnginesWrapper.dll", EntryPoint = "LogiLedSaveCurrentLighting", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SaveCurrentLighting();

        [DllImport("C:/Users/Andrew4699/Desktop/LED/Lib/LogitechLedEnginesWrapper/x86/LogitechLedEnginesWrapper.dll", EntryPoint = "LogiLedRestoreLighting", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool RestoreLighting();

        [DllImport("C:/Users/Andrew4699/Desktop/LED/Lib/LogitechLedEnginesWrapper/x86/LogitechLedEnginesWrapper.dll", EntryPoint = "LogiLedStopEffects", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool StopEffects();

        [DllImport("C:/Users/Andrew4699/Desktop/LED/Lib/LogitechLedEnginesWrapper/x86/LogitechLedEnginesWrapper.dll", EntryPoint = "LogiLedFlashLighting", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool FlashLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        [DllImport("C:/Users/Andrew4699/Desktop/LED/Lib/LogitechLedEnginesWrapper/x86/LogitechLedEnginesWrapper.dll", EntryPoint = "LogiLedPulseLighting", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool PulseLighting(int redPercentage, int greenPercentage, int bluePercentage, int milliSecondsDuration, int milliSecondsInterval);

        [DllImport("C:/Users/Andrew4699/Desktop/LED/Lib/LogitechLedEnginesWrapper/x86/LogitechLedEnginesWrapper.dll", EntryPoint = "LogiLedSetTargetDevice", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetTargetDevice(int targetDevice);

        [DllImport("C:/Users/Andrew4699/Desktop/LED/Lib/LogitechLedEnginesWrapper/x86/LogitechLedEnginesWrapper.dll", EntryPoint = "LogiLedSetLighting", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool SetLighting(int redPercentage, int greenPercentage, int bluePercentage);
    }
}
