# Sen-1 T-03A: safe bootstrap and staging seam (result)

**Claim [#570](https://github.com/jermur788/CCF/issues/570), orchestrator [#544](https://github.com/jermur788/CCF/issues/544), predecessor [#563](https://github.com/jermur788/CCF/issues/563) / review [#569](https://github.com/jermur788/CCF/issues/569).** Offline source work only. This is not a Unity compile, not a runtime test, not native serialization, not integration, and it ticks none of the 63 ISO items.

**Outcome in one line:** I implemented the fail-closed parts that fit inside the allowlist, and stopped at the seam for the rest. Nothing in the out-of-scope list was touched. The parts that need files outside the allowlist, or a true off-scene staging design, are listed in §7 as requests.

| Record | Value |
| --- | --- |
| Writer | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness inside T3 Code, invoked as the implementation sub-agent under #544. Sole writer, no delegates. **Effort: set by the dispatching harness; not observable from inside the session.** |
| Why this writer (fallback) | OpenAI pool A was quota-refused until 2026-11-18T13:43. OpenAI pool B refused this task with a usage limit. With both OpenAI pools unavailable, this Anthropic writer is the accepted fallback. |
| Independence note | The T-02R writer (Opus 5.5) and its delta reviewer (Sonnet 5.5) were also Anthropic. **The T-03A review should be non-Anthropic** when an OpenAI pool returns. A cross-provider re-check of T-02R is also still owed. |
| Branch / worktree | `feature/scen1-shared-three-profile-save-v2` at `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115` |
| Starting HEAD | `460fd51cb8972dd5b7819632c94e64d1070a98b7`. Verified clean, and in sync with origin, before the [RUNNING] receipt. |
| Source commit | `b730459abc5bf14b3772f9a1cc87fbda69950867`. That commit contains the five C# files only. This Markdown is in the next commit. |
| Authority | [Binding packet at `976a01f3`](https://github.com/jermur788/CCF/blob/976a01f327e685df7fc1bac243cc2400b6aaad2d/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/Sen-1-T03A-OFFLINE-SAFE-BOOTSTRAP-AND-STAGING-PACKET-20261116.md). Signed contract `200add4e`: read with `git show`, not added and not edited. |
| Pre-edit receipt | [#570 RUNNING comment](https://github.com/jermur788/CCF/issues/570#issuecomment-6904092936), with the narrower allowlist |
| Durable evidence | `/media/jer/Files/CCF/Verification/SharedSaveT03A-570-20261116/` |

## 1. Narrower allowlist actually used

There are 5 C# files plus this Markdown. That is within the packet's six reserved paths. There is no new file, no `.meta`, and no protected file.

| File | Method(s) touched | Purpose |
| --- | --- | --- |
| `Assets/ForestPrototype/ForestSaveSessionContext.cs` | new `TryAuthorizeWorldConstruction` (static), `TryAuthorizeConstruction`, `TryGetPreferencePrefixes` | Pure construction gate, plus the active-profile preference namespace |
| `Assets/ForestPrototype/ForestSaveProfiles.cs` | new `TryGetPreferencePrefixes`, `OwnsPreferenceKey` | Preference namespace only. Availability is unchanged. |
| `Assets/ForestPrototype/ForestSaveController.cs` | `CapturedSource` ctor, `AdmittedSave` ctor/`TryCreate`/`Apply(…, out refusal)`, `TrySave(…, out refusal)`, `CaptureData`, `TryPrepareCapture`; new field `incompleteWorldReason` | Staged-application boundary and refusal mapping only. The reader, the writer (T-04), the archive code and the `ApplyAdmittedData` body are unchanged. |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` | `Awake` only (+8/−1) | Selection-before-creation ordering only |
| `Assets/ForestPrototype/ForestSaveValidation.Geometry.cs` | `ValidateGeometry` (tail); new `ReservedCompactGeometryModel`, `ValidateGeometryBinding` | Geometry `2` refusal dispatch only |

`ScenarioOneSaveMigration.cs` was **not changed**. Capture has been observational since T-02: understorey setup happens in `InitializeNewScenario` behind `worldSetupComplete`, not in `CaptureSaveData`. The harness's existing pure-capture checks still pass. No migration split was needed.

`git diff --numstat 460fd51c` for the source commit: controller +50/−8, profiles +23/−0, session context +27/−0, geometry +26/−3, manager +7/−1. `git diff --check` is clean.

## 2. Per-requirement outcome

| # | Requirement | Outcome |
| --- | --- | --- |
| 1 | Selection resolved and validated before any load/generation/scene/forest creation | **Implemented for every caller inside the allowlist. Stopped at the seam for two construction sites outside it.** |
| 2 | Staged-target boundary that cannot corrupt the previous world; a partial apply is refused | **Implemented as fail-closed refusal plus a Save lock.** True off-scene staging, with the old live world retained, is **stopped at the seam** (FR-04 revised-candidate need). |
| 3 | Historical-only Reference preview/return safety | **Partly separable.** The existing return guard is verified, and a failure part-way through entry or return is now contained by R2. Player feedback on a failed return, and keeping the entry snapshot, need manager preview edits outside the allowlist, so they are **stopped at the seam**. |
| 4 | Geometry `2` numeric reservation that refuses without a bound signed manifest | **Implemented in the validator dispatch.** The generator/helper dispatch (`StandGeometryModel.cs`, `ForestStartingStand.cs`) is **stopped at the seam**: those files are not allowlisted. |
| 5 | Preference namespace isolation for the new profile | **Seam implemented** (a resolver and key ownership). Wiring the existing consumers (`LearningObjectivesView*`, `MenuHelpView*`) is **stopped at the seam**: those files are not allowlisted. |

No spec gap was found. Every stop above comes from the allowlist or from the engine lifecycle, not from an ambiguity in the contract.

### R1: selection before construction

- **Selection and seal, unchanged from T-01:** `ForestSaveSessionContext.cs:96-111`. `ResetInitialSelection` runs at `SubsystemRegistration`. `BeginInitialWorldCreation` runs at `BeforeSceneLoad`, before any scene `Awake`, and seals the context.
- **New gate, pure:** `ForestSaveSessionContext.cs:47-68`, `TryAuthorizeConstruction`. It allows construction only when the context is sealed (`WorldCreationStarted`), *and* the selected profile is a registered usable record (`CanUse`), *and* that profile's chapter equals the scenario being built (ordinal comparison). It reads nothing else: no file, no prefs, no scene.
- **Caller:** `ScenarioOne/ScenarioOneManager.cs:462-468`, `Awake`. The gate is consulted before the original new-world block. Every world mutation in `Awake` sits in the gate's `else` branch: the four `ecology.*` model assignments (`:478-481`), `ApplyStandGeometry` (`:486`), `InitializeNewScenario` (`:488`) and `InitializeRackPlanForNewWorld` (`:492`). On refusal it logs an error and builds nothing.
- **Historical effect today: none reachable.** `BeforeSceneLoad` always seals Historical, and `ScenarioOne.asset` has `scenarioId: scenario-one`. Scene reloads (`Restart`) keep the sealed startup context, so the gate passes. Editor `AddComponent` in edit mode does not call `Awake` on this non-`ExecuteAlways` class.

### R2: staged-application boundary

**What I did not do, and why.** I did not implement an in-place rollback. `ApplyAdmittedData` hides doomed trees with `SetActive(false)` and calls `Destroy`, and Unity completes `Destroy` at the end of the frame. A same-frame re-apply of a pre-apply capture would map saved trees back onto objects that are about to be destroyed. A same-frame re-capture would still find them (`FindObjectsInactive.Include`), so it could falsely confirm a restore. That is exactly the "claimed rollback" FR-04 forbids. True staging means building the destination off-scene while the old world stays live, and that needs scene/lifecycle work outside these files. That part is **stopped at the seam** (§7).

**What I did (fail-closed):**

- `ForestSaveController.cs:120-156`, `AdmittedSave.Apply`. All admission checks still run first (`TryPrepare`, `:123`), before the caller's teardown (`:132`) and before the first mutation (`ApplyAdmittedData`, `:137`). The mutating call is now inside `try { } catch (System.Exception) { } finally { }` (`:136-139`). If it fails part-way:
  - it returns `false` (refused); it never rethrows, continues or shows "Game loaded";
  - it logs the exception (`Debug.LogException`, `:150`);
  - it sets `incompleteWorldReason` (`:151`);
  - it shows the refusal on the HUD even on silent paths (`:152-153`).
- `ForestSaveController.cs:208-213`, `TrySave`. While `incompleteWorldReason` is set, every Save is refused: F5, `Save()`, and the menu's `TrySave` with teardown. This happens after the profile check and **before** capture or any temp-file write. The previous save's primary, `.tmp` and `.bak` stay byte-identical.
- **No laundering through captures.** `CapturedSource` records the flag at capture time (`:28-33`, `:310`). An admission built from a capture carries it (`:46`, `:59-60`, `:85`, `:626`). A whole apply sets the flag to the source's own state (`:144`). A disk or archive source is whole (null). A capture of an incomplete world brings the flag back. So a preview of an incomplete world cannot re-enable Save on return.
- **Recovery path:** Load stays available. A whole disk load clears the lock. `Restart` reloads the scene and creates a fresh controller, so the lock is cleared.
- **Refused input** (anything `TryPrepare` rejects) changes nothing, the flag included. That is T-02's pre-mutation refusal, unchanged.

### R3: Reference preview/return

- **Already in place, now verified for a failure part-way through a return:** `EndReferencePreview` (`ScenarioOneManager.cs:735-757`, not edited) drops the guard and the return snapshot only after `RestoreCapturedData` succeeds. On failure it keeps `referencePreviewActive`, `previewReturnData` and `previewYear`, and Tab/Esc retries. Disk Save/Load stay refused during preview (`CanOperateOnSelectedWorld`, controller `:244-248`). A failure part-way through the return is now caught by R2: it returns false, shows the refusal on the HUD, and sets the lock. A retry succeeds and clears it.
- **Entry:** a refused archive admission changes nothing (T-02). A failure part-way through applying the archive now returns false instead of throwing, so the manager shows its existing "preview was not opened" feedback and the Save lock protects disk.
- **Stopped at the seam (manager preview code is outside the allowlist):**
  1. A failed return does not update the manager's own `feedback` text. The HUD carries the refusal instead. The harness records this as a GAP assertion.
  2. On a failure part-way through *entry*, the local `returnData` snapshot is dropped. The player must Load to recover; disk is protected by the lock.

### R4: geometry `2`

- `ForestSaveValidation.Geometry.cs:22-44`. `ReservedCompactGeometryModel = 2` and an exhaustive `switch` in `ValidateGeometryBinding`:
  - 0 and 1 are admitted for Historical only;
  - **2 is refused for every profile**: *"reserved, and no signed world manifest is bound to it"* (*"…cannot be used by a historical save"* in the historical slot);
  - `default` refuses as unknown.
- No manifest type exists, so "bound manifest" is never true and the case refuses unconditionally. A future signed manifest binding must replace this case. **The number names no dimensions, cells or content.**
- `StandGeometryModel.IsKnown`/`Label` are unchanged (0/1 only). There is no one-constant patch.
- `ValidateGeometry` (`:10-20`) keeps the pre-v20 rule byte-for-byte and routes v20+ through the dispatch as Historical. Only the historical slot reaches it: new-profile envelopes are refused earlier by `ValidateSelectedJson`.

### R5: preferences

- `ForestSaveProfiles.cs:113-135`. `TryGetPreferencePrefixes` returns the two SI-09 prefixes only for a registered **usable** profile, so the reserved Compact/O2 namespaces cannot be read, written or reset while unavailable. `OwnsPreferenceKey` classifies keys for enumerated resets: never another profile's key, never `CCF.UI.ReduceMotion`.
- `ForestSaveSessionContext.cs:70-72` resolves the active context's namespace.
- Historical prefixes are exactly `CCF.Learning.v1.` / `CCF.MenuHelp.v1.`. No existing preference consumer or key changed.

## 3. Caller map and ordering proof

Static, Roslyn syntax only (`ordering/Program.cs` → `runs/ordering.log`, **18 PASS / 0 FAIL**). Engine call order itself is not observable offline.

**Who selects.** `ForestSaveSessionContext.ResetInitialSelection` (SubsystemRegistration) and `BeginInitialWorldCreation` (BeforeSceneLoad). Inventory of every `RuntimeInitializeOnLoadMethod` in runtime Assets:

| Site | Load type |
| --- | --- |
| `ForestSaveSessionContext.cs:96` | SubsystemRegistration |
| `ForestSaveSessionContext.cs:103` | BeforeSceneLoad |
| `ScenarioProtectionVisuals.cs:26` | AfterSceneLoad |
| `UI/StandaloneSessionMenu.cs:28` | AfterSceneLoad |

**Nothing but the selection seam runs before scene `Awake`.**

**Who constructs** (every `ApplyStandGeometry` / `Generate` / `InitializeNewScenario` / `InitializeRackPlanForNewWorld` in runtime Assets):

| Site | Method | Gate |
| --- | --- | --- |
| `ScenarioOne/ScenarioOneManager.cs:486`, `:488`, `:492` | `Awake` | **GATED** by `TryAuthorizeWorldConstruction` (`:466`) |
| `ForestSaveController.cs:656` | `ApplyAdmittedData` | Reached only through `AdmittedSave.Apply`, after `TryPrepare` → `MatchesCurrentContext` (`CanUse`, same context, same path, chapter match) |
| `ScenarioOne/ScenarioOneManager.cs:1501` | `RestoreAdmittedSaveData` (v1–v9) | Reached only with an active matching admission (`TryGetManagementData`) |
| `ForestStartingStand.cs:68`, `:70` | `Awake` (DefaultExecutionOrder 100) | **NOT GATED, outside the allowlist** |
| `ForestEcologyController.cs:189-196` | `Awake` → `RebuildGrid`, boundary component | **NOT GATED, outside the allowlist** (no geometry call, but it builds the grid) |

**First point world state could change:**

- In `ScenarioOneManager.Awake`: `ScenarioOneManager.cs:478` (`ecology.RngModelVersion = …`), strictly after and under the gate (`:466`).
- In a load: `ForestSaveController.cs:652-657` (`StandApproachExpected` / `ApplyStandGeometry`), strictly after `TryPrepare` (`:123`) and the teardown callback (`:132`).
- In a save: `WriteSaveAtomically`, after the profile check (`:202`), the incomplete-world refusal (`:208`) and capture.
- For a disk read: `TryReadSave` (`:476`), the only save-file read, after `CanOperateOnSelectedWorld` and `File.Exists`.

The selected-profile refusal comes before every read and every write.

## 4. Negatives (all in the offline harness, `T03ATests.cs`)

- **Refused selection → nothing constructed:**
  - Compact, O2, null, empty, case/space variants and chapter-only IDs are refused.
  - The selection stays empty; the context cannot be sealed; construction is refused.
  - A selected-but-unsealed context, a forced unavailable Compact (even force-sealed), a forged lookalike Historical record, and a chapter mismatch are all refused.
  - The actual bootstrap callbacks were invoked by reflection: refuse after reset, allow after seal.
- **Unavailable profile → refused before any read:** a forced Compact selection and an unselected context. `CanLoad`/`TryLoad` return exactly the profile reason, identical whether the slot file is absent, a valid envelope, a valid historical save or garbage. `TrySave` is refused before capture. Disk SHA-256 manifest and world sentinel are unchanged.
- **Failed staged apply → previous saved world byte-identical:**
  - Injected failure after a real partial mutation: returns false, the reason is a refusal (never "Game loaded"), and the HUD shows it.
  - The SHA-256 manifest of every sandbox slot file (primary/.tmp/.bak of all nine paths plus sentinels) is identical before and after the failed apply.
  - It is identical again after F5 `Save()`, `TrySave`, and the menu-style `TrySave` with teardown (teardown not run), all of which are refused.
  - A refused foreign load keeps the lock. Restoring a capture of the incomplete world re-locks (no laundering). A whole disk load or whole capture unlocks. The silent `showMessage=false` path still locks and shows the refusal.
- **Unbound geometry `2` → refused, no generation:**
  - Dispatch: 2 is refused for Historical, Compact, O2 and null. Unknown values (3, 4, −1, `int.MaxValue`, `int.MinValue`) are refused for every profile.
  - New profiles refuse 0/1. The refusal text contains no 30/36/cell.
  - `IsKnown(2)` is false. Historical admit/refuse parity with the baseline validator holds for v1/19/20/23/24/25 × models −3..5. Reasons are identical except model 2.
  - A plain historical file claiming geometry 2: `CanLoad` and `Load` are refused, with no apply, no world-sentinel change and no file change.
- **Reference return failing part-way:** keeps guard, snapshot and year; the failure is visible on the HUD; Save stays refused; a retry returns and clears. The manager-feedback gap is asserted as a GAP.
- **Preferences:**
  - Historical prefixes are exactly unchanged. Unavailable, forged and null profiles get none.
  - All six prefixes are pairwise non-prefix.
  - Ownership matrix over all sentinel keys; ReduceMotion is never owned.
  - The enumerated reset model removes exactly the historical two.

**Existing negatives:** the T-02R harness's 460 assertions are unchanged and all pass on this source. Two harness edits were made, neither of which weakens a test:

1. A `Debug.LogException` stub, needed to compile the new call.
2. The stubbed apply adapter can now throw once, after a partial mutation, when a T-03A test asks it to.

No assertion was deleted or loosened.

## 5. Harness counts, mutants, typecheck

| Check | Result |
| --- | --- |
| Existing T-02R harness on T-03A source (unchanged tests) | **460 PASS / 0 FAIL** (`runs/` → `work-existing/run.log`) |
| T-02R harness + T-03A tests | **748 PASS / 0 FAIL** (460 + 288 new). Final foreground run on the committed worktree: `runs/harness-final.log` |
| Static ordering/caller proof | **18 PASS / 0 FAIL** |
| Control on the copied source used for mutants | 748/0 and 18/0 |
| Mutants (16 defined, each a single behavioural change) | **PARTIAL: 3 of 16 attempted by the author, 3 of 3 killed.** T1 `partial_apply_propagates` was killed by the harness (unhandled exception) and the ordering proof. T2 `no_save_lock` was killed by the harness (F5 wrote a file) and the ordering proof. T3 `capture_launders_flag` was killed by the harness (the capture restore did not re-lock). T4–T16 were **not run by the author**: the run was stopped so the source could be committed. **The independent reviewer must run them:** `python3 mutants_t03a.py T4_success_always_clears …` in the evidence directory (list in `mutants_t03a.py`). |
| Roslyn typecheck, real Unity 6000.6 reference DLLs, player defines | **NOT RUN by the author.** The script is ready at `unity_typecheck.py <src> <out> player`; the reviewer should run it. |
| Same, `UNITY_EDITOR` defines | **NOT RUN by the author** (`unity_typecheck.py <src> <out> editor`). |

When run, the typecheck compiles every non-Editor `Assets/**/*.cs` file against `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine*.dll` (plus `UnityEditor*` for the Editor run), the CCF-U3 package ScriptAssemblies and Newtonsoft. It was read-only. **It is not a Unity compile:** no asmdef layout, no ILPP, no domain reload, no import.

## 6. Historical behaviour disclosures

1. **A failure part-way through a load.** Before: the exception propagated, the world was left half-applied, and the next F5 would overwrite the good file with it. Now: the load is refused with a HUD reason, and Save is refused until a whole load or a new forest. This is new player-visible text on an exception path only. Success paths are byte-for-byte the same code.
2. **Refusal text for a historical file claiming geometry 2.** It was refused before as *"unknown stand geometry model 2"*. It is still refused, now as *"…reserved for a separately signed world and cannot be used by a historical save"*. Admit/refuse is unchanged for every version and model.
3. **`Awake` gate:** not reachable for historical play today (see R1).

There was no change to historical serialization, validators (all 25 in `ForestSaveValidation.cs` are byte-identical, per the generator check), Reference archive bytes, preview admission, preferences or save paths.

## 7. Stopped at the seam: requests for a T-03B scope (not taken)

1. **`ForestStartingStand.cs` `Awake` and `ForestEcologyController.cs` `Awake`.** Consult `TryAuthorizeWorldConstruction` before `ApplyStandGeometry`/`Generate`/`RebuildGrid`, so that a refused selection builds no stand at all. Today only the manager is gated.
2. **True staged apply (FR-04).** Build the destination world off-scene (for example an additive staging scene or an inactive root) while the old world stays live, and publish only on success. This needs scene/lifecycle design and probably a new file. The fail-closed lock here is the interim seam, not that mechanism.
3. **Manager preview code.** Set player-facing `feedback` on a failed return, and keep the return snapshot when entry fails part-way.
4. **Geometry consumers.** `StandGeometryModel.StandSizeMeters/CellsPerAxis` (40 m fallback for any non-1 value) and `ForestStartingStand.Generate` (Legacy fallback) need the same exhaustive dispatch, with a refusing case 2.
5. **Preference consumers.** `LearningObjectivesView.Curriculum.cs`, `LearningObjectivesView.cs`, `MenuHelpView.cs`: take the prefix from `ForestSaveSessionContext.Current.TryGetPreferencePrefixes`, and reset via `OwnsPreferenceKey`.

None of these need a product choice. Items 1, 3, 4 and 5 are mechanical; item 2 needs a design.

## 8. Not verified

- **Mutation coverage is PARTIAL** (3 of 16 run and killed). T4–T16 were not run by the author.
- **No Roslyn typecheck against the Unity 6000.6 reference assemblies was run** for this candidate, under either define set. The new calls (`Debug.LogException`, `StringComparison` in the session context, an optional parameter after `out` parameters) are therefore checked only by the net8.0 harness build with stubs. That is not a Unity API check, and it is not a Unity compile.

- No Unity Editor, Player, import, compile, domain reload, or engine callback order (`SubsystemRegistration` → `BeforeSceneLoad` → `Awake`) was observed. Bootstrap callbacks were invoked by reflection.
- The real `ApplyAdmittedData` was not run. The harness replaces it with a stub that mutates a sentinel and can throw. Real exception sites, `Destroy` timing and the actual partial-world state are unobserved.
- I did not check whether `Debug.LogException` plus the HUD message is visible at real HUD scale, or whether the Save-lock message fits the HUD panel.
- Native `JsonUtility` behaviour: the harness uses a Newtonsoft stub. The T-02R `centuryReview` residual is unchanged and still owed to T-05.
- No real save, backup, PlayerPrefs, EditorPrefs or archive copy was written. The fixture manifest reads archived host-save copies read-only and hash-checks them before and after, as in T-02R.
- No geometry-2 world exists, so "no generation" is shown only as "no apply and no world-sentinel change" offline.
- No ISO item is claimed. Compact and O2 remain unavailable (`IsAvailable == false`, asserted).
- Dashboard / Drive / active-tasks: not touched, NOT VERIFIED.
