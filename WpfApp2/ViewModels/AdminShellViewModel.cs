using System.Windows.Input;

namespace BrickByBrick.ViewModels
{
    /// <summary>
    /// Drives navigation state for the Admin shell's sidebar.
    /// UI-only — just tracks which section is currently selected.
    /// </summary>
    public class AdminShellViewModel : BaseViewModel
    {
        private string _activeSection = "Dashboard";
        public string ActiveSection
        {
            get => _activeSection;
            set
            {
                _activeSection = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateCommand { get; }

        public AdminShellViewModel()
        {
            NavigateCommand = new RelayCommand(ExecuteNavigate);
        }

        private void ExecuteNavigate(object? parameter)
        {
            if (parameter is string section)
            {
                ActiveSection = section;
            }
        }
    }
}