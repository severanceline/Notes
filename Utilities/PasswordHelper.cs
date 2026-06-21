using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Noots.Utilities
{
    public static class PasswordHelper
    {
        // تبدیل پسورد ساده به هش (برای ثبت نام)
        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // بررسی پسورد وارد شده با هش دیتابیس (برای لاگین)
        public static bool VerifyPassword(string inputPassword, string hashFromDatabase)
        {
            string hashOfInput = HashPassword(inputPassword);
            return StringComparer.OrdinalIgnoreCase.Compare(hashOfInput, hashFromDatabase) == 0;
        }
    }
}
