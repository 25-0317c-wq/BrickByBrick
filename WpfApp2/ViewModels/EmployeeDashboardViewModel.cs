using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BrickByBrick.Models;
using BrickByBrick.Services;

namespace BrickByBrick.ViewModels
{
    /// <summary>
    /// Drives the Field Employee's dashboard: approved projects assigned to them,
    /// and the ability to post progress updates that Clients can see.
    /// Reads from the shared ProposalStore — once a Manager approves a proposal
    /// assigned to this employee, it appears here automatically.
    /// </summary>
    public class EmployeeDashboardViewModel : BaseViewModel
    {
        // For now there's a single logged-in Employee context (Carlo Santos),
        // matching the dummy user seeded in UserStore. When real auth/login
        // exists, this should come from the signed-in user instead of a constant.
        private const string CurrentEmployeeName = "Carlo Santos";

        public ObservableCollection<ProjectProposal> AssignedProjects { get; }

        // -----------------------------------------------------------------
        // Selected project + new update form state
        // -----------------------------------------------------------------
        private ProjectProposal? _selectedProject;
        public ProjectProposal? SelectedProject
        {
            get => _selectedProject;
            set
            {
                _selectedProject = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedProject));
            }
        }

        public bool HasSelectedProject => SelectedProject != null;

        private string _newUpdateNote = string.Empty;
        public string NewUpdateNote
        {
            get => _newUpdateNote;
            set { _newUpdateNote = value; OnPropertyChanged(); }
        }

        private int _newUpdatePercent;
        public int NewUpdatePercent
        {
            get => _newUpdatePercent;
            set { _newUpdatePercent = value; OnPropertyChanged(); }
        }

        public ICommand PostUpdateCommand { get; }
        public ICommand SelectProjectCommand { get; }

        public int ActiveProjectCount => AssignedProjects.Count(p => p.Status == ProposalStatus.Approved);

        public EmployeeDashboardViewModel()
        {
            AssignedProjects = new ObservableCollection<ProjectProposal>();

            RefreshAssignedProjects();

            // Keep this list in sync whenever the shared store changes —
            // e.g. a new proposal is added/removed from the store.
            ProposalStore.Instance.Proposals.CollectionChanged += (s, e) =>
            {
                AttachStatusWatchers();
                RefreshAssignedProjects();
            };

            // Also watch each existing proposal's Status directly, since a
            // Manager approving a Pending proposal changes a property on an
            // existing item rather than adding/removing from the collection.
            AttachStatusWatchers();

            SelectedProject = AssignedProjects.FirstOrDefault();

            PostUpdateCommand = new RelayCommand(_ => ExecutePostUpdate());
            SelectProjectCommand = new RelayCommand(ExecuteSelectProject);
        }

        private readonly System.Collections.Generic.HashSet<ProjectProposal> _watchedProposals = new();

        private void AttachStatusWatchers()
        {
            foreach (var proposal in ProposalStore.Instance.Proposals)
            {
                if (_watchedProposals.Contains(proposal)) continue;

                proposal.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(ProjectProposal.Status) ||
                        e.PropertyName == nameof(ProjectProposal.AssignedEmployee))
                    {
                        RefreshAssignedProjects();
                    }
                };
                _watchedProposals.Add(proposal);
            }
        }

        private void RefreshAssignedProjects()
        {
            var stillSelectedTitle = SelectedProject?.Title;

            AssignedProjects.Clear();

            var relevant = ProposalStore.Instance.Proposals
                .Where(p => p.Status == ProposalStatus.Approved && p.AssignedEmployee == CurrentEmployeeName)
                .OrderByDescending(p => p.SubmittedOn);

            foreach (var project in relevant)
            {
                AssignedProjects.Add(project);
            }

            OnPropertyChanged(nameof(ActiveProjectCount));

            // Try to keep the same project selected across a refresh, if it's still in the list.
            if (stillSelectedTitle != null)
            {
                var stillThere = AssignedProjects.FirstOrDefault(p => p.Title == stillSelectedTitle);
                if (stillThere != null)
                {
                    SelectedProject = stillThere;
                }
            }
        }

        private void ExecuteSelectProject(object? parameter)
        {
            if (parameter is ProjectProposal project)
            {
                SelectedProject = project;
            }
        }

        private void ExecutePostUpdate()
        {
            if (SelectedProject == null || string.IsNullOrWhiteSpace(NewUpdateNote))
            {
                return;
            }

            SelectedProject.ProgressUpdates.Add(new ProgressUpdate
            {
                Note = NewUpdateNote,
                PostedBy = SelectedProject.AssignedEmployee,
                PostedOn = DateTime.Now,
                PercentComplete = NewUpdatePercent
            });

            // Force a refresh of bindings that read from the collection's latest entry.
            OnPropertyChanged(nameof(SelectedProject));

            NewUpdateNote = string.Empty;
        }
    }
}
