﻿using LiveChartsCore;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.WinForms;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsSample.General.TemplatedLegends
{
    public partial class CustomLegend : UserControl, IChartLegend<SkiaSharpDrawingContext>
    {
        public CustomLegend()
        {
            InitializeComponent();
        }

        public LegendOrientation Orientation { get; set; }

        public void Draw(Chart<SkiaSharpDrawingContext> chart)
        {
            var wfChart = (Chart)chart.View;

            var series = chart.DrawableSeries;
            var legendOrientation = chart.LegendOrientation;

            Visible = true;
            if (legendOrientation == LegendOrientation.Auto) Orientation = LegendOrientation.Vertical;
            Dock = DockStyle.Right;

            DrawAndMesure(series, wfChart);

            BackColor = wfChart.LegendBackColor;
        }

        private void DrawAndMesure(IEnumerable<IDrawableSeries<SkiaSharpDrawingContext>> series, Chart chart)
        {
            SuspendLayout();
            Controls.Clear();

            var h = 0f;
            var w = 0f;

            var parent = new Panel();
            Controls.Add(parent);
            using var g = CreateGraphics();
            foreach (var s in series)
            {
                var size = g.MeasureString(s.Name, chart.LegendFont);

                var p = new Panel();
                p.Location = new Point(0, (int)h);
                parent.Controls.Add(p);

                p.Controls.Add(new MotionCanvas
                {
                    Location = new Point(6, 0),
                    PaintTasks = s.DefaultPaintContext.PaintTasks,
                    Width = (int)s.DefaultPaintContext.Width,
                    Height = (int)s.DefaultPaintContext.Height
                });
                p.Controls.Add(new Label
                {
                    Text = s.Name,
                    ForeColor = Color.Blue,
                    Font = chart.LegendFont,
                    Location = new Point(6 + (int)s.DefaultPaintContext.Width + 6, 0)
                });

                var thisW = size.Width + 36 + (int)s.DefaultPaintContext.Width;
                p.Width = (int)thisW + 6;
                p.Height = (int)size.Height + 6;
                h += size.Height + 6;
                w = thisW > w ? thisW : w;
            }
            h += 6;
            parent.Height = (int)h;

            Width = (int)w;
            parent.Location = new Point(0, (int)(Height / 2 - h / 2));

            ResumeLayout();
        }
    }
}
