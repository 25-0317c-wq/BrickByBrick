using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BrickByBrick.Models;
using BrickByBrick.Services;

namespace BrickByBrick.ViewModels
{
    /// <summary>
    /// Drives the "Create Account" sign-up form. Verifies the typed email
    /// with Hunter.io before allowing account creation, then writes the new
    /// user to SQL Server via UserStore.AddUser.
    /// </summary>
    public class SignUpViewModel : BaseViewModel
    {
        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _confirmPassword = string.Empty;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set { _confirmPassword = value; OnPropertyChanged(); }
        }

        private UserRole _role = UserRole.Client;
        public UserRole Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }

        public ObservableCollection<UserRole> RoleOptions { get; } = new ObservableCollection<UserRole>
        {
            UserRole.Client,
            UserRole.Employee,
            UserRole.Manager,
            UserRole.Admin
        };

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasStatusMessage)); }
        }

        public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

        private bool _isStatusError;
        public bool IsStatusError
        {
            get => _isStatusError;
            set { _isStatusError = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand CreateAccountCommand { get; }

        /// <summary>Raised after a successful sign-up, so the view can navigate back to login.</summary>
        public event EventHandler? AccountCreated;

        public SignUpViewModel()
        {
            CreateAccountCommand = new RelayCommand(_ => _ = ExecuteCreateAccountAsync());
        }

        private async System.Threading.Tasks.Task ExecuteCreateAccountAsync()
        {
            StatusMessage = string.Empty;
            IsStatusError = false;

            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ShowError("Please fill in your name, email, and password.");
                return;
            }

            if (Password != ConfirmPassword)
            {
                ShowError("Passwords don't match.");
                return;
            }

            if (Password.Length < 8)
            {
                ShowError("Password must be at least 8 characters.");
                return;
            }

            IsBusy = true;
            StatusMessage = "Verifying email address...";
            IsStatusError = false;

            var verification = await EmailVerificationService.VerifyAsync(Email);

            if (!verification.IsValid)
            {
                IsBusy = false;
                ShowError(verification.Message);
                return;
            }

            try
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(Password);

                var newUser = new UserListItem
                {
                    FullName = FullName.Trim(),
                    Email = Email.Trim(),
                    Role = Role,
                    IsActive = true,
                    Department = "Unassigned"
                };

                UserStore.Instance.AddUser(newUser, passwordHash);

                StatusMessage = "Account created! You can now sign in.";
                IsStatusError = false;
                IsBusy = false;

                AccountCreated?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                IsBusy = false;
                ShowError($"Couldn't create your account: {ex.Message}");
            }
        }

        private void ShowError(string message)
        {
            StatusMessage = message;
            IsStatusError = true;
        }
    }
}
