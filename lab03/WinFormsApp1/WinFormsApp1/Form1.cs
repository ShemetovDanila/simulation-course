using System;
using System.Drawing;
using System.Windows.Forms;
using WinFormsApp1;

namespace WinFormsApp1
{
    // ══════════════════════════════════════════════════════════════════════════
    //  ГЛАВНАЯ ФОРМА
    // ══════════════════════════════════════════════════════════════════════════

    public partial class Form1 : Form
    {
        // ── Константы ────────────────────────────────────────────────────────

        private const int CellSize = 8;     // px/клетку
        private const int GridCols = 100;
        private const int GridRows = 75;
        private const int MinInterval = 16;   // ~60 FPS
        private const int MaxInterval = 600;

        // ── Поля симуляции ────────────────────────────────────────────────────

        private ForestAutomaton _automaton;
        private SimulationParams _params;
        private Bitmap _backBuffer;
        private System.Windows.Forms.Timer _timer;
        private bool _running;

        // ── Кисть ────────────────────────────────────────────────────────────

        private CellType _brushType = CellType.Fire;
        private BrushSize _brushSize = BrushSize.Single;
        private bool _mouseDown;
        private int _lastRow = -1, _lastCol = -1;

        // ── UI-поля (объявлены, инициализируются в Designer.cs) ──────────────

        private SimulationPanel _renderPanel;
        private Panel _rightPanel;

        private Button _btnStartPause;
        private Button _btnStep;
        private Button _btnRegen;
        private Button _btnClearAll;

        private Button[] _sizeBtns;   // 3 кнопки размера кисти
        private Button[] _brushBtns;  // 8 кнопок типа кисти

        // Слайдеры параметров + метки значений
        private TrackBar _trkSpeed;
        private TrackBar _trkLightning;
        private TrackBar _trkGrowth;
        private TrackBar _trkAshBonus;
        private TrackBar _trkIgnition;
        private TrackBar _trkLongRange;

        private Label _lblSpeedVal;
        private Label _lblLightningVal;
        private Label _lblGrowthVal;
        private Label _lblAshBonusVal;
        private Label _lblIgnitionVal;
        private Label _lblLongRangeVal;
        private Label _lblStatus;
        private Label _lblGeneration;

        // ══════════════════════════════════════════════════════════════════════
        //  КОНСТРУКТОР
        // ══════════════════════════════════════════════════════════════════════

        public Form1()
        {
            InitializeComponent();

            _params = new SimulationParams();
            _automaton = new ForestAutomaton(GridRows, GridCols);
            _backBuffer = new Bitmap(
                GridCols * CellSize, GridRows * CellSize,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

            _automaton.RenderTo(_backBuffer, CellSize);
            InitBrushButtons();
            SelectBrushType(6);   // Огонь по умолчанию
            SelectBrushSize(0);   // Один пиксель по умолчанию
            UpdateStatus();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ИНИЦИАЛИЗАЦИЯ КНОПОК КИСТИ
        // ══════════════════════════════════════════════════════════════════════

        private void InitBrushButtons()
        {
            // ── Размеры кисти ────────────────────────────────────────────────
            string[] sizeLabels = { "▪ 1×1", "✚ 3×3", "⊕ 5×5" };
            for (int i = 0; i < 3; i++)
            {
                int idx = i;
                _sizeBtns[i] = new Button
                {
                    Text = sizeLabels[i],
                    Width = 56,
                    Height = 28,
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(50, 50, 55),
                    ForeColor = Color.FromArgb(210, 210, 210),
                    Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    Location = new Point(2 + i * 60, 2),
                };
                _sizeBtns[i].FlatAppearance.BorderColor = Color.FromArgb(70, 70, 80);
                _sizeBtns[i].Click += (s, e) => SelectBrushSize(idx);
                _sizePanel.Controls.Add(_sizeBtns[i]);
            }

            // ── Типы кисти ───────────────────────────────────────────────────
            var brushDefs = new (CellType type, string label, Color color)[]
            {
                (CellType.Rock,      "Камень",     Color.FromArgb(115, 110, 105)),
                (CellType.Water,     "Вода",       Color.FromArgb(28, 134, 238)),
                (CellType.Empty,     "Пусто",      Color.FromArgb(185, 170, 140)),
                (CellType.Ash,       "Пепел",      Color.FromArgb(105, 95, 80)),
                (CellType.Grass,     "Трава",      Color.FromArgb(148, 200, 45)),
                (CellType.YoungTree, "Молод. лес", Color.FromArgb(34, 139, 34)),
                (CellType.AdultTree, "Взрослый лес",Color.FromArgb(0, 100, 0)),
                (CellType.Fire,      "Огонь",      Color.FromArgb(255, 120, 0)),
            };

            for (int i = 0; i < brushDefs.Length; i++)
            {
                int idx = i;
                Color swatchClr = brushDefs[i].color;

                var btn = new Button
                {
                    Text = brushDefs[i].label,
                    Height = 28,
                    Dock = DockStyle.Top,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(26, 0, 0, 0),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(45, 45, 50),
                    ForeColor = Color.FromArgb(215, 215, 215),
                    Font = new Font("Segoe UI", 8f),
                    Cursor = Cursors.Hand,
                };
                btn.FlatAppearance.BorderColor = Color.FromArgb(62, 62, 70);

                // Цветной маркер в кнопке
                btn.Paint += (s, e) =>
                {
                    e.Graphics.FillRectangle(new SolidBrush(swatchClr), 6, 6, 14, 14);
                    e.Graphics.DrawRectangle(Pens.Gray, 6, 6, 14, 14);
                };
                btn.Click += (s, e) => SelectBrushType(idx);

                _brushBtns[i] = btn;
            }

            // DockStyle.Top: добавляем в обратном порядке
            for (int i = brushDefs.Length - 1; i >= 0; i--)
                _brushTypePanel.Controls.Add(_brushBtns[i]);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ВЫБОР КИСТИ
        // ══════════════════════════════════════════════════════════════════════

        private void SelectBrushSize(int idx)
        {
            _brushSize = (BrushSize)idx;
            for (int i = 0; i < _sizeBtns.Length; i++)
            {
                bool active = (i == idx);
                _sizeBtns[i].BackColor = active
                    ? Color.FromArgb(80, 80, 100)
                    : Color.FromArgb(50, 50, 55);
                _sizeBtns[i].FlatAppearance.BorderColor = active
                    ? Color.FromArgb(160, 140, 230)
                    : Color.FromArgb(70, 70, 80);
            }
        }

        private void SelectBrushType(int idx)
        {
            CellType[] types =
            {
                CellType.Rock, CellType.Water, CellType.Empty, CellType.Ash,
                CellType.Grass, CellType.YoungTree, CellType.AdultTree, CellType.Fire
            };
            _brushType = types[idx];

            for (int i = 0; i < _brushBtns.Length; i++)
            {
                bool active = (i == idx);
                _brushBtns[i].FlatAppearance.BorderColor = active
                    ? Color.FromArgb(220, 180, 60)
                    : Color.FromArgb(62, 62, 70);
                _brushBtns[i].FlatAppearance.BorderSize = active ? 2 : 1;
            }
        }

        private CellState CreateBrushState(CellType type)
        {
            switch (type)
            {
                case CellType.Rock: return RockState.Instance;
                case CellType.Water: return WaterState.Instance;
                case CellType.Empty: return EmptyState.Instance;
                case CellType.Ash: return new AshState();
                case CellType.Grass: return new GrassState();
                case CellType.YoungTree: return new YoungTreeState();
                case CellType.AdultTree: return new AdultTreeState();
                case CellType.Fire: return new FireState(2); // Пользователь рисует макс. огонь
                default: return EmptyState.Instance;
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
            _btnStartPause.BackColor = Color.FromArgb(170, 70, 35);
            _btnStep.Enabled = false;
        }

        private void Pause()
        {
            _running = false;
            _timer.Stop();
            _btnStartPause.Text = "▶  Старт";
            _btnStartPause.BackColor = Color.FromArgb(38, 130, 60);
            _btnStep.Enabled = true;
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
            // Всегда встаём на паузу при перегенерации
            Pause();
            _automaton.InitializeRandom();
            Redraw();
            UpdateStatus();
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            bool was = _running;
            Pause();
            _automaton.ClearAll();
            Redraw();
            UpdateStatus();
            if (was) Start(); // Если была запущена — продолжаем после очистки
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DoStep();
        }

        private void DoStep()
        {
            _automaton.Step(_params);
            Redraw();
            UpdateStatus();
        }

        private void Redraw()
        {
            _automaton.RenderTo(_backBuffer, CellSize);
            _renderPanel.Invalidate();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  РИСОВАНИЕ МЫШЬЮ
        // ══════════════════════════════════════════════════════════════════════

        private void RenderPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
            ApplyBrush(e.X, e.Y, e.Button);
        }

        private void RenderPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_mouseDown) return;
            ApplyBrush(e.X, e.Y, e.Button);
        }

        private void RenderPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
            _lastRow = -1;
            _lastCol = -1;
        }

        private void ApplyBrush(int px, int py, MouseButtons btn)
        {
            var (row, col) = _automaton.PixelToCell(px, py, CellSize);

            // Выбираем состояние: ЛКМ — активная кисть, ПКМ — пусто (ластик)
            CellState state = btn == MouseButtons.Right
                ? EmptyState.Instance
                : CreateBrushState(_brushType);

            if (_lastRow >= 0)
                _automaton.PaintLine(_lastRow, _lastCol, row, col, state, _brushSize);
            else
                _automaton.PaintBrush(row, col, state, _brushSize);

            _lastRow = row;
            _lastCol = col;

            // Обновляем буфер немедленно (в паузе без таймера)
            _automaton.RenderTo(_backBuffer, CellSize);
            _renderPanel.Invalidate();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  СЛАЙДЕРЫ ПАРАМЕТРОВ
        // ══════════════════════════════════════════════════════════════════════

        private void TrkSpeed_ValueChanged(object sender, EventArgs e)
        {
            int v = _trkSpeed.Value; // 1..10
            int interval = MaxInterval - (v - 1) * (MaxInterval - MinInterval) / 9;
            _timer.Interval = interval;
            _lblSpeedVal.Text = $"{v}";
        }

        private void TrkLightning_ValueChanged(object sender, EventArgs e)
        {
            _params.LightningProb = _trkLightning.Value * 0.000001; // 0..0.0001
            _lblLightningVal.Text = $"{_trkLightning.Value}×10⁻⁶";
        }

        private void TrkGrowth_ValueChanged(object sender, EventArgs e)
        {
            _params.GrowthProb = _trkGrowth.Value * 0.0001; // 0.0001..0.02
            _lblGrowthVal.Text = $"{_trkGrowth.Value * 0.01:F2}%";
        }

        private void TrkAshBonus_ValueChanged(object sender, EventArgs e)
        {
            _params.AshFertilizeBonus = _trkAshBonus.Value * 0.001; // 0..0.05
            _lblAshBonusVal.Text = $"+{_trkAshBonus.Value * 0.1:F1}%";
        }

        private void TrkIgnition_ValueChanged(object sender, EventArgs e)
        {
            _params.IgnitionBase = _trkIgnition.Value * 0.01; // 0.10..0.90
            _lblIgnitionVal.Text = $"{_trkIgnition.Value}%";
        }

        private void TrkLongRange_ValueChanged(object sender, EventArgs e)
        {
            double v = _trkLongRange.Value * 0.01;
            _params.LongRangeLevel2 = v;           // L2: полный слайдер
            _params.LongRangeLevel1 = v * 0.40;   // L1: 40% от L2
            _lblLongRangeVal.Text = $"L1:{_params.LongRangeLevel1 * 100:F0}% / L2:{v * 100:F0}%";
        }

        // ══════════════════════════════════════════════════════════════════════
        //  РЕНДЕРИНГ ПАНЕЛИ
        // ══════════════════════════════════════════════════════════════════════

        private void RenderPanel_Paint(object sender, PaintEventArgs e)
        {
            // Paint: просто копируем готовый backbuffer. Минимум работы в UI-потоке.
            e.Graphics.DrawImageUnscaled(_backBuffer, Point.Empty);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  КЛАВИАТУРА
        // ══════════════════════════════════════════════════════════════════════

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space: BtnStartPause_Click(this, EventArgs.Empty); break;
                case Keys.Right: BtnStep_Click(this, EventArgs.Empty); break;
                case Keys.R: BtnRegen_Click(this, EventArgs.Empty); break;
                case Keys.C: BtnClearAll_Click(this, EventArgs.Empty); break;
                case Keys.D1: case Keys.NumPad1: SelectBrushSize(0); break;
                case Keys.D2: case Keys.NumPad2: SelectBrushSize(1); break;
                case Keys.D3: case Keys.NumPad3: SelectBrushSize(2); break;
            }
            base.OnKeyDown(e);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  ВСПОМОГАТЕЛЬНЫЕ
        // ══════════════════════════════════════════════════════════════════════

        private void UpdateStatus()
        {
            _lblGeneration.Text = $"Поколение: {_automaton.Generation:N0}";
            string stateStr = _running ? "▶ Симуляция" : "⏸ Пауза";
            _lblStatus.Text = $"{stateStr}  |  Gen: {_automaton.Generation:N0}"
                            + "  |  ЛКМ: кисть  |  ПКМ: ластик  |  Пробел: старт/пауза";
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Перехватываем Space до того, как WinForms отдаст его кнопке
            if (keyData == Keys.Space)
            {
                BtnStartPause_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _timer?.Stop();
            _backBuffer?.Dispose();
            base.OnFormClosing(e);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  КАСТОМНАЯ ПАНЕЛЬ С АППАРАТНЫМ ДВОЙНЫМ БУФЕРОМ
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Panel с DoubleBuffered=true и AllPaintingInWmPaint.
    /// Устраняет мерцание при частой перерисовке.
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