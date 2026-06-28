using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using BrickByBrick.ViewModels;

namespace BrickByBrick.View
{
    /// <summary>
    /// Self-drawing progress-over-time line chart. No external charting
    /// library — just Canvas + Polyline/Path, redrawn whenever the bound
    /// data points or control size changes.
    /// </summary>
    public partial class ProgressChartControl : UserControl
    {
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
                nameof(Points),
                typeof(List<ProgressChartPoint>),
                typeof(ProgressChartControl),
                new PropertyMetadata(null, OnPointsChanged));

        public List<ProgressChartPoint>? Points
        {
            get => (List<ProgressChartPoint>?)GetValue(PointsProperty);
            set => SetValue(PointsProperty, value);
        }

        public ProgressChartControl()
        {
            InitializeComponent();
        }

        private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ProgressChartControl)?.Redraw();
        }

        private void RootGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
        }

        private void Redraw()
        {
            ChartCanvas.Children.Clear();
            LabelCanvas.Children.Clear();

            var points = Points;
            if (points == null || points.Count == 0)
            {
                return;
            }

            double width = ChartCanvas.ActualWidth;
            double height = ChartCanvas.ActualHeight;

            if (width <= 0 || height <= 0)
            {
                return;
            }

            // Gridlines at 0/25/50/75/100%.
            for (int i = 0; i <= 4; i++)
            {
                double y = height - (height * (i * 25 / 100.0));
                var gridLine = new Line
                {
                    X1 = 0,
                    Y1 = y,
                    X2 = width,
                    Y2 = y,
                    Stroke = new SolidColorBrush(Color.FromRgb(0xE2, 0xE8, 0xF0)),
                    StrokeThickness = 1
                };
                ChartCanvas.Children.Add(gridLine);
            }

            int count = points.Count;
            double stepX = count > 1 ? width / (count - 1) : 0;

            var screenPoints = new PointCollection();
            for (int i = 0; i < count; i++)
            {
                double x = count > 1 ? i * stepX : width / 2.0;
                double percent = System.Math.Max(0, System.Math.Min(100, points[i].PercentComplete));
                double y = height - (height * (percent / 100.0));
                screenPoints.Add(new Point(x, y));
            }

            // Filled area under the line, using the SDG9 orange at low opacity.
            if (screenPoints.Count > 0)
            {
                var areaPoints = new PointCollection(screenPoints);
                areaPoints.Add(new Point(screenPoints[screenPoints.Count - 1].X, height));
                areaPoints.Add(new Point(screenPoints[0].X, height));

                var areaPolygon = new Polygon
                {
                    Points = areaPoints,
                    Fill = new SolidColorBrush(Color.FromArgb(40, 0xFD, 0x69, 0x25))
                };
                ChartCanvas.Children.Add(areaPolygon);
            }

            // The line itself.
            var polyline = new Polyline
            {
                Points = screenPoints,
                Stroke = new SolidColorBrush(Color.FromRgb(0xFD, 0x69, 0x25)),
                StrokeThickness = 2.5,
                StrokeLineJoin = PenLineJoin.Round
            };
            ChartCanvas.Children.Add(polyline);

            // Data point markers + X-axis labels.
            for (int i = 0; i < count; i++)
            {
                var marker = new Ellipse
                {
                    Width = 8,
                    Height = 8,
                    Fill = new SolidColorBrush(Color.FromRgb(0xFD, 0x69, 0x25)),
                    Stroke = Brushes.White,
                    StrokeThickness = 1.5
                };
                Canvas.SetLeft(marker, screenPoints[i].X - 4);
                Canvas.SetTop(marker, screenPoints[i].Y - 4);
                ChartCanvas.Children.Add(marker);

                var label = new TextBlock
                {
                    Text = points[i].DateLabel,
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Color.FromRgb(0x94, 0xA3, 0xB8))
                };
                label.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                double labelX = screenPoints[i].X - (label.DesiredSize.Width / 2);
                Canvas.SetLeft(label, labelX);
                Canvas.SetTop(label, 2);
                LabelCanvas.Children.Add(label);
            }
        }
    }
}
