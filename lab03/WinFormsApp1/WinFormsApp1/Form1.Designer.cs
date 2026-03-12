using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            _timer = new System.Windows.Forms.Timer(components);
            _timer.Interval = 100;
            _timer.Tick += Timer_Tick;

            Text = "ForestFire";
            BackColor = Color.FromArgb(28, 28, 30);
            ForeColor = Color.FromArgb(220, 220, 220);
            Font = new Font("Segoe UI", 8.5f);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.None;
            KeyPreview = true;

            const int renderW = GridCols * CellSize;   // 800
            const int renderH = GridRows * CellSize;   // 600
            const int panelW = 232;
            const int statusH = 26;
            const int lx = 8;
            const int iw = 212;   // ширина контролов в правой панели

            ClientSize = new Size(renderW + panelW, renderH + statusH);

            // ══════════════════════════════════════════════════════════════════
            //  СТАТУС-БАР
            // ══════════════════════════════════════════════════════════════════
            var statusBar = new Panel
            {
                Height = statusH,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(20, 20, 22),
            };
            _lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(138, 138, 150),
                Font = new Font("Segoe UI", 7.5f),
                Padding = new Padding(8, 0, 0, 0),
            };
            statusBar.Controls.Add(_lblStatus);

            // ══════════════════════════════════════════════════════════════════
            //  ПРАВАЯ ПАНЕЛЬ — AutoScroll, явные Y
            // ══════════════════════════════════════════════════════════════════
            _rightPanel = new Panel
            {
                Width = panelW,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(34, 34, 38),
                AutoScroll = true,
            };

            // Внутренний контейнер (нужен для корректного AutoScroll)
            var inner = new Panel
            {
                Width = panelW - SystemInformation.VerticalScrollBarWidth - 2,
                BackColor = Color.Transparent,
            };
            _rightPanel.Controls.Add(inner);

            int y = 6;

            // ── Заголовок ─────────────────────────────────────────────────────
            inner.Controls.Add(Lbl("🌲  ForestFire", lx, y, iw, 32,
                Color.FromArgb(210, 172, 60),
                new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ContentAlignment.MiddleCenter));
            y += 38;

            // ══════════════════════════════════════════════════════════════════
            //  СИМУЛЯЦИЯ
            // ══════════════════════════════════════════════════════════════════
            inner.Controls.Add(SecHdr("СИМУЛЯЦИЯ", lx, y, iw)); y += 21;

            _btnStartPause = Btn("▶  Старт", Color.FromArgb(38, 130, 60), lx, y, iw, 34);
            _btnStartPause.Click += BtnStartPause_Click;
            inner.Controls.Add(_btnStartPause); y += 38;

            int hw = (iw - 4) / 2;

            _btnStep = Btn("→  Шаг", Color.FromArgb(40, 72, 135), lx, y, hw, 30);
            _btnStep.Click += BtnStep_Click;
            inner.Controls.Add(_btnStep);

            _btnRegen = Btn("↺  Новая карта", Color.FromArgb(72, 52, 118), lx + hw + 4, y, hw, 30);
            _btnRegen.Click += BtnRegen_Click;
            inner.Controls.Add(_btnRegen);
            y += 34;

            _btnClearAll = Btn("⬜  Очистить всё", Color.FromArgb(78, 50, 38), lx, y, iw, 30);
            _btnClearAll.Click += BtnClearAll_Click;
            inner.Controls.Add(_btnClearAll); y += 36;

            // ── Скорость ──────────────────────────────────────────────────────
            inner.Controls.Add(Cap("Скорость симуляции", lx, y, iw)); y += 16;
            _trkSpeed = Trk(1, 10, 5, lx, y, iw);
            _trkSpeed.ValueChanged += TrkSpeed_ValueChanged;
            inner.Controls.Add(_trkSpeed); y += 26;
            _lblSpeedVal = Val("5", lx, y, iw); inner.Controls.Add(_lblSpeedVal); y += 16;

            _lblGeneration = Lbl("Поколение: 0", lx, y, iw, 18,
                Color.FromArgb(125, 195, 125), new Font("Segoe UI", 8f));
            inner.Controls.Add(_lblGeneration); y += 22;

            // ══════════════════════════════════════════════════════════════════
            //  КИСТЬ
            // ══════════════════════════════════════════════════════════════════
            inner.Controls.Add(Sep(lx, y, iw)); y += 10;
            inner.Controls.Add(SecHdr("КИСТЬ", lx, y, iw)); y += 21;

            inner.Controls.Add(Cap("Размер  ( клавиши 1 / 2 / 3 )", lx, y, iw)); y += 16;
            _sizePanel = new Panel { Location = new Point(lx, y), Size = new Size(iw, 32), BackColor = Color.Transparent };
            inner.Controls.Add(_sizePanel); y += 36;

            inner.Controls.Add(Cap("Тип  ( ЛКМ — нанести,  ПКМ — ластик )", lx, y, iw)); y += 16;
            _brushTypePanel = new Panel
            {
                Location = new Point(lx, y),
                Size = new Size(iw, 9 * 30),   // 9 кнопок
                BackColor = Color.Transparent,
            };
            inner.Controls.Add(_brushTypePanel); y += 9 * 30 + 4;

            // ══════════════════════════════════════════════════════════════════
            //  ПАРАМЕТРЫ
            // ══════════════════════════════════════════════════════════════════
            inner.Controls.Add(Sep(lx, y, iw)); y += 10;
            inner.Controls.Add(SecHdr("ПАРАМЕТРЫ  (работают в реальном времени)", lx, y, iw)); y += 21;

            // Молния [f]
            inner.Controls.Add(Cap("Молния / поджог  [f]", lx, y, iw)); y += 16;
            _trkLightning = Trk(0, 100, 35, lx, y, iw);
            _trkLightning.ValueChanged += TrkLightning_ValueChanged;
            inner.Controls.Add(_trkLightning); y += 26;
            _lblLightningVal = Val("35×10⁻⁶", lx, y, iw); inner.Controls.Add(_lblLightningVal); y += 18;

            // Рост [p]
            inner.Controls.Add(Cap("Рост растительности  [p]", lx, y, iw)); y += 16;
            _trkGrowth = Trk(1, 200, 60, lx, y, iw);
            _trkGrowth.ValueChanged += TrkGrowth_ValueChanged;
            inner.Controls.Add(_trkGrowth); y += 26;
            _lblGrowthVal = Val("0.60%", lx, y, iw); inner.Controls.Add(_lblGrowthVal); y += 18;

            // Удобрение пепла
            inner.Controls.Add(Cap("Удобрение пепла", lx, y, iw)); y += 16;
            _trkAshBonus = Trk(0, 50, 24, lx, y, iw);
            _trkAshBonus.ValueChanged += TrkAshBonus_ValueChanged;
            inner.Controls.Add(_trkAshBonus); y += 26;
            _lblAshBonusVal = Val("+2.4%", lx, y, iw); inner.Controls.Add(_lblAshBonusVal); y += 18;

            // База воспламенения
            inner.Controls.Add(Cap("База воспламенения", lx, y, iw)); y += 16;
            _trkIgnition = Trk(5, 90, 40, lx, y, iw);
            _trkIgnition.ValueChanged += TrkIgnition_ValueChanged;
            inner.Controls.Add(_trkIgnition); y += 26;
            _lblIgnitionVal = Val("40%", lx, y, iw); inner.Controls.Add(_lblIgnitionVal); y += 18;

            // Дальний огонь
            inner.Controls.Add(Cap("Перенос искр через клетку  (L1 / L2)", lx, y, iw)); y += 16;
            _trkLongRange = Trk(0, 60, 28, lx, y, iw);
            _trkLongRange.ValueChanged += TrkLongRange_ValueChanged;
            inner.Controls.Add(_trkLongRange); y += 26;
            _lblLongRangeVal = Val("L1:10%  L2:28%", lx, y, iw); inner.Controls.Add(_lblLongRangeVal); y += 18;

            // ══════════════════════════════════════════════════════════════════
            //  СПРАВКА
            // ══════════════════════════════════════════════════════════════════
            inner.Controls.Add(Sep(lx, y, iw)); y += 10;
            inner.Controls.Add(SecHdr("СПРАВКА", lx, y, iw)); y += 21;

            var help = new Label
            {
                Text =
                    "4 основных правила:\n" +
                    "1. Огонь → пепел / пусто\n" +
                    "2. Растение + сосед горит → огонь\n" +
                    "3. Взрослое дерево → огонь  (вер. f)\n" +
                    "4. Пусто → растительность  (вер. p)\n\n" +
                    "Дополнительные правила:\n" +
                    "• Камень и вода не горят\n" +
                    "  (огонь может перелетать через них)\n" +
                    "• Пепел — только от взрослых деревьев;\n" +
                    "  ускоряет рост рядом (+удобрение)\n" +
                    "• Трава и молодые → огонь L1 (слабый)\n" +
                    "• Взрослые → огонь L2 (мощный)\n\n" +
                    "Клавиши:\n" +
                    "  Пробел  — Старт / Пауза\n" +
                    "  →       — Шаг вперёд\n" +
                    "  R       — Новая карта (→ пауза)\n" +
                    "  C       — Очистить всё\n" +
                    "  1/2/3   — Размер кисти",
                Location = new Point(lx, y),
                Size = new Size(iw, 280),
                ForeColor = Color.FromArgb(112, 112, 126),
                Font = new Font("Segoe UI", 7.5f),
            };
            inner.Controls.Add(help);
            y += 285;

            inner.Height = y + 10;

            // ══════════════════════════════════════════════════════════════════
            //  ПАНЕЛЬ РЕНДЕРИНГА
            // ══════════════════════════════════════════════════════════════════
            _renderPanel = new SimulationPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Cursor = Cursors.Cross,
            };
            _renderPanel.Paint += RenderPanel_Paint;
            _renderPanel.MouseDown += RenderPanel_MouseDown;
            _renderPanel.MouseMove += RenderPanel_MouseMove;
            _renderPanel.MouseUp += RenderPanel_MouseUp;

            Controls.Add(_renderPanel);
            Controls.Add(_rightPanel);
            Controls.Add(statusBar);

            // Инициализируем слайдеры: _params уже создан (field init), NRE исключён
            TrkSpeed_ValueChanged(this, EventArgs.Empty);
            TrkLightning_ValueChanged(this, EventArgs.Empty);
            TrkGrowth_ValueChanged(this, EventArgs.Empty);
            TrkAshBonus_ValueChanged(this, EventArgs.Empty);
            TrkIgnition_ValueChanged(this, EventArgs.Empty);
            TrkLongRange_ValueChanged(this, EventArgs.Empty);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ФАБРИКИ UI — компактные вспомогательные методы
        // ══════════════════════════════════════════════════════════════════════

        private static Label Lbl(string t, int x, int y, int w, int h,
            Color fore, Font font,
            ContentAlignment align = ContentAlignment.MiddleLeft)
            => new Label
            {
                Text = t,
                Location = new Point(x, y),
                Size = new Size(w, h),
                ForeColor = fore,
                Font = font,
                TextAlign = align
            };

        private static Label SecHdr(string t, int x, int y, int w)
            => new Label
            {
                Text = t,
                Location = new Point(x, y),
                Size = new Size(w, 20),
                ForeColor = Color.FromArgb(190, 148, 50),
                Font = new Font("Segoe UI", 8f, FontStyle.Bold)
            };

        private static Label Cap(string t, int x, int y, int w)
            => new Label
            {
                Text = t,
                Location = new Point(x, y),
                Size = new Size(w, 15),
                ForeColor = Color.FromArgb(165, 165, 178),
                Font = new Font("Segoe UI", 7.5f)
            };

        private static Label Val(string t, int x, int y, int w)
            => new Label
            {
                Text = t,
                Location = new Point(x + 2, y),
                Size = new Size(w, 15),
                ForeColor = Color.FromArgb(105, 195, 115),
                Font = new Font("Consolas", 7.5f)
            };

        private static Button Btn(string t, Color c, int x, int y, int w, int h)
        {
            Color bc = c;
            var b = new Button
            {
                Text = t,
                Location = new Point(x, y),
                Size = new Size(w, h),
                FlatStyle = FlatStyle.Flat,
                BackColor = bc,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
            };
            b.FlatAppearance.BorderColor = ControlPaint.Dark(bc, 0.2f);
            b.FlatAppearance.BorderSize = 1;
            b.MouseEnter += (s, e) => ((Button)s).BackColor = ControlPaint.Light(bc, 0.18f);
            b.MouseLeave += (s, e) => ((Button)s).BackColor = bc;
            return b;
        }

        private static TrackBar Trk(int min, int max, int val, int x, int y, int w)
            => new TrackBar
            {
                Minimum = min,
                Maximum = max,
                Value = val,
                TickStyle = TickStyle.None,
                Location = new Point(x, y),
                Size = new Size(w, 26),
                BackColor = Color.FromArgb(34, 34, 38)
            };

        private static Panel Sep(int x, int y, int w)
            => new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, 1),
                BackColor = Color.FromArgb(56, 56, 68)
            };
    }
}