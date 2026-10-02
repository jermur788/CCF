using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CCF.Forestry.TimberYield;
using CCF.Forestry.WorkEconomy;
#if !UNITY_5_3_OR_NEWER
using System.Text.Json;
#endif

// Pure, repeatable fixtures; Editor-only archive diagnostics read but never restore/save worlds.
public static class TimberAssortmentYieldVerification
{
    private static readonly List<string> Evidence = new List<string>();
    private static int assertions;
#if UNITY_EDITOR
    public static void Begin()
    {
        try { Run(); ArchiveDiagnostics(); UnityEditor.EditorApplication.Exit(0); }
        catch (Exception exception) { UnityEngine.Debug.LogException(exception); UnityEditor.EditorApplication.Exit(1); }
    }
#elif !UNITY_5_3_OR_NEWER
    public static int Main()
    {
        try { Run(); return 0; } catch (Exception exception) { Console.Error.WriteLine(exception); return 1; }
    }
#endif

    public static void Run()
    {
        Evidence.Clear(); assertions = 0;
        var config = TimberYieldDefaults.CreateSitka();
        var tiny = Request("tiny", 4, 3.5);
        var below = Resolve("below-merchantable", tiny, config);
        Equal(0, below.Logs.Length, "below threshold has no product");
        Equal(below.StemVolumeCm3, below.ResidualVolumeCm3, "tiny stem stays residual");
        var pulpRequest = Request("pulp-only", 9, 14); pulpRequest.Stem.Quality = StemQualityFlags.PoorForm;
        var pulp = Resolve("pulp-only", pulpRequest, config);
        Check(pulp.Logs.Length > 0 && Array.TrueForAll(pulp.Logs, x => x.Assortment == TimberAssortment.Pulp), "poor-form small stem is pulp only");
        var stakeRequest = Request("stake-pulp", 14, 14);
        var stake = Resolve("stake-pulp", stakeRequest, config);
        Has(stake, TimberAssortment.Stake); Has(stake, TimberAssortment.Pulp);
        var palletRequest = Request("intermediate", 18, 16);
        var pallet = Resolve("pallet-top", palletRequest, config);
        Has(pallet, TimberAssortment.Pallet);
        Check(Array.Exists(pallet.Logs, x => x.Assortment == TimberAssortment.Stake || x.Assortment == TimberAssortment.Pulp), "secondary top products");
        var matureRequest = Request("mature", 35, 25);
        var mature = Resolve("sawlog-secondary", matureRequest, config);
        Has(mature, TimberAssortment.Sawlog); Has(mature, TimberAssortment.Pallet); Has(mature, TimberAssortment.Stake);
        Check(ProductCount(mature) >= 3, "multiple assortments from one stem");
        foreach (var r in new[] { below, pulp, stake, pallet, mature }) LogSummary("REPRESENTATIVE", r);

        // Independent geometric oracle: paraboloid cumulative volume is 3/4 at half height.
        var analytic = Request("analytic", 14, 10); analytic.Stem.StemVolumeCm3 = 1000000;
        var model = new MerchantableStemModel(analytic.Stem, 1300);
        Equal(750000, model.CumulativeVolumeAt(5000), "half-height cumulative oracle");
        Equal(250000, model.VolumeBetween(5000, 10000), "upper-half volume oracle");
        Check(model.DiameterSquaredAt(1300) == 19600m, "DBH anchor");
        Equal(0, model.DiameterFloorAt(10000), "zero-diameter tip");

        var retainedRequest = Request("kept-poles", 14, 14);
        retainedRequest.DispositionOverrides = new[] { new AssortmentDispositionRule { Assortment = TimberAssortment.Stake, Disposition = TimberDisposition.KeepForUse } };
        retainedRequest.ExtractRetainedToRoadside = true;
        var retained = Resolve("retained-poles", retainedRequest, config);
        Check(retained.RetainedForUseVolumeCm3 > 0 && retained.SaleAssortmentVolumeCm3 > 0, "mixed sold/retained material");
        Check(Array.Exists(retained.Logs, x => x.Disposition == TimberDisposition.KeepForUse && x.PotentialUse == PotentialMaterialUse.PolesAndStakes), "useful pole/stake category");
        var keptSaw = Request("kept-saw", 35, 25); keptSaw.DefaultDisposition = TimberDisposition.KeepForUse;
        var kept = Resolve("retained-sawlog", keptSaw, config);
        Equal(0, kept.SaleAssortmentVolumeCm3, "kept sawlog has no sale material");
        Check(Array.Exists(kept.Logs, x => x.PotentialUse == PotentialMaterialUse.SawlogForProcessing), "retained processing sawlog");
        var deadwoodRequest = Request("deadwood", 18, 16); deadwoodRequest.DefaultDisposition = TimberDisposition.RetainAsFallenDeadwood;
        var deadwood = Resolve("deadwood-reference-only", deadwoodRequest, config);
        Check(deadwood.DeadwoodAssortmentVolumeCm3 > 0 && Array.TrueForAll(deadwood.Logs, x => !x.ExtractToRoadside), "deadwood not extracted");

        var shorter = Roundtrip(config); shorter.LengthPreference = LogLengthPreference.ShortestFirst;
        var shortResult = Resolve("shortest-first", palletRequest, shorter);
        Check(shortResult.Logs[0].EndHeightMm != pallet.Logs[0].EndHeightMm, "explicit length strategy changes allocation");
        var changed = Roundtrip(config); changed.Specifications[0].MinimumSmallEndDiameterMm = 350;
        var changedResult = Resolve("changed-specification", matureRequest, changed);
        Check(!Array.Exists(changedResult.Logs, x => x.Assortment == TimberAssortment.Sawlog), "changed diameter specification changes product allocation");
        var pulpFirst = Roundtrip(config); pulpFirst.Specifications[3].Priority = 0;
        var pulpFirstResult = Resolve("configured-priority", matureRequest, pulpFirst);
        Check(pulpFirstResult.Logs[0].Assortment == TimberAssortment.Pulp, "priority is explicit, no price optimiser");
        var bounded = Roundtrip(config); bounded.Specifications[0].MaximumEndHeightMm = 5250;
        Check(Array.TrueForAll(Resolve("section-bound", matureRequest, bounded).Logs, x => x.Assortment != TimberAssortment.Sawlog || x.EndHeightMm <= 5250), "bounded stem section");
        var maximum = Roundtrip(config); maximum.Specifications[0].MaximumLargeEndDiameterMm = 250;
        Check(Resolve("large-end-limit", matureRequest, maximum).Logs[0].Assortment != TimberAssortment.Sawlog, "large-end maximum enforced");

        var neutral = Resolve("neutral-quality", Request("quality-neutral", 35, 25), config);
        var prunedRequest = Request("quality-pruned", 35, 25); prunedRequest.Stem.Quality = StemQualityFlags.Pruned;
        var pruned = Resolve("pruned-metadata", prunedRequest, config);
        Equal(neutral.SaleAssortmentVolumeCm3, pruned.SaleAssortmentVolumeCm3, "no invented pruning premium/yield uplift");
        Equal(neutral.Logs.Length, pruned.Logs.Length, "pruning flag alone does not change bucking");
        var damaged = Request("quality-damaged", 35, 25); damaged.Stem.Quality = StemQualityFlags.WindDamage | StemQualityFlags.BrowseFormDamage;
        Check(Array.TrueForAll(Resolve("damage-interface", damaged, config).Logs, x => x.Assortment == TimberAssortment.Pulp), "supplied damage downgrades configured grades without simulating damage");
        var unusable = Request("unusable", 35, 25); unusable.Stem.Quality = StemQualityFlags.Unusable;
        Equal(0, Resolve("unusable-quality", unusable, config).Logs.Length, "unusable sections not awarded");
        var qualityRange = Request("quality-range", 35, 25);
        qualityRange.Stem.QualitySections = new[] { new StemQualitySection { StartHeightMm = 350, EndHeightMm = 5250, Flags = StemQualityFlags.Pruned } };
        var requiresPruned = Roundtrip(config); requiresPruned.Specifications[0].RequiredQuality = StemQualityFlags.Pruned;
        var range = Resolve("pruned-full-section", qualityRange, requiresPruned);
        Equal(1, Count(range, TimberAssortment.Sawlog), "required quality must cover whole log, not overlap a small part");
        qualityRange.Stem.QualitySections[0].EndHeightMm = 5249;
        Equal(0, Count(Resolve("pruned-incomplete-section", qualityRange, requiresPruned), TimberAssortment.Sawlog), "one unpruned mm rejects whole-log requirement");
        var localized = Request("localized-defect", 35, 25);
        localized.Stem.QualitySections = new[] { new StemQualitySection { StartHeightMm = 350, EndHeightMm = 1200, Flags = StemQualityFlags.Unusable } };
        var localizedResult = Resolve("skip-defect", localized, config);
        Check(localizedResult.Logs.Length > 0 && localizedResult.Logs[0].StartHeightMm >= 1200, "scan resumes after unusable basal section");

        var measured = Request("measured", 18, 16); measured.Stem.ProfileKind = StemProfileKind.MeasuredSquaredDiameterProfile;
        measured.Stem.ProfilePoints = new[] { Point(0, 200), Point(1300, 180), Point(8000, 140), Point(16000, 0) };
        var measuredResult = Resolve("measured-profile", measured, config);
        Check(measuredResult.ProfileKind == StemProfileKind.MeasuredSquaredDiameterProfile, "measured profile path");
        var measuredModel = new MerchantableStemModel(measured.Stem, 1300);
        Check(measuredModel.DiameterSquaredAt(8000) == 19600m, "measured node exact");
        var snapshot = measuredModel.DiameterSquaredAt(8000); measured.Stem.ProfilePoints[2].DiameterMm = 1;
        Check(snapshot == measuredModel.DiameterSquaredAt(8000), "model snapshots measured points");

        var subunit = Request("subunit", 35, 25); subunit.Stem.StemVolumeCm3 = 1;
        var subunitResult = Resolve("volume-rounding-boundary", subunit, config);
        Equal(1, subunitResult.StemVolumeCm3, "one cm3 budget");
        Check(Array.TrueForAll(subunitResult.Logs, x => x.VolumeCm3 > 0), "no zero-volume material awards");
        var zero = Request("zero-volume", 14, 14); zero.Stem.StemVolumeCm3 = 0;
        Equal(0, Resolve("zero-volume", zero, config).Logs.Length, "zero budget no material");
        var boundaryConfig = Roundtrip(config); boundaryConfig.StumpHeightMm = 0; boundaryConfig.CuttingLossMm = 0;
        boundaryConfig.Specifications = new[] { boundaryConfig.Specifications[3] };
        var boundary = Request("diameter-boundary", 7, 4.3); // d(1.3 m) == 70 mm: use a 1.3 m fixture specification.
        boundaryConfig.Specifications[0].MinimumLengthMm = 1300; boundaryConfig.Specifications[0].MaximumLengthMm = 1300;
        boundaryConfig.Specifications[0].NominalLengthsMm = new[] { 1300 };
        Equal(1, Resolve("exact-diameter-boundary", boundary, boundaryConfig).Logs.Length, "unrounded exact minimum admitted");
        boundary.Stem.DbhMm = 69;
        Equal(0, Resolve("below-diameter-boundary", boundary, boundaryConfig).Logs.Length, "69 mm cannot round up to 70");

        var requests = new[] { tiny, pulpRequest, stakeRequest, palletRequest, matureRequest, retainedRequest, keptSaw };
        var operation = TimberYieldCalculator.ResolveStandOperation("thinning-job", requests, config);
        long stemTotal = 0, saleTotal = 0, keptTotal = 0, residualTotal = 0;
        foreach (var request in requests)
        {
            var single = TimberYieldCalculator.ResolveSingleStem(request, config);
            stemTotal += single.StemVolumeCm3; saleTotal += single.SaleAssortmentVolumeCm3;
            keptTotal += single.RetainedForUseVolumeCm3; residualTotal += single.ResidualVolumeCm3;
        }
        Equal(stemTotal, operation.StemVolumeCm3, "aggregate stem sum"); Equal(saleTotal, operation.SaleAssortmentVolumeCm3, "aggregate sold sum");
        Equal(keptTotal, operation.RetainedForUseVolumeCm3, "aggregate retained sum"); Equal(residualTotal, operation.ResidualVolumeCm3, "aggregate residual sum");
        long summaryVolume = 0; int summaryPieces = 0;
        foreach (var summary in operation.Assortments) { summaryVolume += summary.VolumeCm3; summaryPieces += summary.Pieces; }
        Equal(operation.SaleAssortmentVolumeCm3 + operation.RetainedForUseVolumeCm3, summaryVolume, "summary volume sum");
        int pieces = 0; foreach (var stem in operation.Stems) pieces += stem.Logs.Length; Equal(pieces, summaryPieces, "summary pieces sum");
        var operationJson = Json(operation); Array.Reverse(requests);
        Check(operationJson == Json(TimberYieldCalculator.ResolveStandOperation("thinning-job", requests, config)), "input order independence");
        for (int i = 0; i < 30; i++) Check(operationJson == Json(TimberYieldCalculator.ResolveStandOperation("thinning-job", requests, config)), "repeated deterministic operation");
        Check(operationJson == Json(Roundtrip(operation)), "result serialization roundtrip");
        Check(operationJson == Json(TimberYieldCalculator.ResolveStandOperation("thinning-job", Roundtrip(requests), Roundtrip(config))), "input/config serialization roundtrip");
        Evidence.Add("aggregate:" + operationJson);
        var empty = TimberYieldCalculator.ResolveStandOperation("empty", Array.Empty<StemYieldRequest>(), config);
        Equal(0, empty.StemVolumeCm3, "empty operation");
        var unsupportedSpecies = Request("other-species", 18, 16); unsupportedSpecies.Stem.SpeciesId = "beech";
        Equal(0, Resolve("no-silent-species-transfer", unsupportedSpecies, config).Logs.Length, "no automatic Sitka specification for Beech");

        var bindings = Bindings();
        var batches = TimberYieldEconomyAdapter.ToTimberBatches(operation, bindings);
        Equal(pieces, batches.Length, "direct economy batch per disjoint log");
        long convertedVolume = 0;
        foreach (var batch in batches) { Check(batch.Unit == TimberQuantityUnit.CubicCentimetres && batch.Quantity > 0, "volume units preserved"); convertedVolume += batch.Quantity; }
        Equal(summaryVolume, convertedVolume, "economy adapter does not mint volume");
        var resources = new ExecutionResources { Capabilities = WorkCapability.ProductionHarvesting, Tools = WorkTool.HarvestingSystem,
            ContractorAvailable = true, AvailableCashCents = 10000000 };
        // Retained onsite logs need an explicit split/final quote under the existing accounting contract.
        var task = new ForestryTask { TaskId = "thinning-job", WorldOperationId = "plan-thinning", Type = ForestryTaskType.Harvest,
            Quantity = new WorkQuantity { Amount = requests.Length }, Timber = batches, HasHarvestQuote = true,
            HarvestQuote = new QuotedHarvestCosts { HarvestingCents = 120000, ForwardingCents = 30000, Source = "Fixture final job quote; not empirical calibration" } };
        var economic = ForestryWorkCalculator.Resolve(task, WorkExecutionMethod.Contractor, resources, Stage1EconomyDefaults.Create());
        Check(economic.Resolved, "WorkEconomy consumes aggregate directly");
        Check(economic.Quote.RetainedForUseReferenceValueCents > 0, "retained economic reference");
        Equal(economic.ExternalCashFlowCents, ForestryWorkCalculator.SumLedger(economic.Ledger), "existing accounting still reconciles");
        Evidence.Add("economy:" + Json(economic));
        var frozen = operationJson; matureRequest.Stem.DbhMm = 1; config.Specifications[0].MinimumSmallEndDiameterMm = 1;
        Check(frozen == Json(operation), "result snapshot isolation"); config = TimberYieldDefaults.CreateSitka();

        Invalid("nan", () => MerchantableStemModel.FromMetres("x", "sitka-spruce", double.NaN, 10, 0.1, "test"));
        Invalid("infinite", () => MerchantableStemModel.FromMetres("x", "sitka-spruce", 10, double.PositiveInfinity, 0.1, "test"));
        Invalid("negative-dimension", () => { var r = Request("x", 10, 10); r.Stem.DbhMm = -1; TimberYieldCalculator.ResolveSingleStem(r, config); });
        Invalid("dbh-above-height", () => TimberYieldCalculator.ResolveSingleStem(Request("x", 10, 1.3), config));
        Invalid("negative-volume", () => { var r = Request("x", 10, 10); r.Stem.StemVolumeCm3 = -1; TimberYieldCalculator.ResolveSingleStem(r, config); });
        Invalid("duplicate-trees", () => TimberYieldCalculator.ResolveStandOperation("x", new[] { tiny, tiny }, config));
        Invalid("duplicate-length", () => { var c = Roundtrip(config); c.Specifications[0].NominalLengthsMm = new[] { 4900, 4900 }; TimberYieldCalculator.ResolveSingleStem(tiny, c); });
        Invalid("unknown-quality", () => { var r = Request("x", 10, 10); r.Stem.Quality = (StemQualityFlags)128; TimberYieldCalculator.ResolveSingleStem(r, config); });
        Invalid("quality-outside-stem", () => { var r = Request("x", 10, 10); r.Stem.QualitySections = new[] { new StemQualitySection { StartHeightMm = 0, EndHeightMm = 11000 } }; TimberYieldCalculator.ResolveSingleStem(r, config); });
        Invalid("profile-order", () => { var r = Request("x", 18, 16); r.Stem.ProfileKind = StemProfileKind.MeasuredSquaredDiameterProfile; r.Stem.ProfilePoints = new[] { Point(0, 200), Point(16000, 0), Point(1300, 180) }; TimberYieldCalculator.ResolveSingleStem(r, config); });
        Invalid("missing-binding", () => TimberYieldEconomyAdapter.ToTimberBatches(operation, Array.Empty<TimberMarketBinding>()));
        Invalid("tampered-volume", () => { var op = Roundtrip(operation); op.Stems[0].ResidualVolumeCm3++; TimberYieldEconomyAdapter.ToTimberBatches(op, bindings); });

        // Conservation sweep across dimensions, neutral/poor form and both length preferences.
        foreach (int dbh in new[] { 40, 70, 90, 140, 180, 250, 350, 500 })
        foreach (int height in new[] { 3500, 8000, 12000, 20000, 35000 })
        foreach (var preference in new[] { LogLengthPreference.LongestFirst, LogLengthPreference.ShortestFirst })
        {
            var c = Roundtrip(config); c.LengthPreference = preference;
            var r = Request("sweep", dbh / 10.0, height / 1000.0); r.Stem.Quality = dbh == 90 ? StemQualityFlags.PoorForm : StemQualityFlags.None;
            var result = TimberYieldCalculator.ResolveSingleStem(r, c); Conserve(result); Evidence.Add("sweep:" + Json(result));
        }
        var culture = CultureInfo.CurrentCulture;
        try { var r = Request("culture", 18, 16); string text = Json(TimberYieldCalculator.ResolveSingleStem(r, config));
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR"); Check(text == Json(TimberYieldCalculator.ResolveSingleStem(r, config)), "culture independent"); }
        finally { CultureInfo.CurrentCulture = culture; }
        Performance(config);
        string digest; using (var sha = SHA256.Create()) digest = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", Evidence)))).Replace("-", "");
        Log("TIMBER_YIELD_VERIFY_PASS fixtures=" + Evidence.Count + " assertions=" + assertions + " evidenceSha256=" + digest);
    }

    private static StemYieldResult Resolve(string name, StemYieldRequest request, TimberYieldConfiguration config)
    {
        var result = TimberYieldCalculator.ResolveSingleStem(request, config); Conserve(result); Evidence.Add(name + ":" + Json(result)); return result;
    }
    private static void Conserve(StemYieldResult r)
    {
        Equal(r.StemVolumeCm3, r.SaleAssortmentVolumeCm3 + r.RetainedForUseVolumeCm3 + r.DeadwoodAssortmentVolumeCm3 + r.ResidualVolumeCm3, "material budget reconciles");
        var sections = new List<(int start, int end, long volume)>(); long total = 0;
        foreach (var log in r.Logs) { Check(log.VolumeCm3 > 0, "positive log volume"); sections.Add((log.StartHeightMm, log.EndHeightMm, log.VolumeCm3)); }
        foreach (var part in r.ResidualSections) { Check(part.VolumeCm3 >= 0, "nonnegative residual"); sections.Add((part.StartHeightMm, part.EndHeightMm, part.VolumeCm3)); }
        sections.Sort((a, b) => a.start.CompareTo(b.start)); int position = 0;
        foreach (var section in sections) { Equal(position, section.start, "no overlap or missing section"); Check(section.end > section.start, "positive section length"); position = section.end; total += section.volume; }
        Equal(r.TotalHeightMm, position, "full stem partition"); Equal(r.StemVolumeCm3, total, "section volume sum");
    }
    private static StemYieldRequest Request(string id, double dbh, double height) => new StemYieldRequest { Stem = MerchantableStemModel.FromMetres(id, "sitka-spruce", dbh, height,
        dbh * dbh * 0.00007854 * height * 0.5, "Starting-branch biological volume formula; neutral quality [S]") };
    private static StemProfilePoint Point(int height, int diameter) => new StemProfilePoint { HeightMm = height, DiameterMm = diameter };
    private static int Count(StemYieldResult r, TimberAssortment type) { int n = 0; foreach (var log in r.Logs) if (log.Assortment == type) n++; return n; }
    private static void Has(StemYieldResult r, TimberAssortment type) => Check(Count(r, type) > 0, "has " + type);
    private static int ProductCount(StemYieldResult r) { var kinds = new HashSet<TimberAssortment>(); foreach (var log in r.Logs) kinds.Add(log.Assortment); return kinds.Count; }
    private static TimberMarketBinding[] Bindings() => new[] { Binding(TimberAssortment.Pulp), Binding(TimberAssortment.Stake), Binding(TimberAssortment.Pallet), Binding(TimberAssortment.Sawlog) };
    private static TimberMarketBinding Binding(TimberAssortment type) => new TimberMarketBinding { SpeciesId = "sitka-spruce", Assortment = type,
        RoadsidePriceId = "sitka-" + type.ToString().ToLowerInvariant(), DensityId = "sitka-fresh-roadside" };
    private static void LogSummary(string tag, StemYieldResult r)
    {
        var parts = new List<string>(); foreach (TimberAssortment type in Enum.GetValues(typeof(TimberAssortment))) { long volume = 0;
            foreach (var log in r.Logs) if (log.Assortment == type) volume += log.VolumeCm3; parts.Add(type + "=" + volume + "cm3/" + Count(r, type) + "pieces"); }
        Log(tag + " " + r.TreeId + " dbhMm=" + r.DbhMm + " heightMm=" + r.TotalHeightMm + " totalCm3=" + r.StemVolumeCm3 + " " + string.Join(" ", parts) + " residualCm3=" + r.ResidualVolumeCm3);
    }
    private static void Performance(TimberYieldConfiguration config)
    {
        TimberYieldCalculator.ResolveStandOperation("warmup", new[] { Request("warmup", 18, 16) }, config);
        foreach (int count in new[] { 100, 500, 1500 })
        {
            var requests = new StemYieldRequest[count]; for (int i = 0; i < count; i++) requests[i] = Request("B" + i.ToString("D5", CultureInfo.InvariantCulture), 10 + i % 31, 10 + i % 21);
            var watch = Stopwatch.StartNew(); var result = TimberYieldCalculator.ResolveStandOperation("performance", requests, config); watch.Stop();
            Equal(count, result.Stems.Length, "benchmark stem count");
            Log("TIMBER_YIELD_PERFORMANCE stems=" + count + " milliseconds=" + watch.Elapsed.TotalMilliseconds.ToString("F3", CultureInfo.InvariantCulture) + " totalCm3=" + result.StemVolumeCm3);
        }
    }
#if UNITY_EDITOR
    private static void ArchiveDiagnostics()
    {
        var archive = ScenarioReferenceArchive.Load(); Check(archive != null, "read-only actual Scenario One archive available");
        foreach (int year in new[] { 0, 50, 100 })
        {
            var trees = archive.AtYear(year).world.trees.FindAll(x => x.speciesId == "sitka-spruce" && x.stage != (int)ForestTreeStage.Stump && x.heightMeters > 1.3f);
            trees.Sort((a, b) => { int order = a.diameterCm.CompareTo(b.diameterCm); return order != 0 ? order : StringComparer.Ordinal.Compare(a.treeId, b.treeId); });
            var chosen = year == 0 ? new[] { trees[0], trees[trees.Count / 2], trees[trees.Count - 1] } : new[] { trees[trees.Count / 2] };
            foreach (var tree in chosen)
            {
                var request = Request(tree.treeId, tree.diameterCm, tree.heightMeters);
                // Exact current volume-interface float arithmetic is read-only, not a new volume calibration.
                request.Stem.StemVolumeCm3 = (long)Math.Floor((double)(tree.diameterCm * tree.diameterCm * 0.00007854f * tree.heightMeters * 0.5f) * 1000000);
                LogSummary("ARCHIVE_Y" + year, TimberYieldCalculator.ResolveSingleStem(request, TimberYieldDefaults.CreateSitka()));
            }
        }
        Log("TIMBER_YIELD_ARCHIVE_DIAGNOSTIC_PASS readOnly=true");
    }
#endif
    private static void Check(bool value, string message) { assertions++; if (!value) throw new Exception("FAILED: " + message); }
    private static void Equal(long expected, long actual, string message) => Check(expected == actual, message + " expected=" + expected + " actual=" + actual);
    private static void Invalid(string name, Action action) { bool rejected = false; try { action(); } catch (ArgumentException) { rejected = true; } Check(rejected, name); Evidence.Add(name + ":REJECTED"); }
    private static string Json<T>(T value)
    {
#if UNITY_5_3_OR_NEWER
        // Unity JsonUtility needs a class wrapper for top-level arrays; fixtures use this only via Roundtrip.
        return UnityEngine.JsonUtility.ToJson(value);
#else
        return JsonSerializer.Serialize(value, new JsonSerializerOptions { IncludeFields = true });
#endif
    }
    [Serializable] private sealed class RequestList { public StemYieldRequest[] Values; }
    private static StemYieldRequest[] Roundtrip(StemYieldRequest[] value) => Roundtrip(new RequestList { Values = value }).Values;
    private static T Roundtrip<T>(T value)
    {
#if UNITY_5_3_OR_NEWER
        return UnityEngine.JsonUtility.FromJson<T>(Json(value));
#else
        return JsonSerializer.Deserialize<T>(Json(value), new JsonSerializerOptions { IncludeFields = true });
#endif
    }
    private static void Log(string text)
    {
#if UNITY_5_3_OR_NEWER
        UnityEngine.Debug.Log(text);
#else
        Console.WriteLine(text);
#endif
    }
}
