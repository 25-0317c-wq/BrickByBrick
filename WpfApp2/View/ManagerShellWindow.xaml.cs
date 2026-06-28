using System.Windows;
using BrickByBrick.Models;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for ManagerShellWindow.xaml
    /// Simple top-bar shell hosting the Manager's approval dashboard, with
    /// navigation into the shared ProjectDetailView when a project is clicked.
    /// </summary>
    public partial class ManagerShellWindow : Window
    {
        private readonly ManagerDashboardView _dashboardView;
        private readonly ManagerDashboardViewModel _dashboardViewModel;

        public ManagerShellWindow()
        {
            InitializeComponent();

            _dashboardView = new ManagerDashboardView();
            _dashboardViewModel = (ManagerDashboardViewModel)_dashboardView.DataContext;
            _dashboardViewModel.PropertyChanged += DashboardViewModel_PropertyChanged;

            ContentHost.Content = _dashboardView;
        }

        private void DashboardViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ManagerDashboardViewModel.SelectedProjectForDetail))
            {
                var project = _dashboardViewModel.SelectedProjectForDetail;
                if (project != null)
                {
                    ShowProjectDetail(project);
                }
            }
        }

        private void ShowProjectDetail(ProjectProposal project)
        {
            var detailView = new ProjectDetailView(project);
            detailView.BackRequested += (s, e) =>
            {
                _dashboardViewModel.SelectedProjectForDetail = null;
                ContentHost.Content = _dashboardView;
            };

            ContentHost.Content = detailView;
        }
    }
}
