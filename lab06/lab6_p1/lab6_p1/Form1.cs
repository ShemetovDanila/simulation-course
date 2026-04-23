using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace lab6_p1
{
    public partial class Form1 : Form
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

        private void ClearLabels()
        {
            label7.Text = "";
            label8.Text = "";
            label9.Text = "";
            label10.Text = "";
            label11.Text = "";
            label12.Text = "";
            label13.Text = "";
            label14.Text = "";
            label15.Text = "";
        }

        public Form1()
        {
            InitializeComponent();
            ClearLabels();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ClearLabels();

            double[] probs = new double[5];
            TextBox[] boxes = { textBox1, textBox2, textBox3, textBox4, textBox5 };
            Label[] probLabels = { label7, label8, label9, label10, label11 };

            for (int i = 0; i < 5; i++)
            {
                if (!double.TryParse(boxes[i].Text.Replace('.', ','), out probs[i]) || probs[i] < 0)
                {
                    MessageBox.Show($"Некорректное значение в Prob {i + 1}");
                    return;
                }
            }

            if (!int.TryParse(textBox6.Text, out int N) || N <= 0)
            {
                MessageBox.Show("Некорректное число экспериментов");
                return;
            }

            // нормировка
            double sum = probs.Sum();
            bool normalized = Math.Abs(sum - 1.0) > 1e-9;

            if (normalized)
            {
                label12.Text = "Вероятности изменены";
                for (int i = 0; i < 5; i++)
                {
                    probs[i] /= sum;
                    probLabels[i].Text = probs[i].ToString("F4");
                }
            }

            // генерация
            var rng = new MultRandom(DateTime.Now.Ticks % int.MaxValue);
            int[] counts = new int[5];

            for (int exp = 0; exp < N; exp++)
            {
                double u = rng.Next();
                double cumulative = 0;
                for (int k = 0; k < 5; k++)
                {
                    cumulative += probs[k];
                    if (u < cumulative)
                    {
                        counts[k]++;
                        break;
                    }
                }
            }

            // эмпирические вероятности; теоретические среднее и дисперсия; выборочное среднее и дисперсия
            double[] empirical = counts.Select(c2 => (double)c2 / N).ToArray();

            double theorMean = 0, theorVar = 0;
            for (int i = 0; i < 5; i++)
            {
                theorMean += (i + 1) * probs[i];
            }
            for (int i = 0; i < 5; i++)
            {
                theorVar += probs[i] * Math.Pow(i + 1 - theorMean, 2);
            }

            double sampleMean = 0, sampleVar = 0;
            for (int i = 0; i < 5; i++)
            {
                sampleMean += (i + 1) * empirical[i];
            }
            for (int i = 0; i < 5; i++)
            {
                sampleVar += empirical[i] * Math.Pow(i + 1 - sampleMean, 2);
            }

            double meanError = theorMean != 0 ? Math.Abs((sampleMean - theorMean) / theorMean) * 100 : 0;
            double varError = theorVar != 0 ? Math.Abs((sampleVar - theorVar) / theorVar) * 100 : 0;

            // Хи-квадрат
            double chiSq = 0;
            for (int i = 0; i < 5; i++)
            {
                double expected = N * probs[i];
                if (expected > 0)
                    chiSq += Math.Pow(counts[i] - expected, 2) / expected;
            }

            // Критическое значение хи-квадрат (степень свободы=4, alpha=0.05)
            double chiCritical = 9.488;
            bool reject = chiSq > chiCritical;

            label13.Text = $"Average: {sampleMean:F3} (error = {meanError:F0}%)";
            label14.Text = $"Variance: {sampleVar:F3} (error = {varError:F0}%)";
            label15.Text = $"Chi-squared: {chiSq:F2} > {chiCritical} is {(reject ? "true" : "false")}";

            label15.ForeColor = reject ? System.Drawing.Color.Red : System.Drawing.Color.Green;

            DrawChart(empirical);
        }

        private void DrawChart(double[] empirical)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            var area = chart1.ChartAreas.Add("main");
            area.AxisX.Minimum = 0.5;
            area.AxisX.Maximum = 5.5;
            area.AxisX.Interval = 1;
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 0.3;
            area.AxisY.Interval = 0.05;
            area.AxisY.LabelStyle.Format = "0.##";

            var series = new Series("Empirical")
            {
                ChartType = SeriesChartType.Column,
                ChartArea = "main",
                IsVisibleInLegend = false
            };
            series["PointWidth"] = "0.6";
            series.Color = System.Drawing.Color.SteelBlue;
            series.BorderWidth = 1;
            series.BorderColor = System.Drawing.Color.Navy;

            for (int i = 0; i < empirical.Length; i++)
            {
                var pt = series.Points.AddXY(i + 1, empirical[i]);
                series.Points[i].Label = empirical[i].ToString("F3");
            }

            chart1.Series.Add(series);
            chart1.Invalidate();
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }
    }
}
