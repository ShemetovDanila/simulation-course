using System;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WinFormsApp1
{
    public class MultRandom
    {
        private long x_mult;
        private const long c = (long)int.MaxValue / 2 + 3;
        private const long m = (long)int.MaxValue;

        public MultRandom(long seed)
        {
            x_mult = seed;
        }

        public double Next()
        {
            x_mult = (c * x_mult) % m;
            return (double)x_mult / m;
        }
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            labelMean.Text = "";
            labelVariance.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double lambda;
            if (!double.TryParse(textBoxLambda.Text, out lambda) || lambda <= 0) {
                MessageBox.Show("Введите корректное значение λ (положительное число).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            double T;
            if (!double.TryParse(textBoxT.Text, out T) || T <= 0) {
                MessageBox.Show("Введите корректное значение T (положительное число).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int N;
            if (!int.TryParse(textBoxN.Text, out N) || N <= 0) {
                MessageBox.Show("Введите корректное значение N (целое положительное число).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            long seed;
            if (!long.TryParse(textBoxSeed.Text, out seed) || seed <= 0) {
                MessageBox.Show("Введите корректное значение Seed (положительное целое число).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            labelMean.Text = "";
            labelVariance.Text = "";

            MultRandom rnd = new MultRandom(seed);

            //основной алгоритм
            int maxEvents = 1000;
            int[] freq = new int[maxEvents];
            int[] results = new int[N];

            for (int k = 0; k < N; k++)
            {
                double t = 0.0;
                int eventsCount = 0;

                while (true)
                {
                    double u = rnd.Next();
                    if (u >= 1.0) u = 0.9999999;

                    double tau = -Math.Log(1.0 - u) / lambda;
                    t += tau;

                    if (t <= T)
                        eventsCount++;
                    else
                        break;
                }

                results[k] = eventsCount;

                if (eventsCount < maxEvents)
                    freq[eventsCount]++;
            }

            double[] probabilities = new double[maxEvents];
            for (int i = 0; i < maxEvents; i++)
            {
                probabilities[i] = (double)freq[i] / N;
            }

            // среднее и дисперсия
            double mean = results.Average();

            double variance = 0;
            for (int i = 0; i < N; i++)
            {
                variance += Math.Pow(results[i] - mean, 2);
            }
            variance /= N;
    
            labelMean.Text = "Среднее: " + mean.ToString("F4") +
                            "   (теор: " + (lambda * T).ToString("F4") + ")";
            labelVariance.Text = "Дисперсия: " + variance.ToString("F4") +
                                 "   (теор: " + (lambda * T).ToString("F4") + ")";
            // гистограмма
            chart1.Series.Clear();
            chart1.Titles.Clear();

            Series series = new Series("Вероятность");
            series.ChartType = SeriesChartType.Column;
            series.Color = System.Drawing.Color.SteelBlue;
            series.BorderColor = System.Drawing.Color.DarkSlateBlue;
            series.BorderWidth = 1;

            for (int i = 0; i < maxEvents; i++)
            {
                if (freq[i] > 0)
                {
                    DataPoint dp = new DataPoint(i, probabilities[i]);
                    dp.ToolTip = $"k={i}, P={probabilities[i]:F4}";
                    series.Points.Add(dp);
                }
            }

            chart1.Series.Add(series);

            ChartArea area = chart1.ChartAreas[0];
            area.AxisX.Title = "Число запросов k";
            area.AxisY.Title = "Вероятность P(X = k)";
            area.AxisX.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            area.AxisY.TitleFont = new System.Drawing.Font("Arial", 10, System.Drawing.FontStyle.Bold);
            area.AxisX.Interval = 1;
            area.AxisX.LabelStyle.Angle = -45;
            area.AxisY.Minimum = 0;
            area.AxisX.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            area.AxisY.MajorGrid.LineColor = System.Drawing.Color.LightGray;
            area.BackColor = System.Drawing.Color.WhiteSmoke;

            chart1.Titles.Add(new Title(
                $"Пуассоновский поток: λ={lambda}, T={T}, N={N}",
                Docking.Top,
                new System.Drawing.Font("Arial", 11, System.Drawing.FontStyle.Bold),
                System.Drawing.Color.DarkSlateBlue
            ));
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxLambda_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxT_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxSeed_TextChanged(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
