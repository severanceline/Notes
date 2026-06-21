using Noots.Models;

namespace Noots.UserControls
{
    partial class NoteCardControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlCard = new Panel();
            flpImagePreview = new FlowLayoutPanel();
            flpLabels = new FlowLayoutPanel();
            lblContent = new Label();
            lblTitle = new Label();
            pnlCard.SuspendLayout();
            SuspendLayout();
            // 
            // pnlCard
            // 
            pnlCard.BackColor = Color.White;
            pnlCard.BorderStyle = BorderStyle.FixedSingle;
            pnlCard.CausesValidation = false;
            pnlCard.Controls.Add(flpImagePreview);
            pnlCard.Controls.Add(flpLabels);
            pnlCard.Controls.Add(lblContent);
            pnlCard.Controls.Add(lblTitle);
            pnlCard.Cursor = Cursors.Hand;
            pnlCard.Dock = DockStyle.Fill;
            pnlCard.Location = new Point(0, 0);
            pnlCard.Name = "pnlCard";
            pnlCard.Size = new Size(300, 170);
            pnlCard.TabIndex = 0;
            // 
            // flpImagePreview
            // 
            flpImagePreview.Dock = DockStyle.Bottom;
            flpImagePreview.Location = new Point(0, 88);
            flpImagePreview.Name = "flpImagePreview";
            flpImagePreview.Size = new Size(298, 50);
            flpImagePreview.TabIndex = 3;
            flpImagePreview.WrapContents = false;
            // 
            // flpLabels
            // 
            flpLabels.Dock = DockStyle.Bottom;
            flpLabels.Location = new Point(0, 138);
            flpLabels.Name = "flpLabels";
            flpLabels.Size = new Size(298, 30);
            flpLabels.TabIndex = 2;
            // 
            // lblContent
            // 
            lblContent.AutoSize = true;
            lblContent.Dock = DockStyle.Fill;
            lblContent.ForeColor = Color.Gray;
            lblContent.Location = new Point(0, 17);
            lblContent.Name = "lblContent";
            lblContent.Size = new Size(38, 15);
            lblContent.TabIndex = 1;
            lblContent.Text = "label1";
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(0, 17);
            lblTitle.TabIndex = 0;
            // 
            // NoteCardControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlCard);
            Name = "NoteCardControl";
            Size = new Size(300, 170);
            pnlCard.ResumeLayout(false);
            pnlCard.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlCard;
        private Label lblTitle;
        private FlowLayoutPanel flpLabels;
        private Label lblContent;
        private FlowLayoutPanel flpImagePreview;
    }
}
