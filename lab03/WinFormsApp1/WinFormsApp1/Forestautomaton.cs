using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WinFormsApp1
{
    // ══════════════════════════════════════════════════════════════════════════
    //  ПАТТЕРНЫ КИСТИ
    // ══════════════════════════════════════════════════════════════════════════

    public enum BrushSize
    {
        Single = 0,  // 1 клетка
        Cross = 1,  // Крестик 3×3 (5 клеток)
        Circle = 2   // Круг/ромб r=2 (13 клеток)
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  КЛЕТОЧНЫЙ АВТОМАТ «ЛЕСНОЙ ПОЖАР»
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Двумерный КА с двойной буферизацией.
    /// Принцип: все переходы читаются из _current и записываются в _next,
    /// затем буферы меняются местами (O(1)). Это гарантирует,
    /// что все клетки меняются одновременно.
    /// </summary>
    public class ForestAutomaton
    {
        // ── Паттерны кисти: смещения (dr, dc) от центра ─────────────────────
        private static readonly (int dr, int dc)[][] BrushPatterns =
        {
            // Размер 0: одна клетка
            new[] { (0, 0) },

            // Размер 1: крестик (5 клеток)
            new[] { (0, 0), (-1, 0), (1, 0), (0, -1), (0, 1) },

            // Размер 2: круг/ромб r=2 (13 клеток)
            new[]
            {
                (0, 0),
                (-1, 0), (1, 0), (0, -1), (0, 1),
                (-1,-1), (-1, 1), (1,-1), (1, 1),
                (-2, 0), (2, 0), (0,-2), (0, 2)
            }
        };

        // ── Поля ─────────────────────────────────────────────────────────────

        private CellState[,] _current;
        private CellState[,] _next;
        private readonly Random _rng = new Random();

        public int Rows { get; }
        public int Cols { get; }
        public long Generation { get; private set; }
        public CellState[,] Grid => _current;

        // ── Конструктор ──────────────────────────────────────────────────────

        public ForestAutomaton(int rows, int cols)
        {
            Rows = rows; Cols = cols;
            _current = new CellState[rows, cols];
            _next = new CellState[rows, cols];
            InitializeRandom();
        }

        // ── Инициализация ────────────────────────────────────────────────────

        /// <summary>
        /// Случайный лесной ландшафт без огня.
        /// Распределение: 5% камни, 4% вода, 5% трава, 18% молодые, 68% взрослые.
        /// Возраст задаётся случайно для органичного вида.
        /// </summary>
        public void InitializeRandom()
        {
            Generation = 0;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                {
                    // Генерация ландшафта с помощью наложения шумов (простая версия)
                    double v = _rng.NextDouble();
                    double nx = c / (double)Cols, ny = r / (double)Rows;

                    // Кластеризованное расположение воды (реки/озёра)
                    bool waterCluster = _rng.NextDouble() < 0.04;

                    if (v < 0.05) _current[r, c] = RockState.Instance;
                    else if (waterCluster) _current[r, c] = WaterState.Instance;
                    else if (v < 0.10) _current[r, c] = new GrassState(_rng.Next(35));
                    else if (v < 0.25) _current[r, c] = new YoungTreeState(_rng.Next(55));
                    else _current[r, c] = new AdultTreeState(_rng.Next(75));
                }

            // Сглаживаем воду: создаём блоки из смежных водных клеток
            SmoothWater();
        }

        /// <summary>
        /// Расширяет одиночные клетки воды в небольшие кластеры (реки/озёра).
        /// </summary>
        private void SmoothWater()
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                {
                    if (_current[r, c].Type != CellType.Water) continue;
                    // Расширяем воду случайно на 1-2 клетки
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            int nr = r + dr, nc = c + dc;
                            if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols
                                && _current[nr, nc].Type != CellType.Rock
                                && _rng.NextDouble() < 0.5)
                                _current[nr, nc] = WaterState.Instance;
                        }
                }
        }

        /// <summary>
        /// Полная очистка — все клетки становятся пустыми (EmptyState).
        /// </summary>
        public void ClearAll()
        {
            Generation = 0;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    _current[r, c] = EmptyState.Instance;
        }

        // ── Шаг симуляции ────────────────────────────────────────────────────

        /// <summary>
        /// Один шаг КА: вычислить _next из _current, поменять буферы местами.
        /// </summary>
        public void Step(SimulationParams p)
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    _next[r, c] = _current[r, c].ComputeNextState(_current, r, c, _rng, p);

            // Swap без копирования — O(1)
            CellState[,] tmp = _current;
            _current = _next;
            _next = tmp;

            Generation++;
        }

        // ── Рисование кистью ─────────────────────────────────────────────────

        /// <summary>
        /// Рисует кистью заданного размера в точке (row, col).
        /// </summary>
        public void PaintBrush(int row, int col, CellState state, BrushSize size)
        {
            var pattern = BrushPatterns[(int)size];
            foreach (var (dr, dc) in pattern)
                SetCell(row + dr, col + dc, state);
        }

        /// <summary>
        /// Рисует кистью по пиксельным координатам.
        /// </summary>
        public void PaintBrushByPixel(int px, int py, int cellSize, CellState state, BrushSize size)
        {
            int col = px / cellSize;
            int row = py / cellSize;
            PaintBrush(row, col, state, size);
        }

        /// <summary>
        /// Интерполирует кисть по линии Брезенхема (заполняет пропуски при быстром движении мыши).
        /// </summary>
        public void PaintLine(int r0, int c0, int r1, int c1, CellState state, BrushSize size)
        {
            int dr = Math.Abs(r1 - r0), dc = Math.Abs(c1 - c0);
            int sr = r0 < r1 ? 1 : -1, sc = c0 < c1 ? 1 : -1;
            int err = dr - dc;
            while (true)
            {
                PaintBrush(r0, c0, state, size);
                if (r0 == r1 && c0 == c1) break;
                int e2 = 2 * err;
                if (e2 > -dc) { err -= dc; r0 += sr; }
                if (e2 < dr) { err += dr; c0 += sc; }
            }
        }

        public void SetCell(int row, int col, CellState state)
        {
            if (row >= 0 && row < Rows && col >= 0 && col < Cols)
                _current[row, col] = state;
        }

        public (int row, int col) PixelToCell(int px, int py, int cellSize)
            => (py / cellSize, px / cellSize);

        // ── Рендеринг ────────────────────────────────────────────────────────

        /// <summary>
        /// Полная перерисовка сетки в Bitmap.
        /// Оптимизирована: CompositingMode.SourceCopy + NearestNeighbor.
        /// </summary>
        public void RenderTo(Bitmap bmp, int cellSize)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.PixelOffsetMode = PixelOffsetMode.None;
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.SmoothingMode = SmoothingMode.None;

                // Рисуем каждую клетку одним FillRectangle
                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    for (int r = 0; r < Rows; r++)
                        for (int c = 0; c < Cols; c++)
                        {
                            brush.Color = _current[r, c].DisplayColor;
                            g.FillRectangle(brush, c * cellSize, r * cellSize, cellSize, cellSize);
                        }
                }
            }
        }
    }
}