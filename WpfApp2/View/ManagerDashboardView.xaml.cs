using System.Windows.Controls;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for ManagerDashboardView.xaml
    /// The Project Manager's approval queue for client-submitted project proposals.
    /// </summary>
    public partial class ManagerDashboardView : UserControl
    {
        public ManagerDashboardView()
        {
            InitializeComponent();
            DataContext = new ManagerDashboardViewModel();
        }
    }
}
