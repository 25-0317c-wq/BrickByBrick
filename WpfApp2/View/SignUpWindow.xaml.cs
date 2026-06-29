using System;
using System.Windows;
using System.Windows.Controls;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for SignUpWindow.xaml
    /// The "Create Account" sign-up form. Verifies the email via
    /// EmailVerificationService before creating the account in SQL Server.
    /// </summary>
    public partial class SignUpWindow : Window
    {
        private readonly SignUpViewModel _viewModel;

        public SignUpWindow()
        {
            InitializeComponent();

            _viewModel = new SignUpViewModel();
            DataContext = _viewModel;

            _viewModel.AccountCreated += OnAccountCreated;
        }

        // PasswordBox.Password can't be bound directly in WPF for security
        // reasons, so these handlers push the typed value into the ViewModel
        // manually, same approach as MainWindow's login form.
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.Password = PasswordBox.Password;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.ConfirmPassword = ConfirmPasswordBox.Password;
        }

        private void OnAccountCreated(object? sender, EventArgs e)
        {
            // Give the person a moment to see the success message before
            // automatically returning them to the login screen.
            var timer = new System.Windows.Threading.DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.5)
            };
            timer.Tick += (s, args) =>
            {
                timer.Stop();
                ReturnToLogin();
            };
            timer.Start();
        }

        private void BackToLoginButton_Click(object sender, RoutedEventArgs e)
        {
            ReturnToLogin();
        }

        private void ReturnToLogin()
        {
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
