namespace SE_Modz_Installer
{
    partial class frmMain
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtGamePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlDrop = new System.Windows.Forms.Panel();
            this.lbxContents = new System.Windows.Forms.ListBox();
            this.pbxIcon = new System.Windows.Forms.PictureBox();
            this.lnkSEMForum = new System.Windows.Forms.LinkLabel();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(201, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "SE-Modz Block Installer";
            // 
            // txtGamePath
            // 
            this.txtGamePath.Location = new System.Drawing.Point(9, 61);
            this.txtGamePath.Name = "txtGamePath";
            this.txtGamePath.Size = new System.Drawing.Size(436, 20);
            this.txtGamePath.TabIndex = 1;
            this.txtGamePath.Text = "Click here to locate your game directory";
            this.txtGamePath.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtGamePath_MouseClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Path to Space Engineers";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 305);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(536, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = false;
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(521, 17);
            this.lblStatus.Spring = true;
            this.lblStatus.Text = "Idle";
            // 
            // pnlDrop
            // 
            this.pnlDrop.AllowDrop = true;
            this.pnlDrop.BackColor = System.Drawing.Color.Maroon;
            this.pnlDrop.Enabled = false;
            this.pnlDrop.Location = new System.Drawing.Point(9, 93);
            this.pnlDrop.Name = "pnlDrop";
            this.pnlDrop.Size = new System.Drawing.Size(515, 88);
            this.pnlDrop.TabIndex = 4;
            this.pnlDrop.DragEnter += new System.Windows.Forms.DragEventHandler(this.pnlDrop_DragEnter);
            // 
            // lbxContents
            // 
            this.lbxContents.FormattingEnabled = true;
            this.lbxContents.Location = new System.Drawing.Point(9, 188);
            this.lbxContents.Name = "lbxContents";
            this.lbxContents.Size = new System.Drawing.Size(515, 95);
            this.lbxContents.TabIndex = 5;
            // 
            // pbxIcon
            // 
            this.pbxIcon.Location = new System.Drawing.Point(451, 9);
            this.pbxIcon.Name = "pbxIcon";
            this.pbxIcon.Size = new System.Drawing.Size(73, 68);
            this.pbxIcon.TabIndex = 6;
            this.pbxIcon.TabStop = false;
            // 
            // lnkSEMForum
            // 
            this.lnkSEMForum.AutoSize = true;
            this.lnkSEMForum.Location = new System.Drawing.Point(363, 16);
            this.lnkSEMForum.Name = "lnkSEMForum";
            this.lnkSEMForum.Size = new System.Drawing.Size(82, 13);
            this.lnkSEMForum.TabIndex = 7;
            this.lnkSEMForum.TabStop = true;
            this.lnkSEMForum.Text = "SE-Modz Forum";
            this.lnkSEMForum.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSEMForum_LinkClicked);
            // 
            // frmMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(536, 327);
            this.Controls.Add(this.lnkSEMForum);
            this.Controls.Add(this.pbxIcon);
            this.Controls.Add(this.lbxContents);
            this.Controls.Add(this.pnlDrop);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtGamePath);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.Text = "SE-Modz";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxIcon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGamePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Panel pnlDrop;
        private System.Windows.Forms.ListBox lbxContents;
        private System.Windows.Forms.PictureBox pbxIcon;
        private System.Windows.Forms.LinkLabel lnkSEMForum;
    }
}

