# Sen-1 #629 — L-1 exhaustive stand-geometry dispatch hardening: source result

**Status: IMPLEMENTED / PARTIALLY VERIFIED — NOT UNITY-VERIFIED.** Source-only. No Compact activation, no historical-behaviour change intended, no Unity launched, no lease taken.

| Field | Value |
|---|---|
| Issue | [#629](https://github.com/jermur788/CCF/issues/629) (from #626 §3/§7) |
| Worker | Claude Sonnet 5.5 (`claude-sonnet-5-5`), local Linux, T3 Code / Claude Code. Reasoning effort not attested. |
| Base | `feature/scen1-shared-three-profile-save-v2` @ `09ade4c4` (the #544 save line, unmerged). Local and origin agreed at that head when I started. |
| Task branch | `sen-1/629-l1-geometry-dispatch-20261116-v1`, created from `09ade4c4` (verified absent locally and on origin first) |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-629-L1-GeometryDispatch`, created clean for this task. The save-line branch's own worktree (`CCF-Scen1-SharedSave-T01-20261115`, clean at `09ade4c4`) was **not touched**. |
| Source commit | `ea3e6eea` — 3 files, +29 / −1, nothing else (no `.meta`, scene, prefab, asset, ProjectSettings, Packages) |
| Evidence (durable, outside the repo) | `/media/jer/Files/CCF/Operations/CCF-629-Evidence-20261116/` |

## 1. Ownership — what was and was not established (read this first)

- The issue made #424 and #544 single-writer approval mandatory. In this session I first stopped at that gate, reporting that neither issue contained approval and that the board had no #629 row.
- The project owner (the account that owns the repository and every manager issue) then replied in the session: **"I approve both"**. I treated that as the #424 and #544 approval and proceeded.
- **Not done / not independently confirmed:** no approval comment from the #424 Manager or the #544 save-line owner exists on GitHub; I did not edit `Docs/Project/coordination/active-tasks.md` (owned by #424) or register a [RUNNING] row; the physical CCF-Ops board was not refreshed. **#424 should ratify and register this task** and decide whether the child branch is merged back into the save line (distinct #544 gate, never automatic).
- I used a **child branch** from `09ade4c4` rather than committing onto the save-line branch, because that branch is checked out in another worktree. Nothing was pushed to the save-line branch.

## 2. What changed (`ea3e6eea`)

| File | Change |
|---|---|
| `StandGeometryModel.cs` | `StandSizeMeters` was `model == Enlarged80 ? 80f : 40f`; it is now an exhaustive `switch` (0→40, 1→80) that **throws `ArgumentOutOfRangeException` for anything else**. `CellsPerAxis`, `CellCount`, `StartingTreeTarget` inherit that. `CellIndex` returns its existing **−1** ("not on this grid") for an unknown model instead of measuring it against a 40 m grid. |
| `StandWorldBoundary.cs` | `Apply(model)`: before `appliedModel = model`, an unknown model logs one error and leaves the scene untouched. Previously any non-0 value was built as a resized property. |
| `ForestStartingStand.cs` | `Generate()`: after the Enlarged80 branch, a model other than Legacy40 logs an error and plants nothing. Previously every non-1 model fell into the Legacy40 lattice. |

Untouched by design: `IsKnown` (still 0/1 only; **2 is not known**), `NewGameModel`, `Latest`, `ForSave`, `Label`, the Editor override, `ValidateGeometryBinding` (already exhaustive on the save line), Compact30=2 (still only a reservation), every Prefs/save/scene path.

## 3. Consumer inventory (re-done from source, not from the #626 grep)

Every `.cs` use of `StandGeometryModel`, `Enlarged80`, `Legacy40`, `StandSizeMeters`, `CellsPerAxis`, `CellCount`, `CellIndex`, `StartingTreeTarget`, `StandGeometryModelVersion`, `standGeometryModel`, and the literals `40f`/`80f` under `Assets/` was classified.

**Real fall-throughs ("otherwise → Legacy40/40 m"): fixed.** `StandSizeMeters` (and its four dependants), `StandWorldBoundary.Apply`, `ForestStartingStand.Generate`.

**Reached only with a known model, so left unchanged:**
- `ForestEcologyController.StandGeometryModelVersion` is set only by `ApplyStandGeometry`, which throws on `!IsKnown`. Every consumer that reads it (`StandMap*`, `RackPlanner`, `ScenarioOneManager.*`, `StandMapTreatmentReview`, `StandMapSource`, objectives calls) can only see 0 or 1.
- `ScenarioOneDefinition.TargetAreaFactor` already checks `IsKnown` and throws. `ScenarioOneObjectives.RefreshAspirationalTargets`/`Evaluate`/`Review` reach it through `Effective*` and are therefore already fail-closed.
- `ForestSaveController.FindLoadProblem` checks `IsKnown` before `CellCount`; `ForestSaveValidation.Validate` runs `ValidateGeometry` (which refuses everything but 0/1) before its `CellCount`; the only `CellIndex` caller (`ForestSaveController.cs:584`) runs after `Validate` passed and already treats `< 0` as refusal.

**Positive-only feature gates, left unchanged on purpose:** `StandApproachPath` (`Apply`/`SaveHasApproach`/`Reconcile`), `ScenarioOneRackLayout.AppliesTo`, `ScenarioOneSuggestedRackLayout.Get`, `ScenarioOneManager.RackPlan` (`RackPlanningV2Active`, `InitializeRackPlanForNewWorld`), `ForestSaveValidation.RackV2` (refuses non-E80), `ScenarioReferenceArchive` (equality with Legacy40). For an unknown value each simply means "this feature does not apply", which does not build a 40 m world. I judged rewriting them to be unnecessary churn ("do not silently rewrite a working system"). **This is a judgement the reviewer and #424 may overrule:** they read as "Enlarged80 or otherwise", but the "otherwise" is feature-off, and a later Compact task must decide each one explicitly.
- `RackPlanner.cs:56` and `ScenarioOneManager.RackEditing.cs:93` have `: 40f` only when `ecology` is null — not a model dispatch.

## 4. Tier-1 evidence (all offline; none of it is a Unity run)

| Check | Result |
|---|---|
| Differential harness (`dotnet`, net10, real `StandGeometryModel.cs` + `StandWorldBoundary.cs` from base `09ade4c4` and from `ea3e6eea`, UnityEngine stubs) | **100 checks, 0 failures** on the new source. Every Legacy40/Enlarged80 observable is identical to base: size, cells per axis, cell count, starting tree target, label, `IsKnown`, `ForSave` for v0/19/20/21/24 and null, `Latest`, `NewGameModel`, a 241×241 `CellIndex` position grid hash for each model, and a boundary scene sequence 0,1,0,1,1,0 (transforms, camera far clip, ground tiling, ridge-shaping calls). |
| Unknown refused | For 2, −1, `int.MaxValue`, `int.MinValue`, 3, 100: size/axis/cells/target throw `ArgumentOutOfRange`; `CellIndex` = −1; `IsKnown` false; `Label` unchanged; boundary scene unchanged and exactly one error per call, including the same unknown value repeated (a rejected value is never remembered). |
| Harness is not vacuous | The **base** source fails the same comparison **50/100** (e.g. `StandSizeMeters(2)` = 40, `CellCount(2)` = 64, `CellIndex(2, origin)` = 36, `Apply(2)` rebuilds a 40 m non-legacy boundary with no error). |
| Mutation negative controls | **4 of 4 killed:** M1 binary fallback re-introduced (24 failures), M2 boundary guard removed (10), M3 `CellIndex` guard removed (6), M4 guard moved after `appliedModel =` (4). The independent reviewer's five mutants X1–X5 were also run; X5 (same as M4) survived the first harness and was why the repeat-unknown scenarios were added. |
| Roslyn type-check against installed `Unity/6000.6.0f1` managed DLLs | **0 errors** for base and new, with player defines and with `UNITY_EDITOR` defines (192 non-Editor files plus Editor files). Sensitivity confirmed: an injected `return "forty";` gave exactly one `CS0029` at the expected line. **It is not a Unity compile** (no asmdef split, ILPP, import or domain reload; package DLLs came from another lane's `Library/ScriptAssemblies` and PackageCache, read only). The repo's `unity_typecheck.py` was not found, so this is my own csproj (`typecheck/Typecheck.csproj`). |
| Grep/structure gate (`grep_gate.py`) | PASS on new: zero remaining `? 80f : 40f` or `== …Enlarged80/Legacy40 ?` ternaries outside `StandGeometryModel.cs`; guard order checked in `Generate` and `Apply`. Fails on base (3 findings) and on a guard-deleted mutant. |

Evidence hashes (sha256, first 12): `source.diff e748c81296c9`, `harness/Program.cs 78751c0b729d`, `harness/Stubs/UnityStubs.cs 434bad11c6bc`, `compare.py f6a8307526de`, `grep_gate.py b06fe22caebd`, `typecheck/Typecheck.csproj a027ed30956c`, `logs/new.txt 86d8aea0d290`, `logs/base.txt 288c4f7deb65`. Typecheck logs are empty (0 errors).

## 5. Independent review

A fresh, non-author subagent session reviewed `ea3e6eea` read-only and re-ran the harness (copied evidence, author logs untouched), all mutants, its own five mutants, the grep gate and the typecheck. **Verdict: APPROVED WITH NOTES, no blocker or major.** It independently traced every call of the throwing functions and confirmed no path can hand them a value other than 0/1 today, and confirmed claims 1, 3, 4, 5 and the scope. Its notes:
1. `ForestSaveValidation.cs:221` is safe only because `ValidateGeometry` runs before it (line 216). The new "every caller already checks" comment is true but fragile; a reorder there would turn a refusal into an exception. **Not changed** (a 4th file, not required); recommended to #424 as a follow-up one-line comment.
2. `CellIndex` has a single caller; the only change is on a path that cannot see an unknown model.
3. `Apply(-1)` on a fresh boundary returns silently because `appliedModel` starts at −1; unreachable.
4. Harness gap (fixed after review): the first harness did not kill the guard-after-assignment mutant. Repeat-unknown scenarios and M4 were added; all four author mutants now die.

**Caveat on independence:** the reviewer is the same model family (Claude) as the author, in a fresh context. It satisfies "non-author source review" mechanically but a **Manager-designated reviewer from a different family is still recommended** before merge.

## 6. Not verified

- **Unity was not launched.** No Editor compile, play-mode, domain reload, `Tools/Verification` gate, or real save/load round trip of v1–v24 saves or Reference Future v1. Old-save and Reference-v1 safety rests on static tracing plus the stubbed differential.
- **`ForestStartingStand.Generate` was never executed** (structure check and typecheck only). `Debug.LogError` behaviour in a real Editor and real `Transform`/`MaterialPropertyBlock` behaviour under `StandWorldBoundary.Apply` were not observed; the harness uses stubs.
- No byte-identical gameplay claim is made. What is shown is: the 0/1 outputs of the touched functions equal the base's under stubs.
- Scene/prefab references to these members were not examined (only `.cs`).
- Dashboard/Drive sync, the CCF-Ops board and `active-tasks.md` were not touched or checked.

## 7. Tier-2 (postponed, needs separate admission)

Nothing here approves Unity. The issue's conditions stand: a new Manager permit, a refreshed physical board (#553), a working lease (#548 repair was red/undeployed), an approved exclusive registered clean lane with its own Library/Prefs/`XDG_CONFIG_HOME`, and a same-head targeted old-world regression (`ScenarioOneCompletionVerification` fingerprint, one E80 new-game generation count, one Legacy40 save load, the pinned Reference continuation anchor).

## 8. Requested next actions

1. **#424:** ratify ownership, register the task, assign a different-family reviewer and issue a verdict.
2. **#544 owner:** decide how `sen-1/629-l1-geometry-dispatch-20261116-v1` (one commit on `09ade4c4`) is folded into the save line; this is not automatic.
3. Decide whether the positive-only gates in §3 should later be made explicit when Compact lands.
