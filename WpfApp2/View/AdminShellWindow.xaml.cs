using System.Windows;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for AdminShellWindow.xaml
    /// The Admin's main shell — sidebar navigation between Dashboard, User Management, etc.
    /// </summary>
    public partial class AdminShellWindow : Window
    {
        public AdminShellWindow()
        {
            InitializeComponent();
            DataContext = new AdminShellViewModel();
        }
    }
}
