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
            label4.Text = string.Empty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(textBox1.Text.Replace(',', '.'),
                   System.Globalization.NumberStyles.Any,
                   System.Globalization.CultureInfo.InvariantCulture,
                   out double lambda) || lambda <= 0)
            {
                MessageBox.Show("Введите корректное значение λ.", "Ошибка ввода",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!double.TryParse(textBox2.Text.Replace(',', '.'),
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out double mu) || mu <= 0)
            {
                MessageBox.Show("Введите корректное значение μ.", "Ошибка ввода",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!int.TryParse(textBox3.Text, out int totalRequests) || totalRequests <= 0)
            {
                MessageBox.Show("Введите корректное количество заявок.", "Ошибка ввода",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
            int N = 1; 

            int accepted = 0;
            int rejected = 0;
            int processed = 0;
            double busyTime = 0.0;

            double tau = GenExp(lambda);
            double delta = double.PositiveInfinity;

            while (processed < totalRequests)
            {
                if (tau < delta)
                {
                    if (x > 0) busyTime += tau;
                    t += tau;

                    if (x < N)
                    {
                        x++;
                        accepted++;
                        delta = GenExp(mu * x);
                    }
                    else
                    {
                        rejected++;
                        delta -= tau;
                    }

                    processed++;
                    tau = GenExp(lambda);
                }
                else
                {
                    if (x > 0) busyTime += delta;
                    t += delta;

                    tau -= delta;

                    x--;

                    if (x > 0)
                        delta = GenExp(mu * x);
                    else
                        delta = double.PositiveInfinity;
                }
            }

            double rho = lambda / mu;
            double p0Theory = 1.0 / (1.0 + rho);
            double p1Theory = rho / (1.0 + rho);

            double p1Emp = (t > 0) ? (busyTime / t) : 0.0;
            double p0Emp = 1.0 - p1Emp;

            double p_rej_stat = (double)rejected / totalRequests;

            double diffP0 = Math.Abs(p0Theory - p0Emp);
            double diffP1 = Math.Abs(p1Theory - p1Emp);
            double A_theory = lambda * p0Theory;
            double A_emp = (t > 0) ? (double)accepted / t : 0.0;

            label4.Text =
                "═══════════════════════════════════════\r\n" +
                "  РЕЗУЛЬТАТЫ МОДЕЛИРОВАНИЯ  M/M/1/0\r\n" +
                "═══════════════════════════════════════\r\n" +
                $"  λ = {lambda:F4}   μ = {mu:F4}   N = {totalRequests}\r\n" +
                $"  Принято: {accepted}   Отклонено: {rejected}\r\n" +
                $"  Модельное время: {t:F4}\r\n" +
                "───────────────────────────────────────\r\n" +
                $"  1. Коэффициент загрузки:  ρ = {rho:F4}\r\n\r\n" +
                "  2. Вероятность простоя P₀:\r\n" +
                $"       Теория:   {p0Theory:F4}\r\n" +
                $"       Эмпирика: {p0Emp:F4}\r\n" +
                $"       Разница:  {diffP0:F4}\r\n\r\n" +
                "  3. Вероятность занятости P₁:\r\n" +
                $"       Теория:   {p1Theory:F4}\r\n" +
                $"       Эмпирика: {p1Emp:F4}\r\n" +
                $"       Разница:  {diffP1:F4}\r\n\r\n" +
                "  4. Абсолютная пропускная способность A:\r\n" +
                $"       A (теория)   = {A_theory:F4}\r\n" +
                $"       A (эмпирика) = {A_emp:F4}\r\n\r\n" +
                "  5. Вероятность отказа (стат.):\r\n" +
                $"       P_отк = rejected/N = {p_rej_stat:F4}\r\n" +
                "═══════════════════════════════════════";

            DrawChart(p0Theory, p0Emp, p1Theory, p1Emp);
        }

        private void DrawChart(double p0Theory, double p0Emp, double p1Theory, double p1Emp)
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            ChartArea area = new ChartArea();
            area.AxisY.Title = "Вероятность";
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 1.0;
            area.AxisY.Interval = 0.1;
            area.AxisX.LabelStyle.Enabled = false;
            chart1.ChartAreas.Add(area);

            Series sTheory = new Series("Теория");
            sTheory.ChartType = SeriesChartType.Column;
            sTheory.Color = Color.SteelBlue;
            sTheory.IsValueShownAsLabel = true;
            sTheory.LabelFormat = "{0:F2}";
            sTheory["DrawSideBySide"] = "True";
            sTheory.Points.AddXY(0, p0Theory);
            sTheory.Points.AddXY(1, p1Theory);
            chart1.Series.Add(sTheory);

            Series sEmp = new Series("Эмпирика");
            sEmp.ChartType = SeriesChartType.Column;
            sEmp.Color = Color.LightGreen;
            sEmp.IsValueShownAsLabel = true;
            sEmp.LabelFormat = "{0:F2}";
            sEmp["DrawSideBySide"] = "True";
            sEmp.Points.AddXY(0, p0Emp);
            sEmp.Points.AddXY(1, p1Emp);
            chart1.Series.Add(sEmp);

            CustomLabel labelP0 = new CustomLabel();
            labelP0.FromPosition = -0.5;
            labelP0.ToPosition = 0.5;
            labelP0.Text = "P0";
            area.AxisX.CustomLabels.Add(labelP0);

            CustomLabel labelP1 = new CustomLabel();
            labelP1.FromPosition = 0.5;
            labelP1.ToPosition = 1.5;
            labelP1.Text = "P1";
            area.AxisX.CustomLabels.Add(labelP1);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
    public class MultRandom
    {
        private long x_mult;
        private const long c = (long)int.MaxValue / (2^10) + 3;
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
