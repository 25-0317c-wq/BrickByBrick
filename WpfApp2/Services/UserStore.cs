using System.Collections.ObjectModel;
using BrickByBrick.Models;
using BrickByBrick.ViewModels;

namespace BrickByBrick.Services
{
    /// <summary>
    /// Single shared, in-memory source of truth for all registered users and their roles.
    /// Admin's User Management screen reads from and writes to this same collection.
    /// Still UI-only / in-memory — resets on app restart.
    /// </summary>
    public sealed class UserStore
    {
        private static readonly UserStore _instance = new UserStore();
        public static UserStore Instance => _instance;

        public ObservableCollection<UserListItem> Users { get; }

        private UserStore()
        {
            Users = new ObservableCollection<UserListItem>();
            SeedDummyData();
        }

        private void SeedDummyData()
        {
            // Dummy password for every seeded account, for this UI-only login stage.
            const string demoPassword = "password123";

            Users.Add(new UserListItem { FullName = "Dana Reyes", Email = "dana.reyes@brickbybrick.com", Department = "Operations", InitialsBadge = "DR", Role = UserRole.Admin, IsActive = true, Password = demoPassword });
            Users.Add(new UserListItem { FullName = "Marcus Villar", Email = "marcus.villar@brickbybrick.com", Department = "Site Engineering", InitialsBadge = "MV", Role = UserRole.Manager, IsActive = true, Password = demoPassword });
            Users.Add(new UserListItem { FullName = "Lea Fernandez", Email = "lea.fernandez@brickbybrick.com", Department = "Site Engineering", InitialsBadge = "LF", Role = UserRole.Manager, IsActive = true, Password = demoPassword });
            Users.Add(new UserListItem { FullName = "Carlo Santos", Email = "carlo.santos@brickbybrick.com", Department = "Field Crew", InitialsBadge = "CS", Role = UserRole.Employee, IsActive = true, Password = demoPassword });
            Users.Add(new UserListItem { FullName = "Joy Dimaculangan", Email = "joy.dimaculangan@brickbybrick.com", Department = "Field Crew", InitialsBadge = "JD", Role = UserRole.Employee, IsActive = true, Password = demoPassword });
            Users.Add(new UserListItem { FullName = "Rafael Cruz", Email = "rafael.cruz@brickbybrick.com", Department = "Field Crew", InitialsBadge = "RC", Role = UserRole.Employee, IsActive = true, Password = demoPassword });
            Users.Add(new UserListItem { FullName = "Amelia Tan", Email = "amelia.tan@stonepeakdev.com", Department = "External Partner", InitialsBadge = "AT", Role = UserRole.Client, IsActive = true, Password = demoPassword });
            Users.Add(new UserListItem { FullName = "Brian Lo", Email = "brian.lo@harborbuild.com", Department = "External Partner", InitialsBadge = "BL", Role = UserRole.Client, IsActive = true, Password = demoPassword });
        }
    }
}
