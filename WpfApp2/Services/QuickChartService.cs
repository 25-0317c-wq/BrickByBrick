using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BrickByBrick.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BrickByBrick.Services
{
    /// <summary>
    /// Converts parsed FiscalChartData into a Chart.js configuration and
    /// sends it to QuickChart's POST endpoint (https://quickchart.io/chart)
    /// to render an actual chart image. The rendered PNG is saved locally
    /// (same per-project folder pattern as documents) and its file path is
    /// returned for display.
    ///
    /// Uses Newtonsoft.Json, matching the package already used elsewhere
    /// in this project.
    /// </summary>
    public static class QuickChartService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private const string QuickChartEndpoint = "https://quickchart.io/chart";

        public class ChartRenderException : Exception
        {
            public ChartRenderException(string message, Exception? inner = null) : base(message, inner) { }
        }

        /// <summary>
        /// A small fixed palette so multiple datasets get distinct, readable
        /// colors without relying on Chart.js defaults.
        /// </summary>
        private static readonly string[] DatasetColors =
        {
            "#FD6925", // SDG9 orange — primary dataset
            "#1B2A41", // steel navy
            "#1E8A5F", // role employee green
            "#1B6FC0", // role manager blue
            "#C0392B", // role admin red
        };

        /// <summary>
        /// Builds a Chart.js config from the parsed CSV data, sends it to
        /// QuickChart, and saves the resulting image into the project's
        /// local documents-style folder. Returns the local file path of the
        /// saved PNG, which the UI can bind an Image control to directly.
        /// </summary>
        public static async Task<string> RenderAndSaveAsync(FiscalChartData data, string targetFolder, string chartType = "bar")
        {
            object chartConfig = BuildChartConfig(data, chartType);

            var postBody = new JObject
            {
                ["width"] = 700,
                ["height"] = 380,
                ["backgroundColor"] = "white",
                ["format"] = "png",
                ["chart"] = JToken.FromObject(chartConfig)
            };

            string json = JsonConvert.SerializeObject(postBody);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync(
                    QuickChartEndpoint,
                    new StringContent(json, Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                throw new ChartRenderException(
                    "Couldn't reach QuickChart. Check your internet connection and try again.", ex);
            }

            if (!response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                throw new ChartRenderException(
                    $"QuickChart returned an error ({(int)response.StatusCode}): {body}");
            }

            byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

            Directory.CreateDirectory(targetFolder);
            string fileName = $"financial_chart_{DateTime.Now:yyyyMMdd_HHmmss}.png";
            string fullPath = Path.Combine(targetFolder, fileName);

            await File.WriteAllBytesAsync(fullPath, imageBytes);

            return fullPath;
        }

        /// <summary>
        /// Translates FiscalChartData (labels + one or more datasets) into a
        /// Chart.js v2/v3-compatible configuration object.
        /// </summary>
        private static object BuildChartConfig(FiscalChartData data, string chartType)
        {
            var datasets = new List<object>();

            for (int i = 0; i < data.Datasets.Count; i++)
            {
                var ds = data.Datasets[i];
                string color = DatasetColors[i % DatasetColors.Length];

                datasets.Add(new
                {
                    label = ds.Name,
                    data = ds.Values,
                    backgroundColor = chartType == "line" ? color + "33" : color, // light fill under line charts
                    borderColor = color,
                    borderWidth = 2,
                    fill = chartType == "line"
                });
            }

            return new
            {
                type = chartType,
                data = new
                {
                    labels = data.Labels,
                    datasets = datasets
                },
                options = new
                {
                    title = new
                    {
                        display = true,
                        text = string.IsNullOrWhiteSpace(data.SourceFileName)
                            ? "Financial Overview"
                            : $"Financial Overview — {data.SourceFileName}"
                    },
                    legend = new { display = true }
                }
            };
        }
    }
}
