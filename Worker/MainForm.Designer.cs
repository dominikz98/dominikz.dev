namespace Worker
{
    partial class MainForm
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
            tabMenu = new TabControl();
            tpGeneral = new TabPage();
            rtbGeneralLog = new RichTextBox();
            tpCrontab = new TabPage();
            tpQueue = new TabPage();
            tabMenu.SuspendLayout();
            tpGeneral.SuspendLayout();
            SuspendLayout();
            // 
            // tabMenu
            // 
            tabMenu.Controls.Add(tpGeneral);
            tabMenu.Controls.Add(tpCrontab);
            tabMenu.Controls.Add(tpQueue);
            tabMenu.Dock = DockStyle.Fill;
            tabMenu.Location = new Point(0, 0);
            tabMenu.Name = "tabMenu";
            tabMenu.SelectedIndex = 0;
            tabMenu.Size = new Size(1136, 739);
            tabMenu.TabIndex = 0;
            // 
            // tpGeneral
            // 
            tpGeneral.BackColor = Color.FromArgb(28, 26, 26);
            tpGeneral.Controls.Add(rtbGeneralLog);
            tpGeneral.Location = new Point(4, 24);
            tpGeneral.Name = "tpGeneral";
            tpGeneral.Padding = new Padding(3);
            tpGeneral.Size = new Size(1128, 711);
            tpGeneral.TabIndex = 0;
            tpGeneral.Text = "General";
            // 
            // rtbGeneralLog
            // 
            rtbGeneralLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            rtbGeneralLog.BackColor = Color.FromArgb(28, 26, 26);
            rtbGeneralLog.Location = new Point(584, 3);
            rtbGeneralLog.Name = "rtbGeneralLog";
            rtbGeneralLog.Size = new Size(541, 705);
            rtbGeneralLog.TabIndex = 0;
            rtbGeneralLog.Text = "";
            // 
            // tpCrontab
            // 
            tpCrontab.BackColor = Color.FromArgb(28, 26, 26);
            tpCrontab.Location = new Point(4, 24);
            tpCrontab.Name = "tpCrontab";
            tpCrontab.Padding = new Padding(3);
            tpCrontab.Size = new Size(1128, 711);
            tpCrontab.TabIndex = 1;
            tpCrontab.Text = "Crontab";
            // 
            // tpQueue
            // 
            tpQueue.BackColor = Color.FromArgb(28, 26, 26);
            tpQueue.Location = new Point(4, 24);
            tpQueue.Name = "tpQueue";
            tpQueue.Padding = new Padding(3);
            tpQueue.Size = new Size(1128, 711);
            tpQueue.TabIndex = 3;
            tpQueue.Text = "Queue";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(28, 26, 26);
            ClientSize = new Size(1136, 739);
            Controls.Add(tabMenu);
            ForeColor = SystemColors.ControlLight;
            Name = "MainForm";
            Text = "Worker";
            tabMenu.ResumeLayout(false);
            tpGeneral.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabMenu;
        private TabPage tpGeneral;
        private TabPage tpCrontab;
        private TabPage tpQueue;
        private RichTextBox rtbGeneralLog;
    }
}