using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using BrickByBrick.Models;

namespace BrickByBrick.Services
{
    /// <summary>
    /// Parses a CSV file into FiscalChartData, following the layout agreed
    /// for chart uploads: the FIRST column is always the label (e.g. Month,
    /// Week), and every other column is a numeric dataset to plot.
    ///
    /// This is intentionally a plain hand-rolled CSV reader rather than a
    /// full library — it's enough for well-formed, comma-separated files
    /// with a header row and no embedded commas/quotes inside values, which
    /// matches the sample files this feature is built against.
    /// </summary>
    public static class CsvChartParser
    {
        /// <summary>
        /// Thrown when the file can't be parsed into valid chart data —
        /// e.g. empty file, header-only file, or a non-numeric value in a
        /// data column. The message is meant to be shown directly to the user.
        /// </summary>
        public class CsvParseException : Exception
        {
            public CsvParseException(string message) : base(message) { }
        }

        public static FiscalChartData Parse(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new CsvParseException($"File not found: {Path.GetFileName(filePath)}");
            }

            string[] lines = File.ReadAllLines(filePath)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .ToArray();

            if (lines.Length < 2)
            {
                throw new CsvParseException(
                    "This CSV needs at least a header row and one data row.");
            }

            string[] headers = SplitCsvLine(lines[0]);

            if (headers.Length < 2)
            {
                throw new CsvParseException(
                    "This CSV needs at least two columns: one for labels and one for values.");
            }

            var result = new FiscalChartData
            {
                SourceFileName = Path.GetFileName(filePath),
                LabelColumnName = headers[0].Trim()
            };

            // One dataset per column after the first, named after its header.
            for (int col = 1; col < headers.Length; col++)
            {
                result.Datasets.Add(new ChartDataset { Name = headers[col].Trim() });
            }

            for (int rowIndex = 1; rowIndex < lines.Length; rowIndex++)
            {
                string[] cells = SplitCsvLine(lines[rowIndex]);

                if (cells.Length != headers.Length)
                {
                    throw new CsvParseException(
                        $"Row {rowIndex + 1} has {cells.Length} value(s), but the header has {headers.Length}. " +
                        "Every row needs the same number of columns as the header.");
                }

                result.Labels.Add(cells[0].Trim());

                for (int col = 1; col < cells.Length; col++)
                {
                    string rawValue = cells[col].Trim();

                    if (!double.TryParse(rawValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
                    {
                        throw new CsvParseException(
                            $"\"{rawValue}\" in row {rowIndex + 1}, column \"{headers[col].Trim()}\" isn't a plain number. " +
                            "Remove any currency symbols, percent signs, or commas from the data cells.");
                    }

                    result.Datasets[col - 1].Values.Add(numericValue);
                }
            }

            return result;
        }

        /// <summary>
        /// Splits a single CSV line on commas. Does not handle quoted fields
        /// containing embedded commas — sufficient for the simple, well-formed
        /// CSV layout this feature expects.
        /// </summary>
        private static string[] SplitCsvLine(string line)
        {
            return line.Split(',');
        }
    }
}
