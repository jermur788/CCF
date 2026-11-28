using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.ScotsPine;

namespace SitkaP1.Core;

/// <summary>One moisture zone of the woodland, in stand coordinates, with its plain label and bog caution.</summary>
internal sealed record SiteMoistureZoneView(string MoistureClass, string Label, double MinXM, double MaxXM, double MinZM, double MaxZM,
    double AreaM2, bool HabitatCaution);

/// <summary>The separate establishment and growth modifiers of one live species on one moisture class (from the Part 1 table).</summary>
internal sealed record SiteMoistureModifierView(string SpeciesId, string MoistureClass, double Establishment, double Growth,
    string EstablishmentTier, string GrowthTier);

/// <summary>The woodland's site moisture as the player sees it: label, caution, zones and every species' modifiers per zone.</summary>
internal sealed record SiteMoistureView(string Mode, string WholeClass, string WholeStandLabel, bool IsNeutral, bool HabitatCaution,
    string? HabitatCautionText, string DominantByAreaClass, IReadOnlyList<SiteMoistureZoneView> Zones,
    IReadOnlyList<SiteMoistureModifierView> Modifiers, string Evidence);

/// <summary>The Scots pine and site-moisture read-out of one committed root (null parts when the woodland has none).</summary>
internal sealed record MultiAreaScotsPineView(int EcologicalYear, SiteMoistureView Site, bool HasScotsPineState,
    IReadOnlyList<string> PineTreeIds, IReadOnlyList<string> TightEarlyStockingTreeIds, IReadOnlyList<PlantedJuvenile> PlantedPine,
    IReadOnlyList<ScotsPineRecruit> Recruits, IReadOnlyList<ScotsPineYearAccount> Accounts, bool NextYearIsGoodSeedYear,
    string Evidence);

/// <summary>One tree's site context and, for a Scots pine, its growth, pruning and timber read-out (inspector).</summary>
internal sealed record ScotsPineTreeView(string TreeId, string AreaId, string SpeciesId, string MoistureClass, string MoistureLabel,
    double EstablishmentModifier, double GrowthModifier, bool ModifierIsNeutral, bool HabitatCaution,
    bool IsScotsPine, bool? WithinGrowthDomain, double? PotentialDbhIncrementCm, double? SiteAdjustedDbhIncrementCm,
    string? GrowthNote, int? PruningLifts, double? PrunedHeightM, double? NextLiftTargetM, bool? TightEarlyStocking,
    string? TimberSortment, double? TimberPriceEurPerTonne, double? TimberVolumeM3, double? TimberGreenTonnes, double? TimberValueEur,
    string? TimberReason, string Evidence);

/// <summary>Builds the Scots pine / site-moisture views (pure; the UI renders these strings and numbers and computes nothing).</summary>
internal static class ScotsPineReadouts
{
    private static readonly string[] LiveSpecies =
    {
        Phase2Species.SitkaId, Phase2Species.OakId, Phase2Species.BeechId, ScotsPineLive.SpeciesId,
    };

    public static SiteMoistureView Site(SiteMoistureField field)
    {
        var setting = field.Setting;
        var zones = new List<SiteMoistureZoneView>();
        var b = field.Bounds;
        foreach (var zone in field.ZoneAreas())
        {
            double minX = b.MinX, maxX = b.MaxX;
            if (setting.Mode == SiteMoistureMode.DryRidgeMoistSlope)
            {
                double mid = SiteMoistureEngine.CreateDryRidgeAndMoistSlope(b).MidpointX;
                if (zone.Moisture == SiteMoistureClass.DryFreeDraining) maxX = mid; else minX = mid;
            }
            zones.Add(new SiteMoistureZoneView(zone.Moisture.ToString(), SiteMoistureLabels.ClassLabel(zone.Moisture), minX, maxX,
                b.MinY, b.MaxY, (double)zone.AreaM2, SpeciesMoistureTable.HabitatProtectionCaution(zone.Moisture)));
        }
        var modifiers = new List<SiteMoistureModifierView>();
        foreach (var zone in field.ZoneAreas())
            foreach (var species in LiveSpecies)
            {
                var cell = SpeciesMoistureTable.Cell(SiteMoistureLabels.TableSpeciesId(species), zone.Moisture);
                modifiers.Add(new SiteMoistureModifierView(species, zone.Moisture.ToString(), cell.Response.EstablishmentMultiplier,
                    cell.Response.GrowthMultiplier, cell.EstablishmentTier.ToString(), cell.GrowthTier.ToString()));
            }
        bool caution = SiteMoistureLabels.HabitatCaution(setting);
        return new SiteMoistureView(SiteMoistureSetting.ModeWire(setting.Mode), setting.WholeClass.ToString(),
            SiteMoistureLabels.WholeStandLabel(setting), setting.IsNeutral, caution, caution ? SiteMoistureLabels.HabitatCautionText : null,
            SiteMoistureEngine.SelectDominant(field.ZoneAreas()).Moisture.ToString(), zones.AsReadOnly(), modifiers.AsReadOnly(),
            SiteMoistureLabels.Evidence);
    }

    public static MultiAreaScotsPineView Build(MultiAreaRoot root, SiteMoistureField field, ulong woodlandSeed)
    {
        var phase2 = root.Phase2;
        var pine = root.ScotsPine;
        return new MultiAreaScotsPineView(root.EcologicalYear, Site(field), pine is not null,
            Array.AsReadOnly((phase2?.TreeSpecies ?? new Dictionary<string, string>()).Where(kv => ScotsPineLive.IsPine(kv.Value))
                .Select(kv => kv.Key).Order(StringComparer.Ordinal).ToArray()),
            pine?.TightEarlyStockingTreeIds ?? Array.Empty<string>(),
            Array.AsReadOnly((phase2?.Juveniles ?? Array.Empty<PlantedJuvenile>()).Where(j => ScotsPineLive.IsPine(j.SpeciesId)).ToArray()),
            pine?.Recruits ?? Array.Empty<ScotsPineRecruit>(),
            pine?.Accounts ?? Array.Empty<ScotsPineYearAccount>(),
            ScotsPineEngine.IsGoodSeedYear(woodlandSeed, root.EcologicalYear),
            ScotsPineLive.Evidence);
    }

    public static ScotsPineTreeView Tree(MultiAreaRoot root, SiteMoistureField field, string areaId, AnnualTree tree, float areaHa)
    {
        string species = root.Phase2?.SpeciesIdOf(tree.TreeId) ?? Phase2Species.SitkaId;
        var moisture = field.At(tree.XM, tree.ZM);
        var cell = SpeciesMoistureTable.Cell(SiteMoistureLabels.TableSpeciesId(species), moisture);
        bool isPine = ScotsPineLive.IsPine(species);
        bool? inDomain = null, tight = null;
        double? potential = null, adjusted = null, next = null, pruned = null, price = null, volume = null, tonnes = null, value = null;
        int? lifts = null;
        string? note = null, sortment = null, reason = null;
        if (isPine)
        {
            var area = root.Areas.First(a => a.AreaId == areaId);
            var living = area.State.Trees.Where(t => t.IsLiving).ToArray();
            var stand = WorkRules.Summarize(area.State.Trees, areaHa);
            double top = living.Length == 0 ? 0.0 : living.Max(t => (double)t.HeightM);
            // The same start-of-year rack request the next annual step will make for this stem.
            var growth = ScotsPineAdult.Evaluate(tree, stand, top, field);
            inDomain = growth.IsWithinCalibrationDomain;
            potential = growth.PotentialDbhIncrementCm;
            adjusted = growth.SiteAdjustedDbhIncrementCm;
            note = inDomain == true
                ? "Inside the Irish GROWFOR domain (age 10-60, 100-5,000 stems/ha, 10-100 m2/ha, top height >= 5 m)."
                : "Outside the Irish GROWFOR domain: no DBH growth is modelled this year (age 10-60, 100-5,000 stems/ha, 10-100 m2/ha, top height >= 5 m).";
            var pruning = root.Stage4?.PruningOf(tree.TreeId) ?? TreePruningState.Unpruned;
            lifts = pruning.Lifts;
            pruned = pruning.Lifts > 0 ? pruning.CrownBaseHeightM : 0.0;
            float nextLift = ScotsPinePruning.NextLiftTargetM(pruning.Lifts);
            next = float.IsNaN(nextLift) ? null : nextLift;
            tight = root.ScotsPine?.IsTight(tree.TreeId) ?? false;
            float stemVolM3 = FallenDeadwoodRecord.StemVolumeM3(tree.DbhCm, tree.HeightM);
            decimal vol = stemVolM3 <= 0f ? 0m : decimal.Floor((decimal)stemVolM3 * 1_000_000m) / 1_000_000m;
            var grade = ScotsPineGrading.Grade(tree, vol, pruning, tight.Value);
            sortment = grade.Sortment.ToString();
            price = (double)grade.UnitPriceEurPerTonne;
            volume = (double)grade.VolumeM3;
            tonnes = (double)grade.GreenTonnes;
            value = (double)grade.GrossValueEur;
            reason = grade.Reason;
        }
        return new ScotsPineTreeView(tree.TreeId, areaId, species, moisture.ToString(), SiteMoistureLabels.ClassLabel(moisture),
            cell.Response.EstablishmentMultiplier, cell.Response.GrowthMultiplier, moisture == SiteMoistureClass.NeutralLegacy,
            SpeciesMoistureTable.HabitatProtectionCaution(moisture), isPine, inDomain, potential, adjusted, note, lifts, pruned, next, tight,
            sortment, price, volume, tonnes, value, reason, isPine ? ScotsPineLive.Evidence : SiteMoistureLabels.Evidence);
    }
}

internal sealed partial class MultiAreaContinuation
{
    /// <summary>The Scots pine / site-moisture read-out of the committed root under one lock. Changes nothing.</summary>
    internal MultiAreaScotsPineView ReadScotsPineView()
    {
        lock (sync)
            return ScotsPineReadouts.Build(root, SiteFieldOf(root) ?? new SiteMoistureField(SiteMoistureSetting.Neutral, siteBounds), woodlandSeed);
    }

    /// <summary>Read-only site and Scots pine detail of one exact tree (inspector). A stale fence refuses; nothing changes.</summary>
    internal ScotsPineTreeView ReadScotsPineTree(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            RequireFence(fence);
            int index = AreaIndex(areaId);
            var tree = FindTree(index, treeId ?? string.Empty);
            if (tree is null)
            {
                if (!string.IsNullOrEmpty(treeId) && OwnerOf(treeId) is string owner)
                    throw new P1Refusal("MultiAreaWrongArea", "tree " + treeId + " belongs to " + owner + ", not " + areaId);
                throw new P1Refusal("UnknownTreeId", "no tree of this area has that exact ID");
            }
            var field = SiteFieldOf(root) ?? new SiteMoistureField(SiteMoistureSetting.Neutral, siteBounds);
            return ScotsPineReadouts.Tree(root, field, areaId, tree, areas[index].AreaHa);
        }
    }
}
