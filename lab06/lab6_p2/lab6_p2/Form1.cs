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

        private double[] BuildNormalHistogram(double mean, double sigma,
                                              out double[] xs, out double[] ys)
        {
            // Покрываем диапазон [mean - 4σ, mean + 4σ]
            int bins = 8;
            double lo = mean - 4 * sigma;
            double hi = mean + 4 * sigma;
            double width = (hi - lo) / bins;

            xs = new double[bins + 1];   // границы интервалов x_0 .. x_bins
            ys = new double[bins];       // высоты (плотности) y_1 .. y_bins

            for (int i = 0; i <= bins; i++)
                xs[i] = lo + i * width;

            // y_k = плотность нормального распределения в середине k-го интервала
            // Нормируем так, чтобы сумма y_k * (x_k - x_{k-1}) = 1
            for (int k = 0; k < bins; k++)
            {
                double mid = (xs[k] + xs[k + 1]) / 2.0;
                ys[k] = NormalPDF(mid, mean, sigma);
            }

            return ys;
        }

        // Генерация одного значения методом гистограммы
        private double GenerateByHistogram(MultRandom rng,
                                           double[] xs, double[] ys)
        {
            double alpha = rng.Next();   // равномерное α ∈ (0,1)
            double M = alpha;
            int k = 0;

            // M := M - y_k * (x_k - x_{k-1}), пока M >= 0
            while (k < ys.Length)
            {
                double delta = ys[k] * (xs[k + 1] - xs[k]);
                M -= delta;
                if (M < 0)
                {
                    // ξ = x_k + M / y_k   (M уже отрицательное → добавляем)
                    return xs[k + 1] + M / ys[k];
                }
                k++;
            }
            return xs[ys.Length]; // край диапазона (крайне редко)
        }

        // Плотность нормального распределения
        private double NormalPDF(double x, double mean, double sigma)
        {
            return (1.0 / (sigma * Math.Sqrt(2 * Math.PI))) *
                   Math.Exp(-0.5 * Math.Pow((x - mean) / sigma, 2));
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

            // Строим гистограмму нормального распределения
            BuildNormalHistogram(mean, sigma, out double[] xs, out double[] ys);

            // Генерируем выборку методом гистограммы
            var rng = new MultRandom(DateTime.Now.Ticks % int.MaxValue);
            double[] sample = new double[N];
            for (int i = 0; i < N; i++)
                sample[i] = GenerateByHistogram(rng, xs, ys);

            // Выборочное среднее и дисперсия
            double sampleMean = sample.Average();
            double sampleVar = sample.Select(x => Math.Pow(x - sampleMean, 2)).Sum() / N;

            double meanError = Math.Abs(mean) > 1e-12
                ? Math.Abs((sampleMean - mean) / mean) * 100
                : Math.Abs(sampleMean) * 100;
            double varError = Math.Abs((sampleVar - variance) / variance) * 100;

            // Хи-квадрат по тем же бинам гистограммы
            int bins = xs.Length - 1;
            int[] observed = new int[bins];
            foreach (double val in sample)
            {
                for (int i = 0; i < bins; i++)
                {
                    if (val >= xs[i] && (i == bins - 1 ? val <= xs[i + 1] : val < xs[i + 1]))
                    { observed[i]++; break; }
                }
            }

            double chiSq = 0;
            int dfCount = 0;
            for (int i = 0; i < bins; i++)
            {
                // Ожидаемое число = N * y_k * ширина
                double expected = N * ys[i] * (xs[i + 1] - xs[i]);
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

            DrawChart(sample, mean, sigma, xs, ys);
        }

        private void DrawChart(double[] sample, double mean, double sigma,
                               double[] xs, double[] ys)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            var area = chart1.ChartAreas.Add("main");
            area.AxisX.LabelStyle.Format = "0.#";
            area.AxisX.LabelStyle.Angle = 0;
            area.AxisY.Minimum = 0;
            area.AxisY.LabelStyle.Format = "0.##";
            area.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            area.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            area.AxisX.IsMarginVisible = false;

            int bins = xs.Length - 1;
            double binWidth = xs[1] - xs[0];

            // --- Гистограмма: эмпирическая плотность ---
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
                int count = sample.Count(x =>
                    x >= xs[i] && (i == bins - 1 ? x <= xs[i + 1] : x < xs[i + 1]));
                double density = (double)count / (sample.Length * binWidth);

                histSeries.Points.AddXY(i + 1, density);

                // Подпись вида "(-3; -2]" на оси X
                string loBracket = i == 0 ? "(" : "(";
                string hiBracket = "]";
                histSeries.Points[i].AxisLabel =
                    $"({xs[i]:F0}; {xs[i + 1]:F0}]";
            }

            chart1.Series.Add(histSeries);

            // Настройка оси X: позиции совпадают с серединами столбцов
            area.AxisX.Minimum = 0.5;
            area.AxisX.Maximum = bins + 0.5;
            area.AxisX.Interval = 1;

            // --- Теоретическая кривая нормального распределения ---
            var curveSeries = new Series("Curve")
            {
                ChartType = SeriesChartType.Line,
                ChartArea = "main",
                Color = System.Drawing.Color.Green,
                BorderWidth = 3,
                IsVisibleInLegend = false
            };
            curveSeries.XAxisType = AxisType.Secondary;

            // Вторичная ось X совпадает с реальными значениями
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
            // Аппроксимация для больших df
            return df + 1.645 * Math.Sqrt(2.0 * df);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
