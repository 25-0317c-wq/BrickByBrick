using System.Windows.Input;
using BrickByBrick.Models;

namespace BrickByBrick.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        // -----------------------------------------------------------------
        // 1. Private Backing Fields (State Management)
        // -----------------------------------------------------------------
        private UserRole _currentUserRole = UserRole.Pending;
        private string _statusMessage = "Awaiting secure credential token...";

        // -----------------------------------------------------------------
        // 2. Public Data-Bound Properties (Observed by the View)
        // -----------------------------------------------------------------
        public UserRole CurrentUserRole
        {
            get => _currentUserRole;
            set
            {
                _currentUserRole = value;
                // Notify the UI to instantly update visibility or dashboard states
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                // Notify the UI to update the status alert message box
                OnPropertyChanged();
            }
        }

        // -----------------------------------------------------------------
        // 3. MVVM Commands (Replaces traditional button click events)
        // -----------------------------------------------------------------
        public ICommand SimulateLoginCommand { get; }

        // -----------------------------------------------------------------
        // 4. Constructor
        // -----------------------------------------------------------------
        public MainViewModel()
        {
            // Bind the button commands to the execution logic method
            SimulateLoginCommand = new RelayCommand(ExecuteLoginSimulation);
        }

        // -----------------------------------------------------------------
        // 5. Business Logic (Zero View/Code-Behind Dependency)
        // -----------------------------------------------------------------
        private void ExecuteLoginSimulation(object? parameter)
        {
            if (parameter is string roleTarget)
            {
                switch (roleTarget.ToLower())
                {
                    case "admin":
                        CurrentUserRole = UserRole.Admin;
                        StatusMessage = "Authenticated via Google OAuth: Welcome System Administrator.";
                        break;
                    case "manager":
                        CurrentUserRole = UserRole.Manager;
                        StatusMessage = "Authenticated via Google OAuth: Welcome Project Manager.";
                        break;
                    case "employee":
                        CurrentUserRole = UserRole.Employee;
                        StatusMessage = "Authenticated via Google OAuth: Welcome Field Tracker Log.";
                        break;
                    case "client":
                        CurrentUserRole = UserRole.Client;
                        StatusMessage = "Authenticated via Google OAuth: Welcome External Client Dashboard.";
                        break;
                    default:
                        CurrentUserRole = UserRole.Pending;
                        StatusMessage = "Access Denied: Token verification failed.";
                        break;
                }
            }
        }
    }
}