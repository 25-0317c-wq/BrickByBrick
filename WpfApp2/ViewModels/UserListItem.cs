using System.Collections.ObjectModel;
using BrickByBrick.Models;

namespace BrickByBrick.ViewModels
{
    /// <summary>
    /// Lightweight row model for displaying a user in the Admin Dashboard's user list.
    /// UI-only for now — not tied to persistence.
    /// </summary>
    public class UserListItem : BaseViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string InitialsBadge { get; set; } = string.Empty;

        private UserRole _role;
        public UserRole Role
        {
            get => _role;
            set
            {
                _role = value;
                OnPropertyChanged();
            }
        }

        private bool _isActive = true;
        public bool IsActive
        {
            get => _isActive;
            set
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }
    }
}