using System;
using WinFormsApp1;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // Вспомогательные панели для кистей
        private Panel _sizePanel;
        private Panel _brushTypePanel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null) components.Dispose();
            base.Dispose(disposing);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ПОСТРОЕНИЕ UI
        // ══════════════════════════════════════════════════════════════════════
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();

            // ── Таймер ────────────────────────────────────────────────────────
            _timer = new System.Windows.Forms.Timer(components);
            _timer.Interval = 100;
            _timer.Tick += Timer_Tick;

            // ── Форма ─────────────────────────────────────────────────────────
            Text = "🌲 Лесной пожар — Клеточный автомат  [Пробел: старт/пауза | →: шаг | R: генерация]";
            BackColor = Color.FromArgb(28, 28, 30);
            ForeColor = Color.FromArgb(220, 220, 220);
            Font = new Font("Segoe UI", 8.5f);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            AutoScaleMode = AutoScaleMode.None;
            KeyPreview = true;

            // ── Размер формы ─────────────────────────────────────────────────
            const int renderW = GridCols * CellSize; // 800
            const int renderH = GridRows * CellSize; // 600
            const int panelW = 224;
            const int statusH = 26;
            ClientSize = new Size(renderW + panelW, renderH + statusH);

            // ══════════════════════════════════════════════════════════════════
            //  ПРАВАЯ ПАНЕЛЬ УПРАВЛЕНИЯ (AutoScroll)
            // ══════════════════════════════════════════════════════════════════
            _rightPanel = new Panel
            {
                Width = panelW,
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(35, 35, 38),
                AutoScroll = true,
                Padding = new Padding(8, 8, 4, 8),
            };

            // Контейнер содержимого (чтобы AutoScroll работал корректно)
            Panel content = new Panel
            {
                Width = panelW - 4,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.Transparent,
            };

            // Заполняем content снизу вверх (DockStyle.Top добавляет сверху)
            // Поэтому добавляем в обратном порядке относительно визуального
            // ══════════════════════════════════════════════════════════════════

            // ── Заголовок ─────────────────────────────────────────────────────
            var lblTitle = MakeHeader("ЛЕСНОЙ ПОЖАР · КА", 38);

            // ── Управление симуляцией ─────────────────────────────────────────
            var secSim = MakeSectionLabel("СИМУЛЯЦИЯ");
            _btnStartPause = MakeBtn("▶  Старт", Color.FromArgb(38, 130, 60));
            _btnStep = MakeBtn("→  Шаг", Color.FromArgb(45, 80, 140));
            _btnRegen = MakeBtn("↺  Новая карта", Color.FromArgb(75, 55, 120));
            _btnClearAll = MakeBtn("⬜  Очистить всё", Color.FromArgb(75, 55, 45));

            _btnStartPause.Click += BtnStartPause_Click;
            _btnStep.Click += BtnStep_Click;
            _btnRegen.Click += BtnRegen_Click;
            _btnClearAll.Click += BtnClearAll_Click;

            // ── Информация ────────────────────────────────────────────────────
            _lblGeneration = new Label
            {
                Text = "Поколение: 0",
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.FromArgb(150, 200, 150),
                Font = new Font("Segoe UI", 8f),
                Padding = new Padding(2, 2, 0, 0),
            };

            // ── Размер кисти ──────────────────────────────────────────────────
            var secBrushSize = MakeSectionLabel("РАЗМЕР КИСТИ  [1/2/3]");
            _sizeBtns = new Button[3];
            _sizePanel = new Panel
            {
                Height = 34,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
            };

            // ── Тип кисти ─────────────────────────────────────────────────────
            var secBrushType = MakeSectionLabel("ТИП КИСТИ");
            _brushBtns = new Button[8];
            _brushTypePanel = new Panel
            {
                Height = 8 * 29,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
                AutoSize = false,
            };

            // ── Параметры симуляции ───────────────────────────────────────────
            var secParams = MakeSectionLabel("ПАРАМЕТРЫ  (изменяются в реальном времени)");

            // Скорость
            (var lblSpeedName, _lblSpeedVal, _trkSpeed) = MakeSlider(
                "Скорость", "5", 1, 10, 5, TrkSpeed_ValueChanged);

            // Молния f
            (var lblLightName, _lblLightningVal, _trkLightning) = MakeSlider(
                "Молния [f]", "35×10⁻⁶", 0, 100, 35, TrkLightning_ValueChanged);

            // Рост p
            (var lblGrowthName, _lblGrowthVal, _trkGrowth) = MakeSlider(
                "Рост [p]", "0.60%", 1, 200, 60, TrkGrowth_ValueChanged);

            // Пепел бонус
            (var lblAshName, _lblAshBonusVal, _trkAshBonus) = MakeSlider(
                "Удобрение пепла", "+2.4%", 0, 50, 24, TrkAshBonus_ValueChanged);

            // Воспламенение
            (var lblIgnName, _lblIgnitionVal, _trkIgnition) = MakeSlider(
                "Воспламенение", "45%", 10, 90, 45, TrkIgnition_ValueChanged);

            // Дальний огонь
            (var lblLongName, _lblLongRangeVal, _trkLongRange) = MakeSlider(
                "Дальний огонь", "L1:12% / L2:30%", 0, 60, 30, TrkLongRange_ValueChanged);

            // ── Легенда ───────────────────────────────────────────────────────
            var secLegend = MakeSectionLabel("СПРАВКА");
            var lblLegend = new Label
            {
                Text = "Правила:\n"
                     + "1. Огонь → пепел (правило 1)\n"
                     + "2. Дерево + сосед горит → огонь\n"
                     + "3. Дерево → огонь с вероятн. f\n"
                     + "4. Пустое → дерево с вероятн. p\n\n"
                     + "Доп. правила:\n"
                     + "• Вода блокирует огонь\n"
                     + "• Камень непроходим\n"
                     + "• Пепел удобряет почву\n"
                     + "• 3 уровня огня (0/1/2)\n"
                     + "  L2 поджигает через клетку\n\n"
                     + "Горячие клавиши:\n"
                     + "Пробел   Старт / Пауза\n"
                     + "→        Шаг вперёд\n"
                     + "R        Новая карта\n"
                     + "C        Очистить\n"
                     + "1/2/3    Размер кисти\n"
                     + "ПКМ      Ластик (пусто)",
                Dock = DockStyle.Top,
                AutoSize = false,
                Height = 210,
                ForeColor = Color.FromArgb(130, 130, 140),
                Font = new Font("Consolas", 7.5f),
                Padding = new Padding(2, 4, 0, 0),
            };

            // ── Добавляем всё в content СВЕРХУ ВНИЗ (каждый с DockStyle.Top) ─
            // DockStyle.Top: последний добавленный → самый снизу видимый.
            // Порядок добавления здесь = визуальный порядок.

            content.Controls.Add(lblLegend);
            content.Controls.Add(secLegend);
            content.Controls.Add(MakeSep());

            content.Controls.Add(lblLongName);
            content.Controls.Add(_trkLongRange);
            content.Controls.Add(_lblLongRangeVal);
            content.Controls.Add(lblIgnName);
            content.Controls.Add(_trkIgnition);
            content.Controls.Add(_lblIgnitionVal);
            content.Controls.Add(lblAshName);
            content.Controls.Add(_trkAshBonus);
            content.Controls.Add(_lblAshBonusVal);
            content.Controls.Add(lblGrowthName);
            content.Controls.Add(_trkGrowth);
            content.Controls.Add(_lblGrowthVal);
            content.Controls.Add(lblLightName);
            content.Controls.Add(_trkLightning);
            content.Controls.Add(_lblLightningVal);
            content.Controls.Add(lblSpeedName);
            content.Controls.Add(_trkSpeed);
            content.Controls.Add(_lblSpeedVal);
            content.Controls.Add(secParams);
            content.Controls.Add(MakeSep());

            content.Controls.Add(_brushTypePanel);
            content.Controls.Add(secBrushType);
            content.Controls.Add(MakeSep());

            content.Controls.Add(_sizePanel);
            content.Controls.Add(secBrushSize);
            content.Controls.Add(MakeSep());

            content.Controls.Add(_lblGeneration);
            content.Controls.Add(_btnClearAll);
            content.Controls.Add(_btnRegen);
            content.Controls.Add(_btnStep);
            content.Controls.Add(_btnStartPause);
            content.Controls.Add(secSim);

            content.Controls.Add(lblTitle);

            _rightPanel.Controls.Add(content);

            // ══════════════════════════════════════════════════════════════════
            //  СТРОКА СТАТУСА
            // ══════════════════════════════════════════════════════════════════
            Panel statusBar = new Panel
            {
                Height = statusH,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(22, 22, 24),
                Padding = new Padding(8, 0, 0, 0),
            };
            _lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.FromArgb(145, 145, 155),
                Font = new Font("Segoe UI", 7.8f),
                Text = "⏸ Пауза",
            };
            statusBar.Controls.Add(_lblStatus);

            // ══════════════════════════════════════════════════════════════════
            //  ПАНЕЛЬ РЕНДЕРИНГА
            // ══════════════════════════════════════════════════════════════════
            _renderPanel = new SimulationPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Black,
                Cursor = Cursors.Crosshair,
            };
            _renderPanel.Paint += RenderPanel_Paint;
            _renderPanel.MouseDown += RenderPanel_MouseDown;
            _renderPanel.MouseMove += RenderPanel_MouseMove;
            _renderPanel.MouseUp += RenderPanel_MouseUp;

            // ── Добавляем на форму ────────────────────────────────────────────
            Controls.Add(_renderPanel);
            Controls.Add(_rightPanel);
            Controls.Add(statusBar);

            // ── Инициализируем слайдеры (вызов обработчиков для установки params)
            TrkSpeed_ValueChanged(this, EventArgs.Empty);
            TrkLightning_ValueChanged(this, EventArgs.Empty);
            TrkGrowth_ValueChanged(this, EventArgs.Empty);
            TrkAshBonus_ValueChanged(this, EventArgs.Empty);
            TrkIgnition_ValueChanged(this, EventArgs.Empty);
            TrkLongRange_ValueChanged(this, EventArgs.Empty);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ФАБРИЧНЫЕ МЕТОДЫ — UI-элементы
        // ══════════════════════════════════════════════════════════════════════

        private static Label MakeHeader(string text, int height = 36)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = height,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.FromArgb(195, 155, 55),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(30, 30, 32),
            };
        }

        private static Label MakeSectionLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 22,
                TextAlign = ContentAlignment.BottomLeft,
                ForeColor = Color.FromArgb(170, 130, 50),
                Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                Padding = new Padding(2, 0, 0, 0),
            };
        }

        private static Button MakeBtn(string text, Color color)
        {
            var btn = new Button
            {
                Text = text,
                Height = 32,
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 3),
            };
            btn.FlatAppearance.BorderColor = ControlPaint.Dark(color, 0.25f);
            btn.FlatAppearance.BorderSize = 1;
            btn.MouseEnter += (s, e) => ((Button)s).BackColor = ControlPaint.Light(color, 0.2f);
            btn.MouseLeave += (s, e) => ((Button)s).BackColor = color;
            return btn;
        }

        private static Panel MakeSep(int height = 8)
        {
            return new Panel
            {
                Height = height,
                Dock = DockStyle.Top,
                BackColor = Color.Transparent,
            };
        }

        /// <summary>
        /// Создаёт тройку: строка с названием, строка со значением, TrackBar.
        /// Добавляются в content в порядке: label_name → trkbar → label_value
        /// (DockStyle.Top снизу вверх: value внизу, name сверху).
        /// </summary>
        private static (Label name, Label val, TrackBar trk) MakeSlider(
            string title, string initVal,
            int min, int max, int defVal,
            EventHandler handler)
        {
            var lblName = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 18,
                ForeColor = Color.FromArgb(175, 175, 185),
                Font = new Font("Segoe UI", 7.8f),
                Padding = new Padding(2, 0, 0, 0),
            };

            var lblVal = new Label
            {
                Text = initVal,
                Dock = DockStyle.Top,
                Height = 14,
                ForeColor = Color.FromArgb(120, 200, 130),
                Font = new Font("Consolas", 7.2f),
                Padding = new Padding(4, 0, 0, 0),
            };

            var trk = new TrackBar
            {
                Minimum = min,
                Maximum = max,
                Value = defVal,
                TickFrequency = Math.Max(1, (max - min) / 10),
                TickStyle = TickStyle.None,
                Dock = DockStyle.Top,
                Height = 28,
                BackColor = Color.FromArgb(35, 35, 38),
            };
            trk.ValueChanged += handler;

            return (lblName, lblVal, trk);
        }
    }
}