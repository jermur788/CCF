# #360: Onboarding keyboard-focus repair and the last targeted Unity checks (RESULT)

**Worker:** Claude Sonnet 5.5 (`claude-sonnet-5-5`, Anthropic) in T3 Code; reasoning-effort setting not exposed to the worker. This is the same provider and model family as the #358 author, not the Manager-preferred GPT-6.1 Sol/Codex; the packet allowed "an observed capable Unity/C# implementer". **Self-review is not independence:** the author wrote and ran the fix and its probes, so #316 must review both.
**Date:** 2026-11-09. **Game source candidate (tested):** `fb599eecde983552dec4f06fb9efcf8e38397aed` on `integration/scenario-one-focused-playtest-20261109-v1` (parent `27fc985f`; original game source `045966d2` is an ancestor, not the tested candidate). This report is a separate, later docs-only commit.

## Verdict: **NEEDS CHANGES (narrow, verifier-only residual).**

* The **product defect is fixed and verified in the Editor**: Right → Enter now goes forward at both sizes, with real OS key events and with synthetic Input System events. No other product finding.
* **Session 1 still ends `CCF359_V2_CLOSURE_FAIL` (171 PASS / 2 FAIL).** Both FAILs are `FORCED_LESSON_5/7_INDEX_TAKES topic=0`: my correction for probe defect 1 (adopt `TeachingWorldVersion` first) was **not sufficient**, so the synthetic stage 5/7 Work Plan lock presentation is still **NOT VERIFIED**. That is a verifier defect, not a game finding, but I do not call it PASS.
* Sessions 2 (old world) and 3 (v24 save contract) **PASS** on the candidate.
* Not qualified for external demo; no merge, no #316 launch, no Player build, no personal-test release (Manager's call).

## 1. Cause of the focus defect (traced, not guessed)

Traced in the Unity 6000.6.0f1 `UnityEngine.UIElementsModule.dll` IL (Mono.Cecil dump, receipts in the evidence folder):

1. `RuntimePanel` builds a **`NavigateFocusRing`** (directional/spatial navigation), so arrow keys move focus.
2. `ScenarioOneUiRoot.HandleKeys()` polls the same Right press and calls `Onboarding.Next()`; `Render()` recreates the buttons and schedules `Focus()` on the new Next button.
3. The same press also reaches UI Toolkit as `NavigationMoveEvent(Right)`. With focus on Next (the rightmost control) the first spatial pass finds nothing to the right, the second pass wraps (`FindBestInDirection(..., firstPass:false)` uses the opposite side) and **focus lands on "← Back"**.
4. `NavigationMoveEvent.PostDispatch` always calls `SwitchFocusOnEvent`, which returns early only if the event was marked `processedByFocusController`; `EventBase.PreventDefault()` does exactly that (`StopPropagation` + `FocusController.IgnoreEvent`). The same wrap happened for Left.

**Fix (one game file, +10 lines, `ScenarioOneOnboardingView.cs` only):** `Root.RegisterCallback<NavigationMoveEvent>(KeepArrowKeysForPageChanges, TrickleDown.TrickleDown)`, which calls `PreventDefault()` for `Direction.Left`/`Right` only. Tab/Shift+Tab (`Next`/`Previous`), Up/Down, Enter, Esc, mouse, the deliberate primary-button `Focus()`, copy, art and layout are untouched. `ScenarioOneUiRoot.cs` is **unchanged** (a view-local change was sufficient, so the conditional root edit was not used). Up/Down still use spatial navigation (they can wrap to Back); I left that alone as out of scope.

## 2. Offline gates (before any Unity)

* Roslyn typecheck (netstandard2.1, real 6000.6 DLLs + U3 `Assembly-CSharp.dll`) of the changed view and all three probes: 0 errors, 0 warnings. **Negative controls** both rejected (`Direction.Sideways` CS0117; `ui.Learning.NoSuchMember` CS1061). Typecheck also caught my first use of an inaccessible `CurrentLesson`, fixed before publishing.
* `check_focus_source.py` (registration phase, exact Left/Right-only guard, Tab path and primary focus intact): real file PASS; **8/8 mutants caught**; the original source FAILS. Static only, not runtime proof.
* Diff vs `27fc985f`: exactly 3 paths. Diff of game dirs vs `045966d2`: **only** `ScenarioOneOnboardingView.cs`; no Packages/ProjectSettings/scene/.meta change. The other probes, historical harnesses, strict Workbench runner and #358 witness files are byte-identical.

## 3. Unity admission and lease

Lane `/media/jer/ZX20/CCF-U3`, previously clean at `045966d2` on `qualify/scenario-one-ui-lifecycle-20261109-v2`; I created a **new local branch `qualify/scenario-one-onboarding-focus-20261109-v3`** at `fb599eec` (non-forced switch, same warm lane/Library). Admission (holder.sh, fresh): free **36.14 GiB** (≥35), quiet I/O 0.0 MB/s, 20 s disk monitor (28/25/20 thresholds; **minimum observed 36.13 GiB**), no Unity process, free lock, 27/27 licensed pack hashes, user saves/prefs hashes, scene/packages/ProjectVersion/Reference Future v1 archive+.meta hashes, original Workbench 8-dirty identity (status + hashes), Files stage lane clean and equal to the pushed tip. Lock **`9e12a68f371e4be39da8f5115d6ae658`** (owner "Claude Code (Sonnet 5.5) via T3 Code #360", task `CCF-360-ONBOARDING-FOCUS-REPAIR-AND-LAST-UNITY-GATES`, lane/branch/HEAD verified by `status --json`), held ~10 min, **released owner-safe by that id** (`forced: False`), then `Unity lock: free`, no Editor, U3 clean at `fb599eec`. Three Editor processes, one lease, private Xephyr display, throwaway `XDG_CONFIG_HOME`, licence copy removed after each run.

## 4. Session results

| # | Probe | Result | Notes |
|---|---|---|---|
| 1 | `Ccf359V2Closure` (#360 bytes, SHA `39feddbe…80bc`), 1920x1080 + 1280x720 | **FAIL** 171 PASS / 2 FAIL | 283 s; Editor exit code is -9 because the runner's post-verdict reaper killed it after its own FAIL verdict and `Cleanup mono` |
| 2 | `Ccf359OldWorld` (unchanged), `CCF_STAND_GEOMETRY=0` | **PASS** 39 checks, 12 shots | 108 s |
| 3 | `Ccf359V24SaveContract` (unchanged) | **PASS** 25 checks | 76 s |

### Session 1: keyboard (the point of #360), both sizes
* **OS-injected** (xdotool key down/up on the private Xephyr display via `native_input.py`, so X server → Unity Linux input → New Input System; **not a physical keyboard**): Right → page 2 with **Next focused**; Enter → **page 3 (forward, not back)** with Begin focused; Left → page 2, Next focused; Esc → page 1, Next focused. All PASS at 1920x1080 and 1280x720.
* **Synthetic** (queued Input System state): Right/Enter forward, Left back, **Tab → Back focused, Shift+Tab → Next focused, no page change**, Enter after Tab/Shift+Tab advances exactly one page to Begin, double Esc returns to page 1 with the modal still open. PASS at both sizes.
* **Mouse** (a `Clickable` invocation, not a real pointer): Next → page 2 and Back → page 1, Next focused. PASS.
* Retained original checks PASS: F1 cannot replace/skip onboarding, Esc on page 1 does not skip, Begin closes the modal, **no focus left on a hidden onboarding element**, W moves the player afterwards (3.19 m). A real physical-keyboard test has **not** been done.

### Session 1: the three corrected probe defects
1. **World-version sync / forced stages: NOT FIXED.** Stage 0 takes and its lock count is PASS (3 locked). Stages 5 and 7 still read `topic=0` (`FORCED_LESSON_5/7_INDEX_TAKES` FAIL). The probe correctly **failed closed**: lock counts for 5 and 7 are reported "NOT VERIFIED", and every forced stage is labelled SYNTHETIC. The remaining cause is not isolated and I did not guess a second fix in a fourth session (candidates, unverified: the facts being cleared by a later world/work-plan refresh after the last `ShowWorkPlan`, or the V2 `Bay`-aware branch of `CurrentUnlatched` needing facts the injection does not set). Real stage 5/7 play stays **NOT RUN**.
2. **Figure-only font floor: FIXED, PASS** at both sizes. 18 figure-owned labels (title, in-frame labels/legend, 2 notes), smallest **20** ui units; exactly one excluded direct label, the lesson caption (18, class `diagram-caption`, after the notes); negative control proves the same threshold rejects the caption size.
3. **Stale map during Reference Preview: FIXED.** Strokes are counted only when the Stand Map is the open screen, shown, and `PaintedVersion == Model.Version` (after-preview and V2 states: 8 strokes, painted 43 = model 43). During the preview the map is not displayed (painted 41, `shown=False`): recorded **NOT VERIFIED** for strokes; model headland count 0 and 3D surface 0 active are asserted and PASS.

### Other Session 1 PASS (current source, not a broad rerun)
Fresh V2 world, surface create/deactivate/reactivate (same object, no duplicate), mesh ↔ model ↔ strokes numeric alignment with negative controls, five-rack figure structure and fit, planning/confirmed Mini/Full parity, Help open/close/focus. Antecedent #359 broad checks were not intentionally rerun, but these ran as part of the same probe and none regressed.

### Session 2 (old world) / Session 3 (v24 save contract)
Session 2: Legacy40 new game and Reference Future v1 preview show no V2 surface, headland strokes, five-rack figure or V2 wording, including after the preview. Session 3: v24 hash identical to the independent strip (`A4C22867F535328B`), the historic save03b strip does **not** match (provenance witness, `C4FF53A70F84AAA0`), the guided-review latch never moves an identity, nine semantic rejections, `RestoreSaveData(null)` stays model 0, V2 never active on Legacy40, a model-1 Legacy40 save refused. This covers the uncovered v24 cases; the historic save03b FAIL and the UxPedagogy 778/38 FAIL are **unchanged and not relabelled**. Pre-V2 model-0 fixtures are not a Legacy40 new-game equivalent and were not used as one.

## 5. A–O

| Gate | Result |
|---|---|
| A Import/compile | **PASS** on `fb599eec` (three sessions, 0 `error CS`) |
| B Surface lifecycle | PASS (this run, current source) |
| C Map vs 3D | PASS (this run); human visual review still the reviewer's |
| D Full vs Mini | Historic StandMapParity FAIL ×2 retained; V2 witness and planning/confirmed parity PASS (this run) at stated scope |
| E Legend / paths | NOT VERIFIED (no observer comprehension check) |
| F Performance | **MEASURED, NOT VERIFIED** (not asked for; not rerun; no baseline) |
| G Five-rack diagram | **PASS** including the corrected figure-only floor |
| H Old-world guard | **PASS**: Legacy40 new game + Reference preview (Session 2); pre-V2 load and V2-origin preview PASS (Session 1) |
| I Card fit / guidance | UxPedagogy canonical FAIL retained; forced-stage 5/7 locks **NOT VERIFIED (probe FAIL, see above)**; real stage 5/7 **NOT RUN** |
| J Native controls / focus | **PASS (synthetic + X11-OS-injected, both sizes)**; physical keyboard NOT RUN |
| K Work Plan / MFM | PASS inherited, not rerun |
| L Workbench retirement | 107/107 in-Editor inherited; **strict runner exit 1 remains FAIL**; #287 NOT FULLY QUALIFIED |
| M CTA / feedback checker | PASS source-static inherited, not rerun |
| N Save/load | save03b canonical + diagnostic FAIL retained; **v24 save-contract replacement PASS (25)** |
| O Final integrated regression | NOT RUN |

## 6. Protection and restores

After each Editor stop and at lock release: protected scene/packages/ProjectVersion/Reference Future v1 archive+.meta hashes and the four user save/prefs hashes **unchanged**; 27/27 licensed pack hashes OK at entry; original Workbench worktree untouched (entry identity verified); Reference Future v1 not edited. Each Editor regenerated only members of the committed finite envelope (Section Five `.mat` and the U3 `.vscode/settings.json`): session 1 → 8 paths, session 2 → 12, session 3 → 4. `restore12.sh` verified exact known generated SHA + HEAD blob for each, saved before-images, restored **worktree only** with the index unchanged, and left the lane clean. No test verdict was changed by a restore. The temporary probe copies and `.meta` files were removed from `Assets/ForestPrototype/` after each run. Minimum ZX20 free **36.13 GiB**, final 36.14.

## 7. Manager decisions needed

1. Accept the keyboard fix as a candidate for #316 (my recommendation), and decide whether the residual probe defect (forced stage 5/7 injection) justifies another bounded Editor run or stays a disclosed NOT VERIFIED for the first internal Editor-only playtest.
2. Whether Up/Down arrows wrapping focus to Back during onboarding needs changing (not requested, not changed).
3. Physical-keyboard check remains a user action.

## 8. Evidence

`/media/jer/Files/CCF/Verification/CCF-360-OnboardingFocusFix-20261109/` (manifest `evidence-manifest.sha256`, 204 files; sha256 of the manifest `1e8d532bce4505888f766917a1bd1862c4ffba414382acd224e4e895b484c750`): `offline/` (IL dump tool + receipts, typecheck projects, `check_focus_source.py`), `run/session{1,2,3}-*` (Unity logs, evidence, shots, staged-sources), `run/gates/*` (hashes, status, restore proof), `run/holder.log`, `run/unity-lock-*`, `run/disk-monitor.log`, `run/restores/*`. Scripts are #359's holder/gate/restore12/run_gate with task id, pins and branch changed. The #359 raw `CCF359_V2_CLOSURE_FAIL` receipt and the #359 probe bytes (`81818e7f`) are preserved.
