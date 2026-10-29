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
        // 0 = not used. Otherwise the log's mid-length diameter must reach this (inclusive); for Oak products the
        // trade quotes a mid-log diameter. Sitka specifications leave it 0, so their allocation is unchanged.
        public int MinimumMidDiameterMm;
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

        // Scenario One yield configuration: the Sitka specifications exactly as CreateSitka, plus the Oak and Beech
        // ladders (D-062). Specifications are species-explicit, so a Sitka stem sees only Sitka rules and its
        // allocation is identical to CreateSitka; only the configuration id differs.
        public static TimberYieldConfiguration CreateScenarioOne()
        {
            TimberYieldConfiguration config = CreateSitka();
            config.Id = "scenario-one-sitka-oak-beech-v1";
            config.ModelSource += " Oak and Beech ladders: Manager-approved D-062 product ladders; eligibility from size and the stem's pruned section, not from species alone.";
            var all = new System.Collections.Generic.List<AssortmentSpecification>(config.Specifications);
            all.AddRange(BroadleafSpecifications());
            config.Specifications = all.ToArray();
            return config;
        }

        public const string OakSpeciesId = "sessile-oak";
        public const string BeechSpeciesId = "beech";

        // Oak and Beech use explicit ladders. Live eligibility uses size + recorded pruning/crown history only;
        // generic allocator defect flags below are not evidence of living-tree fork/form state in Scenario One.
        // [C] / GAMEPLAY CALIBRATION marks selected dimensions and the pruning-only quality proxy.
        public static AssortmentSpecification[] BroadleafSpecifications()
        {
            // Defects that stop a section being clean or straight. Pruned is a quality of a section, not a defect.
            var anyDefect = StemQualityFlags.PoorForm | StemQualityFlags.StemDefect | StemQualityFlags.WindDamage | StemQualityFlags.BrowseFormDamage | StemQualityFlags.Unusable;
            var structural = StemQualityFlags.PoorForm | StemQualityFlags.StemDefect | StemQualityFlags.WindDamage | StemQualityFlags.Unusable;
            var sound = StemQualityFlags.StemDefect | StemQualityFlags.WindDamage | StemQualityFlags.Unusable;
            var roundwood = StemQualityFlags.WindDamage | StemQualityFlags.Unusable;
            return new[]
            {
                // ---- Oak, highest to lowest priority ----
                Broadleaf("oak-veneer-planking", OakSpeciesId, TimberAssortment.VeneerPlanking, PotentialMaterialUse.SawlogForProcessing, 10,
                    400, 0, 450, new[] { 2800 }, StemQualityFlags.Pruned, anyDefect, EvidenceLabel.ManagementGuidance,
                    "DIRECT / ManagementGuidance: Teagasc hardwood grading, length above 2.7 m and mid-log diameter >=45 cm. GAMEPLAY CALIBRATION: selected 2.8 m length, small-end engine guard and pruning-only proxy for the source's clean grade; fork/defect shape is not modelled."),
                Broadleaf("oak-beam", OakSpeciesId, TimberAssortment.Beam, PotentialMaterialUse.SawlogForProcessing, 20,
                    250, 0, 300, new[] { 2600 }, StemQualityFlags.None, structural, EvidenceLabel.ManagementGuidance,
                    "DIRECT / ManagementGuidance: Teagasc hardwood grading, length above 2.5 m and mid-log diameter >=30 cm. GAMEPLAY CALIBRATION: selected 2.6 m length, small-end guard and generic allocator exclusions; live eligibility does not infer fork/defect shape."),
                Broadleaf("oak-fencing", OakSpeciesId, TimberAssortment.Fencing, PotentialMaterialUse.PolesAndStakes, 30,
                    120, 250, 0, new[] { 2400, 1800 }, StemQualityFlags.None, roundwood, EvidenceLabel.GameplayCalibration,
                    "[C] Fencing material: small roundwood, small end 12 to 25 cm, 1.8 or 2.4 m. Dimensions are not an Irish specification; the upper bound keeps larger sound logs for planking."),
                Broadleaf("oak-second-planking", OakSpeciesId, TimberAssortment.SecondPlanking, PotentialMaterialUse.SawlogForProcessing, 40,
                    250, 0, 0, new[] { 2400 }, StemQualityFlags.None, sound, EvidenceLabel.GameplayCalibration,
                    "[C] Second-quality planking: sound logs with a small end of 25 cm or more, 2.4 m. Dimensions are not an Irish specification."),
                Broadleaf("oak-firewood", OakSpeciesId, TimberAssortment.Firewood, PotentialMaterialUse.SmallRoundwood, 50,
                    70, 0, 0, new[] { 2000 }, StemQualityFlags.None, StemQualityFlags.None, EvidenceLabel.GameplayCalibration,
                    "[C] Fallback outlet for anything else with a small end of 7 cm or more, accepting any quality. Not a supported Irish stumpage product."),
                // ---- Beech: a shorter, lower ladder; no veneer, beam or fencing class ----
                Broadleaf("beech-second-planking", BeechSpeciesId, TimberAssortment.SecondPlanking, PotentialMaterialUse.SawlogForProcessing, 10,
                    250, 0, 0, new[] { 2400 }, StemQualityFlags.None, sound, EvidenceLabel.GameplayCalibration,
                    "[C] Second-quality planking: sound logs with a small end of 25 cm or more, 2.4 m. Beech is not given Oak's upper classes."),
                Broadleaf("beech-firewood", BeechSpeciesId, TimberAssortment.Firewood, PotentialMaterialUse.SmallRoundwood, 20,
                    70, 0, 0, new[] { 2000 }, StemQualityFlags.None, StemQualityFlags.None, EvidenceLabel.GameplayCalibration,
                    "[C] Fallback outlet for anything else with a small end of 7 cm or more, accepting any quality.")
            };
        }

        private static AssortmentSpecification Broadleaf(string id, string speciesId, TimberAssortment assortment, PotentialMaterialUse use,
            int priority, int minimumSmallEnd, int maximumSmallEnd, int minimumMid, int[] lengths,
            StemQualityFlags required, StemQualityFlags excluded, EvidenceLabel dimensionEvidence, string source)
        {
            int low = int.MaxValue, high = 0;
            foreach (int length in lengths) { low = Math.Min(low, length); high = Math.Max(high, length); }
            return new AssortmentSpecification { Id = id, SpeciesId = speciesId, Assortment = assortment, PotentialUse = use,
                Priority = priority, MinimumSmallEndDiameterMm = minimumSmallEnd, MaximumSmallEndDiameterMm = maximumSmallEnd,
                MinimumMidDiameterMm = minimumMid, MinimumLengthMm = low, MaximumLengthMm = high, NominalLengthsMm = lengths,
                RequiredQuality = required, ExcludedQuality = excluded, DimensionEvidence = dimensionEvidence,
                SelectionEvidence = EvidenceLabel.GameplayCalibration, Source = source };
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
