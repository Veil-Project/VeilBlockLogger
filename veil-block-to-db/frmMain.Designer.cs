namespace VeilBlockToDB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.rtxStatus = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnReprocessAll = new System.Windows.Forms.Button();
            this.chkSaveToFileSystem = new System.Windows.Forms.CheckBox();
            this.txtSavePath = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtResyncBlock = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.LimeGreen;
            this.btnStart.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnStart.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.ForeColor = System.Drawing.Color.White;
            this.btnStart.Location = new System.Drawing.Point(17, 15);
            this.btnStart.Margin = new System.Windows.Forms.Padding(6);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(153, 73);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "Start Saving\r\nBlocks";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.BtnStart_Click);
            // 
            // btnStop
            // 
            this.btnStop.BackColor = System.Drawing.Color.Red;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnStop.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(182, 15);
            this.btnStop.Margin = new System.Windows.Forms.Padding(6);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(153, 73);
            this.btnStop.TabIndex = 1;
            this.btnStop.Text = "Stop Saving\r\nBlocks";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Click += new System.EventHandler(this.BtnStop_Click);
            // 
            // rtxStatus
            // 
            this.rtxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtxStatus.Font = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtxStatus.Location = new System.Drawing.Point(12, 220);
            this.rtxStatus.Name = "rtxStatus";
            this.rtxStatus.Size = new System.Drawing.Size(1011, 561);
            this.rtxStatus.TabIndex = 2;
            this.rtxStatus.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 191);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 26);
            this.label1.TabIndex = 3;
            this.label1.Text = "App Status:";
            // 
            // btnReprocessAll
            // 
            this.btnReprocessAll.BackColor = System.Drawing.Color.LimeGreen;
            this.btnReprocessAll.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnReprocessAll.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReprocessAll.ForeColor = System.Drawing.Color.White;
            this.btnReprocessAll.Location = new System.Drawing.Point(867, 15);
            this.btnReprocessAll.Margin = new System.Windows.Forms.Padding(6);
            this.btnReprocessAll.Name = "btnReprocessAll";
            this.btnReprocessAll.Size = new System.Drawing.Size(153, 73);
            this.btnReprocessAll.TabIndex = 4;
            this.btnReprocessAll.Text = "Reprocess All";
            this.btnReprocessAll.UseVisualStyleBackColor = false;
            this.btnReprocessAll.Click += new System.EventHandler(this.BtnReprocessAll_Click);
            // 
            // chkSaveToFileSystem
            // 
            this.chkSaveToFileSystem.AutoSize = true;
            this.chkSaveToFileSystem.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSaveToFileSystem.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSaveToFileSystem.Location = new System.Drawing.Point(27, 105);
            this.chkSaveToFileSystem.Name = "chkSaveToFileSystem";
            this.chkSaveToFileSystem.Size = new System.Drawing.Size(283, 27);
            this.chkSaveToFileSystem.TabIndex = 5;
            this.chkSaveToFileSystem.Text = "Save Json Datasets to File System";
            this.chkSaveToFileSystem.UseVisualStyleBackColor = true;
            // 
            // txtSavePath
            // 
            this.txtSavePath.Location = new System.Drawing.Point(115, 138);
            this.txtSavePath.Name = "txtSavePath";
            this.txtSavePath.Size = new System.Drawing.Size(695, 34);
            this.txtSavePath.TabIndex = 6;
            this.txtSavePath.Text = "C:\\git\\VeilStats\\";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(23, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "Save Path:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(650, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(198, 23);
            this.label3.TabIndex = 8;
            this.label3.Text = "Delete and re-sync block:";
            // 
            // txtResyncBlock
            // 
            this.txtResyncBlock.Location = new System.Drawing.Point(652, 47);
            this.txtResyncBlock.Name = "txtResyncBlock";
            this.txtResyncBlock.Size = new System.Drawing.Size(201, 34);
            this.txtResyncBlock.TabIndex = 9;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1035, 793);
            this.Controls.Add(this.txtResyncBlock);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSavePath);
            this.Controls.Add(this.chkSaveToFileSystem);
            this.Controls.Add(this.btnReprocessAll);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.rtxStatus);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnStart);
            this.Font = new System.Drawing.Font("Comic Sans MS", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Veil Block to Database";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.RichTextBox rtxStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnReprocessAll;
        private System.Windows.Forms.CheckBox chkSaveToFileSystem;
        private System.Windows.Forms.TextBox txtSavePath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtResyncBlock;
    }
}

