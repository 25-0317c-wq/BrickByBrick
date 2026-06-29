using System.Windows;
using BrickByBrick.Models;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for AdminShellWindow.xaml
    /// The Admin's main shell — sidebar navigation between Dashboard, User
    /// Management, Projects, and Settings. When a project is clicked from
    /// AdminProjectsView, this overlays the shared ProjectDetailView on top
    /// of the normal sidebar-driven content, and restores it on Back.
    /// </summary>
    public partial class AdminShellWindow : Window
    {
        private readonly AdminShellViewModel _shellViewModel;

        public AdminShellWindow()
        {
            InitializeComponent();

            _shellViewModel = new AdminShellViewModel();
            DataContext = _shellViewModel;

            // ContentHost's normal Content is driven by XAML DataTriggers bound
            // to ActiveSection. We additionally listen here so that whenever the
            // "Projects" section's AdminProjectsView raises a detail-view request,
            // we can swap ContentHost.Content to ProjectDetailView directly —
            // something a declarative DataTrigger can't do, since
            // ProjectDetailView needs a constructor argument (the project).
            ContentHost.Loaded += (s, e) => TryHookProjectsView();
            _shellViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(AdminShellViewModel.ActiveSection))
                {
                    // Give the DataTrigger a moment to apply the new Content,
                    // then hook into it if it's the Projects view.
                    Dispatcher.BeginInvoke(new System.Action(TryHookProjectsView));
                }
            };
        }

        private void TryHookProjectsView()
        {
            if (ContentHost.Content is AdminProjectsView projectsView &&
                projectsView.DataContext is AdminProjectsViewModel projectsViewModel)
            {
                projectsViewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(AdminProjectsViewModel.SelectedProjectForDetail))
                    {
                        var project = projectsViewModel.SelectedProjectForDetail;
                        if (project != null)
                        {
                            ShowProjectDetail(project, projectsView);
                        }
                    }
                };
            }
        }

        private void ShowProjectDetail(ProjectProposal project, AdminProjectsView returnToView)
        {
            var detailView = new ProjectDetailView(project, currentViewerName: "Dana Reyes", canManageDocuments: true);
            detailView.BackRequested += (s, e) =>
            {
                if (returnToView.DataContext is AdminProjectsViewModel vm)
                {
                    vm.SelectedProjectForDetail = null;
                }
                ContentHost.Content = returnToView;
            };

            ContentHost.Content = detailView;
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionHelper.SignOut(this);
        }
    }
}
