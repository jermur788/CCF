# RACK-V2-COMBINED-INTEGRATION — NEEDS CHANGES

**Recommendation: NEEDS CHANGES for integration to canonical `main`. The requested qualification token has not been earned.**

The two qualified Rack lanes are combined without any production-byte changes. Fresh V2 generation, the authoritative runtime and disk persistence can coexist. However, the required combined lesson flow fails on the qualified headland geometry: five confirmed racks and five marked entrances produce 149 correctly approved rack-owned orders, while the legacy tracker still reports **0/5**, and **`rack.approve` remains false**. Execution, `rack.result`, subsequent teaching and the completed-state disk round trip are therefore not qualified. This is a measured failure, not a waived legacy test.

Task: `RACK-V2-COMBINED-INTEGRATION`; dispatch and packet from `main`. Role: integrator. Claude Sonnet 5.5 prepared the merges/harness; T3 Code / Codex gpt-6.1-sol completed recovery and the measurements reported below. No delegated workers were used in this continuation.

## Exact identities and integration

| Identity | Full SHA |
|---|---|
| Current main at original task start and continuation fetch | `9cd1788cbc53504bc64316ade9e1e6b0c76c40ae` |
| Locked canonical context | `eb218eab8b8e200a81cc99bfef7e8906bc21a88d` |
| Common designated Rack source base | `c6342f79a3e89c040fadd232017a937132779385` |
| Merge base of main and Rack source base | `c1564b857972ee18917e8a461c457533a54cc3f1` |
| 02B-R qualified production candidate | `7f708e4e30e61f3ce952a4ac75bde5fa2d8d38cd` |
| 02B-R published documentation tip | `fef344d537e510649082b0ae84f8ef678e26f91d` |
| 03C production implementation | `2178d392dd0fd285a30383e0bcf4d0df1745a8af` |
| 03C final source-lane tested HEAD | `84f801850acc26906a7cd8dd1a452d0a8da3f90a` |
| 03C published documentation tip | `098e18745cef9fdca5627bfb1b7f9bd3935e98e0` |
| Final combined candidate / exact qualification HEAD | **`96d9abe2b32a53e744b30c79c2ac7b194350dcfc`** |

Integration worktree: `/media/jer/ZX20/CCF-RackV2-Combined`; branch: `integration/scenario-one-rack-v2-v1`. This handoff and the assertion mapping are a subsequent documentation-only publication; its commit is available from `git log -1 --format=%H -- Docs/Handoffs/RACK-V2-Combined-Integration/HANDOFF.md` and the external `publication.json` receipt. The tested candidate above is unchanged by that publication.

The branch started from current main. Merge `e9dfc85` brought in 02B-R; merge `1dd6c82` brought in 03C. The only textual conflict was add/add in the 03C handoff, resolved to the source lane's final published handoff after inspecting it and the Manager approval. No production conflict or serialized-asset conflict occurred. `8c04e43` retained the headland/access harnesses; `942f379` and `8763623` added/extended the combined harness. Continuation commits `c08aede` and `96d9abe` correct only that new harness's geometry fixture and its approved-edit probe.

All required source commits were fetched, exist, descend from the designated base and are reachable from the specified published lane branches. `2178d39..84f8018` changes only the runtime harness; `84f8018..098e187` and `7f708e4..fef344d` are handoff-only deltas. Evidence: `preflight-source-identity.txt`, `continuation-audit.json` and `final-cleanup.json` in the durable evidence root below.

**Production preservation:** all five 02B-R changed paths (including the two new `.meta` files) and all six 03C production paths are byte-identical in the combined candidate to their qualified source. Their path sets are disjoint. Relative to 02B-R, the candidate's production delta is exactly the six 03C files, +748/−8 lines; relative to 03C it is exactly the five 02B-R paths, +150/−1. No additional production change was made to obtain a pass.

**Lineage warning for the Manager:** main does not contain the preceding Scenario One candidate line. Before this handoff-only publication, `main..96d9abe` is 457 paths, approximately +58k/−1k lines, with 189 Assets paths. The two Rack lanes themselves account for only 11 production paths, +898/−9. Integrating this branch also integrates that preceding Scenario One history; it is not an 11-file merge into main. S1-C/S1-D, Multi-Scale Forestry Management and unrelated parity work were not merged.

## Combined failure and player-facing boundary

The combined gate constructs racks from the five independently qualified entrance rows, with the access endpoint at `ScenarioOneHeadlandGeometry.HeadlandX`. The runtime selects exactly `PO0203`, `PO1204`, `PO2003`, `PO2803`, `PO3803`. The confirmed removal union contains 149 trees. Confirm creates only Pending work and fells no tree; it writes a real v21 save after two entrance marks.

In a separate cold Editor process, the actual startup Continue button is enabled and invoked against that disk file. The gate proves exact rack IDs, geometry, frozen removal sets, entrance IDs, mark state, order IDs/status, counters, living IDs and world hash. It then marks the other three entrances through the authoritative API and approves through the real Work Plan button. Every one of the 149 orders becomes Approved; none remains Pending.

Measured state at failure:

```text
RACK_LAYOUT_TRACKER done=0/5 complete=False
LESSONS_AFTER_APPROVE create=True layout=False approve=False
RACKV2COMBINED_CONTINUE_FAIL checks=22
  lesson rack.approve is recorded after approval of the V2 rack work
```

`UI/RackLayoutTracker.cs` still calls `RackPlanner.LayoutStatus()` on historical authored lines. `LearningObjectivesView.UpdateRackLessons` returns before approval/result credit whenever that tracker is incomplete. `LearningObjectivesView.Sequence.cs` also derives access completion and later teaching geometry from those old lines. These are the F-4/F-8/F-9 couplings already identified by the approved plan. They must be reconciled with confirmed rack snapshots in a bounded follow-up; asserting a pass on the old layout would hide the defect. The gate stops at the first failed required lesson assertion; it does not inject cached facts or force progression.

There is also **no player authoring route to the V2 API in production**: a search of Assets finds no production callers of `AddRack`, `UpdateRack`, `DeleteRack`, `ConfirmRackPlan`, `ReopenRackPlan` or `MarkRackEntrance`. The existing B tool and map Add rack still call legacy `ConfirmRack`, which intentionally refuses a V2 world. Tasks 04–10 (map editor, map/minimap propagation, removal of competing authoring, physical spray and teaching) are not in this candidate. Treat this as a V2 foundation under qualification, not a playable replacement Objective 1. No user-playability or full-feature acceptance follows from API-driven gates.

## Unity evidence and results

Durable evidence root:

`/media/jer/Files/CCF/Verification/RackV2-Combined-20261027T1334Z/`

Unity **6000.6.0f1**, existing registered lane `/media/jer/ZX20/CCF-U1`, detached at the exact tested candidate; own existing Library, no copied cache. Graphical runs use an isolated Xephyr/openbox display and a separate XDG config/save directory for each gate. No standalone or Windows build was run. Synthetic New Input System events and UI callbacks are not a native-player-input certification.

Entry drivers are `driver/phase1_shakeout.sh` and `driver/phase2_chain.sh`; the interrupted orchestration was resumed using `driver/phase2_remaining.sh` and finally `driver/phase_model0_only.sh`. Each `unity-*/result.json` records HEAD, exit, duration and required tokens; raw harness logs and `summary-lines.txt` are retained. A phase driver's final exit is not a substitute for these individual receipts; diagnostic runs deliberately fail.

<!-- GATE_RESULTS_START -->
| Gate / evidence directory | Result | Editor exit | Observed evidence |
|---|---|---|---|
| `unity-combined-write` | PASS | 0 | RACKV2COMBINED_WRITE_PASS checks=35 |
| `unity-combined-continue` | FAIL | 1 | RACKV2COMBINED_CONTINUE_FAIL checks=22 System.InvalidOperationException: lesson rack.approve is recorded after approval of the V2 rack work |
| `unity-runtime` | PASS | 0 | RACKV2RUNTIME_VERIFY_PASS checks=14 |
| `unity-racksave` | PASS | 0 | RACKV2SAVE_VERIFY_PASS checks=35 |
| `unity-headland` | PASS | 0 | HEADLAND_QUALIFICATION_PASS checks=124 |
| `unity-access` | PASS | 0 | HEADLAND_ACCESS_PASS checks=8 |
| `unity-legacy40` | PASS | 0 | STAND_GEOMETRY_VERIFY_PASS checks=10 |
| `unity-g3-enlarged80-original` | FAIL | 1 | ENLARGED80_VERIFY_FAIL: System.InvalidOperationException: regenerating the enlarged stand reproduces every ID, position, DBH and height |
| `unity-enlarged80` | PASS | 0 | ENLARGED80_VERIFY_PASS checks=12 |
| `unity-reference` | PASS | 0 | REFERENCE_FUTURE_V1_CONTRACT_PASS archive=True preview=True continuation=True replay=NotRequired; REFERENCE_FUTURE_V1_PASS |
| `unity-completion-growth2` | PASS | 0 | SCENARIO_ONE_COMPLETION_VERIFY_PASS |
| `unity-completion-v18` | PASS | 0 | SCENARIO_ONE_COMPLETION_VERIFY_PASS |
| `unity-rackux-as-written` | FAIL | 1 | Measured assertions: 48 PASS, 9 FAIL. RACKUX_RACK_FAILED shots=6 hardChecks=48 exception=System.NullReferenceException: Object reference not set to an instance of an object |
| `unity-rackux-model0` | FAIL | 1 | Measured assertions: 50 PASS, 8 FAIL. RACKUX_RACK_FAILED shots=6 hardChecks=50 exception=System.NullReferenceException: Object reference not set to an instance of an object |

All receipts above record exact `96d9abe`; compiler-error counts are zero unless explicitly stated in `final-results.json`. The failed original Enlarged80 and RackUx runs are retained diagnostic measurements, not qualification passes. The required combined Continue gate is a genuine failing requirement. Its later probe, execution/result, Stage 2 and completed-state round trip are not reached.
<!-- GATE_RESULTS_END -->

<!-- FINGERPRINTS_START -->
| Protected fingerprint | Expected | Measured on combined candidate | Evidence |
|---|---|---|---|
| Legacy40 start | `5546B9B3B50673B2` | EXACT | `unity-legacy40` |
| Historical Enlarged80 start | `94589C749852F359` | EXACT | `unity-enlarged80` |
| Historical Enlarged80 year 3 | `92F9635566BF6AFE` | EXACT | `unity-enlarged80` |
| Reference archive year 100 | `7AD177B3CC2F73C7` | EXACT | `unity-reference` |
| Reference continuation v15 layout | `BFFF66015A6619DA` | EXACT | `unity-reference` |
| Completion growth-2 layout | `9C77EB4BEE427FB9` | EXACT | `unity-completion-growth2` |
| Completion historical v18 | `702766DECE591E21` | EXACT | `unity-completion-v18` |

No anchor moved. These historical fixtures are distinct from the intentionally different fresh model-1 headland world.
<!-- FINGERPRINTS_END -->

**Save-v21 boundary:** the combined write/cold Continue checks prove a real pending/partially marked V2 disk save. The RackV2Save gate separately covers schema validation, malformed-data rejection before mutation, stable IDs/links, historical/model-0 saves and empty-plan hash compatibility. The headland gate's v20 example is a relabelled current capture, not a genuine archived v20 file. Completed-state cold/disk coverage in the combined gate is not reached after the lesson failure. Reference Future v1 is checked from its original embedded archive data; no archive was rewritten or reserialized into a replacement.

## RackUxRack / F-10 and G3

Every original assertion site is accounted for in [F10-MAPPING.md](F10-MAPPING.md): **41 retained, 43 replaced, 11 retired for V2**, with actual as-written and historical-model-0 observations alongside each. The source contains 95 Hard call sites, four of which expand over five racks; the inherited “98 assertions” note was not a verified source count. Branch-dependent runtime totals are reported in the raw logs rather than fabricated from that note.

The original `RackUxRack.cs` remains unchanged. Its as-written run measures the mismatch in a fresh V2 world. An external variant adds only a historical-model-0 world initialization block and one explicit model identity assertion; all old checks remain. Neither diagnostic run is a waived V2 pass gate. Exact stale copy, session RackGroup assumptions, B-tool authoring and fixed-row behavior are distinguished from still-valid modal, persistence, harvest/economy and subsequent-objective invariants. Replacements that depend on tasks 04–10 remain open. The mapping is submitted for Manager G3 review, not self-approved closure.

The as-written run measured **48 PASS / 9 FAIL**, and the model-0 variant **50 PASS / 8 FAIL**, both Editor exit 1 with a harness `NullReferenceException` in the old ForestTool sequence. The variant explicitly proved model 0. Both runs retain stale cue/title expectations and assume an immediate full-row tool proposal, whereas the current bounded tool first asks for a start. Their initial `19-hud-rack-first` images actually show onboarding page 2; those filenames do not establish HUD coverage. The V2-only extra numerical failure is the legacy row-2 `>20` tree assumption (measured 20); that threshold was preserved, not lowered. Later Work Plan/map/harvest/Stage-2 assertions are not reached by either old run. This is unresolved harness coverage, not proof of a new model-0 gameplay regression. The independent runtime gate does verify the model-0 backend.

The historical Enlarged80 fixture is measured both before and after the already-published source-lane model-0 initialization adjustment. The protected historical hashes are not expectations for the new headland world. The original run and the corrected fixture are both retained; no fingerprint was rebaselined.

## Rendered review

All eight final combined captures, `unity-combined-continue/evidence/c01..c04` at **1280×720 and 1920×1080**, were directly inspected. Continue HUD, confirmed pending world from above, Pending Work Plan and Approved Work Plan render at the expected sizes. The headland/feeder is visible in the top-down world, and the Work Plan money/residual panels and action buttons are readable at both sizes.

The captures corroborate the failure: the HUD, footer and Work Plan feedback report 0/5 routes and continue to advertise legacy B/map planning despite the confirmed V2 plan and approved orders. The minimap still depicts the authored layout, not the persisted rack objects. Whole-job timber and money estimates are present; the F8 text probe finding of no per-rack “estimated income” string must not be misreported as absence of all financial information. Runtime measurements show 149 ordinary planned markers but zero rack-specific markers and zero legacy RackGroups.

No c05–c07 annual review, harvested world or corridor capture exists from the combined gate, because it stops at `rack.approve`. No map editing handles/spacing, painted orange trunk or complete Objective/Help presentation is claimed: the corresponding production lanes are pending. Rendered conformance is **FAIL/INCOMPLETE** for the requested V2 flow, even where individual layouts are readable.

<!-- ADDITIONAL_RENDER_REVIEW_START -->
All four `unity-headland/evidence/` feeder/headland views and all four `unity-access/evidence/` junction views were directly inspected at both supported sizes. The soil route and junctions remain visible and the captured route is clear of standing stems. These checks support the infrastructure rendering only; they also retain the stale B-tool teaching copy and minimap omission already noted in the source handoff. They do not qualify physical orange entrance marking. All six as-written diagnostic images were inspected, plus the model-0 initial/choose-start views at both resolutions. They corroborate the stale onboarding/tool preconditions described above; no legacy end-to-end visual PASS is claimed.
<!-- ADDITIONAL_RENDER_REVIEW_END -->

## Failures, recovery and cleanup

1. The inherited phase started at 15:04Z under lock `1195d0f3107c4f90bbd1524a3362c5a1`. All three launches timed out before required tokens. S1-C's handoff records its worker's unauthorized transient checkout/launch collision during that ownership, so those results are invalid independently of the timeout. They are retained under `failed-phase1-20261027T1504Z/` and were not reused as qualification.
2. Recovery removed only the leftover disposable combined harness after proving it byte-identical to the tracked source and archiving it. A Scene Backup dialog was handled with **Keep Backups**; the recovered scene and its metadata were moved to evidence after the Editor exited. No user scene or asset was deleted. The runner now uses socket-capable `/tmp/` with its required trailing slash, checks task/HEAD/Editor/clean-lane ownership before staging, waits for a killed timeout process, and stops phase 1 on the first failed gate.
3. The first uncontaminated run at `8763623` exposed a bad new harness fixture: it used legacy row centers and property-edge endpoints and therefore selected different entrance trees. Its exit-1 failure is retained under `fixture-mismatch-8763623/`. The correction uses qualified headland geometry while preserving the independently expected entrance IDs. It completes all five API entrance marks before approval. No production, threshold or expected entrance ID changed.
4. The exploratory corrected-fixture run at `c08aede` first exposed the real lesson mismatch and is retained under `exploratory-c08aede/`. `96d9abe` corrects a later harness-only approved-edit probe to select any confirmed rack after all marks are complete; the final write/Continue runs on exact `96d9abe` reproduce the same failure before that later probe.
5. The continuation worker edited an external shell driver while phase 2 was running, causing a shifted shell read position (`HeadlandQualification.Begin: command not found`). That execution session also ended with signal 15 during Legacy40 startup; the signal source was not established. The completed runtime/save/access exit-0 receipts are retained; the skipped headland and inconclusive Legacy40 were resumed under a new normal lock from `driver/phase2_remaining.sh`, which was left unchanged while running. The interrupted log is preserved in `driver-interruption-phase2/`; the interrupted launch ended, its licence copy and byte-identical disposable harness/metas were removed, and the original lock was released normally. No production source or other owner's process was touched.
6. The later unmodified long execution session also ended with signal 15 during the model-0 diagnostic. Its unfinished attempt is retained in `interrupted-model0-1719Z/`; no result is inferred from its partial assertions. Its child exited, temporary files/licence copy were cleaned, and only the missing model-0 comparison was restarted under a new normal lock with a session-independent `nohup`/`setsid` launcher. The signal source remains unproved; no product or completed-gate failure is attributed to it.
7. Known Editor-generated settings/material changes were captured to per-run patches and restored only after the disposable scripts/metas were removed. Unrecognized changes are refused by the cleanup helper. No routine `.vscode/settings.json` edit is committed. Post-shutdown reaping, if present in the receipts, is limited to this U1 run after its own `Cleanup mono` and at least 90 seconds of log silence; it never turns missing evidence into a pass.

<!-- CLEANUP_START -->
Final audit: `final-cleanup.json`. U1 is clean including untracked files and restored to detached `8763623c7b8a65c357e136a94ded117ff42492e3`; no disposable script or generated `.meta` remains in Assets. Machine lock FREE, no Editor PIDs, no task reaper, no retained licence copies. All 14 final measured processes exited normally with code 0 or 1; no post-shutdown-hang allowance was needed. One inherited unfinished-probe licence directory was removed during the final audit.

The player's real save SHA-256 is unchanged: `7effe9b72c741d372efbb8acc1115d7be57a3514300da1d3d2de42801e565075`. Both frozen Reference archive/schedule Git blobs remain identical to main. Both qualified production path sets remain byte-exact. Primary's pre-existing `AGENTS.md`, `Docs/Project/coordination/active-tasks.md`, untracked `Docs/Operations/StorageArchitecture.md` and `Docs/ScenarioTwoPreProductionAudit.md` are byte-identical to the continuation preflight; none was staged or committed.

| Unity lock | Run / disposition |
|---|---|
| `1195d0f3107c4f90bbd1524a3362c5a1` | Inherited invalid/contaminated phase; normal release already recorded at 16:19Z. |
| `b547934c59644358afdc69cb79ce5749` | Uncontaminated original-fixture failure at `8763623`; normal release. |
| `830d63f4d3a7467b818109380d1a5048` | Exploratory corrected fixture at `c08aede`; normal release. |
| `06559492bb244ba49042cd5eba5b9efb` | Final exact-HEAD write/Continue at `96d9abe`; normal release. |
| `63f30af75ebc4b0781a983db5f1a413c` | Completed runtime/save/access; interrupted later startup cleaned, then own normal release. |
| `992e5940dee640b3ac7febbb2db4e06a` | Remaining compatibility gates and as-written diagnostic; interrupted model-0 attempt not reused, normal release. |
| `a570f96a6afd4cbb8be431838035017c` | Final model-0 diagnostic; clean baseline restoration and normal release at 17:28Z. |

No force release, other owner's process termination, main merge, Windows build or dependency change was performed.
<!-- CLEANUP_END -->

## Spec conformance and next action

Spec: `SPEC-scenario-one-objective-1-rack-planning-field-marking-v2-V1`, approved. No spec/checklist status was promoted. The 01–03 foundation and headland are the integrated scope; map/field-marking player workflows and whole-feature tasks remain unimplemented. **Combined qualification conformance: FAIL**, specifically packet requirement 10 and subsequent-result/progression coverage. Other gates remain distinct in the results table; a green source/runtime/save gate cannot satisfy the failed combined lesson gate.

Documentation conflict for the Manager: spec §39.7 still labels OPEN-3 open and says not to implement it, while the later 03C Manager review and this dispatch explicitly accept/require the approved-but-unexecuted and completed-linked lifecycle boundaries. This integration follows those exact already-approved source identities without redesigning them; the Manager should reconcile the canonical decision/spec status before declaring whole-feature conformance.

Manager follow-up: issue a bounded F-4/F-8/F-9 reconciliation task for confirmed-rack consumers and teaching predicates, preserve model-0 behavior, and rerun the cold Continue → approval → execution → `rack.result` → subsequent-objective chain on its exact new HEAD. Complete the already-planned player-authoring/field-marking/UI lanes before calling Objective 1 playable. Review the broad preceding Scenario One lineage before any merge to main. No product redesign, economy/ecology retuning, package, save-schema or frozen-reference change is proposed by this handoff.

To reproduce the current failure without overwriting these receipts, when U1 is clean, unowned and no Editor is running:

```bash
CCF_RV2C_OUTPUT_ROOT=/media/jer/Files/CCF/Verification/RackV2-Combined-Manager-Repro \
  bash /media/jer/Files/CCF/Verification/RackV2-Combined-20261027T1334Z/driver/phase1_shakeout.sh \
  96d9abe2b32a53e744b30c79c2ac7b194350dcfc
```

Use a new output root for each attempt. Expected: Write PASS/35, real cold Continue exact-state checks, all five entrance marks, 149 correct approvals, then Continue FAIL/22 at `rack.approve`; clean lane restoration and normal lock release. The script obtains its own normal machine lock and refuses another owner. Exact per-gate commands and environment are in the retained drivers. This is an Editor/API reproduction, not a manual player playthrough.

**DASHBOARD SYNC: NOT AVAILABLE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NO.** The dispatch already recorded publication unavailable; the designated live coordination record remains dirty in the user's Primary checkout and was preserved rather than committed or overwritten. This tracked handoff is the task result for the Manager to reconcile into that record and the dashboard. Required operational follow-up: update the live task/ownership row, run supported ingestion/publication, and verify GitHub audit plus published readback. No generated dashboard state or permanent Drive mirror was edited.

**Exit: BLOCKED — confirmed headland V2 racks do not satisfy legacy lesson consumers (`rack.approve` is false after all rack work is approved). NEEDS CHANGES.** Main is unchanged. No user acceptance, external-demo readiness, Multi-Scale Forestry Management readiness, S1-D completion or Windows-build readiness is claimed.
