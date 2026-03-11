using System;
using System.Drawing;

namespace WinFormsApp1
{
    // ══════════════════════════════════════════════════════════════════════════
    //  ПЕРЕЧИСЛЕНИЕ ТИПОВ КЛЕТОК
    // ══════════════════════════════════════════════════════════════════════════

    public enum CellType
    {
        Rock,       // Камень       — непреодолимый барьер
        Water,      // Вода         — блокирует распространение огня
        Empty,      // Пустая       — очищенная/голая земля, медленно зарастает
        Ash,        // Пепел        — после пожара, удобряет и быстро зарастает
        Grass,      // Трава        — 0-й уровень растительности
        YoungTree,  // Молодые дер. — 1-й уровень растительности
        AdultTree,  // Взрослый лес — базовый лес (самый устойчивый к огню)
        Fire        // Огонь        — уровень 0/1/2 зависит от сожжённой клетки
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  АБСТРАКТНОЕ СОСТОЯНИЕ — паттерн «Состояние» (State Pattern)
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Базовый класс состояния клетки КА.
    /// Каждое конкретное состояние инкапсулирует логику перехода в себе.
    /// Общие вспомогательные методы анализа окрестности вынесены сюда.
    /// </summary>
    public abstract class CellState
    {
        public abstract CellType Type { get; }
        public abstract Color DisplayColor { get; }

        /// <summary>
        /// Уровень огня: 0/1/2 для FireState, -1 для всех остальных.
        /// Используется для определения дальности распространения.
        /// </summary>
        public virtual int FireLevel => -1;

        /// <summary>
        /// Вычисляет следующее состояние клетки.
        /// Каждый подкласс реализует свою логику перехода.
        /// </summary>
        public abstract CellState ComputeNextState(
            CellState[,] grid, int row, int col, Random rng, SimulationParams p);

        // ── Вспомогательные методы окрестности ───────────────────────────────

        protected static bool HasWaterNeighbor(CellState[,] grid, int row, int col)
        {
            int R = grid.GetLength(0), C = grid.GetLength(1);
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr >= 0 && nr < R && nc >= 0 && nc < C
                        && grid[nr, nc].Type == CellType.Water)
                        return true;
                }
            return false;
        }

        protected static bool HasAshNeighbor(CellState[,] grid, int row, int col)
        {
            int R = grid.GetLength(0), C = grid.GetLength(1);
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr >= 0 && nr < R && nc >= 0 && nc < C
                        && grid[nr, nc].Type == CellType.Ash)
                        return true;
                }
            return false;
        }

        /// <summary>
        /// Пытается воспламенить клетку.
        /// Учитывает: непосредственных горящих соседей (правило 2),
        /// дальнее распространение уровней 1-2 (через одну клетку),
        /// блокировку водой.
        /// </summary>
        /// <param name="resistanceFactor">
        /// Коэффициент воспламеняемости:
        /// 1.3 = трава (легко), 1.0 = молодые деревья, 0.55 = взрослый лес (трудно)
        /// </param>
        /// <param name="resultFireLevel">Уровень огня при воспламенении (0/1/2)</param>
        /// <returns>FireState при воспламенении или null</returns>
        protected static CellState TryIgnite(
            CellState[,] grid, int row, int col, Random rng, SimulationParams p,
            double resistanceFactor, int resultFireLevel)
        {
            // Вода блокирует огонь — дополнительное правило
            if (HasWaterNeighbor(grid, row, col)) return null;

            int R = grid.GetLength(0), C = grid.GetLength(1);

            // ── Правило 2: воспламенение от непосредственного соседа ─────────
            int immediateCount = 0;
            for (int dr = -1; dr <= 1; dr++)
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (dr == 0 && dc == 0) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr >= 0 && nr < R && nc >= 0 && nc < C
                        && grid[nr, nc].FireLevel >= 0)
                        immediateCount++;
                }

            if (immediateCount > 0)
            {
                double chance = p.IgnitionBase * resistanceFactor
                              + p.IgnitionBonusPerNeighbor * (immediateCount - 1);
                chance = Math.Min(chance, 0.95);
                if (rng.NextDouble() < chance)
                    return new FireState(resultFireLevel);
            }

            // ── Степени огня: дальнее распространение через одну клетку ─────
            // Уровень 1 (молодые деревья): малый шанс дальнего поджига
            // Уровень 2 (взрослый лес): хороший шанс дальнего поджига
            for (int dr = -2; dr <= 2; dr++)
                for (int dc = -2; dc <= 2; dc++)
                {
                    // Пропускаем непосредственную окрестность (уже обработана)
                    if (Math.Abs(dr) <= 1 && Math.Abs(dc) <= 1) continue;
                    int nr = row + dr, nc = col + dc;
                    if (nr < 0 || nr >= R || nc < 0 || nc >= C) continue;

                    int fl = grid[nr, nc].FireLevel;
                    if (fl < 1) continue; // Уровень 0 не достаёт через клетку

                    double longChance = fl == 1 ? p.LongRangeLevel1 : p.LongRangeLevel2;
                    if (rng.NextDouble() < longChance)
                        return new FireState(resultFireLevel);
                }

            return null; // Клетка не загорелась
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  КАМЕНЬ — непреодолимый барьер
    // ══════════════════════════════════════════════════════════════════════════
    public sealed class RockState : CellState
    {
        public static readonly RockState Instance = new RockState();
        private RockState() { }
        public override CellType Type => CellType.Rock;
        public override Color DisplayColor => Color.FromArgb(115, 110, 105);
        public override CellState ComputeNextState(CellState[,] g, int r, int c, Random rng, SimulationParams p)
            => this;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ВОДА — блокирует огонь (дополнительное правило)
    // ══════════════════════════════════════════════════════════════════════════
    public sealed class WaterState : CellState
    {
        public static readonly WaterState Instance = new WaterState();
        private WaterState() { }
        public override CellType Type => CellType.Water;
        public override Color DisplayColor => Color.FromArgb(28, 134, 238);
        public override CellState ComputeNextState(CellState[,] g, int r, int c, Random rng, SimulationParams p)
            => this;
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ПУСТАЯ КЛЕТКА — очищенная земля, зарастает медленно
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Пустая клетка — голая земля (например, нарисованная пользователем).
    /// Зарастает травой с вероятностью p. Может получить бонус от соседнего пепла.
    /// </summary>
    public sealed class EmptyState : CellState
    {
        public static readonly EmptyState Instance = new EmptyState();
        private EmptyState() { }
        public override CellType Type => CellType.Empty;
        public override Color DisplayColor => Color.FromArgb(185, 170, 140);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            // Медленное зарастание (правило 4 из схемы — пустое → дерево с вероятностью p)
            bool ashNear = HasAshNeighbor(grid, row, col);
            double chance = p.GrowthProb + (ashNear ? p.AshFertilizeBonus * 0.4 : 0);
            return rng.NextDouble() < chance ? (CellState)new GrassState() : this;
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ПЕПЕЛ — временный, удобряет землю (дополнительное правило)
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Пепел — остаток после пожара.
    /// ✦ Дополнительное правило: пепел может остаться после огня.
    /// ✦ Пепел удобряет почву: вероятность роста растений рядом возрастает.
    /// ✦ Со временем зарастает травой. Цвет темнеет → светлеет по мере остывания.
    /// </summary>
    public sealed class AshState : CellState
    {
        private readonly int _ticks;

        public AshState(int ticks = 20) { _ticks = ticks; }

        public override CellType Type => CellType.Ash;

        public override Color DisplayColor
        {
            get
            {
                // Тёмно-коричневый (горячий) → серо-бежевый (остывший)
                double t = 1.0 - (double)_ticks / 20.0; // 0→1
                int r = (int)(55 + t * 100);  // 55 → 155
                int g = (int)(45 + t * 90);   // 45 → 135
                int b = (int)(30 + t * 65);   // 30 → 95
                return Color.FromArgb(
                    Math.Min(r, 255), Math.Min(g, 255), Math.Min(b, 255));
            }
        }

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            if (_ticks <= 1)
                return new GrassState(); // Пепел зарос травой

            // Пепел сам по себе удобрен → растёт быстрее (p + полный бонус)
            double growthChance = p.GrowthProb + p.AshFertilizeBonus;
            if (rng.NextDouble() < growthChance)
                return new GrassState();

            return new AshState(_ticks - 1);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ТРАВА — 0-й уровень растительности, лёгкая воспламеняемость
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Трава. Воспламеняется с самой высокой вероятностью (сухая, легковоспламеняемая).
    /// При горении даёт огонь УРОВНЯ 0 — распространяется только на непосредственных соседей.
    /// Вырастает в молодые деревья.
    /// </summary>
    public sealed class GrassState : CellState
    {
        private readonly int _age;
        public GrassState(int age = 0) { _age = age; }

        public override CellType Type => CellType.Grass;
        public override Color DisplayColor => Color.FromArgb(148, 200, 45);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            // ── Воспламенение (правило 2: от соседнего огня) ─────────────────
            // resistanceFactor = 1.3 → трава горит легче всего
            // resultFireLevel  = 0   → огонь травы не достаёт через клетку
            var fired = TryIgnite(grid, row, col, rng, p, resistanceFactor: 1.3, resultFireLevel: 0);
            if (fired != null) return fired;

            // ── Рост в молодые деревья (правило 4) ────────────────────────────
            if (_age >= p.GrassGrowthAge)
            {
                bool ashNear = HasAshNeighbor(grid, row, col);
                double growthChance = p.GrowthProb * 0.6
                                    + (ashNear ? p.AshFertilizeBonus * 0.25 : 0);
                if (rng.NextDouble() < growthChance)
                    return new YoungTreeState();
            }

            return new GrassState(_age + 1);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  МОЛОДЫЕ ДЕРЕВЬЯ — 1-й уровень, умеренная воспламеняемость
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Молодые деревья. При горении дают огонь УРОВНЯ 1:
    /// распространяется на непосредственных соседей + небольшой шанс поджечь
    /// клетку через одну (LongRangeLevel1).
    /// </summary>
    public sealed class YoungTreeState : CellState
    {
        private readonly int _age;
        public YoungTreeState(int age = 0) { _age = age; }

        public override CellType Type => CellType.YoungTree;
        public override Color DisplayColor => Color.FromArgb(34, 139, 34);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            // resistanceFactor = 1.0 → стандартная воспламеняемость
            // resultFireLevel  = 1   → умеренный дальний поджиг
            var fired = TryIgnite(grid, row, col, rng, p, resistanceFactor: 1.0, resultFireLevel: 1);
            if (fired != null) return fired;

            // Рост во взрослый лес
            if (_age >= p.YoungTreeGrowthAge && rng.NextDouble() < p.GrowthProb * 0.5)
                return new AdultTreeState();

            return new YoungTreeState(_age + 1);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ВЗРОСЛЫЕ ДЕРЕВЬЯ — базовый лес, высокая стойкость к огню
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Взрослый лес. Наиболее стойкий к огню.
    /// При горении даёт огонь УРОВНЯ 2 — самый мощный:
    /// поджигает соседей с хорошей вероятностью И может поджечь клетку
    /// через одну с нормальной вероятностью (LongRangeLevel2).
    ///
    /// Правило 3: может загореться спонтанно с вероятностью f (молния/человек).
    /// </summary>
    public sealed class AdultTreeState : CellState
    {
        private readonly int _age;
        public AdultTreeState(int age = 0) { _age = age; }

        public override CellType Type => CellType.AdultTree;
        public override Color DisplayColor => Color.FromArgb(0, 100, 0);

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            // resistanceFactor = 0.55 → взрослый лес сложнее поджечь
            // resultFireLevel  = 2    → мощный огонь, далеко распространяется
            var fired = TryIgnite(grid, row, col, rng, p, resistanceFactor: 0.55, resultFireLevel: 2);
            if (fired != null) return fired;

            // ── Правило 3: спонтанное возгорание (молния / человек) ──────────
            if (rng.NextDouble() < p.LightningProb)
                return new FireState(2);

            return new AdultTreeState(_age + 1);
        }
    }

    // ══════════════════════════════════════════════════════════════════════════
    //  ОГОНЬ — три уровня интенсивности
    // ══════════════════════════════════════════════════════════════════════════

    /// <summary>
    /// Огонь с тремя уровнями интенсивности:
    ///
    ///  Уровень 0 (трава)           → тёмно-оранжевый/красный
    ///                               Горит 4 тика, только непосредственные соседи.
    ///
    ///  Уровень 1 (молодые деревья) → ярко-оранжевый
    ///                               Горит 6 тиков, + небольшой дальний поджиг.
    ///
    ///  Уровень 2 (взрослый лес)    → жёлто-белый (самый интенсивный)
    ///                               Горит 9 тиков, + хороший дальний поджиг.
    ///
    /// Правило 1: горящая клетка становится пустой (AshState) после догорания.
    /// Сам огонь НЕ распространяется активно — соседние клетки сами решают,
    /// загораться ли (распределённая логика паттерна «Состояние»).
    /// </summary>
    public sealed class FireState : CellState
    {
        public int Level { get; }
        private readonly int _ticks;

        public FireState(int level, int ticks = -1)
        {
            Level = level;
            _ticks = ticks < 0 ? DefaultDuration(level) : ticks;
        }

        private static int DefaultDuration(int lvl)
            => lvl == 0 ? 4 : lvl == 1 ? 6 : 9;

        public override CellType Type => CellType.Fire;
        public override int FireLevel => Level;

        public override Color DisplayColor
        {
            get
            {
                double t = (double)_ticks / DefaultDuration(Level); // 1.0 → 0
                switch (Level)
                {
                    case 0: // Трава — тёмный оранжево-красный огонь
                        return Color.FromArgb(210, (int)(55 * t), 0);

                    case 1: // Молодые деревья — ярко-оранжевый
                        return Color.FromArgb(255, (int)(140 * t), 0);

                    default: // Взрослый лес — жёлто-белый, самый яркий
                        int g = (int)(210 * t);
                        int b = (int)(60 * t);
                        return Color.FromArgb(255, Math.Min(g, 255), Math.Min(b, 255));
                }
            }
        }

        public override CellState ComputeNextState(CellState[,] grid, int row, int col, Random rng, SimulationParams p)
        {
            // Правило 1: горящая клетка становится пустой (пепел) после догорания
            int dur = Level == 0 ? p.FireDurationL0 : Level == 1 ? p.FireDurationL1 : p.FireDurationL2;
            if (_ticks <= 1)
                return new AshState(p.AshDuration);

            return new FireState(Level, _ticks - 1);
        }
    }
}
