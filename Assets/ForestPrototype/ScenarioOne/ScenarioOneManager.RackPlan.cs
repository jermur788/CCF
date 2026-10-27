using System.Collections.Generic;
using UnityEngine;

// Rack Planning & Field Marking V2 plan state (save v21, D-064). This is the persisted plan the map
// editor and field marking will consume: one workflow identity, the confirmation flag, the id
// allocator and the authoritative rack records. Everything else about a rack (angle, length, spacing,
// validity, lock state, counters, order set, suggested layout, headland) stays derived from these
// records and the live world; nothing here implements the editor, confirm/reconcile or marking yet.
public sealed partial class ScenarioOneManager
{
    [SerializeField] private int rackPlanningModel = RackPlanningModel.Legacy;
    [SerializeField] private bool rackPlanConfirmed;
    [SerializeField] private int nextRackId = 1;
    [SerializeField] private List<RackSaveData> racks = new List<RackSaveData>();

    // Named ModelVersion (like ecology.GrowthModelVersion) so it never shadows the RackPlanningModel constants.
    public int RackPlanningModelVersion => rackPlanningModel;
    public bool RackPlanConfirmed => rackPlanConfirmed;
    public int NextRackId => nextRackId;
    public IReadOnlyList<RackSaveData> Racks => racks;

    // The one runtime answer to "is this a V2 rack world?". True only when the world is Enlarged80 AND
    // its Scenario One state explicitly identifies as model 1. Never inferred from the save version,
    // from the geometry alone, from a non-null rack list, from authored layout lines or from objective
    // progress. Legacy40, Reference Future v1, pre-v21 saves and pre-V2 Enlarged80 continuations are
    // not V2 worlds even though some of their rack code paths still exist.
    public bool RackPlanningV2Active =>
        rackPlanningModel == RackPlanningModel.RackPlanningFieldMarkingV2
        && ecology != null
        && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80;

    // New-game identity, called from the fresh-game setup right after InitializeNewScenario (which is
    // model-neutral because legacy loads call it too). A brand-new Enlarged80 Scenario One game is a
    // V2 world from its first frame - with zero racks, unconfirmed, nextRackId 1. A brand-new Legacy40
    // game stays model 0.
    private void InitializeRackPlanForNewWorld()
    {
        ClearRackPlan();
        if (ecology != null && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80)
            rackPlanningModel = RackPlanningModel.RackPlanningFieldMarkingV2;
    }

    // The empty/legacy plan: model 0, unconfirmed, no racks. RestoreSaveData(null) and every fresh
    // scenario reset land here, so a legacy load can never accidentally activate V2.
    private void ClearRackPlan()
    {
        rackPlanningModel = RackPlanningModel.Legacy;
        rackPlanConfirmed = false;
        nextRackId = 1;
        racks.Clear();
    }
}
