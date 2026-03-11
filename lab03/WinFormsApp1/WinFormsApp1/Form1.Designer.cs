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

        // ══════════════════════════════════════════════════════════════════════
        //  ПОСТРОЕНИЕ UI — явные координаты, без путаницы DockStyle.Top
        // ══════════════════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ── Таймер ────────────────────────────────────────────────────────
            _timer = new System.Windows.Forms.Timer(components);
            _timer.Interval = 100;
            _timer.Tick += Timer_Tick;

            // ── Форма ─────────────────────────────────────────────────────────
            Text = "ForestFire";
            BackColor = Color.FromArgb(28, 28, 30);
            ForeColor = Color.FromArgb(220, 220, 220);
            Font = new Font("Segoe UI", 8.5f);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.None;
            KeyPreview = true;

            const int renderW = GridCols * CellSize;  // 800
            const int renderH = GridRows * CellSize;  // 600
            const int panelW = 230;
            const int statusH = 26;
            const int innerW = 210;  // ширина контролов внутри правой панели
            const int lx = 8;    // левый отступ в правой панели

            ClientSize = new Size(renderW + panelW, renderH + statusH);

            // ══════════════════════════════════════════════════════════════════
            //  СТРОКА СТАТУСА
            // ══════════════════════════════════════════════════════════════════
            Panel statusBar = new Panel
            {
                Height = statusH,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(20, 20, 22),
                Padding = new Padding(8, 0, 0, 0),
            };
            _lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(140, 140, 150),
                Font = new Font("Segoe UI", 7.6f),
                Text = "⏸  Пауза",
            };
            statusBar.Controls.Add(_lblStatus);

            // ══════════════════════════════════════════════════════════════════
            //  ПРАВАЯ ПАНЕЛЬ — ScrollPanel с явными Y-позициями
            // ══════════════════════════════════════════════════════════════════
            _rightPanel = new Panel
            {
                Width = panelW,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(34, 34, 38),
                AutoScroll = true,
            };

            // Внутренний контейнер для AutoScroll
            Panel inner = new Panel
            {
                Width = panelW - SystemInformation.VerticalScrollBarWidth - 2,
                BackColor = Color.Transparent,
            };
            _rightPanel.Controls.Add(inner);

            // Счётчик Y для расположения контролов
            int y = 8;

            // ── Заголовок ─────────────────────────────────────────────────────
            inner.Controls.Add(MkLabel("🌲  ForestFire", lx, y, innerW, 32,
                Color.FromArgb(210, 172, 60), new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ContentAlignment.MiddleCenter));
            y += 38;

            // ══════════════════════════════════════════════════════════════════
            //  СЕКЦИЯ: СИМУЛЯЦИЯ
            // ══════════════════════════════════════════════════════════════════
            inner.Controls.Add(MkSection("СИМУЛЯЦИЯ", lx, y, innerW)); y += 22;

            _btnStartPause = MkBtn("▶  Старт", Color.FromArgb(38, 130, 60), lx, y, innerW, 34);
            _btnStartPause.Click += BtnStartPause_Click;
            inner.Controls.Add(_btnStartPause);
            y += 38;

            int halfW = (innerW / 2) - 3;
            _btnStep = MkBtn("→  Шаг", Color.FromArgb(40, 72, 135),
                             lx, y, halfW, 30);
            _btnStep.Click += BtnStep_Click;
            inner.Controls.Add(_btnStep);

            _btnRegen = MkBtn("↺  Новая карта", Color.FromArgb(72, 52, 118),
                              lx + halfW + 6, y, halfW, 30);
            _btnRegen.Click += BtnRegen_Click;
            inner.Controls.Add(_btnRegen);
            y += 34;

            _btnClearAll = MkBtn("⬜  Очистить всё", Color.FromArgb(80, 52, 40),
                                 lx, y, innerW, 30);
            _btnClearAll.Click += BtnClearAll_Click;
            inner.Controls.Add(_btnClearAll);
            y += 36;

            // ── Скорость ──────────────────────────────────────────────────────
            inner.Controls.Add(MkCaption("Скорость симуляции", lx, y, innerW)); y += 17;
            _trkSpeed = MkTrack(1, 10, 5, lx, y, innerW);
            _trkSpeed.ValueChanged += TrkSpeed_ValueChanged;
            inner.Controls.Add(_trkSpeed); y += 26;
            _lblSpeedVal = MkValue("5", lx, y, innerW);
            inner.Controls.Add(_lblSpeedVal); y += 16;

            // ── Поколение ─────────────────────────────────────────────────────
            _lblGeneration = MkLabel("Поколение: 0", lx, y, innerW, 18,
                Color.FromArgb(130, 200, 130), new Font("Segoe UI", 8f));
            inner.Controls.Add(_lblGeneration); y += 22;

            // ══════════════════════════════════════════════════════════════════
            //  СЕКЦИЯ: КИСТЬ
            // ══════════════════════════════════════════════════════════════════
            inner.Controls.Add(MkSep(lx, y, innerW)); y += 10;
            inner.Controls.Add(MkSection("КИСТЬ", lx, y, innerW)); y += 22;

            // Размер кисти
            inner.Controls.Add(MkCaption("Размер  ( клавиши 1 / 2 / 3 )", lx, y, innerW)); y += 17;
            _sizePanel = new Panel
            {
                Location = new Point(lx, y),
                Size = new Size(innerW, 32),
                BackColor = Color.Transparent,
            };
            inner.Controls.Add(_sizePanel); y += 36;

            // Тип кисти
            inner.Controls.Add(MkCaption("Тип  (ЛКМ — рисовать, ПКМ — ластик)", lx, y, innerW)); y += 17;
            _brushTypePanel = new Panel
            {
                Location = new Point(lx, y),
                Size = new Size(innerW, 8 * 30),
                BackColor = Color.Transparent,
            };
            inner.Controls.Add(_brushTypePanel); y += 8 * 30 + 4;

            // ══════════════════════════════════════════════════════════════════
            //  СЕКЦИЯ: ПАРАМЕТРЫ
            // ══════════════════════════════════════════════════════════════════
            inner.Controls.Add(MkSep(lx, y, innerW)); y += 10;
            inner.Controls.Add(MkSection("ПАРАМЕТРЫ  (работают в реальном времени)", lx, y, innerW)); y += 22;

            // Молния [f]
            inner.Controls.Add(MkCaption("Молния / поджог  [f]", lx, y, innerW)); y += 17;
            _trkLightning = MkTrack(0, 100, 35, lx, y, innerW);
            _trkLightning.ValueChanged += TrkLightning_ValueChanged;
            inner.Controls.Add(_trkLightning); y += 26;
            _lblLightningVal = MkValue("35×10⁻⁶", lx, y, innerW);
            inner.Controls.Add(_lblLightningVal); y += 18;

            // Рост [p]
            inner.Controls.Add(MkCaption("Рост растительности  [p]", lx, y, innerW)); y += 17;
            _trkGrowth = MkTrack(1, 200, 60, lx, y, innerW);
            _trkGrowth.ValueChanged += TrkGrowth_ValueChanged;
            inner.Controls.Add(_trkGrowth); y += 26;
            _lblGrowthVal = MkValue("0.60%", lx, y, innerW);
            inner.Controls.Add(_lblGrowthVal); y += 18;

            // Удобрение пепла
            inner.Controls.Add(MkCaption("Удобрение пепла", lx, y, innerW)); y += 17;
            _trkAshBonus = MkTrack(0, 50, 24, lx, y, innerW);
            _trkAshBonus.ValueChanged += TrkAshBonus_ValueChanged;
            inner.Controls.Add(_trkAshBonus); y += 26;
            _lblAshBonusVal = MkValue("+2.4%", lx, y, innerW);
            inner.Controls.Add(_lblAshBonusVal); y += 18;

            // База воспламенения
            inner.Controls.Add(MkCaption("База воспламенения", lx, y, innerW)); y += 17;
            _trkIgnition = MkTrack(5, 90, 45, lx, y, innerW);
            _trkIgnition.ValueChanged += TrkIgnition_ValueChanged;
            inner.Controls.Add(_trkIgnition); y += 26;
            _lblIgnitionVal = MkValue("45%", lx, y, innerW);
            inner.Controls.Add(_lblIgnitionVal); y += 18;

            // Дальний огонь
            inner.Controls.Add(MkCaption("Дальний огонь  (уровни L1 / L2)", lx, y, innerW)); y += 17;
            _trkLongRange = MkTrack(0, 60, 30, lx, y, innerW);
            _trkLongRange.ValueChanged += TrkLongRange_ValueChanged;
            inner.Controls.Add(_trkLongRange); y += 26;
            _lblLongRangeVal = MkValue("L1:12% / L2:30%", lx, y, innerW);
            inner.Controls.Add(_lblLongRangeVal); y += 18;

            // ══════════════════════════════════════════════════════════════════
            //  СЕКЦИЯ: СПРАВКА
            // ══════════════════════════════════════════════════════════════════
            inner.Controls.Add(MkSep(lx, y, innerW)); y += 10;
            inner.Controls.Add(MkSection("СПРАВКА", lx, y, innerW)); y += 22;

            string help =
                "4 основных правила:\n" +
                "1. Огонь → пепел после догорания\n" +
                "2. Дерево + сосед горит → огонь\n" +
                "3. Дерево → огонь с вероятн. f\n" +
                "4. Пусто → растение с вероятн. p\n\n" +
                "Доп. правила:\n" +
                "• Вода блокирует распростр. огня\n" +
                "• Камень — непреодолимый барьер\n" +
                "• Пепел удобряет почву (+рост)\n" +
                "• Уровни огня 0/1/2:\n" +
                "  0 = трава (только соседи)\n" +
                "  1 = молодые (+ малый дальний)\n" +
                "  2 = взрослые (+ норм. дальний)\n\n" +
                "Клавиши:\n" +
                "  Пробел    Старт / Пауза\n" +
                "  →         Шаг вперёд\n" +
                "  R         Новая карта\n" +
                "  C         Очистить\n" +
                "  1 / 2 / 3 Размер кисти";

            Label lblHelp = new Label
            {
                Text = help,
                Location = new Point(lx, y),
                Size = new Size(innerW, 260),
                ForeColor = Color.FromArgb(115, 115, 128),
                Font = new Font("Segoe UI", 7.5f),
            };
            inner.Controls.Add(lblHelp);
            y += 265;

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

            // ── Добавляем на форму ────────────────────────────────────────────
            // Порядок важен: сначала Fill, потом Right, потом Bottom
            Controls.Add(_renderPanel);
            Controls.Add(_rightPanel);
            Controls.Add(statusBar);

            // ── Принудительно вызываем обработчики слайдеров ─────────────────
            // Это устанавливает правильные значения в _params и метках.
            // _params уже создан (field init в Form1.cs), поэтому NRE не будет.
            TrkSpeed_ValueChanged(this, EventArgs.Empty);
            TrkLightning_ValueChanged(this, EventArgs.Empty);
            TrkGrowth_ValueChanged(this, EventArgs.Empty);
            TrkAshBonus_ValueChanged(this, EventArgs.Empty);
            TrkIgnition_ValueChanged(this, EventArgs.Empty);
            TrkLongRange_ValueChanged(this, EventArgs.Empty);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ФАБРИКИ UI-ЭЛЕМЕНТОВ
        // ══════════════════════════════════════════════════════════════════════

        private static Label MkLabel(string text, int x, int y, int w, int h,
            Color fore, Font font,
            ContentAlignment align = ContentAlignment.MiddleLeft)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                ForeColor = fore,
                Font = font,
                TextAlign = align,
            };
        }

        private static Label MkSection(string text, int x, int y, int w)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 20),
                ForeColor = Color.FromArgb(188, 148, 52),
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
            };
        }

        private static Label MkCaption(string text, int x, int y, int w)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 16),
                ForeColor = Color.FromArgb(170, 170, 182),
                Font = new Font("Segoe UI", 7.5f),
            };
        }

        private static Label MkValue(string text, int x, int y, int w)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x + 2, y),
                Size = new Size(w, 16),
                ForeColor = Color.FromArgb(105, 195, 115),
                Font = new Font("Consolas", 7.5f),
            };
        }

        private static Button MkBtn(string text, Color color, int x, int y, int w, int h)
        {
            Color baseColor = color;
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                FlatStyle = FlatStyle.Flat,
                BackColor = baseColor,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
            };
            btn.FlatAppearance.BorderColor = ControlPaint.Dark(baseColor, 0.2f);
            btn.FlatAppearance.BorderSize = 1;
            btn.MouseEnter += (s, e) => ((Button)s).BackColor = ControlPaint.Light(baseColor, 0.18f);
            btn.MouseLeave += (s, e) => ((Button)s).BackColor = baseColor;
            return btn;
        }

        private static TrackBar MkTrack(int min, int max, int val, int x, int y, int w)
        {
            return new TrackBar
            {
                Minimum = min,
                Maximum = max,
                Value = val,
                TickStyle = TickStyle.None,
                Location = new Point(x, y),
                Size = new Size(w, 26),
                BackColor = Color.FromArgb(34, 34, 38),
            };
        }

        private static Panel MkSep(int x, int y, int w)
        {
            return new Panel
            {
                Location = new Point(x, y),
                Size = new Size(w, 1),
                BackColor = Color.FromArgb(58, 58, 68),
            };
        }
    }
}