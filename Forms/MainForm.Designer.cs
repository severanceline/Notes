namespace Notes.Forms
{
    partial class MainForm
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
            panel1 = new Panel();
            btnLogout = new Button();
            txtSearch = new TextBox();
            btnCreatNote = new Button();
            lblWelcome = new Label();

            pnlLeft = new Panel();
            pnlFilters = new Panel();
            lblFilterLabels = new Label();
            clbFilterLabels = new CheckedListBox();
            btnClearLabelFilters = new Button();
            flpSidebarLabels = new FlowLayoutPanel();
            btnAddLabel = new Button();

            flpNotes = new FlowLayoutPanel();

            panel1.SuspendLayout();
            pnlLeft.SuspendLayout();
            pnlFilters.SuspendLayout();
            SuspendLayout();

            // =========================
            // panel1
            // =========================
            panel1.BackColor = Color.LightGray;
            panel1.Controls.Add(btnLogout);
            panel1.Controls.Add(txtSearch);
            panel1.Controls.Add(btnCreatNote);
            panel1.Controls.Add(lblWelcome);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1000, 60);
            panel1.TabIndex = 0;

            // =========================
            // btnLogout
            // =========================
            btnLogout.Location = new Point(285, 18);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(80, 25);
            btnLogout.TabIndex = 0;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click_1;

            // =========================
            // txtSearch
            // =========================
            txtSearch.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Left |
                AnchorStyles.Right;

            txtSearch.Location = new Point(375, 19);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(500, 23);
            txtSearch.TabIndex = 1;

            // =========================
            // btnCreatNote
            // =========================
            btnCreatNote.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Right;

            btnCreatNote.Location = new Point(900, 0);
            btnCreatNote.Name = "btnCreatNote";
            btnCreatNote.Size = new Size(100, 60);
            btnCreatNote.TabIndex = 2;
            btnCreatNote.Text = "Add Note";
            btnCreatNote.UseVisualStyleBackColor = true;
            btnCreatNote.Click += btnCreateNote_Click;

            // =========================
            // lblWelcome
            // =========================
            lblWelcome.AutoSize = true;
            lblWelcome.Font = new Font(
                "Segoe UI",
                11.25F,
                FontStyle.Bold,
                GraphicsUnit.Point,
                0);

            lblWelcome.Location = new Point(10, 20);
            lblWelcome.Name = "lblWelcome";
            lblWelcome.Size = new Size(74, 20);
            lblWelcome.TabIndex = 3;
            lblWelcome.Text = "Welcome";

            // =========================
            // pnlLeft
            // =========================
            pnlLeft.BackColor = Color.WhiteSmoke;
            pnlLeft.Controls.Add(flpSidebarLabels);
            pnlLeft.Controls.Add(btnAddLabel);
            pnlLeft.Controls.Add(pnlFilters);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 60);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new Size(240, 497);
            pnlLeft.TabIndex = 1;

            // =========================
            // pnlFilters
            // =========================
            pnlFilters.Controls.Add(lblFilterLabels);
            pnlFilters.Controls.Add(clbFilterLabels);
            pnlFilters.Controls.Add(btnClearLabelFilters);
            pnlFilters.Dock = DockStyle.Top;
            pnlFilters.Location = new Point(0, 0);
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Size = new Size(240, 190);
            pnlFilters.TabIndex = 0;

            // =========================
            // lblFilterLabels
            // =========================
            lblFilterLabels.AutoSize = true;
            lblFilterLabels.Font = new Font(
                "Segoe UI",
                9F,
                FontStyle.Bold,
                GraphicsUnit.Point,
                0);

            lblFilterLabels.Location = new Point(7, 7);
            lblFilterLabels.Name = "lblFilterLabels";
            lblFilterLabels.Size = new Size(188, 15);
            lblFilterLabels.TabIndex = 0;
            lblFilterLabels.Text = "Filter Labels (must match all)";

            // =========================
            // clbFilterLabels
            // =========================
            clbFilterLabels.BorderStyle = BorderStyle.FixedSingle;
            clbFilterLabels.CheckOnClick = true;
            clbFilterLabels.FormattingEnabled = true;
            clbFilterLabels.IntegralHeight = false;
            clbFilterLabels.Location = new Point(7, 31);
            clbFilterLabels.Name = "clbFilterLabels";
            clbFilterLabels.Size = new Size(226, 118);
            clbFilterLabels.TabIndex = 1;

            // =========================
            // btnClearLabelFilters
            // =========================
            btnClearLabelFilters.Location = new Point(7, 156);
            btnClearLabelFilters.Name = "btnClearLabelFilters";
            btnClearLabelFilters.Size = new Size(226, 27);
            btnClearLabelFilters.TabIndex = 2;
            btnClearLabelFilters.Text = "Clear Label Filters (All Labels)";
            btnClearLabelFilters.UseVisualStyleBackColor = true;

            // =========================
            // btnAddLabel
            // =========================
            btnAddLabel.Dock = DockStyle.Bottom;
            btnAddLabel.Location = new Point(0, 467);
            btnAddLabel.Name = "btnAddLabel";
            btnAddLabel.Size = new Size(240, 30);
            btnAddLabel.TabIndex = 1;
            btnAddLabel.Text = "+ Add Label";
            btnAddLabel.UseVisualStyleBackColor = true;
            btnAddLabel.Click += btnAddLabel_Click;

            // =========================
            // flpSidebarLabels
            // =========================
            flpSidebarLabels.AutoScroll = true;
            flpSidebarLabels.Dock = DockStyle.Fill;
            flpSidebarLabels.FlowDirection = FlowDirection.TopDown;
            flpSidebarLabels.Location = new Point(0, 190);
            flpSidebarLabels.Name = "flpSidebarLabels";
            flpSidebarLabels.Padding = new Padding(4);
            flpSidebarLabels.Size = new Size(240, 277);
            flpSidebarLabels.TabIndex = 2;
            flpSidebarLabels.WrapContents = false;
            flpSidebarLabels.Paint += flpSidebarLabels_Paint;

            // =========================
            // flpNotes
            // =========================
            flpNotes.AutoScroll = true;
            flpNotes.Dock = DockStyle.Fill;
            flpNotes.Location = new Point(240, 60);
            flpNotes.Name = "flpNotes";
            flpNotes.Size = new Size(760, 497);
            flpNotes.TabIndex = 2;

            // =========================
            // MainForm
            // =========================
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 557);

            Controls.Add(flpNotes);
            Controls.Add(pnlLeft);
            Controls.Add(panel1);

            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainForm";
            Load += MainForm_Load;

            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            pnlLeft.ResumeLayout(false);
            pnlFilters.ResumeLayout(false);
            pnlFilters.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button btnLogout;
        private TextBox txtSearch;
        private Button btnCreatNote;
        private Label lblWelcome;

        private Panel pnlLeft;
        private Panel pnlFilters;
        private Label lblFilterLabels;
        private CheckedListBox clbFilterLabels;
        private Button btnClearLabelFilters;
        private FlowLayoutPanel flpSidebarLabels;
        private Button btnAddLabel;

        private FlowLayoutPanel flpNotes;
    }
}