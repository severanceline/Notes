using Noots.Models;

namespace Noots.Session
{
    public static class UserSession
    {
        // نگهداری اطلاعات کاربر لاگین شده
        public static User CurrentUser { get; private set; }

        public static bool IsLoggedIn => CurrentUser != null;

        public static void Login(User user)
        {
            CurrentUser = user;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
