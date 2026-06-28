using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BrickByBrick.Models
{
    /// <summary>
    /// Status of a project proposal as it moves through the approval workflow:
    /// Client submits (Pending) -> Manager reviews (Approved/Rejected) -> Employee works on it.
    /// </summary>
    public enum ProposalStatus
    {
        Pending,
        Approved,
        Rejected
    }

    /// <summary>
    /// Broad category of construction work, set when a project is created.
    /// </summary>
    public enum ProjectType
    {
        Commercial,
        Residential,
        Industrial,
        Renovation,
        Infrastructure
    }

    /// <summary>
    /// The kind of building/structure the project concerns.
    /// </summary>
    public enum BuildingCategory
    {
        Office,
        ShoppingMall,
        Hotel,
        RetailStore,
        ShoppingCenter,
        Residential,
        Warehouse
    }

    /// <summary>
    /// A single progress update posted by an Employee against an approved project.
    /// Shown to the Client as visibility into work being done.
    /// </summary>
    public class ProgressUpdate
    {
        public string Note { get; set; } = string.Empty;
        public string PostedBy { get; set; } = string.Empty;
        public DateTime PostedOn { get; set; } = DateTime.Now;
        public int PercentComplete { get; set; }
    }

    /// <summary>
    /// Shared project proposal record used by Client (submission/tracking),
    /// Manager (approval queue), and Employee (work view) dashboards.
    /// Implements INotifyPropertyChanged so that when one dashboard changes
    /// Status or AssignedEmployee on a shared instance (from ProposalStore),
    /// every other window currently displaying that same proposal updates
    /// immediately — no manual refresh needed.
    /// Still UI-only / in-memory — not persisted to disk or a database.
    /// </summary>
    public class ProjectProposal : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SubmittedByClient { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime SubmittedOn { get; set; } = DateTime.Now;

        /// <summary>Current phase label, e.g. "Foundation", "Framing", "Finishing".</summary>
        public string Phase { get; set; } = "Planning";

        public ProjectType ProjectType { get; set; } = ProjectType.Commercial;
        public BuildingCategory BuildingCategory { get; set; } = BuildingCategory.Office;

        /// <summary>
        /// Team members working on this project (names only, for now).
        /// The AssignedEmployee is always included; this allows for future
        /// multi-person teams without changing the approval/assignment flow.
        /// </summary>
        public ObservableCollection<string> Team { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Placeholder for a project image. No real image upload yet —
        /// this is an emoji/icon used as a stand-in visual on the detail view.
        /// </summary>
        public string ImagePlaceholderEmoji { get; set; } = "🏗";

        private ProposalStatus _status = ProposalStatus.Pending;
        public ProposalStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged();
            }
        }

        private string _assignedEmployee = string.Empty;

        /// <summary>Set once a Manager approves and an Employee is assigned to it.</summary>
        public string AssignedEmployee
        {
            get => _assignedEmployee;
            set
            {
                _assignedEmployee = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProgressUpdate> ProgressUpdates { get; } = new ObservableCollection<ProgressUpdate>();

        public ProjectProposal()
        {
            // Whenever a new progress update is posted, OverallPercentComplete
            // needs to notify bound UI too, since it's a derived/computed property.
            ProgressUpdates.CollectionChanged += (s, e) => OnPropertyChanged(nameof(OverallPercentComplete));
        }

        public int OverallPercentComplete
        {
            get
            {
                if (ProgressUpdates.Count == 0) return 0;
                var latest = ProgressUpdates[ProgressUpdates.Count - 1];
                return latest.PercentComplete;
            }
        }
    }
}


