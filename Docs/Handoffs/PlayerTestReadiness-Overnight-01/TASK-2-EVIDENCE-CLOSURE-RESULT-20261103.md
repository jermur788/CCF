# Scenario One Tier-3 — Task 2 Evidence Closure Result — 2026-11-03

**Claim:** #240 (parent #231)  
**Handoff:** `Docs/Handoffs/ScenarioOne-Tier3-PlayerTest-Candidate/TASK-2-EVIDENCE-CLOSURE-HANDOFF-20261103.md`  
**Worker:** Claude Code / Anthropic `claude-opus-5-5` (Claude Opus 5.5), local Unity/code worker in T3 Code  
**Local result:** all three evidence actions **PASS**; no candidate production defect demonstrated  
**Independent delta review:** see `FINAL-INDEPENDENT-REVIEW.md` (Task 2 section)

## 1. Identity

| Field | Value |
|---|---|
| Original production candidate | `6849caa0ef05baf1a52b95465ec6bba4ff94202b` |
| Task 1 persisted tip (start) | `2a232885912857a1d1c2e43d4f147df1cc0888c6` |
| Task 2 evidence head | `c648ab299fd6e0b63eb104ae127c96f7def0f8b8` (single verification-only commit on top of `2a232885`) |
| Production diff `6849caa0..c648ab29 -- Assets Packages ProjectSettings` | **empty** |
| Lane | `/media/jer/ZX20/CCF-U1`, branch `qual/scenario-one-final-tier3-v1` |
| Unity | 6000.6.0f1 |
| Unity lock | `7c43a0947057409999b9fa208bcd0354` acquired at `c648ab29` 13:06Z, released normally; final status FREE |
| Evidence root | `/media/jer/Files/CCF/Verification/ScenarioOneFinalTier3-Task2-20261103/` (35 MB) |
| Windows build | none; #194 NO ACTION |

Preflight (fresh): active-tasks.md re-read from `origin/main`; #231/#240 refreshed; lane clean including untracked at `2a232885` = pushed tip; lock FREE / no Editor PIDs; no Unity/Xephyr/openbox/xdotool processes; host saves `forest-save.json` `7effe9b7…`, `.bak` `c0169790…`; Task 1 evidence root intact (727 MB); ZX20 22 GiB free. Task 1 was **not** rerun.

## 2. Verification-only changes (commit `c648ab29`)

| File | Change |
|---|---|
| `Tools/Verification/ScenarioOneEconomyIntegrationVerification.cs` | Yields **one normal frame** between `TryBeginReferencePreview(20)` and `EndReferencePreview()` (the product path: button entry `WorkPlanView.cs:584`, Tab/Esc exit read via `wasPressedThisFrame` in `ScenarioOneManager.Update`, `ScenarioOneManager.cs:584-587`). Proves the Year-20 world really replaced the forest at the later frame, then asserts saved tree count + identity, live scene tree objects, version/geometry, ecological year, shelter state and **exact canonical save equality**, plus the existing WorldHash check. No harness restore/reload before the assertions. |
| `Tools/Verification/Mfm12IntegratedQualification/Mfm11CalibrationImpactGate.cs` | New `CaptureCompletedState` at the actual Year-25 Completed point, **before** the save/load and baseline restore; walking HUD + Annual Review objectives heading + outcome line at both sizes; visibility/unclipped and non-black content checks; world hash unchanged after. Old end-of-run shots renamed `mfm11-after-baseline-restore-pathC-*` (they never showed completion). |
| `Tools/Verification/Mfm12IntegratedQualification/run_gate.py` | Adds the logged Editor window keeper (the extension runner already has one; its absence explains the black Task 1 captures). Acts only on geometry/focus change. |
| `Tools/Verification/RackV2Editing/RackV2EditingVerification.cs` | Records pointer-up and the per-drag world path; on the handle rotation asserts real release within 1 m of the requested endpoint and path deviation < 2 m; writes `rotation-receipt-<size>.json`. Existing angle (6–14°) and fixed-entrance assertions unchanged. |
| `Tools/Verification/RackV2Editing/pointer_driver.py` | Logs the OS pointer (`xdotool getmouselocation`) immediately before each press/release. |
| `Tools/Verification/RackV2Editing/run_gate.py` | Window keeper is logged (`focuser.log`) and acts only on geometry/focus change (previously re-moved/resized/activated every 2 s). |
| `Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs` | Stale `Count == 5` replaced with the exact accepted six-tab V2 set (`Access & position`, `Light`, `Regeneration`, `Browsing / protection`, `Fell & crop marks`, `Treatment review`). Plan-Racks-is-not-a-tab, sibling and selection-preservation assertions unchanged. |

## 3. A — Reference-preview normal-path discriminator: **PASS (no state loss)**

Run: `run_qualification.py`, batch Editor, label `A-preview-frame-separated` (`A-preview-frame-separated/`). Protected-player fixture: owner sheltered sessile-oak planting, shelter `S-JPJ0001`, Year 1, v24, geometry 1.

```
ECONOMY_PREVIEW_CAPTURE trees=1344 version=24 geometry=1
ECONOMY_PREVIEW_FRAME_SEPARATED entryFrame=20 exitFrame=21 previewYear=20 previewLiveTrees=352 playerLiveTrees=1344
ECONOMY_PREVIEW_EXIT shelters=1 before=D6A4223F7A641C80 after=D6A4223F7A641C80 version=24 …
ECONOMY_PREVIEW_TREES beforeCount=1344 afterCount=1344 missing=[] extra=[]
ECONOMY_PREVIEW_RESTORE_FRAME_SEPARATED exitFrame=21 assertFrame=23 savedTrees=1344->1344 liveTrees=1344->1344 year=1->1
  version=24->24 geometry=1->1 shelters=[S-JPJ0001]->[S-JPJ0001] browseShelters=1 previewActive=False canonicalEqual=True
ECONOMY_PREVIEW_PLAYER_POSITION present=True delta=0.0000
```

Every preview assertion passed, including the original *“preview exit replaces exact saved protection/player state”*. Exit after one normal frame restores the exact protected-player world. With the Task 1 same-frame receipt (`49i`: 1344 → 335) this confirms the failure is the **same-frame harness race only**; the normal player path does not lose state.

**Out-of-scope observation (not one of the three actions; not fixed):** the gate's terminal marker is `SCENARIO_ONE_ECONOMY_INTEGRATION_VERIFY_FAIL … v14 safe migration`, a later check Task 1 never reached. Production refuses the harness's synthetic fixture with *“Save rejected before loading: a save older than version 20 cannot carry a stand geometry model. The current forest was not changed.”* The fixture relabels a current geometry-1 world as `version=14`, a combination the accepted v20+ geometry rule deliberately rejects before mutation (consistent with accepted MFM-06/hardening evidence). Classified as a **stale verification fixture**, not a product defect. The economy checks after that line (failed designation, invalid planting, invariance, ten-year mid-plan continuation) remain unexecuted on this candidate in both Task 1 and Task 2. A mechanical fixture correction (v14 relabel with Legacy40 geometry) is recommended for Browser Overall disposition; it was not made because it is outside the Task 2 edit authority. The world was restored and no save slot was written.

Compile proof: the edited six-tab map harness was staged as a compile-only companion in the same run (its auto-start needs its own command-line marker). `compile_errors: []`, `unexpected_tree_changes: []`.

## 4. B — Rack 1920×1080 clean native-input rotation: **PASS**

Run: `RackV2Editing/run_gate.py --gate editing --display :86 --screen 2560x1440 --sizes 1920x1080` (`B-rack-rotation-1920/`). Fresh private Xephyr/openbox. The 2560×1440 screen holds the whole 1920×1080 Game view (Task 1 used a 1920×1080 screen: calibration `by=1124` put the view's bottom edge off-screen). Calibration: `ax=1 bx=-320 ay=-1 by=1304`, validation error 0 px.

`RACK_V2_EDITING_UNITY_PASS checks=241 shots=9`, exit 0, not reaped.

Rotation receipt (`evidence/rotation-receipt-1920x1080.json`):

| Item | Value |
|---|---|
| Requested screen path | (1245.15, 1043) → (1203.87, 908.84) |
| OS pointer observed before press / release (driver.log) | X=1245 Y=1043 / X=1204 Y=909 |
| Requested world endpoint | (31.98, -10.95) |
| Observed release (game) | world (31.996, -10.969); mouse (884, 395) |
| Release error / max path deviation | **0.020 m / 0.127 m** (10 traced events) |
| Angle | 0° → **8.68°** (contract window 6–14°) |
| Entrance | (-35.4, -23.95) → (-35.4, -23.95), **fixed** |

Checks passed: real release received; release at the requested endpoint; path on the requested path; *“rotating well off level is free (8.681965 deg), and the entrance did not move”*; rotated rack keeps id 1; near-level release snaps to level; valid ghost while rotating. The window keeper's last action was ~70 s before the drag (`focuser.log`), none during it.

Task 1's failure is explained by input contamination: its trace shows the game pointer jump from (885,394) to (0,506) mid-hold with no corresponding request, then a release at world (-49.88,-30.74). The clean gesture rotates correctly. **No product defect.** 720p was not rerun.

## 5. C — Real Year-25 Completed rendering: **PASS**

Run: `Mfm12IntegratedQualification/run_gate.py --gate calibration --display :87` (`C-completed-render/`). `MFM11_UNITY_PASS checks=95 shots=8`, exit 0, `pass=True`, host saves unchanged, harness removed, no lane changes, no disk saves.

Data witness (identical to the accepted Task 1 witness): `outcome=Completed outcomeYear=25 finalYear=25 cash=348152`, `8 objectives, 0 unachieved`.

Captures taken at the Completed state before any save/load or baseline restore (`evidence/`):

| File | Shows | In-Unity lit fraction / luma SD | Independent PIL mean / SD |
|---|---|---|---|
| `mfm11-completed-hud-1280x720.png` | HUD “Scenario One · Year 25 · objectives met · 5 interventions”, €3481.52 | 0.999 / 0.111 | 41.2 / 27.0 |
| `mfm11-completed-review-objectives-1280x720.png` | Annual review · Year 25 complete; `OBJECTIVES 8 OF 8` heading | 0.999 / 0.112 | 38.7 / 32.5 |
| `mfm11-completed-review-outcome-1280x720.png` | all 8 `✓ done` lines + “Scenario objectives met in year 25. Continue to the Century Review (year 100).” | 0.999 / 0.115 | 38.2 / 32.6 |
| `mfm11-completed-hud-1920x1080.png` | HUD as above | 1.000 / 0.110 | 40.8 / 27.0 |
| `mfm11-completed-review-objectives-1920x1080.png` | heading, all 8 objectives and the outcome line in one frame | 1.000 / 0.127 | 37.0 / 31.7 |
| `mfm11-completed-review-outcome-1920x1080.png` | same card, outcome line | 1.000 / 0.135 | 36.9 / 31.4 |

Gate checks: Completed year = outcome year = 25; 8/8; HUD status line on-screen and unclipped; review heading `OBJECTIVES 8 OF 8`; every objective line present; heading and outcome line fully inside the scroll viewport and unclipped; captures contentful; **world hash unchanged** by the presentation. Screenshots were also inspected visually: readable at both sizes.

Black-capture cause confirmed: with the window keeper added, the renamed post-restore Path C captures are also non-black (mean ≈ 52).

Honest presentation note: the HUD's guided-curriculum card still reads “Objective 2 of 9” and “Rack entrances marked 0/5”, because this scripted witness uses direct entry and bypasses the guided curriculum and physical marking. It does not contradict the scenario Completed state. A player who followed the curriculum would have advanced it; that is not demonstrated here.

## 6. Rack six-tab correction

Mechanical verification-only correction made (§2). Compiled inside run A; not executed, by instruction (no full Rack product rerun solely for it).

## 7. Cleanup / final state

- Lane `/media/jer/ZX20/CCF-U1` clean including untracked at the docs head (after this commit). Editor rewrites of `.vscode/settings.json` and three SectionFive `.mat` files were captured to `A-side-effects.patch` / `B-side-effects.patch` and restored byte-exactly.
- No staged harness or `.meta` under `Assets/ForestPrototype/`; copied licences removed.
- No Editor, Xephyr, openbox, pointer/native-input driver or focuser running.
- Unity lock FREE (`final-lock-status.json`); host saves unchanged (`7effe9b7…` / `c0169790…`).
- No Windows build; #194 NO ACTION.
