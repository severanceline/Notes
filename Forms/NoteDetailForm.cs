using Notes.DataAccess;
using Notes.Models;
using Notes.Session;
using Notes.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Notes.Forms
{
    public partial class NoteDetailForm : Form
    {
        private Note currentNote;

        private readonly NoteRepository _noteRepo;
        private readonly LabelRepository _labelRepo;

        // Auto-save timer for note title and content.
        private readonly System.Windows.Forms.Timer _autoSaveTimer;

        // Prevents auto-save while form controls are being populated.
        private bool _isLoadingNote;

        // Prevents multiple save operations from running at the same time.
        private bool _isSavingNote;

        // Prevents auto-save when the note is being deleted.
        private bool _isDeletingNote;

        // Stores the last successfully saved values.
        private string _lastSavedTitle = string.Empty;
        private string _lastSavedContent = string.Empty;

        private readonly ToolTip _toolTip = new ToolTip();

        public NoteDetailForm()
        {
            InitializeComponent();

            _noteRepo = new NoteRepository();
            _labelRepo = new LabelRepository();

            // Save 500 milliseconds after the user stops typing.
            _autoSaveTimer = new System.Windows.Forms.Timer();
            _autoSaveTimer.Interval = 500;
            _autoSaveTimer.Tick += AutoSaveTimer_Tick;

            txtTitle.TextChanged += NoteTextChanged;
            txtContent.TextChanged += NoteTextChanged;

            FormClosing += NoteDetailForm_FormClosing;
            FormClosed += NoteDetailForm_FormClosed;
        }

        // =========================
        // LOAD NOTE
        // =========================
        public void LoadNote(Note note)
        {
            if (note == null || note.NoteInfo == null)
            {
                throw new ArgumentNullException(
                    nameof(note),
                    "A valid note is required.");
            }

            _isLoadingNote = true;

            try
            {
                currentNote = note;

                txtTitle.Text = note.NoteInfo.Title ?? string.Empty;
                txtContent.Text = note.NoteInfo.Content ?? string.Empty;

                _lastSavedTitle = txtTitle.Text;
                _lastSavedContent = txtContent.Text;

                LoadComboLabels();
                LoadLabels();
                LoadImages();
            }
            finally
            {
                _isLoadingNote = false;
            }
        }

        // =========================
        // AUTO SAVE
        // =========================
        private void NoteTextChanged(object sender, EventArgs e)
        {
            if (_isLoadingNote ||
                _isDeletingNote ||
                currentNote == null ||
                currentNote.NoteInfo == null)
            {
                return;
            }

            // Restart the timer every time the user types.
            _autoSaveTimer.Stop();
            _autoSaveTimer.Start();
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            _autoSaveTimer.Stop();

            SaveNoteIfChanged();
        }

        private bool SaveNoteIfChanged()
        {
            if (currentNote == null ||
                currentNote.NoteInfo == null ||
                _isSavingNote ||
                _isLoadingNote ||
                _isDeletingNote)
            {
                return true;
            }

            string newTitle = txtTitle.Text ?? string.Empty;
            string newContent = txtContent.Text ?? string.Empty;

            // Do not send an update request if nothing has changed.
            if (newTitle == _lastSavedTitle &&
                newContent == _lastSavedContent)
            {
                return true;
            }

            string oldTitle = currentNote.NoteInfo.Title;
            string oldContent = currentNote.NoteInfo.Content;
            DateTime oldUpdatedAt = currentNote.NoteInfo.UpdatedAt;

            try
            {
                _isSavingNote = true;

                currentNote.NoteInfo.Title = newTitle;
                currentNote.NoteInfo.Content = newContent;
                currentNote.NoteInfo.UpdatedAt = DateTime.Now;

                bool updated = _noteRepo.UpdateNote(currentNote.NoteInfo);

                if (!updated)
                {
                    throw new Exception(
                        "The note was not found or could not be saved.");
                }

                _lastSavedTitle = newTitle;
                _lastSavedContent = newContent;

                return true;
            }
            catch (Exception ex)
            {
                // Restore the model values.
                // The text remains visible in the TextBoxes so the user does not lose it.
                currentNote.NoteInfo.Title = oldTitle;
                currentNote.NoteInfo.Content = oldContent;
                currentNote.NoteInfo.UpdatedAt = oldUpdatedAt;

                MessageBox.Show(
                    "Auto-save failed:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return false;
            }
            finally
            {
                _isSavingNote = false;
            }
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
        // LOAD NOTE LABELS
        // =========================
        private void LoadLabels()
        {
            if (currentNote == null || currentNote.NoteInfo == null)
            {
                return;
            }

            List<LabelModel> labels =
                _labelRepo.GetLabelsForNote(currentNote.NoteInfo.Id);

            ClearLabelChips();

            foreach (LabelModel label in labels)
            {
                Panel chip = CreateLabelChip(label);
                flpLabels.Controls.Add(chip);
            }
        }

        private void ClearLabelChips()
        {
            while (flpLabels.Controls.Count > 0)
            {
                Control control = flpLabels.Controls[0];

                flpLabels.Controls.RemoveAt(0);
                control.Dispose();
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
                "Remove this label from the note.");

            deleteButton.Location = new Point(chip.Width - 26, 3);
            deleteButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            deleteButton.Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show(
                    $"Remove the label \"{label.Name}\" from this note?",
                    "Remove Label",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                try
                {
                    _labelRepo.RemoveLabelFromNote(
                        currentNote.NoteInfo.Id,
                        label.Id);

                    LoadLabels();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "The label could not be removed:\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
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
        // ADD EXISTING LABEL TO NOTE
        // =========================
        private void btnAddLabel_Click(object sender, EventArgs e)
        {
            if (currentNote == null || currentNote.NoteInfo == null)
            {
                return;
            }

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

            try
            {
                // The repository prevents duplicate label connections.
                _labelRepo.AddLabelToNote(
                    currentNote.NoteInfo.Id,
                    selectedLabel.Id);

                LoadLabels();

                cmbLabels.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The label could not be added:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // CREATE NEW LABEL AND ADD IT TO NOTE
        // =========================
        private void btnNewLabel_Click(object sender, EventArgs e)
        {
            if (currentNote == null ||
                currentNote.NoteInfo == null ||
                UserSession.CurrentUser == null)
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

            LabelModel label = new LabelModel
            {
                Id = Guid.NewGuid(),
                Name = name.Trim()
            };

            try
            {
                // Creates the label, connects it to the user,
                // and attaches it to the current note in one transaction.
                _labelRepo.CreateLabelForUserAndNote(
                    label,
                    UserSession.CurrentUser.Id,
                    currentNote.NoteInfo.Id);

                LoadComboLabels();
                LoadLabels();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The label could not be created:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // LOAD IMAGES
        // =========================
        private void LoadImages()
        {
            if (currentNote == null || currentNote.NoteInfo == null)
            {
                return;
            }

            List<NoteImage> images =
                _noteRepo.GetNoteImagesByNoteId(currentNote.NoteInfo.Id);

            ResetImageList();

            foreach (NoteImage image in images)
            {
                Panel imageTile = CreateImageTile(image);
                flpImages.Controls.Add(imageTile);
            }
        }

        private void ResetImageList()
        {
            while (flpImages.Controls.Count > 0)
            {
                Control control = flpImages.Controls[0];

                flpImages.Controls.RemoveAt(0);

                // Do not dispose the Add Image button.
                if (control != btnAddImage)
                {
                    control.Dispose();
                }
            }

            flpImages.Controls.Add(btnAddImage);
        }

        private Panel CreateImageTile(NoteImage noteImage)
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

            string fullPath =
                ImageStorageHelper.GetFullPath(noteImage.ImagePath);

            Image image = TryLoadImageWithoutLock(fullPath);

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

            // Dispose the loaded image when the PictureBox is disposed.
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
                "Remove this image from the note.");

            deleteButton.Location = new Point(96, 3);
            deleteButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            deleteButton.Click += (sender, e) =>
            {
                DialogResult result = MessageBox.Show(
                    "Remove this image from the note?",
                    "Remove Image",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                DeleteImage(noteImage);
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
        // ADD IMAGE
        // =========================
        private void btnAddImage_Click(object sender, EventArgs e)
        {
            if (currentNote == null || currentNote.NoteInfo == null)
            {
                return;
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png";
                ofd.Multiselect = false;
                ofd.CheckFileExists = true;

                if (ofd.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string relativePath = null;

                try
                {
                    relativePath = ImageStorageHelper.SaveImage(
                        currentNote.NoteInfo.Id,
                        ofd.FileName);

                    _noteRepo.AddNoteImage(
                        currentNote.NoteInfo.Id,
                        relativePath);

                    LoadImages();
                }
                catch (Exception ex)
                {
                    // Remove the copied file if saving to the database failed.
                    if (!string.IsNullOrWhiteSpace(relativePath))
                    {
                        try
                        {
                            ImageStorageHelper.DeleteImage(relativePath);
                        }
                        catch
                        {
                            // Keep the original error as the main error.
                        }
                    }

                    MessageBox.Show(
                        "The image could not be added:\n" + ex.Message,
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        // =========================
        // DELETE IMAGE
        // =========================
        private void DeleteImage(NoteImage image)
        {
            try
            {
                // Delete the database record first.
                _noteRepo.DeleteNoteImage(image.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The image could not be removed from the database:\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            try
            {
                // Delete the physical file after the database record is removed.
                ImageStorageHelper.DeleteImage(image.ImagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The image record was removed, but the image file could not be deleted:\n" +
                    ex.Message,
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            LoadImages();
        }

        // =========================
        // DELETE NOTE, LABEL CONNECTIONS, AND IMAGES
        // =========================
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (currentNote == null || currentNote.NoteInfo == null)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                "This will permanently delete the note, its images, and its label connections.\n\nDo you want to continue?",
                "Delete Note",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                return;
            }

            _autoSaveTimer.Stop();

            try
            {
                // Deletes NoteLabels, NoteImages, and Notes in one database transaction.
                List<string> imagePaths =
                    _noteRepo.DeleteNoteWithRelations(currentNote.NoteInfo.Id);

                List<string> failedFiles = new List<string>();

                // Delete physical image files after the database transaction succeeds.
                foreach (string relativePath in imagePaths)
                {
                    try
                    {
                        ImageStorageHelper.DeleteImage(relativePath);
                    }
                    catch
                    {
                        failedFiles.Add(relativePath);
                    }
                }

                _isDeletingNote = true;

                if (failedFiles.Count > 0)
                {
                    MessageBox.Show(
                        "The note was deleted from the database, but " +
                        failedFiles.Count +
                        " image file(s) could not be deleted from storage.",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                Close();
            }
            catch (Exception ex)
            {
                // Resume auto-save if deletion did not succeed.
                _autoSaveTimer.Start();

                MessageBox.Show(
                    "The note could not be deleted:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // DELETE BUTTON HOVER BEHAVIOR
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
                UpdateDeleteButtonVisibility(container, deleteButton);
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
                // The form handle may already be closing.
            }
        }

        // =========================
        // CLOSE FORM
        // =========================
        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void NoteDetailForm_FormClosing(
            object sender,
            FormClosingEventArgs e)
        {
            if (_isDeletingNote)
            {
                return;
            }

            _autoSaveTimer.Stop();

            // Save final changes before closing the form.
            bool saved = SaveNoteIfChanged();

            // Keep the form open if saving failed.
            if (!saved)
            {
                e.Cancel = true;
            }
        }

        private void NoteDetailForm_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            _autoSaveTimer.Dispose();
            _toolTip.Dispose();
        }

        private void txtContent_TextChanged(object sender, EventArgs e)
        {

        }
    }
}