using System.Windows;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for RoleDemoWindow.xaml
    /// Panelist demo screen — lets you jump straight into any role's view.
    /// </summary>
    public partial class RoleDemoWindow : Window
    {
        public RoleDemoWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
