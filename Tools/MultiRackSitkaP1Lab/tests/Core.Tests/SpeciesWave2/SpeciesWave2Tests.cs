using System.Runtime.CompilerServices;
using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.Tests.SpeciesWave2;

/// <summary>
/// SPEC-multi-2d-species-wave2-profiles-V1 author tests (W2-01..W2-09). Program.cs is outside this lane's file fence, so
/// the cases register themselves through a module initializer, which runs before Program's top-level statements and
/// therefore before T.RunAll(). These are the author's feature tests, not independent qualification.
/// </summary>
internal static class SpeciesWave2Tests
{
    private const SuitabilityTier G = SuitabilityTier.Good;
    private const SuitabilityTier M = SuitabilityTier.Moderate;
    private const SuitabilityTier L = SuitabilityTier.Limited;
    private const SuitabilityTier X = SuitabilityTier.VeryPoor;

    private static readonly SiteMoistureClass[] Authored =
    {
        SiteMoistureClass.DryFreeDraining, SiteMoistureClass.FreshMoistMineral, SiteMoistureClass.SeasonallyWetAerated,
        SiteMoistureClass.WetMovingAlluvial, SiteMoistureClass.StagnantPoorBog,
    };

    // Independent restatement of the task-packet table, so a typo in the implementation table cannot pass silently.
    private static readonly (string Id, (SuitabilityTier E, SuitabilityTier Gr)[] Row)[] Expected =
    {
        ("sitka-spruce", new[] { (M, L), (G, G), (G, G), (M, M), (L, L) }),
        ("sessile-oak", new[] { (M, M), (G, G), (M, M), (L, L), (X, X) }),
        ("european-beech", new[] { (M, M), (G, G), (L, L), (X, X), (X, X) }),
        ("scots-pine", new[] { (G, G), (G, G), (M, M), (M, L), (L, L) }),
        ("downy-birch", new[] { (M, M), (G, G), (G, G), (G, M), (M, L) }),
        ("common-alder", new[] { (L, L), (G, G), (G, G), (G, G), (L, X) }),
    };

    private static double Tier(SuitabilityTier t) => t switch { G => 1.00, M => 0.70, L => 0.35, _ => 0.10 };

    private static readonly (string Name, SpeciesProfileCard Card, double[] Anchors)[] CrownAnchors =
    {
        ("pine", FutureSpeciesCatalog.ScotsPine, new[] { 1.47, 2.48, 3.39, 4.54 }),
        ("birch", FutureSpeciesCatalog.DownyBirch, new[] { 2.06, 3.22, 4.18, 5.43 }),
        ("alder", FutureSpeciesCatalog.CommonAlder, new[] { 2.44, 3.52, 4.36, 5.40 }),
    };

    [ModuleInitializer]
    internal static void Register()
    {
        T.Add("species-wave2 W2-01: all 30 species x moisture cells equal the exact tier pairs and NeutralLegacy is (1.0, 1.0)", () =>
        {
            T.Eq(6, SpeciesMoistureTable.SpeciesIds.Count, "species id count");
            int checkedCells = 0;
            foreach (var (id, row) in Expected)
            {
                T.True(SpeciesMoistureTable.SpeciesIds.Contains(id), $"species id {id} exposed");
                for (int i = 0; i < Authored.Length; i++)
                {
                    var cell = SpeciesMoistureTable.Cell(id, Authored[i]);
                    T.Eq(row[i].E, cell.EstablishmentTier, $"{id}/{Authored[i]} establishment tier");
                    T.Eq(row[i].Gr, cell.GrowthTier, $"{id}/{Authored[i]} growth tier");
                    var response = SpeciesMoistureTable.Response(id, Authored[i]);
                    T.Eq(new SiteResponse(Tier(row[i].E), Tier(row[i].Gr)), response, $"{id}/{Authored[i]} response");
                    T.Eq(EvidenceClass.Calibration, cell.Evidence.Primary, $"{id}/{Authored[i]} evidence");
                    checkedCells++;
                }
                var neutral = SpeciesMoistureTable.Response(id, SiteMoistureClass.NeutralLegacy);
                T.True(neutral.EstablishmentMultiplier == 1.0 && neutral.GrowthMultiplier == 1.0, $"{id} NeutralLegacy is exactly (1.0, 1.0)");
            }
            T.Eq(30, checkedCells, "authored cell count");
            T.Eq(1.00, SpeciesMoistureTable.TierMultiplier(G), "G");
            T.Eq(0.70, SpeciesMoistureTable.TierMultiplier(M), "M");
            T.Eq(0.35, SpeciesMoistureTable.TierMultiplier(L), "L");
            T.Eq(0.10, SpeciesMoistureTable.TierMultiplier(X), "X");
            T.Eq(EvidenceClass.Calibration, SpeciesMoistureTable.TierEvidence.Primary, "tier defaults are Calibration");
            bool refused = false;
            try { SpeciesMoistureTable.Response("not-a-species", SiteMoistureClass.FreshMoistMineral); }
            catch (ArgumentException) { refused = true; }
            T.True(refused, "unknown species id is rejected");
        });

        T.Add("species-wave2 W2-02: HabitatProtectionCaution is true only on StagnantPoorBog", () =>
        {
            foreach (var moisture in SpeciesMoistureTable.MoistureClasses)
                T.Eq(moisture == SiteMoistureClass.StagnantPoorBog, SpeciesMoistureTable.HabitatProtectionCaution(moisture), $"caution on {moisture}");
            T.Eq(6, SpeciesMoistureTable.MoistureClasses.Count, "moisture class count");
        });

        T.Add("species-wave2 W2-03: crown radius anchors at D=10/20/30/45 cm are within 0.02 m for pine, birch and alder", () =>
        {
            double[] dbh = { 10, 20, 30, 45 };
            foreach (var (name, card, anchors) in CrownAnchors)
                for (int i = 0; i < dbh.Length; i++)
                {
                    double r = card.Crown.ComputePotentialCrownRadiusM(dbh[i]);
                    T.True(Math.Abs(r - anchors[i]) <= 0.02, $"{name} D={dbh[i]}: expected {anchors[i]} +/- 0.02, got {r:R}");
                }
        });

        T.Add("species-wave2 W2-04: Scots pine radius is clamped at D=58.6 cm and non-decreasing above it", () =>
        {
            var crown = FutureSpeciesCatalog.ScotsPine.Crown;
            double atClamp = crown.ComputePotentialCrownRadiusM(58.6);
            foreach (double d in new[] { 70.0, 100.0, 200.0 })
                T.True(Math.Abs(crown.ComputePotentialCrownRadiusM(d) - atClamp) <= 1e-9, $"pine D={d} equals the clamp radius");
            double previous = crown.ComputePotentialCrownRadiusM(50.0);
            for (double d = 50.0; d <= 250.0; d += 0.5)
            {
                double r = crown.ComputePotentialCrownRadiusM(d);
                T.True(r >= previous, $"pine non-decreasing at D={d}");
                previous = r;
            }
        });

        T.Add("species-wave2 W2-05: juvenile radii at D=0.5/1/2/3 cm are finite, positive and non-decreasing", () =>
        {
            double[] dbh = { 0.5, 1.0, 2.0, 3.0 };
            foreach (var (name, card, _) in CrownAnchors)
            {
                double previous = 0.0;
                foreach (double d in dbh)
                {
                    double r = card.Crown.ComputePotentialCrownRadiusM(d);
                    T.True(double.IsFinite(r) && r > 0.0, $"{name} D={d} finite and positive, got {r:R}");
                    T.True(r >= previous, $"{name} D={d} non-decreasing");
                    T.True(r >= CrownProfile.MinimumRadiusM, $"{name} D={d} respects the 0.10 m floor");
                    previous = r;
                }
            }
        });

        T.Add("species-wave2 W2-06: target radius equals potential at ci=0 and potential/sqrt(2) at ci=ci50", () =>
        {
            foreach (var (name, card, _) in CrownAnchors)
                foreach (double d in new[] { 0.5, 10.0, 30.0, 45.0, 80.0 })
                {
                    var crown = card.Crown;
                    double pot = crown.ComputePotentialCrownRadiusM(d);
                    T.True(Math.Abs(crown.ComputeTargetCrownRadiusM(d, 0.0) - pot) <= 1e-12, $"{name} D={d} ci=0");
                    T.True(Math.Abs(crown.ComputeTargetCrownRadiusM(d, crown.Ci50Default) - pot / Math.Sqrt(2.0)) <= 1e-12, $"{name} D={d} ci=ci50");
                }
        });

        T.Add("species-wave2 W2-07: colonisation is AuthoredOnly, premium buyer on birch/alder only, water dispersal on alder only", () =>
        {
            foreach (var card in FutureSpeciesCatalog.All)
            {
                T.Eq(ExternalColonisation.AuthoredOnly, card.Colonisation.ExternalColonisation, $"{card.SpeciesId} external colonisation");
                T.True(card.Colonisation.LocalOnMapSeedDispersalAllowed, $"{card.SpeciesId} local on-map seed dispersal");
            }
            T.Eq(false, FutureSpeciesCatalog.ScotsPine.Timber.PremiumBuyerRequired, "pine premium buyer");
            T.Eq(true, FutureSpeciesCatalog.DownyBirch.Timber.PremiumBuyerRequired, "birch premium buyer");
            T.Eq(true, FutureSpeciesCatalog.CommonAlder.Timber.PremiumBuyerRequired, "alder premium buyer");
            T.Eq(false, FutureSpeciesCatalog.ScotsPine.Colonisation.WaterAssistedDispersalEligible, "pine water dispersal");
            T.Eq(false, FutureSpeciesCatalog.DownyBirch.Colonisation.WaterAssistedDispersalEligible, "birch water dispersal");
            T.Eq(true, FutureSpeciesCatalog.CommonAlder.Colonisation.WaterAssistedDispersalEligible, "alder water dispersal");
            T.True(FutureSpeciesCatalog.CommonAlder.Colonisation.HardWindDispersalCeilingM is null, "alder has no hard wind ceiling");
            T.True(FutureSpeciesCatalog.CommonAlder.Silviculture.ThinningMoreImportantThanPruning, "alder thinning > pruning");
            T.True(FutureSpeciesCatalog.CommonAlder.Silviculture.SelfPruning, "alder self-pruning");
        });

        T.Add("species-wave2 W2-08: no silent Sitka reuse — Ci50 not 5.0, crown evidence as specified, sortment bands non-empty", () =>
        {
            foreach (var card in FutureSpeciesCatalog.All)
            {
                T.True(card.Crown.Ci50Default != 5.0, $"{card.SpeciesId} Ci50 default is not the Sitka 5.0");
                T.True(card.Crown.Ci50Band.Contains(card.Crown.Ci50Default), $"{card.SpeciesId} Ci50 default inside its band");
                T.Eq(EvidenceClass.Calibration, card.Crown.Ci50Evidence.Primary, $"{card.SpeciesId} Ci50 evidence");
                T.True(card.Timber.SortmentBands.Count > 0, $"{card.SpeciesId} sortment bands non-empty");
                foreach (var band in card.Timber.SortmentBands)
                    T.True(band.EurPerTonne.Low > 0 && band.EurPerTonne.High > band.EurPerTonne.Low, $"{card.SpeciesId} {band.Sortment} band ordered");
            }
            T.Eq(4.75, FutureSpeciesCatalog.ScotsPine.Crown.Ci50Default, "pine Ci50");
            T.Eq(new ValueBand(4.0, 5.5), FutureSpeciesCatalog.ScotsPine.Crown.Ci50Band, "pine Ci50 band");
            T.Eq(4.25, FutureSpeciesCatalog.DownyBirch.Crown.Ci50Default, "birch Ci50");
            T.Eq(new ValueBand(3.5, 5.0), FutureSpeciesCatalog.DownyBirch.Crown.Ci50Band, "birch Ci50 band");
            T.Eq(4.75, FutureSpeciesCatalog.CommonAlder.Crown.Ci50Default, "alder Ci50");
            T.Eq(new ValueBand(4.0, 5.5), FutureSpeciesCatalog.CommonAlder.Crown.Ci50Band, "alder Ci50 band");

            T.Eq(EvidenceTag.Of(EvidenceClass.Transfer), FutureSpeciesCatalog.ScotsPine.Crown.PotentialRadiusEvidence, "pine crown evidence");
            T.Eq(EvidenceTag.Of(EvidenceClass.Inference, EvidenceClass.Calibration), FutureSpeciesCatalog.DownyBirch.Crown.PotentialRadiusEvidence, "birch crown evidence");
            T.Eq(EvidenceTag.Of(EvidenceClass.Empirical, EvidenceClass.Inference), FutureSpeciesCatalog.CommonAlder.Crown.PotentialRadiusEvidence, "alder crown evidence");

            T.Eq(ShadeTolerance.LightModerate, FutureSpeciesCatalog.ScotsPine.Crown.ShadeTolerance, "pine shade");
            T.Eq(ShadeTolerance.Light, FutureSpeciesCatalog.DownyBirch.Crown.ShadeTolerance, "birch shade");
            T.Eq(ShadeTolerance.Moderate, FutureSpeciesCatalog.CommonAlder.Crown.ShadeTolerance, "alder shade");

            T.Eq(1.02, FutureSpeciesCatalog.ScotsPine.Timber.GreenDensityTPerM3, "pine density");
            T.Eq(0.93, FutureSpeciesCatalog.DownyBirch.Timber.GreenDensityTPerM3, "birch density");
            T.Eq(0.842, FutureSpeciesCatalog.CommonAlder.Timber.GreenDensityTPerM3, "alder density");
            T.Eq(EvidenceTag.Of(EvidenceClass.Transfer), FutureSpeciesCatalog.CommonAlder.Timber.GreenDensityEvidence, "alder density evidence");
            T.Eq(3, FutureSpeciesCatalog.ScotsPine.Timber.SortmentBands.Count, "pine sortment count");
        });

        T.Add("species-wave2 W2-09: readiness records carry the GROWFOR pine domain, birch YC8 pioneer trajectory and alder short-lived trajectory", () =>
        {
            var pine = FutureSpeciesCatalog.ScotsPine.Readiness;
            T.Eq(ReadinessStatus.ReadyForInitialDesign, pine.Status, "pine status");
            T.Eq<ValueBand?>(new ValueBand(10, 60), pine.AgeRangeYears, "pine GROWFOR age range");
            T.Eq<ValueBand?>(new ValueBand(100, 5000), pine.StemsPerHectare, "pine GROWFOR stems/ha");
            T.Eq<ValueBand?>(new ValueBand(10, 100), pine.BasalAreaM2PerHa, "pine GROWFOR basal area");
            T.Eq<double?>(5.0, pine.MinimumTopHeightM, "pine GROWFOR minimum top height");
            T.True(pine.IrishYieldClassPotential is null, "pine has no YC on its readiness record");
            T.True(pine.Notes.Contains("GROWFOR"), "pine readiness note cites GROWFOR");

            var birch = FutureSpeciesCatalog.DownyBirch.Readiness;
            T.Eq(ReadinessStatus.PartialWithAbstractions, birch.Status, "birch status");
            T.Eq<int?>(8, birch.IrishYieldClassPotential, "birch Irish YC potential");
            T.True(birch.Trajectory.Contains("pioneer", StringComparison.OrdinalIgnoreCase)
                && birch.Trajectory.Contains("competition", StringComparison.OrdinalIgnoreCase), "birch pioneer -> competition trajectory");
            T.True(birch.AgeRangeYears is null && birch.StemsPerHectare is null && birch.BasalAreaM2PerHa is null
                && birch.MinimumTopHeightM is null, "birch numeric domains are null");

            var alder = FutureSpeciesCatalog.CommonAlder.Readiness;
            T.Eq(ReadinessStatus.PartialWithAbstractions, alder.Status, "alder status");
            T.True(alder.Trajectory.Contains("pioneer", StringComparison.OrdinalIgnoreCase)
                && alder.Trajectory.Contains("short-lived", StringComparison.OrdinalIgnoreCase), "alder pioneer / short-lived trajectory");
            T.True(alder.IrishYieldClassPotential is null && alder.AgeRangeYears is null && alder.StemsPerHectare is null
                && alder.BasalAreaM2PerHa is null && alder.MinimumTopHeightM is null, "alder numeric domains and YC are null");

            foreach (var card in FutureSpeciesCatalog.All)
                T.Eq(ExternalColonisation.AuthoredOnly, card.Colonisation.ExternalColonisation, $"{card.SpeciesId} still AuthoredOnly");
        });
    }
}
