using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace Notes.DataAccess
{
    public static class DatabaseManager
    {
        // خواندن کانکشن استرینگ از فایل App.config
        public static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["NotesDb"].ConnectionString;

        // متدی برای دریافت یک شیء SqlConnection جدید
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}