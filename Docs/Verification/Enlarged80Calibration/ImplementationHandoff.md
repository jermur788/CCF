# Enlarged80 area-sensitive calibration — implementation handoff

Task: preserve proportional Scenario One targets for the 80 m property, retain Legacy40/economy/save/reference compatibility, and correct aspirational Century Review wording.
Worker/role: SOL/CODEX, primary implementation worker (user reassigned the Claude-labelled packet).
Branch: `task/enlarged80-area-calibration`.
Base/context: `f4535caf606bca46f35f02408920408d6b9f20d3`.
Final production implementation: `fa9f66dfed1b7f8a24295f379e2bb9e302e84a66`.
Worktree: `/home/jer/CCF-e80-calibration`.
State: implemented on task branch; Manager review/integration remains separate.

## Implementation

ScenarioOneDefinition retains the serialized Legacy40 baseline. Seven effective-target accessors derive the factor centrally from the squared side length of the two accepted geometry models (1 or 4). Unknown geometry is rejected. Objectives, completion and Century Review receive the live restored geometry explicitly; Learning/Objectives and Annual Review use the manager's shared objective results.

Century Review distinguishes the real frozen Reference Future from `aspirational-design-targets`. The fallback says “Compared with Scenario One aspirational design targets: not a forecast, optimum or prescription” and labels rows “target”. Real frozen comparisons retain their existing wording and “reference” label.

Restoring a pre-calibration Enlarged80 aspirational review refreshes its extensive target values and achieved flags through the same effective-target layer. Original measured values, year, outcome and management comparisons are preserved. The saved input record is cloned rather than mutated. Legacy40 cached reviews and frozen-reference cached reviews remain byte-equivalent. The new gate reproduced the stale-target failure before the fix and verifies the corrected restore boundary.

| Target | Legacy40 | Enlarged80 |
|---|---:|---:|
| Minimum retained original-species trees | 60 | 240 |
| Minimum regenerating cells | 3 | 12 |
| Minimum fallen deadwood, m³ | 0.02 | 0.08 |
| Aspirational original-species trees | 120 | 480 |
| Aspirational broadleaf presence, each planted species | 10 | 40 |
| Aspirational regenerating cells | 12 | 48 |
| Aspirational deadwood, m³ | 0.5 | 2.0 |

Unchanged in both geometries: canopy minimum 0.35, aspirational canopy 0.65, completion horizon year 25, century year 100, starting cash €12,000, minimum commissioned harvest €2,500 per annual job, owner capacity 2,400 minutes/year, contractor/per-tree labour, stock/shelter prices, clearance labour and timber prices. This is gameplay continuity calibration, not empirical forestry thresholds. Save v20, definition `scenario-one-v13`, starting geometry/stocking and frozen Reference Future v1 remain unchanged.

## Viability and economy

Two fresh final-code Editor processes produce identical results:

| Measure | Result |
|---|---:|
| Completion | Completed |
| Completion year | 25 |
| Minimum cash | €11,451.16 |
| Intervention years | 3 |
| Trees felled | 462 |
| Total reported owner minutes | 24 |
| Oak / Beech planting | 8 / 8 |
| Final original-species living trees | 793 |
| Final regenerating cells | 232 |
| Final deadwood | 58.73983 m³ |
| Final mean canopy | 0.8837839 |

Final ecological values are at year 30; completion occurs at year 25. The established scripted crop-tree/competitor-release strategy is one feasibility witness, not an optimal or prescribed forestry solution. All work uses ordinary manager APIs and real cash/time limits.

One-tree quote: €2,500 cost, €2.98 revenue. First 276-tree job quote: €2,500 cost, €2,077.16 revenue, with at most one minimum-job adjustment. Both lose money, while the larger job spreads the unchanged fixed minimum. Starting cash remains €12,000. Sixteen first-intervention stems are retained as deadwood, matching the established Legacy40 script's four per property area; natural mortality also contributes to final deadwood.

## Required verification

| Gate | Final result |
|---|---|
| Compile/import | Zero C# compilation/importer errors; existing Editor Search diagnostic disclosed below |
| Dedicated calibration | PASS ×2, including restored review targets |
| Enlarged80 viability | PASS ×2, identical results |
| Rendered calibration review | PASS at 1280×720 and 1920×1080; exit 0 |
| StandGeometryVerification | PASS, 10 check groups |
| Enlarged80Verification | PASS, 12 check groups, physical anchors exact |
| Teaching Copy | Batch ×2 + rendered PASS |
| MenuTutorial | Standalone Editor ×2 PASS; full-regression repeat PASS |
| Clearance / Removal / Interaction | PASS |
| P2 | Batch ×2 + interactive PASS; all three layouts pass; no historical allowance needed |
| P3 | Batch ×2 + interactive PASS |
| Full explicit Legacy40 regression | 24/24 PASS; all established historical hash sets exact; production source unchanged in every gate |

| Required anchor | Exact hash |
|---|---|
| Enlarged80 start | `04A78188A6F8E6F9` |
| Enlarged80 year 3 | `081DB58CE3034648` |
| P2 | `F58FB0B1A421D28B` |
| P3 | `F7C2FC966816BBAD` |
| Model2 v18 completion compatibility | `702766DECE591E21` |
| Reference year 100 | `7AD177B3CC2F73C7` |
| Reference legacy-v15 continuation | `9CDF21A541C5968D` |


All checks use Linux Unity Editor 6000.6.0f1, the task's independent Library and installed project packages, one Editor at a time. Historical gates use the existing explicit `CCF_STAND_GEOMETRY=0` override. Enlarged80/calibration gates use the production new-game policy. Existing fixtures/tolerances are unchanged.

Compilation/import completed with zero C# compilation errors. Batch logs also contain the pre-existing UnityEditor.Search indexing exception; its ten-line signature is exactly identical to the accepted base's `Build/Enlarged80/run1.log`. This Editor diagnostic is disclosed in `editor-search-baseline.json`; the logs are not described as entirely error-free. Earlier rendered harness attempts completed their assertions but crashed/stalled during Editor shutdown and remain recorded as FAIL. The new harness now leaves Play Mode and persists its exit code across domain reload before closing from an Editor update; the final rendered run exits 0.

Rendered Objectives, Annual Review objective rows, aspirational Century Review and real Legacy40 frozen comparison are checked at 1280×720 and 1920×1080. Assertions verify text, effective values and parent layout bounds; captures show readable target rows. The century presentation fixture changes only an in-memory review record and restores the original world; it does not simulate a century for a copy/layout test.

## Scope and preservation audit

Five production C# files changed: ScenarioOneDefinition, ScenarioOneObjectives, ScenarioOneManager, ScenarioOneUiFacts and AnnualReviewView. New disposable calibration/viability/rendered gates and their runner/readme are under Tools/Verification/Enlarged80Calibration. Verification evidence is under Docs/Verification/Enlarged80Calibration.

No committed scene, prefab, material, ScenarioOne.asset, ProjectSettings, package, metadata/GUID, save schema, frozen reference, ecology or economy changes. Unity's local-editor-setting/material normalization is recorded and restored byte-for-byte after the final Editor operation. Existing player save and backup are preserved and checked. No staged Assets harness copies or generated harness meta remain. `/home/jer/CCF-main` and other source/recovery/settings/material work were not changed.

Full local logs, earlier failed attempts and recovery data are retained outside source under `/media/jer/Files/CCF-e80-calibration-local-output/Build/Enlarged80Calibration` and the existing gate-specific Build subdirectories. The task's own Build/Library links are removed for final clean Git status; the underlying output/cache remains preserved. Before another task-worktree Editor run, recreate those two links to this task's own external directories, or supply another independent Library/output directory. No cache is shared between worktrees.

## Follow-ups and next step

Unchanged known follow-ups: malformed cell-index save hardening, minimal startup/title screen, standalone Player performance/build profiling, and native Windows technical smoke. No Windows build was generated here and no native Windows/Player readiness is claimed. Stage A is not labelled ready.

Ready for Manager review: YES.
Decisions needed within this approved calibration: none.
Recommended next step: Manager inspect the exact branch/HEAD and evidence, then authorize deliberate integration and post-integration verification. This worker has not integrated to main or refreshed canonical project context.

Evidence: the JSON files beside this handoff and the eight `captures/` PNGs. The evidence-only final branch HEAD is reported in the worker response/external handoff; the production implementation tested by these runs is the exact SHA above. The existing runner’s approved synthetic-v5 fixture disposition is unchanged.
