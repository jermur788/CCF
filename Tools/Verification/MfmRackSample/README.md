# MFM-02 rack-served sample and coverage geometry gate

`MfmRackSampleGeometryVerification.cs` is the disposable Tier-2 Unity gate for MFM-02 (Multi-Scale Forestry Management V1, D-065). It checks
`ScenarioOneRackSampleGeometry` and the V2 entry of the rack-area lesson rules as the live game feeds them: Lessons 2-5 are driven through
the production `LearningObjectivesView.TeachingFacts` path. The decision rules themselves are also checked on plain .NET by
`ScenarioOneNonUi/OfflineLogic/RackSampleGeometryOfflineCheck.cs` (token `NONUI_RACKSAMPLE_PASS`).

Select the mode with `CCF_MFM02_MODE` (each mode is its own Editor process):

| Mode | World | Pass token | What it proves |
|---|---|---|---|
| `v2` | fresh Enlarged80 Rack Planning V2 world, the five qualified headland racks (entrance trees `PO0203 PO1204 PO2003 PO2803 PO3803`, headland x -35.4 to the east edge) | `MFM02_V2_PASS checks=N` | draft/reopened plans have no teaching area; the third rack is selected; anchor, 12 m footprint, fixed 9.0 m half-width and actual corridor exclusion equal an independent re-computation for every living tree; the selection survives a JSON save/load; the same geometry planned in another id order selects the same physical rack; live coverage boundaries; the real Lessons 2-5 chain |
| `model0` | historical model-0 Enlarged80 (regenerated with the rack plan cleared) | `MFM02_MODEL0_PASS checks=N` | same Lessons 2-5 chain on the authored middle line; footprint still the accepted 69 trees / 1.29490 m² with the exact live ~8.0 m authored gap formula (the regenerated rows are 0.009 m inside the older 8.00385 m scene fixture; membership/BA are identical) |
| `legacy40` | fresh Legacy40 game (`CCF_STAND_GEOMETRY=0`) | `MFM02_LEGACY40_PASS checks=N` | still no teaching area |

Measurements (rack ids/endpoints/angles/widths, anchor, footprint tree count and basal area, coverage units and remainders) are logged as
`MFM02_MEASURE` lines and written to `mfm02-<mode>-measurements.txt` in `CCF_ACCEPTANCE_OUTPUT`.

The five-rack V2 fixture is **verification-only**: it reproduces the qualified Rack V2 geometry while the player authoring path (RACK-V2-04/05)
is unfinished. It is not production geometry policy and does not satisfy the later requalification on the real player-authored workflow.

Copy -> run -> remove: copy into `Assets/ForestPrototype`, run `MfmRackSampleGeometryVerification.Begin`, then remove the copy and its `.meta`
(the gate runner stages and removes it). Run under the machine-wide Unity lock in a registered Unity lane. Save safety: the gate refuses to
run unless `Application.persistentDataPath` is inside `XDG_CONFIG_HOME`, and it only saves/loads in memory.
