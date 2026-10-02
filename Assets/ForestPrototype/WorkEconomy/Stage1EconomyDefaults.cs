namespace CCF.Forestry.WorkEconomy
{
    // Only this replaceable configuration factory contains economic/labour defaults.
    // Evidence and selected scenario values are distinct; this is not a 2026 price quotation.
    public static class Stage1EconomyDefaults
    {
        private const string Report = "Irish Forestry Economics, Labour and Contractor Operations for CCF Stage 1.pdf";

        public static ForestryPriceBook Create()
        {
            return new ForestryPriceBook
            {
                Id = "stage1-irish-sitka-2024-anchor-v1",
                TimberPrices = new[]
                {
                    Timber("sitka-pulp", TimberAssortment.Pulp, 3600, 3800, 4000),
                    Timber("sitka-stake", TimberAssortment.Stake, 4200, 4700, 5200),
                    Timber("sitka-pallet", TimberAssortment.Pallet, 4800, 6800, 7500),
                    Timber("sitka-sawlog", TimberAssortment.Sawlog, 8800, 9500, 10500)
                },
                Densities = new[]
                {
                    new GreenTimberDensity
                    {
                        Id = "sitka-fresh-roadside", SpeciesId = "sitka-spruce", MoistureCondition = "Fresh roadside logs; not dry timber",
                        KilogramsPerCubicMetre = Parameter(750, 890, 950, ParameterUnit.KilogramsPerCubicMetre,
                            EvidenceLabel.GameplayCalibration, EvidenceLabel.Inference, 2023,
                            "Report pp.19–20; Irish mill-delivery study ~890 kg/m³ [E]; sensitivity range [C]")
                    }
                },
                HarvestSchedules = new[]
                {
                    Harvest(HarvestOperationContext.FirstThinning, 2000, 2100, 2200),
                    Harvest(HarvestOperationContext.SecondThinning, 2200, 2300, 2400),
                    Harvest(HarvestOperationContext.LaterThinning, 2000, 2000, 2000),
                    Harvest(HarvestOperationContext.Clearfell, 1400, 1500, 1600)
                },
                MinimumHarvestJobCents = Parameter(150000, 250000, 400000, ParameterUnit.CentsPerJob,
                    EvidenceLabel.GameplayCalibration, EvidenceLabel.GameplayCalibration, 0,
                    "Report pp.2,20,23: calibration, no robust Irish minimum tariff found"),
                HaulageCentsPerTonne = Parameter(1200, 1200, 1200, ParameterUnit.CentsPerTonne,
                    EvidenceLabel.Empirical, EvidenceLabel.Empirical, 2024, "Report p.7; IFA ~€12/t; apply only if owner bears transport"),
                ContractorLabourCentsPerHour = Parameter(4500, 4500, 4500, ParameterUnit.CentsPerHour,
                    EvidenceLabel.GameplayCalibration, EvidenceLabel.GameplayCalibration, 0,
                    "Placeholder for non-harvest work: existing prototype €45/h calibration; report has no national manual tariff"),
                OwnerEquipmentCentsPerHour = Parameter(0, 0, 0, ParameterUnit.CentsPerHour,
                    EvidenceLabel.SimulationAbstraction, EvidenceLabel.SimulationAbstraction, 0, "Unquoted v1 equipment operating cost; configure per scenario"),
                OwnerTimeValueCentsPerHour = Parameter(0, 0, 0, ParameterUnit.CentsPerHour,
                    EvidenceLabel.SimulationAbstraction, EvidenceLabel.SimulationAbstraction, 0, "Report p.9: time tracked physically, optional non-cash shadow price"),
                Materials = new[]
                {
                    Plant("sitka-sapling", 45, 45, 45),
                    Plant("beech-sapling", 95, 95, 120),
                    Plant("sessile-oak-sapling", 100, 100, 100)
                },
                WorkProfiles = new[]
                {
                    Profile(ForestryTaskType.Inspection, WorkQuantityUnit.SquareMetres, 10000, 120, 180, 240, WorkCapability.BasicManagement, WorkTool.InspectionEquipment, EvidenceLabel.GameplayCalibration),
                    Profile(ForestryTaskType.Measurement, WorkQuantityUnit.SquareMetres, 10000, 120, 180, 240, WorkCapability.BasicManagement, WorkTool.MeasuringTools, EvidenceLabel.GameplayCalibration),
                    Profile(ForestryTaskType.Marking, WorkQuantityUnit.SquareMetres, 10000, 360, 600, 900, WorkCapability.TreeMarking, WorkTool.MarkingEquipment, EvidenceLabel.Inference),
                    // Harvest production time is not inferred from €/t. Machinery costing is separate.
                    Profile(ForestryTaskType.Harvest, WorkQuantityUnit.Items, 1, 0, 0, 0, WorkCapability.ProductionHarvesting, WorkTool.HarvestingSystem, EvidenceLabel.SimulationAbstraction, false),
                    Profile(ForestryTaskType.Planting, WorkQuantityUnit.Items, 100, 34, 60, 100, WorkCapability.Planting, WorkTool.PlantingTools, EvidenceLabel.GameplayCalibration),
                    Profile(ForestryTaskType.ShelterInstallation, WorkQuantityUnit.Items, 45, 45, 60, 90, WorkCapability.Protection, WorkTool.ProtectionTools, EvidenceLabel.GameplayCalibration),
                    Profile(ForestryTaskType.Fencing, WorkQuantityUnit.Millimetres, 80000, 256, 480, 960, WorkCapability.Protection, WorkTool.FencingTools, EvidenceLabel.GameplayCalibration),
                    Profile(ForestryTaskType.Pruning, WorkQuantityUnit.Items, 15, 45, 60, 75, WorkCapability.LowRiskPruning, WorkTool.PruningTools, EvidenceLabel.GameplayCalibration),
                    Profile(ForestryTaskType.Monitoring, WorkQuantityUnit.SquareMetres, 10000, 120, 180, 240, WorkCapability.BasicManagement, WorkTool.InspectionEquipment, EvidenceLabel.GameplayCalibration)
                }
            };
        }

        private static TimberPrice Timber(string id, TimberAssortment assortment, long low, long reference, long high)
        {
            return new TimberPrice { Id = id, SpeciesId = "sitka-spruce", Assortment = assortment,
                RoadsideCentsPerTonne = Parameter(low, reference, high, ParameterUnit.CentsPerTonne,
                    EvidenceLabel.Empirical, EvidenceLabel.GameplayCalibration, 2024, "Report pp.5–6,20: IFA roadside ex VAT; generic selected price [C]") };
        }

        private static HarvestCostSchedule Harvest(HarvestOperationContext context, long low, long reference, long high)
        {
            return new HarvestCostSchedule { Context = context, Basis = HarvestCostBasis.CombinedHarvestingAndForwarding,
                CombinedCentsPerTonne = Parameter(low, reference, high, ParameterUnit.CentsPerTonne,
                    EvidenceLabel.Empirical, EvidenceLabel.Empirical, 2024, "Report p.7: IFA combined harvest + forward, not two additive tariffs"),
                HarvestingCentsPerTonne = Parameter(0, 0, 0, ParameterUnit.CentsPerTonne, EvidenceLabel.SimulationAbstraction, EvidenceLabel.SimulationAbstraction, 0, "Unused in combined mode; needs a split quote"),
                ForwardingCentsPerTonne = Parameter(0, 0, 0, ParameterUnit.CentsPerTonne, EvidenceLabel.SimulationAbstraction, EvidenceLabel.SimulationAbstraction, 0, "Unused in combined mode; needs a split quote") };
        }

        private static MaterialPrice Plant(string id, long low, long reference, long high)
        {
            return new MaterialPrice { Id = id, Category = MaterialCategory.NurseryStock, QuantityUnit = WorkQuantityUnit.Items,
                CentsPerReferenceQuantity = Parameter(low, reference, high, ParameterUnit.CentsPerReferenceQuantity,
                    EvidenceLabel.Empirical, EvidenceLabel.Empirical, 2025, "Report p.10: None-So-Hardy 2025–26 wholesale list, ex VAT, not complete planted cost") };
        }

        private static TaskWorkProfile Profile(ForestryTaskType type, WorkQuantityUnit unit, long referenceQuantity,
            long low, long reference, long high, WorkCapability capability, WorkTool tool, EvidenceLabel label, bool ownerAllowed = true)
        {
            return new TaskWorkProfile { Type = type, QuantityUnit = unit, ReferenceQuantity = referenceQuantity,
                MinutesPerReferenceQuantity = Parameter(low, reference, high, ParameterUnit.MinutesPerReferenceQuantity,
                    label, label, 0, "Report p.9; reciprocal productivity expressed in rounded minutes; fencing assumes 8 h/person-day [C]"),
                LandownerAllowed = ownerAllowed, RequiredCapabilities = capability, RequiredTools = tool,
                RequiresMaterialSpecification = type == ForestryTaskType.Fencing,
                RequiredMaterialCategory = MaterialCategory.FenceMaterial,
                OwnerRestriction = ownerAllowed ? "" : "Commercial production felling is not ordinary owner DIY; chainsaw ownership is insufficient (report pp.8–9 [G])." };
        }

        private static EconomicParameter Parameter(long low, long reference, long high, ParameterUnit unit,
            EvidenceLabel range, EvidenceLabel selected, int year, string source)
        {
            return new EconomicParameter { Low = low, Reference = reference, High = high, Selected = reference, Unit = unit,
                RangeEvidence = range, SelectedEvidence = selected, SourceYear = year, Source = Report + "; " + source };
        }
    }
}
