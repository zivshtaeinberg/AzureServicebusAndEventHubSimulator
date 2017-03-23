namespace ServiceBusSimulator
{
    partial class MessageForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxMessage = new System.Windows.Forms.TextBox();
            this.textBoxMessageCount = new System.Windows.Forms.TextBox();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxProperty = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonImportaFile = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.imageListControls = new System.Windows.Forms.ImageList(this.components);
            this.pictureBoxPreview = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Message:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 508);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Count:";
            // 
            // textBoxMessage
            // 
            this.textBoxMessage.Location = new System.Drawing.Point(16, 32);
            this.textBoxMessage.Multiline = true;
            this.textBoxMessage.Name = "textBoxMessage";
            this.textBoxMessage.Size = new System.Drawing.Size(442, 248);
            this.textBoxMessage.TabIndex = 2;
            // 
            // textBoxMessageCount
            // 
            this.textBoxMessageCount.AccessibleRole = System.Windows.Forms.AccessibleRole.ScrollBar;
            this.textBoxMessageCount.Location = new System.Drawing.Point(96, 508);
            this.textBoxMessageCount.Name = "textBoxMessageCount";
            this.textBoxMessageCount.Size = new System.Drawing.Size(150, 26);
            this.textBoxMessageCount.TabIndex = 3;
            this.textBoxMessageCount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxMessageCount_KeyPress);
            this.textBoxMessageCount.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxMessageCount_KeyUp);
            // 
            // hScrollBar
            // 
            this.hScrollBar.LargeChange = 100;
            this.hScrollBar.Location = new System.Drawing.Point(249, 508);
            this.hScrollBar.Maximum = 100000;
            this.hScrollBar.Minimum = 1;
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(209, 26);
            this.hScrollBar.SmallChange = 10;
            this.hScrollBar.TabIndex = 4;
            this.hScrollBar.Value = 1;
            this.hScrollBar.ValueChanged += new System.EventHandler(this.hScrollBar_ValueChanged);
            this.hScrollBar.LocationChanged += new System.EventHandler(this.hScrollBar_LocationChanged);
            // 
            // buttonCreate
            // 
            this.buttonCreate.Image = ((System.Drawing.Image)(resources.GetObject("buttonCreate.Image")));
            this.buttonCreate.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonCreate.Location = new System.Drawing.Point(321, 550);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(137, 108);
            this.buttonCreate.TabIndex = 5;
            this.buttonCreate.Text = "Create";
            this.buttonCreate.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Image = ((System.Drawing.Image)(resources.GetObject("buttonCancel.Image")));
            this.buttonCancel.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonCancel.Location = new System.Drawing.Point(16, 550);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(137, 108);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // textBoxProperty
            // 
            this.textBoxProperty.Location = new System.Drawing.Point(96, 469);
            this.textBoxProperty.Name = "textBoxProperty";
            this.textBoxProperty.Size = new System.Drawing.Size(362, 26);
            this.textBoxProperty.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 469);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Property:";
            // 
            // buttonImportaFile
            // 
            this.buttonImportaFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.buttonImportaFile.Image = ((System.Drawing.Image)(resources.GetObject("buttonImportaFile.Image")));
            this.buttonImportaFile.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.buttonImportaFile.Location = new System.Drawing.Point(16, 294);
            this.buttonImportaFile.Name = "buttonImportaFile";
            this.buttonImportaFile.Size = new System.Drawing.Size(137, 108);
            this.buttonImportaFile.TabIndex = 9;
            this.buttonImportaFile.Text = "Search";
            this.buttonImportaFile.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonImportaFile.UseVisualStyleBackColor = true;
            this.buttonImportaFile.Click += new System.EventHandler(this.buttonImportaFile_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Menu;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(169, 314);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(272, 98);
            this.textBox1.TabIndex = 11;
            this.textBox1.Text = "Please type your message manuely or click on the Import button and upload your fi" +
    "le.";
            // 
            // imageListControls
            // 
            this.imageListControls.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListControls.ImageStream")));
            this.imageListControls.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListControls.Images.SetKeyName(0, "23.png");
            this.imageListControls.Images.SetKeyName(1, "new_post-48.png");
            // 
            // pictureBoxPreview
            // 
            this.pictureBoxPreview.Location = new System.Drawing.Point(496, 32);
            this.pictureBoxPreview.Name = "pictureBoxPreview";
            this.pictureBoxPreview.Size = new System.Drawing.Size(550, 614);
            this.pictureBoxPreview.TabIndex = 12;
            this.pictureBoxPreview.TabStop = false;
            // 
            // MessageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 670);
            this.Controls.Add(this.pictureBoxPreview);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.buttonImportaFile);
            this.Controls.Add(this.textBoxProperty);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonCreate);
            this.Controls.Add(this.hScrollBar);
            this.Controls.Add(this.textBoxMessageCount);
            this.Controls.Add(this.textBoxMessage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MessageForm";
            this.Text = "MessageForm";
            this.Load += new System.EventHandler(this.MessageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxMessage;
        private System.Windows.Forms.TextBox textBoxMessageCount;
        private System.Windows.Forms.HScrollBar hScrollBar;
        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxProperty;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonImportaFile;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ImageList imageListControls;
        private System.Windows.Forms.PictureBox pictureBoxPreview;
    }
}