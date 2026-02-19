using System.Windows.Forms.DataVisualization.Charting;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private Series currentSeries;
        double v, vx, vy, k, m, S, x, y, y0, alpha, dt, ymax, xmax, v0;
        int runNumber = 0;
        const double g = 9.81, C = 0.15, rho = 1.29;

        public Form1()
        {
            InitializeComponent();
            chart1.Series.Clear();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            results.AllowUserToAddRows = false;
            results.ReadOnly = true;
            results.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            results.MultiSelect = false;
            results.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (results.Columns.Count == 0)
            {
                results.Columns.Add("colNum", "№");
                results.Columns.Add("colH", "h (м)");
                results.Columns.Add("colV", "v (м/с)");
                results.Columns.Add("colAlph", "alph");
                results.Columns.Add("colM", "m (кг)");
                results.Columns.Add("colS", "S (м^2)");
                results.Columns.Add("colDt", "dt (с)");
                results.Columns.Add("colDist", "Дальность полёта (м)");
                results.Columns.Add("colMaxH", "Максимальная высота (м)");
                results.Columns.Add("colVLast", "Скорость в конце (м/с)");
            }
        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            dt = double.Parse(textBoxSt.Text);
            v = double.Parse(textBoxV.Text);
            y0 = double.Parse(textBoxH.Text);
            alpha = double.Parse(textBoxA.Text);
            m = double.Parse(textBoxM.Text);
            S = double.Parse(textBoxPl.Text);

            k = 0.5 * C * rho * S / m;
            alpha = alpha * Math.PI / 180.0;

            vx = v * Math.Cos(alpha);
            vy = v * Math.Sin(alpha);

            x = 0;
            y = y0;
            ymax = 0;
            xmax = 0;
            v0 = v;

            runNumber++;
            currentSeries = new Series
            {
                ChartType = SeriesChartType.Line,
                Name = $"Series{runNumber}"
            };
            currentSeries.LegendText = $"dt={dt:F5}, h={y0}м, v={v}м/с, alph={textBoxA.Text}, m={m}кг, S={S}м^2";
            chart1.Series.Add(currentSeries);

            currentSeries.Points.AddXY(x, y);

            timer1.Start();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (y <= 0)
            {
                timer1.Stop();

                int rowIndex = results.Rows.Add(
                    runNumber,
                    y0.ToString("F2"),
                    v0.ToString("F2"),
                    textBoxA.Text,
                    m.ToString("F2"),
                    S.ToString("F4"),
                    dt.ToString("F4"),
                    xmax.ToString("F2"),
                    ymax.ToString("F2"),
                    v.ToString("F2")
                );

                results.FirstDisplayedScrollingRowIndex = rowIndex;

                return;
            }

            v = Math.Sqrt(vx * vx + vy * vy);

            x = x + vx * dt;
            y = y + vy * dt;

            vx = vx - k * vx * v * dt;
            vy = vy - (g + k * vy * v) * dt;

            if (y > ymax) ymax = y;
            if (x > xmax) xmax = x;

            currentSeries.Points.AddXY(x, y);
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click_1(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}