using Notes.DataAccess;
using Notes.Models;
using Notes.Session;
using Notes.UserControls;
using Notes.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Notes.Forms
{
    public partial class MainForm : Form
    {
        private readonly NoteRepository _noteRepo;
        private readonly LabelRepository _labelRepo;

        private List<Note> allNotes = new List<Note>();

        private readonly ToolTip _toolTip = new ToolTip();

        // Prevents filter code from running while CheckedListBox is loading.
        private bool _isLoadingLabelFilters;

        // Stores the latest database result for selected label filters.
        private string _cachedLabelFilterKey = string.Empty;

        private HashSet<Guid> _cachedFilteredNoteIds =
            new HashSet<Guid>();

        public MainForm()
        {
            InitializeComponent();

            _noteRepo = new NoteRepository();
            _labelRepo = new LabelRepository();

            txtSearch.TextChanged += txtSearch_TextChanged;

            clbFilterLabels.ItemCheck +=
                clbFilterLabels_ItemCheck;

            btnClearLabelFilters.Click +=
                btnClearLabelFilters_Click;

            FormClosed += MainForm_FormClosed;
        }

        // =========================
        // FORM LOAD
        // =========================
        private void MainForm_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(900, 600);

            SetWelcome();
            RefreshMainData();
        }

        // =========================
        // WELCOME USER
        // =========================
        private void SetWelcome()
        {
            if (UserSession.CurrentUser != null)
            {
                lblWelcome.Text =
                    $"Welcome, {UserSession.CurrentUser.FullName} 👋";
            }
        }

        // =========================
        // REFRESH NOTES AND LABELS
        // =========================
        private void RefreshMainData()
        {
            if (UserSession.CurrentUser == null)
            {
                return;
            }

            HashSet<Guid> selectedLabelIds =
                GetSelectedFilterLabelIds();

            LoadData();

            ClearLabelFilterCache();

            LoadLabelFilters(selectedLabelIds);
            LoadSidebarLabels();

            ApplyFilters();
        }

        // =========================
        // LOAD NOTES
        // =========================
        private void LoadData()
        {
            if (UserSession.CurrentUser == null)
            {
                allNotes = new List<Note>();
                return;
            }

            allNotes = _noteRepo.GetNotesByUserId(
                UserSession.CurrentUser.Id);
        }

        // =========================
        // RENDER NOTES
        // =========================
        private void RenderNotes(List<Note> notes)
        {
            ClearControls(flpNotes);

            foreach (Note note in notes)
            {
                NoteCardControl card =
                    new NoteCardControl(note);

                card.NoteClicked += (s, e) =>
                {
                    using (NoteDetailForm form =
                        new NoteDetailForm())
                    {
                        form.LoadNote(note);
                        form.ShowDialog();
                    }

                    RefreshMainData();
                };

                flpNotes.Controls.Add(card);
            }
        }

        // =========================
        // CREATE NOTE
        // =========================
        private void btnCreateNote_Click(object sender, EventArgs e)
        {
            using (CreateNoteForm form =
                new CreateNoteForm())
            {
                form.ShowDialog();
            }

            RefreshMainData();
        }

        // =========================
        // SEARCH TEXT CHANGED
        // =========================
        private void txtSearch_TextChanged(
            object sender,
            EventArgs e)
        {
            ApplyFilters();
        }

        // =========================
        // CHECKED LABEL CHANGED
        // =========================
        private void clbFilterLabels_ItemCheck(
            object sender,
            ItemCheckEventArgs e)
        {
            if (_isLoadingLabelFilters)
            {
                return;
            }

            // CheckedItems updates after this event finishes.
            // BeginInvoke runs the filter after the checkbox state is updated.
            try
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    if (!IsDisposed)
                    {
                        ClearLabelFilterCache();
                        ApplyFilters();
                    }
                });
            }
            catch (InvalidOperationException)
            {
                // The form may be closing.
            }
        }

        // =========================
        // CLEAR LABEL FILTERS
        // =========================
        private void btnClearLabelFilters_Click(
            object sender,
            EventArgs e)
        {
            _isLoadingLabelFilters = true;

            try
            {
                for (int i = 0; i < clbFilterLabels.Items.Count; i++)
                {
                    clbFilterLabels.SetItemChecked(i, false);
                }
            }
            finally
            {
                _isLoadingLabelFilters = false;
            }

            ClearLabelFilterCache();
            ApplyFilters();
        }

        // =========================
        // APPLY SEARCH + AND FILTER
        // =========================
        private void ApplyFilters()
        {
            if (UserSession.CurrentUser == null)
            {
                RenderNotes(new List<Note>());
                return;
            }

            string searchText =
                (txtSearch.Text ?? string.Empty).Trim();

            HashSet<Guid> selectedLabelIds =
                GetSelectedFilterLabelIds();

            IEnumerable<Note> filteredNotes = allNotes;

            // Search title and content.
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                filteredNotes = filteredNotes.Where(note =>
                {
                    string title =
                        note.NoteInfo?.Title ?? string.Empty;

                    string content =
                        note.NoteInfo?.Content ?? string.Empty;

                    return title.IndexOf(
                               searchText,
                               StringComparison.OrdinalIgnoreCase) >= 0
                           ||
                           content.IndexOf(
                               searchText,
                               StringComparison.OrdinalIgnoreCase) >= 0;
                });
            }

            // AND logic:
            // Notes must have every selected Label.
            if (selectedLabelIds.Count > 0)
            {
                HashSet<Guid> noteIdsWithAllLabels =
                    GetNoteIdsWithAllSelectedLabels(
                        selectedLabelIds);

                filteredNotes = filteredNotes.Where(note =>
                    note.NoteInfo != null &&
                    noteIdsWithAllLabels.Contains(
                        note.NoteInfo.Id));
            }

            RenderNotes(filteredNotes.ToList());
        }

        // =========================
        // GET SELECTED LABEL IDS
        // =========================
        private HashSet<Guid> GetSelectedFilterLabelIds()
        {
            HashSet<Guid> selectedIds =
                new HashSet<Guid>();

            foreach (object item in clbFilterLabels.CheckedItems)
            {
                LabelFilterItem filterItem =
                    item as LabelFilterItem;

                if (filterItem != null &&
                    filterItem.Id.HasValue)
                {
                    selectedIds.Add(filterItem.Id.Value);
                }
            }

            return selectedIds;
        }

        // =========================
        // GET NOTE IDS FOR AND FILTER
        // =========================
        private HashSet<Guid> GetNoteIdsWithAllSelectedLabels(
            HashSet<Guid> selectedLabelIds)
        {
            string cacheKey = string.Join(
                "|",
                selectedLabelIds
                    .OrderBy(id => id)
                    .Select(id => id.ToString("N")));

            if (cacheKey == _cachedLabelFilterKey)
            {
                return _cachedFilteredNoteIds;
            }

            _cachedLabelFilterKey = cacheKey;

            _cachedFilteredNoteIds =
                _labelRepo.GetNoteIdsForUserWithAllLabels(
                    UserSession.CurrentUser.Id,
                    selectedLabelIds);

            return _cachedFilteredNoteIds;
        }

        private void ClearLabelFilterCache()
        {
            _cachedLabelFilterKey = string.Empty;
            _cachedFilteredNoteIds.Clear();
        }

        // =========================
        // LOAD LABEL CHECKBOXES
        // =========================
        private void LoadLabelFilters(
            HashSet<Guid> selectedLabelIds)
        {
            if (UserSession.CurrentUser == null)
            {
                return;
            }

            List<LabelModel> labels =
                _labelRepo.GetLabelsByUserId(
                    UserSession.CurrentUser.Id);

            _isLoadingLabelFilters = true;

            clbFilterLabels.BeginUpdate();

            try
            {
                clbFilterLabels.Items.Clear();

                foreach (LabelModel label in labels)
                {
                    LabelFilterItem item =
                        new LabelFilterItem
                        {
                            Id = label.Id,
                            Name = label.Name
                        };

                    clbFilterLabels.Items.Add(
                        item,
                        selectedLabelIds.Contains(label.Id));
                }
            }
            finally
            {
                clbFilterLabels.EndUpdate();
                _isLoadingLabelFilters = false;
            }
        }

        // =========================
        // LOAD SIDEBAR LABELS
        // =========================
        private void LoadSidebarLabels()
        {
            if (UserSession.CurrentUser == null)
            {
                return;
            }

            List<LabelModel> labels =
                _labelRepo.GetLabelsByUserId(
                    UserSession.CurrentUser.Id);

            ClearControls(flpSidebarLabels);

            foreach (LabelModel label in labels)
            {
                Panel sidebarItem =
                    CreateSidebarLabelItem(label);

                flpSidebarLabels.Controls.Add(sidebarItem);
            }
        }

        // =========================
        // CREATE SIDEBAR LABEL ITEM
        // =========================
        private Panel CreateSidebarLabelItem(LabelModel label)
        {
            Panel item = new Panel
            {
                Width = 210,
                Height = 34,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(3),
                Cursor = Cursors.Hand,
                Tag = label
            };

            Label labelText = new Label
            {
                Text = label.Name,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 30, 0),
                AutoEllipsis = true,
                Cursor = Cursors.Hand,
                Tag = label
            };

            Button deleteButton = new Button
            {
                Text = "×",
                Dock = DockStyle.Right,
                Width = 30,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TabStop = false,
                Tag = label
            };

            deleteButton.FlatAppearance.BorderSize = 0;
            deleteButton.FlatAppearance.MouseOverBackColor =
                Color.MistyRose;

            deleteButton.FlatAppearance.MouseDownBackColor =
                Color.LightCoral;

            _toolTip.SetToolTip(
                deleteButton,
                "Delete this label from your notes and label list.");

            // Clicking a Label toggles its checkbox in the filter list.
            EventHandler toggleFilter = (s, e) =>
            {
                ToggleLabelFilter(label.Id);
            };

            item.Click += toggleFilter;
            labelText.Click += toggleFilter;

            deleteButton.Click += (s, e) =>
            {
                DeleteSidebarLabel(label);
            };

            item.Controls.Add(labelText);
            item.Controls.Add(deleteButton);

            deleteButton.BringToFront();

            return item;
        }

        // =========================
        // TOGGLE LABEL FILTER
        // =========================
        private void ToggleLabelFilter(Guid labelId)
        {
            for (int i = 0; i < clbFilterLabels.Items.Count; i++)
            {
                LabelFilterItem item =
                    clbFilterLabels.Items[i] as LabelFilterItem;

                if (item != null && item.Id == labelId)
                {
                    bool isChecked =
                        clbFilterLabels.GetItemChecked(i);

                    clbFilterLabels.SetItemChecked(
                        i,
                        !isChecked);

                    return;
                }
            }
        }

        // =========================
        // DELETE LABEL
        // =========================
        private void DeleteSidebarLabel(LabelModel label)
        {
            if (UserSession.CurrentUser == null)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Delete the label \"{label.Name}\"?\n\n" +
                "This label will be removed from your notes and label list. " +
                "Your notes will not be deleted.",
                "Delete Label",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                return;
            }

            try
            {
                bool deleted = _labelRepo.DeleteLabelForUser(
                    label.Id,
                    UserSession.CurrentUser.Id);

                if (!deleted)
                {
                    MessageBox.Show(
                        "The label could not be found or was already removed.",
                        "Label",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                RefreshMainData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The label could not be deleted:\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // ADD NEW LABEL
        // =========================
        private void btnAddLabel_Click(
            object sender,
            EventArgs e)
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

            try
            {
                List<LabelModel> labels =
                    _labelRepo.GetLabelsByUserId(
                        UserSession.CurrentUser.Id);

                bool duplicateExists = labels.Any(label =>
                    string.Equals(
                        label.Name?.Trim(),
                        normalizedName,
                        StringComparison.OrdinalIgnoreCase));

                if (duplicateExists)
                {
                    MessageBox.Show(
                        "This label already exists.",
                        "Duplicate Label",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                _labelRepo.GetOrCreateLabelForUser(
                    normalizedName,
                    UserSession.CurrentUser.Id);

                RefreshMainData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "The label could not be created:\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================
        // LOGOUT
        // =========================
        private void btnLogout_Click_1(
            object sender,
            EventArgs e)
        {
            UserSession.Logout();
            LoginStorage.Clear();

            Hide();

            new SigninForm().Show();
        }

        // =========================
        // CLEAR FLOWLAYOUT CONTROLS
        // =========================
        private void ClearControls(FlowLayoutPanel panel)
        {
            while (panel.Controls.Count > 0)
            {
                Control control = panel.Controls[0];

                panel.Controls.RemoveAt(0);
                control.Dispose();
            }
        }

        // Keeps compatibility with the Event in Designer.
        private void flpSidebarLabels_Paint(
            object sender,
            PaintEventArgs e)
        {
        }

        // =========================
        // CLEAN UP
        // =========================
        private void MainForm_FormClosed(
            object sender,
            FormClosedEventArgs e)
        {
            _toolTip.Dispose();
        }

        // =========================
        // FILTER ITEM MODEL
        // =========================
        private class LabelFilterItem
        {
            public Guid? Id { get; set; }

            public string Name { get; set; }

            public override string ToString()
            {
                return Name ?? string.Empty;
            }
        }
    }
}