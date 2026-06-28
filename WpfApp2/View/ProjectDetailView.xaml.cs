using System;
using System.Windows;
using System.Windows.Controls;
using BrickByBrick.Models;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Interaction logic for ProjectDetailView.xaml
    /// Shared project detail screen used identically by Admin, Manager, and
    /// Employee dashboards. Raises BackRequested so whichever shell hosts
    /// this view can swap back to its own project list.
    /// </summary>
    public partial class ProjectDetailView : UserControl
    {
        /// <summary>Raised when the user clicks "Back to projects".</summary>
        public event EventHandler? BackRequested;

        public ProjectDetailView(ProjectProposal project)
        {
            InitializeComponent();
            DataContext = new ProjectDetailViewModel(project);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
