namespace StudentClientWinForms
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.RichTextBox txtReceived,txtChatBox;
        private System.Windows.Forms.ListBox lstDownloads;
        private System.Windows.Forms.Button btnOpenSelected;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtServer = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.txtReceived = new System.Windows.Forms.RichTextBox();
            this.lstDownloads = new System.Windows.Forms.ListBox();
            this.btnOpenSelected = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(70, 12);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(150, 22);
            this.txtServer.TabIndex = 3;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(270, 12);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(60, 22);
            this.txtPort.TabIndex = 4;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(397, 13);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(180, 22);
            this.txtName.TabIndex = 5;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(15, 45);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(80, 25);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(105, 45);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(90, 25);
            this.btnDisconnect.TabIndex = 7;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // txtReceived
            // 
            this.txtReceived.Location = new System.Drawing.Point(12, 91);
            this.txtReceived.Name = "txtReceived";
            this.txtReceived.ReadOnly = true;
            this.txtReceived.Size = new System.Drawing.Size(565, 322);
            this.txtReceived.TabIndex = 8;
            this.txtReceived.Text = "";
            // 
            // lstDownloads
            // 
            this.lstDownloads.ItemHeight = 16;
            this.lstDownloads.Location = new System.Drawing.Point(600, 89);
            this.lstDownloads.Name = "lstDownloads";
            this.lstDownloads.Size = new System.Drawing.Size(236, 324);
            this.lstDownloads.TabIndex = 9;
            this.lstDownloads.DoubleClick += new System.EventHandler(this.lstDownloads_DoubleClick);
            // 
            // btnOpenSelected
            // 
            this.btnOpenSelected.Location = new System.Drawing.Point(751, 428);
            this.btnOpenSelected.Name = "btnOpenSelected";
            this.btnOpenSelected.Size = new System.Drawing.Size(85, 30);
            this.btnOpenSelected.TabIndex = 10;
            this.btnOpenSelected.Text = "Open";
            this.btnOpenSelected.Click += new System.EventHandler(this.btnOpenSelected_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(21, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(230, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Port:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(340, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Name:";
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(848, 500);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.txtReceived);
            this.Controls.Add(this.lstDownloads);
            this.Controls.Add(this.btnOpenSelected);
            this.Name = "Form1";
            this.Text = "Student Client";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
