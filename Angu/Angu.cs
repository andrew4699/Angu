using Ionic.Zip;
using Microsoft.Win32;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Angu
{
    public partial class Angu : Form
    {
        private void Angu_Load(object sender, EventArgs e)
        {
            RUN_PATH = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Utils.OpenLogFile();

            //ClipboardWatcher.nextClipboardViewer = (IntPtr)SetClipboardViewer((int)this.Handle);

            /*if (!LED.Init())
            {
                Debug.WriteLine("Couldn't initialize LED SDK");
            }

            LED.SetTargetDevice(LED.LOGI_DEVICETYPE_ALL);
            LED.SetLighting(30, 40, 34);*/

            verifyRegistry();
            Audio.scanDevices();
            //Communication.initialize();

            (new Thread(handleChromeMessage)).Start();
        }

        public void onGlobalKeyPress(string key, bool doubleTap)
        {
            if (key == "F10")
            {
                SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_MUTE);
            }
            else if (key == "F9")
            {
                sendChromeMessage("skip");
                Notification.create("Song skipped");
            }
            else if (key == "F8")
            {
                Audio.swapDevice();
            }
            else if (key == "F7")
            {
                if (doubleTap)
                {
                    sendChromeMessage("delete");
                    Notification.create("Removed from playlist");
                }
                else
                {
                    sendChromeMessage("add");
                    Notification.create("Added to playlist");
                }
            }
            else if (key == "F6")
            {
                sendChromeMessage("pause");
                Notification.create("Paused");
            }
        }

        public void onGlobalMouseWheel(int delta)
        {
            if(KeyProcessor.volumeKeyDown)
            {
                if(delta > 0)
                {
                    SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_UP);
                }
                else
                {
                    SendMessageW(this.Handle, WM_APPCOMMAND, this.Handle, (IntPtr)APPCOMMAND_VOLUME_DOWN);
                }
            }
        }

        public void onClipboardChange()
        {
            if(Clipboard.ContainsText())
            {
                Notification.create("Copied text to clipboard", 3000, true);
            }
            else if(Clipboard.ContainsImage())
            {
                Notification.create("Copied image to clipboard", 3000, true);
            }
            else if(Clipboard.ContainsFileDropList())
            {
                Notification.create("Copied file(s) to clipboard", 3000, true);
            }
        }

        public void onReceiveMessage(int type, byte[] data)
        {
            if (type == Message.FINISHED)
            {
                Communication.messageState = Message.STATE_FINISHED;
            }
            else if (type == Message.NEXT)
            {
                Communication.messageState = Message.STATE_NEXT;
            }
            else if (type == Message.TEXT)
            {
                try
                {
                    ClipboardWatcher.firstSet = true;
                    ClipboardWatcher.skipNext = true;

                    Program.anguMain.BeginInvoke((Action)delegate
                    {
                        Notification.create("Received clipboard");
                    });

                    Thread thread = new Thread(() => Clipboard.SetText(Encoding.ASCII.GetString(data), TextDataFormat.Text));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                }
                catch (Exception ex)
                {
                    Utils.Log("Failed setting clipboard: " + ex.ToString());
                }
            }
            else if (type == Message.FILE)
            {
                string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "recv.zip");
                File.WriteAllBytes(savePath, data);

                Program.anguMain.BeginInvoke((Action)delegate
                {
                    Notification.create("Received files");
                });
            }
            else if (type == Message.IMAGE)
            {
                string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "recv.png");
                File.WriteAllBytes(savePath, data);

                Program.anguMain.BeginInvoke((Action)delegate
                {
                    Notification.create("Received image");
                });
            }
        }

        private void handleChromeMessage()
        {
            while(true)
            {
                JObject msg = readChromeMessage();
                
            }
        }

        // ====================================================
        // ====================================================

        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xAFFFF;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        private const string REG_PATH = "HKEY_CURRENT_USER\\Software\\Google\\Chrome\\NativeMessagingHosts\\com.angu.angu";
        public static string RUN_PATH;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        protected static extern int SetClipboardViewer(int hWndNewViewer);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr hWndRemove, IntPtr hWndNewNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        public Angu()
        {
            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var Params = base.CreateParams;
                Params.ExStyle |= 0x80;
                return Params;
            }
        }

        private static void sendChromeMessage(string data)
        {
            string msg = "{\"text\":\"" + data + "\"}";
            int dataLength = msg.Length;

            Stream stdout = Console.OpenStandardOutput();
            stdout.WriteByte((byte)((dataLength >> 0) & 0xFF));
            stdout.WriteByte((byte)((dataLength >> 8) & 0xFF));
            stdout.WriteByte((byte)((dataLength >> 16) & 0xFF));
            stdout.WriteByte((byte)((dataLength >> 24) & 0xFF));

            Console.Write(msg);
        }

        private static JObject readChromeMessage()
        {
            var stdin = Console.OpenStandardInput();
            var length = 0;

            var lengthBytes = new byte[4];
            stdin.Read(lengthBytes, 0, 4);
            length = BitConverter.ToInt32(lengthBytes, 0);

            var buffer = new char[length];
            using (var reader = new StreamReader(stdin))
            {
                while (reader.Peek() >= 0)
                {
                    reader.Read(buffer, 0, buffer.Length);
                }
            }

            //return new string(buffer);
            return (JObject)JsonConvert.DeserializeObject<JObject>(new string(buffer));
        }

        private static void verifyRegistry()
        {
            var keyValue = Registry.GetValue(REG_PATH, null, "");
            
            if(keyValue.ToString().Length == 0)
            {
                Registry.SetValue(REG_PATH, null, RUN_PATH + "\\host.json");
            }
        }

        public static void sendClipboard()
        {
            if (Clipboard.ContainsText())
            {
                Communication.queueMessage(Message.TEXT, Encoding.ASCII.GetBytes(Clipboard.GetText()));
            }
            else if (Clipboard.ContainsFileDropList())
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "recv.zip");
                StringCollection fileList = Clipboard.GetFileDropList();

                using (ZipFile zip = new ZipFile())
                {
                    foreach (string file in fileList)
                    {
                        string fileName = Path.GetFileName(file);
                        FileAttributes attr = File.GetAttributes(file);

                        if (attr.HasFlag(FileAttributes.Directory)) // Check if is a directory
                        {
                            zip.AddDirectory(file, fileName);
                        }
                        else
                        {
                            zip.AddFile(file, "");
                        }
                    }

                    zip.Save(filePath);
                }

                Communication.sendFile(filePath);
            }
            else if (Clipboard.ContainsImage())
            {
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "recv.png");

                Image image = Utils.GetImageFromClipboard(); // Fixes Chrome transparency issue
                image.Save(filePath, ImageFormat.Png);

                Communication.sendImage(filePath);
            }
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            ClipboardWatcher.wndProc(ref m);
        }

        public void processWndProc(ref System.Windows.Forms.Message m)
        {
            base.WndProc(ref m);
        }

        private void Angu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Utils.CloseLogFile();
        }
    }
}
