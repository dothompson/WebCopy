namespace WebCopy
{
    partial class Form1
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
            this.txtFromURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDestinationFolder = new System.Windows.Forms.TextBox();
            this.btnStartDownload = new System.Windows.Forms.Button();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkLogging = new System.Windows.Forms.CheckBox();
            this.btnPickFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "From URL";
            // 
            // txtFromURL
            // 
            this.txtFromURL.Location = new System.Drawing.Point(137, 10);
            this.txtFromURL.Name = "txtFromURL";
            this.txtFromURL.Size = new System.Drawing.Size(347, 20);
            this.txtFromURL.TabIndex = 1;
            this.txtFromURL.Text = "http://localhost:50388/";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Destination Folder";
            // 
            // txtDestinationFolder
            // 
            this.txtDestinationFolder.Location = new System.Drawing.Point(137, 37);
            this.txtDestinationFolder.Name = "txtDestinationFolder";
            this.txtDestinationFolder.Size = new System.Drawing.Size(345, 20);
            this.txtDestinationFolder.TabIndex = 3;
            this.txtDestinationFolder.Text = "C:\\WebCopyDownloads";
            this.txtDestinationFolder.TextChanged += new System.EventHandler(this.txtDestinationFolder_TextChanged);
            // 
            // btnStartDownload
            // 
            this.btnStartDownload.Location = new System.Drawing.Point(17, 80);
            this.btnStartDownload.Name = "btnStartDownload";
            this.btnStartDownload.Size = new System.Drawing.Size(155, 30);
            this.btnStartDownload.TabIndex = 4;
            this.btnStartDownload.Text = "Start Download";
            this.btnStartDownload.UseVisualStyleBackColor = true;
            this.btnStartDownload.Click += new System.EventHandler(this.btnStartDownload_Click);
            // 
            // lbLog
            // 
            this.lbLog.FormattingEnabled = true;
            this.lbLog.Location = new System.Drawing.Point(16, 157);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(772, 277);
            this.lbLog.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(25, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Log";
            // 
            // chkLogging
            // 
            this.chkLogging.AutoSize = true;
            this.chkLogging.Location = new System.Drawing.Point(359, 92);
            this.chkLogging.Name = "chkLogging";
            this.chkLogging.Size = new System.Drawing.Size(106, 17);
            this.chkLogging.TabIndex = 7;
            this.chkLogging.Text = "Logging Enabled";
            this.chkLogging.UseVisualStyleBackColor = true;
            // 
            // btnPickFolder
            // 
            this.btnPickFolder.Location = new System.Drawing.Point(505, 40);
            this.btnPickFolder.Name = "btnPickFolder";
            this.btnPickFolder.Size = new System.Drawing.Size(75, 23);
            this.btnPickFolder.TabIndex = 8;
            this.btnPickFolder.Text = "Pick Folder";
            this.btnPickFolder.UseVisualStyleBackColor = true;
            this.btnPickFolder.Click += new System.EventHandler(this.btnPickFolder_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnPickFolder);
            this.Controls.Add(this.chkLogging);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lbLog);
            this.Controls.Add(this.btnStartDownload);
            this.Controls.Add(this.txtDestinationFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFromURL);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "WebCopy Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtFromURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDestinationFolder;
        private System.Windows.Forms.Button btnStartDownload;
        private System.Windows.Forms.ListBox lbLog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkLogging;
        private System.Windows.Forms.Button btnPickFolder;
    }
}

