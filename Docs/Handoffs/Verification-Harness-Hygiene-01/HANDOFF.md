# Verification Harness Hygiene 01 — Handoff

**Task:** #193 (parent backlog #28) — repair two verification-tool blind spots.
**State:** implementation and offline verification complete; ready for Manager review.
**Success token:** `VERIFICATION HARNESS HYGIENE QUALIFIED — READY FOR MANAGER REVIEW`

## Routing

- Worker: OpenCode Go / `deepseek-v4.1-flash`.
- Branch: `verify/harness-hygiene-v1`.
- Worktree: `/media/jer/Videos/CCF-HARNESS-HYGIENE`.
- Exact start: `5736563ce00fc4826402b608e9e97f62aed3aab2`.
- Implementation HEAD: `b4b68a91` (8 files).
- Handoff HEAD: the branch tip containing this file.
- Unity: not used; no machine-wide lock acquired, no Editor launched.
- Windows: not used.
- Start preflight: refreshed `main`, `Docs/Project/coordination/active-tasks.md` at the start SHA,
  #28 and #193. #193 is the only owner of `Tools/Verification/CloudTypeCheck/` and the bounded
  SiteInfrastructure verification tooling; no other live writer owned either surface. `[RUNNING —
  OFFLINE]` was posted on #193 before the first edit.

## What changed

1. **CloudTypeCheck body binding.**
   - Added `Tools/Verification/CloudTypeCheck/Painter2DStub.cs`: the narrow Unity 6 UI Toolkit
     `Painter2D` / `Angle` / `LineCap` / `LineJoin` / `ArcDirection` surface plus
     `Painter2DCompat.Get(MeshGenerationContext)`.
   - `run_typecheck.py` now copies only files that use `context.painter2D` into a temporary shim
     directory and rewrites that exact member access to `Painter2DCompat.Get(context)`. The project
     stays on C# 9; the source is not modified in the repository. The shim is applied in both
     `prod` and `tools` modes.
   - Added a declaration-level tripwire. Error codes that stop method-body binding (`CS0101`,
     `CS0111`, `CS0234`, `CS0246`, `CS0433`, `CS0518`, `CS0579`) now print `TYPECHECK_BLIND` and
     exit 3. `--update-baseline` refuses to write such an entry.
   - Added `--replace REPO_PATH FILE` so a scratch/mutated harness can be compiled in place of the
     repository file and its diagnostics mapped back to the repository path. This is the mutation
     control path; it does not edit the worktree.
   - Extended `InputSystemStub.cs` and `UnityEditorStub.cs` only with members the existing harnesses
     use (key names, `numpadEnterKey`, `shiftKey`, `deviceId`, `onAfterUpdate`, `InputState`,
     `InputUpdateType`, `MouseState.scroll`, `SerializedObject`/`SerializedProperty`, `SessionState`,
     `UnityStats`, `MonoScript`, `EditorWindow.maximized`/`focusedWindow`, `AssetDatabase.FindAssets`/
     `GUIDToAssetPath`/`GetDependencies(string[], bool)`).
   - Tools mode no longer compiles standalone offline-check programs/stubs that deliberately define
     colliding global helper or production-stand-in types: `RackV2Editing/OfflineCheck.cs`,
     `RackV2Spacing/OfflineCheck.cs`, `RackV2SuggestedLayout/OfflineCheck.cs`,
     `S1CManagementHistory/HistoryOfflineStubs.cs`, `S1CManagementHistory/S1CPerYearHistoryOfflineCheck.cs`,
     `S1CManagementHistory/UnityShim.cs`. Each has its own runner. The two duplicate RackV2 harness
     copies (`RackV2Spacing`, `RackV2ConfirmMinimap`) are compiled in separate supplemental passes,
     so their bodies are still checked. No Painter2D-affected file is excluded; all are compiled via
     the shim.

2. **SiteInfrastructure verification seam allow-list.**
   - Added `VerificationAllowList`, an exact repository-relative path list. It contains exactly
     `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs`, the reviewed consumer
     named by this packet. That harness does not reference the seam today, so the real scan reports
     `allowed=0`; the entry is the pre-authorised path for the adult gate.
   - `H_NO_OTHER_VERIFICATION_TOOL_REFERENCES_THE_NEW_TYPES` is replaced by
     `H_NO_UNLISTED_VERIFICATION_TOOL_REFERENCES_THE_NEW_TYPES`: only exact allow-listed paths are
     accepted; there is no directory, prefix or glob matching.
   - `H_NO_PRODUCTION_FILE_REFERENCES_THE_NEW_TYPES_OR_RESOLVER` is unchanged in meaning and still
     scans every production file strictly. The allow-list is not consulted for production.
   - Added six controls: allow-listed reference accepted; unlisted synthetic reference rejected;
     clean unlisted file accepted; directory/pattern-wide entries rejected; allow-list entries are
     explicit existing `.cs` files; a production-looking path is rejected.
   - The adult harness was **not** edited.

## CloudTypeCheck evidence

Before (`pre/typecheck-tools-before.txt`, start tree):

```text
[tools] distinct errors: 22 (baseline 43)
  4 Painter2D declaration errors (StandMapCanvas, StandMapLegendView, OnboardingIcon, TeachingDiagram)
 18 duplicate-type declaration errors from compiling every tool together
 43 baseline entries reported GONE because method bodies were not bound
exit 1
```

Before `prod` (`pre/typecheck-prod-before.txt`): 4 distinct errors, all `Painter2D`, 7 baseline
entries, exit 1.

After (`after/typecheck-tools-clean-final.txt`, `after/typecheck-prod-clean-final.txt`):

```text
[prod]  distinct errors: 6 (baseline 6)   TYPECHECK_CLEAN mode=prod   exit 0
[tools] distinct errors: 6 (baseline 6)   TYPECHECK_CLEAN mode=tools  exit 0
```

Mutation control (`after/mutation-control-body-errors-final.txt`): a scratch copy of
`BroadleafAdultGrowthVerification.cs` with `int wrong = "not an int";` and
`species.NotARealMember` produced exactly two NEW diagnostics in the harness:

```text
NEW  Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs: error CS0029: Cannot implicitly convert type 'string' to 'int'
NEW  Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs: error CS1061: 'TreeSpeciesDefinition' does not contain a definition for 'NotARealMember' ...
exit 1
```

Declaration tripwire (`after/blind-tripwire-control-final.txt`): a scratch copy with
`private static UnknownType deliberateDeclarationError;` printed `TYPECHECK_BLIND mode=tools` and
exited 3. With `--update-baseline` it refused to write and `baseline-tools.txt` stayed byte-identical
(`after/blind-refuses-baseline.txt`).

The tool is still explicitly an approximation: it does not run Unity and is not a substitute for a
Unity import/compile.

## SiteInfrastructure evidence

- Before (`pre/site-check-before.txt`):
  `SITE_INFRASTRUCTURE_PASS checks=63 siteProfiles=1250 registeredSpecies=3 isolatedCompile=true`.
- After (`after/site-check-final.txt`):
  `SITE_INFRASTRUCTURE_PASS checks=69 siteProfiles=1250 registeredSpecies=3 isolatedCompile=true`.
- 62 of the 63 pre-existing checks are preserved exactly. The one replacement is the old
  all-or-nothing tool guard, now the unlisted-reference guard; six controls were added
  (63 - 1 + 1 + 6 = 69).
- Real-file negative control (`after/site-check-unlisted-real-file-final.txt`): a temporary
  `Tools/Verification/Synthetic/UnlistedConsumer.cs` containing `SpeciesSiteResponses.Beech` failed
  the gate with `unlisted: Tools/Verification/Synthetic/UnlistedConsumer.cs` and exit 1. The
  temporary file/directory was removed; the clean rerun passed.
- The production zero-reference check still passes with 179 production files scanned.
- The isolated compile still succeeds with only the three production data files and the offline
  check.

## Baseline classification

Full line-by-line grouping: `/media/jer/Files/CCF/Verification/VerificationHarnessHygiene01-20261031/BASELINE-CLASSIFICATION.md`.

- `baseline-prod.txt`: 7 -> 6. Removed the 2 `WalkingHudView` `Painter2D` entries because the stub
  now supplies that API and body binding succeeds. Kept 5 existing 2021.3-vs-Unity-6 API-gap
  entries (`FindObjectsByType()` 0-arg in ForestBuildable/ForestPlayer/ForestSawPit,
  `Mouse.WarpCursorPosition`, `Object.GetEntityId`). Added 1 `StandMapSource.cs` CS1503 for the
  Unity 6-only `FindObjectsByType(FindObjectsInactive, FindObjectsSortMode)` overload.
- `baseline-tools.txt`: 43 -> 6. Removed the same 2 `Painter2D` entries and 36 tool-only entries
  that vanish once method bodies are bound: 35 Editor-surface stub gaps (`Editor`,
  `ScriptableSingleton<>`, `EditorWindow.Show`, `SessionState`, `UnityStats`, `MonoScript`) and one
  `AssetDatabase.GetDependencies(string[], bool)` overload. Kept the same 5 production API-gap
  entries. Added the same `StandMapSource.cs` entry. The tools baseline now equals the prod baseline
  because no tool-only approximation errors remain.
- No entry was removed merely to get green. Every removed entry is proved absent from the new run;
  the retained/added entries are all genuine approximation gaps, not production or harness defects.
  A future declaration-level blocker cannot be baselined: it produces `TYPECHECK_BLIND` first.

## Verification commands

```text
python3 -I -B Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod
python3 -I -B Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools
python3 -I -B Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools \
  --replace Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs \
  <scratch-mutated-copy>
python3 -I -B Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools \
  --replace Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs \
  <scratch-declaration-error-copy> --update-baseline
python3 -I -B Tools/Verification/SiteInfrastructure/run_site_check.py
```

Determinism: `prod` and `tools` clean outputs were byte-identical over two runs. Site check output was
identical over two runs except `dotnet build`'s `Time Elapsed` line; all PASS/FAIL lines and the
summary were identical.

## Scope / boundary

Changed files (8, all verification tooling):

```text
Tools/Verification/CloudTypeCheck/Painter2DStub.cs          (new)
Tools/Verification/CloudTypeCheck/run_typecheck.py
Tools/Verification/CloudTypeCheck/InputSystemStub.cs
Tools/Verification/CloudTypeCheck/UnityEditorStub.cs
Tools/Verification/CloudTypeCheck/baseline-prod.txt
Tools/Verification/CloudTypeCheck/baseline-tools.txt
Tools/Verification/SiteInfrastructure/OfflineLogic/SiteInfrastructureOfflineCheck.cs
Tools/Verification/SiteInfrastructure/run_site_check.py
```

Plus this handoff. No `Assets/ForestPrototype/` file, production source, save/schema, scene, prefab,
package, project setting, gameplay value, accepted threshold, calibration or protected anchor
changed. No `.meta` file changed or added. The adult harness was not edited.

## Limitations / follow-ups

- The tool remains an approximation against UnityEngine 2021.3; the six baselined errors are Unity 6
  / newer Input System APIs that the approximation cannot resolve. Unity remains authoritative.
- The Painter2D shim only rewrites `context.painter2D`. A future different receiver name would show
  a body-level CS1061 instead of being silently ignored; extend the shim deliberately if that happens.
- The tools mode excludes offline-check programs with duplicate/stub types and compiles the two
  duplicate RackV2 harnesses in separate passes. Those offline checks still have their own runners;
  the duplicate harness copies are byte-similar iterations.
- If the `UnityEngine.Modules` package is upgraded to a version that includes `Painter2D`, the stub
  will collide and the declaration tripwire will report `TYPECHECK_BLIND`; remove or narrow the stub
  at that point rather than baselining the collision.
- No dashboard publisher exists in this session. `DASHBOARD SYNC: NOT PERFORMED`; published dashboard
  reflects this task: `NO`.
- The branch is local (not pushed) unless the Manager requests publication.

## Return state

- Worktree clean after the handoff commit; no untracked files.
- No Unity lock, no Editor, no Windows action.
- No file outside the authorised verification tooling and this handoff changed.

## Standard handoff block

```text
IMPLEMENTATION HANDOFF
Task: Verification Harness Hygiene 01 (#193; parent #28)
Worker: OpenCode Go / deepseek-v4.1-flash
Role: offline verification-tooling worker (implementation + negative controls)
Branch: verify/harness-hygiene-v1
Base: 5736563ce00fc4826402b608e9e97f62aed3aab2
Context commit: not applicable
HEAD: b4b68a91 (implementation); handoff commit is the branch tip containing this file
GITHUB HANDOFF PATH: Docs/Handoffs/Verification-Harness-Hygiene-01/HANDOFF.md
GITHUB HANDOFF COMMIT: not pushed (local branch)
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/VerificationHarnessHygiene01-20261031/
WHAT CHANGED: CloudTypeCheck method-body binding around the Unity 6 Painter2D gap, declaration-level
  blind tripwire, mutation-control --replace path, narrow stub completion, evidence-backed baseline
  refresh, and a SiteInfrastructure exact-path verification allow-list with positive/negative controls.
FILES CHANGED: 8 verification files + this handoff (list above)
SERIALIZED UNITY FILES TOUCHED: none
PLAYER-FACING RESULT: none
SIMULATION / SAVE IMPACT: none
ASSETS ADDED OR MODIFIED: none
MUST-NOT-CHANGE HASHES / ANCHORS: not touched
INTENTIONALLY CHANGED HASHES / ANCHORS: none
VERIFICATION RUN: prod and tools CloudTypeCheck clean; body mutation caught (CS0029 + CS1061);
  declaration tripwire BLIND and baseline refusal; site check 69 PASS; real unlisted-file negative
  control FAIL as expected; determinism repeated.
VERIFICATION RESULTS: PASS (offline only; no Unity compile claimed)
SPEC CONFORMANCE: packet outcomes 1-6 met
REMAINING VERIFICATION GATES: Manager review; optional independent replay; Unity remains the
  authoritative compile gate but is not part of this task.
KNOWN LIMITATIONS: approximation only; six Unity 6 API gaps remain baselined; shim covers only
  context.painter2D; offline-check and duplicate-harness exceptions are documented above.
UNRELATED WORK PRESERVED: yes
DECISIONS NEEDED: none from the worker; Manager decides whether to push/integrate the branch.
WORKER ASSESSMENT: Ready for review: Yes
RECOMMENDED NEXT STEP: Manager review of b4b68a91 + handoff, then close #193 and #28 if accepted.
DASHBOARD SYNC: NOT PERFORMED
PUBLISHED DASHBOARD REFLECTS THIS TASK: NO
```
