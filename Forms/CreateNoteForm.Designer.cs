namespace Noots.Forms
{
    partial class CreateNoteForm
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
            pnlHeader = new Panel();
            btnClose = new Button();
            btnSave = new Button();
            txtTitle = new TextBox();
            txtContent = new TextBox();
            pnlImages = new Panel();
            flpImages = new FlowLayoutPanel();
            btnAddImage = new Button();
            pnlLabels = new Panel();
            flpLabels = new FlowLayoutPanel();
            btnNewLabel = new Button();
            btnAddLabel = new Button();
            cmbLabels = new ComboBox();
            pnlHeader.SuspendLayout();
            pnlImages.SuspendLayout();
            pnlLabels.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.LightGray;
            pnlHeader.Controls.Add(btnClose);
            pnlHeader.Controls.Add(btnSave);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(371, 50);
            pnlHeader.TabIndex = 0;
            // 
            // btnClose
            // 
            btnClose.Dock = DockStyle.Right;
            btnClose.Location = new Point(291, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(80, 50);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // btnSave
            // 
            btnSave.Dock = DockStyle.Left;
            btnSave.Location = new Point(0, 0);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(80, 50);
            btnSave.TabIndex = 0;
            btnSave.Text = "Save";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // txtTitle
            // 
            txtTitle.Dock = DockStyle.Top;
            txtTitle.Font = new Font("Segoe UI", 11.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtTitle.Location = new Point(0, 50);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(371, 27);
            txtTitle.TabIndex = 1;
            // 
            // txtContent
            // 
            txtContent.Dock = DockStyle.Fill;
            txtContent.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtContent.Location = new Point(0, 77);
            txtContent.Multiline = true;
            txtContent.Name = "txtContent";
            txtContent.ScrollBars = ScrollBars.Vertical;
            txtContent.Size = new Size(371, 498);
            txtContent.TabIndex = 2;
            // 
            // pnlImages
            // 
            pnlImages.BackColor = Color.WhiteSmoke;
            pnlImages.Controls.Add(btnAddImage);
            pnlImages.Controls.Add(flpImages);
            pnlImages.Dock = DockStyle.Bottom;
            pnlImages.Location = new Point(0, 295);
            pnlImages.Name = "pnlImages";
            pnlImages.Size = new Size(371, 160);
            pnlImages.TabIndex = 0;
            // 
            // flpImages
            // 
            flpImages.AutoScroll = true;
            flpImages.Dock = DockStyle.Bottom;
            flpImages.Location = new Point(0, 36);
            flpImages.Name = "flpImages";
            flpImages.Size = new Size(371, 124);
            flpImages.TabIndex = 0;
            // 
            // btnAddImage
            // 
            btnAddImage.Dock = DockStyle.Top;
            btnAddImage.Location = new Point(0, 0);
            btnAddImage.Name = "btnAddImage";
            btnAddImage.Size = new Size(371, 30);
            btnAddImage.TabIndex = 0;
            btnAddImage.Text = "+ Add Image";
            btnAddImage.UseVisualStyleBackColor = true;
            btnAddImage.Click += btnAddImage_Click;
            // 
            // pnlLabels
            // 
            pnlLabels.Controls.Add(flpLabels);
            pnlLabels.Controls.Add(btnNewLabel);
            pnlLabels.Controls.Add(btnAddLabel);
            pnlLabels.Controls.Add(cmbLabels);
            pnlLabels.Dock = DockStyle.Bottom;
            pnlLabels.Location = new Point(0, 455);
            pnlLabels.Name = "pnlLabels";
            pnlLabels.Size = new Size(371, 120);
            pnlLabels.TabIndex = 1;
            // 
            // flpLabels
            // 
            flpLabels.AutoScroll = true;
            flpLabels.Dock = DockStyle.Fill;
            flpLabels.Location = new Point(0, 83);
            flpLabels.Name = "flpLabels";
            flpLabels.Size = new Size(371, 37);
            flpLabels.TabIndex = 3;
            // 
            // btnNewLabel
            // 
            btnNewLabel.Dock = DockStyle.Top;
            btnNewLabel.Location = new Point(0, 53);
            btnNewLabel.Name = "btnNewLabel";
            btnNewLabel.Size = new Size(371, 30);
            btnNewLabel.TabIndex = 2;
            btnNewLabel.Text = "+ New Label";
            btnNewLabel.UseVisualStyleBackColor = true;
            btnNewLabel.Click += btnNewLabel_Click;
            // 
            // btnAddLabel
            // 
            btnAddLabel.Dock = DockStyle.Top;
            btnAddLabel.Location = new Point(0, 23);
            btnAddLabel.Name = "btnAddLabel";
            btnAddLabel.Size = new Size(371, 30);
            btnAddLabel.TabIndex = 1;
            btnAddLabel.Text = "Add Label";
            btnAddLabel.UseVisualStyleBackColor = true;
            btnAddLabel.Click += btnAddLabel_Click;
            // 
            // cmbLabels
            // 
            cmbLabels.Dock = DockStyle.Top;
            cmbLabels.FormattingEnabled = true;
            cmbLabels.Location = new Point(0, 0);
            cmbLabels.Name = "cmbLabels";
            cmbLabels.Size = new Size(371, 23);
            cmbLabels.TabIndex = 0;
            // 
            // CreateNoteForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(371, 575);
            Controls.Add(pnlImages);
            Controls.Add(pnlLabels);
            Controls.Add(txtContent);
            Controls.Add(txtTitle);
            Controls.Add(pnlHeader);
            Name = "CreateNoteForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CreateNoteForm";
            pnlHeader.ResumeLayout(false);
            pnlImages.ResumeLayout(false);
            pnlLabels.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlHeader;
        private Button btnClose;
        private Button btnSave;
        private TextBox txtTitle;
        private TextBox txtContent;
        private Panel pnlImages;
        private FlowLayoutPanel flpImages;
        private Button btnAddImage;
        private Panel pnlLabels;
        private FlowLayoutPanel flpLabels;
        private Button btnNewLabel;
        private Button btnAddLabel;
        private ComboBox cmbLabels;
    }
}