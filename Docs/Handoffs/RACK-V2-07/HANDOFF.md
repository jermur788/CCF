# RACK-V2-07 — HANDOFF

```text
IMPLEMENTATION HANDOFF
Task: RACK-V2-07 — Confirmation + Minimap Implementation / Qualification (claim #47)
Worker: T3 Code / Claude B Pro (`claudeAgent_claude_b_pro`) claude-sonnet-5-5, single writer, by direct user instruction
        (the claim preferred Codex GPT-6.1 Terra xhigh; the user named Claude B as main agent for this run).
        Helper: OpenCode / opencode-go/deepseek-v4.1-flash — read-only code survey and the static-checker script,
        written outside the repo and reviewed/copied in by the writer. No second writer.
Role: Primary
Branch: feature/scenario-one-rack-v2-confirm-minimap-v1
Starting main: 36d82f027e1112529f8d167e20fb67985d28b79d (descends from e57987ebb3631673d482c503e146b7e11b82c7c6)
Coordination commit (amendment applied, on main): 4e71674becbf94881398eb6fc958d8bc4c918a29
Implementation commits: 7ddf650, ee16a08, a548f9b, 38366c8
EXACT UNITY-TESTED CANDIDATE: 38366c8ba31fa7d42d87e8e7f61bd387310ca9dc
Final tip: the commit that adds this file (docs only; production and gate bytes are identical to the candidate)
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/RackV2-07/
Exit: CONFIRMED PLAN PROPAGATES — READY FOR MANAGER REVIEW
```

## Coordination amendment

Applied at task start on `main` (`4e71674`), after refreshing main, the claim and live coordination: RACK-V2-06 row → `QUALIFIED / MANAGER APPROVED / INTEGRATED` (PR #46, `3b5e157f…`, Manager review linked); RACK-V2-07 row added. The RACK-V2-04 and RACK-V2-05 rows did not contain the superseded OPEN-3 phrase, so they were left byte-for-byte unchanged. No other row was touched.

## What changed

**Authoritative layer (narrow; `ScenarioOneManager.RackEditing.cs` only).** `ConfirmRackPlan`, `ReopenRackPlan`, the per-rack lock, the consent flag and mark invalidation already existed in `ScenarioOneManager.RackPlan.cs`, which is **byte-identical to starting main** (as are `RackGeometry`, `RackPlanner`, `RackEditing`, `RackTool`, `PlannedFellMarkers`, the suggested-layout and save files). Added read-only/facade members only:
- `CanConfirmRackPlan(out reason)` — true only for exactly five racks, each re-validated with the map's own editing rules (locked racks are skipped); otherwise an honest reason (`…needs exactly 5 racks. The plan has N.` / `Rack N is not valid: …`).
- `ConfirmRackPlanFromMap(allowCancelApproved)` — checks the gate, delegates to the existing `ConfirmRackPlan`, drops the map's undo memory. No flag, order, approval or mark of its own.
- `DescribeRackReopen()` / `RackReopenSummary` — derived counts (racks, locked, editable, racks with approved work) so the map can warn first.

**Stand Map.** `Confirm Rack Plan` (disabled with the reason beside it unless the gate passes); `Reopen Rack Plan` on a confirmed plan; a warning panel before reopening when approved-but-unexecuted work exists (`Reopen the plan` / `Keep the plan confirmed`; reopening itself cancels nothing); a consent panel when an edit, delete, undo, reset or confirm would cancel approved work (`Cancel approved work` repeats the same authoritative call with `allowCancelApproved`; `Keep approved work` drops it and changes nothing). A confirmed plan hides the pointer layer and the dashed guidance and legend row. Harvested racks stay locked.

**Counter.** `RackLayoutTracker.V2CounterText` is computed on every read from `PlannedRackCount`, `RackPlanConfirmed` and `MarkedRackEntranceCount`: `Racks planned: X / 5` while the plan is open, `Rack entrances marked: X / 5` once confirmed. Used by the Stand Map card, the Objective 1 instruction, summary line and HUD progress line, only in a V2 world. After confirmation the text also says the removal work is in the Work Plan for review and approval.

**Minimap.** One shared model, no copied rack state. `StandMapSource.ReadRacks` reads the foundation's suggested routes and `RackPlan`. Before confirmation: dashed suggestions. After: solid confirmed racks (pale if harvested). A draft rack is `FullOnly` (full map only), so the minimap's solid lines mean the confirmed plan. Outside planning the dashed guidance is carried in a separate `StandMapModel.MiniGuidance` list, so `Racks` still holds no suggestion outside planning mode (the protected RACK-V2-04 assertion is unchanged). Legacy authored lines and session rack groups are not read in a V2 world. `RackSuggested`, `RackPlayer` and `RackHarvested` are now minimap symbols (same colours/dashes as the Stand Map). The first-time note `Dashed lines show suggested rack positions. Solid lines show your confirmed plan.` sits under the minimap, non-interactive, shown for 30 s of actually-visible time and never after confirmation. Painting either map was shown to change no authoritative state.

## OPEN-3 state coverage (accepted boundary; no second lifecycle was needed)

| State | Evidence |
|---|---|
| Pending work, zero executed | Reopen allowed; orders and marks unchanged by reopening; edit reconciles that rack's pending orders without duplicates; reconfirm unchanged fabricates nothing (same order signature). |
| Approved but unexecuted | Reopen warning panel; decline keeps the plan confirmed and changes nothing; edit returns the authoritative warning, nothing changes until consent; consent cancels and recreates only the edited rack (others keep approved work). |
| Executed removal | That rack is locked/harvested: move/delete refused even with consent, mark kept, order untouched (same order id, still exactly one Completed); another rack stays editable; plan reconfirms around it. All five executed → nothing left to reopen. The `EXECUTED-WORK REOPEN CONFLICT` stop condition did not occur. |

## Confirm / reopen / order evidence

Unity gate Part A (213 checks, shared with Part B): empty/four/six/invalid-rack plans refuse with the reason and the save is byte-identical; five valid racks confirm once creating Pending orders only (none approved/executed, no marks, one order per tree); a second confirm is a no-op (`Invalid` through the facade, `NoChange` through `ConfirmRackPlan`, order and save signatures identical); confirmed plan is closed to geometry edits. Part B drives the real desktop pointer: disabled Confirm with the reason, Reset → five racks, Delete → four racks with Confirm disabled and a click on it changing nothing, Undo, Confirm, Reopen, drag edit, reopen warning, decline/accept, consent decline/accept, executed-rack lock, reconfirm.

## Mark-state preservation

Marks set through the existing `MarkRackEntrance` (player spraying is RACK-V2-09): reopening alone clears none (four stay stored while the counter reads `Racks planned`); changing rack A clears exactly A's mark (`MarksCleared == 1`) and B–D keep theirs; reconfirm counts 3/5, then 2/5 after a consented edit of B; a locked rack's mark survives refused edits; after the real-pointer drag rack 1 loses its mark and rack 2 keeps it, counter 1/5.

## Minimap evidence (both resolutions)

Captured at 1280×720 and 1920×1080 (21 captures): `01` HUD with dashed guidance and the note (layout asserted inside the panel and on screen; note text 15.6 screen px at 1080p; readable at 720p), `07` confirmed solid plan (note gone). A first run exposed that the first-run intro covered the first 720p capture; the gate now pages through the intro, and the final captures were re-read. `1280x720-14` shows the pale harvested rack beside four solid racks. Minimap and Stand Map identities and geometries are asserted equal (rack ids and endpoints) before and after confirmation and after execution.

## Preservation gates (all on candidate `38366c8`, nothing rebaselined)

| Gate | Result |
|---|---|
| RACK-V2-03 runtime (Unity) | PASS 14 |
| RACK-V2-04 map planning (Unity) | PASS 142 checks / 8 captures |
| RACK-V2-05 + 06 editing and spacing (Unity) | PASS 344 checks / 25 captures (first attempt aborted on a black screenshot at its first capture, before any assertion — the harness's known display flake; re-run on the same candidate passed; both logs kept) |
| `RackV2Editing/run_offline.py` | PASS 61 |
| `RackV2Spacing/run_offline.py` | PASS 11 |
| `RackV2SuggestedLayout/run_offline.py` | PASS 147 + read-only static PASS |
| `StandMap/OfflineLogic/run_standmap_check.py` | PASS 171 (was 165; see below) |
| `RackV2ConfirmMinimap/run_static.py` | PASS 10 |
| Runtime source type compile (172 files, Unity 6000.6 refs) and harness compile (Editor refs) | 0 errors |

## INTENTIONALLY CHANGED ASSERTIONS (G3 — Manager review requested)

Two protected RACK-V2-04 assertions in `StandMap/OfflineLogic/StandMapOfflineCheck.cs` stated that V2 racks do not appear on the minimap (`V2 presentation does not expand the minimap`, `a harvested V2 rack does not expand the minimap`). Packet requirement 8 requires the opposite. Each was replaced by a stronger assertion about what the minimap now draws (dashed suggestion, solid plan, matching colours, harvested drawn), with a comment naming the supersession, and six new checks were added (draft rack hidden on the minimap, confirmed and harvested shown, guidance outside planning drawn dashed with `Racks` empty of suggestions, version change on confirm). The Unity RACK-V2-04 assertion `no suggestions outside planning mode`, which an earlier design of this change broke, was kept byte-identical by moving the guidance to `MiniGuidance`. No Unity gate source was modified (gate sources for 03/04/05/06 are byte-identical to starting main).

## Unity lock receipts

Lock free at every acquisition; MFM-03 #39 was not an owner and was never bypassed. Lane `/media/jer/ZX20/CCF-U1` placed detached at the exact candidate, clean before and after (side effects diffed and restored; run harness and `.meta` removed). Final-candidate locks, all released normally by owner/task/lock id: confirm `b8a86f9a25294c3a8364bbe47ec93a82`; spacing `ad87dde4b21548c186d6111f1eddf062` (black-capture abort) and `92ef546b65434ff2abb749230cea4c99` (pass); map `314c5eb0740a4995a21f17e14987da79`; runtime `6e8d8d1d93344069acb4ac89d5e5b103`. Earlier development runs (confirm `edab776e…` on `ee16a08`; chain on `a548f9b` incl. the map-gate failure that drove the `MiniGuidance` redesign) are in `superseded/` and `unity-confirm-run1`. No Windows/Player build. Lock now free.

## Files changed (candidate vs coordination commit)

Production: `ScenarioOne/ScenarioOneManager.RackEditing.cs`; `UI/StandMapView.cs`; `UI/Map/StandMapRackEditor.cs`, `StandMapSource.cs`, `StandMapModel.cs`, `StandMapDrawPlan.cs`, `StandMapSymbols.cs`; `UI/HudMinimapView.cs`; `UI/RackLayoutTracker.cs`; `UI/LearningObjectivesView.cs`, `LearningObjectivesView.Sequence.cs`. Verification: `Tools/Verification/RackV2ConfirmMinimap/{README.md,RackV2ConfirmMinimapVerification.cs,run_gate.py,run_static.py}`; `Tools/Verification/StandMap/OfflineLogic/StandMapOfflineCheck.cs`. No scene, prefab, material, ScriptableObject, ProjectSettings, package, `.uss`, save/schema or `.meta` change. Unrelated dirty CCF-Primary work untouched.

## Known limitations / not claimed

- The first-time note is session memory (no schema change is in scope), so a new session shows it again; RACK-V2-11 may persist it.
- The HUD's lower hint bar still reads the model-0 wording (`[B] Create rack …`). Removing competing first-person authoring is RACK-V2-08 and was not touched.
- Marks and executed removals are fixtures through existing authoritative APIs; no player spraying (RACK-V2-09), no Help expansion (-10), no save work (-11).
- The reopen warning is a Stand Map presentation of the existing authoritative consent; `ReopenRackPlan` itself still takes no consent argument by design (its RACK-V2-03 contract, protected, is that reopening changes no order or mark). Cancellation always requires the authoritative `allowCancelApproved`.
- Not a whole Objective-1 qualification, no Player build, no user acceptance. Dashboard publication/readback was not performed or verified; no dashboard sync is claimed.

## Decisions needed

G3 acceptance of the two superseded minimap assertions (above). Otherwise none.

**Ready for review: Yes.**

`CONFIRMED PLAN PROPAGATES — READY FOR MANAGER REVIEW`
