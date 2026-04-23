using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace lab6_p2
{
    public partial class Form1 : Form
    {
        public class MultRandom
        {
            private long x_mult;
            private const long c = (long)int.MaxValue / 2 + 3;
            private const long m = (long)int.MaxValue;
            public MultRandom(long seed) { x_mult = seed; }
            public double Next()
            {
                x_mult = (c * x_mult) % m;
                return (double)x_mult / m;
            }
        }

        private void ClearLabels()
        {
            label4.Text = "";
            label5.Text = "";
            label6.Text = "";
        }
        public Form1()
        {
            InitializeComponent();
            ClearLabels();
        }

        // Метод Бокса-Мюллера
        private double GenerateNormal(MultRandom rng, double mean, double sigma)
        {
            double u1 = rng.Next();
            double u2 = rng.Next();
            if (u1 < 1e-12) u1 = 1e-12;
            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);
            return mean + sigma * z;
        }

        // Плотность нормального распределения
        private double NormalPDF(double x, double mean, double sigma)
        {
            return (1.0 / (sigma * Math.Sqrt(2 * Math.PI))) *
                   Math.Exp(-0.5 * Math.Pow((x - mean) / sigma, 2));
        }

        // Формула Стёрджеса: k = ceil(log2(n)) + 1
        private int SturgessBins(int n)
        {
            return (int)Math.Ceiling(Math.Log(n, 2)) + 1;
        }


        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearLabels();

            if (!double.TryParse(textBox1.Text.Replace('.', ','), out double mean))
            { MessageBox.Show("Некорректное значение Mean"); return; }

            if (!double.TryParse(textBox2.Text.Replace('.', ','), out double variance) || variance <= 0)
            { MessageBox.Show("Некорректное значение Variance (> 0)"); return; }

            if (!int.TryParse(textBox3.Text, out int N) || N <= 0)
            { MessageBox.Show("Некорректное значение Sample size"); return; }

            double sigma = Math.Sqrt(variance);

            // Генерация выборки методом Бокса-Мюллера
            var rng = new MultRandom(DateTime.Now.Ticks % int.MaxValue);
            double[] sample = new double[N];
            for (int i = 0; i < N; i++)
                sample[i] = GenerateNormal(rng, mean, sigma);

            // Число бинов по формуле Стёрджеса
            int bins = SturgessBins(N);

            double xMin = sample.Min();
            double xMax = sample.Max();
            double binWidth = (xMax - xMin) / bins;

            // Границы интервалов
            double[] xs = new double[bins + 1];
            for (int i = 0; i <= bins; i++)
                xs[i] = xMin + i * binWidth;

            // Подсчёт наблюдений в каждом бине
            int[] observed = new int[bins];
            foreach (double val in sample)
            {
                int idx = (int)((val - xMin) / binWidth);
                if (idx >= bins) idx = bins - 1;
                observed[idx]++;
            }

            // Выборочное среднее и дисперсия
            double sampleMean = sample.Average();
            double sampleVar = sample.Select(x => Math.Pow(x - sampleMean, 2)).Sum() / N;

            double meanError = Math.Abs(mean) > 1e-12
                ? Math.Abs((sampleMean - mean) / mean) * 100
                : Math.Abs(sampleMean) * 100;
            double varError = Math.Abs((sampleVar - variance) / variance) * 100;

            // Хи-квадрат
            double chiSq = 0;
            int dfCount = 0;
            for (int i = 0; i < bins; i++)
            {
                // Интегрируем PDF по интервалу методом трапеций
                int steps = 200;
                double h = binWidth / steps;
                double integral = 0;
                for (int s = 0; s <= steps; s++)
                {
                    double xv = xs[i] + s * h;
                    double w = (s == 0 || s == steps) ? 0.5 : 1.0;
                    integral += w * NormalPDF(xv, mean, sigma) * h;
                }
                double expected = N * integral;
                if (expected >= 1)
                {
                    chiSq += Math.Pow(observed[i] - expected, 2) / expected;
                    dfCount++;
                }
            }

            int df = Math.Max(1, dfCount - 3);
            double chiCritical = GetChiCritical(df);
            bool reject = chiSq > chiCritical;

            label4.Text = $"Average: {sampleMean:F3} (error = {meanError:F1}%)";
            label5.Text = $"Variance: {sampleVar:F3} (error = {varError:F1}%)";
            label6.Text = $"Chi-squared: {chiSq:F2} > {chiCritical:F3}  is {(reject ? "true" : "false")}";
            label6.ForeColor = reject ? System.Drawing.Color.Red : System.Drawing.Color.Green;

            DrawChart(sample, mean, sigma, xs, observed, binWidth, N);
        }

        private void DrawChart(double[] sample, double mean, double sigma,
                               double[] xs, int[] observed, double binWidth, int N)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            var area = chart1.ChartAreas.Add("main");
            area.AxisY.Minimum = 0;
            area.AxisY.LabelStyle.Format = "0.##";
            area.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            area.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            area.AxisX.IsMarginVisible = false;

            int bins = xs.Length - 1;

            // --- Гистограмма ---
            var histSeries = new Series("Hist")
            {
                ChartType = SeriesChartType.Column,
                ChartArea = "main",
                Color = System.Drawing.Color.FromArgb(160, 100, 149, 237),
                BorderColor = System.Drawing.Color.Red,
                BorderWidth = 1,
                IsVisibleInLegend = false
            };
            histSeries["PointWidth"] = "1.0";

            for (int i = 0; i < bins; i++)
            {
                double density = (double)observed[i] / (N * binWidth);
                histSeries.Points.AddXY(i + 1, density);
                histSeries.Points[i].AxisLabel = $"({xs[i]:F1}; {xs[i + 1]:F1}]";
            }

            chart1.Series.Add(histSeries);

            area.AxisX.Minimum = 0.5;
            area.AxisX.Maximum = bins + 0.5;
            area.AxisX.Interval = 1;

            // --- Теоретическая кривая через вторичную ось X ---
            var curveSeries = new Series("Curve")
            {
                ChartType = SeriesChartType.Line,
                ChartArea = "main",
                Color = System.Drawing.Color.Green,
                BorderWidth = 3,
                IsVisibleInLegend = false,
                XAxisType = AxisType.Secondary
            };

            area.AxisX2.Minimum = xs[0];
            area.AxisX2.Maximum = xs[bins];
            area.AxisX2.LabelStyle.Enabled = false;
            area.AxisX2.MajorTickMark.Enabled = false;
            area.AxisX2.MajorGrid.Enabled = false;

            int curvePoints = 300;
            double step = (xs[bins] - xs[0]) / curvePoints;
            for (int i = 0; i <= curvePoints; i++)
            {
                double x = xs[0] + i * step;
                curveSeries.Points.AddXY(x, NormalPDF(x, mean, sigma));
            }

            chart1.Series.Add(curveSeries);
            chart1.Invalidate();
        }

        // Таблица критических значений хи-квадрат alpha=0.05
        private double GetChiCritical(int df)
        {
            double[] table = {
                3.841, 5.991, 7.815, 9.488, 11.070,
                12.592, 14.067, 15.507, 16.919, 18.307,
                19.675, 21.026, 22.362, 23.685, 24.996,
                26.296, 27.587, 28.869, 30.144, 31.410
            };
            if (df < 1) df = 1;
            if (df <= table.Length) return table[df - 1];
            return df + 1.645 * Math.Sqrt(2.0 * df);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
