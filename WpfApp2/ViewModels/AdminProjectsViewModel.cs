using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BrickByBrick.Models;
using BrickByBrick.Services;

namespace BrickByBrick.ViewModels
{
    /// <summary>
    /// Drives Admin's "Projects" section — a read-only overview list of every
    /// project in the shared ProposalStore, with the ability to click into
    /// the shared ProjectDetailView for any of them.
    /// </summary>
    public class AdminProjectsViewModel : BaseViewModel
    {
        public ObservableCollection<ProjectProposal> AllProjects => ProposalStore.Instance.Proposals;

        private ProjectProposal? _selectedProjectForDetail;
        public ProjectProposal? SelectedProjectForDetail
        {
            get => _selectedProjectForDetail;
            set { _selectedProjectForDetail = value; OnPropertyChanged(); }
        }

        public ICommand ViewProjectDetailCommand { get; }

        public AdminProjectsViewModel()
        {
            ViewProjectDetailCommand = new RelayCommand(ExecuteViewProjectDetail);
        }

        private void ExecuteViewProjectDetail(object? parameter)
        {
            if (parameter is ProjectProposal proposal)
            {
                SelectedProjectForDetail = proposal;
            }
        }
    }
}
