using System;

namespace CCF.Forestry.WorkEconomy
{
    public enum EvidenceLabel { Empirical, ManagementGuidance, Inference, SimulationAbstraction, GameplayCalibration }
    public enum ParameterUnit { CentsPerTonne, CentsPerHour, CentsPerReferenceQuantity, CentsPerJob, MinutesPerReferenceQuantity, KilogramsPerCubicMetre }
    public enum HarvestOperationContext { FirstThinning, SecondThinning, LaterThinning, Clearfell }
    public enum HarvestCostBasis { CombinedHarvestingAndForwarding, SeparateHarvestingAndForwarding }

    [Serializable]
    public sealed class EconomicParameter
    {
        public long Low;
        public long Reference;
        public long High;
        public long Selected; // May depart from evidence range in an explicitly selected scenario.
        public ParameterUnit Unit;
        public EvidenceLabel RangeEvidence;
        public EvidenceLabel SelectedEvidence;
        public string Source = "";
        public int SourceYear;
    }

    [Serializable]
    public sealed class TimberPrice
    {
        public string Id = "";
        public string SpeciesId = "";
        public TimberAssortment Assortment;
        public EconomicParameter RoadsideCentsPerTonne = new EconomicParameter();
    }

    [Serializable]
    public sealed class GreenTimberDensity
    {
        public string Id = "";
        public string SpeciesId = "";
        public string MoistureCondition = "";
        public EconomicParameter KilogramsPerCubicMetre = new EconomicParameter();
    }

    [Serializable]
    public sealed class HarvestCostSchedule
    {
        public HarvestOperationContext Context;
        public HarvestCostBasis Basis;
        public EconomicParameter CombinedCentsPerTonne = new EconomicParameter();
        public EconomicParameter HarvestingCentsPerTonne = new EconomicParameter();
        public EconomicParameter ForwardingCentsPerTonne = new EconomicParameter();
    }

    [Serializable]
    public sealed class MaterialPrice
    {
        public string Id = "";
        public MaterialCategory Category;
        public WorkQuantityUnit QuantityUnit;
        public long ReferenceQuantity = 1;
        public EconomicParameter CentsPerReferenceQuantity = new EconomicParameter();
    }

    [Serializable]
    public sealed class TaskWorkProfile
    {
        public ForestryTaskType Type;
        public WorkQuantityUnit QuantityUnit;
        public long ReferenceQuantity = 1;
        public EconomicParameter MinutesPerReferenceQuantity = new EconomicParameter();
        public bool ContractorAllowed = true;
        public bool LandownerAllowed;
        public string OwnerRestriction = "";
        public WorkCapability RequiredCapabilities;
        public WorkTool RequiredTools;
        public bool RequiresMaterialSpecification;
        public MaterialCategory RequiredMaterialCategory;
        public int ContractorLeadDays;
        public int LandownerLeadDays;
    }

    // Plain serializable scenario data. No MonoBehaviour, singleton, wallet or file IO.
    [Serializable]
    public sealed class ForestryPriceBook
    {
        public string Id = "";
        public TimberPrice[] TimberPrices = Array.Empty<TimberPrice>();
        public GreenTimberDensity[] Densities = Array.Empty<GreenTimberDensity>();
        public HarvestCostSchedule[] HarvestSchedules = Array.Empty<HarvestCostSchedule>();
        public MaterialPrice[] Materials = Array.Empty<MaterialPrice>();
        public TaskWorkProfile[] WorkProfiles = Array.Empty<TaskWorkProfile>();
        public EconomicParameter MinimumHarvestJobCents = new EconomicParameter();
        public EconomicParameter HaulageCentsPerTonne = new EconomicParameter();
        public EconomicParameter ContractorLabourCentsPerHour = new EconomicParameter();
        public EconomicParameter OwnerEquipmentCentsPerHour = new EconomicParameter();
        public EconomicParameter OwnerTimeValueCentsPerHour = new EconomicParameter();

        public TimberPrice FindTimberPrice(string id) => Array.Find(TimberPrices, x => x.Id == id);
        public GreenTimberDensity FindDensity(string id) => Array.Find(Densities, x => x.Id == id);
        public MaterialPrice FindMaterial(string id) => Array.Find(Materials, x => x.Id == id);
        public TaskWorkProfile FindProfile(ForestryTaskType type) => Array.Find(WorkProfiles, x => x.Type == type);
        public HarvestCostSchedule FindHarvestSchedule(HarvestOperationContext context) => Array.Find(HarvestSchedules, x => x.Context == context);
    }
}
