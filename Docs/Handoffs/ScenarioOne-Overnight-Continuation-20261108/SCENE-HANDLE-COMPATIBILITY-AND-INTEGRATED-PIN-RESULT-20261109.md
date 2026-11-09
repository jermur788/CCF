# #355 Scene-Handle Compatibility and Integrated Pin Result — 2026-11-09

**Verdict: READY FOR MANAGER SOURCE/INTEGRITY REVIEW** (offline scope only). The Stand Map `CS0619` defect that blocked #354 is repaired as the Manager specified. The repaired source compiles with 0 errors against the real Unity 6000.6.0f1 reference DLLs, and CTA check 08 is back to 10/0/2. This is not Unity compilation and not a Unity-qualified, integrated, main-ready or user-accepted candidate. Unity was not run.

Binding packet: [Manager #354→#355](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-354-TO-355-SCENE-HANDLE-COMPATIBILITY-PACKET-20261109.md). Claim: [#355](https://github.com/jermur788/CCF/issues/355). Parent: [#354 result](https://github.com/jermur788/CCF/blob/b63309260ade1ec6f4062b932cc90e285d5656a5/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/FOCUSED-SOURCE-STAGING-RESULT-20261109.md).

## 1. Identity

| Item | Value |
|---|---|
| Executor | Claude Code, Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`), through T3 Code. Reasoning effort was not exposed to the session. |
| Branch / worktree | `integration/scenario-one-focused-playtest-20261109-v1` / `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109` (store `/media/jer/Files/CCF/Repository.git`) |
| Start | `b63309260ade1ec6f4062b932cc90e285d5656a5` (#354 report tip). Equal to origin, tracked/untracked/ignored clean, no `Library`, no Unity process, no competing owner in live `active-tasks.md`, #355 READY. |
| #354 source tip (old) | `04d65a56ae0ce6ae8b61e8a0cb0687e177978e64` |
| New source commit | `7dc327903d62e3ab14a09527cae01ab48e1659a6` (5 files) |
| Report commit | The branch tip once this file is committed (it cannot name its own hash). It is recorded in the #355 hand-off comment. |
| Evidence | `/media/jer/Files/CCF/Verification/FocusedSourceStage-20261109/355/` with `evidence-manifest.sha256` (Files, not ZX20) |

## 2. The smallest source diff

`Assets/ForestPrototype/UI/Map/StandMapSource.cs` — exactly the four replacements the packet approves, and nothing else:

```diff
 using UnityEngine;
+using UnityEngine.SceneManagement;
...
-    private int headlandScene = -1, headlandLookupAttempts;
+    // The scene the cached headland belongs to. A SceneHandle is stored as-is (Unity 6000.6 deprecates converting it to int);
+    // null means no scene has been cached yet, which is what the old -1 meant.
+    private SceneHandle? headlandScene;
+    private int headlandLookupAttempts;
...
-            headlandScene = -1;
+            headlandScene = null;
...
-        int scene = eco.gameObject.scene.handle;
+        SceneHandle scene = eco.gameObject.scene.handle;
```

The existing `headlandScene != scene` condition and `headlandScene = scene` assignment are unchanged, as are all gating, lost-reference, cadence, counter and invalidation logic. The field declaration had to be split in two because one declaration cannot mix `SceneHandle?` and `int`. This is a transient presentation cache, so there is no save or schema effect.

| StandMapSource.cs | git blob | SHA-256 |
|---|---|---|
| Old (anchor `1b68ddcd`, D-072 tip `9789369d`, #354 source tip, #354 report tip: all identical) | `70f38210d6f7e3417c8483054bb2985d420888fd` | — |
| New (`7dc32790`) | `e1be7d1edfe252be1e536a1d6896251874bf468b` | `423985588083bba5b901254b0019fc2f6ade49c2b7ea9a02c12bd8ddeb0c0093` |

**Not used (not approved):** no `(int)` cast, `unchecked`, truncation, `GetRawData()`-to-`int`, `ulong` sentinel, API shim, suppressed error or changed cache policy. Nothing was improvised: the packet's STOP condition (a C# 9 constraint on the real DLL) did not occur, because `SceneHandle? != SceneHandle` binds against the real `SceneHandle`.

### Verification files changed (only these four, all under `Tools/Verification/StandMap/HeadlandOffline/`)

* `HeadlandSourceStubs.cs`: `Scene.handle` is now a `SceneHandle` stub. It has **no `int` conversion**, equality by raw value, `==`/`!=`, `IEquatable`, `GetRawData()`. The old `int` stub was the reason no source-only check saw the engine mismatch. Like the other stand-ins it lives in the `UnityEngine` namespace, which the seam already uses. The real namespace `UnityEngine.SceneManagement` is covered by the real-DLL compile below, not by this stub.
* `HeadlandOfflineCheck.cs`: one line. The scene-change fixture now builds `new Scene { handle = new SceneHandle(2) }`. The expectation is unchanged: "scene identity invalidates cache immediately".
* `run_headland_check.py`: a new `--integrated` flag plus a separate `check_integrated_identity()`. The only line removed is the single `check_stage_inventory()` call, which became a mode choice. `check_stage_inventory`, `ANCHOR_297`, the stage constants and every stage assertion are byte-untouched. The seam construction was **not** edited.
* `test_stage_inventory_guard.py`: a new `--integrated` option and an `INTEGRATED_MUTATIONS` table. The historical `MUTATIONS` table is untouched, and default-mode output labels are unchanged.

## 3. Results

All commands ran offline from the candidate worktree (Python 3.12.3; .NET SDK 8.0.133 pinned by an external `global.json` working directory where the Roslyn stage needs it). No file was written inside the repo, and `git status` was empty after every run.

| # | Check | Command (abridged) | Expected | Actual |
|---|---|---|---|---|
| 1 | **Real-DLL compile** (before any harness edit) | `objective_cta_roslyn_binding.py` from the SDK-8.0.133 cwd | 0 errors, full source set | **PASS**, exit 0, 15 s: `files=189 … errors=0`, `sourceSetSha256=071641fd963d…`. C# 9, Player defines, 320 references verified by SHA-256, manifest `978123241d3d`, all taken from `/media/jer/ZX20/Unity/6000.6.0f1/`. The source set is not reduced: 189 player-like `.cs` files on disk, 189 compiled (198 `.cs` in total, 9 in Editor folders). |
| 2 | **CTA `check_baseline.py`** (key check 08) | `--rotation-state post-fix --instruction-state post-fix`, SDK-pinned cwd | 10 PASS / 0 FAIL / 2 NOT-TESTABLE | **PASS**, exit 0: `PASS=10 FAIL=0 NOT-TESTABLE=2`. Check 08 PASS with `0 compile errors` (second compile receipt, on the committed tree), `Objectives visibility update x1 -> ScenarioOneUiRoot.Update()`, and the six real `SnapFar` cases. The 884/112 campaign was **not** re-run (CTA checker bytes unchanged; see 6). |
| 3 | **Headland, integrated mode** | `run_headland_check.py --integrated --out <new>` | pass | **PASS**, exit 0, 20 s: 42 SOURCE assertions (29 retained behavioural/source plus 13 new `integrated:`) and `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0`, including **"scene identity invalidates cache immediately"** on the typed stub. Final marker `HEADLAND_INTEGRATED_IDENTITY_AND_OFFLINE_PASS`. |
| 4 | **Integrated negative controls** | `test_stage_inventory_guard.py --integrated` (disposable sparse clone at `7dc32790`) | clean ×2 pass; 8 mutations each caught by its intended assertion | **PASS**, exit 0, 27 s: `INTEGRATED_IDENTITY_INTEGRITY_TALLY MUTATIONS=8 CLEAN_RUNS=2 FAIL=0`. Detail below. |
| 5 | **Original stage pin, original fixture** | `test_stage_inventory_guard.py --commit 9789369d` with the edited runner | unchanged: 39 SOURCE / 92 C#, historical mutations all caught | **PASS**, exit 0, 26 s: both clean runs `PASS_SOURCE=39 C#=92/92`; `MUTATIONS=20 CLEAN_RUNS=2 FAIL=0`; 20 `PASS CAUGHT` lines. (The packet's "19" was an approximate count; the table holds 20.) |
| 6 | Stage mode on the combined tree | `run_headland_check.py` (no flag) | fails by design | exit 1: `stage 1: present and byte-identical to accepted #297 content Assets/ForestPrototype/UI/Map/StandMapSource.cs`. See section 5. |

### Integrated identity assertions (the 13 new `integrated:` PASS lines)

1. The accepted `StandMapSource.cs` is recoverable at the #297 anchor `1b68ddcd` and at the D-072 tip `9789369d`, each as pinned blob `70f38210`. (2 assertions)
2. Each of the four edits applies to exactly one place in the accepted bytes. (4)
3. The file equals the accepted #297 content plus only those edits, byte for byte. (1)
4. Explicit pins: SHA-256 `42398558…0093` and blob `e1be7d1e…` (these cannot drift together with the edit table). (2)
5. Production paths differing from the common ancestor `50042bbe` (tracked or untracked) are exactly the 9 Workbench/C2 paths and the 11 D-072 Assets paths, 20 in all. `StandMapSource.cs` is one of the 11, so there is no new path, no `.meta` and no save, scene or package drift. (1)
6. The 9 Workbench/C2 paths (including `ForestTest.unity`, `Packages/manifest.json` and `packages-lock.json`) are byte-identical to C2 `b82cb240`. (1)
7. The other 10 D-072 Assets paths are byte-identical to the approved D-072 tip. (1)
8. `ProjectSettings/` is byte-identical to C2. (1)

### Integrated negative controls (all caught by the intended assertion)

| Case | Rejected by |
|---|---|
| i1 `StandMapSource.cs` altered by one space | accepted-plus-approved-edits |
| i2 reverted to the CS0619 `int` cache | accepted-plus-approved-edits |
| i3 scene-change comparison disabled (`false && headlandScene != scene`) | accepted-plus-approved-edits |
| i4 extra file committed under `Assets/` | production path set |
| i5 extra untracked `.meta` under `Assets/` | production path set |
| i6 C2 `Packages/manifest.json` altered | C2 byte identity |
| i7 C2 `ForestTest.unity` altered | C2 byte identity |
| i8 another D-072 file (`WorkPlanView.cs`) altered | D-072 byte identity |

## 4. Protected bytes and allowed-delta audit

* **Delta against the #354 report tip is exactly the 5 authorised files** (+135/−14): `StandMapSource.cs` and the four `HeadlandOffline` files above. No other `Assets/`, `Packages/`, `.unity`, `.meta`, `ProjectSettings/`, save/Reference data, five-rack checker, CTA checker or Workbench/C2 file differs.
* **Against C2:** 0 differences in the 9 Workbench/C2 paths, 0 in `Assets/Scenes`, `Packages/` or `ProjectSettings/`, and 0 `.meta`, `.unity`, `.prefab` or `.asset`. The C2 Reference Preview fix is retained verbatim. The branch differs from C2 in 33 files: the 31 transferred paths plus the #353 and #354 report files.
* **The #354 manifest of 31 transferred blobs, re-verified against their origins:** 26 byte-identical, 5 changed, and the 5 are exactly the authorised files. All 8 CTA checker files, all 6 five-rack files and `NextActionGuidance/check_sources.py` are untouched.
* **Production allowed-delta set:** still 9 C2 paths ∪ 11 D-072 Assets paths = 20, with one new allowed change *within* `StandMapSource.cs` and no new path.

## 5. Stage-legacy expectations — EXPECTED STAGE-SCOPE MISMATCH / NOT A PASS

Nothing historical was edited, rebaselined, skipped, stubbed or waived, and no historical result is reported as an integrated PASS.

* **Headland stage mode on the combined tree** now stops at the **stage-1** pin for `StandMapSource.cs` (it previously stopped later, at the stage-2 inventory, because this file used to match). That pin says "this file equals the #297 bytes", which is still true of the accepted history and is now correctly false of the repaired file. Stage-2 and the #297 anchor are not applicable to a combined tree. **Not a pass, not waived.** Its authority remains the original fixture (row 5, unchanged). The integrated mode is a separate, explicitly named check and does not substitute for it.
* **Five-rack checker (#354 result, 14 failures):** **not re-run, not amended**, as instructed. Its results from #354 stand as stage-scope evidence. One consequence to flag for Manager: it byte-pins `run_headland_check.py` and `test_stage_inventory_guard.py` to the #303 commit (`run_five_rack_check.py` lines 59–61), and those two files are now authorised-changed. Those pins were already unreachable on this tree because that stage is decided by commit ancestry, but any future witness that accepts the combined tree against the five-rack checker should list these two as authorised deltas. This prediction is **not verified by a run**.
* **`NextActionGuidance/check_sources.py`:** its two #354 failures (pre-Workbench base diffs) are unaffected and not re-run.

## 6. Not run (by packet instruction)

Unity or the Editor, any lease, `Library`, package import, native input/render, a Player build, B3 replay, the 884-negative/112-benign CTA campaign (its checker bytes are unchanged since #354), the full five-rack run, any A–O or Workbench Tier-2 gate, and any merge to `main` or PR. No new verifier security programme was started. `active-tasks.md` was not touched.

## 7. Residual risks

1. **Offline compile is not Unity compile.** It uses the SDK 8.0.133 C# 9 compiler against the real 6000.6.0f1 DLLs. It excludes Unity's analyzers and generators and the Editor assemblies. The stub `SceneHandle` is a model, and equality semantics on the real struct were established only by the real-DLL compile, not by executing it.
2. Other approved but never-Unity-compiled D-072 code could still hide an issue that this single-source-set compile did not expose. It reported 0 errors across all 189 files, so none is known.
3. Presentation, native rendering, and the live scene-change behaviour (`SceneHandle?` invalidation when a scene reloads) remain unverified at runtime.
4. The five-rack pin consequence in section 5.

## 8. Local state and verdict

Local clean receipt: after the source commit and every run, `git status --short --ignored` was empty; the only later addition is this report. There is no `Library`, `Temp`, `obj` or `Logs`, and no Unity process was observed. Disposable work clones were created only under the Files evidence folder and were removed by the guard.

**READY FOR MANAGER SOURCE/INTEGRITY REVIEW.** Recommended single next step: Manager G3 pin-integrity and G4 source review of this branch (the 5-file diff above and the 33-file difference from C2). Then, only on Manager's ruling, a single integrated Unity compile and qualification under #315. Do not start #315 or #316 automatically.
