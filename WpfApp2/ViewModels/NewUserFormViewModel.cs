using BrickByBrick.Models;

namespace BrickByBrick.ViewModels
{
    /// <summary>
    /// Backs the "Add User" dialog's input fields.
    /// Mirrors the fields a real registration/sign-up form would eventually collect,
    /// so this stays consistent once Admin-created accounts and self-registered
    /// accounts both feed into the same user list.
    /// </summary>
    public class NewUserFormViewModel : BaseViewModel
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

        private string _department = string.Empty;
        public string Department
        {
            get => _department;
            set { _department = value; OnPropertyChanged(); }
        }

        private UserRole _role = UserRole.Employee;
        public UserRole Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }

        /// <summary>Resets all fields back to their defaults, ready for the next entry.</summary>
        public void Reset()
        {
            FullName = string.Empty;
            Email = string.Empty;
            Department = string.Empty;
            Role = UserRole.Employee;
        }
    }
}
