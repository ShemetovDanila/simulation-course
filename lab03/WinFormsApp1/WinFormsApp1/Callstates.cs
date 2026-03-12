using System;
using System.Drawing;

namespace WinFormsApp1
{
    public enum CellType
    {
        Rock,       // Камень       — непреодолимый барьер
        Water,      // Вода         — непреодолимый барьер
        Empty,      // Пусто        — голая земля, постепенно зарастает
        Ash,        // Пепел        — остаток взрослого дерева, удобряет почву
        Grass,      // Трава        — 1-й уровень растительности
        YoungTree,  // Молодые дер. — 2-й уровень растительности
        AdultTree,  // Взрослый лес — 3-й уровень растительности
        Fire        // Огонь        — L1 (трава/молодые) или L2 (взрослые)
    }

    // состояния
    public abstract class CellState
    {
        public abstract CellType Type { get; }
        public abstract Color DisplayColor { get; }

        // уровень огня: 1 или 2 для FireState, 0 для остальных
        public virtual int FireLevel => 0;

        public abstract CellState ComputeNextState(
            CellState[,] grid, int row, int col, Random rng, SimulationParams p);

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

        protected static FireState TryIgnite(
            CellState[,] grid, int row, int col,
            Random rng, SimulationParams p,
            double resistFactor, int resultLevel)
        {
            int R = grid.GetLength(0), C = grid.GetLength(1);

            // непосредственные горящие соседи
            int nearFire = 0;
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr >= 0 && nr < R && nc >= 0 && nc < C && grid[nr, nc].FireLevel > 0)
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

            // перенос искр через одну клетку
            for (int dr = -2; dr <= 2; dr++)
                for (int dc = -2; dc <= 2; dc++)
                {
                    if (Math.Abs(dr) <= 1 && Math.Abs(dc) <= 1) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr < 0 || nr >= R || nc < 0 || nc >= C) continue;

                    int fl = grid[nr, nc].FireLevel;
                    if (fl == 0) continue;

                    double longChance = fl == 1 ? p.LongRangeL1 : p.LongRangeL2;
                    if (rng.NextDouble() < longChance)
                        return new FireState(resultLevel);
                }

            return null;
        }
    }

    // камень и вода
    public sealed class RockState : CellState
    {
        public static readonly RockState Instance = new RockState();
        private RockState() { }
        public override CellType Type => CellType.Rock;
        public override Color DisplayColor => Color.FromArgb(118, 112, 108);
        public override CellState ComputeNextState(CellState[,] g, int r, int c, Random rng, SimulationParams p) => this;
    }

    public sealed class WaterState : CellState
    {
        public static readonly WaterState Instance = new WaterState();
        private WaterState() { }
        public override CellType Type => CellType.Water;
        public override Color DisplayColor => Color.FromArgb(28, 136, 240);
        public override CellState ComputeNextState(CellState[,] g, int r, int c, Random rng, SimulationParams p) => this;
    }

    // пустая клетка
    public sealed class EmptyState : CellState
    {
        public static readonly EmptyState Instance = new EmptyState();
        private EmptyState() { }
        public override CellType Type => CellType.Empty;
        public override Color DisplayColor => Color.FromArgb(185, 170, 140);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            bool ash = HasAshNeighbor(grid, row, col);
            double bonus = ash ? p.AshFertilizeBonus : 0;

            double r = rng.NextDouble();
            double pg = p.GrowthProb + bonus;          // трава (основное)
            double py = pg * 0.1;                     // молодые (редко)
            double pa = pg * 0.01;                    // взрослые (очень редко)

            if (r < pa) return new AdultTreeState();
            if (r < py) return new YoungTreeState();
            if (r < pg) return new GrassState();
            return this;
        }
    }

    // пепел - только от взрослого дерева; удобряет почву клетки и соседних клеток
    public sealed class AshState : CellState
    {
        private readonly int _t, _max;
        public AshState(int ticks = 20) { _t = ticks; _max = ticks; }

        public override CellType Type => CellType.Ash;
        public override Color DisplayColor // при остывании меняет цвет
        {
            get
            {
                double k = _max > 0 ? 1.0 - (double)_t / _max : 1.0;
                return Color.FromArgb(Cl(55 + (int)(k * 105)), Cl(45 + (int)(k * 88)), Cl(30 + (int)(k * 62)));
            }
        }
        private static int Cl(int v) => v < 0 ? 0 : v > 255 ? 255 : v;

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            if (_t <= 1) return new GrassState();
            // пепел сам удобрен - зарастает быстрее
            if (rng.NextDouble() < p.GrowthProb + p.AshFertilizeBonus)
                return new GrassState();
            return new AshState(_t - 1);
        }
    }

    // растительность
    public sealed class GrassState : CellState
    {
        private readonly int _age;
        public GrassState(int age = 0) { _age = age; }
        public override CellType Type => CellType.Grass;
        public override Color DisplayColor => Color.FromArgb(148, 202, 46);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            // воспламенение в первый уровень
            var fire = TryIgnite(grid, row, col, rng, p, resistFactor: 1.4, resultLevel: 1);
            if (fire != null) return fire;

            // поджог
            if (rng.NextDouble() < p.LightningProb) return new FireState(1);

            // рост в молодые деревья
            if (_age >= p.GrassGrowthAge)
            {
                bool ash = HasAshNeighbor(grid, row, col);
                double growUp = p.GrowthProb * 0.8 + (ash ? p.AshFertilizeBonus * 0.3 : 0);
                if (rng.NextDouble() < growUp) return new YoungTreeState();
            }

            // деградация в пустоту
            if (rng.NextDouble() < 0.002) return EmptyState.Instance;

            return new GrassState(_age + 1);
        }
    }

    public sealed class YoungTreeState : CellState
    {
        private readonly int _age;
        public YoungTreeState(int age = 0) { _age = age; }
        public override CellType Type => CellType.YoungTree;
        public override Color DisplayColor => Color.FromArgb(34, 139, 34);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            // воспламенение в первый уровень
            var fire = TryIgnite(grid, row, col, rng, p, resistFactor: 1.0, resultLevel: 1);
            if (fire != null) return fire;
            
            // поджог
            if (rng.NextDouble() < p.LightningProb) return new FireState(1);

            // рост во взрослые деревья
            if (_age >= p.YoungGrowthAge && rng.NextDouble() < p.GrowthProb * 0.6)
                return new AdultTreeState();

            // деградация в траву
            if (rng.NextDouble() < 0.0001) return new GrassState();

            return new YoungTreeState(_age + 1);
        }
    }

    public sealed class AdultTreeState : CellState
    {
        private readonly int _age;
        public AdultTreeState(int age = 0) { _age = age; }
        public override CellType Type => CellType.AdultTree;
        public override Color DisplayColor => Color.FromArgb(0, 100, 0);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            // воспламенение во второй уровень
            var fire = TryIgnite(grid, row, col, rng, p, resistFactor: 0.6, resultLevel: 2);
            if (fire != null) return fire;

            // молния / поджог
            if (rng.NextDouble() < p.LightningProb) return new FireState(2);

            return new AdultTreeState(_age + 1);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ОГОНЬ — два уровня
    //
    //  L1 (трава + молодые деревья):
    //    — тёмно-красный → оранжевый, 5 тиков
    //    — после догорания: EmptyState (пепла нет)
    //    — дальний поджиг: слабый шанс через клетку
    //
    //  L2 (взрослый лес):
    //    — ярко-оранжевый → жёлто-белый, 9 тиков
    //    — после догорания: AshState (пепел всегда)
    //    — дальний поджиг: нормальный шанс через клетку
    // ══════════════════════════════════════════════════════════════════════════

    public sealed class FireState : CellState
    {
        public int Level { get; }
        private readonly int _t, _max;

        public FireState(int level, int ticks = -1)
        {
            Level = level < 1 ? 1 : level > 2 ? 2 : level;
            _max = Level == 1 ? 5 : 9;
            _t = ticks < 0 ? _max : ticks;
        }

        public override CellType Type => CellType.Fire;
        public override int FireLevel => Level;

        public override Color DisplayColor
        {
            get
            {
                double k = _max > 0 ? (double)_t / _max : 1.0; // меняет цвет по мере остывания 
                if (Level == 1)
                {
                    return Color.FromArgb(200 + (int)(55 * k), (int)(90 * k), 0);
                }
                else
                {
                    return Color.FromArgb(255, Cl((int)(215 * k)), Cl((int)(60 * k)));
                }
            }
        }

        private static int Cl(int v) => v < 0 ? 0 : v > 255 ? 255 : v;

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            if (_t <= 1)
            {
                return Level == 1 ? (CellState)EmptyState.Instance : new AshState(p.AshDuration);
            }

            return new FireState(Level, _t - 1);
        }
    }
}