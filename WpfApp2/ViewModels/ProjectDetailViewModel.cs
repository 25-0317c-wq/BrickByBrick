using System;
using System.Collections.Generic;
using System.Linq;
using BrickByBrick.Models;

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
    /// and a progress-over-time chart built from the project's real
    /// ProgressUpdates (no dummy chart data).
    /// </summary>
    public class ProjectDetailViewModel : BaseViewModel
    {
        public ProjectProposal Project { get; }

        public ProjectDetailViewModel(ProjectProposal project)
        {
            Project = project;
            Project.ProgressUpdates.CollectionChanged += (s, e) => OnPropertyChanged(nameof(ChartPoints));
            Project.PropertyChanged += (s, e) => OnPropertyChanged(nameof(ChartPoints));
        }

        /// <summary>
        /// Chart points built directly from the project's progress update history.
        /// Starts at 0% on the submission date so the chart always has a visible
        /// starting point, even for a brand-new approved project with one update.
        /// </summary>
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
