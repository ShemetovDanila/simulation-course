using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Globalization;
using System.Drawing;

namespace lab6_p2
{
    public partial class Form1 : Form
    {
        public class MultRandom
        {
            private long x_mult;

            // параметры линейного конгруэнтного генератора
            private const long a = 1103515245;
            private const long c = 12345;
            private const long m = 2147483648;

            public MultRandom(long seed)
            {
                x_mult = seed;
            }

            public double Next()
            {
                x_mult = (a * x_mult + c) % m;
                return (double)x_mult / m;
            }
        }

        public Form1()
        {
            InitializeComponent();
            ClearLabels();
        }

        private void ClearLabels()
        {
            label4.Text = "";
            label5.Text = "";
            label6.Text = "";
        }

        // метод Бокса-Мюллера
        private double GenerateNormal(MultRandom rng, double mean, double sigma)
        {
            double u1 = rng.Next();
            double u2 = rng.Next();

            if (u1 < 1e-12) u1 = 1e-12;

            double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2);

            return mean + sigma * z;
        }

        // плотность нормального распределения
        private double NormalPDF(double x, double mean, double sigma)
        {
            return (1.0 / (sigma * Math.Sqrt(2.0 * Math.PI))) * Math.Exp(-Math.Pow(x - mean, 2.0) / (2.0 * sigma * sigma));
        }

        // функция ошибок
        private double Erf(double x)
        {
            double sign = x < 0 ? -1 : 1;
            x = Math.Abs(x);

            double t = 1.0 / (1.0 + 0.3275911 * x);

            double y = 1.0 - (((((1.061405429 * t - 1.453152027) * t +
                        1.421413741) * t - 0.284496736) * t +
                        0.254829592) * t) * Math.Exp(-x * x);

            return sign * y;
        }

        // функция распределения нормального закона
        private double NormalCDF(double x, double mean, double sigma)
        {
            return 0.5 * (1.0 + Erf((x - mean) / (sigma * Math.Sqrt(2.0))));
        }

        // формула Стёрджеса
        private int SturgessBins(int n)
        {
            return (int)Math.Ceiling(1.0 + 3.322 * Math.Log10(n));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearLabels();

            if (!double.TryParse(textBox1.Text.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out double mean))
            {
                MessageBox.Show("Некорректное значение Mean");
                return;
            }

            if (!double.TryParse(textBox2.Text.Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out double variance) || variance <= 0)
            {
                MessageBox.Show("Некорректное значение Variance (>0)");
                return;
            }

            if (!int.TryParse(textBox3.Text, out int N) || N <= 0)
            {
                MessageBox.Show("Некорректное значение Sample size");
                return;
            }

            double sigma = Math.Sqrt(variance);

            // генерация выборки
            long seed = DateTime.Now.Ticks % int.MaxValue;
            MultRandom rng = new MultRandom(seed);

            double[] sample = new double[N];

            for (int i = 0; i < N; i++)
                sample[i] = GenerateNormal(rng, mean, sigma);

            // выборочные характеристики
            double sampleMean = sample.Average();
            double sampleVar = sample.Sum(x => Math.Pow(x - sampleMean, 2.0)) / N;

            // ошибки
            double meanError = Math.Abs(mean) > 1e-12
                ? Math.Abs(sampleMean - mean) / Math.Abs(mean) * 100.0
                : Math.Abs(sampleMean - mean) * 100.0;

            double varError = Math.Abs(sampleVar - variance) / variance * 100.0;

            // интервалы гистограммы
            int bins = SturgessBins(N);

            double xMin = mean - 4.0 * sigma;
            double xMax = mean + 4.0 * sigma;
            double binWidth = (xMax - xMin) / bins;

            double[] borders = new double[bins + 1];

            for (int i = 0; i <= bins; i++)
                borders[i] = xMin + i * binWidth;

            // наблюдаемые частоты
            int[] observed = new int[bins];

            foreach (double x in sample)
            {
                if (x < xMin || x > xMax) continue;

                int index = (int)((x - xMin) / binWidth);
                if (index >= bins) index = bins - 1;

                observed[index]++;
            }

            // критерий хи-квадрат
            double chiSq = 0.0;

            for (int i = 0; i < bins; i++)
            {
                double p = NormalCDF(borders[i + 1], mean, sigma) - NormalCDF(borders[i], mean, sigma);
                double expected = N * p;

                if (expected >= 5.0)
                    chiSq += Math.Pow(observed[i] - expected, 2.0) / expected;
            }

            int df = bins - 3;
            if (df < 1) df = 1;

            double chiCritical = GetChiCritical(df);
            bool reject = chiSq > chiCritical;

            label4.Text = $"Average: {sampleMean:F3} (error = {meanError:F1}%)";
            label5.Text = $"Variance: {sampleVar:F3} (error = {varError:F1}%)";
            label6.Text = $"Chi-squared: {chiSq:F2} > {chiCritical:F2} is {(reject ? "true" : "false")}";
            label6.ForeColor = reject ? Color.Red : Color.Green;

            DrawChart(mean, sigma, borders, observed, binWidth, N);
        }

        private void DrawChart(double mean, double sigma, double[] borders, int[] observed, double binWidth, int N)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            ChartArea area = new ChartArea("main");
            chart1.ChartAreas.Add(area);

            area.AxisY.Minimum = 0;
            area.AxisY.LabelStyle.Format = "0.##";

            area.AxisX.MajorGrid.Enabled = false;
            area.AxisY.MajorGrid.LineColor = Color.LightGray;

            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisX.LabelStyle.Font = new Font("Segoe UI", 7);

            int bins = observed.Length;

            // гистограмма
            Series hist = new Series("Hist");
            hist.ChartType = SeriesChartType.Column;
            hist.IsVisibleInLegend = false;
            hist.Color = Color.FromArgb(160, 100, 149, 237);
            hist.BorderColor = Color.Red;
            hist.BorderWidth = 1;
            hist["PointWidth"] = "1";
            hist.XValueType = ChartValueType.String;
            hist.IsXValueIndexed = true;

            for (int i = 0; i < bins; i++)
            {
                double density = (double)observed[i] / (N * binWidth);
                string label = $"({borders[i]:F1};{borders[i + 1]:F1}]";

                hist.Points.AddXY(label, density);
            }

            chart1.Series.Add(hist);

            // теоретическая кривая
            Series curve = new Series("Curve");
            curve.ChartType = SeriesChartType.Line;
            curve.BorderWidth = 3;
            curve.Color = Color.Green;
            curve.IsVisibleInLegend = false;
            curve.XAxisType = AxisType.Secondary;

            area.AxisX2.Enabled = AxisEnabled.True;
            area.AxisX2.Minimum = borders[0];
            area.AxisX2.Maximum = borders[bins];
            area.AxisX2.LabelStyle.Enabled = false;
            area.AxisX2.MajorGrid.Enabled = false;
            area.AxisX2.MajorTickMark.Enabled = false;

            int points = 300;
            double step = (borders[bins] - borders[0]) / points;

            for (int i = 0; i <= points; i++)
            {
                double x = borders[0] + i * step;
                curve.Points.AddXY(x, NormalPDF(x, mean, sigma));
            }

            chart1.Series.Add(curve);
            chart1.Invalidate();
        }

        // таблица критических значений хи-квадрат
        private double GetChiCritical(int df)
        {
            double[] table =
            {
                3.841, 5.991, 7.815, 9.488, 11.070,
                12.592, 14.067, 15.507, 16.919, 18.307,
                19.675, 21.026, 22.362, 23.685, 24.996,
                26.296, 27.587, 28.869, 30.144, 31.410
            };

            if (df < 1) df = 1;
            if (df <= table.Length) return table[df - 1];

            return df + 1.645 * Math.Sqrt(2.0 * df);
        }

        private void chart1_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
    }
}