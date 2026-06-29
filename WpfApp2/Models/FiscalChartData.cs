using System.Collections.Generic;

namespace BrickByBrick.Models
{
    /// <summary>
    /// One dataset extracted from a CSV column — its header name and the
    /// numeric values down that column, aligned with FiscalChartData.Labels.
    /// </summary>
    public class ChartDataset
    {
        public string Name { get; set; } = string.Empty;
        public List<double> Values { get; set; } = new List<double>();
    }

    /// <summary>
    /// The result of parsing an uploaded CSV file into chart-ready data:
    /// the first column becomes Labels, every other column becomes a
    /// ChartDataset. This is what gets turned into a QuickChart request
    /// and what would eventually be persisted to SQL Server.
    /// </summary>
    public class FiscalChartData
    {
        public string SourceFileName { get; set; } = string.Empty;
        public string LabelColumnName { get; set; } = string.Empty;
        public List<string> Labels { get; set; } = new List<string>();
        public List<ChartDataset> Datasets { get; set; } = new List<ChartDataset>();
    }
}
