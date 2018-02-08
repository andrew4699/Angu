using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Angu
{
    static class Utils
    {
        private static FileStream logFile;

        public static Image GetImageFromClipboard()
        {
            if (Clipboard.GetDataObject() == null) return null;
            if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Dib))
            {
                var dib = ((System.IO.MemoryStream)Clipboard.GetData(DataFormats.Dib)).ToArray();
                var width = BitConverter.ToInt32(dib, 4);
                var height = BitConverter.ToInt32(dib, 8);
                var bpp = BitConverter.ToInt16(dib, 14);
                if (bpp == 32)
                {
                    var gch = GCHandle.Alloc(dib, GCHandleType.Pinned);
                    Bitmap bmp = null;
                    try
                    {
                        var ptr = new IntPtr((long)gch.AddrOfPinnedObject() + 40);
                        bmp = new Bitmap(width, height, width * 4, System.Drawing.Imaging.PixelFormat.Format32bppArgb, ptr);
                        bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
                        return new Bitmap(bmp);
                    }
                    finally
                    {
                        gch.Free();
                        if (bmp != null) bmp.Dispose();
                    }
                }
            }
            return Clipboard.ContainsImage() ? Clipboard.GetImage() : null;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static void OpenLogFile()
        {
            //logFile = File.Open(Path.Combine(Angu.RUN_PATH, "log.txt"), FileMode.Append, FileAccess.Write, FileShare.Read);
        }

        public static void CloseLogFile()
        {
            //logFile.Close();
        }

        public static void Log(object text)
        {
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(text);
            }

            try
            {
                using (StreamWriter sw = File.AppendText(Path.Combine(Angu.RUN_PATH, "log.txt")))
                {
                    sw.WriteLine("[" + DateTime.Now.ToString("M/d/yy H:mm:ss") + "] " + text.ToString());
                }
            }
            catch { }

            //byte[] data = new UTF8Encoding(true).GetBytes("[" + DateTime.Now.ToString("M/d/yy H:mm:ss") + "] " + text);
            //logFile.Write(data, 0, data.Length);
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
