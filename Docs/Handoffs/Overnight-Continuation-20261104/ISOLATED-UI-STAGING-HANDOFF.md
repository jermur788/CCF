# Isolated UI staging handoff — #269

Status: **IMPLEMENTED (staged) / REVIEWED by the staging worker only.** Not Unity-verified, not accepted, not integrated. Independent cross-provider review is still outstanding.

- Claim: https://github.com/jermur788/CCF/issues/269 — packet `ISOLATED-UI-STAGING-PACKET.md`
- Worker: Claude Sonnet 5.5 (T3 Code / Claude Code harness); effort/quota/context: UNKNOWN (not exposed). No sub-agents, no escalation.
- Branch: `integration/scenario-one-overnight-ui-qualification-v1`; worktree `/media/jer/Files/CCF/Operations/CCF-Overnight-UIQualification-20261104`
- Base: `72b4efbd1c653fa2231f632a79030a83ff76f833` (freshly fetched `origin/integration/scenario-one-player-test-ready-v1` resolved to this SHA; lane was clean at that HEAD before writing).
- PR target: `integration/scenario-one-player-test-ready-v1` (never `main`). PRs #261/#262/#264 were not merged, rebased or retargeted.

## Provenance annex (path ← source PR ← head SHA)

Provenance gate: `git diff --name-status 72b4efbd..<head>` for each head listed exactly the paths below and nothing else (3 + 6 + 4 = 13); all are inside the packet whitelist.

| Path | Status | Source PR | Source head |
|---|---|---|---|
| Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs | M | #262 (#255) | 693e24c8891637470052031d5023883b922e9334 |
| Assets/ForestPrototype/UI/MenuHelpView.Text.cs | M | #262 (#255) | 693e24c8… |
| Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs | M | #262 (#255) | 693e24c8… |
| Tools/Verification/RackV2HelpTeaching/run_offline.py | M | #262 (#255) | 693e24c8… |
| Tools/Verification/RackV2Objective1/RackV2Objective1EndToEnd.cs | M | #262 (#255) | 693e24c8… |
| Docs/Handoffs/Overnight-Continuation-20261104/OBJECTIVE-COPY-HANDOFF.md | A | #262 (#255) | 693e24c8… |
| Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs | M | #261 (#256) | 9304d318f346b281a19cb805feebc6dc1a95008e |
| Assets/ForestPrototype/UI/StandMapView.cs | M | #261 (#256) | 9304d318… |
| Docs/Handoffs/Overnight-Continuation-20261104/RACK-HANDLE-HANDOFF.md | A | #261 (#256) | 9304d318… |
| Tools/Verification/OvernightPlaytestFeedback/README.md | A | #264 (#260) | 6efba8fdc35f22098faf205a5f7d14ccf65a3e1a |
| Tools/Verification/OvernightPlaytestFeedback/baseline-output.txt | A | #264 (#260) | 6efba8fd… |
| Tools/Verification/OvernightPlaytestFeedback/check_baseline.py | A | #264 (#260) | 6efba8fd… |
| Docs/Handoffs/Overnight-Continuation-20261104/BASELINE-VERIFICATION-HANDOFF.md | A | #264 (#260) | 6efba8fd… |

Files were replayed with `git checkout <head> -- <path>` (byte-identical to the source heads, no commits cherry-picked). Three logical commits plus this handoff commit; `git diff --stat 72b4efbd..HEAD` shows exactly 13 files before this handoff is added (14 with it).

## Delta review (worker's own, source-only)

- **#255 copy.** `16–20 m` → `16–20m` (3 places), Tree Marks title → `Introducing tree marking`, intro sentence and orange sentence reworded, "four sides" sentence reworded. `TreeMarkEntries` untouched: orange vertical stripes "Rack / machine-path removal", **Red — Fell**, **Blue — Crop Tree / future timber** remain labels. No colour-resolution claim made; #259's open world/map colour discrepancy is **not** resolved here.
- **#255 tests.** The title check became an exact `==` for `Introducing tree marking`. The orange check became an exact `==` against the approved replacement sentence (was `== s28[0]`; the simplification sentence is still compared to spec §28). `UnitStyle()` replaces only the literal `16–20 m` → `16–20m`, applied to the §31 quote comparison; `InOrder`/`Has` checks were updated only to the `16–20m` literal. No check removed (97 checks run).
- **`run_offline.py`.** Two exact paths added to the S10 allow-list (the #255 handoff and `RackV2Objective1EndToEnd.cs`); no prefix or wildcard added.
- **`RackV2Objective1EndToEnd.cs`.** One literal `16–20 m` → `16–20m`. Not run (Unity-only).
- **#256.** `Propose(px, gesture != Gesture.Reshape && !ShiftHeld(...), …)`: only the round-handle (Reshape) gesture loses snapping; Create and Move receive the same argument as before. `Propose`, `OnPointerDown/Up`, `Apply`, `ShiftHeld` unchanged (confirmed against the base below). Comment and help sentence updated. No rack policy/validation change.
- **#260.** Adds the checker, README, captured output and handoff only; no production file.
- **Observation for the reviewer:** `OBJECTIVE-COPY-HANDOFF.md` (~450 KB) and `RACK-HANDLE-HANDOFF.md` (~323 KB) are very large; they appear to embed full project-context dumps and raw transcripts. They are whitelisted and were staged byte-identical; trimming them is a Manager/reviewer decision, not done here.

## Raw evidence (run from the lane at the stage commits)

### (a) `python3 Tools/Verification/RackV2HelpTeaching/run_offline.py`  → exit 1
```
FAIL S10 only the RACK-V2-10 copy/wiring files changed since 6d3228c: Assets/ForestPrototype/ForestSaveController.cs, Assets/ForestPrototype/ForestSaveData.cs, Assets/ForestPrototype/ForestSaveValidation.cs, Assets/ForestPrototype/ForestStartingStand.cs, Assets/ForestPrototype/ForestTree.cs, Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs
FAIL S11 no marking, rack geometry, manager, save/schema, scene, prefab, material, settings, package or .meta change
RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97
PASS offline wording/unlock/flow check
RACK_V2_HELP_TEACHING_FAIL failed=2 static_and_offline=15
```
- The offline entry gave the expected `RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97`.
- S10/S11 are **branch-scope-guard failures, not project regressions, and were not weakened.** The script's built-in base is `6d3228c`; the six files named were changed between `6d3228c` and the candidate `72b4efbd` (confirmed via `git diff --name-only 6d3228c 72b4efbd`) and none are in this 13-file delta. The wrapper exits non-zero.
- Supplementary diagnostic using the script's own `--base 72b4efbd…` option (no code edited):
```
FAIL S10 only the RACK-V2-10 copy/wiring files changed since 72b4efbd1c653fa2231f632a79030a83ff76f833: Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs, Assets/ForestPrototype/UI/StandMapView.cs, Docs/Handoffs/Overnight-Continuation-20261104/BASELINE-VERIFICATION-HANDOFF.md, Docs/Handoffs/Overnight-Continuation-20261104/RACK-HANDLE-HANDOFF.md, Tools/Verification/OvernightPlaytestFeedback/README.md, Tools/Verification/OvernightPlaytestFeedback/baseline-output.txt
PASS S11 no marking, rack geometry, manager, save/schema, scene, prefab, material, settings, package or .meta change
RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97
RACK_V2_HELP_TEACHING_FAIL failed=1 static_and_offline=15
```
  With the correct base S10 fails only because the combined branch includes #256/#260 paths that this #255-scoped allow-list does not know. That is the expected combined-footprint scope failure.

### (b) Six focused rack-handle snap cases (#256)
These are not a committed harness. They are an inline stdin script in `RACK-HANDLE-HANDOFF.md` (~line 3716). I ran it from `/tmp` via `runpy` of `Tools/Verification/RackV2ConfirmMinimap/run_static.py` helpers. **One deviation:** the original compares against `git show HEAD:…StandMapRackEditor.cs`, which is vacuous in this lane (HEAD already contains the change), so I pinned the comparison base to `72b4efbd`.
```
PASS snap argument: Create, Shift=False -> True
PASS snap argument: Create, Shift=True -> False
PASS snap argument: Move, Shift=False -> True
PASS snap argument: Move, Shift=True -> False
PASS snap argument: Reshape, Shift=False -> False
PASS snap argument: Reshape, Shift=True -> False
PASS unchanged method: Propose
PASS unchanged method: OnPointerDown
PASS unchanged method: OnPointerUp
PASS unchanged method: Apply
PASS unchanged method: ShiftHeld
PASS paired instruction text
```
The `check_baseline.py` check 06b independently runs six direct production `SnapFar` calls on plain .NET (see (c)).

### (c) `python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix` → exit 1
Status lines (full output captured to `/tmp/uiq269/c.txt` during the run; detail lines omitted here):
```
SOURCE-STATIC baseline-source-commit: 72b4efbd1c653fa2231f632a79030a83ff76f833
SOURCE-STATIC tracked-production-vs-baseline: DIFF
SOURCE-STATIC selected-states: rotation=post-fix instruction=post-fix
SOURCE-STATIC PASS 01 requested-spacing-presentation-format: Requested presentation format, not a functional defect; expected to flip with #255. Spacing in metres, not a duration; require compact 16–20m/16-20m.
SOURCE-STATIC PASS 02 tree-marking-heading: Require exact heading literal Introducing tree marking; literal presence alone cannot prove display.
SOURCE-STATIC PASS 03 source-material-copy: No CCF source material phrase in ForestPrototype C# string literals (comments excluded).
SOURCE-STATIC PASS 04 orange-entry-is-not-fell: Bounded legend check: orange rack-removal entry is separate from Red/Fell; no claim about rendered paint.
SOURCE-STATIC PASS 05 no-formal-pro-silva-copy: Conservative lexical guard: no Pro Silva mention in ForestPrototype C# literals; any mention requires human review, not automatic classification as a standard.
SOURCE-STATIC NOT-TESTABLE 06 round-handle-input: BASELINE OBSERVATION only: snap argument is !ShiftHeld(e.modifiers), then forwarded to SnapFar for Reshape. SOURCE-STATIC cannot test physical input.
SOURCE-STATIC PASS 06a rotation-snap-policy: Expected state=post-fix; observed Reshape/Create/Move (Shift up,down)=[False, False, True, False, True, False]. Pre-fix requires [True, False, True, False, True, False]; after #256 require [False, False, True, False
SOURCE-STATIC PASS 06b snapfar-offline-cases: Six direct calls to production SnapFar on plain .NET; 0.001m tolerance; no Unity or gesture dispatch proof.
SOURCE-STATIC PASS SnapFar near-angle enabled=True input=(20.000,0.700) far=(20.012,0.000) angle=0.000 angleSnapped=True boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-angle-disabled enabled=False input=(20.000,0.700) far=(20.000,0.700) angle=2.005 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-angle-window enabled=True input=(20.000,3.000) far=(20.000,3.000) angle=8.531 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-boundary enabled=True input=(39.000,0.000) far=(40.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=True
SOURCE-STATIC PASS SnapFar near-boundary-disabled enabled=False input=(39.000,0.000) far=(39.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-boundary-window enabled=True input=(37.000,0.000) far=(37.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS 06c rack-edit-instruction-copy: Expected state=post-fix; observed state=post-fix. Require exact sentence: Drag a rack to slide it along the headland. Its round handle rotates freely. Hold Shift while drawing to turn snapping off. Source copy
SOURCE-STATIC PASS 07 objective-next-instruction: Unconfirmed V2 instruction names Stand Map / Plan Racks path; later states are quoted separately.
SOURCE-STATIC FAIL 08 objective-primary-cta: Require exactly one primary source button invoking ui.ShowMap(); labels are evidence only. Baseline primary invokes ui.CloseAll(); visual prominence, focus and destination behaviour remain unmeasured.
SOURCE-STATIC NOT-TESTABLE 09 map-minimap-trail-consistency: NOT STATICALLY TESTABLE: no demonstrated coordinate mismatch for the same confirmed rack/waypoint. Shared source/draw path is evidence, not rendered parity; unconfirmed racks are deliberately FullOnl
SOURCE-STATIC TALLY PASS=9 FAIL=1 NOT-TESTABLE=2
```
Tally is `PASS=9 FAIL=1 NOT-TESTABLE=2`, matching expectation. The single FAIL is **check 08**: the Objective 1 primary button is still `ui.CloseAll()` / "Back to forest [O / Esc]" (`LearningObjectivesView.cs:75`). #258's CTA is only drafted. The checker was not edited; the expected failure is preserved. NOT-TESTABLE: 06 (physical round-handle input) and 09 (map/minimap/trail rendered consistency).

## Explicit non-proofs

- No Unity Editor, import, C# compile, UI Toolkit render, text-fit, or scene playback. The `.NET` checks compile only the pure wording files and `RackEditing.SnapFar`.
- Nothing here proves physical mouse/Shift drag, ghost/release agreement, or that the round handle rotates in play.
- `RackV2Objective1EndToEnd.cs` and the Unity harnesses were not run.
- #259's world/map colour discrepancy is open; #258's CTA is unimplemented; spec §28/§31 text is now stale vs. the shipped copy (Manager reconciliation pending, as noted in the test comments).
- Not accepted by the user, not integrated, not Windows-built. The user's playtest branch and save were not touched.

## Next required gate: Unity qualification (separate packet)

A distinct Manager-approved packet must allocate exclusive Unity for the final staged ref. Fresh preflight before any start:
1. **Lock/ownership:** fresh deployed lock admission and #200 live ownership for the target worktree; no writer on `CCF-U1` or its warm Library, do not borrow it.
2. **Disk:** `df` on ZX20 (last DPC receipt ~19 GiB free; cold import needs ~10–12 GiB vs the 15 GiB post-admission floor — do not start cold Unity at that headroom). No deleting unrelated caches.
3. **Process:** no running Unity/Editor, `Unity -batchmode` or verification harness (`pgrep -a Unity`), and a clean worktree at the exact staged SHA.
4. Then: compile/import 0 errors, run `RackV2Objective1EndToEnd`, hand-drag the round handle with/without Shift, and review rendered Tree Marks help; only after that consider acceptance. No Windows/Player build without explicit post-playtest user authorisation.
