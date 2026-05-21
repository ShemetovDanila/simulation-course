using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public class MultRandom
        {
            private long x_mult;
            private const long c = (long)int.MaxValue / 2 + 3;
            private const long m = (long)int.MaxValue;

            public MultRandom(long seed) => x_mult = seed;

            public double Next()
            {
                x_mult = (c * x_mult) % m;
                return (double)x_mult / m;
            }
        }

        private double[,] Q = new double[3, 3];
        private double[] timeInState = new double[3];
        private double[] piTheoretical = new double[3];

        private double currentTime = 0;
        private int currentState = 1;
        private int eventCount = 0;
        private bool isRunning = false;

        private bool stopByEvents = true;
        private int statsEvents = 1000;
        private double maxDays = 30;
        private int updateDelay = 200;
        private bool isUpdatingTable = false;

        private MultRandom rng = null!;
        private readonly List<string[]> csvData = new();

        private readonly Color[] stateColors = { Color.Gold, Color.SteelBlue, Color.DarkGray };
        private readonly Color[] logColors = { Color.DarkGoldenrod, Color.SteelBlue, Color.DimGray };
        private readonly string[] stateNames = { "Ясно", "Облачно", "Пасмурно" };

        public Form1()
        {
            InitializeComponent();
            BindEvents();
            SetupDefaultMatrix();
            ComputeTheoretical();

            rng = new MultRandom(DateTime.Now.Ticks);
            InitializeRandomState();
            UpdateUI();
        }

        private void BindEvents()
        {
            btnStart.Click += BtnStart_Click;
            btnStop.Click += BtnStop_Click;
            btnReset.Click += BtnReset_Click;

            gridQ.CellValueChanged += (s, e) => UpdateDiagonals();
            trackSpeed.ValueChanged += (s, e) => lblSpeedValue.Text = $"{trackSpeed.Value} мс";
            panelPieChart.Paint += PanelPieChart_Paint;

            radioByEvents.CheckedChanged += ToggleMode;
            radioByDays.CheckedChanged += ToggleMode;
        }

        private void ToggleMode(object? s, EventArgs e)
        {
            bool byEvents = radioByEvents.Checked;
            lblEvents.Visible = byEvents; numEvents.Visible = byEvents;
            lblDays.Visible = !byEvents; numDays.Visible = !byEvents;
        }

        private void SetupDefaultMatrix()
        {
            isUpdatingTable = true;
            gridQ.Rows[0].Cells[1].Value = "0.30";
            gridQ.Rows[0].Cells[2].Value = "0.20";
            gridQ.Rows[1].Cells[0].Value = "0.10";
            gridQ.Rows[1].Cells[2].Value = "0.30";
            gridQ.Rows[2].Cells[0].Value = "0.20";
            gridQ.Rows[2].Cells[1].Value = "0.10";
            isUpdatingTable = false;
            UpdateDiagonals();
        }

        private void UpdateDiagonals()
        {
            if (isUpdatingTable) return;
            isUpdatingTable = true;
            for (int i = 0; i < 3; i++)
            {
                double sum = 0;
                for (int j = 0; j < 3; j++)
                {
                    if (i == j) continue;
                    if (double.TryParse(
                            gridQ.Rows[i].Cells[j].Value?.ToString()?.Replace(',', '.'),
                            NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
                        sum += v;
                }
                gridQ.Rows[i].Cells[i].Value = (-sum).ToString("F2", CultureInfo.InvariantCulture);
                gridQ.Rows[i].Cells[i].Style.BackColor = Color.MistyRose;
                gridQ.Rows[i].Cells[i].Style.ForeColor = Color.DarkRed;
                gridQ.Rows[i].Cells[i].ReadOnly = true;
            }
            isUpdatingTable = false;
        }

        private void ReadQ()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    Q[i, j] = double.Parse(
                        gridQ.Rows[i].Cells[j].Value?.ToString()?.Replace(',', '.') ?? "0",
                        CultureInfo.InvariantCulture);
        }

        private double[] ComputeTheoretical()
        {
            ReadQ();
            double[,] A =
            {
                { Q[0,0], Q[1,0], Q[2,0] },
                { Q[0,1], Q[1,1], Q[2,1] },
                { 1,      1,      1      }
            };
            double[] B = { 0, 0, 1 };
            double det = Det3(A);

            if (Math.Abs(det) < 1e-9)
            {
                piTheoretical = new[] { 1.0 / 3, 1.0 / 3, 1.0 / 3 };
                return piTheoretical;
            }

            double[] res = new double[3];
            for (int i = 0; i < 3; i++)
            {
                double[,] Ai = (double[,])A.Clone();
                Ai[0, i] = B[0]; Ai[1, i] = B[1]; Ai[2, i] = B[2];
                res[i] = Det3(Ai) / det;
            }
            piTheoretical = res;
            return res;
        }

        private static double Det3(double[,] m) =>
            m[0, 0] * (m[1, 1] * m[2, 2] - m[1, 2] * m[2, 1]) -
            m[0, 1] * (m[1, 0] * m[2, 2] - m[1, 2] * m[2, 0]) +
            m[0, 2] * (m[1, 0] * m[2, 1] - m[1, 1] * m[2, 0]);

        private void InitializeRandomState()
        {
            double r = rng.Next(), cum = 0;
            for (int i = 0; i < 3; i++)
            {
                cum += piTheoretical[i];
                if (r <= cum) { currentState = i + 1; return; }
            }
            currentState = 3;
        }

        private int GetNextState(int stateIdx, out double tau)
        {
            double qExit = -Q[stateIdx, stateIdx];

            double u = rng.Next();
            tau = Math.Log(1.0 - u) / qExit;

            double r = rng.Next() * qExit, cum = 0;
            for (int j = 0; j < 3; j++)
            {
                if (j == stateIdx) continue;
                cum += Q[stateIdx, j];
                if (r <= cum) return j;
            }
            return stateIdx;
        }

        private void StepSimulation()
        {
            if (!isRunning) return;

            int idx = currentState - 1;
            int nextIdx = GetNextState(idx, out double tau);

            if (!stopByEvents && currentTime + tau > maxDays)
                tau = maxDays - currentTime;

            currentTime += tau;
            timeInState[idx] += tau;
            currentState = nextIdx + 1;
            eventCount++;

            LogWeather(currentTime - tau, tau, idx);
            RecordSnapshot();
            UpdateUI();

            if ((stopByEvents && eventCount >= statsEvents) ||
                (!stopByEvents && currentTime >= maxDays))
                isRunning = false;
        }

        private void LogWeather(double tStart, double dur, int stateIdx)
        {
            var s = TimeSpan.FromDays(tStart);
            var e = TimeSpan.FromDays(tStart + dur);
            string msg = $"[{s.Days}д{s.Hours:D2}:{s.Minutes:D2}-{e.Days}д{e.Hours:D2}:{e.Minutes:D2}] {stateNames[stateIdx],-8} {dur:F2}д";
            LogMessage(msg, logColors[stateIdx]);
        }

        private void LogMessage(string text, Color color)
        {
            if (txtLog.InvokeRequired) { txtLog.Invoke(new Action(() => LogMessage(text, color))); return; }
            txtLog.SelectionStart = txtLog.TextLength;
            txtLog.SelectionColor = color;
            txtLog.AppendText(text + Environment.NewLine);
            txtLog.ScrollToCaret();
        }

        private void RecordSnapshot()
        {
            double total = currentTime > 0 ? currentTime : 1;
            double[] emp = timeInState.Select(t => t / total).ToArray();
            csvData.Add(new[]
            {
                eventCount.ToString(),
                currentTime.ToString("F6", CultureInfo.InvariantCulture),
                currentState.ToString(),
                stateNames[currentState - 1],
                emp[0].ToString("F6", CultureInfo.InvariantCulture),
                emp[1].ToString("F6", CultureInfo.InvariantCulture),
                emp[2].ToString("F6", CultureInfo.InvariantCulture),
                piTheoretical[0].ToString("F6", CultureInfo.InvariantCulture),
                piTheoretical[1].ToString("F6", CultureInfo.InvariantCulture),
                piTheoretical[2].ToString("F6", CultureInfo.InvariantCulture)
            });
        }

        private void SaveToCSV()
        {
            using var sfd = new SaveFileDialog
            {
                Filter = "CSV файлы|*.csv",
                FileName = $"markov_weather_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            using var sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8);
            sw.WriteLine("Event_k,Time_days,State_ID,State_Name," +
                         "Emp_Sunny,Emp_Cloudy,Emp_Overcast," +
                         "Theor_Sunny,Theor_Cloudy,Theor_Overcast");
            foreach (var row in csvData)
                sw.WriteLine(string.Join(",", row));

            MessageBox.Show($"Данные сохранены:\n{sfd.FileName}", "Готово",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            if (isRunning) return;

            ReadQ();
            ComputeTheoretical();
            stopByEvents = radioByEvents.Checked;
            statsEvents = (int)numEvents.Value;
            maxDays = (double)numDays.Value;
            updateDelay = trackSpeed.Value;

            SetControlsEnabled(false);

            rng = new MultRandom(DateTime.Now.Ticks);
            InitializeRandomState();
            currentTime = 0; eventCount = 0;
            Array.Clear(timeInState, 0, 3);
            csvData.Clear();
            txtLog.Clear();

            isRunning = true;

            string mode = stopByEvents ? $"событий: {statsEvents}" : $"дней: {maxDays}";
            LogMessage($"══ СТАРТ ══  Режим: {mode}  |  Нач.состояние: {stateNames[currentState - 1]}", Color.Black);

            await Task.Run(() =>
            {
                while (isRunning)
                {
                    Invoke(new Action(StepSimulation));
                    if (updateDelay > 0) Thread.Sleep(updateDelay);
                }
            });

            LogMessage($"══ ЗАВЕРШЕНО ══  Событий: {eventCount}  |  Время: {currentTime:F4} дней", Color.DarkGreen);
            SaveToCSV();
            SetControlsEnabled(true);
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            isRunning = false;
            SetControlsEnabled(true);
            LogMessage("══ ОСТАНОВЛЕНО ПОЛЬЗОВАТЕЛЕМ ══", Color.Red);
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            isRunning = false;
            currentTime = 0; currentState = 1; eventCount = 0;
            Array.Clear(timeInState, 0, 3);
            csvData.Clear();

            rng = new MultRandom(DateTime.Now.Ticks);
            ComputeTheoretical();
            InitializeRandomState();

            txtLog.Clear();
            UpdateUI();
            panelPieChart.Invalidate();
            SetControlsEnabled(true);
            btnStop.Enabled = false;
        }

        private void SetControlsEnabled(bool enabled)
        {
            btnStart.Enabled = enabled;
            btnReset.Enabled = enabled;
            gridQ.Enabled = enabled;
            numEvents.Enabled = enabled;
            numDays.Enabled = enabled;
            trackSpeed.Enabled = enabled;
            radioByEvents.Enabled = enabled;
            radioByDays.Enabled = enabled;
            btnStop.Enabled = !enabled;
        }

        private void UpdateUI()
        {
            if (stopByEvents)
            {
                lblState.Text = $"Событие {eventCount} / {statsEvents}  —  {stateNames[currentState - 1]}";
                pbProgress.Value = (int)Math.Min(100, statsEvents > 0 ? eventCount * 100.0 / statsEvents : 0);
            }
            else
            {
                lblState.Text = $"Время {currentTime:F2} / {maxDays} дн.  —  {stateNames[currentState - 1]}";
                pbProgress.Value = (int)Math.Min(100, maxDays > 0 ? currentTime * 100.0 / maxDays : 0);
            }

            lblProgress.Text = $"Выполнено: {pbProgress.Value}%";
            lblTime.Text = stopByEvents
                ? $"Модельное время: {currentTime:F4} дней"
                : $"Событий: {eventCount}";

            double total = timeInState[0] + timeInState[1] + timeInState[2];
            if (total <= 0) total = 1;

            for (int i = 0; i < 3; i++)
            {
                double emp = timeInState[i] / total;
                double diff = Math.Abs(emp - piTheoretical[i]);

                lblEmp[i].Text = $"{emp:F4}";
                lblTheor[i].Text = $"{piTheoretical[i]:F4}";
                lblDiff[i].Text = $"{diff:F4}";
            }

            panelPieChart.Invalidate();
        }

        private void PanelPieChart_Paint(object sender, PaintEventArgs e)
        {
            double t0 = timeInState[0], t1 = timeInState[1], t2 = timeInState[2];
            double total = t0 + t1 + t2;
            double[] snap = { t0, t1, t2 };

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int pad = 20;
            var rect = new Rectangle(pad, pad, panelPieChart.Width - pad * 2, panelPieChart.Height - pad * 2);

            if (total <= 0)
            {
                using var bGray = new SolidBrush(Color.Gainsboro);
                g.FillEllipse(bGray, rect);
                using var sf0 = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString("Нет данных", new Font("Segoe UI", 10), new SolidBrush(Color.Gray),
                             new RectangleF(rect.X, rect.Y, rect.Width, rect.Height), sf0);
                return;
            }

            float startAngle = -90f;

            for (int i = 0; i < 3; i++)
            {
                double f = snap[i] / total;
                float sweep = (float)(f * 360.0);
                if (sweep < 0.1f) { startAngle += sweep; continue; }

                using (var brush = new SolidBrush(stateColors[i]))
                    g.FillPie(brush, rect, startAngle, sweep);
                using (var pen = new Pen(Color.White, 2))
                    g.DrawPie(pen, rect, startAngle, sweep);

                if (f > 0.04)
                {
                    float mid = startAngle + sweep / 2f;
                    float rad = mid * MathF.PI / 180f;
                    float r = rect.Width / 3.2f;
                    var cp = new PointF(
                        rect.X + rect.Width / 2f + r * MathF.Cos(rad),
                        rect.Y + rect.Height / 2f + r * MathF.Sin(rad));

                    using var sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString($"{stateNames[i][0]}\n{f:P0}",
                                 new Font("Segoe UI", 8, FontStyle.Bold),
                                 Brushes.White, cp, sf);
                }

                startAngle += sweep;
            }

            int lx = pad, ly = panelPieChart.Height - pad - 14;
            for (int i = 0; i < 3; i++)
            {
                using var b = new SolidBrush(stateColors[i]);
                g.FillRectangle(b, lx, ly, 12, 12);
                g.DrawRectangle(Pens.Gray, lx, ly, 12, 12);
                g.DrawString(stateNames[i], new Font("Segoe UI", 7), Brushes.Black, lx + 15, ly);
                lx += 80;
            }
        }
    }
}