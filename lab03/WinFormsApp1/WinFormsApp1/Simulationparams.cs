namespace WinFormsApp1
{
    /// <summary>
    /// Все настраиваемые параметры КА. Изменение любого поля
    /// немедленно влияет на следующий шаг симуляции.
    /// </summary>
    public class SimulationParams
    {
        // ── Основные 4 правила ───────────────────────────────────────────────

        /// <summary>[f] Вероятность спонтанного возгорания (молния/человек) за тик.</summary>
        public double LightningProb { get; set; } = 0.000035;

        /// <summary>[p] Вероятность роста нового растения в пустой клетке за тик.</summary>
        public double GrowthProb { get; set; } = 0.006;

        // ── Дополнительные правила ───────────────────────────────────────────

        /// <summary>Бонус к вероятности роста рядом/на пепле (удобрение).</summary>
        public double AshFertilizeBonus { get; set; } = 0.024;

        /// <summary>Длительность пепла в тиках перед зарастанием травой.</summary>
        public int AshDuration { get; set; } = 20;

        // ── Воспламенение ────────────────────────────────────────────────────

        /// <summary>
        /// Базовая вероятность воспламенения от одного горящего соседа.
        /// Масштабируется коэффициентом типа клетки:
        ///   Трава ×1.3  |  Молодые ×1.0  |  Взрослые ×0.55
        /// </summary>
        public double IgnitionBase { get; set; } = 0.45;

        /// <summary>Дополнительный бонус за каждого лишнего горящего соседа.</summary>
        public double IgnitionBonusPerNeighbor { get; set; } = 0.06;

        // ── Уровни огня: дальнее распространение ────────────────────────────

        /// <summary>Шанс поджечь клетку через одну для огня уровня 1 (молодые деревья).</summary>
        public double LongRangeLevel1 { get; set; } = 0.12;

        /// <summary>Шанс поджечь клетку через одну для огня уровня 2 (взрослый лес).</summary>
        public double LongRangeLevel2 { get; set; } = 0.30;

        // ── Длительность горения по уровням ─────────────────────────────────

        /// <summary>Трава: короткое горение (тики).</summary>
        public int FireDurationL0 { get; set; } = 4;

        /// <summary>Молодые деревья: среднее горение (тики).</summary>
        public int FireDurationL1 { get; set; } = 6;

        /// <summary>Взрослый лес: долгое и жаркое горение (тики).</summary>
        public int FireDurationL2 { get; set; } = 9;

        // ── Возраст роста растений ───────────────────────────────────────────

        /// <summary>Минимальный возраст травы перед ростом в молодые деревья.</summary>
        public int GrassGrowthAge { get; set; } = 38;

        /// <summary>Минимальный возраст молодых деревьев перед ростом во взрослые.</summary>
        public int YoungTreeGrowthAge { get; set; } = 62;
    }
}