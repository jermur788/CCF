using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

// Which comparable areas the player's demonstrated sample can stand for (MFM-04, D-065, spec section 10). Plain data, no UnityEngine, so it
// can be checked on plain .NET (Tools/Verification/MfmPrescriptionCore) and gives the same answer everywhere.
//
// It READS state and decides nothing on its own: it stores nothing, marks no tree and creates no work. Its answer is data for the later
// layers (candidates, proposals, map) to show and for the player to accept, change or ignore. The tree/ecology simulation stays the only
// authoritative biological state; every number here is a summary derived from it.
//
// Geometry comes from the accepted MFM-02 rack-served units (ScenarioOneRackSampleGeometry). Each unit is split along the rack axis at the
// sample anchor +/- SegmentHalfLengthM into three areas (Low / Centre / High along the common rack axis; for the east-west fixture that is
// west / centre / east). The teaching rack's Centre area is the sample itself and is never a target.
//
// Measurement definitions (MFM-03, part of the metric, not tolerances):
//   * the sample and every area are compared in the same state: after rack access, BEFORE the sample treatment. The sample's own executed
//     removals are put back as they stood (a stump keeps the diameter it was felled at) for the comparison only;
//   * density divides living stems by MASKED NET area: rack corridors, infrastructure masks and the sample footprint are removed from both
//     the stem sets and the areas, identically for the sample and every area;
//   * competition is the authoritative CompetitionShare/Hegyi index of each stem over every other stem of that state.
//
// Classification is evidence-informed GAMEPLAY CALIBRATION (ScenarioOnePrescriptionCalibration), not a forestry standard.
public static class ScenarioOneCoverage
{
    // V1 quantitative rules are calibrated for Sitka adults only (spec section 9). Equals SitkaGrowthModel.SpeciesId.
    public const string SitkaSpeciesId = "sitka-spruce";

    // One adult tree as coverage sees it. A stump (Living false) keeps the diameter it was felled at.
    public readonly struct Stem
    {
        public readonly string Id;
        public readonly float X, Z, DbhCm;
        public readonly bool Living;
        public readonly string SpeciesId;
        // The player's own marks: a Crop Tree designation and a Fell mark that is not yet an order.
        public readonly bool Crop, FellMark;
        public Stem(string id, float x, float z, float dbhCm, bool living, string speciesId, bool crop = false, bool fellMark = false)
        {
            Id = id ?? ""; X = x; Z = z; DbhCm = dbhCm; Living = living; SpeciesId = speciesId ?? "";
            Crop = crop; FellMark = fellMark;
        }
        public bool IsSitka => SpeciesId == SitkaSpeciesId;
    }

    // Ground the prescription must never treat: headland/feeder, forest track, work clearing, player start and built structures. A circle
    // is a capsule whose two ends are the same point. The caller supplies them from the live infrastructure (see the MFM-04 handoff).
    public enum MaskKind { Headland, Track, Clearing, Site }

    public readonly struct Mask
    {
        public readonly MaskKind Kind;
        public readonly double AX, AZ, BX, BZ, RadiusM;
        public Mask(MaskKind kind, double ax, double az, double bx, double bz, double radiusM)
        { Kind = kind; AX = ax; AZ = az; BX = bx; BZ = bz; RadiusM = radiusM; }
        public static Mask Circle(MaskKind kind, double x, double z, double radiusM) => new Mask(kind, x, z, x, z, radiusM);
        public double MinZ => Math.Min(AZ, BZ) - RadiusM;
        public double MaxZ => Math.Max(AZ, BZ) + RadiusM;
        public bool Contains(double x, double z)
        {
            double dx = BX - AX, dz = BZ - AZ, lengthSq = dx * dx + dz * dz;
            double t = lengthSq <= 0.0 ? 0.0 : Math.Max(0.0, Math.Min(1.0, ((x - AX) * dx + (z - AZ) * dz) / lengthSq));
            double ex = x - (AX + t * dx), ez = z - (AZ + t * dz);
            return ex * ex + ez * ez <= RadiusM * RadiusM;
        }
    }

    // Management state that is not a tree. Every kind except NaturalRegeneration excludes the area it lies in; natural regeneration is
    // recorded and flagged only, because felling does not clear cohorts in the model and a blanket exclusion would remove every area (MFM-03).
    public enum FeatureKind { PlantedJuvenile, Shelter, ClearancePatch, ProtectedArea, DeadwoodRecord, ActiveManagementWork, NaturalRegeneration }

    public readonly struct Feature
    {
        public readonly FeatureKind Kind;
        public readonly string Id;
        public readonly float X, Z;
        public Feature(FeatureKind kind, string id, float x, float z) { Kind = kind; Id = id ?? ""; X = x; Z = z; }
    }

    public sealed class Input
    {
        // Must come from a CONFIRMED Rack V2 plan (MFM-02); an invalid geometry gives an invalid result, never a fallback.
        public ScenarioOneRackSampleGeometry.Result Geometry;
        public IReadOnlyList<Stem> Stems;
        public IReadOnlyList<ScenarioOneRackBay.Order> Orders;
        public IReadOnlyList<Mask> Masks;
        public IReadOnlyList<Feature> Features;
        // Areas (and remainders) the player has chosen to manage by hand. A player choice always wins.
        public IReadOnlyCollection<string> PlayerManualAreas;
    }

    public enum Segment { Low, Centre, High }
    // Spec section 10.4: every area is exactly one of these.
    public enum State { Covered, NotCovered, Excluded, Manual }
    public enum Fit { NotCompared, ClearFit, BoundaryFit, BoundaryFail, ClearFail }
    public enum Metric { Density, MeanDbh, MeanCompetition }

    // Closed vocabulary of why an area has its state. Explanations use only what the player can observe or inspect.
    public enum Reason
    {
        // Manual
        SourceSample, PlayerChoseManual, RemainderLeftForManualManagement,
        // Excluded: protected or conflicting state means the prescription must not operate here
        PlantedJuvenile, Shelter, ClearancePatch, ProtectedArea, DeadwoodRecord, ActiveManagementWork,
        PlayerFellMark, PlayerOpenWork, PlayerCompletedWork, NonSitkaAdult,
        // Not covered: materially different from the sample, or too little to compare
        TooFewStems, DensityDiffers, MeanDbhDiffers, MeanCompetitionDiffers
    }

    // Recorded for the explanation; never changes the state.
    public enum Flag { NaturalRegeneration, PlayerCropTrees }

    public sealed class Metrics
    {
        public int Stems;
        public double NetAreaM2, DensityPerHa, MeanDbhCm, MeanCompetition, BasalAreaM2;
        // Stems within the Hegyi reach of the stand boundary: their modelled competition is truncated because the model has no trees
        // beyond the property (MFM-03 section 6). Data for the explanation ("this corner is on the stand edge"), not a rule.
        public int StemsNearStandEdge;
    }

    public sealed class Comparison
    {
        public double DeltaDensity, DeltaMeanDbh, DeltaMeanCompetition;
        // Largest normalised use of any tolerance: at most 1 fits.
        public double Margin;
        public Metric LimitingMetric;
        public Fit Fit;
        public readonly List<Reason> Failures = new List<Reason>();
        public bool Fits => Failures.Count == 0;
    }

    public sealed class Area
    {
        public string Id = "";
        public int UnitIndex, RackId;
        public Segment Segment;
        public bool IsSource;
        public Metrics Metrics = new Metrics();
        // Null when the area was not compared (manual, excluded or too few stems).
        public Comparison Comparison;
        public State State;
        public readonly List<Reason> Reasons = new List<Reason>();
        public readonly List<Flag> Flags = new List<Flag>();
        // The living target stems of this area (outside corridors, masks and the sample), by id.
        public readonly List<string> StemIds = new List<string>();
        // Trees or features that caused an exclusion or flag, by id, so the player can be taken to them.
        public readonly List<string> Evidence = new List<string>();
        public Fit Fit => Comparison == null ? Fit.NotCompared : Comparison.Fit;
    }

    // The stand beyond the outermost rack-served strips (MFM-02). Never compared and never silently assigned to the sample: it is always
    // left for manual management, whether or not it is empty.
    public sealed class Remainder
    {
        public string Id = "";
        public ScenarioOneRackSampleGeometry.Side Side;
        public double AreaM2;
        public int Stems;
        public bool IsEmpty;
        public State State = State.Manual;
        public Reason Reason = Reason.RemainderLeftForManualManagement;
        public bool PlayerChoseManual;
    }

    public sealed class Result
    {
        public bool Valid;
        public string Problem = "";
        public ScenarioOnePrescriptionCalibration Calibration;
        public int TeachingRackId;
        public Metrics Source = new Metrics();
        public readonly List<string> SourceStemIds = new List<string>();
        // The sample's executed removals that were put back for the comparison.
        public readonly List<string> SampleRemovalIds = new List<string>();
        public readonly List<Area> Areas = new List<Area>();
        public readonly List<Remainder> Remainders = new List<Remainder>();
        public int CoveredCount, NonSourceCount;
        // Covered areas over all areas except the sample. Data for the open second-sample question, not a target.
        public double CoveredFraction;
        public bool SecondSampleRequired;
        // Comparison-state competition index of every source and target stem, by id.
        public readonly Dictionary<string, double> Competition = new Dictionary<string, double>(StringComparer.Ordinal);

        public Area Find(string areaId)
        {
            foreach (Area area in Areas) if (area.Id == areaId) return area;
            return null;
        }

        public string Signature()
        {
            if (!Valid) return "INVALID " + Problem;
            var sb = new StringBuilder();
            sb.Append(Calibration.Signature()).Append(" teach=").Append(TeachingRackId);
            sb.Append(" source=").Append(MetricsText(Source)).Append(" ids=").Append(string.Join(",", SourceStemIds));
            sb.Append(" put-back=").Append(string.Join(",", SampleRemovalIds));
            foreach (Area a in Areas)
            {
                sb.Append(" | ").Append(a.Id).Append(' ').Append(a.State).Append(' ').Append(a.Fit);
                sb.Append(" [").Append(string.Join(",", a.Reasons)).Append("] {").Append(string.Join(",", a.Flags)).Append("} ");
                sb.Append(MetricsText(a.Metrics));
                if (a.Comparison != null)
                    sb.Append(" d=").Append(F(a.Comparison.DeltaDensity)).Append('/').Append(F(a.Comparison.DeltaMeanDbh)).Append('/')
                        .Append(F(a.Comparison.DeltaMeanCompetition)).Append(" m=").Append(F(a.Comparison.Margin));
                sb.Append(" stems=").Append(string.Join(",", a.StemIds)).Append(" ev=").Append(string.Join(",", a.Evidence));
            }
            foreach (Remainder r in Remainders)
                sb.Append(" | ").Append(r.Id).Append(' ').Append(r.State).Append(' ').Append(r.Reason).Append(' ').Append(F(r.AreaM2)).Append(' ').Append(r.Stems);
            sb.Append(" | covered=").Append(CoveredCount).Append('/').Append(NonSourceCount).Append(" second=").Append(SecondSampleRequired);
            return sb.ToString();
        }
    }

    public static Result Classify(Input input, ScenarioOnePrescriptionCalibration calibration)
    {
        var result = new Result { Calibration = calibration };
        if (calibration == null) return Fail(result, "No calibration was supplied.");
        if (!(calibration.DensityTolerance > 0 && calibration.MeanDbhTolerance > 0 && calibration.MeanCompetitionTolerance > 0
              && calibration.MinimumStems > 0 && calibration.AreaStepM > 0 && calibration.SegmentHalfLengthM > 0))
            return Fail(result, "The calibration has a non-positive tolerance, minimum, step or segment length.");
        if (input == null || input.Geometry == null || !input.Geometry.Valid)
            return Fail(result, "There is no valid confirmed rack-served geometry" + (input?.Geometry != null ? ": " + input.Geometry.Problem : "."));
        ScenarioOneRackSampleGeometry.Result geometry = input.Geometry;
        result.TeachingRackId = geometry.TeachingRack.Id;

        // Everything below works on stems sorted by id, so the caller's order can never change a sum or a tie.
        List<Stem> stems = SortedStems(input.Stems, out string duplicate);
        if (duplicate != null) return Fail(result, "Tree id " + duplicate + " appears more than once.");
        var frame = new Frame(geometry, calibration, input.Masks);

        var completed = new HashSet<string>(StringComparer.Ordinal);
        var open = new HashSet<string>(StringComparer.Ordinal);
        if (input.Orders != null)
            foreach (ScenarioOneRackBay.Order order in input.Orders)
            {
                if (order.Completed) completed.Add(order.TargetId);
                else if (order.Open) open.Add(order.TargetId);
            }

        // ---- the comparison state: living trees plus the sample's own executed removals put back ----
        var world = new List<Stem>();
        foreach (Stem stem in stems)
        {
            if (stem.Living) world.Add(stem);
            else if (completed.Contains(stem.Id) && geometry.Teaching.Contains(stem.X, stem.Z))
            {
                world.Add(stem);
                result.SampleRemovalIds.Add(stem.Id);
            }
        }

        // ---- areas, in rack order then Low, Centre, High ----
        foreach (ScenarioOneRackSampleGeometry.CoverageUnit unit in geometry.Units)
            foreach (Segment segment in new[] { Segment.Low, Segment.Centre, Segment.High })
                result.Areas.Add(new Area
                {
                    Id = AreaId(unit.RackId, segment), UnitIndex = unit.Index, RackId = unit.RackId, Segment = segment,
                    IsSource = unit.Index == geometry.TeachingRack.Index && segment == Segment.Centre
                });
        foreach (ScenarioOneRackSampleGeometry.OuterRemainder outer in new[] { geometry.RightRemainder, geometry.LeftRemainder })
            result.Remainders.Add(new Remainder { Id = "REM-" + outer.Side, Side = outer.Side, IsEmpty = outer.IsEmpty });

        // ---- stems: source, targets, conflicts ----
        var sourceStems = new List<Stem>();
        var targetStems = new List<List<Stem>>();
        var evidence = new List<SortedSet<string>>();
        var reasons = new List<SortedSet<Reason>>();
        var flags = new List<SortedSet<Flag>>();
        for (int i = 0; i < result.Areas.Count; i++)
        {
            targetStems.Add(new List<Stem>());
            evidence.Add(new SortedSet<string>(StringComparer.Ordinal));
            reasons.Add(new SortedSet<Reason>());
            flags.Add(new SortedSet<Flag>());
        }
        foreach (Stem stem in world)
        {
            Spot spot = frame.Locate(stem.X, stem.Z, true, out int areaIndex, out int remainderIndex);
            if (spot == Spot.Sample) sourceStems.Add(stem);
            else if (spot == Spot.Area && stem.Living) targetStems[areaIndex].Add(stem);
            else if (spot == Spot.Remainder && stem.Living) result.Remainders[remainderIndex].Stems++;
        }
        // Player management and species are judged on every tree in an area, masked or not: a mark on the track is still the player's.
        foreach (Stem stem in stems)
        {
            if (frame.Locate(stem.X, stem.Z, false, out int a, out _) != Spot.Area) continue;
            if (stem.Living)
            {
                if (stem.FellMark) Note(reasons[a], evidence[a], Reason.PlayerFellMark, stem.Id);
                if (open.Contains(stem.Id)) Note(reasons[a], evidence[a], Reason.PlayerOpenWork, stem.Id);
                if (!stem.IsSitka) Note(reasons[a], evidence[a], Reason.NonSitkaAdult, stem.Id);
                if (stem.Crop) { flags[a].Add(Flag.PlayerCropTrees); evidence[a].Add(stem.Id); }
            }
            else if (completed.Contains(stem.Id)) Note(reasons[a], evidence[a], Reason.PlayerCompletedWork, stem.Id);
        }
        if (input.Features != null)
            foreach (Feature feature in input.Features)
            {
                if (frame.Locate(feature.X, feature.Z, false, out int a, out _) != Spot.Area) continue;
                if (feature.Kind == FeatureKind.NaturalRegeneration) { flags[a].Add(Flag.NaturalRegeneration); evidence[a].Add(feature.Id); }
                else Note(reasons[a], evidence[a], ExclusionFor(feature.Kind), feature.Id);
            }

        // ---- net areas ----
        frame.MeasureAreas(result.Areas.Count, out double sourceArea, out double[] areaNet, out double[] remainderArea);
        for (int i = 0; i < result.Remainders.Count; i++) result.Remainders[i].AreaM2 = remainderArea[i];

        // ---- competition in the comparison state (authoritative CompetitionShare over every other stem of that state) ----
        foreach (Stem stem in sourceStems) result.Competition[stem.Id] = CompetitionIndex(stem, world);
        foreach (List<Stem> list in targetStems)
            foreach (Stem stem in list) result.Competition[stem.Id] = CompetitionIndex(stem, world);

        // ---- the source ----
        foreach (Stem stem in sourceStems)
        {
            result.SourceStemIds.Add(stem.Id);
            if (!stem.IsSitka) return Fail(result, "The sample holds a non-Sitka adult (" + stem.Id + "); V1 quantitative rules are Sitka-only.");
        }
        result.Source = Measure(sourceStems, sourceArea, result.Competition, geometry.StandHalfExtentM);
        if (result.Source.Stems < calibration.MinimumStems)
            return Fail(result, "The sample holds " + result.Source.Stems + " stems, fewer than the " + calibration.MinimumStems + " needed to compare.");
        if (!(result.Source.NetAreaM2 > 0 && result.Source.MeanDbhCm > 0 && result.Source.MeanCompetition > 0))
            return Fail(result, "The sample has no net area, diameter or competition to compare against.");

        // ---- classify ----
        var manual = new HashSet<string>(StringComparer.Ordinal);
        if (input.PlayerManualAreas != null) foreach (string id in input.PlayerManualAreas) if (id != null) manual.Add(id);
        for (int i = 0; i < result.Areas.Count; i++)
        {
            Area area = result.Areas[i];
            area.Metrics = Measure(targetStems[i], areaNet[i], result.Competition, geometry.StandHalfExtentM);
            foreach (Stem stem in targetStems[i]) area.StemIds.Add(stem.Id);
            area.Flags.AddRange(flags[i]);
            area.Evidence.AddRange(evidence[i]);
            if (area.IsSource) Set(area, State.Manual, Reason.SourceSample);
            else if (manual.Contains(area.Id)) Set(area, State.Manual, Reason.PlayerChoseManual);
            else if (reasons[i].Count > 0) { area.State = State.Excluded; area.Reasons.AddRange(reasons[i]); }
            else if (area.Metrics.Stems < calibration.MinimumStems) Set(area, State.NotCovered, Reason.TooFewStems);
            else
            {
                area.Comparison = Compare(result.Source, area.Metrics, calibration);
                area.State = area.Comparison.Fits ? State.Covered : State.NotCovered;
                area.Reasons.AddRange(area.Comparison.Failures);
            }
            if (!area.IsSource) result.NonSourceCount++;
            if (area.State == State.Covered) result.CoveredCount++;
        }
        foreach (Remainder remainder in result.Remainders) remainder.PlayerChoseManual = manual.Contains(remainder.Id);

        result.CoveredFraction = result.NonSourceCount > 0 ? (double)result.CoveredCount / result.NonSourceCount : 0.0;
        result.SecondSampleRequired = calibration.SecondSample == ScenarioOnePrescriptionCalibration.SecondSamplePolicy.RequiredAboveCoveredFraction
            && result.CoveredFraction > calibration.SecondSampleCoveredFractionLimit;
        result.Valid = true;
        return result;
    }

    // How one target compares with the sample. Exactly at a tolerance still fits (inclusive).
    public static Comparison Compare(Metrics source, Metrics target, ScenarioOnePrescriptionCalibration calibration)
    {
        var c = new Comparison
        {
            DeltaDensity = Relative(target.DensityPerHa, source.DensityPerHa),
            DeltaMeanDbh = Relative(target.MeanDbhCm, source.MeanDbhCm),
            DeltaMeanCompetition = Relative(target.MeanCompetition, source.MeanCompetition)
        };
        if (Math.Abs(c.DeltaDensity) > calibration.DensityTolerance) c.Failures.Add(Reason.DensityDiffers);
        if (Math.Abs(c.DeltaMeanDbh) > calibration.MeanDbhTolerance) c.Failures.Add(Reason.MeanDbhDiffers);
        if (Math.Abs(c.DeltaMeanCompetition) > calibration.MeanCompetitionTolerance) c.Failures.Add(Reason.MeanCompetitionDiffers);
        double d = Math.Abs(c.DeltaDensity) / calibration.DensityTolerance;
        double b = Math.Abs(c.DeltaMeanDbh) / calibration.MeanDbhTolerance;
        double k = Math.Abs(c.DeltaMeanCompetition) / calibration.MeanCompetitionTolerance;
        c.Margin = Math.Max(d, Math.Max(b, k));
        c.LimitingMetric = d >= b && d >= k ? Metric.Density : b >= k ? Metric.MeanDbh : Metric.MeanCompetition;
        // The state follows the inclusive tolerance tests; the margin only says how close to the edge the area is.
        if (c.Fits) c.Fit = c.Margin < calibration.BoundaryMarginLow ? Fit.ClearFit : Fit.BoundaryFit;
        else c.Fit = c.Margin <= calibration.BoundaryMarginHigh ? Fit.BoundaryFail : Fit.ClearFail;
        return c;
    }

    // A stem's current competition index in a given state: CompetitionShare over every other stem of that state, all counted as standing.
    // Passing the stems sorted by id keeps the floating-point sum identical whatever order the caller used.
    public static double CompetitionIndex(Stem target, IReadOnlyList<Stem> state)
    {
        float reach = ForestEcologyController.HegyiCutoffMeters;
        var samples = new List<CompetitionSample>();
        foreach (Stem other in state)
        {
            if (other.Id == target.Id) continue;
            float dx = other.X - target.X, dz = other.Z - target.Z;
            // Cheap square pre-filter only; CompetitionShare applies the exact reach.
            if (Math.Abs(dx) > reach || Math.Abs(dz) > reach) continue;
            samples.Add(CompetitionShare.SampleFrom(other.Id, other.X, other.Z, target.X, target.Z, other.DbhCm, true, false, false));
        }
        return CompetitionShare.Measure(target.DbhCm, samples).CurrentTotal;
    }

    public static string AreaId(int rackId, Segment segment)
        => "R" + rackId + (segment == Segment.Low ? "L" : segment == Segment.Centre ? "C" : "H");

    public static double BasalAreaM2(float dbhCm) => Math.PI * Math.Pow(dbhCm / 200.0, 2.0);

    internal static List<Stem> SortedStems(IReadOnlyList<Stem> input, out string duplicate)
    {
        duplicate = null;
        var stems = new List<Stem>();
        if (input != null) foreach (Stem stem in input) if (stem.Id.Length > 0) stems.Add(stem);
        stems.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));
        for (int i = 1; i < stems.Count; i++)
            if (stems[i].Id == stems[i - 1].Id) { duplicate = stems[i].Id; break; }
        return stems;
    }

    // ---- internals ----

    private enum Spot { Outside, Corridor, Remainder, Masked, Sample, Area }

    // The geometry, segment split and masks for point location and area measurement.
    private sealed class Frame
    {
        private readonly ScenarioOneRackSampleGeometry.Result geometry;
        private readonly ScenarioOnePrescriptionCalibration calibration;
        private readonly List<Mask> masks = new List<Mask>();
        private readonly double anchorAlong;

        public Frame(ScenarioOneRackSampleGeometry.Result geometry, ScenarioOnePrescriptionCalibration calibration, IReadOnlyList<Mask> masks)
        {
            this.geometry = geometry;
            this.calibration = calibration;
            if (masks != null) this.masks.AddRange(masks);
            anchorAlong = geometry.Anchor.X * (double)geometry.AxisX + geometry.Anchor.Z * (double)geometry.AxisZ;
        }

        public Spot Locate(double x, double z, bool applyMasks, out int areaIndex, out int remainderIndex)
            => Locate(x, z, applyMasks ? masks : null, out areaIndex, out remainderIndex);

        private Spot Locate(double x, double z, List<Mask> activeMasks, out int areaIndex, out int remainderIndex)
        {
            areaIndex = -1; remainderIndex = -1;
            ScenarioOneRackSampleGeometry.Placement placement = geometry.Classify((float)x, (float)z);
            switch (placement.Kind)
            {
                case ScenarioOneRackSampleGeometry.PlacementKind.OutsideStand: return Spot.Outside;
                case ScenarioOneRackSampleGeometry.PlacementKind.RackCorridor: return Spot.Corridor;
                case ScenarioOneRackSampleGeometry.PlacementKind.OuterRemainder:
                    remainderIndex = placement.Side == ScenarioOneRackSampleGeometry.Side.Right ? 0 : 1;
                    return Spot.Remainder;
            }
            if (activeMasks != null)
                foreach (Mask mask in activeMasks)
                    if (mask.Contains(x, z)) return Spot.Masked;
            if (geometry.Teaching.Contains((float)x, (float)z)) return Spot.Sample;
            double along = x * geometry.AxisX + z * geometry.AxisZ - anchorAlong;
            int segment = along < -calibration.SegmentHalfLengthM ? 0 : along < calibration.SegmentHalfLengthM ? 1 : 2;
            areaIndex = placement.UnitIndex * 3 + segment;
            return Spot.Area;
        }

        // MFM-03's method (analyse_coverage.py net_area): midpoint strips of width AreaStepM along the rack axis, and an EXACT interval
        // calculation across each strip. Along one strip line every shape (stand square, rack corridor, mask capsule, sample rectangle,
        // unit and remainder bands) is convex, so it covers one interval of the line. Cutting the line at every interval end leaves pieces
        // on which Locate cannot change, so each piece is classified once at its middle and counted at its exact length. Locate is the
        // same precedence the stems use, so corridors, masks and the sample are removed from every area exactly once. The along range is
        // split at the segment boundaries first, so no strip straddles two segments.
        public void MeasureAreas(int areaCount, out double sourceArea, out double[] areaNet, out double[] remainderArea)
        {
            double h = geometry.StandHalfExtentM;
            double ax = geometry.AxisX, az = geometry.AxisZ, nx = geometry.NormalX, nz = geometry.NormalZ;
            double reach = h * (Math.Abs(ax) + Math.Abs(az));
            double s = calibration.SegmentHalfLengthM;
            var cuts = new List<double> { -reach, reach };
            foreach (double c in new[] { anchorAlong - s, anchorAlong + s }) if (c > -reach && c < reach) cuts.Add(c);
            cuts.Sort();

            sourceArea = 0.0;
            areaNet = new double[areaCount];
            remainderArea = new double[2];
            var ends = new List<double>();
            var stripMasks = new List<Mask>();
            for (int k = 0; k + 1 < cuts.Count; k++)
            {
                double u0 = cuts[k], u1 = cuts[k + 1];
                int n = Math.Max(1, (int)Math.Ceiling((u1 - u0) / calibration.AreaStepM - 1e-9));
                double du = (u1 - u0) / n;
                for (int i = 0; i < n; i++)
                {
                    double u = u0 + (i + 0.5) * du;
                    // The strip line: (x, z) = (ox, oz) + v (nx, nz).
                    double ox = u * ax, oz = u * az;
                    double lo = double.NegativeInfinity, hi = double.PositiveInfinity;
                    ClipSlab(ox, oz, nx, nz, 1, 0, -h, h, ref lo, ref hi);
                    ClipSlab(ox, oz, nx, nz, 0, 1, -h, h, ref lo, ref hi);
                    if (!(hi > lo)) continue;
                    ends.Clear();
                    ends.Add(lo); ends.Add(hi);
                    foreach (ScenarioOneRackSampleGeometry.OrderedRack rack in geometry.Racks)
                        AddCorridor(ends, ox, oz, nx, nz, rack.Entrance.X, rack.Entrance.Z, rack.Far.X, rack.Far.Z, rack.WidthM * 0.5);
                    ScenarioOneRackBay.Frame t = geometry.Teaching;
                    if (t.Corridors != null)
                        foreach (ScenarioOneRackBay.Corridor c in t.Corridors)
                            AddCorridor(ends, ox, oz, nx, nz, c.StartX, c.StartZ, c.EndX, c.EndZ, c.HalfWidth);
                    AddRectangle(ends, ox, oz, nx, nz, t.X, t.Z, t.AlongX, t.AlongZ, ScenarioOneRackBay.HalfLengthM, t.HalfWidth);
                    if (t.Corridors == null)
                        AddRectangle(ends, ox, oz, nx, nz, t.X, t.Z, t.AlongX, t.AlongZ, ScenarioOneRackBay.HalfLengthM, RackGeometry.DefaultWidthM * 0.5);
                    foreach (ScenarioOneRackSampleGeometry.CoverageUnit unit in geometry.Units)
                    {
                        AddCrossing(ends, ox, oz, nx, nz, nx, nz, unit.LowerOffsetM);
                        AddCrossing(ends, ox, oz, nx, nz, nx, nz, unit.UpperOffsetM);
                    }
                    stripMasks.Clear();
                    foreach (Mask mask in masks)
                        if (AddCapsule(ends, ox, oz, nx, nz, mask)) stripMasks.Add(mask);

                    ends.Sort();
                    for (int e = 0; e + 1 < ends.Count; e++)
                    {
                        double v0 = Math.Max(lo, ends[e]), v1 = Math.Min(hi, ends[e + 1]);
                        if (!(v1 > v0)) continue;
                        double v = 0.5 * (v0 + v1), piece = (v1 - v0) * du;
                        switch (Locate(ox + v * nx, oz + v * nz, stripMasks, out int a, out int r))
                        {
                            case Spot.Sample: sourceArea += piece; break;
                            case Spot.Area: areaNet[a] += piece; break;
                            case Spot.Remainder: remainderArea[r] += piece; break;
                        }
                    }
                }
            }
        }

        // Narrows [lo, hi] to where min <= (fx, fz) . (o + v d) <= max on the line o + v d.
        private static void ClipSlab(double ox, double oz, double dx, double dz, double fx, double fz, double min, double max, ref double lo, ref double hi)
        {
            double f0 = fx * ox + fz * oz, fd = fx * dx + fz * dz;
            if (Math.Abs(fd) < 1e-12)
            {
                if (f0 < min || f0 > max) { lo = 0; hi = 0; }
                return;
            }
            double a = (min - f0) / fd, b = (max - f0) / fd;
            lo = Math.Max(lo, Math.Min(a, b));
            hi = Math.Min(hi, Math.Max(a, b));
        }

        // Where the line crosses (fx, fz) . p = value.
        private static void AddCrossing(List<double> ends, double ox, double oz, double dx, double dz, double fx, double fz, double value)
        {
            double fd = fx * dx + fz * dz;
            if (Math.Abs(fd) >= 1e-12) ends.Add((value - (fx * ox + fz * oz)) / fd);
        }

        // An oriented rectangle centred on (cx, cz) with unit along direction (alx, alz).
        private static void AddRectangle(List<double> ends, double ox, double oz, double dx, double dz, double cx, double cz, double alx, double alz,
            double halfLength, double halfWidth)
        {
            double lo = double.NegativeInfinity, hi = double.PositiveInfinity;
            ClipSlab(ox - cx, oz - cz, dx, dz, alx, alz, -halfLength, halfLength, ref lo, ref hi);
            ClipSlab(ox - cx, oz - cz, dx, dz, -alz, alx, -halfWidth, halfWidth, ref lo, ref hi);
            if (hi > lo) { ends.Add(lo); ends.Add(hi); }
        }

        // A rack corridor: the rectangle from start to end, halfWidth either side.
        private static void AddCorridor(List<double> ends, double ox, double oz, double dx, double dz, double sx, double sz, double ex, double ez,
            double halfWidth)
        {
            double lx = ex - sx, lz = ez - sz, length = Math.Sqrt(lx * lx + lz * lz);
            if (length <= 0) return;
            AddRectangle(ends, ox, oz, dx, dz, (sx + ex) * 0.5, (sz + ez) * 0.5, lx / length, lz / length, length * 0.5, halfWidth);
        }

        // A mask capsule is convex, so its interval on the line is the hull of its two end discs and its middle rectangle. True when the
        // line meets it.
        private static bool AddCapsule(List<double> ends, double ox, double oz, double dx, double dz, Mask mask)
        {
            double lo = double.PositiveInfinity, hi = double.NegativeInfinity;
            Disc(ox, oz, dx, dz, mask.AX, mask.AZ, mask.RadiusM, ref lo, ref hi);
            Disc(ox, oz, dx, dz, mask.BX, mask.BZ, mask.RadiusM, ref lo, ref hi);
            double lx = mask.BX - mask.AX, lz = mask.BZ - mask.AZ, length = Math.Sqrt(lx * lx + lz * lz);
            if (length > 0)
            {
                double rlo = double.NegativeInfinity, rhi = double.PositiveInfinity;
                double cx = (mask.AX + mask.BX) * 0.5, cz = (mask.AZ + mask.BZ) * 0.5;
                ClipSlab(ox - cx, oz - cz, dx, dz, lx / length, lz / length, -length * 0.5, length * 0.5, ref rlo, ref rhi);
                ClipSlab(ox - cx, oz - cz, dx, dz, -lz / length, lx / length, -mask.RadiusM, mask.RadiusM, ref rlo, ref rhi);
                if (rhi > rlo) { lo = Math.Min(lo, rlo); hi = Math.Max(hi, rhi); }
            }
            if (!(hi > lo)) return false;
            ends.Add(lo); ends.Add(hi);
            return true;
        }

        private static void Disc(double ox, double oz, double dx, double dz, double cx, double cz, double r, ref double lo, ref double hi)
        {
            // |o + v d - c|^2 = r^2 with |d| = 1.
            double px = ox - cx, pz = oz - cz, b = px * dx + pz * dz, q = b * b - (px * px + pz * pz - r * r);
            if (q <= 0) return;
            double root = Math.Sqrt(q);
            lo = Math.Min(lo, -b - root);
            hi = Math.Max(hi, -b + root);
        }
    }

    private static Metrics Measure(List<Stem> stems, double netAreaM2, Dictionary<string, double> competition, float standHalfExtentM)
    {
        var m = new Metrics { Stems = stems.Count, NetAreaM2 = netAreaM2 };
        if (stems.Count == 0) return m;
        double dbh = 0, ci = 0;
        foreach (Stem stem in stems)
        {
            dbh += stem.DbhCm;
            ci += competition[stem.Id];
            m.BasalAreaM2 += BasalAreaM2(stem.DbhCm);
            double edge = standHalfExtentM - Math.Max(Math.Abs(stem.X), Math.Abs(stem.Z));
            if (edge < ForestEcologyController.HegyiCutoffMeters) m.StemsNearStandEdge++;
        }
        m.MeanDbhCm = dbh / stems.Count;
        m.MeanCompetition = ci / stems.Count;
        m.DensityPerHa = netAreaM2 > 0 ? stems.Count / netAreaM2 * 1e4 : 0.0;
        return m;
    }

    private static Reason ExclusionFor(FeatureKind kind)
    {
        switch (kind)
        {
            case FeatureKind.PlantedJuvenile: return Reason.PlantedJuvenile;
            case FeatureKind.Shelter: return Reason.Shelter;
            case FeatureKind.ClearancePatch: return Reason.ClearancePatch;
            case FeatureKind.ProtectedArea: return Reason.ProtectedArea;
            case FeatureKind.DeadwoodRecord: return Reason.DeadwoodRecord;
            default: return Reason.ActiveManagementWork;
        }
    }

    private static void Note(SortedSet<Reason> reasons, SortedSet<string> evidence, Reason reason, string id)
    {
        reasons.Add(reason);
        if (id.Length > 0) evidence.Add(id);
    }

    private static void Set(Area area, State state, Reason reason)
    {
        area.State = state;
        area.Reasons.Add(reason);
    }

    private static double Relative(double value, double reference) => reference != 0 ? (value - reference) / reference : 0.0;

    private static Result Fail(Result result, string problem)
    {
        result.Valid = false;
        result.Problem = problem;
        return result;
    }

    private static string MetricsText(Metrics m)
        => "n=" + m.Stems + " a=" + F(m.NetAreaM2) + " dens=" + F(m.DensityPerHa) + " dbh=" + F(m.MeanDbhCm) + " ci=" + F(m.MeanCompetition)
            + " ba=" + F(m.BasalAreaM2) + " edge=" + m.StemsNearStandEdge;

    internal static string F(double v) => v.ToString("R", CultureInfo.InvariantCulture);
}
