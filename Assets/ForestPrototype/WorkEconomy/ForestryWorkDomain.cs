using System;

namespace CCF.Forestry.WorkEconomy
{
    public enum ForestryTaskType { Inspection, Measurement, Marking, Harvest, Planting, ShelterInstallation, Fencing, Pruning, Monitoring }
    public enum WorkExecutionMethod { Contractor, LandownerSimulated }
    public enum WorkQuantityUnit { Items, SquareMetres, Millimetres }
    public enum TimberQuantityUnit { GreenGrams, CubicCentimetres }
    public enum TimberAssortment { Pulp, Stake, Pallet, Sawlog }
    // Explicit adapter mapping to the existing FellingMaterialOutcome, not an enum cast.
    public enum TimberDisposition { SellAndExtract, RetainAsFallenDeadwood, KeepForUse }
    public enum MaterialSupply { ExistingStock, PurchaseForJob }
    public enum MaterialCategory { Consumable, NurseryStock, Shelter, FenceMaterial }
    [Flags]
    public enum WorkCapability { None = 0, BasicManagement = 1, TreeMarking = 2, Planting = 4, Protection = 8, LowRiskPruning = 16, ProductionHarvesting = 32 }
    [Flags]
    public enum WorkTool { None = 0, InspectionEquipment = 1, MeasuringTools = 2, MarkingEquipment = 4, PlantingTools = 8, ProtectionTools = 16, FencingTools = 32, PruningTools = 64, HarvestingSystem = 128, Chainsaw = 256 }
    public enum RequirementIssue { MethodProhibited, MissingCapability, MissingTool, InsufficientLabour, InsufficientStock, InsufficientCash, ContractorUnavailable, IncompleteHarvestPricing, MissingMaterialSpecification }
    public enum LedgerCategory { TimberSale, HarvestingAndForwarding, Harvesting, Forwarding, MinimumJobAdjustment, ContractorLabour, Materials, OwnerEquipment, Haulage }

    [Serializable]
    public sealed class WorkQuantity
    {
        public WorkQuantityUnit Unit;
        public long Amount;
    }

    [Serializable]
    public sealed class MaterialRequirement
    {
        public string MaterialId = "";
        public WorkQuantity Quantity = new WorkQuantity();
        public MaterialSupply Supply;
    }

    [Serializable]
    public sealed class AvailableMaterial
    {
        public string MaterialId = "";
        public WorkQuantity Quantity = new WorkQuantity();
    }

    [Serializable]
    public sealed class TimberBatch
    {
        public string BatchId = "";
        public string SourceTreeId = "";
        public string SpeciesId = "";
        public TimberAssortment Assortment;
        public TimberQuantityUnit Unit;
        public long Quantity;
        public string DensityId = ""; // Required only for volume-priced input conversion.
        public string RoadsidePriceId = "";
        public TimberDisposition Disposition;
        public bool ExtractToRoadside;
        public bool OwnerPaysHaulage; // Optional separate transport; roadside baseline is false.
    }

    [Serializable]
    public sealed class QuotedHarvestCosts
    {
        public long HarvestingCents;
        public long ForwardingCents;
        public string Source = ""; // Final job quote, includes mobilisation; no additional model floor/multiplier.
    }

    [Serializable]
    public sealed class ForestryTask
    {
        public string TaskId = "";
        public string WorldOperationId = ""; // References the existing authoritative operation/plan.
        public ForestryTaskType Type;
        public WorkQuantity Quantity = new WorkQuantity();
        public string[] TargetIds = Array.Empty<string>();
        public MaterialRequirement[] Materials = Array.Empty<MaterialRequirement>();
        public TimberBatch[] Timber = Array.Empty<TimberBatch>();
        // Work quantity without a market product (residues/unmarketed species). Never sale revenue.
        public long UnpricedHarvestGreenGrams;
        public long UnpricedForwardGreenGrams;
        public HarvestOperationContext HarvestContext;
        public int SiteCostBasisPoints = 10000; // 1.0, cost only, no automatic CCF premium.
        // Unity inline-class serialization does not preserve null as an optional-value marker.
        public bool HasHarvestQuote;
        public QuotedHarvestCosts HarvestQuote = new QuotedHarvestCosts();
    }

    [Serializable]
    public sealed class ExecutionResources
    {
        // These describe the selected actor, including contractor equipment/capability.
        public WorkCapability Capabilities;
        public WorkTool Tools;
        public bool ContractorAvailable;
        public long AvailableCashCents;
        public long AvailableOwnerMinutes;
        public AvailableMaterial[] Stock = Array.Empty<AvailableMaterial>();
    }

    [Serializable]
    public sealed class UnmetRequirement
    {
        public RequirementIssue Issue;
        public string Detail = "";
        public long Required;
        public long Available;
    }

    [Serializable]
    public sealed class WorkRequirements
    {
        public WorkCapability Capabilities;
        public WorkTool Tools;
        public long PersonMinutes;
        public bool PersonMinutesKnown;
        public long OwnerMinutes;
        public long UpfrontCashCents;
        public int LeadDays;
        public MaterialRequirement[] Materials = Array.Empty<MaterialRequirement>();
    }

    [Serializable]
    public sealed class WorldOperationDescription
    {
        public string TaskId = "";
        public string WorldOperationId = "";
        public ForestryTaskType Type;
        public WorkQuantity Quantity = new WorkQuantity();
        public string[] TargetIds = Array.Empty<string>();
    }

    [Serializable]
    public sealed class TimberValuation
    {
        public TimberBatch Batch = new TimberBatch();
        public long GreenGrams;
        public long SelectedRoadsideCentsPerTonne;
        public long RoadsideReferenceValueCents; // Not all of this is cash or net foregone contribution.
        public long SaleRevenueCents;
    }

    [Serializable]
    public sealed class WorkCostBreakdown
    {
        public long CombinedHarvestingForwardingCents;
        public long HarvestingCents;
        public long ForwardingCents;
        public long MinimumJobAdjustmentCents;
        public long ContractorLabourCents;
        public long MaterialPurchasesCents;
        public long OwnerEquipmentCents;
        public long HaulageCents;
        public long TotalExternalCostCents;
        public long ContractorWorkCents; // Work only; excludes materials and haulage.
    }

    [Serializable]
    public sealed class EconomicLedgerEntry
    {
        public LedgerCategory Category;
        public string ReferenceId = "";
        public long CashDeltaCents; // + receipt, - payment. All euros, ex VAT.
    }

    [Serializable]
    public sealed class WorkQuote
    {
        public string PriceBookId = "";
        public WorkExecutionMethod Method;
        public HarvestOperationContext HarvestContext;
        public string HarvestPricingSource = "";
        public int SiteCostBasisPoints;
        public WorldOperationDescription Operation = new WorldOperationDescription();
        public WorkRequirements Requirements = new WorkRequirements();
        public WorkCostBreakdown Costs = new WorkCostBreakdown();
        public TimberValuation[] Timber = Array.Empty<TimberValuation>();
        public UnmetRequirement[] UnmetRequirements = Array.Empty<UnmetRequirement>();
        public EconomicLedgerEntry[] EstimatedLedger = Array.Empty<EconomicLedgerEntry>();
        public bool PricingComplete;
        public bool Eligible;
        public long GrossRoadsideReferenceValueCents;
        public long TimberSaleRevenueCents;
        public long RetainedForUseReferenceValueCents;
        public long DeadwoodReferenceValueCents;
        public long OwnerTimeOpportunityCostCents; // Non-cash; never included in ledger.
        public long NetCashCents;
        public long NetAfterOwnerTimeCents;
    }

    [Serializable]
    public sealed class WorkResolution
    {
        public WorkQuote Quote = new WorkQuote();
        public bool Resolved; // Accounting resolution only: does not execute the world operation.
        public long ExternalCashFlowCents;
        public long OwnerMinutesConsumed;
        public EconomicLedgerEntry[] Ledger = Array.Empty<EconomicLedgerEntry>();
        public TimberValuation[] MaterialOutputs = Array.Empty<TimberValuation>();
        public MaterialRequirement[] MaterialsConsumed = Array.Empty<MaterialRequirement>();
    }
}
