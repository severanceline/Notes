using System;
using System.IO;

namespace Notes.Utilities
{
    public static class ImageStorageHelper
    {
        private static readonly string baseFolder =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Notes",
                "Images"
            );

        public static string SaveImage(Guid noteId, string sourceFilePath)
        {
            // ساخت فولدر اصلی
            if (!Directory.Exists(baseFolder))
                Directory.CreateDirectory(baseFolder);

            // فولدر مخصوص نوت
            string noteFolder = Path.Combine(baseFolder, noteId.ToString());

            if (!Directory.Exists(noteFolder))
                Directory.CreateDirectory(noteFolder);

            // اسم یکتا برای فایل
            string fileName = Guid.NewGuid() + Path.GetExtension(sourceFilePath);

            string destPath = Path.Combine(noteFolder, fileName);

            File.Copy(sourceFilePath, destPath, true);

            // مسیر قابل ذخیره در DB
            return Path.Combine("Images", noteId.ToString(), fileName);
        }

        public static string GetFullPath(string relativePath)
        {
            return Path.Combine(baseFolder, relativePath.Replace("Images\\", "").Replace("Images/", ""));
        }

        public static void DeleteImage(string relativePath)
        {
            string fullPath = GetFullPath(relativePath);

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}