using System.Windows;

namespace BrickByBrick.View
{
    /// <summary>
    /// Shared sign-out logic used by every role's shell window. Opens a fresh
    /// login screen and closes the calling window — kept in one place so all
    /// four shells behave identically and any future change (e.g. clearing a
    /// session token once real auth exists) only needs to happen once.
    /// </summary>
    public static class SessionHelper
    {
        public static void SignOut(Window currentWindow)
        {
            var loginWindow = new MainWindow();
            loginWindow.Show();

            currentWindow.Close();
        }
    }
}
