using Microsoft.IdentityModel.Tokens.Experimental;
using Noots.DataAccess;
using Noots.Models;
using Noots.Utilities;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Noots.UserControls
{
    public partial class NoteCardControl : UserControl
    {
        public Note NoteData { get; private set; }

        public event EventHandler NoteClicked;

        public NoteCardControl(Note note)
        {
            InitializeComponent();

            NoteData = note;

            BuildUI();
        }

        private void BuildUI()
        {
            lblTitle.Text = NoteData.NoteInfo.Title;
            lblContent.Text =
                (NoteData.NoteInfo.Content ?? "").Length > 100
                ? NoteData.NoteInfo.Content.Substring(0, 100) + "..."
                : NoteData.NoteInfo.Content;

            RenderImages();
            RenderLabels();

            this.Click += Card_Click;
            pnlCard.Click += Card_Click;
            lblTitle.Click += Card_Click;
            lblContent.Click += Card_Click;
        }

        // =========================
        // IMAGE PREVIEW (FIRST 3)
        // =========================
        private void RenderImages()
        {
            flpImagePreview.Controls.Clear();

            var images = NoteData.noteImages?.Take(3).ToList();

            if (images == null) return;

            foreach (var img in images)
            {
                PictureBox pic = new    PictureBox
                {
                    Width = 40,
                    Height = 40,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Margin = new Padding(2),
                    BorderStyle = BorderStyle.FixedSingle
                };

                string path = ImageStorageHelper.GetFullPath(img.ImagePath);

                pic.ImageLocation = path;

                flpImagePreview.Controls.Add(pic);
            }
        }

        // =========================
        // LABELS
        // =========================
        private void RenderLabels()
        {
            flpLabels.Controls.Clear();
            var labelRepo = new LabelRepository();
            if(labelRepo.GetLabelsForNote(NoteData.NoteInfo.Id) == null) return;
            labelRepo.GetLabelsForNote(NoteData.NoteInfo.Id).ForEach(label =>
            {
                Label chip = new Label
                {
                    Text = label.Name,
                    AutoSize = true,
                    BackColor = Color.LightSkyBlue,
                    Padding = new Padding(3),
                    Margin = new Padding(2)
                };
                flpLabels.Controls.Add(chip);
            });
        }

        // =========================
        // CLICK EVENT
        // =========================
        private void Card_Click(object sender, EventArgs e)
        {
            NoteClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}