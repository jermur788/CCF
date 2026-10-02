using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CCF.Forestry.WorkEconomy;
#if !UNITY_5_3_OR_NEWER
using System.Text.Json;
#endif

// Pure calculation fixtures. No scene opening, disk saves, play-mode hooks or world mutation.
public static class Stage1WorkEconomyFoundationVerification
{
    private const long Tonne = 1000000;
    private static readonly List<string> Evidence = new List<string>();
    private static int assertions;

#if UNITY_EDITOR
    public static void Begin()
    {
        try { Run(); UnityEditor.EditorApplication.Exit(0); }
        catch (Exception exception) { UnityEngine.Debug.LogException(exception); UnityEditor.EditorApplication.Exit(1); }
    }
#elif !UNITY_5_3_OR_NEWER
    public static int Main()
    {
        try { Run(); return 0; }
        catch (Exception exception) { Console.Error.WriteLine(exception); return 1; }
    }
#endif

    public static void Run()
    {
        Evidence.Clear(); assertions = 0;
        var book = Stage1EconomyDefaults.Create();
        var actor = Actor();
        var small = Resolve("small", Harvest(10), WorkExecutionMethod.Contractor, actor, book);
        Check(small.Resolved, "small resolves");
        Equal(21000, small.Quote.Costs.CombinedHarvestingForwardingCents, "10 t variable €210");
        Equal(229000, small.Quote.Costs.MinimumJobAdjustmentCents, "small floor adjustment €2290");
        Equal(250000, small.Quote.Costs.TotalExternalCostCents, "small job €2500 floor");
        Equal(-155000, small.ExternalCashFlowCents, "small net loss is legitimate, not clamped");
        Check(!small.Quote.Requirements.PersonMinutesKnown, "harvest time not fabricated from €/t");

        var large = Resolve("large", Harvest(200), WorkExecutionMethod.Contractor, actor, book);
        Equal(420000, large.Quote.Costs.TotalExternalCostCents, "large variable cost €4200");
        Equal(0, large.Quote.Costs.MinimumJobAdjustmentCents, "large floor not added");
        Equal(1480000, large.ExternalCashFlowCents, "large net €14800");

        var mixedTask = Harvest(150);
        mixedTask.Timber = new[] { Batch("saw", TimberAssortment.Sawlog, 100), Batch("poles", TimberAssortment.Stake, 20, TimberDisposition.KeepForUse), Batch("pulp", TimberAssortment.Pulp, 30) };
        var mixed = Resolve("mixed-assortments", mixedTask, WorkExecutionMethod.Contractor, actor, book);
        Equal(3, mixed.MaterialOutputs.Length, "one tree yields three assortments");
        Equal(1064000, mixed.Quote.TimberSaleRevenueCents, "saw+pulp sale only");
        Equal(94000, mixed.Quote.RetainedForUseReferenceValueCents, "retained stake reference €940");
        Equal(1158000, mixed.Quote.GrossRoadsideReferenceValueCents, "gross includes noncash reference");
        Equal(315000, mixed.Quote.Costs.TotalExternalCostCents, "retained extracted poles also cost work");
        Equal(749000, mixed.ExternalCashFlowCents, "mixed net €7490");
        Equal(0, Array.Find(mixed.MaterialOutputs, x => x.Batch.Disposition == TimberDisposition.KeepForUse).SaleRevenueCents, "no retained sale");
        foreach (var output in mixed.MaterialOutputs) Check(output.Batch.SourceTreeId == "TREE001", "same stem provenance");
        Check(Array.TrueForAll(mixed.Ledger, x => x.Category != LedgerCategory.TimberSale || x.ReferenceId != "poles"), "no retained revenue ledger entry");

        mixedTask.Timber[0].OwnerPaysHaulage = true;
        mixedTask.Timber[2].OwnerPaysHaulage = true;
        var hauled = Resolve("haulage", mixedTask, WorkExecutionMethod.Contractor, actor, book);
        Equal(156000, hauled.Quote.Costs.HaulageCents, "130 t owner haulage €1560");
        Equal(471000, hauled.Quote.Costs.TotalExternalCostCents, "haulage added once independently");
        Equal(1064000, hauled.Quote.TimberSaleRevenueCents, "transport does not invent a delivered premium");

        var plant = Task(ForestryTaskType.Planting, WorkQuantityUnit.Items, 100);
        plant.Materials = new[] { Material("beech-sapling", 100, MaterialSupply.PurchaseForJob) };
        var owner = Resolve("owner-planting", plant, WorkExecutionMethod.LandownerSimulated, actor, book);
        Equal(60, owner.OwnerMinutesConsumed, "100 plants/hour");
        Equal(9500, owner.Quote.Costs.MaterialPurchasesCents, "€95 wholesale stock cost");
        Equal(0, owner.Quote.Costs.ContractorWorkCents, "owner does not pay themselves contractor wage");
        Equal(-9500, owner.ExternalCashFlowCents, "owner cash excludes unpaid time");
        book.OwnerTimeValueCentsPerHour.Selected = 2000;
        book.OwnerEquipmentCentsPerHour.Selected = 300;
        var shadow = Resolve("owner-opportunity", plant, WorkExecutionMethod.LandownerSimulated, actor, book);
        Equal(2000, shadow.Quote.OwnerTimeOpportunityCostCents, "time shadow €20");
        Equal(-9800, shadow.ExternalCashFlowCents, "€3 tool operating cash cost");
        Equal(-11800, shadow.Quote.NetAfterOwnerTimeCents, "opportunity view separate from ledger");
        book = Stage1EconomyDefaults.Create();

        plant.Materials[0].Supply = MaterialSupply.ExistingStock;
        var stocked = Resolve("owned-stock", plant, WorkExecutionMethod.LandownerSimulated, actor, book);
        Equal(0, stocked.Quote.Costs.MaterialPurchasesCents, "no double charge for bought stock");
        Equal(100, stocked.MaterialsConsumed[0].Quantity.Amount, "owned material use reported");
        var noStock = Actor(); noStock.Stock = Array.Empty<AvailableMaterial>();
        Ineligible("missing-stock", plant, WorkExecutionMethod.LandownerSimulated, noStock, book, RequirementIssue.InsufficientStock);

        Ineligible("owner-production-prohibited", Harvest(200), WorkExecutionMethod.LandownerSimulated, actor, book, RequirementIssue.MethodProhibited);
        var chainsawOwner = Actor(); chainsawOwner.Tools = WorkTool.Chainsaw;
        Ineligible("chainsaw-not-eligibility", Harvest(200), WorkExecutionMethod.LandownerSimulated, chainsawOwner, book, RequirementIssue.MethodProhibited);
        var noTools = Actor(); noTools.Tools = WorkTool.None;
        Ineligible("missing-tools", plant, WorkExecutionMethod.LandownerSimulated, noTools, book, RequirementIssue.MissingTool);
        var noCapability = Actor(); noCapability.Capabilities = WorkCapability.None;
        Ineligible("missing-capability", plant, WorkExecutionMethod.LandownerSimulated, noCapability, book, RequirementIssue.MissingCapability);
        var noTime = Actor(); noTime.AvailableOwnerMinutes = 59;
        Ineligible("insufficient-owner-time", plant, WorkExecutionMethod.LandownerSimulated, noTime, book, RequirementIssue.InsufficientLabour);
        var noContractor = Actor(); noContractor.ContractorAvailable = false;
        Ineligible("contractor-unavailable", Harvest(200), WorkExecutionMethod.Contractor, noContractor, book, RequirementIssue.ContractorUnavailable);
        var noCash = Actor(); noCash.AvailableCashCents = 249999;
        Ineligible("cash-before-revenue", Harvest(200), WorkExecutionMethod.Contractor, noCash, book, RequirementIssue.InsufficientCash);

        var zero = Harvest(0); zero.Quantity.Amount = 0; zero.Timber = Array.Empty<TimberBatch>();
        var zeroResult = Resolve("zero-job", zero, WorkExecutionMethod.Contractor, new ExecutionResources(), book);
        Equal(0, zeroResult.ExternalCashFlowCents, "no zero-volume minimum");
        Equal(0, zeroResult.Ledger.Length, "zero ledger");
        Equal(0, zeroResult.MaterialOutputs.Length, "zero outputs");
        Equal(0, zeroResult.OwnerMinutesConsumed, "zero time");
        zero.Timber = new[] { Batch("zero", TimberAssortment.Sawlog, 0) };
        Equal(0, Resolve("zero-batch", zero, WorkExecutionMethod.Contractor, new ExecutionResources(), book).MaterialOutputs.Length, "no zero material award");

        var inspect = Task(ForestryTaskType.Inspection, WorkQuantityUnit.SquareMetres, 10000);
        inspect.TargetIds = new[] { "stand-B", "stand-A" };
        var inspectOwner = Resolve("inspect-owner", inspect, WorkExecutionMethod.LandownerSimulated, actor, book);
        var inspectContractor = Resolve("inspect-contractor", inspect, WorkExecutionMethod.Contractor, actor, book);
        Check(Json(inspectOwner.Quote.Operation) == Json(inspectContractor.Quote.Operation), "same world meaning across methods");
        Equal(180, inspectOwner.OwnerMinutesConsumed, "inspection 3 h/ha");
        Equal(0, inspectContractor.OwnerMinutesConsumed, "contractor no owner time burden");
        Equal(13500, inspectContractor.Quote.Costs.ContractorWorkCents, "inspection €135 calibrated fee");
        Check(inspectOwner.ExternalCashFlowCents != inspectContractor.ExternalCashFlowCents, "same meaning, different accounting");
        book.FindProfile(ForestryTaskType.Inspection).ContractorLeadDays = 14;
        var lead = Resolve("lead-time", inspect, WorkExecutionMethod.Contractor, actor, book);
        Equal(14, lead.Quote.Requirements.LeadDays, "configurable logistics delay");

        var changed = Stage1EconomyDefaults.Create(); changed.MinimumHarvestJobCents.Selected = 150000;
        Equal(150000, Resolve("minimum-sensitivity", Harvest(10), WorkExecutionMethod.Contractor, actor, changed).Quote.Costs.TotalExternalCostCents, "minimum changed through data");
        changed.FindTimberPrice("sitka-sawlog").RoadsideCentsPerTonne.Selected = 10000;
        Equal(2000000, Resolve("price-change", Harvest(200), WorkExecutionMethod.Contractor, actor, changed).Quote.TimberSaleRevenueCents, "price changed through data");
        var site = Harvest(200); site.SiteCostBasisPoints = 11500;
        Equal(483000, Resolve("site-cost", site, WorkExecutionMethod.Contractor, actor, book).Quote.Costs.ContractorWorkCents, "1.15 cost multiplier");
        foreach (var context in new[] { HarvestOperationContext.SecondThinning, HarvestOperationContext.LaterThinning, HarvestOperationContext.Clearfell })
        {
            var contextTask = Harvest(200); contextTask.HarvestContext = context;
            long expected = context == HarvestOperationContext.SecondThinning ? 460000 : context == HarvestOperationContext.LaterThinning ? 400000 : 300000;
            Equal(expected, Resolve("context-" + context, contextTask, WorkExecutionMethod.Contractor, actor, book).Quote.Costs.ContractorWorkCents, "operation context cost only");
        }

        var split = Stage1EconomyDefaults.Create(); var schedule = split.FindHarvestSchedule(HarvestOperationContext.FirstThinning);
        schedule.Basis = HarvestCostBasis.SeparateHarvestingAndForwarding;
        schedule.HarvestingCentsPerTonne.Selected = 1300; schedule.ForwardingCentsPerTonne.Selected = 800; // Fixture quotes, not default evidence.
        var splitResult = Resolve("split-rates", Harvest(200), WorkExecutionMethod.Contractor, actor, split);
        Equal(260000, splitResult.Quote.Costs.HarvestingCents, "split harvest €2600");
        Equal(160000, splitResult.Quote.Costs.ForwardingCents, "split forward €1600");
        Equal(0, splitResult.Quote.Costs.CombinedHarvestingForwardingCents, "combined not added to split");

        var deadwood = Harvest(200); deadwood.Timber[0].Disposition = TimberDisposition.RetainAsFallenDeadwood; deadwood.Timber[0].ExtractToRoadside = false;
        Ineligible("fell-only-needs-quote", deadwood, WorkExecutionMethod.Contractor, actor, book, RequirementIssue.IncompleteHarvestPricing);
        var deadResult = Resolve("deadwood-split", deadwood, WorkExecutionMethod.Contractor, actor, split);
        Equal(0, deadResult.Quote.TimberSaleRevenueCents, "deadwood not sold");
        Equal(0, deadResult.Quote.Costs.ForwardingCents, "deadwood not forwarded");
        Equal(260000, deadResult.Quote.Costs.ContractorWorkCents, "deadwood felling is still work");
        Equal(1900000, deadResult.Quote.DeadwoodReferenceValueCents, "reference is not cash or a fabricated opportunity charge");
        deadwood.HasHarvestQuote = true;
        deadwood.HarvestQuote = new QuotedHarvestCosts { HarvestingCents = 5000, Source = "Fixture final professional fell-only quote, includes mobilisation" };
        var finalQuote = Resolve("explicit-fell-only-quote", deadwood, WorkExecutionMethod.Contractor, actor, book);
        Equal(5000, finalQuote.Quote.Costs.ContractorWorkCents, "explicit final quote not given an extra machine floor");
        Check(finalQuote.Quote.HarvestPricingSource == deadwood.HarvestQuote.Source, "final quotation provenance retained");
        Check(Json(finalQuote) == Json(ForestryWorkCalculator.Resolve(Roundtrip(deadwood), WorkExecutionMethod.Contractor, actor, Roundtrip(book))), "enabled final quote roundtrip");
        var partial = ForestryWorkCalculator.Quote(new ForestryTask { TaskId = "partly-retained", WorldOperationId = "plan-mixed",
            Type = ForestryTaskType.Harvest, Quantity = new WorkQuantity { Amount = 2 }, Timber = new[] {
                Batch("sold", TimberAssortment.Sawlog, 10), new TimberBatch { BatchId = "left", SourceTreeId = "TREE002",
                    SpeciesId = "sitka-spruce", RoadsidePriceId = "sitka-pulp", Assortment = TimberAssortment.Pulp, Quantity = Tonne,
                    Disposition = TimberDisposition.RetainAsFallenDeadwood } } }, WorkExecutionMethod.Contractor, actor, book);
        Check(!partial.PricingComplete && !partial.Eligible, "mixed fell-only cost requires quote");
        Equal(0, partial.NetCashCents, "unpriced job does not advertise net receipts as profit");
        Equal(0, partial.EstimatedLedger.Length, "unpriced job has no settlement preview");

        var volume = Harvest(1); volume.Timber[0].Unit = TimberQuantityUnit.CubicCentimetres; volume.Timber[0].Quantity = 1000000; volume.Timber[0].DensityId = "sitka-fresh-roadside";
        var converted = Resolve("volume-conversion", volume, WorkExecutionMethod.Contractor, actor, book);
        Equal(890000, converted.MaterialOutputs[0].GreenGrams, "1 m³ fresh Sitka -> .89 t");
        Equal(8455, converted.Quote.TimberSaleRevenueCents, ".89 t at €95/t");
        Equal(1000000, converted.MaterialOutputs[0].Batch.Quantity, "original biological volume preserved");
        var mixedUnits = Roundtrip(volume); mixedUnits.Timber = new[] { mixedUnits.Timber[0], Batch("measured-mass", TimberAssortment.Sawlog, 1) };
        var mixedUnitResult = Resolve("mixed-mass-volume", mixedUnits, WorkExecutionMethod.Contractor, actor, book);
        Equal(17955, mixedUnitResult.Quote.TimberSaleRevenueCents, "mass and volume converted before valuation");
        Equal(3969, mixedUnitResult.Quote.Costs.CombinedHarvestingForwardingCents, "1.89 t processed once");
        var tiny = Harvest(1); tiny.Timber[0] = Batch("round", TimberAssortment.Pulp, 0); tiny.Timber[0].Quantity = 125;
        var roundingBook = Stage1EconomyDefaults.Create(); roundingBook.FindTimberPrice("sitka-pulp").RoadsideCentsPerTonne.Selected = 4000;
        Equal(1, Resolve("cent-rounding", tiny, WorkExecutionMethod.Contractor, actor, roundingBook).Quote.TimberSaleRevenueCents, "half-cent rounds away from zero");

        // Repeated evaluation and order changes must not consume resources or change snapshots.
        var baseline = Json(ForestryWorkCalculator.Resolve(mixedTask, WorkExecutionMethod.Contractor, actor, book));
        Array.Reverse(mixedTask.Timber);
        Check(baseline == Json(ForestryWorkCalculator.Resolve(mixedTask, WorkExecutionMethod.Contractor, actor, book)), "input batch order independent");
        for (int i = 0; i < 100; i++) Check(baseline == Json(ForestryWorkCalculator.Resolve(mixedTask, WorkExecutionMethod.Contractor, actor, book)), "100 identical resolutions");
        Equal(100, actor.Stock[0].Quantity.Amount, "stock not mutated by calculation");
        var savedSnapshot = Json(mixed);
        mixedTask.Timber[0].Quantity = 1; book.FindTimberPrice("sitka-pulp").RoadsideCentsPerTonne.Selected = 1;
        Check(savedSnapshot == Json(mixed), "prior result isolated from input/config mutation");
        book = Stage1EconomyDefaults.Create();

        // DTO roundtrip uses JsonUtility in Unity, System.Text.Json in the standalone harness.
        var taskCopy = Roundtrip(Harvest(200)); var bookCopy = Roundtrip(book); var actorCopy = Roundtrip(actor);
        Check(!taskCopy.HasHarvestQuote, "disabled quote stays disabled through Unity inline-class serialization");
        var roundtripped = Resolve("dto-roundtrip", taskCopy, WorkExecutionMethod.Contractor, actorCopy, bookCopy);
        Check(Json(roundtripped) == Json(Roundtrip(roundtripped)), "resolution serialization roundtrip");
        Check(Json(roundtripped) == Json(ForestryWorkCalculator.Resolve(Harvest(200), WorkExecutionMethod.Contractor, actor, book)), "serialized inputs retain exact calculation");
        var hugeMoney = Roundtrip(new EconomicLedgerEntry { CashDeltaCents = 9007199254740993L });
        Equal(9007199254740993L, hugeMoney.CashDeltaCents, "integer cents survive beyond binary double exact range");

        // Ordinary owner task profiles and supplied material quotes are configurable, not a hidden inventory.
        foreach (var type in new[] { ForestryTaskType.Measurement, ForestryTaskType.Marking, ForestryTaskType.Pruning, ForestryTaskType.Monitoring })
        {
            var profile = book.FindProfile(type); var work = Task(type, profile.QuantityUnit, profile.ReferenceQuantity);
            Check(Resolve("owner-" + type, work, WorkExecutionMethod.LandownerSimulated, actor, book).Resolved, "owner task eligible " + type);
        }
        var shelterPrice = new MaterialPrice { Id = "quoted-shelter", Category = MaterialCategory.Shelter, QuantityUnit = WorkQuantityUnit.Items,
            CentsPerReferenceQuantity = Roundtrip(book.Materials[0].CentsPerReferenceQuantity) };
        shelterPrice.CentsPerReferenceQuantity.Selected = 300;
        var fencePrice = new MaterialPrice { Id = "quoted-fence-wire", Category = MaterialCategory.FenceMaterial, QuantityUnit = WorkQuantityUnit.Millimetres, ReferenceQuantity = 1000,
            CentsPerReferenceQuantity = Roundtrip(shelterPrice.CentsPerReferenceQuantity) };
        fencePrice.CentsPerReferenceQuantity.Selected = 200;
        var materials = new List<MaterialPrice>(book.Materials) { shelterPrice, fencePrice }; book.Materials = materials.ToArray();
        var shelter = Task(ForestryTaskType.ShelterInstallation, WorkQuantityUnit.Items, 45); shelter.Materials = new[] { Material("quoted-shelter", 45, MaterialSupply.PurchaseForJob) };
        Equal(60, Resolve("owner-shelters", shelter, WorkExecutionMethod.LandownerSimulated, actor, book).OwnerMinutesConsumed, "shelter productivity");
        var fence = Task(ForestryTaskType.Fencing, WorkQuantityUnit.Millimetres, 80000); fence.Materials = new[] { Material("quoted-fence-wire", 80000, MaterialSupply.PurchaseForJob, WorkQuantityUnit.Millimetres) };
        var fenceResult = Resolve("owner-fence", fence, WorkExecutionMethod.LandownerSimulated, actor, book);
        Equal(480, fenceResult.OwnerMinutesConsumed, "80 m fence eight calibrated hours");
        Equal(16000, fenceResult.Quote.Costs.MaterialPurchasesCents, "explicit fence materials, no grant-as-invoice");
        Ineligible("fence-needs-materials", Task(ForestryTaskType.Fencing, WorkQuantityUnit.Millimetres, 80000), WorkExecutionMethod.LandownerSimulated, actor, book, RequirementIssue.MissingMaterialSpecification);
        var prohibitedContractor = Roundtrip(book); prohibitedContractor.FindProfile(ForestryTaskType.Inspection).ContractorAllowed = false;
        Ineligible("contractor-policy", inspect, WorkExecutionMethod.Contractor, actor, prohibitedContractor, RequirementIssue.MethodProhibited);
        var slowPruning = Stage1EconomyDefaults.Create(); slowPruning.FindProfile(ForestryTaskType.Pruning).MinutesPerReferenceQuantity.Selected = 90;
        Equal(90, Resolve("productivity-config", Task(ForestryTaskType.Pruning, WorkQuantityUnit.Items, 15), WorkExecutionMethod.LandownerSimulated, actor, slowPruning).OwnerMinutesConsumed, "productivity changes through data");
        var roundedLabour = Resolve("fractional-labour", Task(ForestryTaskType.Pruning, WorkQuantityUnit.Items, 1), WorkExecutionMethod.LandownerSimulated, actor, book);
        Equal(4, roundedLabour.OwnerMinutesConsumed, "labour rounds upward, never free fractional jobs");
        var lineIsolation = ForestryWorkCalculator.Resolve(Harvest(1), WorkExecutionMethod.Contractor, actor, book);
        var realized = lineIsolation.Ledger[0].CashDeltaCents;
        lineIsolation.Quote.EstimatedLedger[0].CashDeltaCents = -123;
        Equal(realized, lineIsolation.Ledger[0].CashDeltaCents, "realized ledger independent of quote DTO");

        // Both sides of the small-job boundary and tiny positive quantities keep the calibrated floor.
        foreach (var grams in new[] { 1L, 125L, 999L, 100000L, Tonne, 25000000L, 119047619L, 119047620L, 500000000L })
        {
            var threshold = Harvest(1); threshold.Timber[0].Quantity = grams;
            var r = Resolve("mass-boundary-" + grams, threshold, WorkExecutionMethod.Contractor, actor, book);
            Check(r.Quote.Costs.ContractorWorkCents >= 250000, "positive commissioned harvest floor");
            if (grams <= 119047620L) Equal(250000, r.Quote.Costs.ContractorWorkCents, "cent-rounded minimum threshold");
            if (grams == 500000000L) Equal(1050000, r.Quote.Costs.ContractorWorkCents, "500 t above floor");
        }

        // Malformed inputs never create negative quantities, phantom sales or wrapped money.
        Invalid("negative-quantity", () => { var t = Harvest(1); t.Timber[0].Quantity = -1; ForestryWorkCalculator.Quote(t, WorkExecutionMethod.Contractor, actor, book); });
        Invalid("negative-price", () => { var b = Stage1EconomyDefaults.Create(); b.MinimumHarvestJobCents.Selected = -1; ForestryWorkCalculator.Quote(Harvest(1), WorkExecutionMethod.Contractor, actor, b); });
        Invalid("duplicate-batch", () => { var t = Harvest(1); t.Timber = new[] { t.Timber[0], t.Timber[0] }; ForestryWorkCalculator.Quote(t, WorkExecutionMethod.Contractor, actor, book); });
        Invalid("duplicate-stock-requirement", () => { var t = Roundtrip(plant); t.Materials = new[] { t.Materials[0], t.Materials[0] }; ForestryWorkCalculator.Quote(t, WorkExecutionMethod.Contractor, actor, book); });
        Invalid("unknown-enum", () => ForestryWorkCalculator.Quote(Harvest(1), (WorkExecutionMethod)99, actor, book));
        Invalid("species-price-mismatch", () => { var t = Harvest(1); t.Timber[0].SpeciesId = "beech"; ForestryWorkCalculator.Quote(t, WorkExecutionMethod.Contractor, actor, book); });
        Invalid("missing-density", () => { var t = Roundtrip(volume); t.Timber[0].DensityId = "dry-unquoted"; ForestryWorkCalculator.Quote(t, WorkExecutionMethod.Contractor, actor, book); });
        Invalid("wrong-unit", () => { var t = Harvest(1); t.Quantity.Unit = WorkQuantityUnit.Millimetres; ForestryWorkCalculator.Quote(t, WorkExecutionMethod.Contractor, actor, book); });
        Invalid("sale-without-extraction", () => { var t = Harvest(1); t.Timber[0].ExtractToRoadside = false; ForestryWorkCalculator.Quote(t, WorkExecutionMethod.Contractor, actor, book); });
        Invalid("zero-work-phantom-output", () => { var t = Harvest(1); t.Quantity.Amount = 0; ForestryWorkCalculator.Quote(t, WorkExecutionMethod.Contractor, actor, book); });
        Invalid("missing-plant-stock-definition", () => ForestryWorkCalculator.Quote(Task(ForestryTaskType.Planting, WorkQuantityUnit.Items, 1), WorkExecutionMethod.Contractor, actor, book));
        Invalid("null-row", () => { var b = Stage1EconomyDefaults.Create(); b.Materials[0] = null; ForestryWorkCalculator.Quote(Harvest(1), WorkExecutionMethod.Contractor, actor, b); });
        Overflow("accounting-overflow", () => { var b = Stage1EconomyDefaults.Create(); b.FindTimberPrice("sitka-sawlog").RoadsideCentsPerTonne.Selected = long.MaxValue; var t = Harvest(1); t.Timber[0].Quantity = long.MaxValue; ForestryWorkCalculator.Quote(t, WorkExecutionMethod.Contractor, actor, b); });

        // Culture changes cannot alter the calculation or machine serialization.
        var culture = CultureInfo.CurrentCulture;
        try
        {
            var text = Json(ForestryWorkCalculator.Resolve(Harvest(200), WorkExecutionMethod.Contractor, actor, book));
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            Check(text == Json(ForestryWorkCalculator.Resolve(Harvest(200), WorkExecutionMethod.Contractor, actor, book)), "culture-independent");
        }
        finally { CultureInfo.CurrentCulture = culture; }
        string digest;
        using (var sha = SHA256.Create()) digest = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(string.Join("\n", Evidence)))).Replace("-", "");
        Log("STAGE1_ECONOMY_VERIFY_PASS fixtures=" + Evidence.Count + " assertions=" + assertions + " evidenceSha256=" + digest);
    }

    private static WorkResolution Resolve(string name, ForestryTask task, WorkExecutionMethod method, ExecutionResources actor, ForestryPriceBook book)
    {
        var result = ForestryWorkCalculator.Resolve(task, method, actor, book);
        Check(result.Resolved && result.Quote.PricingComplete, name + " resolved and priced");
        Equal(result.ExternalCashFlowCents, ForestryWorkCalculator.SumLedger(result.Ledger), name + " reconciles");
        Check(result.Quote.Costs.TotalExternalCostCents >= 0 && result.Quote.TimberSaleRevenueCents >= 0, name + " nonnegative costs/revenue");
        Evidence.Add(name + ":" + Json(result)); return result;
    }
    private static void Ineligible(string name, ForestryTask task, WorkExecutionMethod method, ExecutionResources actor, ForestryPriceBook book, RequirementIssue issue)
    {
        var result = ForestryWorkCalculator.Resolve(task, method, actor, book);
        Check(!result.Resolved && !result.Quote.Eligible, name + " ineligible");
        Check(Array.Exists(result.Quote.UnmetRequirements, x => x.Issue == issue), name + " reason");
        Equal(0, result.ExternalCashFlowCents, name + " no payment");
        Equal(0, result.Ledger.Length, name + " no realized ledger");
        Equal(0, result.MaterialOutputs.Length, name + " no award");
        Equal(0, result.MaterialsConsumed.Length, name + " no consumption");
        Equal(0, result.OwnerMinutesConsumed, name + " no time consumed");
        Evidence.Add(name + ":" + Json(result));
    }
    private static ForestryTask Harvest(long tonnes) => new ForestryTask { TaskId = "job-001", WorldOperationId = "plan-001-harvest",
        Type = ForestryTaskType.Harvest, Quantity = new WorkQuantity { Unit = WorkQuantityUnit.Items, Amount = 1 },
        TargetIds = new[] { "TREE001" }, Timber = new[] { Batch("saw", TimberAssortment.Sawlog, tonnes) } };
    private static ForestryTask Task(ForestryTaskType type, WorkQuantityUnit unit, long amount) => new ForestryTask { TaskId = "job-" + type,
        WorldOperationId = "plan-" + type, Type = type, Quantity = new WorkQuantity { Unit = unit, Amount = amount } };
    private static TimberBatch Batch(string id, TimberAssortment assortment, long tonnes, TimberDisposition disposition = TimberDisposition.SellAndExtract) => new TimberBatch {
        BatchId = id, SourceTreeId = "TREE001", SpeciesId = "sitka-spruce", Assortment = assortment, Unit = TimberQuantityUnit.GreenGrams, Quantity = checked(tonnes * Tonne),
        RoadsidePriceId = "sitka-" + assortment.ToString().ToLowerInvariant(), Disposition = disposition, ExtractToRoadside = true };
    private static MaterialRequirement Material(string id, long quantity, MaterialSupply supply, WorkQuantityUnit unit = WorkQuantityUnit.Items) =>
        new MaterialRequirement { MaterialId = id, Supply = supply, Quantity = new WorkQuantity { Unit = unit, Amount = quantity } };
    private static ExecutionResources Actor() => new ExecutionResources { Capabilities = (WorkCapability)63, Tools = (WorkTool)511,
        ContractorAvailable = true, AvailableCashCents = 100000000, AvailableOwnerMinutes = 100000,
        Stock = new[] { new AvailableMaterial { MaterialId = "beech-sapling", Quantity = new WorkQuantity { Unit = WorkQuantityUnit.Items, Amount = 100 } } } };
    private static void Check(bool value, string message) { assertions++; if (!value) throw new Exception("FAILED: " + message); }
    private static void Equal(long expected, long actual, string message) => Check(expected == actual, message + ": expected " + expected + ", got " + actual);
    private static void Invalid(string name, Action action) { bool rejected = false; try { action(); } catch (ArgumentException) { rejected = true; } Check(rejected, name); Evidence.Add(name + ":REJECTED"); }
    private static void Overflow(string name, Action action) { bool rejected = false; try { action(); } catch (OverflowException) { rejected = true; } Check(rejected, name); Evidence.Add(name + ":REJECTED"); }
    private static string Json<T>(T value)
    {
#if UNITY_5_3_OR_NEWER
        return UnityEngine.JsonUtility.ToJson(value);
#else
        return JsonSerializer.Serialize(value, new JsonSerializerOptions { IncludeFields = true });
#endif
    }
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
