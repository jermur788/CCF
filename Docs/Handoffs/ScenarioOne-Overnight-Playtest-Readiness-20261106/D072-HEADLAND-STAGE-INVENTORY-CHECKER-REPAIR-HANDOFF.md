# D-072 #303 — Headland stage-inventory checker repair (handoff)

**Status: HANDED-OFF for independent targeted re-review (Manager ruling Stage C). Not self-approved. Source-only and offline. No Unity.**

- **Claim:** [#303](https://github.com/jermur788/CCF/issues/303), under Manager ruling Decision 3 (`MANAGER-D072-DIAGRAM-REVIEW-RULING-AND-SERIAL-REPAIR-20261106.md` on `main`), Stage B. Strictly serial after #296, which handed off and released the lane at `335ee6b3`.
- **Worker:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness in T3 Code, dispatched by the DeepSeek orchestrator. **Observed reasoning-effort setting: 15 (low band).** No higher effort is claimed.
- **Branch / worktree:** `feature/scenario-one-next-action-guidance-v1` at `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106`.
- **Start HEAD:** `335ee6b3fde8b9e514cc914b0e5bd6a6e10d70a3`, the same as origin. **Code HEAD:** `6a8b261fa950efe43f9f20a249fb8c318ded4213`. **End HEAD:** the commit that adds this document; its SHA is in the #303 [HANDED-OFF] comment.
- **Preflight:** `git status --porcelain=v1 -uall` empty; `findmnt` shows exfat with the `rw` option; ZX20 is at 94% (31G free); sole writer.
- **Evidence (durable):** `/media/jer/Files/CCF/Verification/D072-HeadlandStageInventory-20261106/`

## Changed files (exactly)

| File | Change |
|---|---|
| `Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py` | Replaces the single-stage four-file inventory with two-stage exact delta accounting (`check_stage_inventory`). `LearningObjectivesView.cs` leaves the BASE-frozen `protected` list because it is now pinned byte-exactly to anchor + authorised argument. Every other line is unchanged. |
| `Tools/Verification/StandMap/HeadlandOffline/test_stage_inventory_guard.py` | **New** checker-integrity test: 20 forbidden mutations plus 2 clean runs of the real checker, in a disposable sparse clone outside the repository. |
| `Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/D072-HEADLAND-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md` | This document. |

These stayed untouched: the four #297 map sources, all #296 source (`TeachingDiagram*.cs`, `LearningObjectivesView.cs`), the #296 checker, the USS, saves, scenes, prefabs, rack/MFM/Work Plan files and the coordination files.

## The rule as implemented

Anchors: `BASE = 7f5278bf…` (pre-map, unchanged), `ANCHOR_297 = 1b68ddcd…` (#297 handoff) and `ANCHOR_296 = 316429dc…` (accepted #296 bounded repair source). The production roots are `Assets`, `Packages` and `ProjectSettings`. "Changed" means `git diff <anchor> --name-only` on the working tree (it includes staged and committed changes and deletions), together with `git ls-files --others --exclude-standard` (untracked files).

The checks run in this order, and the first that fails names the broken rule:

1. **Stage 1, history:** `git diff BASE ANCHOR_297` over the production roots lists exactly the four map files (`StandMapSource.cs`, `StandMapDrawPlan.cs`, `StandMapModel.cs`, `StandMapLegend.cs`).
2. **Stage 1, content (×4):** each map file exists, and its working-tree bytes equal `ANCHOR_297:<path>`.
3. **Stage 2, content (×2):** `TeachingDiagram.cs` and `TeachingDiagramPlan.cs` exist, and their bytes equal `ANCHOR_296:<path>`.
4. **Stage 2, Objectives (×1):** the checker takes `ANCHOR_297:LearningObjectivesView.cs` and replaces the old call `…lesson.Caption));` with `…lesson.Caption, rackV2Objective));`. The old call must occur exactly once in the anchor and the new call exactly once in the result. The working-tree file must equal the result byte for byte, and so must `ANCHOR_296:LearningObjectivesView.cs`. Any other byte in that file fails, including the `rackV2Objective` computation (line 152) and lines 152–156.
5. **Stage 2, inventory (×1):** the files changed since `ANCHOR_297` are exactly those three files. Any other tracked or untracked production change fails, and so does a missing Stage 2 delta.
6. **Combined (repaired original assertion):** the files changed since `BASE` are exactly the four map files plus the three Stage 2 files.
7. **Original `protected` check (kept):** the T2/test paths `check_sources.py`, `StandMap/OfflineLogic`, `StandMapParity.cs` and `StandApproach`, and the UI files `StandMapView`, `HudMinimapView`, `ScenarioOneUiRoot`, `WorkPlanView` and `ScenarioOneUi.uss`, stay byte-identical to `BASE`.

There is no skip, waiver, expected-failure path, directory exemption or wildcard. Every allowed difference is one named file pinned to an immutable commit's bytes.

## Before and after

| Run | Result |
|---|---|
| Unmodified checker at `335ee6b3` (`before-unmodified.log`) | **FAIL**, exit 1, after 11 PASS SOURCE: `HEADLAND_SOURCE_AND_OFFLINE_FAIL: only four authorised display files changed; world/save/racks/Reference/Mini consumers untouched` |
| Repaired checker at `6a8b261f` (`headland-at-6a8b261f.log`) | **PASS**, exit 0: **39 PASS SOURCE (the original 30 + 9 new stage assertions)**, `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0`, `HEADLAND_SOURCE_AND_OFFLINE_PASS`. `BASELINE_CURRENT_FINGERPRINT_SHA256 1e1bd638…aa6d1` matches the #297 handoff exactly. |

Of the original 30 assertions, 28 are byte-unchanged. The four-file inventory now runs as the combined check (6), under an honest new name, behind the stage checks. The `protected` check keeps its name, minus `LearningObjectivesView.cs`, which is now pinned more tightly by check 4.

## Negative mutations (`integrity-at-6a8b261f.log`; per-case logs in `integrity-at-6a8b261f/logs/`)

The real checker runs in a `git clone --shared` sparse clone at the code HEAD. Before each case the clone is reset; then one mutation is applied. A case passes only if the checker exits 1 with exactly the intended assertion.

```
PASS CLEAN clean-before: exit=0 PASS_SOURCE=39 C#=92/92
PASS CAUGHT 1a unauthorised rack edit                     -> stage 2: since #297 anchor exactly the two #296 diagram files and the Objectives call site changed (tracked or untracked)
PASS CAUGHT 1b unauthorised save edit (ForestSaveData.cs) -> stage 2 inventory
PASS CAUGHT 1c unauthorised scene edit (ForestTest.unity) -> stage 2 inventory
PASS CAUGHT 1d unauthorised Packages edit                 -> stage 2 inventory
PASS CAUGHT 1e unauthorised ProjectSettings edit          -> stage 2 inventory
PASS CAUGHT 1f unauthorised T2 USS edit                   -> stage 2 inventory
PASS CAUGHT 2a original #297 map file missing             -> stage 1: present and byte-identical to accepted #297 content …/StandMapLegend.cs
PASS CAUGHT 2b original #297 map file altered (one space) -> stage 1: … …/StandMapDrawPlan.cs
PASS CAUGHT 2c original #297 map delta reverted to pre-map -> stage 1: … …/StandMapModel.cs
PASS CAUGHT 3a Objectives V2 gate statement changed (line 152) -> stage 2: LearningObjectivesView.cs differs from #297 anchor only by the authorised Objective 1 diagram call-site argument
PASS CAUGHT 3b Objectives unrelated edit elsewhere (one space)  -> stage 2 Objectives
PASS CAUGHT 3c Objectives call site with different argument     -> stage 2 Objectives
PASS CAUGHT 3d Objectives call-site delta reverted              -> stage 2 Objectives
PASS CAUGHT 3e Objectives file missing                          -> stage 2 Objectives
PASS CAUGHT 4a extra untracked file under Assets          -> stage 2 inventory
PASS CAUGHT 4b extra untracked .meta under Assets         -> stage 2 inventory
PASS CAUGHT 4c extra file committed under Assets          -> stage 2 inventory
PASS CAUGHT 5a #296 TeachingDiagram.cs altered by one space        -> stage 2: present and byte-identical to accepted #296 content …/TeachingDiagram.cs
PASS CAUGHT 5b #296 TeachingDiagramPlan.cs reverted to #297 anchor -> stage 2: … …/TeachingDiagramPlan.cs
PASS CAUGHT 5c #296 TeachingDiagram.cs missing                     -> stage 2: … …/TeachingDiagram.cs
PASS CLEAN clean-after: exit=0 PASS_SOURCE=39 C#=92/92
STAGE_INVENTORY_INTEGRITY_TALLY MUTATIONS=20 CLEAN_RUNS=2 FAIL=0
```

Each logged line is the checker's own stderr, for example `exit=1 HEADLAND_SOURCE_AND_OFFLINE_FAIL: stage 2: LearningObjectivesView.cs differs from #297 anchor only by the authorised Objective 1 diagram call-site argument`. The lines are abbreviated above; the full text is in the log.

### Deliberate weakening of the guard is detected (`weakened/`)

The same integrity test ran with `--checker` pointing at modified copies of the checker:

| Checker under test | Integrity result |
|---|---|
| W0: the original unmodified checker from `335ee6b3` | **FAIL**, 22/22: both clean runs fail at 11 PASS SOURCE; all 20 mutations are reported as `CAUGHT BY WRONG ASSERTION` (the old four-file guard) |
| W1: Objectives byte-exactness removed | **FAIL**: 3a, 3b and 3c are `NOT CAUGHT` (the checker exits 0), and 3d is caught by the wrong assertion. With check 4 removed, nothing else in the checker protects those statements. |
| W2: directory exemption for `Assets/ForestPrototype/Racks/` | **FAIL**: 1a and 4c are `NOT CAUGHT` |
| W3: Stage 1 reduced to "file exists" | **FAIL**: 2b and 2c are caught by the wrong assertion |

## Other suites (`other-gates/`)

| # | Gate | Result |
|---|---|---|
| 1 | `run_headland_check.py` at `6a8b261f` | **PASS**: 39 + 92/92 |
| 2 | `test_stage_inventory_guard.py` at `6a8b261f` | **PASS**: 20/20 mutations, 2/2 clean runs |
| 3 | #296 `run_five_rack_check.py` in a clean clone at `335ee6b3` (without the #303 files) | **PASS**: `FIVE_RACK_SUITE_PASS source_checks=131 failed=0` |
| 4 | #296 `run_five_rack_check.py` in the lane at `6a8b261f` | **FAIL, 2/131, newly tripped by the authorised #303 files:** `only allowed files changed since 1b68ddcd :: [run_headland_check.py, test_stage_inventory_guard.py]` and `no package, project-settings or Stand Map checker change (the headland checker belongs to #303)`. The other 129 pass: `FIVE_RACK_PASS checks=109`, `FIVE_RACK_PAINTER_PASS checks=165`, all 29 #296 mutations caught. The sorted PASS/FAIL name list differs from run 3 only in those two lines. |
| 5 | T2 `NextActionGuidance/check_sources.py` | **FAIL, pre-existing, unchanged:** `T2_SOURCE_TALLY PASS=38 FAIL=1`, failing on `production diff restricted to five owned source/style files`. Not greened. |
| 6 | `run_diagram_check.py --design …/ScenarioOneUxPedagogy-20261021T074859Z/astra/stand.json` | **FAIL, pre-existing, unchanged:** `DIAGRAM_DESIGN_MATCH_FAIL lines=762`. Byte-identical to the #296 evidence after the first line. Not greened. |
| 7 | `run_diagram_check.py` (no design) | **PASS**: `DIAGRAM_PLAN_PASS checks=1330` |
| 8 | `git diff --check` | **PASS** |

**About gate 4 (needs a decision; not fixed here):** the #296 checker has the same kind of temporal inventory that #303 just repaired. It reserves `Tools/Verification/StandMap/` for #303 and allowlists only #296's own handoffs. So it fails as soon as #303's authorised files exist, and committing this handoff adds a third out-of-list path, `D072-HEADLAND-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md`. I am forbidden to modify the #296 checker. Under the ruling it belongs to the #296 owner, and recognising #303's files needs a Manager/#296-owner decision. It is not a regression of #296 source: gate 3 proves the suite is green without the #303 delta.

## What still fails closed (by design)

- Any further change to `TeachingDiagram.cs`, `TeachingDiagramPlan.cs` or `LearningObjectivesView.cs`, even an approved one, fails until a new stage with its own anchor is added to this checker. So do any changes to the four map files and any new production file.
- Uncommitted (dirty) production changes in the checkout fail the checker, as the original did.

## Caveats

- The anchors are commit SHAs, so the checker needs full history (a shallow clone without them fails with a git error, exit 1).
- The integrity test clones from the shared store (`git rev-parse --git-common-dir`). It writes the clone and logs only under `--work`, which defaults to the Files evidence tree, deletes the clone afterwards, and refuses a `--work` inside the repository. It checks out a sparse cone (UI, Racks, ScenarioOne, Scenes, Packages, ProjectSettings, Tools/Verification/StandMap). Mutations are applied only inside that cone, and sparse-excluded paths are invisible to the guard in the clone. The real lane check (gate 1) sees the full tree.
- Byte comparisons assume no Git eol/clean filters. The repository has no `.gitattributes` and `core.autocrlf` is unset.
- Evidence of how the checker treats source code only. It proves nothing about Unity behaviour.

## NOT RUN

- Unity 6000.6.0f1 import/compile, Editor, Play Mode, Player/Windows build: **NOT RUN** (forbidden).
- Any rendering, screenshots, layout, fonts, input or performance: **NOT RUN**.
- Save files, scenes, prefabs: not touched, **NOT RUN**.
- `CloudTypeCheck` and `StandMap/OfflineLogic` suites: **NOT RUN** in this pass. Their inputs (production C#) are unchanged since #296 ran them green at `316429dc`.
- Independent review and Manager acceptance: **pending** (Stage C). This handoff is not an approval.
