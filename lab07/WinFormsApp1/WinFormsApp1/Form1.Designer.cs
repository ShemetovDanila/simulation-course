namespace WinFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // ── Левая панель ─────────────────────────────────────────
        private System.Windows.Forms.Panel panelLeft;
        private System.Windows.Forms.DataGridView gridQ;
        private System.Windows.Forms.Button btnStart, btnStop, btnReset;
        private System.Windows.Forms.NumericUpDown numEvents, numDays;
        private System.Windows.Forms.Label lblEvents, lblDays;
        private System.Windows.Forms.TrackBar trackSpeed;
        private System.Windows.Forms.Label lblSpeedValue;
        private System.Windows.Forms.RadioButton radioByEvents, radioByDays;
        private System.Windows.Forms.RichTextBox txtLog;

        // ── Правая панель ────────────────────────────────────────
        private System.Windows.Forms.Panel panelRight;
        private System.Windows.Forms.Panel panelPieChart;
        private System.Windows.Forms.Label lblState, lblTime, lblProgress;
        private System.Windows.Forms.Label lblHeaderEmp, lblHeaderTheor, lblHeaderDiff;
        private System.Windows.Forms.ProgressBar pbProgress;
        private System.Windows.Forms.Label[] lblEmp = new System.Windows.Forms.Label[3];
        private System.Windows.Forms.Label[] lblTheor = new System.Windows.Forms.Label[3];
        private System.Windows.Forms.Label[] lblDiff = new System.Windows.Forms.Label[3];

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            // ── экземпляры ───────────────────────────────────────────
            this.panelLeft = new System.Windows.Forms.Panel();
            this.panelRight = new System.Windows.Forms.Panel();
            this.gridQ = new System.Windows.Forms.DataGridView();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.numEvents = new System.Windows.Forms.NumericUpDown();
            this.numDays = new System.Windows.Forms.NumericUpDown();
            this.trackSpeed = new System.Windows.Forms.TrackBar();
            this.lblSpeedValue = new System.Windows.Forms.Label();
            this.panelPieChart = new System.Windows.Forms.Panel();
            this.lblState = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblProgress = new System.Windows.Forms.Label();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.lblHeaderEmp = new System.Windows.Forms.Label();
            this.lblHeaderTheor = new System.Windows.Forms.Label();
            this.lblHeaderDiff = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.radioByEvents = new System.Windows.Forms.RadioButton();
            this.radioByDays = new System.Windows.Forms.RadioButton();
            this.lblEvents = new System.Windows.Forms.Label();
            this.lblDays = new System.Windows.Forms.Label();

            // ═══════════════════════════════════════════════════════════
            //  ЛЕВАЯ ПАНЕЛЬ (ширина 520)
            // ═══════════════════════════════════════════════════════════
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelLeft.Width = 520;
            this.panelLeft.BackColor = System.Drawing.Color.FromArgb(247, 248, 250);
            this.panelLeft.Padding = new System.Windows.Forms.Padding(0);
            this.panelLeft.AutoScroll = true;

            // ── Заголовок: матрица ────────────────────────────
            var lblSectionMatrix = MakeSectionLabel("МАТРИЦА ИНТЕНСИВНОСТЕЙ Q", 14, 12);

            // ── DataGridView ─────────────────────────────────
            this.gridQ.Location = new System.Drawing.Point(14, 32);
            this.gridQ.Width = 480;
            this.gridQ.Height = 125;
            this.gridQ.AllowUserToAddRows = false;
            this.gridQ.AllowUserToDeleteRows = false;
            this.gridQ.RowHeadersVisible = true;
            this.gridQ.RowHeadersWidth = 150;
            this.gridQ.BackgroundColor = System.Drawing.Color.White;
            this.gridQ.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridQ.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.gridQ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridQ.GridColor = System.Drawing.Color.FromArgb(214, 220, 230);
            this.gridQ.Font = new System.Drawing.Font("Segoe UI", 9);
            this.gridQ.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Single;
            this.gridQ.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(44, 95, 138);
            this.gridQ.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.gridQ.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Bold);
            this.gridQ.ColumnHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.gridQ.ColumnHeadersHeight = 28;
            this.gridQ.EnableHeadersVisualStyles = false;
            this.gridQ.RowHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(232, 237, 245);
            this.gridQ.RowHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(44, 95, 138);
            this.gridQ.RowHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 7.5f, System.Drawing.FontStyle.Bold);
            this.gridQ.RowHeadersDefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.gridQ.RowHeadersDefaultCellStyle.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.gridQ.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.gridQ.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(189, 215, 238);
            this.gridQ.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(30, 60, 100);
            this.gridQ.RowTemplate.Height = 32;

            this.gridQ.Columns.Clear();
            this.gridQ.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "col0", HeaderText = "в Ясно" });
            this.gridQ.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "col1", HeaderText = "в Облачно" });
            this.gridQ.Columns.Add(new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "col2", HeaderText = "в Пасмурно" });

            this.gridQ.Rows.Clear();
            this.gridQ.Rows.Add(3);
            this.gridQ.Rows[0].HeaderCell.Value = "из Ясно";
            this.gridQ.Rows[1].HeaderCell.Value = "из Облачно";
            this.gridQ.Rows[2].HeaderCell.Value = "из Пасмурно";

            for (int i = 0; i < 3; i++)
            {
                this.gridQ.Rows[i].Cells[i].Style.BackColor = System.Drawing.Color.FromArgb(253, 232, 232);
                this.gridQ.Rows[i].Cells[i].Style.ForeColor = System.Drawing.Color.FromArgb(176, 48, 48);
                this.gridQ.Rows[i].Cells[i].Style.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold);
            }

            // ── Режим остановки ───────────────────────────────
            var lblSectionMode = MakeSectionLabel("РЕЖИМ ОСТАНОВКИ", 14, 188);
            this.radioByEvents = new System.Windows.Forms.RadioButton
            {
                Text = "По событиям (N)",
                Location = new System.Drawing.Point(14, 206),
                AutoSize = true,
                Checked = true,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };
            this.radioByDays = new System.Windows.Forms.RadioButton
            {
                Text = "По дням (T)",
                Location = new System.Drawing.Point(220, 206),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };

            // ── Параметры ───────────────────────────────────
            var lblSectionParams = MakeSectionLabel("ПАРАМЕТРЫ", 14, 242);
            this.lblEvents = new System.Windows.Forms.Label
            {
                Text = "N (событий):",
                Location = new System.Drawing.Point(14, 262),
                Size = new System.Drawing.Size(100, 24),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };
            this.numEvents = new System.Windows.Forms.NumericUpDown
            {
                Location = new System.Drawing.Point(120, 262),
                Width = 100,
                Minimum = 100,
                Maximum = 100000,
                Value = 1000,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };

            this.lblDays = new System.Windows.Forms.Label
            {
                Text = "T (дней):",
                Location = new System.Drawing.Point(14, 262),
                Size = new System.Drawing.Size(100, 24),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Segoe UI", 9),
                Visible = false
            };
            this.numDays = new System.Windows.Forms.NumericUpDown
            {
                Location = new System.Drawing.Point(120, 262),
                Width = 100,
                Value = 30,
                Minimum = 1,
                Maximum = 10000,
                Font = new System.Drawing.Font("Segoe UI", 9),
                Visible = false
            };

            var lblSpeed = new System.Windows.Forms.Label
            {
                Text = "Задержка (мс):",
                Location = new System.Drawing.Point(14, 302),
                Size = new System.Drawing.Size(100, 24),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                Font = new System.Drawing.Font("Segoe UI", 9)
            };
            this.trackSpeed = new System.Windows.Forms.TrackBar
            {
                Location = new System.Drawing.Point(120, 298),
                Width = 200,
                Height = 30,
                Minimum = 0,
                Maximum = 500,
                Value = 200,
                TickFrequency = 100,
                TickStyle = System.Windows.Forms.TickStyle.None
            };
            this.lblSpeedValue = new System.Windows.Forms.Label
            {
                Text = "200 мс",
                Location = new System.Drawing.Point(330, 302),
                Size = new System.Drawing.Size(60, 24),
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(44, 95, 138),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            // ── Кнопки ────────────────────────────────────────
            this.btnStart = new System.Windows.Forms.Button
            {
                Text = "▶ СТАРТ",
                Location = new System.Drawing.Point(14, 368),
                Size = new System.Drawing.Size(140, 40),
                BackColor = System.Drawing.Color.FromArgb(76, 175, 114),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            this.btnStart.FlatAppearance.BorderSize = 0;

            this.btnStop = new System.Windows.Forms.Button
            {
                Text = "■ СТОП",
                Location = new System.Drawing.Point(164, 368),
                Size = new System.Drawing.Size(140, 40),
                BackColor = System.Drawing.Color.FromArgb(229, 115, 115),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
                Enabled = false
            };
            this.btnStop.FlatAppearance.BorderSize = 0;

            this.btnReset = new System.Windows.Forms.Button
            {
                Text = "↺ СБРОС",
                Location = new System.Drawing.Point(314, 368),
                Size = new System.Drawing.Size(140, 40),
                BackColor = System.Drawing.Color.FromArgb(220, 225, 235),
                ForeColor = System.Drawing.Color.FromArgb(60, 70, 85),
                FlatStyle = System.Windows.Forms.FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold)
            };
            this.btnReset.FlatAppearance.BorderSize = 0;

            // ── Журнал ────────────────────────────────────────
            var lblSectionLog = MakeSectionLabel("ЖУРНАЛ СОБЫТИЙ", 14, 448);
            this.txtLog = new System.Windows.Forms.RichTextBox
            {
                Location = new System.Drawing.Point(14, 468),
                Width = 480,
                Height = 200,
                ReadOnly = true,
                Font = new System.Drawing.Font("Consolas", 8.25f),
                BackColor = System.Drawing.Color.White,
                ForeColor = System.Drawing.Color.FromArgb(60, 70, 80),
                ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical,
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle,
                WordWrap = false
            };

            // ═══════════════════════════════════════════════════════════
            //  ПРАВАЯ ПАНЕЛЬ
            // ═══════════════════════════════════════════════════════════
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.BackColor = System.Drawing.Color.White;
            this.panelRight.Padding = new System.Windows.Forms.Padding(10);

            var lblRightTitle = new System.Windows.Forms.Label
            {
                Text = "ВИЗУАЛИЗАЦИЯ И СТАТИСТИКА",
                Location = new System.Drawing.Point(10, 10),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(44, 95, 138)
            };

            // ── Круговая диаграмма ───────────────────────────
            this.panelPieChart.Location = new System.Drawing.Point(10, 42);
            this.panelPieChart.Size = new System.Drawing.Size(280, 280);
            this.panelPieChart.BackColor = System.Drawing.Color.FromArgb(249, 250, 252);
            this.panelPieChart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // ── Легенда под диаграммой ───────────────────────
            var legendColors = new System.Drawing.Color[]
            {
                System.Drawing.Color.FromArgb(245, 200, 66),
                System.Drawing.Color.FromArgb(74, 143, 196),
                System.Drawing.Color.FromArgb(136, 136, 136)
            };
            string[] legendNames = { "Ясно", "Облачно", "Пасмурно" };
            for (int i = 0; i < 3; i++)
            {
                int x = 20 + i * 85;
                var dot = new System.Windows.Forms.Panel
                {
                    Location = new System.Drawing.Point(x, 330),
                    Size = new System.Drawing.Size(12, 12),
                    BackColor = legendColors[i]
                };
                var lbl = new System.Windows.Forms.Label
                {
                    Text = legendNames[i],
                    Location = new System.Drawing.Point(x + 16, 328),
                    AutoSize = true,
                    Font = new System.Drawing.Font("Segoe UI", 8),
                    ForeColor = System.Drawing.Color.FromArgb(80, 90, 100)
                };
                this.panelRight.Controls.Add(dot);
                this.panelRight.Controls.Add(lbl);
            }

            // ── Статус-карточка ──────────────────────────────
            var panelStatus = new System.Windows.Forms.Panel
            {
                Location = new System.Drawing.Point(310, 42),
                Size = new System.Drawing.Size(260, 80),
                BackColor = System.Drawing.Color.FromArgb(240, 244, 250),
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            };
            this.lblState = new System.Windows.Forms.Label
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(240, 60),
                Font = new System.Drawing.Font("Segoe UI", 9.5f, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(30, 61, 107),
                BackColor = System.Drawing.Color.Transparent
            };
            panelStatus.Controls.Add(this.lblState);
            this.panelRight.Controls.Add(panelStatus);

            // ── Время и прогресс ─────────────────────────────
            this.lblTime = new System.Windows.Forms.Label
            {
                Location = new System.Drawing.Point(310, 135),
                Size = new System.Drawing.Size(260, 24),
                Font = new System.Drawing.Font("Segoe UI", 9),
                ForeColor = System.Drawing.Color.FromArgb(85, 100, 120)
            };
            this.pbProgress = new System.Windows.Forms.ProgressBar
            {
                Location = new System.Drawing.Point(310, 165),
                Size = new System.Drawing.Size(260, 14),
                Minimum = 0,
                Maximum = 100,
                Style = System.Windows.Forms.ProgressBarStyle.Continuous
            };
            this.lblProgress = new System.Windows.Forms.Label
            {
                Location = new System.Drawing.Point(310, 185),
                Size = new System.Drawing.Size(260, 20),
                Font = new System.Drawing.Font("Segoe UI", 8),
                ForeColor = System.Drawing.Color.FromArgb(115, 130, 150),
                TextAlign = System.Drawing.ContentAlignment.MiddleRight
            };

            // ── Таблица сравнения ────────────────────────────
            var lblSectionStat = MakeSectionLabel("СРАВНЕНИЕ РАСПРЕДЕЛЕНИЙ", 10, 385);

            int tx0 = 10;
            int tx1 = 210;
            int tx2 = 330;
            int tx3 = 450;
            int tyH = 408;
            int rowHeight = 30;

            var lblStateName = MakeStatHeader("Состояние", tx0, tyH, 190, System.Drawing.ContentAlignment.MiddleLeft, new System.Windows.Forms.Padding(4, 0, 0, 0));
            this.lblHeaderEmp = MakeStatHeader("Эмпир.", tx1, tyH, 110, System.Drawing.ContentAlignment.MiddleCenter, new System.Windows.Forms.Padding(0));
            this.lblHeaderTheor = MakeStatHeader("Теорет.", tx2, tyH, 110, System.Drawing.ContentAlignment.MiddleCenter, new System.Windows.Forms.Padding(0));
            this.lblHeaderDiff = MakeStatHeader("Разн.", tx3, tyH, 110, System.Drawing.ContentAlignment.MiddleCenter, new System.Windows.Forms.Padding(0));

            string[] rowNames = { "1 — Ясно", "2 — Облачно", "3 — Пасмурно" };
            for (int i = 0; i < 3; i++)
            {
                int ry = tyH + rowHeight + i * rowHeight;
                System.Drawing.Color rowBg = i % 2 == 0
                    ? System.Drawing.Color.White
                    : System.Drawing.Color.FromArgb(247, 249, 252);

                var lblName = new System.Windows.Forms.Label
                {
                    Text = rowNames[i],
                    Location = new System.Drawing.Point(tx0, ry),
                    Size = new System.Drawing.Size(190, rowHeight - 2),
                    Font = new System.Drawing.Font("Segoe UI", 8.5f),
                    ForeColor = System.Drawing.Color.FromArgb(55, 70, 90),
                    BackColor = rowBg,
                    TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
                    Padding = new System.Windows.Forms.Padding(4, 0, 0, 0)
                };
                this.lblEmp[i] = new System.Windows.Forms.Label
                {
                    Location = new System.Drawing.Point(tx1, ry),
                    Size = new System.Drawing.Size(110, rowHeight - 2),
                    Font = new System.Drawing.Font("Consolas", 8.5f),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    BackColor = rowBg,
                    ForeColor = System.Drawing.Color.FromArgb(40, 55, 75)
                };
                this.lblTheor[i] = new System.Windows.Forms.Label
                {
                    Location = new System.Drawing.Point(tx2, ry),
                    Size = new System.Drawing.Size(110, rowHeight - 2),
                    Font = new System.Drawing.Font("Consolas", 8.5f),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    BackColor = rowBg,
                    ForeColor = System.Drawing.Color.FromArgb(40, 55, 75)
                };
                this.lblDiff[i] = new System.Windows.Forms.Label
                {
                    Location = new System.Drawing.Point(tx3, ry),
                    Size = new System.Drawing.Size(110, rowHeight - 2),
                    Font = new System.Drawing.Font("Consolas", 8.5f),
                    TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                    BackColor = rowBg,
                    ForeColor = System.Drawing.Color.FromArgb(40, 55, 75)
                };

                this.panelRight.Controls.Add(lblName);
                this.panelRight.Controls.Add(lblEmp[i]);
                this.panelRight.Controls.Add(lblTheor[i]);
                this.panelRight.Controls.Add(lblDiff[i]);
            }

            // ── Разделитель ──────────────────────────────────
            var divider = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Left,
                Width = 2,
                BackColor = System.Drawing.Color.FromArgb(210, 216, 228)
            };

            // ── Сборка правой панели ─────────────────────────
            this.panelRight.Controls.Add(lblRightTitle);
            this.panelRight.Controls.Add(this.panelPieChart);
            this.panelRight.Controls.Add(this.lblTime);
            this.panelRight.Controls.Add(this.pbProgress);
            this.panelRight.Controls.Add(this.lblProgress);
            this.panelRight.Controls.Add(lblSectionStat);
            this.panelRight.Controls.Add(lblStateName);
            this.panelRight.Controls.Add(this.lblHeaderEmp);
            this.panelRight.Controls.Add(this.lblHeaderTheor);
            this.panelRight.Controls.Add(this.lblHeaderDiff);

            // ── Сборка левой панели ──────────────────────────
            this.panelLeft.Controls.AddRange(new System.Windows.Forms.Control[]
            {
                lblSectionMatrix, gridQ,
                lblSectionMode, radioByEvents, radioByDays,
                lblSectionParams, lblEvents, numEvents, lblDays, numDays,
                lblSpeed, trackSpeed, lblSpeedValue,
                btnStart, btnStop, btnReset,
                lblSectionLog, txtLog
            });

            // ── Сборка формы ─────────────────────────────────
            this.Controls.Add(this.panelRight);
            this.Controls.Add(divider);
            this.Controls.Add(this.panelLeft);

            this.Text = "Марковская модель погоды (CTMC)";
            this.Size = new System.Drawing.Size(1170, 800);
            this.MinimumSize = new System.Drawing.Size(1170, 800);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("Segoe UI", 9);
        }

        private static System.Windows.Forms.Label MakeSectionLabel(string text, int x, int y)
        {
            return new System.Windows.Forms.Label
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                AutoSize = true,
                Font = new System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.Color.FromArgb(90, 106, 126)
            };
        }

        private static System.Windows.Forms.Label MakeStatHeader(
            string text, int x, int y, int width,
            System.Drawing.ContentAlignment align,
            System.Windows.Forms.Padding padding)
        {
            return new System.Windows.Forms.Label
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new System.Drawing.Size(width, 24),
                Font = new System.Drawing.Font("Segoe UI", 8, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.Color.FromArgb(232, 237, 245),
                ForeColor = System.Drawing.Color.FromArgb(44, 95, 138),
                TextAlign = align,
                Padding = padding
            };
        }
    }
}