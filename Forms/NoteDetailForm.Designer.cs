namespace Noots.Forms
{
    partial class NoteDetailForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            pnlHeader = new Panel();
            btnClose = new Button();
            btnDelete = new Button();
            txtTitle = new TextBox();
            txtContent = new TextBox();
            pnlImages = new Panel();
            flpImages = new FlowLayoutPanel();
            btnAddImage = new Button();
            pnlLabels = new Panel();
            cmbLabels = new ComboBox();
            btnAddLabel = new Button();
            btnNewLabel = new Button();
            flpLabels = new FlowLayoutPanel();

            pnlHeader.SuspendLayout();
            pnlImages.SuspendLayout();
            flpImages.SuspendLayout();
            pnlLabels.SuspendLayout();
            SuspendLayout();

            // =========================
            // pnlHeader
            // =========================
            pnlHeader.Controls.Add(btnClose);
            pnlHeader.Controls.Add(btnDelete);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(386, 60);
            pnlHeader.TabIndex = 0;

            // =========================
            // btnClose
            // =========================
            btnClose.Dock = DockStyle.Left;
            btnClose.Location = new Point(0, 0);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(75, 60);
            btnClose.TabIndex = 0;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;

            // =========================
            // btnDelete
            // =========================
            btnDelete.Dock = DockStyle.Right;
            btnDelete.Location = new Point(311, 0);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(75, 60);
            btnDelete.TabIndex = 1;
            btnDelete.Text = "Delete";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;

            // =========================
            // txtTitle
            // =========================
            txtTitle.Dock = DockStyle.Top;
            txtTitle.Font = new Font(
                "Segoe UI",
                10F,
                FontStyle.Bold,
                GraphicsUnit.Point,
                0);

            txtTitle.Location = new Point(0, 60);
            txtTitle.Name = "txtTitle";
            txtTitle.Size = new Size(386, 25);
            txtTitle.TabIndex = 2;

            // =========================
            // txtContent
            // =========================
            txtContent.Dock = DockStyle.Fill;
            txtContent.Font = new Font(
                "Segoe UI",
                9F,
                FontStyle.Regular,
                GraphicsUnit.Point,
                0);

            txtContent.Location = new Point(0, 85);
            txtContent.Multiline = true;
            txtContent.Name = "txtContent";
            txtContent.ScrollBars = ScrollBars.Vertical;
            txtContent.Size = new Size(386, 416);
            txtContent.TabIndex = 3;

            // =========================
            // pnlImages
            // =========================
            pnlImages.Controls.Add(flpImages);
            pnlImages.Dock = DockStyle.Bottom;
            pnlImages.Location = new Point(0, 201);
            pnlImages.Name = "pnlImages";
            pnlImages.Size = new Size(386, 180);
            pnlImages.TabIndex = 4;

            // =========================
            // flpImages
            // =========================
            flpImages.AutoScroll = true;
            flpImages.Controls.Add(btnAddImage);
            flpImages.Dock = DockStyle.Fill;
            flpImages.Location = new Point(0, 0);
            flpImages.Name = "flpImages";
            flpImages.Size = new Size(386, 180);
            flpImages.TabIndex = 0;

            // =========================
            // btnAddImage
            // =========================
            btnAddImage.Location = new Point(4, 4);
            btnAddImage.Margin = new Padding(4);
            btnAddImage.Name = "btnAddImage";
            btnAddImage.Size = new Size(108, 32);
            btnAddImage.TabIndex = 0;
            btnAddImage.Text = "+ Add Image";
            btnAddImage.UseVisualStyleBackColor = true;
            btnAddImage.Click += btnAddImage_Click;

            // =========================
            // pnlLabels
            // =========================
            pnlLabels.Controls.Add(cmbLabels);
            pnlLabels.Controls.Add(btnAddLabel);
            pnlLabels.Controls.Add(btnNewLabel);
            pnlLabels.Controls.Add(flpLabels);
            pnlLabels.Dock = DockStyle.Bottom;
            pnlLabels.Location = new Point(0, 381);
            pnlLabels.Name = "pnlLabels";
            pnlLabels.Size = new Size(386, 120);
            pnlLabels.TabIndex = 5;

            // =========================
            // cmbLabels
            // =========================
            cmbLabels.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLabels.FormattingEnabled = true;
            cmbLabels.Location = new Point(3, 6);
            cmbLabels.Name = "cmbLabels";
            cmbLabels.Size = new Size(260, 23);
            cmbLabels.TabIndex = 0;

            // =========================
            // btnAddLabel
            // =========================
            btnAddLabel.Location = new Point(269, 5);
            btnAddLabel.Name = "btnAddLabel";
            btnAddLabel.Size = new Size(114, 25);
            btnAddLabel.TabIndex = 1;
            btnAddLabel.Text = "+ Add Label";
            btnAddLabel.UseVisualStyleBackColor = true;
            btnAddLabel.Click += btnAddLabel_Click;

            // =========================
            // btnNewLabel
            // =========================
            btnNewLabel.Location = new Point(3, 36);
            btnNewLabel.Name = "btnNewLabel";
            btnNewLabel.Size = new Size(110, 26);
            btnNewLabel.TabIndex = 2;
            btnNewLabel.Text = "+ New Label";
            btnNewLabel.UseVisualStyleBackColor = true;
            btnNewLabel.Click += btnNewLabel_Click;

            // =========================
            // flpLabels
            // =========================
            flpLabels.AutoScroll = true;
            flpLabels.Dock = DockStyle.Bottom;
            flpLabels.FlowDirection = FlowDirection.LeftToRight;
            flpLabels.Location = new Point(0, 76);
            flpLabels.Name = "flpLabels";
            flpLabels.Size = new Size(386, 44);
            flpLabels.TabIndex = 3;
            flpLabels.WrapContents = false;

            // =========================
            // NoteDetailForm
            // =========================
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(386, 501);

            Controls.Add(pnlImages);
            Controls.Add(pnlLabels);
            Controls.Add(txtContent);
            Controls.Add(txtTitle);
            Controls.Add(pnlHeader);

            MinimumSize = new Size(402, 540);
            Name = "NoteDetailForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Note Details";

            pnlHeader.ResumeLayout(false);
            pnlImages.ResumeLayout(false);
            flpImages.ResumeLayout(false);
            pnlLabels.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlHeader;
        private Button btnClose;
        private Button btnDelete;

        private TextBox txtTitle;
        private TextBox txtContent;

        private Panel pnlImages;
        private FlowLayoutPanel flpImages;
        private Button btnAddImage;

        private Panel pnlLabels;
        private ComboBox cmbLabels;
        private Button btnAddLabel;
        private Button btnNewLabel;
        private FlowLayoutPanel flpLabels;
    }
}