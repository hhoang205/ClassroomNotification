namespace btck
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ListBox listClients;
        private System.Windows.Forms.Label lblClientCount;
        private System.Windows.Forms.TextBox txtAnnouncement;
        private System.Windows.Forms.Button btnSendAll;
        private System.Windows.Forms.Button btnSendSelected;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Panel pnlAttachment;
        private System.Windows.Forms.Label lblAttachmentName;
        private System.Windows.Forms.Button btnRemoveAttachment;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.listClients = new System.Windows.Forms.ListBox();
            this.lblClientCount = new System.Windows.Forms.Label();
            this.txtAnnouncement = new System.Windows.Forms.TextBox();
            this.btnSendAll = new System.Windows.Forms.Button();
            this.btnSendSelected = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnUpload = new System.Windows.Forms.Button();
            this.pnlAttachment = new System.Windows.Forms.Panel();
            this.lblAttachmentName = new System.Windows.Forms.Label();
            this.btnRemoveAttachment = new System.Windows.Forms.Button();
            this.pnlAttachment.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(60, 12);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(80, 22);
            this.txtPort.TabIndex = 1;
            this.txtPort.Text = "5000";
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(160, 10);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 27);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(270, 10);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 27);
            this.btnStop.TabIndex = 3;
            this.btnStop.Text = "Stop";
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // listClients
            // 
            this.listClients.ItemHeight = 16;
            this.listClients.Location = new System.Drawing.Point(15, 70);
            this.listClients.Name = "listClients";
            this.listClients.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listClients.Size = new System.Drawing.Size(520, 196);
            this.listClients.TabIndex = 6;
            // 
            // lblClientCount
            // 
            this.lblClientCount.AutoSize = true;
            this.lblClientCount.Location = new System.Drawing.Point(160, 50);
            this.lblClientCount.Name = "lblClientCount";
            this.lblClientCount.Size = new System.Drawing.Size(60, 16);
            this.lblClientCount.TabIndex = 5;
            this.lblClientCount.Text = "Clients: 0";
            // 
            // txtAnnouncement
            // 
            this.txtAnnouncement.Location = new System.Drawing.Point(12, 380);
            this.txtAnnouncement.Multiline = true;
            this.txtAnnouncement.Name = "txtAnnouncement";
            this.txtAnnouncement.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAnnouncement.Size = new System.Drawing.Size(520, 168);
            this.txtAnnouncement.TabIndex = 8;
            // 
            // btnSendAll
            // 
            this.btnSendAll.Location = new System.Drawing.Point(12, 554);
            this.btnSendAll.Name = "btnSendAll";
            this.btnSendAll.Size = new System.Drawing.Size(120, 30);
            this.btnSendAll.TabIndex = 9;
            this.btnSendAll.Text = "Send to All";
            this.btnSendAll.Click += new System.EventHandler(this.btnSendAll_Click);
            // 
            // btnSendSelected
            // 
            this.btnSendSelected.Location = new System.Drawing.Point(160, 554);
            this.btnSendSelected.Name = "btnSendSelected";
            this.btnSendSelected.Size = new System.Drawing.Size(150, 30);
            this.btnSendSelected.TabIndex = 10;
            this.btnSendSelected.Text = "Send to Selected";
            this.btnSendSelected.Click += new System.EventHandler(this.btnSendSelected_Click);
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(550, 12);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(338, 536);
            this.txtLog.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Connected Clients:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 325);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(99, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Announcement:";
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(12, 344);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(120, 30);
            this.btnUpload.TabIndex = 0;
            this.btnUpload.Text = "📎 Upload File";
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // pnlAttachment
            // 
            this.pnlAttachment.BackColor = System.Drawing.Color.AliceBlue;
            this.pnlAttachment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAttachment.Controls.Add(this.lblAttachmentName);
            this.pnlAttachment.Controls.Add(this.btnRemoveAttachment);
            this.pnlAttachment.Location = new System.Drawing.Point(18, 391);
            this.pnlAttachment.Name = "pnlAttachment";
            this.pnlAttachment.Size = new System.Drawing.Size(428, 30);
            this.pnlAttachment.TabIndex = 0;
            this.pnlAttachment.Visible = false;
            // 
            // lblAttachmentName
            // 
            this.lblAttachmentName.AutoSize = true;
            this.lblAttachmentName.Location = new System.Drawing.Point(8, 8);
            this.lblAttachmentName.Name = "lblAttachmentName";
            this.lblAttachmentName.Size = new System.Drawing.Size(45, 16);
            this.lblAttachmentName.TabIndex = 0;
            this.lblAttachmentName.Text = "No file";
            // 
            // btnRemoveAttachment
            // 
            this.btnRemoveAttachment.BackColor = System.Drawing.Color.LightCoral;
            this.btnRemoveAttachment.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveAttachment.Location = new System.Drawing.Point(398, -1);
            this.btnRemoveAttachment.Name = "btnRemoveAttachment";
            this.btnRemoveAttachment.Size = new System.Drawing.Size(29, 29);
            this.btnRemoveAttachment.TabIndex = 1;
            this.btnRemoveAttachment.Text = "X";
            this.btnRemoveAttachment.UseVisualStyleBackColor = false;
            this.btnRemoveAttachment.Click += new System.EventHandler(this.btnRemoveAttachment_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 619);
            this.Controls.Add(this.pnlAttachment);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblClientCount);
            this.Controls.Add(this.listClients);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtAnnouncement);
            this.Controls.Add(this.btnSendAll);
            this.Controls.Add(this.btnSendSelected);
            this.Controls.Add(this.txtLog);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Teacher Server (TCP Broadcast)";
            this.pnlAttachment.ResumeLayout(false);
            this.pnlAttachment.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
