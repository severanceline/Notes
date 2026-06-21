using Noots.DataAccess;
using Noots.Models;
using Noots.Session;
using Noots.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Noots.Forms
{
    public partial class CreateNoteForm : Form
    {
        private readonly NoteRepository _noteRepo;
        private readonly LabelRepository _labelRepo;

        // Temporary images. They are not saved until the user clicks Save.
        private readonly List<NoteImage> _tempImages =
            new List<NoteImage>();

        // Temporary labels. They are not saved until the user clicks Save.
        private readonly List<LabelModel> _tempLabels =
            new List<LabelModel>();

        // IDs of labels created temporarily in this form.
        private readonly HashSet<Guid> _newTemporaryLabelIds =
            new HashSet<Guid>();

        // Prevents saving the same label connection more than once
        // when the user retries after a partial save error.
        private readonly HashSet<Guid> _linkedTemporaryLabelIds =
            new HashSet<Guid>();

        private readonly ToolTip _toolTip = new ToolTip();

        private bool _isSaving;

        // These fields are used only if the note was saved successfully
        // but saving one or more labels failed.
        private bool _noteAlreadySaved;
        private Guid _savedNoteId;

        public CreateNoteForm()
        {
            InitializeComponent();

            _noteRepo = new NoteRepository();
            _labelRepo = new LabelRepository();

            LoadComboLabels();

            FormClosed += CreateNoteForm_FormClosed;
        }

        // =========================
        // LOAD LABEL COMBOBOX
        // =========================
        private void LoadComboLabels()
        {
            if (UserSession.CurrentUser == null)
            {
                return;
            }

            List<LabelModel> labels =
                _labelRepo.GetLabelsByUserId(UserSession.CurrentUser.Id);

            cmbLabels.DataSource = null;
            cmbLabels.DataSource = labels;
            cmbLabels.DisplayMember = "Name";
            cmbLabels.ValueMember = "Id";
            cmbLabels.SelectedIndex = -1;
        }

        // =========================
        // ADD EXISTING LABEL
        // =========================
        private void btnAddLabel_Click(object sender, EventArgs e)
        {
            LabelModel selectedLabel =
                cmbLabels.SelectedItem as LabelModel;

            if (selectedLabel == null)
            {
                MessageBox.Show(
                    "Please select a label first.",
                    "Label",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            bool alreadyAdded = _tempLabels.Any(x =>
                x.Id == selectedLabel.Id);

            if (alreadyAdded)
            {
                MessageBox.Show(
                    "This label has already been added.",
                    "Label",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            _tempLabels.Add(selectedLabel);

            RenderLabels();

            cmbLabels.SelectedIndex = -1;
        }

        // =========================
        // CREATE NEW TEMPORARY LABEL
        // =========================
        private void btnNewLabel_Click(object sender, EventArgs e)
        {
            if (UserSession.CurrentUser == null)
            {
                return;
            }

            string name = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter a name for the new label:",
                "New Label");

            if (string.IsNullOrWhiteSpace(name))
            {
                return;
            }

            string normalizedName = name.Trim();

            // Check labels that already exist for the current user.
            List<LabelModel> existingLabels =
                _labelRepo.GetLabelsByUserId(UserSession.CurrentUser.Id);

            bool labelAlreadyExistsInDatabase = existingLabels.Any(x =>
                string.Equals(
                    x.Name?.Trim(),
                    normalizedName,
                    StringComparison.OrdinalIgnoreCase));

            if (labelAlreadyExistsInDatabase)
            {
                MessageBox.Show(
                    "A label with this name already exists. Please select it from the label list.",
                    "Duplicate Label",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            // Check labels that were added temporarily in this form.
            bool labelAlreadyExistsTemporarily = _tempLabels.Any(x =>
                string.Equals(
                    x.Name?.Trim(),
                    normalizedName,
                    StringComparison.OrdinalIgnoreCase));

            if (labelAlreadyExistsTemporarily)
            {
                MessageBox.Show(
                    "This label has already been added.",
                    "Duplicate Label",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            LabelModel newLabel = new LabelModel
            {
                Id = Guid.NewGuid(),
                Name = normalizedName
            };

            // This label is only temporary until Save is clicked.
            _tempLabels.Add(newLabel);
            _newTemporaryLabelIds.Add(newLabel.Id);

            RenderLabels();
        }

        // =========================
        // RENDER TEMPORARY LABELS
        // =========================
        private void RenderLabels()
        {
            while (flpLabels.Controls.Count > 0)
            {
                Control control = flpLabels.Controls[0];

                flpLabels.Controls.RemoveAt(0);
                control.Dispose();
            }

            foreach (LabelModel label in _tempLabels)
            {
                Panel chip = CreateLabelChip(label);

                flpLabels.Controls.Add(chip);
            }
        }

        private Panel CreateLabelChip(LabelModel label)
        {
            Size textSize = TextRenderer.MeasureText(
                label.Name ?? string.Empty,
                txtContent.Font);

            int chipWidth = Math.Max(
                90,
                Math.Min(230, textSize.Width + 42));

            Panel chip = new Panel
            {
                Width = chipWidth,
                Height = 30,
                BackColor = Color.LightSkyBlue,
                Margin = new Padding(3)
            };

            Label text = new Label
            {
                Text = label.Name,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 25, 0),
                AutoEllipsis = true
            };

            Button deleteButton = CreateDeleteButton(
                "Remove this label before saving.");

            deleteButton.Location = new Point(chip.Width - 26, 3);
            deleteButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            deleteButton.Click += (sender, e) =>
            {
                _tempLabels.RemoveAll(x => x.Id == label.Id);

                _newTemporaryLabelIds.Remove(label.Id);
                _linkedTemporaryLabelIds.Remove(label.Id);

                RenderLabels();
            };

            chip.Controls.Add(text);
            chip.Controls.Add(deleteButton);

            deleteButton.BringToFront();

            AddDeleteHoverBehavior(
                chip,
                deleteButton,
                text,
                deleteButton);

            return chip;
        }

        // =========================
        // ADD TEMPORARY IMAGE
        // =========================
        private void btnAddImage_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                ofd.Multiselect = false;
                ofd.CheckFileExists = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                bool imageAlreadyAdded = _tempImages.Any(x =>
                    string.Equals(
                        x.ImagePath,
                        ofd.FileName,
                        StringComparison.OrdinalIgnoreCase));

                if (imageAlreadyAdded)
                {
                    MessageBox.Show(
                        "This image has already been added.",
                        "Image",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                _tempImages.Add(new NoteImage
                {
                    Id = Guid.NewGuid(),

                    // Original source path. The file is copied only after Save.
                    ImagePath = ofd.FileName
                });

                RenderImages();
            }
        }

        // =========================
        // RENDER TEMPORARY IMAGES
        // =========================
        private void RenderImages()
        {
            while (flpImages.Controls.Count > 0)
            {
                Control control = flpImages.Controls[0];

                flpImages.Controls.RemoveAt(0);
                control.Dispose();
            }

            foreach (NoteImage image in _tempImages)
            {
                Panel imageTile = CreateImageTile(image);

                flpImages.Controls.Add(imageTile);
            }
        }

        private Panel CreateImageTile(NoteImage tempImage)
        {
            Panel imageTile = new Panel
            {
                Width = 124,
                Height = 124,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(4)
            };

            PictureBox picture = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            Image image = TryLoadImageWithoutLock(tempImage.ImagePath);

            if (image != null)
            {
                picture.Image = image;
            }
            else
            {
                picture.BackColor = Color.Gainsboro;

                _toolTip.SetToolTip(
                    picture,
                    "This image file could not be found or opened.");
            }

            picture.Disposed += (sender, e) =>
            {
                if (picture.Image != null)
                {
                    Image loadedImage = picture.Image;

                    picture.Image = null;
                    loadedImage.Dispose();
                }
            };

            Button deleteButton = CreateDeleteButton(
                "Remove this image before saving.");

            deleteButton.Location = new Point(96, 3);
            deleteButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            deleteButton.Click += (sender, e) =>
            {
                _tempImages.Remove(tempImage);

                RenderImages();
            };

            imageTile.Controls.Add(picture);
            imageTile.Controls.Add(deleteButton);

            deleteButton.BringToFront();

            AddDeleteHoverBehavior(
                imageTile,
                deleteButton,
                picture,
                deleteButton);

            return imageTile;
        }

        // =========================
        // LOAD IMAGE WITHOUT LOCKING FILE
        // =========================
        private Image TryLoadImageWithoutLock(string fullPath)
        {
            try
            {
                if (!File.Exists(fullPath))
                {
                    return null;
                }

                using (FileStream stream = new FileStream(
                    fullPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite))
                {
                    using (Image source = Image.FromStream(stream))
                    {
                        // Clone the image so the source file is not locked.
                        return new Bitmap(source);
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        // =========================
        // SAVE NOTE
        // =========================
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_isSaving)
            {
                return;
            }

            if (UserSession.CurrentUser == null)
            {
                MessageBox.Show(
                    "No signed-in user was found.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            // If the note was created previously but saving labels failed,
            // this Save click only retries the unsaved labels.
            if (_noteAlreadySaved)
            {
                RetryPendingLabels();

                return;
            }

            bool hasTitle = !string.IsNullOrWhiteSpace(txtTitle.Text);
            bool hasContent = !string.IsNullOrWhiteSpace(txtContent.Text);
            bool hasLabels = _tempLabels.Count > 0;
            bool hasImages = _tempImages.Count > 0;

            // At least one item is required to create a note.
            if (!hasTitle && !hasContent && !hasLabels && !hasImages)
            {
                MessageBox.Show(
                    "Please add a title, content, at least one label, or at least one image before saving.",
                    "Nothing to Save",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            _isSaving = true;
            btnSave.Enabled = false;

            List<string> copiedImagePaths = new List<string>();

            try
            {
                Guid noteId = Guid.NewGuid();

                string title = string.IsNullOrWhiteSpace(txtTitle.Text)
                    ? string.Empty
                    : txtTitle.Text.Trim();

                string content = string.IsNullOrWhiteSpace(txtContent.Text)
                    ? string.Empty
                    : txtContent.Text;

                Note note = new Note
                {
                    NoteInfo = new NoteInfo
                    {
                        Id = noteId,
                        UserId = UserSession.CurrentUser.Id,
                        Title = title,
                        Content = content,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },

                    noteImages = new List<NoteImage>()
                };

                // Copy images and prepare image records.
                foreach (NoteImage tempImage in _tempImages)
                {
                    string relativePath = ImageStorageHelper.SaveImage(
                        noteId,
                        tempImage.ImagePath);

                    copiedImagePaths.Add(relativePath);

                    note.noteImages.Add(new NoteImage
                    {
                        Id = Guid.NewGuid(),
                        ImagePath = relativePath,
                        NoteId = noteId
                    });
                }

                // Saves Note and NoteImages in one transaction.
                _noteRepo.AddNote(note);

                _noteAlreadySaved = true;
                _savedNoteId = noteId;

                // Save existing and newly created labels.
                SavePendingLabels(noteId);

                MessageBox.Show(
                    "Note created successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Close();
            }
            catch (Exception ex)
            {
                // If the note itself was not saved,
                // remove copied image files.
                if (!_noteAlreadySaved)
                {
                    foreach (string relativePath in copiedImagePaths)
                    {
                        try
                        {
                            ImageStorageHelper.DeleteImage(relativePath);
                        }
                        catch
                        {
                            // Preserve the original error.
                        }
                    }

                    MessageBox.Show(
                        "The note could not be created:\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(
                        "The note was created, but one or more labels could not be saved.\n\n" +
                        "Please click Save again to retry.\n\nDetails:\n" +
                        ex.Message,
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            finally
            {
                _isSaving = false;

                if (!IsDisposed)
                {
                    btnSave.Enabled = true;
                }
            }
        }

        // =========================
        // RETRY LABEL SAVE
        // =========================
        private void RetryPendingLabels()
        {
            _isSaving = true;
            btnSave.Enabled = false;

            try
            {
                SavePendingLabels(_savedNoteId);

                MessageBox.Show(
                    "Note created successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "One or more labels could not be saved.\n\n" +
                    "Please click Save again to retry.\n\nDetails:\n" +
                    ex.Message,
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                _isSaving = false;

                if (!IsDisposed)
                {
                    btnSave.Enabled = true;
                }
            }
        }

        // =========================
        // SAVE TEMPORARY LABELS
        // =========================
        private void SavePendingLabels(Guid noteId)
        {
            if (UserSession.CurrentUser == null)
            {
                throw new Exception("No signed-in user was found.");
            }

            foreach (LabelModel tempLabel in _tempLabels)
            {
                // This temporary label was linked during an earlier save attempt.
                if (_linkedTemporaryLabelIds.Contains(tempLabel.Id))
                {
                    continue;
                }

                Guid labelIdToLink = tempLabel.Id;

                // New labels are created only at Save time.
                if (_newTemporaryLabelIds.Contains(tempLabel.Id))
                {
                    LabelModel savedLabel =
                        _labelRepo.GetOrCreateLabelForUser(
                            tempLabel.Name,
                            UserSession.CurrentUser.Id);

                    // If a label with this name already exists,
                    // its ID is returned instead of creating a duplicate.
                    labelIdToLink = savedLabel.Id;
                }

                _labelRepo.AddLabelToNote(
                    noteId,
                    labelIdToLink);

                _linkedTemporaryLabelIds.Add(tempLabel.Id);
            }
        }

        // =========================
        // CREATE DELETE BUTTON
        // =========================
        private Button CreateDeleteButton(string tooltipText)
        {
            Button button = new Button
            {
                Text = "×",
                Width = 24,
                Height = 24,
                Visible = false,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false
            };

            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseOverBackColor = Color.MistyRose;
            button.FlatAppearance.MouseDownBackColor = Color.LightCoral;

            _toolTip.SetToolTip(button, tooltipText);

            return button;
        }

        // =========================
        // SHOW X BUTTON ON HOVER
        // =========================
        private void AddDeleteHoverBehavior(
            Panel container,
            Button deleteButton,
            params Control[] hoverControls)
        {
            EventHandler showButton = (sender, e) =>
            {
                deleteButton.Visible = true;
            };

            EventHandler checkMousePosition = (sender, e) =>
            {
                UpdateDeleteButtonVisibility(
                    container,
                    deleteButton);
            };

            container.MouseEnter += showButton;
            container.MouseLeave += checkMousePosition;

            foreach (Control control in hoverControls)
            {
                control.MouseEnter += showButton;
                control.MouseLeave += checkMousePosition;
            }
        }

        private void UpdateDeleteButtonVisibility(
            Panel container,
            Button deleteButton)
        {
            if (IsDisposed || Disposing || !IsHandleCreated)
            {
                return;
            }

            try
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    if (container.IsDisposed || deleteButton.IsDisposed)
                    {
                        return;
                    }

                    Point mousePosition =
                        container.PointToClient(Cursor.Position);

                    deleteButton.Visible =
                        container.ClientRectangle.Contains(mousePosition);
                });
            }
            catch (InvalidOperationException)
            {
                // The form may be closing.
            }
        }

        // =========================
        // CLOSE FORM
        // =========================
        private void btnClose_Click(object sender, EventArgs e)
        {
            // Nothing is saved before Save is clicked.
            Close();
        }

        private void CreateNoteForm_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            _toolTip.Dispose();
        }
    }
}