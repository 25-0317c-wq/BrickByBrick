using System.Windows.Controls;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for EmployeeDashboardView.xaml
    /// Shows the Field Employee's assigned projects and lets them post progress updates.
    /// </summary>
    public partial class EmployeeDashboardView : UserControl
    {
        public EmployeeDashboardView()
        {
            InitializeComponent();
            DataContext = new EmployeeDashboardViewModel();
        }
    }
}
