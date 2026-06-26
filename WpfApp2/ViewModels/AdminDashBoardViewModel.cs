using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BrickByBrick.Models;

namespace BrickByBrick.ViewModels
{
    public class AdminDashboardViewModel : BaseViewModel
    {
        // -----------------------------------------------------------------
        // 1. User List (dummy data for now — UI only)
        // -----------------------------------------------------------------
        public ObservableCollection<UserListItem> Users { get; }

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

        public AdminDashboardViewModel()
        {
            AvailableRoles = new ObservableCollection<UserRole>
            {
                UserRole.Admin,
                UserRole.Manager,
                UserRole.Employee,
                UserRole.Client
            };

            Users = new ObservableCollection<UserListItem>
            {
                new UserListItem { FullName = "Dana Reyes",      Email = "dana.reyes@brickbybrick.com",      Department = "Operations",     InitialsBadge = "DR", Role = UserRole.Admin,    IsActive = true },
                new UserListItem { FullName = "Marcus Villar",    Email = "marcus.villar@brickbybrick.com",   Department = "Site Engineering",InitialsBadge = "MV", Role = UserRole.Manager,  IsActive = true },
                new UserListItem { FullName = "Lea Fernandez",    Email = "lea.fernandez@brickbybrick.com",   Department = "Site Engineering",InitialsBadge = "LF", Role = UserRole.Manager,  IsActive = true },
                new UserListItem { FullName = "Carlo Santos",     Email = "carlo.santos@brickbybrick.com",    Department = "Field Crew",      InitialsBadge = "CS", Role = UserRole.Employee, IsActive = true },
                new UserListItem { FullName = "Joy Dimaculangan",Email = "joy.dimaculangan@brickbybrick.com", Department = "Field Crew",      InitialsBadge = "JD", Role = UserRole.Employee, IsActive = true },
                new UserListItem { FullName = "Rafael Cruz",      Email = "rafael.cruz@brickbybrick.com",     Department = "Field Crew",      InitialsBadge = "RC", Role = UserRole.Employee, IsActive = false },
                new UserListItem { FullName = "Amelia Tan",       Email = "amelia.tan@stonepeakdev.com",      Department = "External Partner",InitialsBadge = "AT", Role = UserRole.Client,   IsActive = true },
                new UserListItem { FullName = "Brian Lo",         Email = "brian.lo@harborbuild.com",         Department = "External Partner",InitialsBadge = "BL", Role = UserRole.Client,   IsActive = true },
            };

            RoleChangedCommand = new RelayCommand(ExecuteRoleChanged);

            // Recalculate summary counts whenever any user's role changes
            foreach (var user in Users)
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
        }

        private void ExecuteRoleChanged(object? parameter)
        {
            // UI-only for now. Hook this up to a real persistence/service call later.
            // parameter is the UserListItem whose role was just changed via the dropdown.
        }
    }
}