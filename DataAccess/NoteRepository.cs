using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Notes.Models;

namespace Notes.DataAccess
{
    public class NoteRepository
    {
        public List<Note> GetNotesByUserId(Guid userId)
        {
            List<Note> notes = new List<Note>();

            string query = @"
                SELECT Id, UserId, Title, Content, CreatedAt, UpdatedAt
                FROM Notes
                WHERE UserId = @UserId
                ORDER BY UpdatedAt DESC";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        NoteInfo noteInfo = new NoteInfo
                        {
                            Id = reader.GetGuid(0),
                            UserId = reader.GetGuid(1),
                            Title = reader.IsDBNull(2)
                                ? string.Empty
                                : reader.GetString(2),

                            Content = reader.IsDBNull(3)
                                ? string.Empty
                                : reader.GetString(3),

                            CreatedAt = reader.GetDateTime(4),
                            UpdatedAt = reader.GetDateTime(5)
                        };

                        Note note = new Note
                        {
                            NoteInfo = noteInfo,
                            noteImages = GetNoteImagesByNoteId(noteInfo.Id)
                        };

                        notes.Add(note);
                    }
                }
            }

            return notes;
        }

        public List<NoteImage> GetNoteImagesByNoteId(Guid noteId)
        {
            List<NoteImage> images = new List<NoteImage>();

            string query = @"
                SELECT Id, ImagePath, NoteId
                FROM NoteImages
                WHERE NoteId = @NoteId";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@NoteId", noteId);

                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        images.Add(new NoteImage
                        {
                            Id = reader.GetGuid(0),
                            ImagePath = reader.GetString(1),
                            NoteId = reader.GetGuid(2)
                        });
                    }
                }
            }

            return images;
        }

        public void AddNote(Note note)
        {
            string noteQuery = @"
                INSERT INTO Notes
                    (Id, UserId, Title, Content, CreatedAt, UpdatedAt)
                VALUES
                    (@Id, @UserId, @Title, @Content, @CreatedAt, @UpdatedAt)";

            string imageQuery = @"
                INSERT INTO NoteImages
                    (Id, ImagePath, NoteId)
                VALUES
                    (@Id, @ImagePath, @NoteId)";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            noteQuery,
                            conn,
                            transaction))
                        {
                            cmd.Parameters.AddWithValue("@Id", note.NoteInfo.Id);
                            cmd.Parameters.AddWithValue("@UserId", note.NoteInfo.UserId);

                            cmd.Parameters.AddWithValue(
                                "@Title",
                                note.NoteInfo.Title ?? string.Empty);

                            cmd.Parameters.AddWithValue(
                                "@Content",
                                string.IsNullOrEmpty(note.NoteInfo.Content)
                                    ? (object)DBNull.Value
                                    : note.NoteInfo.Content);

                            cmd.Parameters.AddWithValue(
                                "@CreatedAt",
                                note.NoteInfo.CreatedAt);

                            cmd.Parameters.AddWithValue(
                                "@UpdatedAt",
                                note.NoteInfo.UpdatedAt);

                            cmd.ExecuteNonQuery();
                        }

                        if (note.noteImages != null &&
                            note.noteImages.Count > 0)
                        {
                            foreach (NoteImage image in note.noteImages)
                            {
                                using (SqlCommand imageCmd = new SqlCommand(
                                    imageQuery,
                                    conn,
                                    transaction))
                                {
                                    imageCmd.Parameters.AddWithValue(
                                        "@Id",
                                        image.Id);

                                    imageCmd.Parameters.AddWithValue(
                                        "@ImagePath",
                                        image.ImagePath);

                                    imageCmd.Parameters.AddWithValue(
                                        "@NoteId",
                                        note.NoteInfo.Id);

                                    imageCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        transaction.Commit();
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
        // AUTO SAVE UPDATE
        // =========================
        public bool UpdateNote(NoteInfo note)
        {
            string query = @"
                UPDATE Notes
                SET
                    Title = @Title,
                    Content = @Content,
                    UpdatedAt = @UpdatedAt
                WHERE Id = @NoteId";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue(
                    "@Title",
                    note.Title ?? string.Empty);

                cmd.Parameters.AddWithValue(
                    "@Content",
                    string.IsNullOrEmpty(note.Content)
                        ? (object)DBNull.Value
                        : note.Content);

                cmd.Parameters.AddWithValue(
                    "@UpdatedAt",
                    note.UpdatedAt);

                cmd.Parameters.AddWithValue(
                    "@NoteId",
                    note.Id);

                conn.Open();

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public void AddNoteImage(Guid noteId, string imagePath)
        {
            string query = @"
                INSERT INTO NoteImages (Id, NoteId, ImagePath)
                VALUES (@Id, @NoteId, @ImagePath)";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@NoteId", noteId);
                cmd.Parameters.AddWithValue("@ImagePath", imagePath);

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteNoteImage(Guid imageId)
        {
            string query = "DELETE FROM NoteImages WHERE Id = @Id";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Id", imageId);

                conn.Open();

                cmd.ExecuteNonQuery();
            }
        }

        // =========================
        // DELETE NOTE + RELATIONS
        // =========================
        public List<string> DeleteNoteWithRelations(Guid noteId)
        {
            List<string> imagePaths = new List<string>();

            string getImagesQuery = @"
                SELECT ImagePath
                FROM NoteImages
                WHERE NoteId = @NoteId";

            string deleteNoteLabelsQuery = @"
                DELETE FROM NoteLabels
                WHERE NoteId = @NoteId";

            string deleteNoteImagesQuery = @"
                DELETE FROM NoteImages
                WHERE NoteId = @NoteId";

            string deleteNoteQuery = @"
                DELETE FROM Notes
                WHERE Id = @NoteId";

            using (SqlConnection conn = DatabaseManager.GetConnection())
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        // ابتدا مسیر تمام عکس‌ها را نگه می‌داریم
                        using (SqlCommand getImagesCmd = new SqlCommand(
                            getImagesQuery,
                            conn,
                            transaction))
                        {
                            getImagesCmd.Parameters.AddWithValue("@NoteId", noteId);

                            using (SqlDataReader reader =
                                getImagesCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    imagePaths.Add(reader.GetString(0));
                                }
                            }
                        }

                        // حذف ارتباط لیبل‌ها با همین نوت
                        using (SqlCommand deleteLabelsCmd = new SqlCommand(
                            deleteNoteLabelsQuery,
                            conn,
                            transaction))
                        {
                            deleteLabelsCmd.Parameters.AddWithValue(
                                "@NoteId",
                                noteId);

                            deleteLabelsCmd.ExecuteNonQuery();
                        }

                        // حذف رکورد عکس‌های همین نوت
                        using (SqlCommand deleteImagesCmd = new SqlCommand(
                            deleteNoteImagesQuery,
                            conn,
                            transaction))
                        {
                            deleteImagesCmd.Parameters.AddWithValue(
                                "@NoteId",
                                noteId);

                            deleteImagesCmd.ExecuteNonQuery();
                        }

                        // حذف خود نوت
                        using (SqlCommand deleteNoteCmd = new SqlCommand(
                            deleteNoteQuery,
                            conn,
                            transaction))
                        {
                            deleteNoteCmd.Parameters.AddWithValue(
                                "@NoteId",
                                noteId);

                            int rowsAffected =
                                deleteNoteCmd.ExecuteNonQuery();

                            if (rowsAffected == 0)
                            {
                                throw new Exception(
                                    "یادداشت موردنظر پیدا نشد.");
                            }
                        }

                        transaction.Commit();

                        return imagePaths;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // برای سازگاری با بخش‌های قدیمی برنامه نگه داشته شده است.
        // برای حذف کامل نوت در فرم NoteDetailForm از DeleteNoteWithRelations استفاده می‌شود.
        public bool DeleteNote(Guid noteId)
        {
            using (SqlConnection conn = DatabaseManager.GetConnection())
            {
                string query = "DELETE FROM Notes WHERE Id = @NoteId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NoteId", noteId);

                    conn.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }
    }
}