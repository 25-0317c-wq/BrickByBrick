using System.Windows.Input;
using BrickByBrick.Models;

namespace BrickByBrick.ViewModels
{
    public class MainViewModel : BaseViewModel
    {

        private UserRole _currentUserRole = UserRole.Pending;
        private string _statusMessage = "Awaiting secure credential token...";


        public UserRole CurrentUserRole
        {
            get => _currentUserRole;
            set
            {
                _currentUserRole = value;

                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;

                OnPropertyChanged();
            }
        }


        public ICommand SimulateLoginCommand { get; }


        public MainViewModel()
        {

            SimulateLoginCommand = new RelayCommand(ExecuteLoginSimulation);
        }


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