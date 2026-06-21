using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Notes.Models;

namespace Notes.DataAccess
{
    public class LabelRepository
    {
        // =========================
        // GET USER LABELS
        // =========================
        public List<LabelModel> GetLabelsByUserId(Guid userId)
        {
            List<LabelModel> labels = new List<LabelModel>();

            string query = @"
                SELECT DISTINCT L.Id, L.Name
                FROM Labels L
                INNER JOIN UserLabels UL ON L.Id = UL.LabelId
                WHERE UL.UserId = @UserId
                ORDER BY L.Name";

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

        // =========================
        // GET OR CREATE UNIQUE LABEL
        // =========================
        public LabelModel GetOrCreateLabelForUser(
            string labelName,
            Guid userId)
        {
            string normalizedName = (labelName ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                throw new ArgumentException(
                    "A label name is required.",
                    nameof(labelName));
            }

            using (SqlConnection conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction =
                    conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        LabelModel label = null;

                        string findLabelQuery = @"
                            SELECT TOP 1 Id, Name
                            FROM Labels WITH (UPDLOCK, HOLDLOCK)
                            WHERE UPPER(LTRIM(RTRIM(Name))) = UPPER(@Name)
                            ORDER BY Id";

                        using (SqlCommand findCmd = new SqlCommand(
                            findLabelQuery,
                            conn,
                            transaction))
                        {
                            findCmd.Parameters.AddWithValue(
                                "@Name",
                                normalizedName);

                            using (SqlDataReader reader =
                                findCmd.ExecuteReader())
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

                        if (label == null)
                        {
                            label = new LabelModel
                            {
                                Id = Guid.NewGuid(),
                                Name = normalizedName
                            };

                            string insertLabelQuery = @"
                                INSERT INTO Labels (Id, Name)
                                VALUES (@Id, @Name)";

                            using (SqlCommand insertCmd = new SqlCommand(
                                insertLabelQuery,
                                conn,
                                transaction))
                            {
                                insertCmd.Parameters.AddWithValue(
                                    "@Id",
                                    label.Id);

                                insertCmd.Parameters.AddWithValue(
                                    "@Name",
                                    label.Name);

                                insertCmd.ExecuteNonQuery();
                            }
                        }

                        string connectUserQuery = @"
                            IF NOT EXISTS
                            (
                                SELECT 1
                                FROM UserLabels WITH (UPDLOCK, HOLDLOCK)
                                WHERE UserId = @UserId
                                AND LabelId = @LabelId
                            )
                            BEGIN
                                INSERT INTO UserLabels (UserId, LabelId)
                                VALUES (@UserId, @LabelId)
                            END";

                        using (SqlCommand connectCmd = new SqlCommand(
                            connectUserQuery,
                            conn,
                            transaction))
                        {
                            connectCmd.Parameters.AddWithValue(
                                "@UserId",
                                userId);

                            connectCmd.Parameters.AddWithValue(
                                "@LabelId",
                                label.Id);

                            connectCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        return label;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // =========================
        // CREATE LABEL FOR USER
        // =========================
        public void CreateLabelForUser(
            LabelModel label,
            Guid userId)
        {
            GetOrCreateLabelForUser(
                label.Name,
                userId);
        }

        // =========================
        // CREATE LABEL FOR USER AND NOTE
        // =========================
        public void CreateLabelForUserAndNote(
            LabelModel label,
            Guid userId,
            Guid noteId)
        {
            LabelModel savedLabel =
                GetOrCreateLabelForUser(
                    label.Name,
                    userId);

            AddLabelToNote(
                noteId,
                savedLabel.Id);
        }

        // =========================
        // ADD LABEL TO NOTE
        // =========================
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

        // =========================
        // GET NOTE LABELS
        // =========================
        public List<LabelModel> GetLabelsForNote(Guid noteId)
        {
            List<LabelModel> labels = new List<LabelModel>();

            string query = @"
                SELECT DISTINCT L.Id, L.Name
                FROM Labels L
                INNER JOIN NoteLabels NL ON L.Id = NL.LabelId
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

        // =========================
        // GET NOTE IDS WITH ALL SELECTED LABELS
        // =========================
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
                INNER JOIN NoteLabels NL ON N.Id = NL.NoteId
                WHERE N.UserId = @UserId
                AND NL.LabelId IN ({string.Join(", ", parameterNames)})
                GROUP BY N.Id
                HAVING COUNT(DISTINCT NL.LabelId) = @SelectedLabelCount";

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

        // =========================
        // REMOVE LABEL FROM ONE NOTE
        // =========================
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

        // =========================
        // DELETE LABEL FOR ONE USER
        // =========================
        public bool DeleteLabelForUser(
            Guid labelId,
            Guid userId)
        {
            string deleteNoteLabelLinksQuery = @"
                DELETE NL
                FROM NoteLabels NL
                INNER JOIN Notes N ON N.Id = NL.NoteId
                WHERE N.UserId = @UserId
                AND NL.LabelId = @LabelId";

            string deleteUserLabelQuery = @"
                DELETE FROM UserLabels
                WHERE UserId = @UserId
                AND LabelId = @LabelId";

            string deleteUnusedGlobalLabelQuery = @"
                DELETE FROM Labels
                WHERE Id = @LabelId
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM UserLabels
                    WHERE LabelId = @LabelId
                )
                AND NOT EXISTS
                (
                    SELECT 1
                    FROM NoteLabels
                    WHERE LabelId = @LabelId
                )";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand deleteLinksCmd = new SqlCommand(
                            deleteNoteLabelLinksQuery,
                            conn,
                            transaction))
                        {
                            deleteLinksCmd.Parameters.AddWithValue(
                                "@UserId",
                                userId);

                            deleteLinksCmd.Parameters.AddWithValue(
                                "@LabelId",
                                labelId);

                            deleteLinksCmd.ExecuteNonQuery();
                        }

                        int deletedUserLabelRows;

                        using (SqlCommand deleteUserLabelCmd = new SqlCommand(
                            deleteUserLabelQuery,
                            conn,
                            transaction))
                        {
                            deleteUserLabelCmd.Parameters.AddWithValue(
                                "@UserId",
                                userId);

                            deleteUserLabelCmd.Parameters.AddWithValue(
                                "@LabelId",
                                labelId);

                            deletedUserLabelRows =
                                deleteUserLabelCmd.ExecuteNonQuery();
                        }

                        using (SqlCommand deleteGlobalLabelCmd = new SqlCommand(
                            deleteUnusedGlobalLabelQuery,
                            conn,
                            transaction))
                        {
                            deleteGlobalLabelCmd.Parameters.AddWithValue(
                                "@LabelId",
                                labelId);

                            deleteGlobalLabelCmd.ExecuteNonQuery();
                        }

                        transaction.Commit();

                        return deletedUserLabelRows > 0;
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