# D-072 #306 — Five-rack stage-inventory checker repair (handoff)

**Status: HANDED-OFF for independent combined #296/#303/#306 source review. Not self-approved. Source-only and offline. No Unity.**

- **Claim:** [#306](https://github.com/jermur788/CCF/issues/306), under Manager ruling
  [`MANAGER-D072-303-HANDOFF-AND-FIVE-RACK-INVENTORY-RECONCILIATION-20261106.md`](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/MANAGER-D072-303-HANDOFF-AND-FIVE-RACK-INVENTORY-RECONCILIATION-20261106.md),
  Stage B serial repair after #303 handed off and released the lane at `69b41605`.
- **Worker / actual model:** DeepSeek V4.1 Flash (`opencode-go/deepseek-v4.1-flash`) in T3 Code / OpenCode. The orchestrator
  performed this bounded Python/verifier edit directly; no separate author subagent was started. That is recorded as a process
  deviation from the "DeepSeek orchestrates only" expectation, not hidden or backdated: no worker start is invented. Effort was
  the harness default; no explicit LOW/HIGH band is claimed.
- **Branch / worktree:** `feature/scenario-one-next-action-guidance-v1` at
  `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106`.
- **Start HEAD:** `69b41605968f78e35a5361eec0562ab9f49e853f`, equal to origin. **End HEAD:** the commit that adds this document;
  its SHA is in the #306 [HANDED-OFF] comment.
- **Preflight (fresh, actual):** `git status --porcelain=v1 -uall` empty; branch HEAD == origin; ZX20 exfat `rw`, 31G free (94%);
  Files ext4 `rw`, 69G free; owner `jer:jer`; no Unity lock or Editor (Python-only). Live coordination was read from
  `origin/main:Docs/Project/coordination/active-tasks.md`: #303 HANDED-OFF/RELEASED and #306 READY FOR LOCAL PREFLIGHT / NOT RUNNING,
  conditional sole writer. Record: `/media/jer/Files/CCF/Verification/D072-FiveRackStageInventory-20261106/preflight.log`.
- **Evidence (durable):** `/media/jer/Files/CCF/Verification/D072-FiveRackStageInventory-20261106/`

## Changed files (exactly)

| File | Change |
|---|---|
| `Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py` | Replaces the two pre-#303 temporal assumptions with an exact stage-3 inventory pinned by commit and bytes. Adds a test-only `--scope-only` stop after the source/scope guards. Every existing substantive check and all 29 mutations are retained. |
| `Tools/Verification/ScenarioOneFiveRackDiagram/test_stage_inventory_guard.py` | **New** checker-integrity test: 14 forbidden mutations plus 2 clean runs of the real checker in a disposable sparse clone outside the repository. Supports `--checker` and a test-only `--skip-clean` for the weakened-checker controls. |
| `Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/D072-FIVE-RACK-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md` | This document. |

These stayed untouched: the two #303 verifier scripts and their historical handoff, `TeachingDiagram*.cs`,
`LearningObjectivesView.cs`, the map source, the USS, saves, scenes, prefabs, rack/MFM/Work Plan files, `active-tasks.md`,
`main` and the user's checkout. No production C#, Unity, Player or Windows build.

## The rule as implemented

The #303 stage is authorised only as **those exact three paths**, pinned to the commits that introduced them and to their
byte hashes:

| Path | Commit | SHA-256 |
|---|---|---|
| `Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py` | `6a8b261f` | `73e111497927e478c89d90484e63ffba24e3dfb4ee984dce96d8c36d80f2eccd` |
| `Tools/Verification/StandMap/HeadlandOffline/test_stage_inventory_guard.py` | `6a8b261f` | `5ae498171f48d4801678de9a12bb53fcf3e261b5100e30279999d0f046220e8b` |
| `Docs/Handoffs/.../D072-HEADLAND-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md` | `69b41605` | `b146d2fbf6eacae8ed2f34ae4598d03a3605e879e84f7a3123fb611cf70d098c` |

The expected presence is decided by **HEAD**, not by the working tree: if HEAD contains `6a8b261f`, the complete pinned stage
must be changed since `1b68ddcd`; if HEAD predates it, no part of the #303 stage may be changed. This rejects a partial stage,
a reverted stage, a same-named file with different content and an extra path. There is no directory prefix, no wildcard, no
skip, no waiver and no expected-failure path.

The two original failing assertions were replaced by tighter named proof, not removed:

1. `only allowed files changed since 1b68ddcd` now allows exactly the three pinned #303 paths (plus this handoff and the
   five-rack test folder). Any other path still fails.
2. The old combined `no package, project-settings or Stand Map checker change` is split into
   `no package or project-settings change` and `stage 3: no Stand Map path outside the two pinned #303 verifier scripts changed`.

New named checks: both pinned commits present; the #303 stage is all-or-nothing; each pinned historical blob matches its
recorded SHA-256; each working-tree file is byte-identical to its pinned commit; no other StandMap path changed.

## Before and after (same combined candidate, current HEAD)

| Run | Result |
|---|---|
| Original checker, unmodified, at current combined HEAD (`before-original.log`) | **FAIL**, exit 1: `FIVE_RACK_SUITE_FAIL source_checks=131 failed=2`. The two failures are `only allowed files changed since 1b68ddcd` (listing the two #303 scripts and its handoff) and `no package, project-settings or Stand Map checker change (the headland checker belongs to #303)`. The other 129 checks passed, including all 29 mutations. |
| Repaired checker, same candidate (`after-repaired.log`) | **PASS**, exit 0: `FIVE_RACK_SUITE_PASS source_checks=140 failed=0`; `FIVE_RACK_PASS checks=109`; `FIVE_RACK_PAINTER_PASS checks=165`; all 29 deliberate plan/painter mutations caught. |

## Negative mutations (`integrity-final.log`; per-case logs in `integrity-final-*/logs/`)

The real checker runs in a `git clone --shared` sparse clone at HEAD. Before each case the clone is reset; then one mutation
is applied. A case passes only if the checker exits 1 with exactly the intended assertion.

```
PASS CLEAN clean-before: exit=0 PASS_SOURCE=140 full+mutations
PASS CAUGHT 303a headland checker altered by one space
PASS CAUGHT 303b stage-inventory test altered by one space
PASS CAUGHT 303c historical handoff altered by one newline
PASS CAUGHT 303d extra .py injected under StandMap
PASS CAUGHT 303e same-named StandMap script in a new subdirectory
PASS CAUGHT 303f extra Markdown beside the #303 handoff
PASS CAUGHT 1a unauthorised rack edit
PASS CAUGHT 1b unauthorised save edit
PASS CAUGHT 1c unauthorised scene edit
PASS CAUGHT 1d unauthorised Packages edit
PASS CAUGHT 1e unauthorised ProjectSettings edit
PASS CAUGHT 2a unauthorised diagram selection edit
PASS CAUGHT 2b unauthorised Objective edit
PASS CAUGHT 2c unauthorised map edit
PASS CLEAN clean-after: exit=0 PASS_SOURCE=82 scope+substantive
STAGE_INVENTORY_INTEGRITY_TALLY MUTATIONS=14 CLEAN_RUNS=2 FAIL=0
STAGE_INVENTORY_INTEGRITY_PASS
```

The exact assertions are in the log. For example, `303a` is rejected by
`stage 3: Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py is byte-identical to pinned #303 commit 6a8b261f`;
`303d` by `stage 3: no Stand Map path outside the two pinned #303 verifier scripts changed`; and `2a` by
`the painter chooses the five-rack figure only through Plan.ShowsFiveRackFigure(lesson, rackV2World); otherwise Plan.For(lesson)`.

### Deliberate weakening / wildcard allowance is detected (`weakened-controls.log`)

The same integrity test ran with `--checker` pointing at weakened copies of the repaired checker (evidence copies under
`weakened/`, not repository files):

| Checker under test | Integrity result |
|---|---|
| W1: working-tree byte-identity comparison removed | **FAIL**, 3/14: `303a`, `303b`, `303c` are `NOT CAUGHT`. |
| W2: broad `Tools/Verification/StandMap/` prefix added and the StandMap subset check removed | **FAIL**, 2/14: `303d`, `303e` are `NOT CAUGHT`. |
| W3: the whole stage-3 inventory replaced by a no-op plus the broad prefix | **FAIL**, 5/14: `303a`–`303e` are `NOT CAUGHT`. |

This is the false-green control: if the repair were a broad StandMap exemption or a wildcard, the test would not catch the
injected paths. It does catch them with the real checker.

## #303 headland suite rerun at the same HEAD (`headland-at-306.log`)

- `run_headland_check.py`: **39 PASS SOURCE**, `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0`,
  `HEADLAND_SOURCE_AND_OFFLINE_PASS`.
- `BASELINE_CURRENT_FINGERPRINT_SHA256 1e1bd6388f287525d17793c553a6bc2adc88f9e279b50d67c98ba5cb6fdaa6d1`,
  matching the #297 handoff exactly. No rebaseline.
- `test_stage_inventory_guard.py`: **20/20 mutations**, 2/2 clean runs (`headland-integrity.log`).

## Preserved and still red

- The #296 29 deliberate plan/painter mutations, `FIVE_RACK_PASS checks=109` and `FIVE_RACK_PAINTER_PASS checks=165` all pass.
  The original 131-check obligations are retained; the repaired run has 140 source checks because the two inventory
  assumptions were replaced by nine tighter stage-3 checks and the package/StandMap split.
- The real archived 1,344-tree snapshot hash, source-derived nonuniform five rack positions, prior lesson byte-identity and
  fail-closed old-world selection are unchanged.
- T2 `NextActionGuidance/check_sources.py` remains **`T2_SOURCE_TALLY PASS=38 FAIL=1`** and the separate design-match remains
  **`DIAGRAM_DESIGN_MATCH_FAIL lines=762`**. They were not rerun in #306, their bytes were not altered, and they are **not**
  claimed green.
- `active-tasks.md`, `main`, the user's checkout, user saves and the #303 checker are untouched.

## NOT RUN

- Unity 6000.6.0f1 import/compile, Editor, Play Mode, rendering, screenshots, layout, fonts, input, performance, Player and
  Windows builds: **NOT RUN** (forbidden).
- T2 and the design-match: not rerun; historical red preserved.
- Independent combined #296/#303/#306 review and Manager acceptance: **pending**. This handoff is not an approval.

## Caveats

- The pinned anchors are commit SHAs, so the checker needs full history. A shallow clone without `6a8b261f` / `69b41605`
  fails the `both pinned #303 commits are present` check by design.
- Byte comparisons assume no Git eol/clean filters. The repository has no `.gitattributes` and `core.autocrlf` is unset.
- `--scope-only` and `--skip-clean` are test-only switches used by the integrity test and its weakened controls. The default
  full run is unchanged.
- This proves source/checker behaviour only. It proves nothing about Unity rendering or the integrated candidate.
