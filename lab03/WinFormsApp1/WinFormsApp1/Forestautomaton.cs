using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WinFormsApp1
{
    public enum BrushSize { Single = 0, Cross = 1, Circle = 2 }

    /// <summary>
    /// Клеточный автомат «Лесной пожар».
    /// Двойная буферизация: _current → читаем, _next → пишем, затем swap (O(1)).
    /// </summary>
    public class ForestAutomaton
    {
        // ── Паттерны кисти ────────────────────────────────────────────────────
        private static readonly (int dr, int dc)[][] Patterns =
        {
            new[] { (0,0) },  // Single
            new[] { (0,0),(-1,0),(1,0),(0,-1),(0,1) },   // Cross 3×3
            new[]             // Circle ~r2 (13 cells)
            {
                (0,0),
                (-1,0),(1,0),(0,-1),(0,1),
                (-1,-1),(-1,1),(1,-1),(1,1),
                (-2,0),(2,0),(0,-2),(0,2)
            }
        };

        private CellState[,] _current;
        private CellState[,] _next;
        private readonly Random _rng = new Random();

        public int Rows { get; }
        public int Cols { get; }
        public long Generation { get; private set; }
        public CellState[,] Grid => _current;

        public ForestAutomaton(int rows, int cols)
        {
            Rows = rows; Cols = cols;
            _current = new CellState[rows, cols];
            _next = new CellState[rows, cols];
            InitializeRandom();
        }

        // ── Инициализация ─────────────────────────────────────────────────────

        /// <summary>Случайный лесной ландшафт без огня.</summary>
        public void InitializeRandom()
        {
            Generation = 0;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                {
                    double v = _rng.NextDouble();
                    bool water = _rng.NextDouble() < 0.04;
                    if (v < 0.05) _current[r, c] = RockState.Instance;
                    else if (water) _current[r, c] = WaterState.Instance;
                    else if (v < 0.10) _current[r, c] = new GrassState(_rng.Next(35));
                    else if (v < 0.26) _current[r, c] = new YoungTreeState(_rng.Next(55));
                    else _current[r, c] = new AdultTreeState(_rng.Next(75));
                }
            SmoothWater();
        }

        private void SmoothWater()
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                {
                    if (_current[r, c].Type != CellType.Water) continue;
                    for (int dr = -1; dr <= 1; dr++)
                        for (int dc = -1; dc <= 1; dc++)
                        {
                            int nr = r + dr, nc = c + dc;
                            if (nr >= 0 && nr < Rows && nc >= 0 && nc < Cols
                                && _current[nr, nc].Type != CellType.Rock
                                && _rng.NextDouble() < 0.55)
                                _current[nr, nc] = WaterState.Instance;
                        }
                }
        }

        /// <summary>Полная очистка — все клетки пустые.</summary>
        public void ClearAll()
        {
            Generation = 0;
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    _current[r, c] = EmptyState.Instance;
        }

        // ── Шаг ──────────────────────────────────────────────────────────────

        public void Step(SimulationParams p)
        {
            for (int r = 0; r < Rows; r++)
                for (int c = 0; c < Cols; c++)
                    _next[r, c] = _current[r, c].ComputeNextState(_current, r, c, _rng, p);

            // Swap без копирования O(1)
            CellState[,] tmp = _current;
            _current = _next;
            _next = tmp;
            Generation++;
        }

        // ── Кисть ────────────────────────────────────────────────────────────

        public void PaintBrush(int row, int col, CellState state, BrushSize size)
        {
            foreach (var (dr, dc) in Patterns[(int)size])
                SetCell(row + dr, col + dc, state);
        }

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

        public (int row, int col) PixelToCell(int px, int py, int cs)
            => (py / cs, px / cs);

        // ── Рендер ───────────────────────────────────────────────────────────

        /// <summary>Отрисовывает весь грид в Bitmap. Единственная SolidBrush на весь вызов.</summary>
        public void RenderTo(Bitmap bmp, int cellSize)
        {
            using (var g = Graphics.FromImage(bmp))
            using (var brush = new SolidBrush(Color.Black))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighSpeed;
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = PixelOffsetMode.None;
                g.SmoothingMode = SmoothingMode.None;

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