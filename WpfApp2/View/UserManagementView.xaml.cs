using System.Windows.Controls;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for UserManagementView.xaml
    /// Formerly "AdminDashboardView" — renamed to reflect its actual purpose:
    /// managing users and their access/role levels. Lives inside the Admin shell.
    /// </summary>
    public partial class UserManagementView : UserControl
    {
        public UserManagementView()
        {
            InitializeComponent();
            DataContext = new AdminDashboardViewModel();
        }
    }
}
