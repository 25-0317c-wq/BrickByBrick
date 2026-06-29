using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using BrickByBrick.Models;
using BrickByBrick.Services;

namespace BrickByBrick.ViewModels
{
    /// <summary>
    /// A single point on the progress-over-time chart: a date label and the
    /// percent-complete value reported at that point.
    /// </summary>
    public class ProgressChartPoint
    {
        public string DateLabel { get; set; } = string.Empty;
        public int PercentComplete { get; set; }
    }

    /// <summary>
    /// Drives the shared Project Detail View — used identically by Admin,
    /// Manager, and Employee dashboards. Shows project info, team, location,
    /// a progress-over-time chart built from real ProgressUpdates, a
    /// document list, and a financial chart space fed by dropped CSV files
    /// via QuickChart.
    /// </summary>
    public class ProjectDetailViewModel : BaseViewModel
    {
        public ProjectProposal Project { get; }

        /// <summary>
        /// Name of the person currently viewing this screen — used to record
        /// "uploaded by" on new documents. Not the same as a security check;
        /// see CanManageDocuments for the actual permission gate.
        /// </summary>
        public string CurrentViewerName { get; }

        /// <summary>
        /// True if the current viewer is Employee, Manager, or Admin — the
        /// only roles allowed to upload or remove documents, or upload
        /// financial chart CSVs. Client can still see the results, just not
        /// add/remove them.
        /// </summary>
        public bool CanManageDocuments { get; }

        public ICommand UploadDocumentCommand { get; }
        public ICommand RemoveDocumentCommand { get; }
        public ICommand OpenDocumentCommand { get; }
        public ICommand UploadFinancialChartCommand { get; }

        private string _uploadErrorMessage = string.Empty;
        public string UploadErrorMessage
        {
            get => _uploadErrorMessage;
            set { _uploadErrorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasUploadError)); }
        }

        public bool HasUploadError => !string.IsNullOrEmpty(UploadErrorMessage);

        private string _chartErrorMessage = string.Empty;
        public string ChartErrorMessage
        {
            get => _chartErrorMessage;
            set { _chartErrorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasChartError)); }
        }

        public bool HasChartError => !string.IsNullOrEmpty(ChartErrorMessage);

        private bool _isRenderingChart;
        public bool IsRenderingChart
        {
            get => _isRenderingChart;
            set { _isRenderingChart = value; OnPropertyChanged(); }
        }

        public ProjectDetailViewModel(ProjectProposal project, string currentViewerName = "Unknown", bool canManageDocuments = false)
        {
            Project = project;
            CurrentViewerName = currentViewerName;
            CanManageDocuments = canManageDocuments;

            Project.ProgressUpdates.CollectionChanged += (s, e) => OnPropertyChanged(nameof(ChartPoints));
            Project.PropertyChanged += (s, e) => OnPropertyChanged(nameof(ChartPoints));

            UploadDocumentCommand = new RelayCommand(_ => ExecuteUploadDocument());
            RemoveDocumentCommand = new RelayCommand(ExecuteRemoveDocument);
            OpenDocumentCommand = new RelayCommand(ExecuteOpenDocument);
            UploadFinancialChartCommand = new RelayCommand(_ => ExecuteUploadFinancialChartViaDialog());
        }

        /// <summary>
        /// Local folder where this project's documents are stored, named by
        /// project title (sanitized) so each project keeps its own files
        /// separate. Lives under the user's AppData folder — no database yet.
        /// </summary>
        private string GetProjectDocumentsFolder()
        {
            string safeTitle = string.Join("_", Project.Title.Split(Path.GetInvalidFileNameChars()));
            string baseFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BrickByBrick", "ProjectDocuments", safeTitle);

            Directory.CreateDirectory(baseFolder);
            return baseFolder;
        }

        private void ExecuteUploadDocument()
        {
            UploadErrorMessage = string.Empty;

            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a document to upload",
                Filter = "All files (*.*)|*.*",
                Multiselect = true
            };

            if (dialog.ShowDialog() != true)
            {
                return; // User cancelled.
            }

            foreach (var sourcePath in dialog.FileNames)
            {
                try
                {
                    AddDocumentFromPath(sourcePath);
                }
                catch (Exception ex)
                {
                    UploadErrorMessage = $"Couldn't upload '{Path.GetFileName(sourcePath)}': {ex.Message}";
                }
            }
        }

        /// <summary>
        /// Copies the given source file into this project's local documents
        /// folder and adds a ProjectDocument record for it. Public so a
        /// drag-and-drop handler in the view can reuse the same logic as the
        /// file-picker button.
        /// </summary>
        public void AddDocumentFromPath(string sourcePath)
        {
            if (!File.Exists(sourcePath))
            {
                UploadErrorMessage = $"File not found: {Path.GetFileName(sourcePath)}";
                return;
            }

            string targetFolder = GetProjectDocumentsFolder();
            string fileName = Path.GetFileName(sourcePath);
            string targetPath = Path.Combine(targetFolder, fileName);

            // Avoid silently overwriting an existing file with the same name.
            if (File.Exists(targetPath))
            {
                string nameOnly = Path.GetFileNameWithoutExtension(fileName);
                string ext = Path.GetExtension(fileName);
                targetPath = Path.Combine(targetFolder, $"{nameOnly}_{DateTime.Now:yyyyMMdd_HHmmss}{ext}");
            }

            File.Copy(sourcePath, targetPath);

            var info = new FileInfo(targetPath);

            Project.Documents.Add(new ProjectDocument
            {
                FileName = Path.GetFileName(targetPath),
                StoredFilePath = targetPath,
                UploadedBy = CurrentViewerName,
                UploadedOn = DateTime.Now,
                FileSizeBytes = info.Length
            });
        }

        private void ExecuteRemoveDocument(object? parameter)
        {
            if (parameter is ProjectDocument doc)
            {
                try
                {
                    if (File.Exists(doc.StoredFilePath))
                    {
                        File.Delete(doc.StoredFilePath);
                    }
                }
                catch
                {
                    // If the file is locked/already gone, still remove it from
                    // the list — the record shouldn't get stuck because the
                    // physical file couldn't be deleted.
                }

                Project.Documents.Remove(doc);
            }
        }

        private void ExecuteOpenDocument(object? parameter)
        {
            if (parameter is ProjectDocument doc && File.Exists(doc.StoredFilePath))
            {
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo(doc.StoredFilePath)
                    {
                        UseShellExecute = true
                    };
                    System.Diagnostics.Process.Start(psi);
                }
                catch
                {
                    // No associated application, or some other open failure —
                    // fail quietly rather than crashing the view.
                }
            }
        }

        /// <summary>
        /// Local folder where this project's rendered chart images are saved.
        /// Separate from the documents folder to keep generated chart PNGs
        /// distinct from user-uploaded files.
        /// </summary>
        private string GetFinancialChartsFolder()
        {
            string safeTitle = string.Join("_", Project.Title.Split(Path.GetInvalidFileNameChars()));
            string baseFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BrickByBrick", "ProjectCharts", safeTitle);

            Directory.CreateDirectory(baseFolder);
            return baseFolder;
        }

        private void ExecuteUploadFinancialChartViaDialog()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Title = "Select a CSV file with fiscal data",
                Filter = "CSV files (*.csv)|*.csv",
                Multiselect = false
            };

            if (dialog.ShowDialog() != true)
            {
                return; // User cancelled.
            }

            // Fire-and-forget is intentional here: this is a UI command
            // handler with no caller to await, and all failure paths are
            // caught and surfaced via ChartErrorMessage instead of throwing.
            _ = ProcessCsvFileAsync(dialog.FileName);
        }

        /// <summary>
        /// Parses the given CSV file and sends it to QuickChart, updating
        /// Project.FinancialChartData / FinancialChartImageUrl on success.
        /// Public so a drag-and-drop handler in the view can reuse this
        /// exact path, same pattern as AddDocumentFromPath for documents.
        /// </summary>
        public async System.Threading.Tasks.Task ProcessCsvFileAsync(string csvFilePath)
        {
            ChartErrorMessage = string.Empty;
            IsRenderingChart = true;

            try
            {
                FiscalChartData parsed = CsvChartParser.Parse(csvFilePath);

                string chartsFolder = GetFinancialChartsFolder();
                string savedImagePath = await QuickChartService.RenderAndSaveAsync(parsed, chartsFolder);

                Project.FinancialChartData = parsed;
                Project.FinancialChartImageUrl = savedImagePath;
            }
            catch (CsvChartParser.CsvParseException ex)
            {
                ChartErrorMessage = ex.Message;
            }
            catch (QuickChartService.ChartRenderException ex)
            {
                ChartErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ChartErrorMessage = $"Something went wrong building the chart: {ex.Message}";
            }
            finally
            {
                IsRenderingChart = false;
            }
        }


        public List<ProgressChartPoint> ChartPoints
        {
            get
            {
                var points = new List<ProgressChartPoint>
                {
                    new ProgressChartPoint { DateLabel = Project.SubmittedOn.ToString("MMM d"), PercentComplete = 0 }
                };

                points.AddRange(Project.ProgressUpdates
                    .OrderBy(u => u.PostedOn)
                    .Select(u => new ProgressChartPoint
                    {
                        DateLabel = u.PostedOn.ToString("MMM d"),
                        PercentComplete = u.PercentComplete
                    }));

                return points;
            }
        }

        public string ProjectTypeLabel => Project.ProjectType.ToString();
        public string BuildingCategoryLabel => SplitCamelCase(Project.BuildingCategory.ToString());

        private static string SplitCamelCase(string value)
        {
            // "ShoppingMall" -> "Shopping Mall" for friendlier display.
            return System.Text.RegularExpressions.Regex.Replace(value, "(?<!^)([A-Z])", " $1");
        }
    }
}
