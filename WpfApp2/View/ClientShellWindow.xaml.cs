using System.Windows;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for ClientShellWindow.xaml
    /// Simple top-bar shell hosting the Client's proposal/tracking dashboard.
    /// </summary>
    public partial class ClientShellWindow : Window
    {
        public ClientShellWindow()
        {
            InitializeComponent();
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            SessionHelper.SignOut(this);
        }
    }
}
