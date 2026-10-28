# RACK-V2-05 — Direct map rack editing: implementation and qualification

**Exit: `RACK EDITING READY — READY FOR MANAGER REVIEW`**

**Recommendation: `APPROVE` for RACK-V2-05 only.** Integration, RACK-V2-06+ and whole-Objective-1 qualification remain separate decisions.

The Stand Map's `Plan Racks` mode now lets the player manipulate authoritative rack objects directly with the mouse: **create** (drag from the headland into the stand), **select** (click), **move** (drag the body; the entrance re-snaps along the headland), **rotate** (drag the round far-end handle; the entrance stays put), **delete** (`Delete rack` button, `Delete` or `Backspace`), **`Undo last rack edit`** (exactly one step) and **`Reset to suggested layout`** (asks first when player edits would be discarded). Snapping toward the suggested orientation, toward a nearby parallel player rack and to the stand boundary is soft and advisory; holding Shift turns it off, and the player can freely deviate. Invalid geometry is refused with a stated reason and is never silently repaired. A rack with any executed removal is locked and drawn as harvested.

## Identity

| Item | Value |
|---|---|
| Task / claim | RACK-V2-05 / GitHub issue #43 (`jermur788/CCF`) |
| Starting `origin/main` | `140d8b8e806624b8dda54ca14d09db237faa7a45` (descends from OPEN-3 record `1a520555` and PR #41 merge `f91411a9`) |
| Branch / worktree | `feature/scenario-one-rack-v2-editing-v1` / `/media/jer/ZX20/CCF-RackV2-05` (no Unity `Library`) |
| Unity lane | `/media/jer/ZX20/CCF-U1`, detached at the exact candidate, machine-wide lock held for every Unity run |
| Production commits | `0f0c716` (rules, facade, pointer editor), `8d4b46a` (key/repaint cadence), `bc4c37e`, `3187c59`, `07efbe2` (three defects found by Unity runs, below) |
| Harness-only commits | `9ee2400`, `9a232a5`, `89076a1`, `240d6cb`, `bfffc9b`, `303de08`, `6635d65`, `0bce021`, `9b92895`, `cc64308`, plus the verification files inside `8d4b46a` |
| **Exact Unity-tested candidate** | **`cc643082b14f0b8bf96a5472e3505608c3025c26`** — production bytes identical to `07efbe2` (`git diff 07efbe2 cc64308 -- Assets Packages ProjectSettings` is empty) |
| Final branch tip | the candidate plus **one docs-only commit** adding this file; `git diff cc64308 <tip> -- Assets Packages ProjectSettings Tools` is empty |
| Unity | 6000.6.0f1 (`/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity`), private Xephyr + openbox display, isolated `XDG_CONFIG_HOME`, real `xdotool` desktop pointer and keyboard. **No Windows build.** |

## Model/provider transition log

Provider limits, not task problems, caused every transition. Each was recorded in claim #43 when it happened; one writer, one branch and one worktree throughout, and no work was lost at any transition (tree clean and pushed each time).

| # | Time (UTC, = Europe/Dublin on 2026-10-28) | From | To | Reason | HEAD |
|---|---|---|---|---|---|
| 1 | 00:29 | T3 Code / Codex `gpt-6.1-sol` (initial dispatch) | T3 Code / Claude A `claude-sonnet-5-5` xhigh, thread `782ebe06-0093-4cd8-8eec-9815324917b7` | Codex usage limit after workspace preparation only; no repository edits | `140d8b8` |
| 1w | ~00:36 | Watcher `opencode/deepseek-v4.1-flash` | Watcher `opencode-go/deepseek-v4.1-flash` | `Upstream request failed: Invalid credential` on the first endpoint; no edits; same model, different endpoint | `140d8b8` |
| 2 | 01:40 → 01:49 | Claude A | T3 Code / Codex `gpt-6.1-terra` xhigh, thread `mcp:6970352a-7710-4982-8685-c4882085e101` | Claude 5-hour session limit at 01:40:09Z (resets 04:20Z), six consecutive failed retries | `bfffc9b` |
| 3 | 02:20 → 02:31 | Codex `gpt-6.1-terra` | T3 Code / Claude B Pro (`claudeAgent_claude_b_pro`) `claude-sonnet-5-5` xhigh, thread `mcp:c98060d4-86c0-411e-86b7-ae74d4265ec6` | Codex usage limit (resets ≈04:52Z); Claude A still limited. Codex had finished the passing editing run but stopped before reporting it | `cc64308` |

The watcher (`opencode-go/deepseek-v4.1-flash`, also the bounded integrator of the RACK-V2-05 and MFM-03 rows of `active-tasks.md` under `OVERNIGHT-COORDINATION.md`) ran from task start; its last checkpoint that the final executor saw before writing this file is its 02:31Z continuation note in claim #43. Per that claim its coordination-file edits were limited to its own two assigned rows, plus one self-reported and corrected cell error (commit `6fe9b4e`). The final executor holds no coordination-write authority and did not edit `active-tasks.md`.

## What was built

All production code is in the allowed surface. Nothing is saved or counted separately.

| File | Role |
|---|---|
| `Assets/ForestPrototype/Racks/RackEditing.cs` (new) | Unity-free rules: entrance resolution onto the headland, `Validate` (non-finite, too short, outside the stand, entrance off the headland, duplicate, overlap), move-along-headland, and the soft snapping (`SnapFar`). Plain numbers so it runs on plain .NET. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackEditing.cs` (new) | Narrow facade over the existing authoritative `RackPlan`: `CreateRackFromMap`, `ReshapeRackFromMap`, `DeleteRackFromMap`, `ResetRacksToSuggestion`, `UndoLastRackEdit`, `ValidateRackEdit`, `GetSuggestedRackLayout`, `RackLayoutMatchesSuggestion`. Validates, then calls the existing `AddRack` / `UpdateRack` / `DeleteRack`, so ids, removal sets, order reconciliation, entrance-mark reset and the completed-work lock stay where they were. One-level undo is in memory only and is offered only while the plan is exactly as the edit left it. |
| `Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs` (new) | Pointer gestures (draw, select, move, rotate), the ghost, the status line and the reset confirmation. Presentation state only; every change goes through the facade. |
| `Assets/ForestPrototype/UI/StandMapView.cs` | Edit card (`Delete rack`, `Undo last rack edit`, `Reset to suggested layout`, confirmation), pointer layer shown only while planning, Delete/Backspace handling. |
| `Assets/ForestPrototype/UI/Map/StandMapModel.cs`, `StandMapSource.cs`, `StandMapDrawPlan.cs`, `StandMapSymbols.cs`, `StandMapLegend.cs` | Ghost (dashed white valid / dashed red refused), selection outline, entrance square, round handle and harvested-rack symbols — all dynamic-layer; a legend row `Harvested rack, locked` appears only when such a rack exists. |
| `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` | **3 lines** so Delete/Backspace reaches the map each frame while it is the open screen (the map's own refresh runs at 4 Hz and would miss a key press). Outside the packet's literal file list; judged the narrowest possible wiring. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs` | Two narrow changes, see "Production behaviour change in an existing path" below. |

Three new `.meta` files accompany the three new scripts. No scene, prefab, ScriptableObject, `ProjectSettings` or `Packages` file was touched.

### Accepted OPEN-3 behaviour as implemented

- Editable while the affected rack has **zero executed removals**; pending work is reconciled through the existing path; approved-but-unexecuted work is refused with a clear warning (`Changing this rack would cancel approved but unexecuted removal work…`) unless the caller passes explicit consent, after which it is cancelled and recreated exactly once per tree. That consent path is exercised through the facade (editing Part A) and the unchanged runtime gate; **the Stand Map pointer editor never passes consent**, so from the map such an edit only shows the warning and changes nothing. Approved work exists only after Confirm, when the pointer layer is hidden, so the player cannot reach this case from the map until RACK-V2-07's reopen/confirmation flow exists.
- A rack with any executed removal is **locked**: move, rotate and delete are refused, there is no rotation handle, `Delete rack` is disabled, the rack is drawn pale as harvested and the card says so. Executed removals are never reversed. `Reset` over a plan that holds an executed rack is refused.
- A geometry change to an editable rack clears only that rack's entrance-mark state through the existing `UpdateRack` contract (unchanged; asserted by the unchanged RACK-V2-03C runtime gate, check `entrance mark is authoritative and geometry-bound`).
- A confirmed plan hides the pointer layer and refuses create/reshape/delete/reset; reopening (RACK-V2-07's UI) will call the same facade. No reopen/confirmation UI was built.
- Legacy40 / model-0 worlds are not V2 worlds: the facade refuses create/delete/reset and offers no suggestion.

### Calibration values (deliberate gameplay calibration, flagged for Manager review)

`RackEditing.cs` states that these are Scenario One gameplay calibration for the editing feel (D-064), **not** forestry rules: minimum rack length 5 m; duplicate tolerance 0.5 m; overlap clearance = rack corridor width (2.4 m); angle snap window 4°; boundary snap 2.5 m; parallel-search reach 30 m. RACK-V2-06 owns the final spacing-teaching copy and may want to retune them.

## Production behaviour change in an existing path (disclosed)

`ScenarioOneManager.RackPlan.cs`:

1. `UpdateRack` previously reconciled orders even for a draft rack that had never held work, which would have put Work Plan orders in front of Confirm every time a map drag reshaped a draft (the file's own header promises none until `ConfirmRackPlan`). Now `ApplyRackOrderChange` runs only when the plan is confirmed or the rack already has an order; every other case is exactly as before. Found by Unity run 3 (`unity-editing-run3-FAIL-draft-edit-created-orders`).
2. `TrySelectRemovalSet` gained an optional `ignoredRackIds` parameter (default `null`) so a dry run can ask what a rack would take if racks being replaced were absent; every existing caller passes none.

The unchanged RACK-V2-03C runtime gate and the unchanged RACK-V2-04 map gate pass against this (below).

## Defects the Unity gate found in production (all fixed, all re-verified)

| Run evidence | Defect | Fix |
|---|---|---|
| `unity-editing-run3-FAIL-draft-edit-created-orders` | Editing a draft rack created Work Plan orders before Confirm | `bc4c37e` |
| `unity-editing-run4-FAIL-stale-undo-after-confirm` | Undo was still offered after the plan was confirmed/approved/executed, so it could restore stale geometry over new work | `3187c59` (undo is offered only while the rack-owned work is exactly as the edit left it) |
| `unity-editing-run7-FAIL-stale-release-position` (move shorter than dragged: 7.70 m) | A release re-derived its proposal from the release event, which can still carry the position from before the last moves when they and the release land in the same frame | `07efbe2`: a release applies the ghost the player was last shown ("what you see when you let go is what you get") |

Other failed runs (1, 2, 5, 6, 8 and the Codex candidates `bfffc9b`…`cc64308`) were harness or timing problems — harness geometry, pointer-event delivery timing under the Editor (the move-length shortfalls of 10.22 m, 10.88 m and 11.21 m of 12 m; see harness commits `9a232a5`, `bfffc9b`, `303de08`, `6635d65`, `9b92895`), a boundary-exact press target, and one intermittent black capture. They are kept under `…/RackV2-05-20261028/unity-editing-*-FAIL-*` and `unity-editing-terra-*` and are **history, not qualification evidence**. The 12 m ± 0.7 m movement assertion was kept unchanged throughout; no assertion was weakened to turn a run green.

## Verification at the exact candidate `cc64308`

### Static / offline (re-run by the final executor; evidence `…/claudeb-offline/`)

| Gate | Result |
|---|---|
| Editing rules (`Tools/Verification/RackV2Editing/run_offline.py`) | `RACK_EDITING_OFFLINE_PASS checks=61` |
| Shared Stand Map (`Tools/Verification/StandMap/OfflineLogic/run_standmap_check.py`) | `165 passed, 0 failed` / `STANDMAP_OFFLINE_PASS` (143 pre-existing + 22 new presentation checks) |
| Suggested layout (`Tools/Verification/RackV2SuggestedLayout/run_offline.py`) | `SUGGESTED_LAYOUT_OFFLINE_PASS checks=147` + `SUGGESTED_LAYOUT_STATIC_PASS no UI dependency or rack mutation call` |
| Runtime type compile against the Unity 6000.6 reference assemblies | 172 sources, 116 references, 0 errors, 0 warnings (type/syntax gate only; it is not Unity's compiler run) |

### Editing Unity gate — real pointer (run by the previous executor, Codex `gpt-6.1-terra`; reviewed, not re-run, by the final executor)

`/media/jer/Files/CCF/Verification/RackV2-05-20261028/unity-editing-terra-cc64308-retry/` — `summary-lines.txt` ends `RACK_V2_EDITING_UNITY_PASS checks=326 shots=17`, lock `43fbfe7be0e64eb69bc8613329459c11`, lane U1 detached at `cc64308`, 2026-10-28T02:17Z–02:23Z.

- **Part A (facade semantics, fresh model-1 Enlarged80 world):** stable ids across every edit; one-level undo for create, move, rotate, delete and reset; reset reproduces exactly the five suggested routes and the suggestion source is not mutated; every refusal (zero/near-zero/just-under-minimum length, entrance off the headland or past the strip end, far end outside the stand east/north, non-finite endpoint, duplicate, overlapping, crossing, too-short rotate, move onto another rack, rotate outside the stand, unknown id, sixth rack) leaves the **whole save byte-identical** with no undo recorded; draft edits create no work order or mark; pending work reconciles without duplicates and other racks' orders are untouched; approved work asks first and cancels/recreates once with consent; confirmed plan refuses create/reshape/delete/reset; an executed rack is locked, other racks stay editable, the executed order is untouched; a projected pre-v21 model-0 save is not a V2 world and every edit is refused.
- **Part B (real desktop pointer and keyboard, 1280×720 and 1920×1080):** the OS pointer is calibrated against `Mouse.current.position` with a three-point fit whose third point validates within 3 px (error ≈6e-5 px at both sizes). At each size: a real click on `Plan Racks` enters planning; a real drag **creates exactly one rack** (id 1, entrance on the headland, near-level far end soft-snapped to the suggested direction, plan untouched while the button is down, no order or mark); a second rack is drawn at a deliberately skewed angle (≈8.6° / 8.8°, outside the 4° window — **deliberate deviation remains possible**); real clicks select; a real drag **moves** a rack ≈12 m along the headland with the entrance re-snapped (x unchanged despite a 6 m east drag); `Undo last rack edit` (real click) restores it exactly and only once; the round handle **rotates** with the entrance fixed; a near-level release snaps to level, and holding **Shift** (real key) turns snapping off; a too-short proposal shows the **red refused ghost** and releasing changes nothing (whole save identical) with the reason on the card; dragging a rack onto another is refused and nothing moves; `Delete rack` (real click), **Delete** and **Backspace** each delete the selected rack and are each undone; `Reset to suggested layout` **asks first** because edits would be lost, `Keep my racks` leaves the plan alone, `Reset layout` replaces it with exactly the five suggested routes, resetting an unedited plan needs no confirmation, and undo of the reset returns the player's racks with their ids; the save outside the rack plan is identical after all pointer editing; at 1920×1080 a confirmed plan hides the pointer layer and a real drag changes nothing, and an **executed rack** is drawn harvested, selects with a "locked" card, has `Delete rack` disabled, cannot be moved, has no handle and cannot be deleted by key, while another rack stays movable with pending work reconciled and not duplicated.
- **Captures:** 17 PNGs, all non-black (the harness rejects black frames; the final executor also checked pixel statistics of every file), in `…/unity-editing-terra-cc64308-retry/evidence/` (eight at 1280×720; nine at 1920×1080, including `08-harvested-locked`): `01-planning-empty`, `02-rack-drawn-selected`, `03-drawing-ghost`, `04-moving-ghost`, `04b-refused-ghost`, `05-after-refusal`, `06-reset-confirmation`, `07-after-reset`. `shots.json` records dimensions, map frame, selection and ghost state per shot. The harness rejects black frames (an earlier run's all-black set was caught and discarded).
- **Readability judgement (final executor, by viewing the images):** at both resolutions the selection outline, blue entrance square, white round rotation handle, grey dashed valid ghost, orange refusal text, edit card controls, reset-confirmation card and `Harvested rack, locked` legend row are legible (the red refused ghost itself is small; see Limitations), and the full map, overview, legend and card stay inside the window at 1280×720. Judged sufficient for the packet's affordance-readability requirement; full rendered-appearance acceptance remains RACK-V2-13.

### Preservation gates (run by the final executor; lock `8f9a85a7d7634b028d5734b09b0719a9`, lane U1 clean and detached at `cc64308`)

| Gate | Evidence | Result |
|---|---|---|
| RACK-V2-04 map planning (`--gate map`) | `…/claudeb-unity-map/`, `claudeb-unity-map.stdout` | Editor exit 0; `RACK_V2_MAP_PLANNING_UNITY_PASS checks=142 shots=8` (same 142-check count as RACK-V2-04's own qualification); world hash `CE21F222D1F189D9` and counters `1/2/0/False/0` identical to RACK-V2-04's record; fixture save before/after all rendered display work identical (`4ed7414c…`); eight captures non-black, dashed-vs-solid distinction intact |
| RACK-V2-03C runtime (`--gate runtime`) | `…/claudeb-unity-runtime/`, `claudeb-unity-runtime.stdout` | `RACKV2RUNTIME_VERIFY_PASS checks=14`, no `error CS`. The Editor hung after its own shutdown (log ends at `Cleanup mono`); the documented reaper killed it by PID only after the verdict and `Cleanup mono` and recorded it in `reaper.txt`; runner exit 0. The 14 checks include draft/confirm/reconfirm exactness, pre-approval edit and delete reconciliation without duplicates, approved-unexecuted warn-then-cancel-and-recreate, entrance-mark geometry binding, completed-work locking, save/load survival and the model-0 path |

The preserved gate sources `Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs` and `Tools/Verification/RackV2Runtime/RackV2RuntimeVerification.cs` are **byte-identical to BASE** (`git diff 140d8b8 HEAD` over both is empty): nothing was rebaselined.

### The map gate's fixture-save hash, and a BASE comparison

The map gate's fixture save SHA at `cc64308` (`4ed7414c…`) differs from the one in RACK-V2-04's own handoff (`b5e91e50…`). This was checked rather than assumed to be harmless:

- The same **unchanged** map gate was run at BASE `140d8b8` (diagnostic only; evidence `…/base-diagnostic/`, lock `43dfe3fecad947f9bd493fa9c9c68908`, 02:41:28Z, runner copy with `ROOT` pointed at U1): `RACK_V2_MAP_PLANNING_UNITY_PASS checks=142 shots=8`, world hash `CE21F222D1F189D9`, counters `1/2/0/False/0`, fixture save `4566619d…` equal before and after display work. That is a **third** distinct hash.
- The full-save SHA is simply not stable from run to run: two runs of the editing gate at the *identical* candidate `cc64308` (same production bytes, same harness) logged different fresh-world hashes (`46cdd690…` in `unity-editing-terra-cc64308/`, `0bf4ea25…` in `…-retry/`), and each earlier candidate logged yet another.
- What *is* stable, and what the gate asserts, is the world hash `CE21F222D1F189D9` (identical in RACK-V2-04's record, at BASE and at `cc64308`), the counters, and the within-run before/after equality of the complete save. All hold.

Conclusion: the hash difference is per-run variation, not movement caused by this branch. The cause of the per-run variation was not investigated further and is outside this task; it only means full-save SHAs must not be compared across runs.

### Non-mutation and preservation summary

- A suggestion read never mutates player rack state (offline check, editing Part A, map gate's before/after save equality).
- No counter was persisted or incremented independently; the rack count derives from the authoritative plan. Draft edits create no order or mark.
- Refusals leave the whole serialized save byte-identical (editing Part A, 1280×720 and 1920×1080 Part B).
- Legacy40 / model-0 / Reference Future v1: no Reference Future v1 file and no save/schema file is in the changed-file list. Legacy/model-0 behaviour is covered by the 143 pre-existing shared-map checks (unchanged and passing), editing Part A's model-0 refusals and the runtime gate's `model-0 legacy path remains viable` check. No save/schema change.
- `Tools/Verification` staging: after every gate U1 held no staged harness script or generated `.meta`; U1's only dirt was the four known Editor side-effect files (below), restored by exact path after each run.

## Unity lock receipts (final candidate)

All acquired normally with live coordination refreshed first; none forced; no Editor ran outside a held lock. Full chronology (including the earlier Claude A and Codex receipts `c0161354…`, `9c17990a…`, `c0d39801…`, `1c488819…`, `b176c024…`, `5084490f…`, `bdb3602d…`, `44c2be0e…`, `7d78f9b1…`, `6c99e32b…`) is in claim #43 and the coordination commit history.

| Receipt | Owner | Candidate | Use |
|---|---|---|---|
| `43fbfe7be0e64eb69bc8613329459c11` | Codex `gpt-6.1-terra` xhigh | `cc64308` | **Editing gate PASS** (326 checks, 17 captures); released normally by the final executor at 02:33:44Z with the exact owner/task/lock triple |
| `8f9a85a7d7634b028d5734b09b0719a9` | Claude B `claude-sonnet-5-5` xhigh | `cc64308` | Acquired 02:33:52Z. Map and runtime preservation gates; released normally 02:41Z with the exact triple (not forced) to re-record the head for the BASE diagnostic |
| `43dfe3fecad947f9bd493fa9c9c68908` | Claude B `claude-sonnet-5-5` xhigh | `140d8b8` (BASE) | Acquired 02:41:28Z. **Diagnostic only**: unchanged map gate at BASE; released normally (`forced: false`, `claudeb-lock-release-final.json`) at ≈02:44Z when all Unity work was finished |

**Final state:** the machine-wide Unity lock is `free`, no Unity Editor process is running, and U1 is clean and detached at `cc64308`. MFM-03 #39 may proceed under its own claim.

U1's four recurring Editor side effects (`.vscode/settings.json` and `Woodland_Forbs_01/Materials/Forb_{Flower_Centre,Leaves,Stems}.mat`) were saved as patches in the evidence folder and restored by exact path between runs; U1 was verified clean and detached before each acquisition.

## Evidence index

Root: `/media/jer/Files/CCF/Verification/RackV2-05-20261028/` (outside the repository; no Git LFS or storage-policy change).

| Folder | Contents |
|---|---|
| `unity-editing-terra-cc64308-retry/` | **Qualifying editing run** — `editing.log` (326 checks), `summary-lines.txt`, `evidence/` (17 PNGs, `shots.json`, `pointer-trace.txt`) |
| `claudeb-unity-map/`, `claudeb-unity-runtime/` | Preservation gates at `cc64308` |
| `claudeb-offline/` | Offline suites and type compile at `cc64308` |
| `base-diagnostic/` | Unchanged map gate run at BASE `140d8b8` (diagnostic only), runner copy and result |
| `unity-editing-run*-FAIL-*`, `unity-editing-terra-*` | Failed/superseded runs: history only |
| `lock-*.json`, `claudeb-lock-acquire.json`, `u1-side-effects-*.patch` | Lock receipts and preserved Editor side effects |

## Limitations (not hidden)

- **Not a Player build.** The pointer was a real X11 desktop pointer driven by `xdotool` into the Editor Game view under Xephyr, not native Windows input; no Player/Windows build was made or authorised. No user acceptance.
- **The final tested candidate's editing pass was earned by the previous executor.** The final executor re-ran the offline suites and the preservation gates, reviewed the captures and log, and did not re-run the editing gate.
- **The first attempt at `cc64308` produced a black capture** (`unity-editing-terra-cc64308/`, intermittent display condition); the harness rejected it and a retry with no code change passed. Black-capture rejection is in the harness; the map and runtime gates have no such check, so the map captures were checked by pixel statistics and by eye.
- The red refused ghost for a too-short proposal is a small stub at the entrance at 1280×720; the reason is carried by the orange card text. Readable, but a candidate for RACK-V2-13 polish.
- The `harvested/locked` capture exists at 1920×1080 only; the lock behaviour is asserted at both sizes in Part A and by real pointer at 1920×1080 in Part B. There is no separate overlap-refusal ghost capture (the overlap refusal is asserted and the card text captured in `05-after-refusal`).
- The `states` array in each `shots.json` is not strict JSON (unquoted strings, harness quirk); the `shots` array parses.
- Disk: ZX20 had ~19 GiB free against the 15 GiB floor, so no second Unity `Library` was created; Unity ran in the existing U1 lane and code/offline work in the Library-free worktree.
- `ScenarioOneUiRoot.cs` (3 lines) sits outside the packet's literal file list; and `Assets/ForestPrototype/Racks/RackEditing.cs` is a new file in the existing rack folder (the packet permits "rack-editing helpers under Assets/ForestPrototype").
- The calibration values above are deliberate gameplay calibration, not forestry facts.
- **No consent UI on the map:** an edit that would cancel approved work is refused with the facade's warning (see OPEN-3 section); the consent/reopen flow is RACK-V2-07's.
- **Per-run save hashes:** full-save SHAs vary run to run (see the BASE comparison), so only the world hash, counters and within-run equality are valid invariants.

## What remains for RACK-V2-06+ (not done here)

RACK-V2-06 spacing-teaching copy/visualisation and measurement UX (only minimal status plumbing exists); RACK-V2-07 confirmation, minimap and the reopen/confirmation user flow (the facade boundaries it will call exist and are tested); RACK-V2-08 first-person author removal; RACK-V2-09 orange field marking; RACK-V2-10 Help; RACK-V2-11 save/schema work and persistence qualification; RACK-V2-13 full rendered-appearance acceptance; whole Objective-1 functional qualification; MFM. No part of these was implemented or approximated.

## Integration note

`main` has moved only by coordination-file commits since BASE (`origin/main` was `a0c2689` at handoff time; the only changed file is `Docs/Project/coordination/active-tasks.md`), so the branch's 22 files do not overlap main's changes. Integration remains the Manager's decision; no PR was opened by this task.

## Complete changed-file list (`git diff --name-status 140d8b8..<tip>`)

```
A  Assets/ForestPrototype/Racks/RackEditing.cs
A  Assets/ForestPrototype/Racks/RackEditing.cs.meta
A  Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackEditing.cs
A  Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackEditing.cs.meta
M  Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs
M  Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs
M  Assets/ForestPrototype/UI/Map/StandMapLegend.cs
M  Assets/ForestPrototype/UI/Map/StandMapModel.cs
A  Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs
A  Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs.meta
M  Assets/ForestPrototype/UI/Map/StandMapSource.cs
M  Assets/ForestPrototype/UI/Map/StandMapSymbols.cs
M  Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs
M  Assets/ForestPrototype/UI/StandMapView.cs
A  Docs/Handoffs/RACK-V2-05/HANDOFF.md
A  Tools/Verification/RackV2Editing/OfflineCheck.cs
A  Tools/Verification/RackV2Editing/README.md
A  Tools/Verification/RackV2Editing/RackV2EditingVerification.cs
A  Tools/Verification/RackV2Editing/pointer_driver.py
A  Tools/Verification/RackV2Editing/run_gate.py
A  Tools/Verification/RackV2Editing/run_offline.py
M  Tools/Verification/StandMap/OfflineLogic/StandMapOfflineCheck.cs
```

(`TASK-PACKET.md`, `OPEN-3-MANAGER-DECISION.md`, `OVERNIGHT-COORDINATION.md`, `DISPATCH.md` and `CLAIM-TEMPLATE.md` were already on BASE.)
