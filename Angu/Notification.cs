using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Angu
{
    public partial class Notification : Form
    {
        private const int H_PADDING = 10;
        private const int V_PADDING = 15;

        private int tickCount = 0;

        public int location;
        private int startFade;
        private bool clipboardClick;

        public Notification(string message, int iStartFade, bool iClipboardClick)
        {
            InitializeComponent();

            Utils.Log("notification: " + message);
            this.location = Notification.getFreeLocation();
            
            if(location == -1) // Too many notifications at once
            {
                this.Close();
                return;
            }
            
            spotTaken[location] = true;
            
            this.startFade = iStartFade;
            this.clipboardClick = iClipboardClick;
            
            if(this.clipboardClick)
            {
                this.Click += this.handleNotificationClick;
                this.notificationText.Click += this.handleNotificationClick;
                this.sideBox.Click += this.handleNotificationClick;
            }
            
            this.notificationText.Text = message;

            this.Size = new Size(this.notificationText.Size.Width + (H_PADDING * 2), this.notificationText.Size.Height + (V_PADDING * 2));
            this.Location = new Point(Screen.PrimaryScreen.WorkingArea.Width - (this.Size.Width), 10 + (location * 100));
           
            this.sideBox.Size = new Size(this.sideBox.Size.Width, this.Size.Height);
            
            this.notificationText.Location = new Point((H_PADDING / 2) + this.sideBox.Width, (this.Size.Height - this.notificationText.Size.Height) / 2);
        }

        private void closeNotification_Tick(object sender, EventArgs e)
        {
            tickCount++;

            var timePassed = closeNotification.Interval * tickCount;

            if (timePassed > this.startFade)
            {
                var postTicks = (timePassed - this.startFade) / closeNotification.Interval;

                this.Opacity = 1 - (postTicks * 0.03);

                if(this.Opacity <= 0)
                {
                    closeNotification.Stop();
                    spotTaken[location] = false;
                    this.Close();
                }
            }
        }

        private void handleNotificationClick(object sender, EventArgs e)
        {
            this.Close();
            Angu.sendClipboard();
        }

        // Management
        private static bool[] spotTaken = new bool[10];
        private static List<Notification> notifications = new List<Notification>();

        public static void create(string message, int startFade = 1500, bool clipboardClick = false)
        {
            Notification notification = new Notification(message, startFade, clipboardClick);
            notifications.Add(notification);

            //if(!notification.IsDisposed)
                notification.Show();
        }

        private static int getFreeLocation()
        {
            for(int i = 0; i < spotTaken.Length; i++)
            {
                if(spotTaken[i] != true)
                {
                    return i;
                }
            }

            return -1;
        }

        // Don't focus on open
        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                //make sure Top Most property on form is set to false
                //otherwise this doesn't work
                int WS_EX_TOPMOST = 0x00000008;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_TOPMOST;
                return cp;
            }
        }
    }
}
