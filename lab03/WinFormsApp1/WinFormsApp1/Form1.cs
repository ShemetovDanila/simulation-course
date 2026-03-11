using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        // ── Константы ────────────────────────────────────────────────────────
        private const int CellSize = 8;
        private const int GridCols = 100;
        private const int GridRows = 75;
        private const int MinInterval = 16;
        private const int MaxInterval = 600;

        // ── Симуляция ─────────────────────────────────────────────────────────
        private ForestAutomaton _automaton;
        // Инициализируем как поле — InitializeComponent() вызывает обработчики
        // слайдеров, которые пишут в _params. Без field init будет NullReferenceException.
        private SimulationParams _params = new SimulationParams();
        private Bitmap _backBuffer;
        private System.Windows.Forms.Timer _timer;
        private bool _running;

        // ── Кисть ────────────────────────────────────────────────────────────
        private CellType _brushType = CellType.Fire;
        private BrushSize _brushSize = BrushSize.Single;
        private bool _mouseDown;
        private int _lastRow = -1, _lastCol = -1;

        // ── UI-поля (объявлены здесь, строятся в Designer.cs) ────────────────
        private SimulationPanel _renderPanel;
        private Panel _rightPanel;
        private Button _btnStartPause;
        private Button _btnStep;
        private Button _btnRegen;
        private Button _btnClearAll;
        private Button[] _sizeBtns = new Button[3];
        private Button[] _brushBtns = new Button[8];
        private Panel _sizePanel;
        private Panel _brushTypePanel;
        private TrackBar _trkSpeed, _trkLightning, _trkGrowth;
        private TrackBar _trkAshBonus, _trkIgnition, _trkLongRange;
        private Label _lblSpeedVal, _lblLightningVal, _lblGrowthVal;
        private Label _lblAshBonusVal, _lblIgnitionVal, _lblLongRangeVal;
        private Label _lblStatus, _lblGeneration;

        // ══════════════════════════════════════════════════════════════════════
        //  КОНСТРУКТОР
        // ══════════════════════════════════════════════════════════════════════

        public Form1()
        {
            InitializeComponent(); // _params уже создан как поле

            _automaton = new ForestAutomaton(GridRows, GridCols);
            _backBuffer = new Bitmap(
                GridCols * CellSize, GridRows * CellSize,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            _automaton.RenderTo(_backBuffer, CellSize);
            BuildBrushButtons();
            SelectBrushType(7);  // Огонь по умолчанию
            SelectBrushSize(0);  // Одна клетка
            UpdateStatus();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ПОСТРОЕНИЕ КНОПОК КИСТИ (после InitializeComponent, панели уже есть)
        // ══════════════════════════════════════════════════════════════════════

        private void BuildBrushButtons()
        {
            // ── Размеры кисти ────────────────────────────────────────────────
            string[] sizeLabels = { "▪  1×1", "✚  3×3", "⊕  5×5" };
            int panW = _sizePanel.Width;
            int bw = (panW / 3) - 2;

            for (int i = 0; i < 3; i++)
            {
                int idx = i;
                var btn = new Button
                {
                    Text = sizeLabels[i],
                    Location = new Point(i * (bw + 3), 0),
                    Size = new Size(bw, 30),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(50, 50, 58),
                    ForeColor = Color.FromArgb(200, 200, 210),
                    Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(70, 70, 82);
                btn.Click += (s, e) => SelectBrushSize(idx);
                _sizeBtns[i] = btn;
                _sizePanel.Controls.Add(btn);
            }

            // ── Типы кисти ───────────────────────────────────────────────────
            var brushDefs = new (CellType t, string label, Color color)[]
            {
                (CellType.Rock,      "■  Камень",         Color.FromArgb(118, 112, 108)),
                (CellType.Water,     "≈  Вода",           Color.FromArgb(28, 136, 240)),
                (CellType.Empty,     "□  Пусто / ластик", Color.FromArgb(185, 170, 140)),
                (CellType.Ash,       "•  Пепел",          Color.FromArgb(105, 95, 80)),
                (CellType.Grass,     "░  Трава",          Color.FromArgb(148, 202, 46)),
                (CellType.YoungTree, "▲  Молодой лес",    Color.FromArgb(34, 139, 34)),
                (CellType.AdultTree, "▲  Взрослый лес",   Color.FromArgb(0, 100, 0)),
                (CellType.Fire,      "🔥  Огонь",         Color.FromArgb(255, 120, 0)),
            };

            for (int i = 0; i < brushDefs.Length; i++)
            {
                int idx = i;
                Color sw = brushDefs[i].color;
                var btn = new Button
                {
                    Text = brushDefs[i].label,
                    Location = new Point(0, i * 30),
                    Size = new Size(_brushTypePanel.Width, 28),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(24, 0, 0, 0),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(44, 44, 50),
                    ForeColor = Color.FromArgb(210, 210, 215),
                    Font = new Font("Segoe UI", 8f),
                    Cursor = Cursors.Hand,
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(60, 60, 70);
                btn.Paint += (s, e) =>
                {
                    e.Graphics.FillRectangle(new SolidBrush(sw), 6, 7, 13, 13);
                    e.Graphics.DrawRectangle(new Pen(Color.FromArgb(155, 155, 155)), 6, 7, 13, 13);
                };
                btn.Click += (s, e) => SelectBrushType(idx);
                _brushBtns[i] = btn;
                _brushTypePanel.Controls.Add(btn);
            }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  УПРАВЛЕНИЕ СИМУЛЯЦИЕЙ
        // ══════════════════════════════════════════════════════════════════════

        private void Start()
        {
            _running = true;
            _timer.Start();
            _btnStartPause.Text = "⏸  Пауза";
            _btnStartPause.BackColor = Color.FromArgb(165, 65, 30);
            _btnStep.Enabled = false;
            UpdateStatus();
        }

        private void Pause()
        {
            _running = false;
            _timer.Stop();
            _btnStartPause.Text = "▶  Старт";
            _btnStartPause.BackColor = Color.FromArgb(38, 130, 60);
            _btnStep.Enabled = true;
            UpdateStatus();
        }

        private void BtnStartPause_Click(object sender, EventArgs e)
        {
            if (_running) Pause(); else Start();
        }

        private void BtnStep_Click(object sender, EventArgs e)
        {
            if (_running) return;
            DoStep();
        }

        private void BtnRegen_Click(object sender, EventArgs e)
        {
            // Всегда встаём на паузу при перегенерации карты
            Pause();
            _automaton.InitializeRandom();
            Redraw();
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            bool was = _running;
            Pause();
            _automaton.ClearAll();
            Redraw();
            // Очистка не восстанавливает симуляцию автоматически — пользователь сам решает
        }

        private void Timer_Tick(object sender, EventArgs e) => DoStep();

        private void DoStep()
        {
            _automaton.Step(_params);
            Redraw();
        }

        private void Redraw()
        {
            _automaton.RenderTo(_backBuffer, CellSize);
            _renderPanel.Invalidate();
            UpdateStatus();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  КИСТЬ
        // ══════════════════════════════════════════════════════════════════════

        private void SelectBrushSize(int idx)
        {
            _brushSize = (BrushSize)idx;
            for (int i = 0; i < _sizeBtns.Length; i++)
            {
                bool on = (i == idx);
                _sizeBtns[i].BackColor = on
                    ? Color.FromArgb(72, 72, 108) : Color.FromArgb(50, 50, 58);
                _sizeBtns[i].FlatAppearance.BorderColor = on
                    ? Color.FromArgb(145, 125, 220) : Color.FromArgb(70, 70, 82);
            }
        }

        private void SelectBrushType(int idx)
        {
            CellType[] map =
            {
                CellType.Rock, CellType.Water, CellType.Empty, CellType.Ash,
                CellType.Grass, CellType.YoungTree, CellType.AdultTree, CellType.Fire
            };
            _brushType = map[idx];

            for (int i = 0; i < _brushBtns.Length; i++)
            {
                bool on = (i == idx);
                _brushBtns[i].FlatAppearance.BorderColor = on
                    ? Color.FromArgb(230, 190, 55) : Color.FromArgb(60, 60, 70);
                _brushBtns[i].FlatAppearance.BorderSize = on ? 2 : 1;
                _brushBtns[i].BackColor = on
                    ? Color.FromArgb(58, 56, 38) : Color.FromArgb(44, 44, 50);
            }
        }

        private CellState MakeBrushState()
        {
            switch (_brushType)
            {
                case CellType.Rock: return RockState.Instance;
                case CellType.Water: return WaterState.Instance;
                case CellType.Empty: return EmptyState.Instance;
                case CellType.Ash: return new AshState(_params.AshDuration);
                case CellType.Grass: return new GrassState();
                case CellType.YoungTree: return new YoungTreeState();
                case CellType.AdultTree: return new AdultTreeState();
                case CellType.Fire: return new FireState(2);
                default: return EmptyState.Instance;
            }
        }

        // ── Мышь ─────────────────────────────────────────────────────────────

        private void RenderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            ApplyBrush(e.X, e.Y, e.Button);
        }

        private void RenderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseDown) ApplyBrush(e.X, e.Y, e.Button);
        }

        private void RenderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
            _lastRow = _lastCol = -1;
        }

        private void ApplyBrush(int px, int py, MouseButtons btn)
        {
            var (row, col) = _automaton.PixelToCell(px, py, CellSize);
            CellState state = btn == MouseButtons.Right
                ? EmptyState.Instance
                : MakeBrushState();

            if (_lastRow >= 0)
                _automaton.PaintLine(_lastRow, _lastCol, row, col, state, _brushSize);
            else
                _automaton.PaintBrush(row, col, state, _brushSize);

            _lastRow = row;
            _lastCol = col;

            _automaton.RenderTo(_backBuffer, CellSize);
            _renderPanel.Invalidate();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  СЛАЙДЕРЫ — изменяют параметры в реальном времени
        // ══════════════════════════════════════════════════════════════════════

        private void TrkSpeed_ValueChanged(object sender, EventArgs e)
        {
            int v = _trkSpeed.Value;
            _timer.Interval = MaxInterval - (v - 1) * (MaxInterval - MinInterval) / 9;
            if (_lblSpeedVal != null) _lblSpeedVal.Text = $"{v}";
        }

        private void TrkLightning_ValueChanged(object sender, EventArgs e)
        {
            _params.LightningProb = _trkLightning.Value * 0.000001;
            if (_lblLightningVal != null) _lblLightningVal.Text = $"{_trkLightning.Value}×10⁻⁶";
        }

        private void TrkGrowth_ValueChanged(object sender, EventArgs e)
        {
            _params.GrowthProb = _trkGrowth.Value * 0.0001;
            if (_lblGrowthVal != null) _lblGrowthVal.Text = $"{_trkGrowth.Value * 0.01:F2}%";
        }

        private void TrkAshBonus_ValueChanged(object sender, EventArgs e)
        {
            _params.AshFertilizeBonus = _trkAshBonus.Value * 0.001;
            if (_lblAshBonusVal != null) _lblAshBonusVal.Text = $"+{_trkAshBonus.Value * 0.1:F1}%";
        }

        private void TrkIgnition_ValueChanged(object sender, EventArgs e)
        {
            _params.IgnitionBase = _trkIgnition.Value * 0.01;
            if (_lblIgnitionVal != null) _lblIgnitionVal.Text = $"{_trkIgnition.Value}%";
        }

        private void TrkLongRange_ValueChanged(object sender, EventArgs e)
        {
            double v = _trkLongRange.Value * 0.01;
            _params.LongRangeLevel2 = v;
            _params.LongRangeLevel1 = v * 0.40;
            if (_lblLongRangeVal != null)
                _lblLongRangeVal.Text =
                    $"L1:{_params.LongRangeLevel1 * 100:F0}% / L2:{v * 100:F0}%";
        }

        // ══════════════════════════════════════════════════════════════════════
        //  РЕНДЕРИНГ ПАНЕЛИ (Paint = просто blit backbuffer)
        // ══════════════════════════════════════════════════════════════════════

        private void RenderPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_backBuffer, Point.Empty);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  КЛАВИАТУРА
        // ══════════════════════════════════════════════════════════════════════

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Space: BtnStartPause_Click(this, EventArgs.Empty); return true;
                case Keys.Right: BtnStep_Click(this, EventArgs.Empty); return true;
                case Keys.R: BtnRegen_Click(this, EventArgs.Empty); return true;
                case Keys.C: BtnClearAll_Click(this, EventArgs.Empty); return true;
                case Keys.D1: case Keys.NumPad1: SelectBrushSize(0); return true;
                case Keys.D2: case Keys.NumPad2: SelectBrushSize(1); return true;
                case Keys.D3: case Keys.NumPad3: SelectBrushSize(2); return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ВСПОМОГАТЕЛЬНЫЕ
        // ══════════════════════════════════════════════════════════════════════

        private void UpdateStatus()
        {
            if (_lblGeneration != null)
                _lblGeneration.Text = $"Поколение: {_automaton?.Generation:N0}";
            if (_lblStatus != null)
            {
                string st = _running ? "▶  Симуляция" : "⏸  Пауза";
                _lblStatus.Text =
                    $"{st}   Поколение: {_automaton?.Generation:N0}   " +
                    "ЛКМ — кисть  ПКМ — ластик  Пробел — старт/пауза  R — новая карта  1/2/3 — размер кисти";
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timer?.Stop();
            _backBuffer?.Dispose();
            base.OnFormClosing(e);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ПАНЕЛЬ С АППАРАТНЫМ ДВОЙНЫМ БУФЕРОМ
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Устраняет мерцание при высокочастотной перерисовке.
    /// Стандартный Panel.DoubleBuffered не публичен — нужен подкласс.
    /// </summary>
    public class SimulationPanel : Panel
    {
        public SimulationPanel()
        {
            SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint,
                true);
            UpdateStyles();
        }
    }
}