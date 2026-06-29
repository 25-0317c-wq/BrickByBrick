using System.Windows;
using BrickByBrick.Models;
using BrickByBrick.Services;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// The real login screen. Sign In checks the typed email/password against
    /// the shared UserStore via AuthService, and on success opens the shell
    /// window matching that user's role.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide any previous error before trying again.
            LoginErrorText.Visibility = Visibility.Collapsed;

            string email = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var user = AuthService.TryLogin(email, password);

            if (user == null)
            {
                LoginErrorText.Visibility = Visibility.Visible;
                return;
            }

            OpenShellForRole(user.Role);

            // Close the login window once the appropriate dashboard is open.
            this.Close();
        }

        private void OpenShellForRole(UserRole role)
        {
            Window shell = role switch
            {
                UserRole.Admin => new AdminShellWindow(),
                UserRole.Manager => new ManagerShellWindow(),
                UserRole.Employee => new EmployeeShellWindow(),
                UserRole.Client => new ClientShellWindow(),
                _ => new RoleDemoWindow() // Fallback for UserRole.Pending or anything unexpected
            };

            shell.Show();
        }

        private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var signUpWindow = new SignUpWindow();
            signUpWindow.Show();
            this.Close();
        }
    }
}
