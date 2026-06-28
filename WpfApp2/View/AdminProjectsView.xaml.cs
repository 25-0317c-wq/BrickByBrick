using System.Windows.Controls;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for AdminProjectsView.xaml
    /// </summary>
    public partial class AdminProjectsView : UserControl
    {
        public AdminProjectsView()
        {
            InitializeComponent();
            DataContext = new AdminProjectsViewModel();
        }
    }
}
