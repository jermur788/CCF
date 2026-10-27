# RACK-V2-COMBINED-REPAIR-01 — QUALIFIED

**Exit: `RACK V2 COMBINED REPAIR QUALIFIED — READY FOR MANAGER REVIEW`**

**Recommendation for the repaired Rack V2 core candidate: APPROVE** for Manager review (G2 for this repair; G3 for the two harness
corrections below; G4 integration remains the Manager's). The blocking failure from the combined handoff is closed on every gate the
packet lists, on the exact tested HEAD. Two things need a Manager decision or awareness and are stated plainly under *Known limitations*:
the lesson 2–5 teaching area is still anchored on the authored middle line, and some pre-existing HUD/map copy still advertises the
legacy `[B]`/`[M]` tools. Neither is new and neither is in this packet's scope.

No user acceptance, whole Objective 1 playability, external-demo readiness, S1-D completion, Multi-Scale Forestry Management readiness or
Windows-build readiness is claimed. Main is unchanged.

## Identities

| Identity | Full SHA |
|---|---|
| Required starting publication (equals `origin/integration/scenario-one-rack-v2-v1` at claim) | `9127c025e539ee09ffa883f0cb8510167fedcc14` |
| Exact failed tested candidate being repaired | `96d9abe2b32a53e744b30c79c2ac7b194350dcfc` |
| Production repair commit | `86a4734f6840ab4a74409ce19161932b6dc9a1f6` |
| New focused regression gate | `387edf6ac58a1e4c43751697a12348e0409f150e` |
| **Exact repaired / tested HEAD** | **`7b557fd43ca9468a1be22c161c24064eec9f955a`** |
| Packet source | `origin/main` @ `3b9958b` (`Docs/Handoffs/RACK-V2-Combined-Repair-01/TASK-PACKET.md`) |

This handoff is a subsequent documentation-only commit; the tested HEAD above is unchanged by it (`git log -1 --format=%H -- Docs/Handoffs/RACK-V2-Combined-Repair-01/HANDOFF.md`).
The packet names no context commit, so I read the canonical docs and spec (D-064, §39.7 OPEN-2 / OPEN-3) from `origin/main` at `3b9958b`.

Worker: Claude Sonnet 5.5 (`claude-sonnet-5-5`) via Claude Code in T3 Code. The claim issue recommended GPT-6.1 Sol; the actual executor is recorded on
claim #7. Worktree `/media/jer/ZX20/CCF-RackV2-Combined` (clean, byte-identical to `origin` at start). Unity ran in the existing lane `/media/jer/ZX20/CCF-U1`,
detached at each exact head, under the machine-wide lock. No delegated workers, no force-release, no Windows build, no push to `main`.

## Precise root cause

On a fresh model-1 Enlarged80 world, five confirmed V2 racks created 149 rack-owned orders and all became Approved, yet the lessons stayed at 0/5 and
`rack.approve` was never recorded. `RackLayoutTracker` measured progress against the **historical authored layout lines** (`RackPlanner.LayoutStatus()`),
and a V2 plan does not follow them (the qualified fixture's racks start at the headland rather than the stand edge, so their tree sets need not match the
authored lines'; I did not isolate which trees kept the authored lines incomplete, only that the tracker read 0/5). `LearningObjectivesView.ObserveRacks` returns before approval/result credit while the tracker is incomplete, and
`LearningObjectivesView.Sequence.cs` derived "access open" and the access year from planted `P…` trees lying on those lines. All three are the F-4/F-8/F-9
couplings the combined handoff named. The V2 runtime (rack objects, rack-owned orders, lifecycle) was correct and is not touched.

## What changed (production: three files, +85 / −19)

| File | Change |
|---|---|
| `UI/RackLayoutTracker.cs` | In a Rack Planning V2 world (`ScenarioOneManager.RackPlanningV2Active`) `Total`/`Done`/`Complete`/`Harvested`/`PlannedTrees`/`HasProgress`/`ProgressInstruction` read `ScenarioOneManager.Racks` and `RackOrderState` (a rack is *planned* when it owns pending, approved or completed work; *resolved* when it owns completed work and nothing open). New `WorkPending`/`WorkApproved`: in V2 only rack-owned orders (`rackId > 0`) count; otherwise the original expression. Nothing is cached, so it cannot outlive a load or a new game. `Lines` (drawn by the Stand Map) and `PreviewFeedback` (legacy tool preview) are unchanged and still count the authored lines in every world. A V2 plan token moves `Version` so presentation rebuilds. |
| `UI/LearningObjectivesView.cs` | `ObserveRacks` and `CurrentInstruction` use `layout.WorkPending` / `WorkApproved` instead of inline "any FellTree" expressions (identical expression for model 0). |
| `UI/LearningObjectivesView.Sequence.cs` | V2: access year = latest completed rack-owned felling; `AccessOpen = tracker.Harvested && year > 0`; HUD uses the same predicate and, in V2 only, the tracker's wording instead of "[B] … or [M] …". Model-0 expressions untouched. |

Consumes only existing authoritative APIs (`Racks`, `RackOrderState`, `RackPlanningV2Active`, `RackPlanTargetCount`). No second rack model, no persisted counter,
no duplicate lifecycle, no save-schema, runtime, headland, map, minimap, tool, economy or ecology change. The 02B-R and 03C production paths are byte-identical to
`9127c02` (`git diff --name-only 9127c02 7b557fd -- Assets` lists only the three files above).

### Before / after lesson predicates (V2 world)

| Predicate | Before (measured, `96d9abe`) | After (measured, `7b557fd`) |
|---|---|---|
| tracker after confirm | `done=0/5 complete=False` | `done=5/5 complete=True` |
| `rack.create` | recorded | recorded (unchanged) |
| `rack.layout` | `False` | recorded when all five racks own work |
| `rack.approve` | `False` after 149 Approved | recorded once no rack-owned work is Pending and approved/completed rack-owned work exists |
| `rack.result` | not reached | recorded: every rack resolved + an annual report + an intervention |
| teaching after execution | not reached | `AccessOpen` true → lesson 2 ("Choose promising trees to retain along the rack") |
| unrelated ordinary FellTree orders | could count | cannot hold back or satisfy any rack predicate in V2 |

## OPEN-3 untouched

Nothing about reopening a rack after approval, partial or complete execution was changed, and no code path I added reaches it. The predicates read the persisted rack
objects and their orders regardless of the confirmation flag, so they do not depend on how OPEN-3 is later decided. **Not exercised:** a reopen-after-execution scenario
(it is OPEN, so there is no accepted behaviour to test against).

## Verification on the exact HEAD `7b557fd`

All runs: Unity 6000.6.0f1, lane `/media/jer/ZX20/CCF-U1` detached at the head, own Library, isolated config + private Xephyr display, synthetic Input System events
(not native-player-input certification). Zero `error CS` lines across every final log; every Editor exit was 0; the reaper never fired.
Evidence root: `/media/jer/Files/CCF/Verification/RackV2-CombinedRepair01-20261027T1845Z/` (`unity-*-final/` per gate: `result.json`, raw log, `summary-lines.txt`, captures).
The driver is the previous integrator's, copied and adapted only for task id/owner (`driver/`, diffable against the original).

| # | Packet item | Gate (dir `unity-…-final`) | Result |
|---|---|---|---|
| 1 | compile/import clean | every run above | **PASS** — 0 compile errors |
| 2 | real combined write + separate cold-start Continue | `combined-write` / `combined-continue` | **PASS** — `RACKV2COMBINED_WRITE_PASS checks=35`, `RACKV2COMBINED_CONTINUE_PASS checks=46` |
| 3 | same five rack IDs/state survive cold Continue | `combined-continue` | **PASS** — ids `[1,2,3,4,5]`, entrances `PO0203 PO1204 PO2003 PO2803 PO3803`; signature (ids, geometry, frozen sets, entrance trees, marks, order ids/status, counters), derived counters and world hash all exact; Continue was the real startup button against the disk file (SHA-256 identical) |
| 4 | 149 approved rack-owned orders | `combined-continue` | **PASS** — 149 orders; Approve approves exactly the frozen rack trees and leaves none Pending; count unchanged, no rebaseline |
| 5 | `rack.create`, layout progression, `rack.approve` | `combined-continue` | **PASS** — `RACK_LAYOUT_TRACKER done=5/5 complete=True`; `LESSONS_AFTER_APPROVE create=True layout=True approve=True` |
| 6 | execution → `rack.result` | `combined-continue` | **PASS** — year 0→1; 1344→1195 living (exactly the 149); all 149 stumps; cash change equals quoted net (−177446); all rack-owned orders Completed; every rack locked; `LESSONS_AFTER_RESULT approve=True result=True harvested=True reports=1 interventions=1` |
| 7 | subsequent teaching reachable | `combined-continue` | **PASS** — rack-first stage ends; cue is lesson 2; `[X] Fell` / `[C] Crop Tree` prompt, X and C marking work (see G3 item 1) |
| 8 | completed-state second save round trip | `combined-continue` | **PASS** — completed state validates on disk, is loadable, and ids, geometry, marks, order states and locks are exact after `Load()` |
| 9 | authoritative runtime gate | `runtime` | **PASS** — `RACKV2RUNTIME_VERIFY_PASS checks=14` |
| 10 | RackV2Save | `racksave` | **PASS** — `RACKV2SAVE_VERIFY_PASS checks=35` |
| 11 | headland / access | `headland`, `access` | **PASS** — `HEADLAND_QUALIFICATION_PASS checks=124`, `HEADLAND_ACCESS_PASS checks=8` |
| 12 | seven protected fingerprints | `legacy40`, `enlarged80`, `reference`, `completion-*` | **PASS** — table below |
| 13 | model-0 / Legacy40 progression regression | `regression-model0`, `regression-legacy40` | **PASS** — `RV2L_MODEL0_PASS checks=21`, `RV2L_LEGACY40_PASS checks=15`; traces identical to the pre-repair bytes (below) |
| 14 | rendered captures if visible copy changed | `combined-continue/evidence/` | **Reviewed** — all 14 images, c01–c07 at 1280×720 and 1920×1080 (below) |

Also run: `StandGeometryVerification` (`STAND_GEOMETRY_VERIFY_PASS checks=10`), `Enlarged80Verification` (`ENLARGED80_VERIFY_PASS checks=12`), `ScenarioReferenceVerification` (`REFERENCE_FUTURE_V1_PASS`, `REFERENCE_FUTURE_V1_CONTRACT_PASS`),
`ScenarioOneCompletionVerification` growth-2 and v18 (`SCENARIO_ONE_COMPLETION_VERIFY_PASS`). The player's real `forest-save.json` SHA-256 is unchanged before and after
(`7effe9b72c741d372efbb8acc1115d7be57a3514300da1d3d2de42801e565075`); every gate refuses to run outside an isolated `XDG_CONFIG_HOME`.

### Protected fingerprints

| Fingerprint | Where it is measured | Result |
|---|---|---|
| `5546B9B3B50673B2` | Legacy40 start world | **EXACT** |
| `94589C749852F359` | Historical Enlarged80 start | **EXACT** |
| `92F9635566BF6AFE` | Historical Enlarged80 year 3 | **EXACT** |
| `7AD177B3CC2F73C7` | Reference archive year 100 (`embeddedHash == stored == pinned`) | **EXACT** |
| `BFFF66015A6619DA` | Reference continuation v15 layout | **EXACT** |
| `9C77EB4BEE427FB9` | Completion growth-2 layout | **EXACT** |
| `702766DECE591E21` | Completion historical v18 projected | **EXACT** |

### Model-0 / Legacy40 regression (item 13)

New disposable gate `RackV2LegacyProgressionRegression.cs`. `model0` regenerates the historical model-0 Enlarged80 world, plans the five authored lines through the legacy
`ConfirmRack`, approves in the real Work Plan, advances a year; `legacy40` does the same in a fresh Legacy40 game. It prints timestamp-free `RV2L_TRACE` lines and reads only
members that exist on the pre-repair bytes, so the **same gate bytes** (SHA-256 `13c08856…2af3`) ran on `9127c02` and on `7b557fd`:

- `model0`: 9 trace lines each, **identical**; `rack.create/layout/approve/result` follow the authored lines, orders carry no rack id (so a V2 predicate could never have recorded `rack.approve` here), lesson 2 opens, a disk round trip keeps model 0 and the lessons re-derive.
- `legacy40`: 5 trace lines each, **identical**; tracker stays empty, only `rack.create` records, the next lesson opens from the first completed felling.

This is the evidence that legacy saves are not routed through V2 predicates: behaviour is bit-for-bit the same before and after, and the V2 branches are gated on `RackPlanningV2Active`.

### Rendered review (item 14)

Visible objective/HUD/progress copy did change, so all 14 captures were opened. They show: HUD card "Create access racks … Access routes: 5 of 5 planned" and footer "Extraction racks: 5 of 5 in the Work Plan" (was 0/5); Work Plan pending then "149 open task(s), 149 approved" with the objective line in the footer;
Annual Review "149 tree(s) thinned, No tasks failed"; post-execution HUD "Objective 2 of 8 · Choose promising trees to retain along the rack"; top-down pending markers and the five opened rack corridors. Text is readable and unclipped at both sizes. Not captured, therefore **not** reviewed: the Stand Map screen (see limitations).

## G3 — harness changes that alter what is measured (for Manager review)

No threshold, hash, expected entrance id or other gate was weakened, deleted or bypassed, and nothing was force-credited. Two harness changes were required and are separately committed (`7b557fd`), each justified by a measurement:

1. **`RackV2CombinedVerification.cs`, Stage 2 cue (was line 498).** It asserted `Summary.Contains("Inspect a living tree")`. That string is the title of an optional practice step (`tree.inspect` in `LearningObjectivesView.Curriculum.cs`), copied from the old `RackUxRack` gate; the sequential lessons replaced the cue with `"Current objective: <lesson title>"`, so no correct repair can satisfy it. Measured on the repaired production bytes at `387edf6`: every assertion before it passed (38 checks) and it failed with the cue `Current objective: 2. Choose promising trees to retain along the rack · [O] Details` (retained: `unity-combined-continue-repaired-387edf6/`). It now asserts the current equivalent: `!TeachingRacks`, `CurrentTopicIndex == 1`, and the cue contains lesson 2's title. The `[X]` / `[C]` marking checks that follow are unchanged.
2. **`RackV2LegacyProgressionRegression.cs`, Legacy40 reload expectation.** My first expectation was that `rack.create` re-derives after a reload. Measured on the pre-repair `9127c02` it does not (in a layout-less world it comes from the session-only `RackGroups`, which a load does not restore), identically before and after the repair. The expectation now records that existing behaviour. Both the baseline and repaired Legacy40 first-attempt failures are retained (`unity-regression-legacy40-baseline-9127c02/`, `…-repaired-387edf6/`).

The new regression gate itself is new coverage and replaces nothing. The legacy `RackUxRack` gate and its F-10 mapping are unchanged; those diagnostics were not rerun (not required by this packet).

## Failures and recovery (honest account)

- Diagnostic stage at `387edf6` (retained, not used for qualification): baseline and repaired Legacy40 regression failed on the same wrong expectation (item 2 above); the combined Continue failed only at the stale cue (item 1). Model-0 passed on both.
- Qualification stage at `7b557fd`: first attempt, all green. No Editor hang, no reaper action, no interrupted run, no restart.
- Editor side effects (`.vscode/settings.json`, SectionFive materials) were captured to per-run patches and restored byte-exactly; any unexpected change would have been refused. No disposable script or generated `.meta` remains under `Assets/`; U1 is clean (0 paths) and restored to its prior detached `8763623`.
- Unity lock sessions (all acquired and released normally, none forced): `47b43023…` baseline regression diagnostic; `3483a9ef…` repaired regression diagnostic; `8ede9d87…` repaired chain diagnostic; `6e2e2f71…` baseline regression with final gate bytes; `110bf2d6…` **final qualification at `7b557fd`**. Lock is free, no Editor process.

## Known limitations and decisions needed

1. **Teaching area for lessons 2–5 is not retargeted (decision for Manager).** `Sequence.cs` still builds the lesson-2–5 "rack area" (`ScenarioOneRackBay`) from the **middle authored layout line**, as before. In the qualified fixture the confirmed racks lie on those lines, so lesson 2 is reached correctly; I verified the transition *into* lesson 2 only. A player who authors racks elsewhere (tasks 04–10) would still be taught against the authored middle line, and, from reading the bay analysis (not measured), rack-owned removals off those lines could be counted as release work. Retargeting the bay to a confirmed rack is a design decision (which rack is "the" teaching rack), not a bounded repair, so I did not make it.
2. **Pre-existing copy still advertises legacy tools.** The walking-HUD footer reads "Extraction racks: 5 of 5 in the Work Plan · [B] Create rack · [M] Plan on the Stand Map · [Tab] Work Plan", and a fresh V2 world's opening cue still says "Press B …". Those strings are outside the allowed files and belong to the pending first-person-authoring / Help-copy tasks. The counts are now truthful; the tool names are not changed by this repair and were already there. The summary line also keeps saying "approve rack work, then advance one year" after approval (it never distinguished pending from approved).
3. **Stand Map text not rendered.** `StandMapView` shows `Done`/`Total` ("Planned layout: n of 5 racks in the Work Plan. A dashed line on the map is a rack still to do.") and now reads the V2 counts in a V2 world, while the map still draws the authored lines (minimap/map V2 propagation is a separate task). This gate captures no map screen, so that presentation is unverified.
4. No production caller of the V2 authoring API exists yet (per the combined handoff); everything here is API/Editor-driven, not a playthrough. Synthetic input only.
5. Dashboard synchronization **NOT AVAILABLE**; published dashboard reflects this task: **NO**. `active-tasks.md` (ownership/lock authority) was not edited by this worker; the Manager should update the live row. The claim issue is the execution-presence record only.

## Standard handoff fields

```text
IMPLEMENTATION HANDOFF
Task: RACK-V2-COMBINED-REPAIR-01
Worker: Claude Sonnet 5.5 (Claude Code in T3 Code)
Role: Primary (bounded verification repair)
Branch: integration/scenario-one-rack-v2-v1
Base: 9127c025e539ee09ffa883f0cb8510167fedcc14
Context commit: none named by the packet; canonical docs read from origin/main 3b9958b
HEAD: 7b557fd43ca9468a1be22c161c24064eec9f955a (tested); handoff is a later docs-only commit
GITHUB HANDOFF PATH: Docs/Handoffs/RACK-V2-Combined-Repair-01/HANDOFF.md
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/RackV2-CombinedRepair01-20261027T1845Z/
SERIALIZED UNITY FILES TOUCHED: none
SIMULATION / SAVE IMPACT: none (UI/lesson derivation only; no schema, no world hash, no RNG)
ASSETS ADDED OR MODIFIED: none
MUST-NOT-CHANGE HASHES / ANCHORS: all seven EXACT
INTENTIONALLY CHANGED HASHES / ANCHORS: none
SPEC CONFORMANCE: D-064 / §39.7 OPEN-2 sequence PASS for the qualified headland fixture; OPEN-3 untouched; tasks 04–10 not implemented and not claimed
UNRELATED WORK PRESERVED: yes — the primary checkout's AGENTS.md, active-tasks.md and untracked docs were not touched; player save unchanged
Ready for review: Yes
RECOMMENDED NEXT STEP: Manager G2/G3 review of this handoff and the two harness corrections; decide limitation 1 (bay anchoring) before any player-authored racks are taught.
```

**DASHBOARD SYNC: NOT AVAILABLE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NO.**
