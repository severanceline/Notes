using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Notes.Models;

namespace Notes.DataAccess
{
    public class LabelRepository
    {
        // GET USER LABELS
        public List<LabelModel> GetLabelsByUserId(Guid userId)
        {
            List<LabelModel> labels = new List<LabelModel>();

            string query = @"
                SELECT Id, Name
                FROM Labels
                WHERE UserId = @UserId
                ORDER BY Name";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        labels.Add(new LabelModel
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            return labels;
        }

        // GET OR CREATE LABEL FOR USER
        public LabelModel GetOrCreateLabelForUser(
            string labelName,
            Guid userId)
        {
            string cleanName = (labelName ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(cleanName))
            {
                throw new ArgumentException(
                    "A label name is required.",
                    nameof(labelName));
            }

            LabelModel label = null;

            string findQuery = @"
                SELECT TOP 1 Id, Name
                FROM Labels
                WHERE UserId = @UserId
                AND UPPER(Name) = UPPER(@Name)";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(findQuery, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Name", cleanName);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        label = new LabelModel
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        };
                    }
                }
            }

            if (label != null)
            {
                return label;
            }

            label = new LabelModel
            {
                Id = Guid.NewGuid(),
                Name = cleanName
            };

            string insertQuery = @"
                INSERT INTO Labels (Id, UserId, Name)
                VALUES (@Id, @UserId, @Name)";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
            {
                cmd.Parameters.AddWithValue("@Id", label.Id);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@Name", label.Name);

                conn.Open();

                cmd.ExecuteNonQuery();
            }

            return label;
        }

        // CREATE LABEL FOR USER
        public void CreateLabelForUser(
            LabelModel label,
            Guid userId)
        {
            if (label == null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            GetOrCreateLabelForUser(label.Name, userId);
        }

        // CREATE LABEL FOR USER AND NOTE
        public void CreateLabelForUserAndNote(
            LabelModel label,
            Guid userId,
            Guid noteId)
        {
            if (label == null)
            {
                throw new ArgumentNullException(nameof(label));
            }

            LabelModel savedLabel =
                GetOrCreateLabelForUser(
                    label.Name,
                    userId);

            AddLabelToNote(
                noteId,
                savedLabel.Id);
        }

        // ADD LABEL TO NOTE
        public void AddLabelToNote(
            Guid noteId,
            Guid labelId)
        {
            string query = @"
                IF NOT EXISTS
                (
                    SELECT 1
                    FROM NoteLabels
                    WHERE NoteId = @NoteId
                    AND LabelId = @LabelId
                )
                BEGIN
                    INSERT INTO NoteLabels (NoteId, LabelId)
                    VALUES (@NoteId, @LabelId)
                END";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@NoteId", noteId);
                cmd.Parameters.AddWithValue("@LabelId", labelId);

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        // GET NOTE LABELS
        public List<LabelModel> GetLabelsForNote(Guid noteId)
        {
            List<LabelModel> labels = new List<LabelModel>();

            string query = @"
                SELECT L.Id, L.Name
                FROM Labels L
                INNER JOIN NoteLabels NL
                    ON L.Id = NL.LabelId
                WHERE NL.NoteId = @NoteId
                ORDER BY L.Name";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@NoteId", noteId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        labels.Add(new LabelModel
                        {
                            Id = reader.GetGuid(0),
                            Name = reader.GetString(1)
                        });
                    }
                }
            }

            return labels;
        }

        // GET NOTE IDS WITH ALL SELECTED LABELS
        public HashSet<Guid> GetNoteIdsForUserWithAllLabels(
            Guid userId,
            IEnumerable<Guid> labelIds)
        {
            List<Guid> selectedLabelIds = labelIds
                .Distinct()
                .ToList();

            HashSet<Guid> noteIds = new HashSet<Guid>();

            if (selectedLabelIds.Count == 0)
            {
                return noteIds;
            }

            List<string> parameterNames = new List<string>();

            for (int i = 0; i < selectedLabelIds.Count; i++)
            {
                parameterNames.Add($"@LabelId{i}");
            }

            string query = $@"
                SELECT N.Id
                FROM Notes N
                INNER JOIN NoteLabels NL
                    ON N.Id = NL.NoteId
                INNER JOIN Labels L
                    ON L.Id = NL.LabelId
                WHERE N.UserId = @UserId
                AND L.UserId = @UserId
                AND NL.LabelId IN ({string.Join(", ", parameterNames)})
                GROUP BY N.Id
                HAVING COUNT(DISTINCT NL.LabelId) =
                    @SelectedLabelCount";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                for (int i = 0; i < selectedLabelIds.Count; i++)
                {
                    cmd.Parameters.AddWithValue(
                        parameterNames[i],
                        selectedLabelIds[i]);
                }

                cmd.Parameters.AddWithValue(
                    "@SelectedLabelCount",
                    selectedLabelIds.Count);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        noteIds.Add(reader.GetGuid(0));
                    }
                }
            }

            return noteIds;
        }

        // REMOVE LABEL FROM ONE NOTE
        public void RemoveLabelFromNote(
            Guid noteId,
            Guid labelId)
        {
            string query = @"
                DELETE FROM NoteLabels
                WHERE NoteId = @NoteId
                AND LabelId = @LabelId";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@NoteId", noteId);
                cmd.Parameters.AddWithValue("@LabelId", labelId);

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        // DELETE LABEL FOR ONE USER
        public bool DeleteLabelForUser(
            Guid labelId,
            Guid userId)
        {
            string deleteNoteLabelsQuery = @"
                DELETE NL
                FROM NoteLabels NL
                INNER JOIN Labels L
                    ON NL.LabelId = L.Id
                WHERE NL.LabelId = @LabelId
                AND L.UserId = @UserId";

            string deleteLabelQuery = @"
                DELETE FROM Labels
                WHERE Id = @LabelId
                AND UserId = @UserId";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction =
                    conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteLinksCmd =
                            new SqlCommand(
                                deleteNoteLabelsQuery,
                                conn,
                                transaction))
                        {
                            deleteLinksCmd.Parameters.AddWithValue(
                                "@LabelId",
                                labelId);

                            deleteLinksCmd.Parameters.AddWithValue(
                                "@UserId",
                                userId);

                            deleteLinksCmd.ExecuteNonQuery();
                        }

                        int deletedRows;

                        using (SqlCommand deleteLabelCmd =
                            new SqlCommand(
                                deleteLabelQuery,
                                conn,
                                transaction))
                        {
                            deleteLabelCmd.Parameters.AddWithValue(
                                "@LabelId",
                                labelId);

                            deleteLabelCmd.Parameters.AddWithValue(
                                "@UserId",
                                userId);

                            deletedRows =
                                deleteLabelCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        return deletedRows > 0;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}