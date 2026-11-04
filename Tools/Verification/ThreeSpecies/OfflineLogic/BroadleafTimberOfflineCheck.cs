// Offline check for the Oak/Beech timber ladders and prices (D-062 and its approved supplement).
// Compiles the PRODUCTION Assets/ForestPrototype/TimberYield and WorkEconomy sources unchanged (none uses
// UnityEngine) with this file and SitkaYieldFixture.cs, on the plain .NET SDK. It executes the real allocator,
// price book and work calculator. It does not exercise the Unity-side adapter wiring (see
// BroadleafTimberEconomyVerification.cs, type-checked only).
//
// Usage: run_offline_check.py --suite timber   Prints THREE_SPECIES_TIMBER_* tokens.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CCF.Forestry.TimberYield;
using CCF.Forestry.WorkEconomy;

public static class BroadleafTimberOfflineCheck
{
    // Captured by running SitkaYieldFixture against the UNMODIFIED sources of base commit c1564b8
    // (TimberYield + WorkEconomy at that commit, CreateSitka configuration).
    private const string BaseSitkaSingleStemFingerprint = "CAA0388524E0D0E5";
    private const string BaseSitkaOperationFingerprint = "2976F3575E0446B3";
    private const long BaseSitkaOperationSaleCm3 = 169245486, BaseSitkaOperationResidualCm3 = 130542728;
    private const int BaseSitkaOperationAssortments = 12;

    private static int failures, checks;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private const string Oak = "sessile-oak", Beech = "beech", Sitka = "sitka-spruce";

    public static int Main()
    {
        TimberYieldConfiguration config = TimberYieldDefaults.CreateScenarioOne();
        ConfigurationAndPrices(config);
        SitkaUnchanged();
        OakLadder(config);
        BeechLadder(config);
        QualityAndPruning(config);
        MidLogRule(config);
        Valuation(config);
        Console.WriteLine(failures == 0 ? $"THREE_SPECIES_TIMBER_PASS checks={checks}" : $"THREE_SPECIES_TIMBER_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    // ---------- fixtures ----------

    // Same volume basis as ForestTree.BiologicalStemVolumeM3 (DBH^2 x 0.00007854 x height x form ratio 0.5).
    private static StemMeasurements Stem(string id, string species, double dbhCm, double heightM, StemQualityFlags quality = StemQualityFlags.None,
        params StemQualitySection[] sections)
    {
        double volume = dbhCm * dbhCm * 0.00007854 * heightM * 0.5;
        StemMeasurements stem = MerchantableStemModel.FromMetres(id, species, dbhCm, heightM, volume, "[S] fixture");
        stem.StemVolumeCm3 = (long)Math.Floor(volume * 1000000d);
        stem.Quality = quality;
        stem.QualitySections = sections ?? Array.Empty<StemQualitySection>();
        return stem;
    }

    // The section the Scenario One adapter derives from a tree's pruning history: stem base to the crown base.
    private static StemQualitySection Pruned(double crownBaseM, double heightM)
        => new StemQualitySection { StartHeightMm = 0, EndHeightMm = (int)Math.Min(Math.Round(crownBaseM * 1000), Math.Round(heightM * 1000) - 1), Flags = StemQualityFlags.Pruned };

    private static StemYieldResult Resolve(StemMeasurements stem, TimberYieldConfiguration config,
        TimberDisposition disposition = TimberDisposition.SellAndExtract)
        => TimberYieldCalculator.ResolveSingleStem(new StemYieldRequest { Stem = stem, DefaultDisposition = disposition, ExtractRetainedToRoadside = true }, config);

    private static long Volume(StemYieldResult r, TimberAssortment a) => r.Logs.Where(l => l.Assortment == a).Sum(l => l.VolumeCm3);
    private static int Count(StemYieldResult r, TimberAssortment a) => r.Logs.Count(l => l.Assortment == a);
    private static string Mix(StemYieldResult r) => string.Join(" ", r.Logs.GroupBy(l => l.Assortment).OrderBy(g => g.Key)
        .Select(g => $"{g.Key}={g.Count()}x/{g.Sum(l => l.VolumeCm3) / 1e6:0.000}m3"));
    private static bool AnyOf(StemYieldResult r, params TimberAssortment[] a) => r.Logs.Any(l => a.Contains(l.Assortment));

    // ---------- configuration and prices ----------

    private static void ConfigurationAndPrices(TimberYieldConfiguration config)
    {
        bool valid = true;
        try { TimberYieldValidation.ValidateConfiguration(config); } catch (Exception e) { valid = false; Console.WriteLine(e.Message); }
        Check(valid, "SCENARIO_YIELD_CONFIGURATION_VALIDATES", $"specifications={config.Specifications.Length}");

        var oak = config.Specifications.Where(s => s.SpeciesId == Oak).OrderBy(s => s.Priority).ToArray();
        var beech = config.Specifications.Where(s => s.SpeciesId == Beech).OrderBy(s => s.Priority).ToArray();
        Check(oak.Select(s => s.Assortment).SequenceEqual(new[] { TimberAssortment.VeneerPlanking, TimberAssortment.Beam, TimberAssortment.Fencing, TimberAssortment.SecondPlanking, TimberAssortment.Firewood }),
            "OAK_LADDER_ORDER", string.Join(" > ", oak.Select(s => s.Assortment)));
        Check(beech.Select(s => s.Assortment).SequenceEqual(new[] { TimberAssortment.SecondPlanking, TimberAssortment.Firewood }),
            "BEECH_LADDER_IS_SHORT_AND_DOES_NOT_INHERIT_OAK", string.Join(" > ", beech.Select(s => s.Assortment)));
        Check(!beech.Any(s => s.Assortment == TimberAssortment.VeneerPlanking || s.Assortment == TimberAssortment.Beam || s.Assortment == TimberAssortment.Fencing),
            "BEECH_HAS_NO_VENEER_BEAM_OR_FENCING_CLASS");
        Check(config.Specifications.Select(s => s.Id).Distinct().Count() == config.Specifications.Length, "SPECIFICATION_IDS_UNIQUE");
        Check(config.Specifications.All(s => s.SpeciesId == Sitka || s.SpeciesId == Oak || s.SpeciesId == Beech), "ONLY_THE_THREE_SPECIES_HAVE_SPECIFICATIONS");

        AssortmentSpecification veneer = oak[0], beam = oak[1];
        Check(veneer.NominalLengthsMm.All(l => l > 2700) && veneer.MinimumMidDiameterMm == 450 && veneer.RequiredQuality == StemQualityFlags.Pruned,
            "OAK_VENEER_RULE_AS_APPROVED", $"lengths={string.Join(",", veneer.NominalLengthsMm)} mid>={veneer.MinimumMidDiameterMm} required={veneer.RequiredQuality}");
        Check(beam.NominalLengthsMm.All(l => l > 2500) && beam.MinimumMidDiameterMm == 300, "OAK_BEAM_RULE_AS_APPROVED", $"lengths={string.Join(",", beam.NominalLengthsMm)} mid>={beam.MinimumMidDiameterMm}");
        Check(config.Specifications.Where(s => s.SpeciesId == Sitka).All(s => s.MinimumMidDiameterMm == 0), "SITKA_SPECIFICATIONS_DO_NOT_USE_MID_LOG_RULE");

        TimberPrice[] prices = Stage1EconomyDefaults.BroadleafTimberPrices();
        Check(prices.Length == 7 && prices.Select(p => p.Id).Distinct().Count() == 7, "SEVEN_BROADLEAF_PRICES_ONE_PER_SPECIES_AND_PRODUCT");
        Check(prices.All(p => p.SpeciesId == Oak || p.SpeciesId == Beech), "NO_SITKA_PRICE_IN_BROADLEAF_TABLE");
        // Every ladder rung has exactly one explicit price of the right species and product.
        bool matched = config.Specifications.Where(s => s.SpeciesId != Sitka).All(s => prices.Count(p => p.SpeciesId == s.SpeciesId && p.Assortment == s.Assortment) == 1);
        Check(matched, "EVERY_LADDER_RUNG_HAS_AN_EXPLICIT_PRICE");
        long P(string id) => prices.First(p => p.Id == id).RoadsideCentsPerTonne.Selected;
        Check(P("oak-veneer-planking") > P("oak-beam") && P("oak-beam") > P("oak-fencing") && P("oak-fencing") > P("oak-second-planking") && P("oak-second-planking") > P("oak-firewood"),
            "OAK_PRICES_FOLLOW_THE_LADDER", string.Join(" ", prices.Where(p => p.SpeciesId == Oak).OrderByDescending(p => p.RoadsideCentsPerTonne.Selected).Select(p => $"{p.Id}={p.RoadsideCentsPerTonne.Selected / 100}")));
        Check(P("beech-second-planking") == P("oak-second-planking") && P("beech-firewood") == P("oak-firewood"), "BEECH_USES_ITS_OWN_ENTRIES_WITH_THE_SAME_EVIDENCE_VALUES");
        Check(prices.All(p => p.RoadsideCentsPerTonne.Low <= p.RoadsideCentsPerTonne.Reference && p.RoadsideCentsPerTonne.Reference <= p.RoadsideCentsPerTonne.High
            && p.RoadsideCentsPerTonne.Selected == p.RoadsideCentsPerTonne.Reference && p.RoadsideCentsPerTonne.Unit == ParameterUnit.CentsPerTonne
            && p.RoadsideCentsPerTonne.Source.Length > 40), "PRICE_PARAMETERS_USE_LOW_REFERENCE_HIGH_SELECTED_WITH_SOURCES");
        Check(prices.Where(p => p.Assortment == TimberAssortment.Firewood).All(p => p.RoadsideCentsPerTonne.RangeEvidence == EvidenceLabel.GameplayCalibration
            && p.RoadsideCentsPerTonne.Source.Contains("NOT supported Irish stumpage evidence")), "FIREWOOD_PRICE_IS_LABELLED_CALIBRATION");
        Check(prices.Where(p => p.Assortment != TimberAssortment.Firewood).All(p => p.RoadsideCentsPerTonne.RangeEvidence == EvidenceLabel.ManagementGuidance), "TEAGASC_TYPICAL_QUOTES_ARE_MANAGEMENT_GUIDANCE");
        Check(prices.All(p => p.RoadsideCentsPerTonne.SelectedEvidence == EvidenceLabel.GameplayCalibration
            && p.RoadsideCentsPerTonne.Source.Contains("Selected reference and roadside basis: GAMEPLAY CALIBRATION")), "SELECTED_REFERENCES_AND_ROADSIDE_BASIS_ARE_CALIBRATION");
        Check(veneer.DimensionEvidence == EvidenceLabel.ManagementGuidance && beam.DimensionEvidence == EvidenceLabel.ManagementGuidance
            && oak.Concat(beech).All(s => s.SelectionEvidence == EvidenceLabel.GameplayCalibration), "DIMENSION_GUIDANCE_IS_DISTINCT_FROM_SELECTED_GRADING_PROXY");
        // Conversion convention: per cubic metre at the work-equivalent proxy, rounded to whole euro per tonne.
        Check(Stage1EconomyDefaults.FreshVolumeProxyKgPerCubicMetre == Stage1EconomyDefaults.Create().Densities.Single().KilogramsPerCubicMetre.Reference, "PROXY_DENSITY_EQUALS_THE_EXISTING_WORK_EQUIVALENT_DENSITY");
        GreenTimberDensity[] densities = Stage1EconomyDefaults.BroadleafDensities();
        Check(densities.Length == 2 && densities.All(d => d.KilogramsPerCubicMetre.Selected == Stage1EconomyDefaults.FreshVolumeProxyKgPerCubicMetre
            && d.KilogramsPerCubicMetre.SelectedEvidence == EvidenceLabel.GameplayCalibration && d.MoistureCondition.Contains("not a measured green density")
            && d.SpeciesId != Sitka), "BROADLEAF_DENSITY_ENTRIES_ARE_LABELLED_WORK_PROXIES_NOT_GREEN_DENSITIES");
        double veneerPerM3 = P("oak-veneer-planking") * 0.89 / 100.0;
        Check(Math.Abs(veneerPerM3 - 230.0) < 1.0, "VENEER_REFERENCE_IS_ABOUT_230_EURO_PER_M3", $"{veneerPerM3:0.0}");
        Check(prices.All(p => p.RoadsideCentsPerTonne.Selected % 100 == 0), "NO_FALSE_PRECISION_BELOW_WHOLE_EURO_PER_TONNE");
        // The flat-rate fallback is not part of this layer: no price is shared between two different products.
        Check(prices.Where(p => p.SpeciesId == Oak).Select(p => p.RoadsideCentsPerTonne.Selected).Distinct().Count() == 5, "NO_FLAT_BROADLEAF_RATE");

        // A harvest quote with the broadleaf prices appended must satisfy the engine's own validation.
        ForestryPriceBook book = Book();
        bool booked = true;
        try { ForestryEconomyValidation.Validate(new ForestryTask { TaskId = "t", WorldOperationId = "w", Type = ForestryTaskType.Monitoring, Quantity = new WorkQuantity { Unit = WorkQuantityUnit.SquareMetres, Amount = 0 } },
            WorkExecutionMethod.Contractor, new ExecutionResources { ContractorAvailable = true }, book); } catch (Exception e) { booked = false; Console.WriteLine(e.Message); }
        Check(booked, "PRICE_BOOK_WITH_BROADLEAF_PRICES_PASSES_ENGINE_VALIDATION");
    }

    private static ForestryPriceBook Book()
    {
        ForestryPriceBook book = Stage1EconomyDefaults.Create();
        book.TimberPrices = book.TimberPrices.Concat(Stage1EconomyDefaults.BroadleafTimberPrices()).ToArray();
        book.Densities = book.Densities.Concat(Stage1EconomyDefaults.BroadleafDensities()).ToArray();
        return book;
    }

    // ---------- Sitka unchanged ----------

    private static void SitkaUnchanged()
    {
        List<StemYieldRequest> requests = SitkaYieldFixture.Requests();
        TimberYieldConfiguration sitkaOnly = TimberYieldDefaults.CreateSitka();
        TimberYieldConfiguration scenario = TimberYieldDefaults.CreateScenarioOne();
        List<StemYieldResult> viaSitka = requests.Select(r => TimberYieldCalculator.ResolveSingleStem(r, sitkaOnly)).ToList();
        List<StemYieldResult> viaScenario = requests.Select(r => TimberYieldCalculator.ResolveSingleStem(r, scenario)).ToList();
        string a = SitkaYieldFixture.Fingerprint(viaSitka), b = SitkaYieldFixture.Fingerprint(viaScenario);
        Check(a == BaseSitkaSingleStemFingerprint, "SITKA_ALLOCATION_IDENTICAL_TO_UNMODIFIED_BASE_COMMIT",
            $"stems={requests.Count} logs={viaSitka.Sum(r => r.Logs.Length)} fingerprint={a} base={BaseSitkaSingleStemFingerprint}");
        Check(b == a, "SITKA_ALLOCATION_IDENTICAL_UNDER_THE_SCENARIO_CONFIGURATION", $"fingerprint={b}");
        Check(!viaScenario.Any(r => r.Logs.Any(l => l.Assortment > TimberAssortment.Sawlog)), "SITKA_NEVER_RECEIVES_AN_OAK_OR_BEECH_PRODUCT");

        StandOperationYield op = TimberYieldCalculator.ResolveStandOperation("golden", requests.Take(400).ToArray(), scenario);
        Check(SitkaYieldFixture.Fingerprint(op.Stems) == BaseSitkaOperationFingerprint && op.SaleAssortmentVolumeCm3 == BaseSitkaOperationSaleCm3
            && op.ResidualVolumeCm3 == BaseSitkaOperationResidualCm3 && op.Assortments.Length == BaseSitkaOperationAssortments,
            "SITKA_STAND_OPERATION_IDENTICAL_TO_UNMODIFIED_BASE_COMMIT", $"sale={op.SaleAssortmentVolumeCm3} residual={op.ResidualVolumeCm3} summaries={op.Assortments.Length}");
        Check(TimberAssortment.Pulp == 0 && TimberAssortment.Stake == (TimberAssortment)1 && TimberAssortment.Pallet == (TimberAssortment)2 && TimberAssortment.Sawlog == (TimberAssortment)3,
            "EXISTING_ASSORTMENT_INTEGER_VALUES_UNCHANGED_FOR_SAVES");
        foreach (TimberAssortment legacy in new[] { TimberAssortment.Pulp, TimberAssortment.Stake, TimberAssortment.Pallet, TimberAssortment.Sawlog })
            if (TimberAssortmentLabels.Label(legacy) != legacy.ToString()) Check(false, "SITKA_PRODUCT_LABELS_UNCHANGED", legacy.ToString());
        Check(true, "SITKA_PRODUCT_LABELS_UNCHANGED");
    }

    // ---------- Oak ----------

    private static void OakLadder(TimberYieldConfiguration config)
    {
        // Small oak: pruned and straight, but too small for any premium product. Species alone earns nothing.
        foreach (double dbh in new[] { 12.0, 20.0, 25.0, 29.0 })
        {
            StemYieldResult small = Resolve(Stem("small" + dbh, Oak, dbh, 6 + dbh * 0.4, StemQualityFlags.None, Pruned(2.5, 6 + dbh * 0.4)), config);
            Check(!AnyOf(small, TimberAssortment.VeneerPlanking, TimberAssortment.Beam), $"SMALL_OAK_DBH{dbh:0}_CANNOT_BE_PREMIUM", Mix(small));
        }
        // The lowest rungs are reachable by smaller oak.
        StemYieldResult poles = Resolve(Stem("poles", Oak, 22, 14), config);
        Check(Count(poles, TimberAssortment.Fencing) > 0 && Count(poles, TimberAssortment.Firewood) > 0, "SMALL_OAK_YIELDS_FENCING_AND_FIREWOOD", Mix(poles));

        // Large, high-quality oak (synthetic dimensions; see the Phase 6 record for how rarely the simulation reaches them).
        StemMeasurements bigPruned = Stem("big", Oak, 60, 24, StemQualityFlags.None, Pruned(6.5, 24));
        StemYieldResult big = Resolve(bigPruned, config);
        Check(Count(big, TimberAssortment.VeneerPlanking) >= 2 && Count(big, TimberAssortment.Beam) >= 1, "LARGE_PRUNED_OAK_REACHES_VENEER_AND_BEAM", Mix(big));
        Check(big.Logs.Where(l => l.Assortment == TimberAssortment.VeneerPlanking).All(l => l.EndHeightMm <= 6500), "VENEER_LOGS_LIE_WITHIN_THE_PRUNED_SECTION");
        Check(big.Logs.Where(l => l.Assortment == TimberAssortment.VeneerPlanking).All(l => l.EndHeightMm - l.StartHeightMm > 2700), "VENEER_LOGS_ARE_LONGER_THAN_2_7_M");
        Check(big.Logs.Where(l => l.Assortment == TimberAssortment.Beam).All(l => l.EndHeightMm - l.StartHeightMm > 2500), "BEAM_LOGS_ARE_LONGER_THAN_2_5_M");
        // Volume is conserved exactly (the engine throws otherwise) and every log belongs to the right species.
        Check(big.Logs.All(l => l.SpeciesId == Oak) && big.SaleAssortmentVolumeCm3 + big.ResidualVolumeCm3 == big.StemVolumeCm3, "OAK_VOLUME_CONSERVED_AND_SPECIES_EXPLICIT");

        // Same oak, never pruned: premium class unavailable, beam still reachable.
        StemYieldResult unpruned = Resolve(Stem("bigU", Oak, 60, 24), config);
        Check(Count(unpruned, TimberAssortment.VeneerPlanking) == 0 && Count(unpruned, TimberAssortment.Beam) >= 1, "UNPRUNED_LARGE_OAK_HAS_NO_VENEER_BUT_KEEPS_BEAM", Mix(unpruned));

        // Synthetic allocator robustness ONLY: Scenario One does not supply living-tree fork/defect state.
        // These flags test the generic allocator's exclusions, not a represented forestry/form mechanic.
        StemYieldResult forkedWhole = Resolve(Stem("forkW", Oak, 60, 24, StemQualityFlags.PoorForm, Pruned(6.5, 24)), config);
        Check(!AnyOf(forkedWhole, TimberAssortment.VeneerPlanking, TimberAssortment.Beam) && forkedWhole.Logs.Length > 0, "FORKED_WHOLE_STEM_OAK_CAPPED_BELOW_VENEER_AND_BEAM", Mix(forkedWhole));
        var fork = new StemQualitySection { StartHeightMm = 9000, EndHeightMm = 14000, Flags = StemQualityFlags.PoorForm };
        StemYieldResult forkedHigh = Resolve(Stem("forkH", Oak, 60, 24, StemQualityFlags.None, Pruned(6.5, 24), fork), config);
        Check(Count(forkedHigh, TimberAssortment.VeneerPlanking) == Count(big, TimberAssortment.VeneerPlanking), "FORK_ABOVE_THE_PRUNED_BUTT_DOES_NOT_REMOVE_THE_CLEAN_BUTT_LOGS", Mix(forkedHigh));
        var forkLow = new StemQualitySection { StartHeightMm = 2500, EndHeightMm = 7000, Flags = StemQualityFlags.PoorForm };
        StemYieldResult forkedLow = Resolve(Stem("forkL", Oak, 60, 24, StemQualityFlags.None, Pruned(6.5, 24), forkLow), config);
        Check(Count(forkedLow, TimberAssortment.VeneerPlanking) == 0 && Count(forkedLow, TimberAssortment.Beam) < Count(unpruned, TimberAssortment.Beam), "LOW_FORK_REMOVES_VENEER_AND_REDUCES_BEAM", Mix(forkedLow));
        StemYieldResult stemDefect = Resolve(Stem("defect", Oak, 60, 24, StemQualityFlags.StemDefect, Pruned(6.5, 24)), config);
        Check(!AnyOf(stemDefect, TimberAssortment.VeneerPlanking, TimberAssortment.Beam), "STEM_DEFECT_OAK_CAPPED_BELOW_VENEER_AND_BEAM", Mix(stemDefect));

        // Low quality always has the firewood fallback, and unusable material never reaches a sawn product.
        StemYieldResult unusable = Resolve(Stem("unusable", Oak, 45, 20, StemQualityFlags.Unusable, Pruned(6.5, 20)), config);
        Check(Count(unusable, TimberAssortment.Firewood) > 0 && Volume(unusable, TimberAssortment.Firewood) == unusable.SaleAssortmentVolumeCm3 && unusable.Logs.Length > 0,
            "UNUSABLE_OAK_FALLS_BACK_TO_FIREWOOD_ONLY", Mix(unusable));
        StemYieldResult wind = Resolve(Stem("wind", Oak, 45, 20, StemQualityFlags.WindDamage), config);
        Check(!AnyOf(wind, TimberAssortment.VeneerPlanking, TimberAssortment.Beam, TimberAssortment.Fencing, TimberAssortment.SecondPlanking) && Count(wind, TimberAssortment.Firewood) > 0,
            "WIND_DAMAGED_OAK_IS_FIREWOOD_ONLY", Mix(wind));

        // Disposition is independent of the ladder: kept-for-use and deadwood logs are classed the same way.
        StemYieldResult kept = Resolve(bigPruned, config, TimberDisposition.KeepForUse);
        Check(kept.Logs.Select(l => (l.StartHeightMm, l.EndHeightMm, l.Assortment)).SequenceEqual(big.Logs.Select(l => (l.StartHeightMm, l.EndHeightMm, l.Assortment))) && kept.SaleAssortmentVolumeCm3 == 0,
            "DISPOSITION_DOES_NOT_CHANGE_THE_GRADING");
    }

    // ---------- Beech ----------

    private static void BeechLadder(TimberYieldConfiguration config)
    {
        StemYieldResult big = Resolve(Stem("bBig", Beech, 60, 24, StemQualityFlags.None, Pruned(6.5, 24)), config);
        Check(!AnyOf(big, TimberAssortment.VeneerPlanking, TimberAssortment.Beam, TimberAssortment.Fencing), "LARGE_PRUNED_BEECH_DOES_NOT_INHERIT_THE_OAK_LADDER", Mix(big));
        Check(Count(big, TimberAssortment.SecondPlanking) >= 1 && Count(big, TimberAssortment.Firewood) >= 1, "LARGE_BEECH_REACHES_SECOND_QUALITY_PLANKING_AND_FIREWOOD", Mix(big));
        StemYieldResult same = Resolve(Stem("oSame", Oak, 60, 24, StemQualityFlags.None, Pruned(6.5, 24)), config);
        Check(same.Logs.Any(l => l.Assortment == TimberAssortment.VeneerPlanking) && !big.Logs.Any(l => l.Assortment == TimberAssortment.VeneerPlanking)
            && big.StemVolumeCm3 == same.StemVolumeCm3, "SAME_SIZE_AND_FORM_OAK_GETS_VENEER_BEECH_DOES_NOT", $"oak={Mix(same)} | beech={Mix(big)}");
        StemYieldResult small = Resolve(Stem("bSmall", Beech, 20, 12), config);
        Check(!AnyOf(small, TimberAssortment.SecondPlanking) && Count(small, TimberAssortment.Firewood) > 0, "SMALL_BEECH_IS_FIREWOOD", Mix(small));
        StemYieldResult unusable = Resolve(Stem("bBad", Beech, 50, 22, StemQualityFlags.Unusable), config);
        Check(unusable.Logs.All(l => l.Assortment == TimberAssortment.Firewood) && unusable.Logs.Length > 0, "UNUSABLE_BEECH_FALLS_BACK_TO_FIREWOOD", Mix(unusable));
        StemYieldResult defect = Resolve(Stem("bDef", Beech, 50, 22, StemQualityFlags.StemDefect), config);
        Check(!AnyOf(defect, TimberAssortment.SecondPlanking) && Count(defect, TimberAssortment.Firewood) > 0, "DEFECTIVE_BEECH_IS_FIREWOOD_NOT_PLANKING", Mix(defect));
        // Pruning does not matter to Beech: no Beech rung requires it.
        StemYieldResult unpruned = Resolve(Stem("bBigU", Beech, 60, 24), config);
        Check(Mix(unpruned) == Mix(big), "BEECH_ELIGIBILITY_DOES_NOT_DEPEND_ON_PRUNING_IN_P0");
    }

    // ---------- quality, pruning and price ----------

    private static void QualityAndPruning(TimberYieldConfiguration config)
    {
        ForestryPriceBook book = Book();
        string before = JsonLike(book);
        StemYieldResult pruned = Resolve(Stem("qp", Oak, 60, 24, StemQualityFlags.None, Pruned(6.5, 24)), config);
        StemYieldResult unpruned = Resolve(Stem("qu", Oak, 60, 24), config);
        // Pruning changes which products are eligible, never what a product is worth.
        Check(JsonLike(book) == before, "PRICE_BOOK_UNCHANGED_BY_GRADING");
        long ValueOf(StemYieldResult r) => r.Logs.Sum(l => (long)Math.Round((decimal)l.VolumeCm3 * Stage1EconomyDefaults.FreshVolumeProxyKgPerCubicMetre / 1000m * book.TimberPrices.First(p => p.SpeciesId == l.SpeciesId && p.Assortment == l.Assortment).RoadsideCentsPerTonne.Selected / 1000000m));
        Check(ValueOf(pruned) > ValueOf(unpruned), "PRUNING_RAISES_VALUE_ONLY_THROUGH_ELIGIBILITY", $"pruned={ValueOf(pruned) / 100.0:0.00} unpruned={ValueOf(unpruned) / 100.0:0.00}");
        long perCubic(StemYieldResult r, TimberAssortment a) => r.Logs.Where(l => l.Assortment == a).Sum(l => l.VolumeCm3);
        Check(perCubic(pruned, TimberAssortment.VeneerPlanking) > 0 && perCubic(unpruned, TimberAssortment.VeneerPlanking) == 0, "PRUNING_UNLOCKS_THE_VENEER_CLASS_BUT_SETS_NO_PRICE");
        // Approved broadleaf targets: the whole log must be inside the recorded pruned section.
        StemYieldResult firstLift = Resolve(Stem("lift1", Oak, 60, 24, StemQualityFlags.None, Pruned(3.5, 24)), config);
        StemYieldResult secondLift = Resolve(Stem("lift2", Oak, 60, 24, StemQualityFlags.None, Pruned(6.0, 24)), config);
        Check(Count(firstLift, TimberAssortment.VeneerPlanking) == 1 && Count(secondLift, TimberAssortment.VeneerPlanking) == 2,
            "BROADLEAF_HIGH_PRUNING_UNLOCKS_ONLY_COVERED_LOGS", $"3.5m={Count(firstLift, TimberAssortment.VeneerPlanking)} 6.0m={Count(secondLift, TimberAssortment.VeneerPlanking)}");
        // Determinism.
        Check(Mix(Resolve(Stem("qp", Oak, 60, 24, StemQualityFlags.None, Pruned(6.5, 24)), config)) == Mix(pruned), "GRADING_IS_DETERMINISTIC");
    }

    private static string JsonLike(ForestryPriceBook book)
        => string.Join("|", book.TimberPrices.Select(p => $"{p.Id}:{p.SpeciesId}:{(int)p.Assortment}:{p.RoadsideCentsPerTonne.Low}/{p.RoadsideCentsPerTonne.Reference}/{p.RoadsideCentsPerTonne.High}/{p.RoadsideCentsPerTonne.Selected}"));

    // ---------- the mid-log rule ----------

    private static void MidLogRule(TimberYieldConfiguration config)
    {
        // Find the smallest DBH (0.1 cm steps) at which the butt log becomes a beam, then check the stated 30 cm
        // mid-log criterion either side of it using the engine's own taper model.
        double height = 22;
        double firstBeamDbh = 0;
        for (double dbh = 20.0; dbh <= 60.0; dbh += 0.1)
        {
            StemYieldResult r = Resolve(Stem("scan", Oak, dbh, height), config);
            if (r.Logs.Any(l => l.Assortment == TimberAssortment.Beam && l.StartHeightMm == 350)) { firstBeamDbh = dbh; break; }
        }
        Check(firstBeamDbh > 0, "BEAM_THRESHOLD_FOUND", $"smallest dbh with a butt beam={firstBeamDbh:0.0} cm at {height} m");
        double MidDiameterMm(double dbhCm)
        {
            StemMeasurements s = Stem("m", Oak, dbhCm, height);
            var model = new MerchantableStemModel(s, 1300);
            return Math.Sqrt((double)model.DiameterSquaredAt(350 + 2600 / 2));
        }
        Check(MidDiameterMm(firstBeamDbh) >= 300 && MidDiameterMm(firstBeamDbh - 0.1) < 300, "BEAM_MID_LOG_BOUNDARY_IS_EXACT", $"mid at threshold={MidDiameterMm(firstBeamDbh):0.0} mm, 0.1 cm below={MidDiameterMm(firstBeamDbh - 0.1):0.0} mm");
        // A stem too thin for the stated mid diameter is not a beam even though its small end passes the engine guard.
        StemYieldResult thin = Resolve(Stem("thin", Oak, firstBeamDbh - 3, height), config);
        Check(!AnyOf(thin, TimberAssortment.Beam), "BELOW_THRESHOLD_STEM_IS_NOT_A_BEAM", Mix(thin));
        // Veneer needs the larger 45 cm mid-log diameter even when pruned and straight.
        StemYieldResult nearVeneer = Resolve(Stem("nv", Oak, 40, 24, StemQualityFlags.None, Pruned(6.5, 24)), config);
        StemYieldResult veneerSize = Resolve(Stem("vs", Oak, 56, 24, StemQualityFlags.None, Pruned(6.5, 24)), config);
        Check(!AnyOf(nearVeneer, TimberAssortment.VeneerPlanking) && AnyOf(veneerSize, TimberAssortment.VeneerPlanking), "VENEER_NEEDS_THE_45_CM_MID_LOG_DIAMETER", $"dbh40={Mix(nearVeneer)} | dbh56={Mix(veneerSize)}");
    }

    // ---------- valuation through the real work calculator ----------

    private static TimberMarketBinding[] Bindings(ForestryPriceBook book) => book.TimberPrices.Select(p => new TimberMarketBinding
        { SpeciesId = p.SpeciesId, Assortment = p.Assortment, RoadsidePriceId = p.Id,
          DensityId = book.Densities.First(d => d.SpeciesId == p.SpeciesId).Id }).ToArray();

    private static WorkQuote Quote(ForestryPriceBook book, params StemMeasurements[] stems)
    {
        TimberYieldConfiguration config = TimberYieldDefaults.CreateScenarioOne();
        StemYieldRequest[] requests = stems.Select(s => new StemYieldRequest { Stem = s, DefaultDisposition = TimberDisposition.SellAndExtract, ExtractRetainedToRoadside = true }).ToArray();
        StandOperationYield op = TimberYieldCalculator.ResolveStandOperation("harvest-7", requests, config);
        TimberBatch[] batches = TimberYieldEconomyAdapter.ToTimberBatches(op, Bindings(book));
        long full = stems.Sum(s => (long)Math.Round((decimal)s.StemVolumeCm3 * Stage1EconomyDefaults.FreshVolumeProxyKgPerCubicMetre / 1000m, MidpointRounding.AwayFromZero));
        long priced = batches.Sum(b => (long)Math.Round((decimal)b.Quantity * Stage1EconomyDefaults.FreshVolumeProxyKgPerCubicMetre / 1000m, MidpointRounding.AwayFromZero));
        var task = new ForestryTask { TaskId = "harvest-7", WorldOperationId = "harvest-7", Type = ForestryTaskType.Harvest,
            Quantity = new WorkQuantity { Amount = stems.Length }, TargetIds = stems.Select(s => s.TreeId).ToArray(), Timber = batches,
            UnpricedHarvestGreenGrams = Math.Max(0, full - priced), UnpricedForwardGreenGrams = Math.Max(0, full - priced), HarvestContext = HarvestOperationContext.FirstThinning };
        var actor = new ExecutionResources { ContractorAvailable = true, Capabilities = WorkCapability.ProductionHarvesting, Tools = WorkTool.HarvestingSystem, AvailableCashCents = 100000000 };
        return ForestryWorkCalculator.Quote(task, WorkExecutionMethod.Contractor, actor, book);
    }

    private static void Valuation(TimberYieldConfiguration config)
    {
        ForestryPriceBook book = Book();
        StemMeasurements oak = Stem("oakV", Oak, 60, 24, StemQualityFlags.None, Pruned(6.5, 24));
        WorkQuote q = Quote(book, oak);
        Check(q.Eligible && q.PricingComplete && q.TimberSaleRevenueCents > 0, "OAK_HARVEST_QUOTE_IS_ELIGIBLE_AND_EARNS_REVENUE", $"revenue={q.TimberSaleRevenueCents / 100.0:0.00} cost={q.Costs.TotalExternalCostCents / 100.0:0.00}");
        // Revenue is the sum over products of volume x that product's own price, nothing else.
        StemYieldResult graded = Resolve(oak, config);
        decimal expected = 0;
        foreach (var group in graded.Logs.GroupBy(l => l.Assortment))
        {
            long grams = (long)Math.Round((decimal)group.Sum(l => l.VolumeCm3) * Stage1EconomyDefaults.FreshVolumeProxyKgPerCubicMetre / 1000m, MidpointRounding.AwayFromZero);
            decimal price = book.TimberPrices.First(p => p.SpeciesId == Oak && p.Assortment == group.Key).RoadsideCentsPerTonne.Selected;
            expected += grams * price / 1000000m;
        }
        Check(Math.Abs((decimal)q.TimberSaleRevenueCents - expected) <= graded.Logs.Length, "REVENUE_IS_PER_PRODUCT_NOT_A_FLAT_RATE", $"quote={q.TimberSaleRevenueCents} expected={expected:0.0}");
        Check(q.Timber.Select(v => v.Batch.Assortment).Distinct().Count() >= 3, "OAK_REVENUE_SPANS_SEVERAL_PRODUCT_PRICES", string.Join(",", q.Timber.Select(v => v.Batch.Assortment).Distinct()));
        Check(q.Timber.All(v => v.Batch.SpeciesId == Oak), "OAK_BATCHES_CARRY_THE_OAK_SPECIES");

        // Beech earns revenue only from its own two products.
        WorkQuote beech = Quote(book, Stem("bV", Beech, 60, 24, StemQualityFlags.None, Pruned(6.5, 24)));
        Check(beech.Eligible && beech.TimberSaleRevenueCents > 0 && beech.Timber.All(v => v.Batch.Assortment == TimberAssortment.SecondPlanking || v.Batch.Assortment == TimberAssortment.Firewood),
            "BEECH_REVENUE_ONLY_FROM_SECOND_PLANKING_AND_FIREWOOD", $"revenue={beech.TimberSaleRevenueCents / 100.0:0.00}");
        // Same volume, same cost: no species-specific harvesting cost multiplier.
        StemMeasurements sitka = Stem("sV", Sitka, 60, 24);
        WorkQuote sitkaQuote = Quote(book, sitka);
        Check(sitkaQuote.Costs.CombinedHarvestingForwardingCents == q.Costs.CombinedHarvestingForwardingCents && sitkaQuote.Costs.TotalExternalCostCents == q.Costs.TotalExternalCostCents
            && sitkaQuote.Costs.MinimumJobAdjustmentCents == q.Costs.MinimumJobAdjustmentCents, "HARVEST_COST_IS_GENERIC_NOT_PER_SPECIES",
            $"sitka={sitkaQuote.Costs.TotalExternalCostCents} oak={q.Costs.TotalExternalCostCents} beech={beech.Costs.TotalExternalCostCents}");
        // A Sitka-only quote is unaffected by the extra price entries.
        ForestryPriceBook sitkaBook = Stage1EconomyDefaults.Create();
        WorkQuote withExtra = Quote(book, sitka), without = Quote(sitkaBook, sitka);
        Check(withExtra.TimberSaleRevenueCents == without.TimberSaleRevenueCents && withExtra.Costs.TotalExternalCostCents == without.Costs.TotalExternalCostCents && withExtra.NetCashCents == without.NetCashCents,
            "SITKA_QUOTE_UNAFFECTED_BY_BROADLEAF_PRICES", $"revenue={withExtra.TimberSaleRevenueCents} net={withExtra.NetCashCents}");
        // Deterministic.
        Check(Quote(book, oak).TimberSaleRevenueCents == q.TimberSaleRevenueCents, "VALUATION_IS_DETERMINISTIC");
    }
}
