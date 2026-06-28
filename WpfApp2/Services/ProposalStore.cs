using System;
using System.Collections.ObjectModel;
using BrickByBrick.Models;

namespace BrickByBrick.Services
{
    /// <summary>
    /// Single shared, in-memory source of truth for all project proposals.
    /// Manager, Employee, and Client dashboards all read from and write to
    /// this same collection, so an action in one (e.g. Manager approving a
    /// proposal) is immediately visible in the others via ObservableCollection
    /// change notifications.
    ///
    /// This is still UI-only / in-memory — nothing here is persisted to disk
    /// or a database. Restarting the app resets everything back to the seed data.
    /// </summary>
    public sealed class ProposalStore
    {
        private static readonly ProposalStore _instance = new ProposalStore();
        public static ProposalStore Instance => _instance;

        public ObservableCollection<ProjectProposal> Proposals { get; }

        private ProposalStore()
        {
            Proposals = new ObservableCollection<ProjectProposal>();
            SeedDummyData();
        }

        private void SeedDummyData()
        {
            var riverside = new ProjectProposal
            {
                Title = "Riverside Tower — Phase 2",
                Description = "Structural framing and exterior cladding for the east wing.",
                SubmittedByClient = "Amelia Tan",
                Location = "Riverside District, Block 12",
                Status = ProposalStatus.Approved,
                AssignedEmployee = "Carlo Santos",
                SubmittedOn = DateTime.Now.AddDays(-18),
                Phase = "Structural Framing",
                ProjectType = ProjectType.Commercial,
                BuildingCategory = BuildingCategory.Office,
                ImagePlaceholderEmoji = "🏢"
            };
            riverside.Team.Add("Marcus Villar");
            riverside.Team.Add("Carlo Santos");
            riverside.ProgressUpdates.Add(new ProgressUpdate { Note = "Foundation work completed and inspected.", PostedBy = "Carlo Santos", PostedOn = DateTime.Now.AddDays(-10), PercentComplete = 25 });
            riverside.ProgressUpdates.Add(new ProgressUpdate { Note = "Structural framing underway on floors 1-4.", PostedBy = "Carlo Santos", PostedOn = DateTime.Now.AddDays(-3), PercentComplete = 45 });
            Proposals.Add(riverside);

            var harborview = new ProjectProposal
            {
                Title = "Harborview Complex — Renovation",
                Description = "Interior renovation of common areas and lobby modernization.",
                SubmittedByClient = "Amelia Tan",
                Location = "Harborview Complex, Building C",
                Status = ProposalStatus.Approved,
                AssignedEmployee = "Carlo Santos",
                SubmittedOn = DateTime.Now.AddDays(-30),
                Phase = "Interior Finishing",
                ProjectType = ProjectType.Renovation,
                BuildingCategory = BuildingCategory.ShoppingMall,
                ImagePlaceholderEmoji = "🏬"
            };
            harborview.Team.Add("Lea Fernandez");
            harborview.Team.Add("Carlo Santos");
            harborview.ProgressUpdates.Add(new ProgressUpdate { Note = "Demolition of old lobby fixtures complete.", PostedBy = "Carlo Santos", PostedOn = DateTime.Now.AddDays(-20), PercentComplete = 15 });
            harborview.ProgressUpdates.Add(new ProgressUpdate { Note = "New flooring installed, lighting fixtures next.", PostedBy = "Carlo Santos", PostedOn = DateTime.Now.AddDays(-6), PercentComplete = 60 });
            Proposals.Add(harborview);

            var crane = new ProjectProposal
            {
                Title = "Crane Maintenance — Site B",
                Description = "Scheduled maintenance and safety inspection of tower crane unit 3.",
                SubmittedByClient = "Brian Lo",
                Location = "Site B — Foundation Crew",
                Status = ProposalStatus.Approved,
                AssignedEmployee = "Carlo Santos",
                SubmittedOn = DateTime.Now.AddDays(-5),
                Phase = "Inspection",
                ProjectType = ProjectType.Industrial,
                BuildingCategory = BuildingCategory.Warehouse,
                ImagePlaceholderEmoji = "🏗"
            };
            crane.Team.Add("Carlo Santos");
            crane.ProgressUpdates.Add(new ProgressUpdate { Note = "Initial inspection complete, parts on order.", PostedBy = "Carlo Santos", PostedOn = DateTime.Now.AddDays(-1), PercentComplete = 30 });
            Proposals.Add(crane);

            Proposals.Add(new ProjectProposal
            {
                Title = "Greenfield Logistics Hub",
                Description = "New 40,000 sq ft warehouse and distribution center with loading docks on the north side.",
                SubmittedByClient = "Amelia Tan",
                Location = "Greenfield Industrial Park, Lot 7",
                Status = ProposalStatus.Pending,
                SubmittedOn = DateTime.Now.AddDays(-2)
            });

            Proposals.Add(new ProjectProposal
            {
                Title = "Harbor Street Retail Renovation",
                Description = "Facade renovation and interior remodel for a 3-unit retail strip.",
                SubmittedByClient = "Brian Lo",
                Location = "Harbor Street, Units 4-6",
                Status = ProposalStatus.Pending,
                SubmittedOn = DateTime.Now.AddDays(-1)
            });

            Proposals.Add(new ProjectProposal
            {
                Title = "Maple Residences — Phase 1",
                Description = "12-unit townhouse development with shared green space.",
                SubmittedByClient = "Amelia Tan",
                Location = "Maple Grove, Parcel 22",
                Status = ProposalStatus.Pending,
                SubmittedOn = DateTime.Now.AddHours(-6)
            });

            Proposals.Add(new ProjectProposal
            {
                Title = "Lakeside Pavilion — Concept",
                Description = "Outdoor event pavilion with adjacent parking expansion.",
                SubmittedByClient = "Brian Lo",
                Location = "Lakeside Park, North Entrance",
                Status = ProposalStatus.Rejected,
                SubmittedOn = DateTime.Now.AddDays(-12)
            });
        }
    }
}
