using System;
using CCF.Forestry.WorkEconomy;

namespace CCF.Forestry.TimberYield
{
    [Serializable]
    public sealed class AssortmentSpecification
    {
        public string Id = "";
        public string SpeciesId = ""; // Explicit species; no automatic broadleaf transfer.
        public TimberAssortment Assortment;
        public PotentialMaterialUse PotentialUse;
        public int Priority;
        public int MinimumSmallEndDiameterMm;
        public int MaximumSmallEndDiameterMm; // 0 = no maximum, otherwise inclusive.
        public int MaximumLargeEndDiameterMm;
        public int MinimumLengthMm;
        public int MaximumLengthMm;
        public int[] NominalLengthsMm = Array.Empty<int>();
        public int MinimumStartHeightMm;
        public int MaximumEndHeightMm; // 0 = full stem.
        public StemQualityFlags RequiredQuality;
        public StemQualityFlags ExcludedQuality;
        public EvidenceLabel DimensionEvidence;
        public EvidenceLabel SelectionEvidence;
        public string Source = "";
    }

    [Serializable]
    public sealed class TimberYieldConfiguration
    {
        public string Id = "";
        public LogAllocationStrategy Strategy;
        public LogLengthPreference LengthPreference;
        public int BreastHeightMm = 1300;
        public int StumpHeightMm = 350;
        public int CuttingLossMm = 5;
        public int UnallocatedScanStepMm = 100;
        // Broad engineering guards [S], not species growth limits.
        public int MaximumStemHeightMm = 100000;
        public int MaximumDbhMm = 5000;
        public string ModelSource = "";
        public AssortmentSpecification[] Specifications = Array.Empty<AssortmentSpecification>();
    }

    public static class TimberYieldDefaults
    {
        public static TimberYieldConfiguration CreateSitka()
        {
            var form = StemQualityFlags.PoorForm | StemQualityFlags.StemDefect | StemQualityFlags.WindDamage | StemQualityFlags.BrowseFormDamage | StemQualityFlags.Unusable;
            return new TimberYieldConfiguration
            {
                Id = "sitka-teagasc-volume-budgeted-v1",
                Strategy = LogAllocationStrategy.GreedyButtToTipConfiguredPriority,
                LengthPreference = LogLengthPreference.LongestFirst,
                ModelSource = "[S] DBH-anchored paraboloid / supplied volume budget. Hamilton 1975 p.190 geometry; not fitted Irish taper. Stump 0.35 m and kerf 5 mm [S]. Neutral form is an unverified assumption.",
                Specifications = new[]
                {
                    Spec("sitka-sawlog", TimberAssortment.Sawlog, PotentialMaterialUse.SawlogForProcessing, 10, 200, 0, new[] { 4900 }, form,
                        "Teagasc Fact Sheet 3 p.1: nominal 4.9 m, 20 cm small end; purchaser-dependent. Selected single published nominal length [S]."),
                    Spec("sitka-pallet", TimberAssortment.Pallet, PotentialMaterialUse.RoundwoodForProcessing, 20, 140, 0, new[] { 3700, 3400, 3100, 2500 }, form,
                        "Teagasc Fact Sheet 3 p.1: 2.5–3.7 m and >=14 cm small end; discrete lengths IFA Apr–Jun 2024. No invented diameter upper bound."),
                    Spec("sitka-stake", TimberAssortment.Stake, PotentialMaterialUse.PolesAndStakes, 30, 70, 130, new[] { 3700, 3400 }, form,
                        "Teagasc Fact Sheet 3 p.1: straight material, standard small-end 7–13 cm; increasingly 3.4–3.7 m. Upper bound is small-end, not whole-log diameter."),
                    Spec("sitka-pulp", TimberAssortment.Pulp, PotentialMaterialUse.SmallRoundwood, 40, 70, 0, new[] { 3000 }, StemQualityFlags.Unusable,
                        "Teagasc Plan your Timber Sale: >=7 cm small end; Fact Sheet 3 p.2: nominal 3 m; accepts poor-form/defective larger wood. Retention does not certify sound construction timber.")
                }
            };
        }

        private static AssortmentSpecification Spec(string id, TimberAssortment assortment, PotentialMaterialUse use, int priority,
            int minimumDiameter, int maximumDiameter, int[] lengths, StemQualityFlags excluded, string source)
        {
            int low = int.MaxValue, high = 0;
            foreach (int length in lengths) { low = Math.Min(low, length); high = Math.Max(high, length); }
            return new AssortmentSpecification { Id = id, SpeciesId = "sitka-spruce", Assortment = assortment, PotentialUse = use,
                Priority = priority, MinimumSmallEndDiameterMm = minimumDiameter, MaximumSmallEndDiameterMm = maximumDiameter,
                MinimumLengthMm = low, MaximumLengthMm = high, NominalLengthsMm = lengths, ExcludedQuality = excluded,
                DimensionEvidence = EvidenceLabel.ManagementGuidance, SelectionEvidence = EvidenceLabel.SimulationAbstraction, Source = source };
        }
    }
}
