namespace WinFormsApp1
{
    public class SimulationParams
    {
        // [f] Вероятность возгорания взрослого дерева от молнии/человека за тик
        public double LightningProb { get; set; } = 0.000035;

        // [p] Базовая вероятность появления травы в пустой клетке за тик
        public double GrowthProb { get; set; } = 0.006;

        // бонус к [p] для клеток рядом с пеплом (удобрение почвы)
        public double AshFertilizeBonus { get; set; } = 0.024;

        // длительность пепла в тиках до зарастания травой
        public int AshDuration { get; set; } = 20;

        // коэффициент воспламенение от соседей 
        public double IgnitionBase { get; set; } = 0.40;

        // прибавка к шансу воспламенения за каждого дополнительного горящего соседа.</summary>
        public double IgnitionBonusPerNeighbor { get; set; } = 0.06;


        // шанс перепрыгнуть через одну клетку для огня L1
        public double LongRangeL1 { get; set; } = 0.10;

        // шанс перепрыгнуть через одну клетку для огня L2 (взрослые деревья)
        public double LongRangeL2 { get; set; } = 0.28;

        // длительность горения
        public int FireDurationL1 { get; set; } = 5;

        public int FireDurationL2 { get; set; } = 9;

        // пороги роста
        public int GrassGrowthAge { get; set; } = 25;

        public int YoungGrowthAge { get; set; } = 35;
    }
}