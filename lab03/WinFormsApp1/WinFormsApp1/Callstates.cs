using System;
using System.Drawing;

namespace WinFormsApp1
{
    // ══════════════════════════════════════════════════════════════════════════
    //  ТИПЫ КЛЕТОК
    // ══════════════════════════════════════════════════════════════════════════

    public enum CellType
    {
        Rock,       // Камень       — непреодолимый барьер
        Water,      // Вода         — блокирует распространение огня
        Empty,      // Пусто        — голая земля, зарастает медленно
        Ash,        // Пепел        — после пожара, удобряет почву
        Grass,      // Трава        — 0-й уровень растительности
        YoungTree,  // Молодые дер. — 1-й уровень
        AdultTree,  // Взрослый лес — 2-й уровень, самый стойкий
        Fire        // Огонь        — уровни 0 / 1 / 2
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  АБСТРАКТНОЕ СОСТОЯНИЕ — паттерн «Состояние»
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Базовый класс. Логика перехода инкапсулирована в каждом подклассе.
    /// Общие методы анализа окрестности вынесены сюда.
    /// </summary>
    public abstract class CellState
    {
        public abstract CellType Type { get; }
        public abstract Color DisplayColor { get; }

        /// <summary>Уровень огня (0/1/2 для FireState, -1 для остальных).</summary>
        public virtual int FireLevel => -1;

        public abstract CellState ComputeNextState(
            CellState[,] grid, int row, int col, Random rng, SimulationParams p);

        // ── Вспомогательные методы ────────────────────────────────────────────

        protected static bool HasWaterNeighbor(CellState[,] g, int r, int c)
        {
            int R = g.GetLength(0), C = g.GetLength(1);
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = r + dr, nc = c + dc;
                    if (nr >= 0 && nr < R && nc >= 0 && nc < C && g[nr, nc].Type == CellType.Water)
                        return true;
                }
            return false;
        }

        protected static bool HasAshNeighbor(CellState[,] g, int r, int c)
        {
            int R = g.GetLength(0), C = g.GetLength(1);
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = r + dr, nc = c + dc;
                    if (nr >= 0 && nr < R && nc >= 0 && nc < C && g[nr, nc].Type == CellType.Ash)
                        return true;
                }
            return false;
        }

        /// <summary>
        /// Единая точка расчёта воспламенения.
        /// Учитывает: блокировку водой, соседний огонь (правило 2),
        /// дальний огонь уровней 1-2 (через клетку), коэффициент типа клетки.
        /// </summary>
        protected static CellState TryIgnite(
            CellState[,] grid, int row, int col,
            Random rng, SimulationParams p,
            double resistFactor, int resultLevel)
        {
            // Вода полностью блокирует распространение огня
            if (HasWaterNeighbor(grid, row, col)) return null;

            int R = grid.GetLength(0), C = grid.GetLength(1);

            // ── Правило 2: непосредственные соседи (r=1) ─────────────────────
            int nearFire = 0;
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr >= 0 && nr < R && nc >= 0 && nc < C && grid[nr, nc].FireLevel >= 0)
                        nearFire++;
                }

            if (nearFire > 0)
            {
                double chance = Math.Min(
                    p.IgnitionBase * resistFactor
                    + p.IgnitionBonusPerNeighbor * (nearFire - 1),
                    0.95);
                if (rng.NextDouble() < chance)
                    return new FireState(resultLevel);
            }

            // ── Уровни огня: дальний поджиг через одну клетку (r=2) ──────────
            // Уровень 0 (трава) — только соседи, дальний поджиг не делает
            // Уровень 1 (молодые) — малый шанс
            // Уровень 2 (взрослые) — нормальный шанс
            for (int dr = -2; dr <= 2; dr++)
                for (int dc = -2; dc <= 2; dc++)
                {
                    if (Math.Abs(dr) <= 1 && Math.Abs(dc) <= 1) continue; // уже проверено
                    int nr = row + dr, nc = col + dc;
                    if (nr < 0 || nr >= R || nc < 0 || nc >= C) continue;

                    int fl = grid[nr, nc].FireLevel;
                    if (fl < 1) continue;

                    double longChance = fl == 1 ? p.LongRangeLevel1 : p.LongRangeLevel2;
                    if (rng.NextDouble() < longChance)
                        return new FireState(resultLevel);
                }

            return null;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  КАМЕНЬ
    // ══════════════════════════════════════════════════════════════════════════

    public sealed class RockState : CellState
    {
        public static readonly RockState Instance = new RockState();
        private RockState() { }
        public override CellType Type => CellType.Rock;
        public override Color DisplayColor => Color.FromArgb(118, 112, 108);
        public override CellState ComputeNextState(CellState[,] g, int r, int c, Random rng, SimulationParams p)
            => this;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ВОДА
    // ══════════════════════════════════════════════════════════════════════════

    public sealed class WaterState : CellState
    {
        public static readonly WaterState Instance = new WaterState();
        private WaterState() { }
        public override CellType Type => CellType.Water;
        public override Color DisplayColor => Color.FromArgb(28, 136, 240);
        public override CellState ComputeNextState(CellState[,] g, int r, int c, Random rng, SimulationParams p)
            => this;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ПУСТО
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>Голая земля. Зарастает с вероятностью p, ускоряется рядом с пеплом.</summary>
    public sealed class EmptyState : CellState
    {
        public static readonly EmptyState Instance = new EmptyState();
        private EmptyState() { }
        public override CellType Type => CellType.Empty;
        public override Color DisplayColor => Color.FromArgb(185, 170, 140);
        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            bool ash = HasAshNeighbor(grid, row, col);
            double chance = p.GrowthProb + (ash ? p.AshFertilizeBonus * 0.4 : 0);
            return rng.NextDouble() < chance ? (CellState)new GrassState() : this;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ПЕПЕЛ — удобряет почву (дополнительное правило)
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Пепел — остаток после пожара.
    /// • Ускоряет рост растений на себе и рядом (удобрение).
    /// • Цвет: тёмно-коричневый (горячий) → серо-бежевый (остывший).
    /// • После AshDuration тиков зарастает травой.
    /// </summary>
    public sealed class AshState : CellState
    {
        private readonly int _ticks;
        private readonly int _maxTicks;

        public AshState(int ticks = 20) { _ticks = ticks; _maxTicks = ticks; }

        public override CellType Type => CellType.Ash;

        public override Color DisplayColor
        {
            get
            {
                double t = _maxTicks > 0 ? 1.0 - (double)_ticks / _maxTicks : 1.0;
                return Color.FromArgb(
                    Clamp((int)(55 + t * 105)),
                    Clamp((int)(45 + t * 88)),
                    Clamp((int)(30 + t * 62)));
            }
        }

        private static int Clamp(int v) => v < 0 ? 0 : v > 255 ? 255 : v;

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            if (_ticks <= 1) return new GrassState();
            double chance = p.GrowthProb + p.AshFertilizeBonus; // удобрен сам по себе
            if (rng.NextDouble() < chance) return new GrassState();
            return new AshState(_ticks - 1);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ТРАВА — 0-й уровень, лёгкая воспламеняемость
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Трава горит с resistFactor=1.3 → даёт FireState(level=0).
    /// Огонь уровня 0 не распространяется дальше соседей.
    /// </summary>
    public sealed class GrassState : CellState
    {
        private readonly int _age;
        public GrassState(int age = 0) { _age = age; }
        public override CellType Type => CellType.Grass;
        public override Color DisplayColor => Color.FromArgb(148, 202, 46);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            var fired = TryIgnite(grid, row, col, rng, p, 1.3, 0);
            if (fired != null) return fired;

            if (_age >= p.GrassGrowthAge)
            {
                bool ash = HasAshNeighbor(grid, row, col);
                double chance = p.GrowthProb * 0.6 + (ash ? p.AshFertilizeBonus * 0.25 : 0);
                if (rng.NextDouble() < chance) return new YoungTreeState();
            }
            return new GrassState(_age + 1);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  МОЛОДЫЕ ДЕРЕВЬЯ — 1-й уровень
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Молодые деревья горят с resistFactor=1.0 → FireState(level=1).
    /// Огонь уровня 1: соседи + небольшой шанс через клетку.
    /// </summary>
    public sealed class YoungTreeState : CellState
    {
        private readonly int _age;
        public YoungTreeState(int age = 0) { _age = age; }
        public override CellType Type => CellType.YoungTree;
        public override Color DisplayColor => Color.FromArgb(34, 139, 34);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            var fired = TryIgnite(grid, row, col, rng, p, 1.0, 1);
            if (fired != null) return fired;

            if (_age >= p.YoungTreeGrowthAge && rng.NextDouble() < p.GrowthProb * 0.5)
                return new AdultTreeState();
            return new YoungTreeState(_age + 1);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ВЗРОСЛЫЕ ДЕРЕВЬЯ — 2-й уровень, самый стойкий
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Взрослый лес горит с resistFactor=0.55 → FireState(level=2).
    /// Огонь уровня 2: соседи + хороший шанс через клетку.
    /// Правило 3: спонтанное возгорание от молнии/человека.
    /// </summary>
    public sealed class AdultTreeState : CellState
    {
        private readonly int _age;
        public AdultTreeState(int age = 0) { _age = age; }
        public override CellType Type => CellType.AdultTree;
        public override Color DisplayColor => Color.FromArgb(0, 100, 0);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            var fired = TryIgnite(grid, row, col, rng, p, 0.55, 2);
            if (fired != null) return fired;

            // Правило 3: молния / человеческий фактор
            if (rng.NextDouble() < p.LightningProb) return new FireState(2);

            return new AdultTreeState(_age + 1);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ОГОНЬ — три уровня интенсивности
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Огонь с тремя уровнями:
    ///   0 = трава      → тёмно-красный,  4 тика, только соседи
    ///   1 = молодые    → оранжевый,      6 тиков, + малый шанс через клетку
    ///   2 = взрослые   → жёлто-белый,    9 тиков, + нормальный шанс через клетку
    ///
    /// Правило 1: горящая клетка → пепел после догорания.
    /// </summary>
    public sealed class FireState : CellState
    {
        public int Level { get; }
        private readonly int _ticks;
        private readonly int _maxTicks;

        public FireState(int level, int ticks = -1)
        {
            Level = level;
            _maxTicks = level == 0 ? 4 : level == 1 ? 6 : 9;
            _ticks = ticks < 0 ? _maxTicks : ticks;
        }

        public override CellType Type => CellType.Fire;
        public override int FireLevel => Level;

        public override Color DisplayColor
        {
            get
            {
                double t = _maxTicks > 0 ? (double)_ticks / _maxTicks : 1.0;
                switch (Level)
                {
                    case 0: return Color.FromArgb(210, (int)(50 * t), 0);
                    case 1: return Color.FromArgb(255, (int)(130 * t), 0);
                    default:
                        return Color.FromArgb(255,
                            Math.Min(255, (int)(215 * t)),
                            Math.Min(255, (int)(55 * t)));
                }
            }
        }

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            int dur = Level == 0 ? p.FireDurationL0 : Level == 1 ? p.FireDurationL1 : p.FireDurationL2;
            if (_ticks <= 1) return new AshState(p.AshDuration); // Правило 1: огонь → пепел
            return new FireState(Level, _ticks - 1);
        }
    }
}