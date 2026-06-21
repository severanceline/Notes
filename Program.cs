using Noots.DataAccess;
using Noots.Forms;
using Noots.Session;
using Noots.Utilities;

namespace Noots
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var savedUserId = LoginStorage.GetSavedUser();

            if (savedUserId != null)
            {
                // ????? ???? ?? ??????? user ?? ?????
                var repo = new UserRepository();
                var user = repo.GetUserById(savedUserId.Value);

                if (user != null)
                {
                    UserSession.Login(user);
                    Application.Run(new MainForm());
                    return;
                }
            }
            Application.Run(new SigninForm());
        }
    }
}