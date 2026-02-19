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
            war.Text = "";

            if (!double.TryParse(textBoxSt.Text, out dt) || dt <= 0
                || !double.TryParse(textBoxV.Text, out v) || v <= 0
                || !double.TryParse(textBoxH.Text, out y0) || y0 <= 0
                || !double.TryParse(textBoxA.Text, out alpha) || alpha < -90 || alpha > 90
                || !double.TryParse(textBoxM.Text, out m) || m <= 0
                || !double.TryParse(textBoxPl.Text, out S) || S <= 0
            )
            {
                war.Text = "Ошибка: введите корректные данные!";
                return;
            }

            k = 0.5 * C * rho * S / m;
            double alphaRad = alpha * Math.PI / 180.0;

            vx = v * Math.Cos(alphaRad);
            vy = v * Math.Sin(alphaRad);

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
            currentSeries.LegendText = $"dt={dt:F5}, h={y0}м, v={v}м/с, alpha={textBoxA.Text}, m={m}кг, S={S}м^2";
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

            v = Math.Sqrt(vx * vx + vy * vy);

            x = x + vx * dt;
            y = y + vy * dt;

            vx = vx - k * vx * v * dt;
            vy = vy - (g + k * vy * v) * dt;

            if (y > ymax) ymax = y;
            if (x > xmax) xmax = x;

            currentSeries.Points.AddXY(x, y);

            if (y <= 0)
            {
                timer1.Stop();

                int rowIndex = results.Rows.Add(
                    runNumber,
                    y0.ToString("F2"),
                    v0.ToString("F2"),
                    textBoxA.Text,
                    m.ToString("F4"),
                    S.ToString("F6"),
                    dt.ToString("F4"),
                    xmax.ToString("F4"),
                    ymax.ToString("F4"),
                    v.ToString("F4")
                );

                return;
            }
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

        private void button2_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            results.Rows.Clear();
            runNumber = 0;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            war.Text = "";

            if (!double.TryParse(textBoxV.Text, out v) || v <= 0
                || !double.TryParse(textBoxH.Text, out y0) || y0 <= 0
                || !double.TryParse(textBoxA.Text, out alpha) || alpha < -90 || alpha > 90
                || !double.TryParse(textBoxM.Text, out m) || m <= 0
                || !double.TryParse(textBoxPl.Text, out S) || S <= 0
            )
            {
                war.Text = "Ошибка: введите корректные данные!";
                return;
            }

            double[] dtValues = { 1, 0.1, 0.01, 0.001, 0.0001 };

            foreach (double step in dtValues)
            {
                dt = step;
                k = 0.5 * C * rho * S / m;
                double alphaRad = alpha * Math.PI / 180.0;

                double vx = v * Math.Cos(alphaRad);
                double vy = v * Math.Sin(alphaRad);

                double x = 0;
                double y = y0;
                double ymax = 0;
                double xmax = 0;
                double v0 = v;
                double currentV = v;

                runNumber++;

                Series series = new Series
                {
                    ChartType = SeriesChartType.Line,
                    Name = $"Series{runNumber}"
                };
                series.LegendText = $"dt={dt:F5}, h={y0}м, v={v}м/с, alpha={textBoxA.Text}, m={m}кг, S={S}м^2";
                chart1.Series.Add(series);

                series.Points.AddXY(x, y);

                while (y > 0)
                {
                    currentV = Math.Sqrt(vx * vx + vy * vy);

                    x = x + vx * dt;
                    y = y + vy * dt;

                    vx = vx - k * vx * currentV * dt;
                    vy = vy - (g + k * vy * currentV) * dt;

                    if (y > ymax) ymax = y;
                    if (x > xmax) xmax = x;

                    series.Points.AddXY(x, y);
                }

                int rowIndex = results.Rows.Add(
                    runNumber,
                    y0.ToString("F2"),
                    v0.ToString("F2"),
                    textBoxA.Text,
                    m.ToString("F4"),
                    S.ToString("F6"),
                    dt.ToString("F4"),
                    xmax.ToString("F4"),
                    ymax.ToString("F4"),
                    currentV.ToString("F4")
                );
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}