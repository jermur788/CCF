# S1-D T-05 — Previous vs Proposed Work Plan — Unity Continuation

**Claim:** #151  
**Label:** LOCAL/CODE REQUIRED  
**Purpose:** qualify the already-offline-approved T-05 candidate in Unity; do not redesign or broaden it.  
**Difficulty:** 3/5 — bounded rendered/runtime qualification of an already-reviewed UI candidate.  
**Best-suited executor:** DeepSeek/local Code verification lane by default; Claude Sonnet 5.5 Medium if a genuine UI/runtime defect requires nontrivial diagnosis.  
**Branch/worktree:** `feature/s1-d-t05-previous-proposed-v1` / `/media/jer/Videos/CCF-S1D-T05`

## Exact lineage

- released start: `cfdc93bf3943125d21570bc5749a9338f52d1cce`
- production candidate: `98ac98b5a33929d30b150dc6e4692d8bdf2b9bc1`
- verification commit: `38904535ceb45da1b9112a7695c7c7295462efec`
- exact pushed handoff/head to qualify: **`4ddf4aa535d4ec5267be46d2635679411d595e0f`**
- current integrated umbrella context after PARITY-08: production merge `7d2049e862df5c9311748167423b981e713d1fa9`, docs successor `8fa83bff22ea1b4943689bee1074eed8eae65d2f`. This does not change the exact #151 qualification target.

Offline/static review is already APPROVED. Focused matrix: 219/219; 7/7 mutations caught; required S1-C/S1-D and additional MFM/save/event regressions green. Do not rerun or reinterpret that as Unity evidence.

## Before Unity

Refresh `Docs/Project/coordination/active-tasks.md`, #91/#106/#151/#156, current integration ref, ZX20 free space, machine-wide Unity lock and Editor processes. Record `[RUNNING — UNITY]` on #151 with actual provider/model, branch/worktree, exact head and lock metadata before launch.

Use U1 / the normal serialized Unity lane according to the existing lock procedure. A PASS marker followed by an Editor hang is not a pass; preserve it and rerun cleanly. Do not force-release another owner's lock or kill another owner's Editor.

## Required Unity evidence

Qualify the exact pushed candidate `4ddf4aa5...` and demonstrate:

1. Unity import/compile succeeds with no new compile errors.
2. The three-cycle fixture renders correctly at **1280×720** and **1920×1080**.
3. Open ordinary Work Plan records are visibly and correctly grouped:
   - Approved = `status == Approved`;
   - Pending new = `status == Pending && createdYear == CurrentEcologicalYear`;
   - Pending carried over = `status == Pending && createdYear < CurrentEcologicalYear`.
4. A future-year record is surfaced as inconsistent and is not mislabelled new/carried.
5. MFM prescription approval creates ordinary **Pending** work and is visibly not Work Plan approval; Work Plan approval remains the commitment gate.
6. Carried/prior work links only to an existing Annual Review history year equal to `createdYear`, labelled as creation/origin rather than approval/cause.
7. Work Plan -> historical Annual Review navigation arrives on the requested exact year and does not get reset to latest on first refresh.
8. Missing history year produces no invented/latest fallback destination.
9. Browsing historical review remains read-only: no acknowledgement/history mutation and no current-world inference.
10. AC-5 presentation includes the required Executed / unplanned-Fell-mark visibility from the accepted T-05 contract where applicable.
11. Collapsed/foldout/layout/readability is acceptable at both target resolutions; no clipped or unusable controls.
12. Host saves/reference resources remain unchanged, staged harness artifacts are removed, worktree/lane are clean, and the Unity lock is released normally.

Capture sufficient screenshots/logs to support the final browser/Manager rendered review. Preserve any failed/hung receipts; never overwrite them with the rerun.

## Scope / stop rules

Default is **verification only**. Do not edit production merely to make a gate pass.

If qualification exposes a genuine defect in the exact reviewed T-05 surface, stop and report the defect before broadening scope. A bounded repair may be separately authorised on the same claim after ownership review.

Still excluded: new persistence/date fields; save/schema/validator/hash/reference changes; duplicate history UI/store; automatic approval/cancellation; MFM-09 teaching; ecology/economy/completion changes; Windows build.

## Deliverable

Update `Docs/Handoffs/S1-D-Implementation-05/HANDOFF.md` with the exact Unity receipts/captures and return:

`S1-D T-05 PREVIOUS/PROPOSED PRESENTATION QUALIFIED — READY FOR MANAGER REVIEW`

No Windows build.
