using System.Windows;
using BrickByBrick.Models;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for EmployeeShellWindow.xaml
    /// Simple top-bar shell hosting the Employee's project dashboard, with
    /// navigation into the shared ProjectDetailView when "View Full Details" is clicked.
    /// </summary>
    public partial class EmployeeShellWindow : Window
    {
        private readonly EmployeeDashboardView _dashboardView;
        private readonly EmployeeDashboardViewModel _dashboardViewModel;

        public EmployeeShellWindow()
        {
            InitializeComponent();

            _dashboardView = new EmployeeDashboardView();
            _dashboardViewModel = (EmployeeDashboardViewModel)_dashboardView.DataContext;
            _dashboardViewModel.PropertyChanged += DashboardViewModel_PropertyChanged;

            ContentHost.Content = _dashboardView;
        }

        private void DashboardViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EmployeeDashboardViewModel.SelectedProjectForDetail))
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
