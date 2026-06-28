using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BrickByBrick.Models;
using BrickByBrick.Services;

namespace BrickByBrick.ViewModels
{
    public class AdminDashboardViewModel : BaseViewModel
    {
        // -----------------------------------------------------------------
        // 1. User List — shared across the whole app via UserStore.
        //    Any change here (role updates, new users) is visible to every
        //    other ViewModel that reads from UserStore.Instance.Users.
        // -----------------------------------------------------------------
        public ObservableCollection<UserListItem> Users => UserStore.Instance.Users;

        // -----------------------------------------------------------------
        // 2. Role options available in the inline dropdown
        // -----------------------------------------------------------------
        public ObservableCollection<UserRole> AvailableRoles { get; }

        // -----------------------------------------------------------------
        // 3. Role Summary Counts (drives the top cards)
        // -----------------------------------------------------------------
        public int AdminCount => Users.Count(u => u.Role == UserRole.Admin);
        public int ManagerCount => Users.Count(u => u.Role == UserRole.Manager);
        public int EmployeeCount => Users.Count(u => u.Role == UserRole.Employee);
        public int ClientCount => Users.Count(u => u.Role == UserRole.Client);

        // -----------------------------------------------------------------
        // 4. Commands
        // -----------------------------------------------------------------
        public ICommand RoleChangedCommand { get; }
        public ICommand OpenAddUserDialogCommand { get; }
        public ICommand SaveNewUserCommand { get; }
        public ICommand CancelAddUserCommand { get; }

        // -----------------------------------------------------------------
        // 5. Add User dialog state
        // -----------------------------------------------------------------
        private bool _isAddUserDialogOpen;
        public bool IsAddUserDialogOpen
        {
            get => _isAddUserDialogOpen;
            set { _isAddUserDialogOpen = value; OnPropertyChanged(); }
        }

        public NewUserFormViewModel NewUserForm { get; }
        public ObservableCollection<UserRole> NewUserRoleOptions { get; }

        public AdminDashboardViewModel()
        {
            AvailableRoles = new ObservableCollection<UserRole>
            {
                UserRole.Admin,
                UserRole.Manager,
                UserRole.Employee,
                UserRole.Client
            };

            NewUserRoleOptions = AvailableRoles;
            NewUserForm = new NewUserFormViewModel();

            RoleChangedCommand = new RelayCommand(ExecuteRoleChanged);
            OpenAddUserDialogCommand = new RelayCommand(_ => ExecuteOpenAddUserDialog());
            SaveNewUserCommand = new RelayCommand(_ => ExecuteSaveNewUser());
            CancelAddUserCommand = new RelayCommand(_ => ExecuteCancelAddUser());

            AttachRoleWatcher();
        }

        private void AttachRoleWatcher()
        {
            // Recalculate summary counts whenever any user's role changes
            foreach (var user in Users)
            {
                AttachRoleWatcherTo(user);
            }
        }

        private void AttachRoleWatcherTo(UserListItem user)
        {
            user.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(UserListItem.Role))
                {
                    OnPropertyChanged(nameof(AdminCount));
                    OnPropertyChanged(nameof(ManagerCount));
                    OnPropertyChanged(nameof(EmployeeCount));
                    OnPropertyChanged(nameof(ClientCount));
                }
            };
        }

        private void ExecuteRoleChanged(object? parameter)
        {
            // UI-only for now. Hook this up to a real persistence/service call later.
            // parameter is the UserListItem whose role was just changed via the dropdown.
        }

        private void ExecuteOpenAddUserDialog()
        {
            NewUserForm.Reset();
            IsAddUserDialogOpen = true;
        }

        private void ExecuteCancelAddUser()
        {
            IsAddUserDialogOpen = false;
        }

        private void ExecuteSaveNewUser()
        {
            // Basic guard: require at least a name and email before adding.
            if (string.IsNullOrWhiteSpace(NewUserForm.FullName) || string.IsNullOrWhiteSpace(NewUserForm.Email))
            {
                return;
            }

            var initials = BuildInitials(NewUserForm.FullName);

            var newUser = new UserListItem
            {
                FullName = NewUserForm.FullName,
                Email = NewUserForm.Email,
                Department = string.IsNullOrWhiteSpace(NewUserForm.Department) ? "Unassigned" : NewUserForm.Department,
                InitialsBadge = initials,
                Role = NewUserForm.Role,
                IsActive = true
            };

            Users.Add(newUser);
            AttachRoleWatcherTo(newUser);

            OnPropertyChanged(nameof(AdminCount));
            OnPropertyChanged(nameof(ManagerCount));
            OnPropertyChanged(nameof(EmployeeCount));
            OnPropertyChanged(nameof(ClientCount));

            IsAddUserDialogOpen = false;
        }

        private static string BuildInitials(string fullName)
        {
            var parts = fullName.Trim().Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            if (parts.Length == 1) return parts[0].Substring(0, System.Math.Min(2, parts[0].Length)).ToUpperInvariant();
            return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
        }
    }
}
