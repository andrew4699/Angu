namespace Angu
{
    partial class Notification
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.closeNotification = new System.Windows.Forms.Timer(this.components);
            this.notificationText = new System.Windows.Forms.Label();
            this.sideBox = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // closeNotification
            // 
            this.closeNotification.Enabled = true;
            this.closeNotification.Interval = 50;
            this.closeNotification.Tick += new System.EventHandler(this.closeNotification_Tick);
            // 
            // notificationText
            // 
            this.notificationText.AutoSize = true;
            this.notificationText.Font = new System.Drawing.Font("Roboto", 12F);
            this.notificationText.ForeColor = System.Drawing.Color.White;
            this.notificationText.Location = new System.Drawing.Point(38, 21);
            this.notificationText.MaximumSize = new System.Drawing.Size(300, 0);
            this.notificationText.Name = "notificationText";
            this.notificationText.Size = new System.Drawing.Size(35, 19);
            this.notificationText.TabIndex = 0;
            this.notificationText.Text = "test";
            // 
            // sideBox
            // 
            this.sideBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
            this.sideBox.Location = new System.Drawing.Point(0, 0);
            this.sideBox.Name = "sideBox";
            this.sideBox.Size = new System.Drawing.Size(15, 60);
            this.sideBox.TabIndex = 1;
            // 
            // Notification
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(33)))), ((int)(((byte)(33)))));
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(250, 59);
            this.Controls.Add(this.sideBox);
            this.Controls.Add(this.notificationText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Notification";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Notification";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer closeNotification;
        private System.Windows.Forms.Label notificationText;
        private System.Windows.Forms.Panel sideBox;
    }
}