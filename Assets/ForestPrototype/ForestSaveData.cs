using System;
using System.Collections.Generic;

[Serializable]
public sealed class ForestSaveData
{
    public const int CurrentVersion = 24;

    public int version = CurrentVersion;
    // Carried wood; the field name stays "wood" so version-1 saves keep loading.
    public int wood;
    public int ecologicalYear;
    public int simulationSeed = 20260914;
    // RNG scheme (SimulationRandom). Deliberately defaults to 0, the legacy
    // scheme, so saves written before this field existed replay unchanged.
    public int rngModelVersion;
    // Version 16: regeneration representation (RegenerationModel). Defaults to
    // 0, legacy single-cohort regeneration, so older saves and Reference Future
    // v1 replay unchanged. Under model 1 cells may hold several cohort records
    // of one species: each is an age band keyed by species + origin + year.
    public int regenerationModel;
    // Version 17: adult growth model (GrowthModel). Defaults to 0, legacy
    // growth with no adult mortality, so older saves and Reference Future v1
    // replay unchanged. 1 = Class III Sitka height + adult density mortality.
    public int growthModel;
    // Version 19: independent storm mechanics. Missing legacy fields mean off.
    public int stormModel;
    // Version 20: the authoritative stand geometry (StandGeometryModel): 0 = Legacy40 (40 x 40 m, 64
    // cells), 1 = Enlarged80 (80 x 80 m, 256 cells). Explicit on every v20 save. A missing field and
    // every save before v20 mean Legacy40, so older saves and Reference Future v1 replay unchanged.
    // Every cell-indexed record below is only meaningful against this geometry's grid.
    public int standGeometryModel;
    public List<string> markedTreeIds = new List<string>();
    public List<string> cropTreeIds = new List<string>();
    public List<TreeSaveData> trees = new List<TreeSaveData>();
    public List<BuildableSaveData> buildables = new List<BuildableSaveData>();
    public List<WoodStorageSaveData> storages = new List<WoodStorageSaveData>();
    public List<ForestCellSaveData> cells = new List<ForestCellSaveData>();
    // Version 10: Scenario One management/economy state. Null in legacy saves.
    // Version 11: structured management events within scenarioOne.
    // Version 12: objective outcome, tutorial review and century comparison.
    // Version 13: persistent two-type tree marks, exact-position individual
    // plantings, treatment patches and retained construction timber.
    // Version 14: explicit biological tree mortality, separate from harvesting.
    // Version 15: scenario execution choices, protection and bounded economy reports.
    // Version 21: Rack Planning & Field Marking V2 state inside scenarioOne (D-064).
    // Version 22: objective achievement history inside scenarioOne (S1-C T-05).
    // Version 23: durable multi-scale prescription state and work-order provenance inside scenarioOne (MFM-06).
    public ScenarioOneSaveData scenarioOne;
}

[Serializable]
public sealed class ScenarioOneSaveData
{
    // Version 24: the first guided rack review genuinely completed in-session. Missing means false; never backfilled.
    public bool firstGuidedRackReviewComplete;
    public string scenarioId = "scenario-one";
    public bool initialized;
    public long cashCents;
    public int nextWorkOrderId = 1;
    public int nextManagementEventId = 1;
    public List<ScenarioOneWorkOrder> workOrders = new List<ScenarioOneWorkOrder>();
    public List<ScenarioInventoryEntry> inventory = new List<ScenarioInventoryEntry>();
    public List<ScenarioAnnualReport> annualReports = new List<ScenarioAnnualReport>();
    public List<ScenarioManagementEvent> managementEvents = new List<ScenarioManagementEvent>();
    public List<ScenarioEcologicalSnapshot> ecologicalSnapshots = new List<ScenarioEcologicalSnapshot>();
    public List<ScenarioUnderstoreyCell> understoreyCells = new List<ScenarioUnderstoreyCell>();
    public List<ScenarioDeadwoodRecord> deadwoodRecords = new List<ScenarioDeadwoodRecord>();
    public int nextDeadwoodId = 1;
    public ScenarioOneOutcome outcome;
    public int outcomeYear = -1;
    public string outcomeReason = "";
    public bool annualReviewSeen;
    public ScenarioCenturyReview centuryReview;
    public float retainedTimberM3;
    public List<PlantedJuvenileSaveData> plantedJuveniles = new List<PlantedJuvenileSaveData>();
    public List<PlantingClearancePatch> clearancePatches = new List<PlantingClearancePatch>();
    // Distinguishes the early v13 draft (which duplicated cohorts with inert
    // individual records) from authoritative individual planting v13.
    public int interactionSchemaVersion;
    public List<BrowseShelter> shelters = new List<BrowseShelter>();
    public List<BrowseProtectedArea> protectedAreas = new List<BrowseProtectedArea>();
    public int ownerMinutesUsedThisYear;
    public List<StormEventRecord> stormEvents = new List<StormEventRecord>();
    // Version 21: Rack Planning & Field Marking V2 (D-064). These four fields are declared last and
    // adjacent on purpose: the world-hash compatibility path reproduces the pre-v21 serialized
    // layout by removing exactly this run (and each work order's rackId) while the plan is still
    // the empty default. See ScenarioReferenceArchive.WithoutRackV2Fields.
    // The world/workflow identity, NOT a save-file version: 0 = legacy rack workflow,
    // 1 = Rack Planning & Field Marking V2. A missing field and every save before v21 are model 0.
    public int rackPlanningModel;
    public bool rackPlanConfirmed;
    // The next rack id to hand out; 1 until a rack has been planned.
    public int nextRackId = 1;
    // The authoritative persisted V2 rack-plan records. Empty until the map editor lands.
    public List<RackSaveData> racks = new List<RackSaveData>();
    // Version 22: S1-C objective/progression history. Declared after the v21 run so that the hash
    // compatibility path can drop this one field without touching the rack-plan strip. Append-only:
    // one record per objective, written when evaluation first sees it achieved. A missing or empty list
    // means "not recorded"; nothing is ever back-filled into a save that predates the field. It is not
    // part of the world identity hash (see ScenarioReferenceArchive.WithoutObjectiveAchievements).
    public List<ScenarioObjectiveAchievement> objectiveAchievements = new List<ScenarioObjectiveAchievement>();
    // Version 23 (MFM-06): Multi-Scale Forestry Management prescription state. Declared after the v22 field,
    // adjacent and last, so the hash compatibility path can drop exactly this run while it is the empty
    // default (ScenarioReferenceArchive.WithoutPrescriptionState). A missing field and every save before v23
    // mean "no prescription state": nothing is ever derived from existing marks or work orders.
    // The next prescription id to hand out; 1 until a prescription exists. Ids are never reused.
    public int nextPrescriptionId = 1;
    public List<PrescriptionSaveData> prescriptions = new List<PrescriptionSaveData>();
}

// The ecological year in which Scenario One evaluation first observed one objective as achieved.
// First achievement only: an objective later lost keeps its record, and is never recorded twice.
[Serializable]
public sealed class ScenarioObjectiveAchievement
{
    // The ScenarioObjectiveResult.objectiveId this record belongs to.
    public string objectiveId = "";
    public int achievedYear;
    // The objective's target at the moment it was achieved, so the record stays readable if targets change.
    public float targetValueAtAchievement;
}

// One persisted Rack Planning & Field Marking V2 rack. The entrance and far endpoints are the
// authoritative centreline: angle, length, width, spacing and counters stay derived (packet
// RACK-V2-03B). Fields are declared in this order so the empty-plan compatibility strip and the
// JSON-presence validation stay exact.
[Serializable]
public sealed class RackSaveData
{
    // Stable positive identity; ids are never reused (nextRackId moves past the highest used).
    public int rackId;
    // The rack entrance end of the centreline (world x/z metres).
    public float entranceX;
    public float entranceZ;
    // The far end of the centreline (world x/z metres).
    public float farX;
    public float farZ;
    // The frozen removal set: the rack's trees, frozen when the plan is confirmed. A draft rack
    // may have none yet. Kept even after the trees are felled, so the rack survives its harvest.
    public List<string> removalTreeIds = new List<string>();
    // The entrance tree (first rack-removal tree nearest the access), part of removalTreeIds.
    public string entranceTreeId = "";
    // True once the entrance tree physically carries the orange rack mark.
    public bool entranceMarked;
}

[Serializable]
public sealed class ScenarioInventoryEntry
{
    public string itemId = "";
    public int quantity;
}

[Serializable]
public sealed class ScenarioAnnualReport
{
    public int year;
    public int completedTasks;
    public int failedTasks;
    public long contractorCostCents;
    public long timberRevenueCents;
    public float harvestedVolumeM3;
    public int regenerationRemovalTasks;
    public float removedRegenerationDensity;
    public int deadwoodCreated;
    public float deadwoodCreatedM3;
    public float deadwoodDecayedM3;
    public float keptForUseVolumeM3;
    public long closingCashCents;
    public long harvestMinimumAdjustmentCents;
    public int ownerMinutes;
    public List<ScenarioTimberSale> timberSales = new List<ScenarioTimberSale>();
}

[Serializable]
public sealed class ScenarioTimberSale
{
    public CCF.Forestry.WorkEconomy.TimberAssortment assortment;
    public long soldVolumeCm3;
    public long revenueCents;
}

[Serializable]
public sealed class TreeSaveData
{
    public string treeId = "";
    // Version 7: explicit individual species identity. Legacy saves omit it
    // and are loaded as the scene's default Sitka species.
    public string speciesId = "";
    public int stage;
    public float stageTimer;
    public int chopProgress;
    // Version 3 simulation state; absent in legacy saves.
    public bool hasSimulation;
    public int ageYears;
    // Version 6: recorded history; older saves start recording at zero.
    public float equivalentSuppressedYears;
    public float heightMeters;
    public float diameterCm;
    public float crownRadiusMeters;
    // Version 11: pruning history. Older saves load as unpruned.
    public int pruningLifts;
    public float crownBaseHeightM;
    public int lastPruningYear = -1;
    public int markType;
    // Older schemas omit these fields and restore no biological mortality.
    public bool biologicallyDead;
    public string mortalityCause = "";
    public int mortalityYear = -1;
    public UnityEngine.Vector3 position;
}

[Serializable]
public sealed class PlantedJuvenileSaveData
{
    public string juvenileId = "";
    public string speciesId = "";
    public UnityEngine.Vector3 position;
    public int cellIndex = -1;
    public int plantingYear;
    public float ageYears;
    public float heightMeters;
    public bool alive = true;
    public string stockItemId = "";
    public string promotedTreeId = "";
    public bool legacyCohortManaged;
}

[Serializable]
public sealed class BuildableSaveData
{
    public string buildId = "";
    public bool built;
}

[Serializable]
public sealed class WoodStorageSaveData
{
    public string storageId = "";
    public int storedWood;
}

[Serializable]
public sealed class ForestCellSaveData
{
    public int index;
    // Versions 1-7: one implicit default-Sitka cohort. Retained so JsonUtility
    // can deserialize legacy saves; version 8 writes cohorts instead.
    public float regenDensity;
    public float regenHeight;
    public int regenEstablishYear = -1;
    // Version 8+: explicit species-keyed regeneration state. Seed rain remains derived.
    public List<ForestRegenerationCohortSaveData> cohorts = new List<ForestRegenerationCohortSaveData>();
    public float recentOpening;
    // Smoothed disturbance response; a short history that cannot be rebuilt
    // from the other fields, so it is saved. Older saves carry -1 and the
    // loader reconstructs it deterministically from the opening.
    public float establishmentSuitability = -1f;
}

[Serializable]
public sealed class ForestRegenerationCohortSaveData
{
    public string speciesId = "";
    public float density;
    public float height;
    public int establishYear = -1;
    // Version 9: diagnostic provenance. Legacy cohorts default to Natural.
    public int origin;
    public int originYear = -1;
}

// Durable Multi-Scale Forestry Management prescription state (save v23, MFM-06). Plain data only: this is the
// storage foundation, not the proposal/approval behaviour (MFM-07). Transient proposal visuals are NOT stored;
// they are recomputed deterministically. Approved work lives in ordinary work orders, which carry a prescription
// link (ScenarioOneWorkOrder.prescriptionId).

// Approval progress of a stored prescription. Append-only: stored numbers never change meaning.
public static class PrescriptionApprovalState
{
    // The player has accepted the coverage decisions; no work has been generated.
    public const int Accepted = 1;
    // Prescription approval happened in approvalYear; pending ordinary work orders were generated.
    public const int Approved = 2;

    public static bool IsKnown(int state) => state == Accepted || state == Approved;
}

// A coverage choice the player explicitly made for one coverage unit. Only deliberate player choices are stored;
// every other unit state is recomputed from the stand.
public static class PrescriptionCoverageChoice
{
    public const int AcceptedCovered = 1;
    public const int LeftManual = 2;

    public static bool IsKnown(int choice) => choice == AcceptedCovered || choice == LeftManual;
}

// Closed-vocabulary reason stored on a generated work order. 0 means "none" (an ordinary order); every other number is
// ScenarioOnePrescription.ProposalReason + 1 (kept in step by the MFM-06 offline check). They live here so the save schema and
// its validator stand alone.
public static class PrescriptionReasonCode
{
    public const int None = 0;
    public const int CompetitorHighShareAroundConfirmedCropTree = 1;

    // True for a real reason: an order generated by a prescription must carry one (None is not a reason).
    public static bool IsKnown(int code) => code == CompetitorHighShareAroundConfirmedCropTree;
}

[Serializable]
public sealed class PrescriptionCoverageDecisionSaveData
{
    // ScenarioOneCoverage.Area.Id (or remainder id).
    public string unitId = "";
    // A PrescriptionCoverageChoice value.
    public int choice;
}

[Serializable]
public sealed class PrescriptionSaveData
{
    // Stable positive identity; never reused (nextPrescriptionId moves past the highest used).
    public int prescriptionId;
    // Deterministic rule and calibration identity (ScenarioOnePrescriptionCalibration.RuleVersion / .Version).
    // Stored as text so an old prescription keeps naming the rule that made it.
    public string ruleVersion = "";
    public string calibrationVersion = "";
    // Source sample: the persisted Rack V2 teaching rack, the year it was sampled and its anchor.
    public int sourceRackId;
    public int sourceYear;
    public float sourceAnchorX;
    public float sourceAnchorZ;
    // Derivation inputs that cannot be safely reconstructed later: the sample's Crop Trees and executed removals, by tree id.
    public List<string> sampleCropTreeIds = new List<string>();
    public List<string> sampleRemovalIds = new List<string>();
    public List<PrescriptionCoverageDecisionSaveData> coverageDecisions = new List<PrescriptionCoverageDecisionSaveData>();
    // A PrescriptionApprovalState value.
    public int approvalState;
    // The ecological year of prescription approval; -1 until approved.
    public int approvalYear = -1;
}
