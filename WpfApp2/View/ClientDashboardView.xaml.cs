using System.Windows.Controls;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for ClientDashboardView.xaml
    /// Lets an External Client submit project proposals and track their status/progress.
    /// </summary>
    public partial class ClientDashboardView : UserControl
    {
        public ClientDashboardView()
        {
            InitializeComponent();
            DataContext = new ClientDashboardViewModel();
        }
    }
}
