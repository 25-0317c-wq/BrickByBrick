using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BrickByBrick.Models;
using BrickByBrick.Services;

namespace BrickByBrick.ViewModels
{
    /// <summary>
    /// Drives the Project Manager's dashboard: a pending-approval queue for
    /// client-submitted proposals, plus a view of already-approved/published projects.
    /// Reads from the shared ProposalStore, so approving/rejecting here is
    /// immediately visible in the Client and Employee dashboards too.
    /// </summary>
    public class ManagerDashboardViewModel : BaseViewModel
    {
        public ObservableCollection<ProjectProposal> AllProposals => ProposalStore.Instance.Proposals;

        public ObservableCollection<ProjectProposal> PendingProposals { get; }
        public ObservableCollection<ProjectProposal> ApprovedProjects { get; }
        public ObservableCollection<ProjectProposal> RejectedProposals { get; }

        public int PendingCount => PendingProposals.Count;
        public int ApprovedCount => ApprovedProjects.Count;
        public int RejectedCount => RejectedProposals.Count;

        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }

        // -----------------------------------------------------------------
        // Approve dialog state (requires picking an Employee before confirming)
        // -----------------------------------------------------------------
        private bool _isApproveDialogOpen;
        public bool IsApproveDialogOpen
        {
            get => _isApproveDialogOpen;
            set { _isApproveDialogOpen = value; OnPropertyChanged(); }
        }

        private ProjectProposal? _proposalPendingApproval;
        public ProjectProposal? ProposalPendingApproval
        {
            get => _proposalPendingApproval;
            set { _proposalPendingApproval = value; OnPropertyChanged(); }
        }

        /// <summary>Employees available to assign, sourced live from the shared UserStore.</summary>
        public ObservableCollection<UserListItem> AvailableEmployees =>
            new ObservableCollection<UserListItem>(
                UserStore.Instance.Users.Where(u => u.Role == UserRole.Employee && u.IsActive));

        private UserListItem? _selectedEmployee;
        public UserListItem? SelectedEmployee
        {
            get => _selectedEmployee;
            set { _selectedEmployee = value; OnPropertyChanged(); }
        }

        public ICommand ConfirmApproveCommand { get; }
        public ICommand CancelApproveCommand { get; }

        // -----------------------------------------------------------------
        // Reject dialog state (asks for a brief reason before rejecting)
        // -----------------------------------------------------------------
        private bool _isRejectDialogOpen;
        public bool IsRejectDialogOpen
        {
            get => _isRejectDialogOpen;
            set { _isRejectDialogOpen = value; OnPropertyChanged(); }
        }

        private ProjectProposal? _proposalPendingRejection;

        private string _rejectReason = string.Empty;
        public string RejectReason
        {
            get => _rejectReason;
            set { _rejectReason = value; OnPropertyChanged(); }
        }

        public ICommand ConfirmRejectCommand { get; }
        public ICommand CancelRejectCommand { get; }

        // -----------------------------------------------------------------
        // Add Project dialog (Manager-initiated project creation)
        // -----------------------------------------------------------------
        private bool _isAddProjectDialogOpen;
        public bool IsAddProjectDialogOpen
        {
            get => _isAddProjectDialogOpen;
            set { _isAddProjectDialogOpen = value; OnPropertyChanged(); }
        }

        public AddProjectFormViewModel NewProjectForm { get; }

        public ICommand OpenAddProjectDialogCommand { get; }
        public ICommand SaveNewProjectCommand { get; }
        public ICommand CancelAddProjectCommand { get; }

        // -----------------------------------------------------------------
        // Project detail navigation — set when a project card is clicked,
        // read by the hosting shell window to swap to ProjectDetailView.
        // -----------------------------------------------------------------
        private ProjectProposal? _selectedProjectForDetail;
        public ProjectProposal? SelectedProjectForDetail
        {
            get => _selectedProjectForDetail;
            set { _selectedProjectForDetail = value; OnPropertyChanged(); }
        }

        public ICommand ViewProjectDetailCommand { get; }

        public ManagerDashboardViewModel()
        {
            PendingProposals = new ObservableCollection<ProjectProposal>();
            ApprovedProjects = new ObservableCollection<ProjectProposal>();
            RejectedProposals = new ObservableCollection<ProjectProposal>();
            NewProjectForm = new AddProjectFormViewModel();

            // Keep the bucketed lists in sync whenever the shared store's
            // proposal collection changes (e.g. a Client submits a new one).
            AllProposals.CollectionChanged += (s, e) => RefreshBuckets();

            RefreshBuckets();

            ApproveCommand = new RelayCommand(ExecuteRequestApprove);
            ConfirmApproveCommand = new RelayCommand(_ => ExecuteConfirmApprove());
            CancelApproveCommand = new RelayCommand(_ => ExecuteCancelApprove());

            RejectCommand = new RelayCommand(ExecuteRequestReject);
            ConfirmRejectCommand = new RelayCommand(_ => ExecuteConfirmReject());
            CancelRejectCommand = new RelayCommand(_ => ExecuteCancelReject());

            OpenAddProjectDialogCommand = new RelayCommand(_ => ExecuteOpenAddProjectDialog());
            SaveNewProjectCommand = new RelayCommand(_ => ExecuteSaveNewProject());
            CancelAddProjectCommand = new RelayCommand(_ => ExecuteCancelAddProject());

            ViewProjectDetailCommand = new RelayCommand(ExecuteViewProjectDetail);
        }

        private void RefreshBuckets()
        {
            PendingProposals.Clear();
            ApprovedProjects.Clear();
            RejectedProposals.Clear();

            foreach (var proposal in AllProposals.OrderByDescending(p => p.SubmittedOn))
            {
                switch (proposal.Status)
                {
                    case ProposalStatus.Pending:
                        PendingProposals.Add(proposal);
                        break;
                    case ProposalStatus.Approved:
                        ApprovedProjects.Add(proposal);
                        break;
                    case ProposalStatus.Rejected:
                        RejectedProposals.Add(proposal);
                        break;
                }
            }

            OnPropertyChanged(nameof(PendingCount));
            OnPropertyChanged(nameof(ApprovedCount));
            OnPropertyChanged(nameof(RejectedCount));
        }

        private void ExecuteRequestApprove(object? parameter)
        {
            if (parameter is ProjectProposal proposal)
            {
                ProposalPendingApproval = proposal;
                SelectedEmployee = AvailableEmployees.FirstOrDefault();
                IsApproveDialogOpen = true;
            }
        }

        private void ExecuteConfirmApprove()
        {
            if (ProposalPendingApproval != null && SelectedEmployee != null)
            {
                ProposalPendingApproval.AssignedEmployee = SelectedEmployee.FullName;
                ProposalPendingApproval.Status = ProposalStatus.Approved;
                RefreshBuckets();
            }

            IsApproveDialogOpen = false;
            ProposalPendingApproval = null;
            SelectedEmployee = null;
        }

        private void ExecuteCancelApprove()
        {
            IsApproveDialogOpen = false;
            ProposalPendingApproval = null;
            SelectedEmployee = null;
        }

        private void ExecuteRequestReject(object? parameter)
        {
            if (parameter is ProjectProposal proposal)
            {
                _proposalPendingRejection = proposal;
                RejectReason = string.Empty;
                IsRejectDialogOpen = true;
            }
        }

        private void ExecuteConfirmReject()
        {
            if (_proposalPendingRejection != null)
            {
                _proposalPendingRejection.Status = ProposalStatus.Rejected;
                // RejectReason is captured for display purposes; not persisted onto the
                // model yet since ProjectProposal doesn't have a reason field at this stage.
                RefreshBuckets();
            }

            IsRejectDialogOpen = false;
            _proposalPendingRejection = null;
        }

        private void ExecuteCancelReject()
        {
            IsRejectDialogOpen = false;
            _proposalPendingRejection = null;
        }

        private void ExecuteOpenAddProjectDialog()
        {
            NewProjectForm.Reset();
            IsAddProjectDialogOpen = true;
        }

        private void ExecuteCancelAddProject()
        {
            IsAddProjectDialogOpen = false;
        }

        private void ExecuteSaveNewProject()
        {
            if (string.IsNullOrWhiteSpace(NewProjectForm.ProjectName))
            {
                return;
            }

            var project = new ProjectProposal
            {
                Title = NewProjectForm.ProjectName,
                Description = $"{NewProjectForm.ProjectType} project — {NewProjectForm.ProjectPhase}",
                Phase = string.IsNullOrWhiteSpace(NewProjectForm.ProjectPhase) ? "Planning" : NewProjectForm.ProjectPhase,
                Location = string.IsNullOrWhiteSpace(NewProjectForm.ProjectLocation) ? "Not specified" : NewProjectForm.ProjectLocation,
                SubmittedByClient = string.IsNullOrWhiteSpace(NewProjectForm.Client) ? "Internal" : NewProjectForm.Client,
                ProjectType = NewProjectForm.ProjectType,
                BuildingCategory = NewProjectForm.BuildingCategory,
                Status = ProposalStatus.Approved, // Manager-created projects are already approved/published.
                SubmittedOn = DateTime.Now
            };

            ProposalStore.Instance.Proposals.Insert(0, project);

            IsAddProjectDialogOpen = false;
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
