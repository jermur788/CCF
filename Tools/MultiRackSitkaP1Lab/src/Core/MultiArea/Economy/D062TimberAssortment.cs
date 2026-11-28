using System.Globalization;

namespace SitkaP1.Core;

// Pure .NET port of D-062 Oak/Beech/Sitka timber assortment and pricing semantics
// (reference only: Assets/ForestPrototype/TimberYield/TimberYieldConfiguration.cs, MerchantableStemModel.cs,
// TimberYieldCalculator.cs, Assets/ForestPrototype/WorkEconomy/Stage1EconomyDefaults.cs).
// Zero Unity imports; strict species-explicit rules; strict stem-volume conservation.

/// <summary>Timber product assortment classes (Sitka: Pulp..Sawlog; Oak/Beech D-062: VeneerPlanking..Firewood).</summary>
internal enum TimberAssortmentKind
{
    Pulp = 0,
    Stake = 1,
    Pallet = 2,
    Sawlog = 3,
    VeneerPlanking = 4,
    Beam = 5,
    Fencing = 6,
    SecondPlanking = 7,
    Firewood = 8,
    // Scots pine Part 2 sortments (graded by ScotsPineEngine.GradeTimber; pine Sawlog uses Sawlog above).
    SmallRoundwood = 9,
    PulpChip = 10,
}

/// <summary>Stem quality flags for D-062 log grading. Pruning is a positive section quality, never a price multiplier.</summary>
[Flags]
internal enum StemQualityFlags
{
    None = 0,
    PoorForm = 1,
    StemDefect = 2,
    WindDamage = 4,
    BrowseFormDamage = 8,
    Pruned = 16,
    Unusable = 32,
}

/// <summary>One height-bounded quality section on a stem (in millimetres from ground).</summary>
internal readonly record struct StemQualitySection(int StartHeightMm, int EndHeightMm, StemQualityFlags Flags);

/// <summary>One species-explicit D-062 / Sitka assortment specification.</summary>
internal sealed record D062AssortmentSpec(
    string Id,
    string SpeciesId,
    TimberAssortmentKind Assortment,
    int Priority,
    int MinimumSmallEndDiameterMm,
    int MaximumSmallEndDiameterMm,
    int MinimumMidDiameterMm,
    IReadOnlyList<int> NominalLengthsMm,
    StemQualityFlags RequiredQuality,
    StemQualityFlags ExcludedQuality,
    EconomyEvidenceLabel DimensionEvidence,
    EconomyEvidenceLabel SelectionEvidence,
    string Source);

/// <summary>One roadside price entry (in integer euro cents per tonne).</summary>
internal sealed record D062PriceEntry(
    string Id,
    string SpeciesId,
    TimberAssortmentKind Assortment,
    long LowCentsPerTonne,
    long ReferenceCentsPerTonne,
    long HighCentsPerTonne,
    long SelectedCentsPerTonne,
    long? ReferenceCentsPerCubicMetre,
    EconomyEvidenceLabel RangeEvidence,
    EconomyEvidenceLabel SelectedEvidence,
    string Source);

/// <summary>One allocated merchantable log cut from a felled stem.</summary>
internal sealed record D062AllocatedLog(
    string LogId,
    string TreeId,
    string SpeciesId,
    string SpecificationId,
    TimberAssortmentKind Assortment,
    int StartHeightMm,
    int EndHeightMm,
    int SmallEndDiameterMm,
    int LargeEndDiameterMm,
    long VolumeCm3,
    long GreenGrams,
    long UnitPriceCentsPerTonne,
    long RevenueCents,
    StemQualityFlags Quality);

/// <summary>Full conserved yield & valuation breakdown of one felled stem.</summary>
internal sealed record D062StemYield(
    string TreeId,
    string SpeciesId,
    int DbhMm,
    int TotalHeightMm,
    long StemVolumeCm3,
    long SaleVolumeCm3,
    long ResidualVolumeCm3,
    long HarvestedGreenGrams,
    long TimberRevenueCents,
    bool HasPrunedSection,
    int PrunedEndHeightMm,
    IReadOnlyList<D062AllocatedLog> Logs);

/// <summary>Aggregated yield & revenue for one (SpeciesId, Assortment) pair across a harvest job.</summary>
internal sealed record D062AssortmentSummary(
    string SpeciesId,
    TimberAssortmentKind Assortment,
    int PieceCount,
    long VolumeCm3,
    long GreenGrams,
    long UnitPriceCentsPerTonne,
    long RevenueCents);

/// <summary>Authoritative D-062 / Sitka price book and species-explicit assortment specification catalog.</summary>
internal static class D062TimberPriceBook
{
    /// <summary>
    /// Fresh-volume work-equivalent proxy (890 kg/m³). A unit convention so broadleaf €/m³ references convert cleanly
    /// to whole €/t matching the harvest work-mass basis — NOT a claimed Oak or Beech green density.
    /// </summary>
    internal const long FreshVolumeProxyKgPerCubicMetre = 890L;

    internal const int BreastHeightMm = 1300;
    internal const int StumpHeightMm = 350;
    internal const int CuttingLossMm = 5;
    internal const int UnallocatedScanStepMm = 100;

    /// <summary>Converts €/m³ (in cents) to whole €/t (in cents) at the 890 kg/m³ fresh-volume work-equivalent proxy.</summary>
    internal static long CentsPerTonneAtProxy(long centsPerCubicMetre)
    {
        decimal perTonne = (decimal)centsPerCubicMetre * 1000m / FreshVolumeProxyKgPerCubicMetre;
        return checked((long)decimal.Round(perTonne / 100m, 0, MidpointRounding.AwayFromZero) * 100L);
    }

    private const StemQualityFlags FormDefects =
        StemQualityFlags.PoorForm | StemQualityFlags.StemDefect | StemQualityFlags.WindDamage |
        StemQualityFlags.BrowseFormDamage | StemQualityFlags.Unusable;

    private const StemQualityFlags StructuralDefects =
        StemQualityFlags.PoorForm | StemQualityFlags.StemDefect | StemQualityFlags.WindDamage | StemQualityFlags.Unusable;

    private const StemQualityFlags SoundDefects =
        StemQualityFlags.StemDefect | StemQualityFlags.WindDamage | StemQualityFlags.Unusable;

    private const StemQualityFlags RoundwoodDefects =
        StemQualityFlags.WindDamage | StemQualityFlags.Unusable;

    /// <summary>All species-explicit specifications (Sitka + Oak + Beech). Each stem only ever inspects its own species.</summary>
    internal static readonly IReadOnlyList<D062AssortmentSpec> Specifications = Array.AsReadOnly(new[]
    {
        // ---- Sitka spruce (unchanged Stage 1 Sitka ladder) ----
        new D062AssortmentSpec("sitka-sawlog", Phase2Species.SitkaId, TimberAssortmentKind.Sawlog, 10,
            200, 0, 0, Array.AsReadOnly(new[] { 4900 }), StemQualityFlags.None, FormDefects,
            EconomyEvidenceLabel.ManagementGuidance, EconomyEvidenceLabel.SimulationAbstraction,
            "Teagasc Fact Sheet 3: nominal 4.9 m, >=20 cm small end [MANAGEMENT GUIDANCE / SIMULATION ABSTRACTION]"),
        new D062AssortmentSpec("sitka-pallet", Phase2Species.SitkaId, TimberAssortmentKind.Pallet, 20,
            140, 0, 0, Array.AsReadOnly(new[] { 3700, 3400, 3100, 2500 }), StemQualityFlags.None, FormDefects,
            EconomyEvidenceLabel.ManagementGuidance, EconomyEvidenceLabel.SimulationAbstraction,
            "Teagasc Fact Sheet 3: 2.5–3.7 m, >=14 cm small end [MANAGEMENT GUIDANCE / SIMULATION ABSTRACTION]"),
        new D062AssortmentSpec("sitka-stake", Phase2Species.SitkaId, TimberAssortmentKind.Stake, 30,
            70, 130, 0, Array.AsReadOnly(new[] { 3700, 3400 }), StemQualityFlags.None, FormDefects,
            EconomyEvidenceLabel.ManagementGuidance, EconomyEvidenceLabel.SimulationAbstraction,
            "Teagasc Fact Sheet 3: 3.4–3.7 m, 7–13 cm small end [MANAGEMENT GUIDANCE / SIMULATION ABSTRACTION]"),
        new D062AssortmentSpec("sitka-pulp", Phase2Species.SitkaId, TimberAssortmentKind.Pulp, 40,
            70, 0, 0, Array.AsReadOnly(new[] { 3000 }), StemQualityFlags.None, StemQualityFlags.Unusable,
            EconomyEvidenceLabel.ManagementGuidance, EconomyEvidenceLabel.SimulationAbstraction,
            "Teagasc Fact Sheet 3: 3.0 m, >=7 cm small end [MANAGEMENT GUIDANCE / SIMULATION ABSTRACTION]"),

        // ---- Sessile Oak (D-062 full hardwood ladder: Veneer -> Beam -> Fencing -> SecondPlanking -> Firewood) ----
        new D062AssortmentSpec("oak-veneer-planking", Phase2Species.OakId, TimberAssortmentKind.VeneerPlanking, 10,
            400, 0, 450, Array.AsReadOnly(new[] { 2800 }), StemQualityFlags.Pruned, FormDefects,
            EconomyEvidenceLabel.ManagementGuidance, EconomyEvidenceLabel.GameplayCalibration,
            "Teagasc hardwood grading: pruned section, mid-log >=450 mm, 2.8 m [MANAGEMENT GUIDANCE / GAMEPLAY CALIBRATION]"),
        new D062AssortmentSpec("oak-beam", Phase2Species.OakId, TimberAssortmentKind.Beam, 20,
            250, 0, 300, Array.AsReadOnly(new[] { 2600 }), StemQualityFlags.None, StructuralDefects,
            EconomyEvidenceLabel.ManagementGuidance, EconomyEvidenceLabel.GameplayCalibration,
            "Teagasc hardwood grading: mid-log >=300 mm, 2.6 m [MANAGEMENT GUIDANCE / GAMEPLAY CALIBRATION]"),
        new D062AssortmentSpec("oak-fencing", Phase2Species.OakId, TimberAssortmentKind.Fencing, 30,
            120, 250, 0, Array.AsReadOnly(new[] { 2400, 1800 }), StemQualityFlags.None, RoundwoodDefects,
            EconomyEvidenceLabel.GameplayCalibration, EconomyEvidenceLabel.GameplayCalibration,
            "Oak fencing small roundwood: 12–25 cm small end, 1.8/2.4 m [GAMEPLAY CALIBRATION]"),
        new D062AssortmentSpec("oak-second-planking", Phase2Species.OakId, TimberAssortmentKind.SecondPlanking, 40,
            250, 0, 0, Array.AsReadOnly(new[] { 2400 }), StemQualityFlags.None, SoundDefects,
            EconomyEvidenceLabel.GameplayCalibration, EconomyEvidenceLabel.GameplayCalibration,
            "Oak second-quality planking: >=250 mm small end, 2.4 m [GAMEPLAY CALIBRATION]"),
        new D062AssortmentSpec("oak-firewood", Phase2Species.OakId, TimberAssortmentKind.Firewood, 50,
            70, 0, 0, Array.AsReadOnly(new[] { 2000 }), StemQualityFlags.None, StemQualityFlags.None,
            EconomyEvidenceLabel.GameplayCalibration, EconomyEvidenceLabel.GameplayCalibration,
            "Oak firewood fallback: >=70 mm small end, 2.0 m [GAMEPLAY CALIBRATION]"),

        // ---- European Beech (D-062 shorter ladder: SecondPlanking and Firewood ONLY) ----
        new D062AssortmentSpec("beech-second-planking", Phase2Species.BeechId, TimberAssortmentKind.SecondPlanking, 10,
            250, 0, 0, Array.AsReadOnly(new[] { 2400 }), StemQualityFlags.None, SoundDefects,
            EconomyEvidenceLabel.GameplayCalibration, EconomyEvidenceLabel.GameplayCalibration,
            "Beech second-quality planking: >=250 mm small end, 2.4 m; Beech never inherits Oak veneer/beam/fencing [GAMEPLAY CALIBRATION]"),
        new D062AssortmentSpec("beech-firewood", Phase2Species.BeechId, TimberAssortmentKind.Firewood, 20,
            70, 0, 0, Array.AsReadOnly(new[] { 2000 }), StemQualityFlags.None, StemQualityFlags.None,
            EconomyEvidenceLabel.GameplayCalibration, EconomyEvidenceLabel.GameplayCalibration,
            "Beech firewood fallback: >=70 mm small end, 2.0 m [GAMEPLAY CALIBRATION]"),
    });

    /// <summary>Roadside price entries keyed by (SpeciesId, Assortment). No Irish broadleaf price series is claimed.</summary>
    internal static readonly IReadOnlyList<D062PriceEntry> Prices = Array.AsReadOnly(new[]
    {
        // ---- Sitka spruce (IFA 2024 roadside ex VAT range [EMPIRICAL]; generic selected [GAMEPLAY CALIBRATION]) ----
        SitkaPrice("sitka-pulp", TimberAssortmentKind.Pulp, 3600, 3800, 4000),
        SitkaPrice("sitka-stake", TimberAssortmentKind.Stake, 4200, 4700, 5200),
        SitkaPrice("sitka-pallet", TimberAssortmentKind.Pallet, 4800, 6800, 7500),
        SitkaPrice("sitka-sawlog", TimberAssortmentKind.Sawlog, 8800, 9500, 10500),

        // ---- Sessile Oak (D-062 €/m³ converted at 890 kg/m³ work-equivalent proxy, rounded to whole €/t) ----
        BroadleafPrice("oak-veneer-planking", Phase2Species.OakId, TimberAssortmentKind.VeneerPlanking,
            18000, 23000, 28000, EconomyEvidenceLabel.ManagementGuidance,
            "Teagasc hardwood quotation ~€180–280/m³ (ref €230/m³ midpoint and roadside basis: GAMEPLAY CALIBRATION; 890 kg/m³ proxy)"),
        BroadleafPrice("oak-beam", Phase2Species.OakId, TimberAssortmentKind.Beam,
            14500, 14500, 14500, EconomyEvidenceLabel.ManagementGuidance,
            "Teagasc hardwood quotation ~€145/m³ (roadside basis: GAMEPLAY CALIBRATION; 890 kg/m³ proxy)"),
        BroadleafPrice("oak-fencing", Phase2Species.OakId, TimberAssortmentKind.Fencing,
            10000, 10000, 10000, EconomyEvidenceLabel.ManagementGuidance,
            "Teagasc hardwood quotation ~€100/m³ (roadside basis: GAMEPLAY CALIBRATION; 890 kg/m³ proxy)"),
        BroadleafPrice("oak-second-planking", Phase2Species.OakId, TimberAssortmentKind.SecondPlanking,
            6000, 6000, 6000, EconomyEvidenceLabel.ManagementGuidance,
            "Teagasc hardwood quotation ~€60/m³ (roadside basis: GAMEPLAY CALIBRATION; 890 kg/m³ proxy)"),
        BroadleafPrice("oak-firewood", Phase2Species.OakId, TimberAssortmentKind.Firewood,
            2000, 3000, 4000, EconomyEvidenceLabel.GameplayCalibration,
            "Fallback firewood €20–40/m³ (ref €30/m³ GAMEPLAY CALIBRATION; not an Irish stumpage price series; 890 kg/m³ proxy)"),

        // ---- European Beech (SecondPlanking and Firewood ONLY) ----
        BroadleafPrice("beech-second-planking", Phase2Species.BeechId, TimberAssortmentKind.SecondPlanking,
            6000, 6000, 6000, EconomyEvidenceLabel.ManagementGuidance,
            "Teagasc hardwood quotation ~€60/m³ second-quality planking (roadside basis: GAMEPLAY CALIBRATION; 890 kg/m³ proxy)"),
        BroadleafPrice("beech-firewood", Phase2Species.BeechId, TimberAssortmentKind.Firewood,
            2000, 3000, 4000, EconomyEvidenceLabel.GameplayCalibration,
            "Fallback firewood €20–40/m³ (ref €30/m³ GAMEPLAY CALIBRATION; not an Irish stumpage price series; 890 kg/m³ proxy)"),
    });

    private static readonly IReadOnlyDictionary<(string SpeciesId, TimberAssortmentKind Assortment), D062PriceEntry> PriceMap =
        Prices.ToDictionary(p => (p.SpeciesId, p.Assortment));

    internal static D062PriceEntry FindPrice(string speciesId, TimberAssortmentKind assortment)
    {
        if (PriceMap.TryGetValue((speciesId, assortment), out var entry)) return entry;
        throw new P1Refusal("EconomyPriceMissing", $"no roadside price exists for {speciesId}/{assortment}");
    }

    internal static IReadOnlyList<D062AssortmentSpec> SpecificationsFor(string speciesId) =>
        Array.AsReadOnly(Specifications
            .Where(s => string.Equals(s.SpeciesId, speciesId, StringComparison.Ordinal))
            .OrderBy(s => s.Priority)
            .ThenBy(s => s.Id, StringComparer.Ordinal)
            .ToArray());

    private static D062PriceEntry SitkaPrice(string id, TimberAssortmentKind assortment, long low, long reference, long high) =>
        new(id, Phase2Species.SitkaId, assortment, low, reference, high, reference, null,
            EconomyEvidenceLabel.Empirical, EconomyEvidenceLabel.GameplayCalibration,
            "IFA 2024 roadside ex VAT range [EMPIRICAL]; selected generic reference [GAMEPLAY CALIBRATION]");

    private static D062PriceEntry BroadleafPrice(string id, string speciesId, TimberAssortmentKind assortment,
        long lowPerM3, long refPerM3, long highPerM3, EconomyEvidenceLabel rangeEvidence, string source)
    {
        long lowT = CentsPerTonneAtProxy(lowPerM3);
        long refT = CentsPerTonneAtProxy(refPerM3);
        long highT = CentsPerTonneAtProxy(highPerM3);
        return new(id, speciesId, assortment, lowT, refT, highT, refT, refPerM3,
            rangeEvidence, EconomyEvidenceLabel.GameplayCalibration, source);
    }
}

/// <summary>Pure deterministic D-062 stem grading and timber revenue calculator.</summary>
internal static class D062TimberAssortmentEngine
{
    /// <summary>
    /// Grades one felled tree using its species-explicit assortment ladder and recorded pruning history.
    /// Strict volume conservation holds: <c>SaleVolumeCm3 + ResidualVolumeCm3 == StemVolumeCm3</c>.
    /// </summary>
    internal static D062StemYield GradeStem(AnnualTree tree, string speciesId, TreePruningState pruning,
        StemQualityFlags stemQuality = StemQualityFlags.None, bool tightEarlyStocking = false)
    {
        ArgumentNullException.ThrowIfNull(tree);
        if (string.IsNullOrWhiteSpace(speciesId))
            throw new P1Refusal("EconomySpeciesRequired", "a species ID is required for timber grading");
        if (ScotsPineLive.IsPine(speciesId))
            return GradeScotsPine(tree, pruning, tightEarlyStocking);

        checked
        {
            int dbhMm = (int)Math.Round(tree.DbhCm * 10.0, MidpointRounding.AwayFromZero);
            int totalHeightMm = (int)Math.Round(tree.HeightM * 1000.0, MidpointRounding.AwayFromZero);
            float stemVolM3 = FallenDeadwoodRecord.StemVolumeM3(tree.DbhCm, tree.HeightM);
            long stemVolumeCm3 = stemVolM3 <= 0f ? 0L : (long)decimal.Floor((decimal)stemVolM3 * 1_000_000m);
            long harvestedGrams = stemVolumeCm3 == 0L
                ? 0L
                : EconomyDefaults.RoundRatio(stemVolumeCm3, D062TimberPriceBook.FreshVolumeProxyKgPerCubicMetre, 1000L);

            var specs = D062TimberPriceBook.SpecificationsFor(speciesId);
            // Pruning quality bridge: only species with a RequiredQuality rung (Oak) read the pruned section,
            // matching ScenarioOneEconomyAdapter.cs so Sitka and Beech are unaffected by pruning.
            bool needsPrunedQuality = specs.Any(s => s.RequiredQuality != StemQualityFlags.None);
            int prunedEndMm = 0;
            StemQualitySection[] qualitySections = Array.Empty<StemQualitySection>();
            if (needsPrunedQuality && pruning.Lifts > 0 && pruning.CrownBaseHeightM > 0f && totalHeightMm > 1)
            {
                int candidateEnd = Math.Min(totalHeightMm - 1, (int)Math.Round(pruning.CrownBaseHeightM * 1000f, MidpointRounding.AwayFromZero));
                if (candidateEnd > 0)
                {
                    prunedEndMm = candidateEnd;
                    qualitySections = new[] { new StemQualitySection(0, prunedEndMm, StemQualityFlags.Pruned) };
                }
            }

            if (stemVolumeCm3 == 0L || specs.Count == 0 || totalHeightMm <= D062TimberPriceBook.BreastHeightMm || dbhMm <= 0)
            {
                return new D062StemYield(tree.TreeId, speciesId, dbhMm, totalHeightMm, stemVolumeCm3,
                    SaleVolumeCm3: 0L, ResidualVolumeCm3: stemVolumeCm3, harvestedGrams, TimberRevenueCents: 0L,
                    HasPrunedSection: prunedEndMm > 0, PrunedEndHeightMm: prunedEndMm,
                    Logs: Array.Empty<D062AllocatedLog>());
            }

            int minDiameter = int.MaxValue;
            var lengthsBySpec = new Dictionary<string, int[]>(StringComparer.Ordinal);
            foreach (var spec in specs)
            {
                if (spec.MinimumSmallEndDiameterMm < minDiameter) minDiameter = spec.MinimumSmallEndDiameterMm;
                var choices = spec.NominalLengthsMm.ToArray();
                Array.Sort(choices);
                Array.Reverse(choices); // LongestFirst
                lengthsBySpec[spec.Id] = choices;
            }

            int merchantableTopMm = HeightToDiameter(dbhMm, totalHeightMm, minDiameter);
            int position = Math.Min(D062TimberPriceBook.StumpHeightMm, totalHeightMm);
            var logs = new List<D062AllocatedLog>();
            long saleVolumeCm3 = 0L;
            long totalRevenueCents = 0L;

            while (position < totalHeightMm && position < merchantableTopMm)
            {
                D062AssortmentSpec? selected = null;
                int selectedEnd = position;
                StemQualityFlags selectedQuality = stemQuality;

                foreach (var spec in specs)
                {
                    foreach (int length in lengthsBySpec[spec.Id])
                    {
                        if (length > totalHeightMm - position) continue;
                        int candidateEnd = position + length;
                        decimal smallSq = DiameterSquaredAt(dbhMm, totalHeightMm, candidateEnd);
                        if (smallSq < (decimal)spec.MinimumSmallEndDiameterMm * spec.MinimumSmallEndDiameterMm) continue;
                        if (spec.MaximumSmallEndDiameterMm > 0 &&
                            smallSq > (decimal)spec.MaximumSmallEndDiameterMm * spec.MaximumSmallEndDiameterMm) continue;
                        if (spec.MinimumMidDiameterMm > 0 &&
                            DiameterSquaredAt(dbhMm, totalHeightMm, position + length / 2) <
                            (decimal)spec.MinimumMidDiameterMm * spec.MinimumMidDiameterMm) continue;

                        var unionQuality = QualityUnion(stemQuality, qualitySections, position, candidateEnd);
                        if ((unionQuality & spec.ExcludedQuality) != 0) continue;
                        if (spec.RequiredQuality != StemQualityFlags.None &&
                            (QualityThroughout(stemQuality, qualitySections, position, candidateEnd) & spec.RequiredQuality) != spec.RequiredQuality)
                            continue;

                        selected = spec;
                        selectedEnd = candidateEnd;
                        selectedQuality = unionQuality;
                        break;
                    }
                    if (selected is not null) break;
                }

                if (selected is null)
                {
                    position += Math.Min(D062TimberPriceBook.UnallocatedScanStepMm, totalHeightMm - position);
                    continue;
                }

                long logVolumeCm3 = VolumeBetween(stemVolumeCm3, totalHeightMm, position, selectedEnd);
                if (logVolumeCm3 > 0L)
                {
                    long logGrams = EconomyDefaults.RoundRatio(logVolumeCm3, D062TimberPriceBook.FreshVolumeProxyKgPerCubicMetre, 1000L);
                    long unitPrice = D062TimberPriceBook.FindPrice(speciesId, selected.Assortment).SelectedCentsPerTonne;
                    long revenue = EconomyDefaults.RoundRatio(logGrams, unitPrice, EconomyDefaults.GramsPerTonne);
                    logs.Add(new D062AllocatedLog(
                        LogId: tree.TreeId + "/log/" + position.ToString(CultureInfo.InvariantCulture),
                        TreeId: tree.TreeId,
                        SpeciesId: speciesId,
                        SpecificationId: selected.Id,
                        Assortment: selected.Assortment,
                        StartHeightMm: position,
                        EndHeightMm: selectedEnd,
                        SmallEndDiameterMm: DiameterFloorAt(dbhMm, totalHeightMm, selectedEnd),
                        LargeEndDiameterMm: DiameterFloorAt(dbhMm, totalHeightMm, position),
                        VolumeCm3: logVolumeCm3,
                        GreenGrams: logGrams,
                        UnitPriceCentsPerTonne: unitPrice,
                        RevenueCents: revenue,
                        Quality: selectedQuality));
                    saleVolumeCm3 += logVolumeCm3;
                    totalRevenueCents += revenue;
                }

                int afterCut = selectedEnd + Math.Min(D062TimberPriceBook.CuttingLossMm, totalHeightMm - selectedEnd);
                position = afterCut;
            }

            long residualVolumeCm3 = stemVolumeCm3 - saleVolumeCm3;
            if (residualVolumeCm3 < 0L || saleVolumeCm3 + residualVolumeCm3 != stemVolumeCm3)
                throw new InvalidOperationException("D-062 stem material conservation invariant violated.");

            return new D062StemYield(tree.TreeId, speciesId, dbhMm, totalHeightMm, stemVolumeCm3,
                saleVolumeCm3, residualVolumeCm3, harvestedGrams, totalRevenueCents,
                HasPrunedSection: prunedEndMm > 0, PrunedEndHeightMm: prunedEndMm,
                Logs: Array.AsReadOnly(logs.ToArray()));
        }
    }

    /// <summary>
    /// One felled Scots pine, graded whole-stem by the Part 1 engine (SPEC-multi-2d-scots-pine-moisture-V1): Sawlog EUR 68/t
    /// (mature and pruned to at least 5.0 m, or grown under tight early stocking), else Small Roundwood EUR 45/t or Pulp/Chip
    /// EUR 38/t, at the pine card's 1.02 green t/m³ — never the Sitka ladder, prices or the 890 kg/m³ proxy. [Calibration]
    /// </summary>
    internal static D062StemYield GradeScotsPine(AnnualTree tree, TreePruningState pruning, bool tightEarlyStocking)
    {
        checked
        {
            int dbhMm = (int)Math.Round(tree.DbhCm * 10.0, MidpointRounding.AwayFromZero);
            int totalHeightMm = (int)Math.Round(tree.HeightM * 1000.0, MidpointRounding.AwayFromZero);
            float stemVolM3 = FallenDeadwoodRecord.StemVolumeM3(tree.DbhCm, tree.HeightM);
            long stemVolumeCm3 = stemVolM3 <= 0f ? 0L : (long)decimal.Floor((decimal)stemVolM3 * 1_000_000m);
            var grade = ScotsPineGrading.Grade(tree, stemVolumeCm3 / 1_000_000m, pruning, tightEarlyStocking);
            long greenGrams = (long)decimal.Round(grade.GreenTonnes * EconomyDefaults.GramsPerTonne, 0, MidpointRounding.AwayFromZero);
            long unitCents = (long)(grade.UnitPriceEurPerTonne * 100m);
            long revenueCents = (long)decimal.Round(grade.GrossValueEur * 100m, 0, MidpointRounding.AwayFromZero);
            int prunedEndMm = pruning.Lifts > 0 && totalHeightMm > 1
                ? Math.Min(totalHeightMm - 1, (int)Math.Round(pruning.CrownBaseHeightM * 1000f, MidpointRounding.AwayFromZero))
                : 0;
            if (stemVolumeCm3 == 0L)
                return new D062StemYield(tree.TreeId, ScotsPineLive.SpeciesId, dbhMm, totalHeightMm, 0L, 0L, 0L, 0L, 0L,
                    prunedEndMm > 0, prunedEndMm, Array.Empty<D062AllocatedLog>());
            var kind = grade.Sortment switch
            {
                SitkaP1.Core.SpeciesWave2.Sortment.Sawlog => TimberAssortmentKind.Sawlog,
                SitkaP1.Core.SpeciesWave2.Sortment.SmallRoundwood => TimberAssortmentKind.SmallRoundwood,
                _ => TimberAssortmentKind.PulpChip,
            };
            var log = new D062AllocatedLog(tree.TreeId + "-L1", tree.TreeId, ScotsPineLive.SpeciesId, "scots-pine-" + kind.ToString().ToLowerInvariant(),
                kind, 0, totalHeightMm, 0, dbhMm, stemVolumeCm3, greenGrams, unitCents, revenueCents,
                prunedEndMm > 0 ? StemQualityFlags.Pruned : StemQualityFlags.None);
            return new D062StemYield(tree.TreeId, ScotsPineLive.SpeciesId, dbhMm, totalHeightMm, stemVolumeCm3,
                SaleVolumeCm3: stemVolumeCm3, ResidualVolumeCm3: 0L, greenGrams, revenueCents, prunedEndMm > 0, prunedEndMm,
                Array.AsReadOnly(new[] { log }));
        }
    }

    internal static IReadOnlyList<D062AssortmentSummary> Summarize(IEnumerable<D062StemYield> stems)
    {
        var map = new Dictionary<(string SpeciesId, TimberAssortmentKind Assortment), (int Pieces, long Vol, long Grams, long UnitPrice, long Rev)>();
        foreach (var stem in stems)
        {
            foreach (var log in stem.Logs)
            {
                var key = (log.SpeciesId, log.Assortment);
                if (map.TryGetValue(key, out var cur))
                    map[key] = (checked(cur.Pieces + 1), checked(cur.Vol + log.VolumeCm3), checked(cur.Grams + log.GreenGrams),
                        log.UnitPriceCentsPerTonne, checked(cur.Rev + log.RevenueCents));
                else
                    map[key] = (1, log.VolumeCm3, log.GreenGrams, log.UnitPriceCentsPerTonne, log.RevenueCents);
            }
        }
        return Array.AsReadOnly(map
            .OrderBy(kv => kv.Key.SpeciesId, StringComparer.Ordinal)
            .ThenBy(kv => kv.Key.Assortment)
            .Select(kv => new D062AssortmentSummary(kv.Key.SpeciesId, kv.Key.Assortment,
                kv.Value.Pieces, kv.Value.Vol, kv.Value.Grams, kv.Value.UnitPrice, kv.Value.Rev))
            .ToArray());
    }

    private static decimal DiameterSquaredAt(int dbhMm, int totalHeightMm, int heightMm) =>
        (decimal)dbhMm * dbhMm * (totalHeightMm - heightMm) / (totalHeightMm - D062TimberPriceBook.BreastHeightMm);

    private static int DiameterFloorAt(int dbhMm, int totalHeightMm, int heightMm)
    {
        decimal sq = DiameterSquaredAt(dbhMm, totalHeightMm, heightMm);
        int result = checked((int)Math.Sqrt((double)sq));
        while ((decimal)result * result > sq) result--;
        while ((decimal)(result + 1) * (result + 1) <= sq) result++;
        return result;
    }

    private static long CumulativeVolumeAt(long stemVolumeCm3, int totalHeightMm, int heightMm)
    {
        if (heightMm == totalHeightMm) return stemVolumeCm3;
        decimal fraction = ((decimal)2 * totalHeightMm * heightMm - (decimal)heightMm * heightMm)
            / ((decimal)totalHeightMm * totalHeightMm);
        return checked((long)decimal.Floor(stemVolumeCm3 * fraction));
    }

    private static long VolumeBetween(long stemVolumeCm3, int totalHeightMm, int startMm, int endMm) =>
        checked(CumulativeVolumeAt(stemVolumeCm3, totalHeightMm, endMm) - CumulativeVolumeAt(stemVolumeCm3, totalHeightMm, startMm));

    private static int HeightToDiameter(int dbhMm, int totalHeightMm, int diameterMm)
    {
        decimal sq = (decimal)diameterMm * diameterMm;
        if (DiameterSquaredAt(dbhMm, totalHeightMm, 0) < sq) return 0;
        int low = 0, high = totalHeightMm;
        while (high - low > 1)
        {
            int mid = low + (high - low) / 2;
            if (DiameterSquaredAt(dbhMm, totalHeightMm, mid) >= sq) low = mid;
            else high = mid;
        }
        return low;
    }

    private static StemQualityFlags QualityUnion(StemQualityFlags baseFlags, StemQualitySection[] sections, int start, int end)
    {
        var flags = baseFlags;
        foreach (var s in sections)
            if (s.StartHeightMm < end && s.EndHeightMm > start) flags |= s.Flags;
        return flags;
    }

    private static StemQualityFlags QualityThroughout(StemQualityFlags baseFlags, StemQualitySection[] sections, int start, int end)
    {
        var boundaries = new List<int> { start };
        foreach (var s in sections)
        {
            if (s.StartHeightMm > start && s.StartHeightMm < end) boundaries.Add(s.StartHeightMm);
            if (s.EndHeightMm > start && s.EndHeightMm < end) boundaries.Add(s.EndHeightMm);
        }
        var common = (StemQualityFlags)63;
        foreach (int pt in boundaries)
        {
            var flags = baseFlags;
            foreach (var s in sections)
                if (s.StartHeightMm <= pt && s.EndHeightMm > pt) flags |= s.Flags;
            common &= flags;
        }
        return common;
    }
}
