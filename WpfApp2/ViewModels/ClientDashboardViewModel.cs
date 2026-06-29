using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BrickByBrick.Models;
using BrickByBrick.Services;

namespace BrickByBrick.ViewModels
{
    /// <summary>
    /// Drives the External Client's dashboard: submit new project proposals
    /// and track the status (Pending/Approved/Rejected) of proposals already submitted,
    /// including progress updates once a proposal is approved and work begins.
    /// Reads from and writes to the shared ProposalStore — a new submission is
    /// immediately visible in the Manager's approval queue, and a Manager's
    /// approve/reject decision is immediately visible here.
    /// </summary>
    public class ClientDashboardViewModel : BaseViewModel
    {
        // For now there's a single logged-in Client context (Amelia Tan),
        // matching the dummy user seeded in UserStore. When real auth/login
        // exists, this should come from the signed-in user instead of a constant.
        private const string CurrentClientName = "Amelia Tan";

        public ObservableCollection<ProjectProposal> MyProposals { get; }

        public int PendingCount => MyProposals.Count(p => p.Status == ProposalStatus.Pending);
        public int ApprovedCount => MyProposals.Count(p => p.Status == ProposalStatus.Approved);
        public int RejectedCount => MyProposals.Count(p => p.Status == ProposalStatus.Rejected);

        // -----------------------------------------------------------------
        // Selected proposal (for viewing progress updates)
        // -----------------------------------------------------------------
        private ProjectProposal? _selectedProposal;
        public ProjectProposal? SelectedProposal
        {
            get => _selectedProposal;
            set
            {
                _selectedProposal = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedProposal));
            }
        }

        public bool HasSelectedProposal => SelectedProposal != null;

        public ICommand SelectProposalCommand { get; }

        // -----------------------------------------------------------------
        // New proposal submission dialog
        // -----------------------------------------------------------------
        private bool _isSubmitDialogOpen;
        public bool IsSubmitDialogOpen
        {
            get => _isSubmitDialogOpen;
            set { _isSubmitDialogOpen = value; OnPropertyChanged(); }
        }

        private string _newTitle = string.Empty;
        public string NewTitle
        {
            get => _newTitle;
            set { _newTitle = value; OnPropertyChanged(); }
        }

        private string _newDescription = string.Empty;
        public string NewDescription
        {
            get => _newDescription;
            set { _newDescription = value; OnPropertyChanged(); }
        }

        private string _newLocation = string.Empty;
        public string NewLocation
        {
            get => _newLocation;
            set { _newLocation = value; OnPropertyChanged(); }
        }

        public ICommand OpenSubmitDialogCommand { get; }
        public ICommand SubmitProposalCommand { get; }
        public ICommand CancelSubmitCommand { get; }

        private readonly HashSet<ProjectProposal> _watchedProposals = new();

        public ClientDashboardViewModel()
        {
            MyProposals = new ObservableCollection<ProjectProposal>();

            RefreshMyProposals();
            AttachStatusWatchers();

            // Keep this list in sync whenever the shared store changes —
            // e.g. a Manager approves/rejects, or this client submits a new one
            // from another window.
            ProposalStore.Instance.Proposals.CollectionChanged += (s, e) =>
            {
                AttachStatusWatchers();
                RefreshMyProposals();
            };

            SelectedProposal = MyProposals.FirstOrDefault();

            SelectProposalCommand = new RelayCommand(ExecuteSelectProposal);
            OpenSubmitDialogCommand = new RelayCommand(_ => ExecuteOpenSubmitDialog());
            SubmitProposalCommand = new RelayCommand(_ => ExecuteSubmitProposal());
            CancelSubmitCommand = new RelayCommand(_ => ExecuteCancelSubmit());
        }

        private void AttachStatusWatchers()
        {
            foreach (var proposal in ProposalStore.Instance.Proposals)
            {
                if (_watchedProposals.Contains(proposal)) continue;

                proposal.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(ProjectProposal.Status))
                    {
                        RefreshMyProposals();
                    }
                };
                _watchedProposals.Add(proposal);
            }
        }

        private void RefreshMyProposals()
        {
            var stillSelectedTitle = SelectedProposal?.Title;

            MyProposals.Clear();

            var mine = ProposalStore.Instance.Proposals
                .Where(p => p.SubmittedByClient == CurrentClientName)
                .OrderByDescending(p => p.SubmittedOn);

            foreach (var proposal in mine)
            {
                MyProposals.Add(proposal);
            }

            OnPropertyChanged(nameof(PendingCount));
            OnPropertyChanged(nameof(ApprovedCount));
            OnPropertyChanged(nameof(RejectedCount));

            if (stillSelectedTitle != null)
            {
                var stillThere = MyProposals.FirstOrDefault(p => p.Title == stillSelectedTitle);
                if (stillThere != null)
                {
                    SelectedProposal = stillThere;
                }
            }
        }

        private void ExecuteSelectProposal(object? parameter)
        {
            if (parameter is ProjectProposal proposal)
            {
                SelectedProposal = proposal;
            }
        }

        private void ExecuteOpenSubmitDialog()
        {
            NewTitle = string.Empty;
            NewDescription = string.Empty;
            NewLocation = string.Empty;
            IsSubmitDialogOpen = true;
        }

        private void ExecuteCancelSubmit()
        {
            IsSubmitDialogOpen = false;
        }

        private void ExecuteSubmitProposal()
        {
            if (string.IsNullOrWhiteSpace(NewTitle) || string.IsNullOrWhiteSpace(NewDescription))
            {
                return;
            }

            var proposal = new ProjectProposal
            {
                Title = NewTitle,
                Description = NewDescription,
                Location = string.IsNullOrWhiteSpace(NewLocation) ? "Not specified" : NewLocation,
                SubmittedByClient = CurrentClientName,
                Status = ProposalStatus.Pending,
                SubmittedOn = DateTime.Now
            };


            ProposalStore.Instance.Proposals.Insert(0, proposal);


            SelectedProposal = proposal;

            IsSubmitDialogOpen = false;
        }
    }
}
