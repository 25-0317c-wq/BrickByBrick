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

        /// <param name="project">The project to display.</param>
        /// <param name="currentViewerName">Name of the signed-in person viewing this screen.</param>
        /// <param name="canManageDocuments">
        /// True for Employee, Manager, or Admin — allows uploading/removing
        /// documents. Client should pass false: they can still see and open
        /// documents, just not add or remove them.
        /// </param>
        public ProjectDetailView(ProjectProposal project, string currentViewerName = "Unknown", bool canManageDocuments = false)
        {
            InitializeComponent();
            DataContext = new ProjectDetailViewModel(project, currentViewerName, canManageDocuments);

            AllowDrop = canManageDocuments;
            if (canManageDocuments)
            {
                Drop += ProjectDetailView_Drop;
                DragEnter += ProjectDetailView_DragEnter;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ProjectDetailView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        private void ProjectDetailView_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            if (DataContext is not ProjectDetailViewModel vm) return;

            var filePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var path in filePaths)
            {
                if (string.Equals(System.IO.Path.GetExtension(path), ".csv", StringComparison.OrdinalIgnoreCase))
                {
                    // Fire-and-forget: ProcessCsvFileAsync reports any failure
                    // via vm.ChartErrorMessage rather than throwing, so there's
                    // nothing here that needs to be awaited or observed.
                    _ = vm.ProcessCsvFileAsync(path);
                }
                else
                {
                    vm.AddDocumentFromPath(path);
                }
            }
        }
    }
}
