namespace ServiceBusSimulator
{
    partial class ClearMessagesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClearMessagesForm));
            this.textBoxTopic = new System.Windows.Forms.TextBox();
            this.textBoxSubscription = new System.Windows.Forms.TextBox();
            this.buttonClearMessage = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.richTextBoxMessagesLog = new System.Windows.Forms.RichTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.textBoxMessageCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxDeletedMessages = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.imageListStatus = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxTopic
            // 
            this.textBoxTopic.Location = new System.Drawing.Point(174, 355);
            this.textBoxTopic.Name = "textBoxTopic";
            this.textBoxTopic.ReadOnly = true;
            this.textBoxTopic.Size = new System.Drawing.Size(305, 26);
            this.textBoxTopic.TabIndex = 19;
            // 
            // textBoxSubscription
            // 
            this.textBoxSubscription.Location = new System.Drawing.Point(174, 387);
            this.textBoxSubscription.Name = "textBoxSubscription";
            this.textBoxSubscription.ReadOnly = true;
            this.textBoxSubscription.Size = new System.Drawing.Size(305, 26);
            this.textBoxSubscription.TabIndex = 20;
            // 
            // buttonClearMessage
            // 
            this.buttonClearMessage.Image = ((System.Drawing.Image)(resources.GetObject("buttonClearMessage.Image")));
            this.buttonClearMessage.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonClearMessage.Location = new System.Drawing.Point(13, 528);
            this.buttonClearMessage.Name = "buttonClearMessage";
            this.buttonClearMessage.Size = new System.Drawing.Size(137, 108);
            this.buttonClearMessage.TabIndex = 18;
            this.buttonClearMessage.Text = "Clear Messages";
            this.buttonClearMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonClearMessage.UseVisualStyleBackColor = true;
            this.buttonClearMessage.Click += new System.EventHandler(this.buttonClearMessage_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 388);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 20);
            this.label3.TabIndex = 17;
            this.label3.Text = "Subscription:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 355);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 20);
            this.label2.TabIndex = 16;
            this.label2.Text = "Topic:";
            // 
            // buttonCancel
            // 
            this.buttonCancel.Image = ((System.Drawing.Image)(resources.GetObject("buttonCancel.Image")));
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonCancel.Location = new System.Drawing.Point(342, 528);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(137, 108);
            this.buttonCancel.TabIndex = 21;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Image = ((System.Drawing.Image)(resources.GetObject("buttonStop.Image")));
            this.buttonStop.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonStop.Location = new System.Drawing.Point(156, 528);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(137, 108);
            this.buttonStop.TabIndex = 22;
            this.buttonStop.Text = "Stop";
            this.buttonStop.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // richTextBoxMessagesLog
            // 
            this.richTextBoxMessagesLog.Location = new System.Drawing.Point(24, 12);
            this.richTextBoxMessagesLog.Name = "richTextBoxMessagesLog";
            this.richTextBoxMessagesLog.Size = new System.Drawing.Size(455, 315);
            this.richTextBoxMessagesLog.TabIndex = 24;
            this.richTextBoxMessagesLog.Text = "";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 655);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(504, 30);
            this.statusStrip1.TabIndex = 25;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripStatusLabel1.Image")));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(24, 25);
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(159, 25);
            this.toolStripStatusLabel.Text = "Clearing Messages";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 24);
            // 
            // textBoxMessageCount
            // 
            this.textBoxMessageCount.Location = new System.Drawing.Point(174, 422);
            this.textBoxMessageCount.Name = "textBoxMessageCount";
            this.textBoxMessageCount.ReadOnly = true;
            this.textBoxMessageCount.Size = new System.Drawing.Size(305, 26);
            this.textBoxMessageCount.TabIndex = 27;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 428);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 20);
            this.label1.TabIndex = 26;
            this.label1.Text = "Message Count:";
            // 
            // textBoxDeletedMessages
            // 
            this.textBoxDeletedMessages.Location = new System.Drawing.Point(174, 460);
            this.textBoxDeletedMessages.Name = "textBoxDeletedMessages";
            this.textBoxDeletedMessages.ReadOnly = true;
            this.textBoxDeletedMessages.Size = new System.Drawing.Size(305, 26);
            this.textBoxDeletedMessages.TabIndex = 29;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 463);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 20);
            this.label4.TabIndex = 28;
            this.label4.Text = "Deleted Messages:";
            // 
            // imageListStatus
            // 
            this.imageListStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListStatus.ImageStream")));
            this.imageListStatus.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListStatus.Images.SetKeyName(0, "48.ico");
            this.imageListStatus.Images.SetKeyName(1, "Stop.ico");
            this.imageListStatus.Images.SetKeyName(2, "Revert.ico");
            // 
            // ClearMessagesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(504, 685);
            this.Controls.Add(this.textBoxDeletedMessages);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxMessageCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.richTextBoxMessagesLog);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxTopic);
            this.Controls.Add(this.textBoxSubscription);
            this.Controls.Add(this.buttonClearMessage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Name = "ClearMessagesForm";
            this.Text = "ClearMessagesForm";
            this.Load += new System.EventHandler(this.ClearMessagesForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxTopic;
        private System.Windows.Forms.TextBox textBoxSubscription;
        private System.Windows.Forms.Button buttonClearMessage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.RichTextBox richTextBoxMessagesLog;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.TextBox textBoxMessageCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxDeletedMessages;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ImageList imageListStatus;
    }
}