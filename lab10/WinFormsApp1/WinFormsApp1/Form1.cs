using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label6.Text = string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text.Replace(',', '.'),
                   System.Globalization.NumberStyles.Any,
                   System.Globalization.CultureInfo.InvariantCulture,
                   out double lambda) || lambda <= 0)
            {
                MessageBox.Show("Введите корректное значение λ.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!double.TryParse(textBox2.Text.Replace(',', '.'),
                   System.Globalization.NumberStyles.Any,
                   System.Globalization.CultureInfo.InvariantCulture,
                   out double mu) || mu <= 0)
            {
                MessageBox.Show("Введите корректное значение μ.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(textBox3.Text, out int totalRequests) || totalRequests <= 0)
            {
                MessageBox.Show("Введите корректное количество заявок.", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox4.Text, out int N) || N <= 0)
            {
                MessageBox.Show("Введите корректное количество приборов (N).", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox5.Text, out int M) || M < 0)
            {
                MessageBox.Show("Введите корректный размер очереди (M).", "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var rng = new MultRandom(DateTime.Now.Ticks & 0x7FFFFFFF);

            double GenExp(double rate)
            {
                double u = rng.Next();
                u = Math.Max(u, 1e-10);
                return -Math.Log(1 - u) / rate;
            }

            double t = 0.0;
            int x = 0;
            int y = 0;

            int processedEvents = 0;
            int accepted = 0;
            int rejected = 0;
            int maxState = N + M;

            double[] stateDurations = new double[N + M + 1];
            double totalQueueTime = 0.0;
            double totalBusyTime = 0.0;

            double tau = GenExp(lambda);
            double delta = double.PositiveInfinity;

            while (processedEvents < totalRequests)
            {
                double timeStep = 0.0;

                if (tau < delta)
                {
                    timeStep = tau;
                    int currentState = x + y;
                    if (currentState < stateDurations.Length)
                    {
                        stateDurations[currentState] += timeStep;
                    }
                    totalQueueTime += y * timeStep;
                    totalBusyTime += x * timeStep;

                    if (x < N)
                    {
                        x = x + 1;
                        accepted++;
                    }
                    else if (y < M)
                    {
                        y = y + 1;
                        accepted++;
                    }
                    else
                    {
                        rejected++;
                    }

                    t = t + tau;
                    delta = delta - tau;
                    tau = GenExp(lambda);
                    processedEvents++;
                }
                else
                {
                    timeStep = delta;
                    int currentState = x + y;
                    if (currentState < stateDurations.Length)
                    {
                        stateDurations[currentState] += timeStep;
                    }
                    totalQueueTime += y * timeStep;
                    totalBusyTime += x * timeStep;

                    if (y == 0)
                    {
                        x = x - 1;
                    }
                    else
                    {
                        y = y - 1;
                    }

                    t = t + delta;
                    tau = tau - delta;
                    delta = double.PositiveInfinity;
                }

                if (x > 0)
                {
                    delta = GenExp(mu * x);
                }
                else
                {
                    delta = double.PositiveInfinity;
                }
            }

            double rho = lambda / mu;
            double avgBusyServers = t > 0 ? totalBusyTime / t : 0.0;
            double avgQueueLength = t > 0 ? totalQueueTime / t : 0.0;
            double p1Emp = avgBusyServers / N;
            double p_rej_stat = (double)rejected / totalRequests;

            double[] probabilities = new double[maxState + 1];
            for (int i = 0; i <= maxState; i++)
            {
                probabilities[i] = t > 0 ? stateDurations[i] / t : 0.0;
            }

            label6.Text =
                "═══════════════════════════════════════\r\n" +
                "          РЕЗУЛЬТАТЫ МОДЕЛИРОВАНИЯ     \r\n" +
                "═══════════════════════════════════════\r\n" +
                $"  λ = {lambda:F4}   μ = {mu:F4}   N = {totalRequests}\r\n" +
                $"  Приборов (N): {N}   Мест в очереди (M): {M}\r\n" +
                $"  Принято: {accepted}   Отклонено: {rejected}\r\n" +
                $"  Модельное время: {t:F4}\r\n" +
                "───────────────────────────────────────\r\n" +
                $"  1. Коэффициент загрузки: ρ = {rho:F4}\r\n\r\n" +
                $"  2. Среднее число занятых приборов:\r\n" +
                $"     Эмпирика: {avgBusyServers:F4}\r\n" +
                $"     Доля занятости: {p1Emp * 100:F1}%\r\n\r\n" +
                $"  3. Средняя длина очереди:\r\n" +
                $"     Эмпирика: {avgQueueLength:F4}\r\n\r\n" +
                $"  4. Вероятность отказа (стат.):\r\n" +
                $"     P_отк = rejected/N = {p_rej_stat:F4}\r\n\r\n" +
                "═══════════════════════════════════════";

            DrawChart(probabilities, N);
        }

        private void DrawChart(double[] probabilities, int N)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            ChartArea area = new ChartArea();
            area.AxisY.Title = "Вероятность";
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 1.0;
            area.AxisY.Interval = 0.1;
            area.AxisX.Interval = 1;
            chart1.ChartAreas.Add(area);

            Series sEmp = new Series("Эмпирика");
            sEmp.ChartType = SeriesChartType.Column;
            sEmp.Color = Color.LightGreen;
            sEmp.IsValueShownAsLabel = true;
            sEmp.LabelFormat = "{0:F2}";

            for (int i = 0; i < probabilities.Length; i++)
            {
                int pointIndex = sEmp.Points.AddXY(i, probabilities[i]);
                if (i <= N)
                {
                    sEmp.Points[pointIndex].Color = Color.LightGreen;
                }
                else
                {
                    sEmp.Points[pointIndex].Color = Color.Coral;
                }
            }

            chart1.Series.Add(sEmp);

            for (int i = 0; i < probabilities.Length; i++)
            {
                CustomLabel label = new CustomLabel();
                label.FromPosition = i - 0.5;
                label.ToPosition = i + 0.5;
                label.Text = $"P{i}";
                area.AxisX.CustomLabels.Add(label);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class MultRandom
    {
        private long x_mult;
        private const long c = (long)int.MaxValue / (2 * 1000) + 3;
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
}