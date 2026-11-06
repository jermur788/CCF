# D-072 #308 — Five-rack checker F-1 to F-8 repair (handoff)

**Status: HANDED-OFF for a new independent combined #296/#303/#306/#308 source review. Not approved and not self-reviewed. Source-only and offline: no Unity, no build, no save, no merge.**

No production defect was found or changed. This repair is only about what the five-rack checker can prove. The Manager's NEEDS CHANGES on the combined candidate stands until the independent review and the Manager verdict.

| Item | Value |
|---|---|
| Claim | [#308](https://github.com/jermur788/CCF/issues/308), under the [Manager ruling](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/MANAGER-D072-STREAM-A-B-REVIEW-RULING-20261106.md), acting on the [#307 report](https://github.com/jermur788/CCF/blob/docs/d072-five-rack-diagram-targeted-re-review-20261106-v1/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/D072-FIVE-RACK-DIAGRAM-TARGETED-RE-REVIEW.md) at `2aa35179` |
| Author | Claude (Anthropic), model `claude-opus-5-5`, launch effort **high**, Claude Code harness in T3 Code |
| Orchestrator | Claude Sonnet 5.5. It launched this worker and did not write the fix. |
| Earlier attempt | Codex `gpt-6.1-sol` failed at once on a usage limit, with no edits, evidence or issue comments. This session is the sole author. |
| Independence limit | Same provider and model as the #307 reviewer and the #296/#303 author. This account's memory held short notes of those reviews, and I read them. I read the #307 reviewer's scripts and reused their ideas. A cross-provider reviewer is preferable if one is available. |
| Branch / worktree | `feature/scenario-one-next-action-guidance-v1`, `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106` |
| Start HEAD | `d531f2308a103d022aca3128497b5c3a975228d5`, equal to origin |
| Code commit | `0f7fe2695c1ed5b2224c5c4bf459dd0e6846569d` (on top of `73704fa18c9a8e13140405fc66f40dd89a03ef16`) |
| Final HEAD | The commit that adds this document. A document cannot contain its own SHA, so the SHA is in the #308 `[HANDED-OFF]` comment. |
| Evidence | `/media/jer/Files/CCF/Verification/D072-FiveRackCheckerF1F3-20261106/` |

**Results in this document come from code commit `0f7fe269`**, where the tree was clean before and after the runs. All four suites were then rerun on the final HEAD, because the Manager wants one identical candidate. Those results are in the `[HANDED-OFF]` comment and under `final-head/` in the evidence folder. This document cannot report them, because it had to be committed first.

## Pre-edit receipt

Taken 2026-11-06 21:05:43 UTC, before the first edit (`preflight.log`). The `[RUNNING]` comment was posted at 21:06 UTC: [comment](https://github.com/jermur788/CCF/issues/308#issuecomment-6695579775).

- `git status --porcelain=v1 -uall --ignored`: empty. Ignored untracked paths under Assets, Packages and ProjectSettings: 0.
- `git ls-files -v`: 5,774 entries, all `H`; no assume-unchanged or skip-worktree flag.
- HEAD and origin both `d531f230`. Owner `jer:jer`. ZX20 exFAT `rw`, 31 G free. Files ext4 `rw`, 62 G free. No Unity process or lockfile.
- Live `origin/main` (`4790c209`) `active-tasks.md` registers #308 as the conditional sole writer.

The unpublished lane `docs/d072-combined-296-303-306-review-20261106-v1` was not read, used or touched. `git worktree list` shows it still at `531ff3ec`.

## Changed files, exactly

| File | Change |
|---|---|
| `Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py` | Repaired. Every existing check keeps its name and relative order. |
| `Tools/Verification/ScenarioOneFiveRackDiagram/test_stage_inventory_guard.py` | Rewritten around exact identities, 80 cases, 4 benign controls and 29 weakened-checker controls. |
| `Tools/Verification/ScenarioOneFiveRackDiagram/expected_assertions.json` | **New fixture.** |
| `Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/D072-FIVE-RACK-CHECKER-F1-F3-REPAIR-HANDOFF.md` | This document. |

**Why the fixture is a separate file.** The exact-name inventory holds 456 assertion names (182 source, 109 structural, 165 painter), the 131 original names and the mutation table digest. Keeping the names in the runner would let one edit change both a check and its expectation. In a separate file, the runner and the test each hold its SHA-256 and read the same reviewable list. It is an exact-named entry in the allowed inventory.

Nothing else changed: no Assets, Packages or ProjectSettings file, no #303 checker, test or handoff, no snapshot, no save, no earlier handoff, no `active-tasks.md`, no `main`.

## The rule as implemented

**File identity.** Nine files are pinned by commit, git blob and SHA-256. Each pin makes three separate checks: the pinned commit holds those bytes, HEAD holds that blob, and the working tree holds those bytes as a regular file.

| Path | Commit | Git blob | SHA-256 |
|---|---|---|---|
| `TeachingDiagram.cs` | `316429dc` | `65d4aba3…` | `5062dd7d…` |
| `TeachingDiagramPlan.cs` | `316429dc` | `6e73f957…` | `967b21b8…` |
| `LearningObjectivesView.cs` | `316429dc` | `6e5d8465…` | `fb97d388…` |
| `FiveRackDiagramCheck.cs` | `316429dc` | `04f8a9f0…` | `5cb387f5…` |
| `PainterProbe/PainterProbe.cs` | `316429dc` | `14b87ecc…` | `8b1b90b6…` |
| `PainterProbe/RecordingUnityStubs.cs` | `316429dc` | `15d203c4…` | `9241cd65…` |
| `OBJECTIVE-GUIDANCE-TEACHING-MAP-IMPLEMENTATION-HANDOFF.md` | `e7bda1f2` | `5ce93c95…` | `641a69d3…` |
| `D072-FIVE-RACK-DIAGRAM-BOUNDED-REPAIR-HANDOFF.md` | `335ee6b3` | `4321956a…` | `f5c60afb…` |
| `D072-FIVE-RACK-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md` | `d531f230` | `26e683f6…` | `09f6dd7c…` |

**Why `316429dc`.** `git log -1` shows it is the last commit that touched each of the first six paths, it is an ancestor of HEAD, and the blobs at `316429dc`, at `335ee6b3` and at HEAD are identical. The #303 checker uses the same anchor for its stage 2, and the #307 report recommends it. The full values are in the `PINS` table in the runner.

The three #303 path, commit and hash pins, the #297 map protections and the exact Objective 1 line check are unchanged and still run.

**Self-pins.** A file cannot contain its own hash, so each pin line is blanked before hashing. The runner pins itself (`SELF_SHA256`), the test with its pin line blanked, and the inventory. The test pins the runner's full SHA-256 and the inventory, and also checks that the running copy equals the copy at the commit under test.

| File at `0f7fe269` | Full SHA-256 |
|---|---|
| `run_five_rack_check.py` | `03d927cb4c733a61534ed6b6976b2cdd54d72ec82231b203f3fd1b8b5a48ec0e` |
| `test_stage_inventory_guard.py` | `52734749163585d4cc7d963111a189a5e1233d9769751e9f1655efb75c5c7ab2` |
| `expected_assertions.json` | `6515df087111ebda1c1832dcba72bf938648721cad6a7c5222c96b38055c4dbe` |

**Path inventory.** `ALLOWED_PREFIX` is gone. Every changed path must be named. The five-rack folder must hold exactly its six files, counting tracked, untracked and ignored ones. The changed set now uses `--no-renames` and `-z`, and also includes `git diff base HEAD`. That closes two gaps I found while repairing, which #307 did not list: a rename reported only the new name, which could hide a deleted production file (F3n), and a committed edit with the working tree put back was invisible (F3o).

**Assertion identities.** The runner compares, by name and order, what actually ran with the inventory for that mode. It checks exactly 109 structural names, 165 painter names, the 29 mutations, the 131 original obligations and a digest of the mutation table. The test repeats these comparisons independently from the runner's printed output.

| Mode | Source assertions | Structural | Painter | Marker | Exit |
|---|---|---|---|---|---|
| Full | **182** | 109 | 165 | `FIVE_RACK_SUITE_PASS` | 0 |
| `--no-mutations` | 122 | 109 | 165 | `FIVE_RACK_SUITE_INCOMPLETE` | 2 |
| `--scope-only` | 108 | not run | not run | `FIVE_RACK_SUITE_SCOPE_ONLY` | 2 |

**Exact negative tests.** A case passes only if the checker exits 1 with exactly the declared set of failing assertions, and still ran every other check by name. The exceptions to the name check are F1e and F1f, where the checker stops on its internal-error assertion, and F2c, which runs at another commit and uses a subset match. An extra failure fails the case, and so does a missing one. That is how removing the all-or-nothing rule is now caught even though the byte pin also fires.

## Disposition of F-1 to F-8

| Finding | Disposition | Proof |
|---|---|---|
| **F-1** runner does not pin the #296 bytes | **Repaired** | S03, S04, S05, S22, S23, F1a to F1i fail the five-rack checker alone. The reviewer's full-mode false-green tree now fails (below). |
| **F-2** weakened checkers pass the integrity test | **Repaired** | W1 to W5, D4, D5 and 22 more controls are each detected. The floor and token test is replaced by exact names and tallies. |
| **F-3** folder allowance, tallies not enforced | **Repaired** | S17, S18, S19, S21, F3a to F3p, L1, L2. |
| **F-4** ignored paths and index flags | **Repaired within limits** | S08, F4a to F4d (ignored paths), S09, F4e, F4f (index flags). Remaining limits below. |
| **F-5** partial runs print the full PASS | **Repaired** | Clean runs in all three modes, a table of 192 run-fact combinations through `suite_verdict`, A12, A13. The test's own switches print `STAGE_INVENTORY_INTEGRITY_PARTIAL` with exit 2, proven by a real partial run. |
| **F-6** `ValueError` on an edited call site | **Repaired** | S02 reports named failures and a suite marker. Other internal errors become the named failure `the checker ran to the end of its mode without an internal error` (F1e, F1f). |
| **F-7** historical output overwrite | **Carried, no frozen file changed** | The #303 runner's default `--out` is unchanged. Every run here used a fresh explicit `--out` or `--work`. The repaired test refuses a non-empty `--work`. |
| **F-8** future integration | **Recorded** | See "Forward note". |

## Before and after: assertion names

From `code-head-2/name-diff.txt`, against unmodified runs at their own commits (`logs/baseline-d531f230.log`, `logs/baseline-335ee6b3.log`).

| Inventory | Before | After | Removed | Result |
|---|---|---|---|---|
| Source, against `d531f230` | 140 | 182 | **none** | The 140 keep their order. 42 added. |
| Source, against `335ee6b3` (original 131) | 131 | 182 | 1, with named successors | `no package, project-settings or Stand Map checker change…` was already split by #306 into two stricter checks. Both still run. |
| Structural | 109 | 109 | none | Same names, same order |
| Painter | 165 | 165 | none | Same names, same order |
| Caught mutations | 29 | 29 | none | Same names, same order. The table digest `3d3151f5…` was computed from both baselines and matches. |

The 42 added source assertions (`UI/` = `Assets/ForestPrototype/UI/`, `FR/` = the five-rack folder, `H/` = the handoff folder):

- #296 pin: UI/TeachingDiagram.cs at 316429dc has its recorded git blob and SHA-256
- #296 pin: UI/TeachingDiagram.cs at HEAD is the pinned git blob
- #296 pin: UI/TeachingDiagram.cs in the working tree has exactly the pinned bytes
- #296 pin: UI/TeachingDiagramPlan.cs at 316429dc has its recorded git blob and SHA-256
- #296 pin: UI/TeachingDiagramPlan.cs at HEAD is the pinned git blob
- #296 pin: UI/TeachingDiagramPlan.cs in the working tree has exactly the pinned bytes
- #296 pin: UI/LearningObjectivesView.cs at 316429dc has its recorded git blob and SHA-256
- #296 pin: UI/LearningObjectivesView.cs at HEAD is the pinned git blob
- #296 pin: UI/LearningObjectivesView.cs in the working tree has exactly the pinned bytes
- #296 verifier pin: FR/FiveRackDiagramCheck.cs at 316429dc has its recorded git blob and SHA-256
- #296 verifier pin: FR/FiveRackDiagramCheck.cs at HEAD is the pinned git blob
- #296 verifier pin: FR/FiveRackDiagramCheck.cs in the working tree has exactly the pinned bytes
- #296 verifier pin: FR/PainterProbe/PainterProbe.cs at 316429dc has its recorded git blob and SHA-256
- #296 verifier pin: FR/PainterProbe/PainterProbe.cs at HEAD is the pinned git blob
- #296 verifier pin: FR/PainterProbe/PainterProbe.cs in the working tree has exactly the pinned bytes
- #296 verifier pin: FR/PainterProbe/RecordingUnityStubs.cs at 316429dc has its recorded git blob and SHA-256
- #296 verifier pin: FR/PainterProbe/RecordingUnityStubs.cs at HEAD is the pinned git blob
- #296 verifier pin: FR/PainterProbe/RecordingUnityStubs.cs in the working tree has exactly the pinned bytes
- handoff pin: H/OBJECTIVE-GUIDANCE-TEACHING-MAP-IMPLEMENTATION-HANDOFF.md at e7bda1f2 has its recorded git blob and SHA-256
- handoff pin: H/OBJECTIVE-GUIDANCE-TEACHING-MAP-IMPLEMENTATION-HANDOFF.md at HEAD is the pinned git blob
- handoff pin: H/OBJECTIVE-GUIDANCE-TEACHING-MAP-IMPLEMENTATION-HANDOFF.md in the working tree has exactly the pinned bytes
- handoff pin: H/D072-FIVE-RACK-DIAGRAM-BOUNDED-REPAIR-HANDOFF.md at 335ee6b3 has its recorded git blob and SHA-256
- handoff pin: H/D072-FIVE-RACK-DIAGRAM-BOUNDED-REPAIR-HANDOFF.md at HEAD is the pinned git blob
- handoff pin: H/D072-FIVE-RACK-DIAGRAM-BOUNDED-REPAIR-HANDOFF.md in the working tree has exactly the pinned bytes
- handoff pin: H/D072-FIVE-RACK-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md at d531f230 has its recorded git blob and SHA-256
- handoff pin: H/D072-FIVE-RACK-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md at HEAD is the pinned git blob
- handoff pin: H/D072-FIVE-RACK-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md in the working tree has exactly the pinned bytes
- this runner matches its own recorded SHA-256 (pin line blanked)
- the integrity test matches its recorded SHA-256 (its checker pin line blanked)
- the expected assertion inventory matches its recorded SHA-256
- the mutation table holds exactly the 15 plan and 14 painter mutations
- the mutation table keeps every original name, anchor, replacement and expected text
- the five-rack folder holds exactly its six named files (tracked, untracked or ignored)
- the #308 handoff, when present, is a regular UTF-8 text file at its one exact path
- no ignored untracked path under Assets, Packages or ProjectSettings
- the git index holds no assume-unchanged, skip-worktree or other non-normal entry (git ls-files -v)
- the checker ran to the end of its mode without an internal error
- the source assertions match the pinned inventory for this mode by name and order
- exactly the 109 pinned structural assertions ran and passed
- exactly the 165 pinned painter assertions ran and passed
- exactly the 29 original diagram mutations ran and each was caught
- all 131 original source obligations are still asserted, by name or by their named stricter successors

## The four suites at `0f7fe269`

Commands, with `R=/media/jer/Files/CCF/Verification/D072-FiveRackCheckerF1F3-20261106/code-head-2`:

```
python3 -I -B Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py --out $R/five-rack-out
python3 -I -B Tools/Verification/ScenarioOneFiveRackDiagram/test_stage_inventory_guard.py --work $R/five-rack-integrity
python3 -I -B Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py --out $R/headland-out
python3 -I -B Tools/Verification/StandMap/HeadlandOffline/test_stage_inventory_guard.py --work $R/headland-integrity
```

| Suite | Exit | Raw result | Log |
|---|---|---|---|
| Five-rack, full | 0 | `FIVE_RACK_SUITE_PASS source_checks=182 failed=0`; `FIVE_RACK_PASS checks=109`; `FIVE_RACK_PAINTER_PASS checks=165`; 29 applied and 29 caught | `1-five-rack.log` |
| Five-rack integrity | 0 | `MUTATIONS=80 BENIGN=4 CLEAN_RUNS=3 CONTROLS=29 FAIL=0`; `STAGE_INVENTORY_INTEGRITY_PASS` | `2-five-rack-integrity.log` |
| Headland (#303, unmodified) | 0 | 39 PASS SOURCE; `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0`; fingerprint `1e1bd6388f287525d17793c553a6bc2adc88f9e279b50d67c98ba5cb6fdaa6d1` | `3-headland.log` |
| Headland integrity (#303, unmodified) | 0 | `MUTATIONS=20 CLEAN_RUNS=2 FAIL=0` | `4-headland-integrity.log` |

Tree state (`state.log`): clean before and after, status 0 lines, index all `H`, HEAD unchanged.

**The reviewer's full-mode false green is closed.** I rebuilt S03 + S04 + S05b + S18 + S19 + S23 on one tree and ran the full five-rack suite (`6-composite.log`). The #307 report recorded `FIVE_RACK_SUITE_PASS` for that tree at `d531f230` (I did not rerun that). Now it exits 1 with `FIVE_RACK_SUITE_FAIL source_checks=182 failed=6`: the four changed files fail their pins, and `exactly the 109…` and `exactly the 165…` fail at 108 and 162.

**The reviewer's own weakened files, run read-only through the test with `--checker`** (`5-weak-*.log`). Each exits 1 with `STAGE_INVENTORY_INTEGRITY_FAIL`, fails the checker pin and never prints the PASS marker. These are old-runner copies, so most cases fail on names the copy lacks. The cases that show each specific weakness are:

| File | Not caught, which proves the weakness |
|---|---|
| `W1-head-tracking-pin.py` | S12 (committed #303 edit) |
| `W2-pin-hash-removed.py` | F2a (wrong hash) |
| `W3-partial-stage-accepted.py` | S10, S11, S13, F2b (partial stage) |
| `W4-headlandoffline-prefix.py` | 303d, S15 (StandMap injection) |
| `W5-guard-and-mutations-removed.py` | No single case isolates it in this old-runner copy, which fails almost every case on names it lacks. The control W5 below, applied to the repaired runner's text, shows the detection directly. |

## Weakened-checker controls

Each control edits the pinned runner's text, re-seals the copy's self-pin so the bytes alone do not give it away, and requires the predicted detection. All 29 also fail the test's byte pin on the runner, separately from the detection shown here.

**Numbering.** The dispatch and the #307 report use W4 and W5 differently. The report's W4 is the HeadlandOffline prefix and its W5 is the guard and mutation removal. The dispatch's W4 is the all-or-nothing removal and its W5 is the allowance widened to `Tools/`, including the `NextActionGuidance/check_sources.py` case. All four are tested: report numbering as W4 and W5, dispatch numbering as D4 and D5.

**A17 and L5 are the honest edge.** A17 turns every failure into a pass and keeps every name, so behaviour no longer rejects anything and only the runner's byte pin in the test objects. L5 neuters every checker-side comparison and prints `FIVE_RACK_SUITE_PASS`, but at a different source count, so the test's independent comparison refuses it. **A weakened runner run alone can still print a pass; the rejection needs the integrity test or a hash comparison.** See limitations.

| Id | Weakening | Detection (case/mode: what is proved) | Result |
|---|---|---|---|
| W1 | reviewer W1: #303 pin follows HEAD, no hash check | S12/scope-only: a committed #303 edit is no longer caught | detected |
| W2 | reviewer W2: #303 SHA-256 comparisons removed | F2a/scope-only: a wrong recorded hash is no longer caught | detected |
| W3 | reviewer W3: partial #303 stage accepted | S10/scope-only: the runner's own name comparison rejects the checks the copy no longer runs<br>S10/scope-only: the independent comparison sees the two lost #303 checks<br>F2b/scope-only: the same for the committed deletion | detected |
| W4 | reviewer W4: HeadlandOffline directory allowance | 303d/scope-only: an injected StandMap script is no longer caught | detected |
| W5 | reviewer W5: guard deleted, four mutations deleted, two expectations emptied | clean-scope-only/scope-only: the runner itself rejects it<br>clean-scope-only/scope-only: the independent name comparison sees the lost guard | detected |
| D4 | dispatch W4: all-or-nothing equality removed | S10/scope-only: deleting a #303 script no longer fails the all-or-nothing rule<br>S13/scope-only: deleting the #303 handoff no longer fails the all-or-nothing rule | detected |
| D5 | dispatch W5: allowance widened to the whole Tools/ tree | F3g/scope-only: the unrelated NextActionGuidance edit is no longer caught | detected |
| A1 | #296 working-tree byte comparison removed | S03/scope-only: a one-byte diagram change is no longer caught<br>S04/scope-only: the title margin is no longer caught<br>S05/scope-only: the false claim is no longer caught<br>S23/scope-only: the entrance size is no longer caught | detected |
| A2 | #296 pin follows HEAD instead of the fixed commit | F1c/scope-only: a committed diagram edit is no longer caught | detected |
| A3 | #296 HEAD blob check removed | F1c/scope-only: a committed edit no longer fails the HEAD pin | detected |
| A4 | ignored production path detector removed | S08/scope-only: the hidden Assets script is no longer caught<br>F4c/scope-only: the info/exclude script is no longer caught | detected |
| A5 | index flag detector removed | S09/scope-only: the assume-unchanged edit is no longer caught<br>F4e/scope-only: the skip-worktree edit is no longer caught | detected |
| A6 | folder prefix allowance restored | S17/scope-only: the folder no longer fails the allowed-files rule | detected |
| A7 | exact folder check removed as well as the allowance | S17/scope-only: an unrelated binary is no longer caught<br>F3a/scope-only: an unrelated test is no longer caught | detected |
| A8 | rename detection left on | F3n/scope-only: a deleted production file hidden by a rename is no longer caught | detected |
| A9 | committed changes no longer compared | F3o/scope-only: a committed rack edit with the working tree put back is no longer caught | detected |
| A10 | integrity test pin removed | S21/scope-only: a loosened integrity test is no longer caught<br>F3b/scope-only: a replaced integrity test is no longer caught | detected |
| A11 | self-pin removed | F3p/scope-only: an unsealed runner edit is no longer caught | detected |
| A12 | scope-only marker claims the full pass | clean-scope-only/scope-only: the copy prints the full marker and the clean-run comparison rejects it | detected |
| A13 | no-mutations marker claims the full pass | verdict-table: the verdict table rejects a pass without the mutations (16 wrong combinations) | detected |
| A14 | map immutability assertion removed | clean-scope-only/scope-only: the runner itself rejects the lost name<br>clean-scope-only/scope-only: the independent name comparison sees the lost map assertion | detected |
| A15 | diagram assertion removed | clean-scope-only/scope-only: the independent name comparison sees it | detected |
| A16 | source identity comparison neutered and a guard deleted | clean-scope-only/scope-only: the runner stays quiet, the independent comparison does not | detected |
| A17 | recording failures as passes (keeps every name) | 303a/scope-only: behaviour no longer rejects anything; only the runner's byte pin can object | detected |
| L1 | #296 pins off, structural check deleted | S18/no-mutations: the runner's exact 109 structural comparison rejects it | detected |
| L2 | #296 pins off, painter assertion deleted | S19/no-mutations: the runner's exact 165 painter comparison rejects it | detected |
| L3 | #296 pins and the structural comparison off, structural check deleted | S18/no-mutations: the independent comparison sees the lost structural assertion | detected |
| L4 | #296 pins and the painter comparison off, painter assertion deleted | S19/no-mutations: the independent comparison sees the lost painter assertion | detected |
| L5 | reviewer W5 with every checker-side comparison neutered, full mode | clean-full/full: the weakened copy's own PASS has a different source count and loses the guard and 4 caught mutations | detected |

## Negative cases

All 80 ran at `0f7fe269` in `--scope-only` mode, each after a clean reset, and each exits 1 with exactly the listed failing assertions. Abbreviations: `UI/`, `FR/`, `HL/` = `Tools/Verification/StandMap/HeadlandOffline/`, `H/`. Exact strings are in the log.

| Id | Change | Exact failing assertions | Result |
|---|---|---|---|
| 303a | headland checker altered by one space | stage 3: HL/run_headland_check.py is byte-identical to pinned #303 commit 6a8b261f | PASS |
| 303b | stage-inventory test altered by one space | stage 3: HL/test_stage_inventory_guard.py is byte-identical to pinned #303 commit 6a8b261f | PASS |
| 303c | historical handoff altered by one newline | stage 3: H/D072-HEADLAND-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md is byte-identical to pinned #303 commit 69b41605 | PASS |
| 303d | extra .py injected under StandMap | only allowed files changed since 1b68ddcd; stage 3: no Stand Map path outside the two pinned #303 verifier scripts changed | PASS |
| 303e | same-named StandMap script in a new subdirectory | only allowed files changed since 1b68ddcd; stage 3: no Stand Map path outside the two pinned #303 verifier scripts changed | PASS |
| 303f | extra Markdown beside the #303 handoff | only allowed files changed since 1b68ddcd | PASS |
| 1a | unauthorised rack edit | no new Assets file or .meta; only allowed files changed since 1b68ddcd | PASS |
| 1b | unauthorised save edit | no new Assets file or .meta; only allowed files changed since 1b68ddcd | PASS |
| 1c | unauthorised scene edit | no new Assets file or .meta; only allowed files changed since 1b68ddcd; unchanged: Assets/Scenes/ForestTest.unity | PASS |
| 1d | unauthorised Packages edit | no package or project-settings change; only allowed files changed since 1b68ddcd | PASS |
| 1e | unauthorised ProjectSettings edit | no package or project-settings change; only allowed files changed since 1b68ddcd | PASS |
| 2a | unauthorised diagram selection edit | #296 pin: UI/TeachingDiagram.cs in the working tree has exactly the pinned bytes; the painter chooses the five-rack figure only through Plan.ShowsFiveRackFigure(lesson, rackV2World); otherwise Plan.For(lesson) | PASS |
| 2b | unauthorised Objective edit | #296 pin: UI/LearningObjectivesView.cs in the working tree has exactly the pinned bytes; LearningObjectivesView.cs differs from base only at the Objective 1 diagram call, which now passes rackV2Objective | PASS |
| 2c | unauthorised map edit | no new Assets file or .meta; only allowed files changed since 1b68ddcd; unchanged: UI/Map/StandMapSource.cs | PASS |
| S03 | one comment byte in TeachingDiagram.cs | #296 pin: UI/TeachingDiagram.cs in the working tree has exactly the pinned bytes | PASS |
| S04 | title margin 6 to 60 UI units | #296 pin: UI/TeachingDiagram.cs in the working tree has exactly the pinned bytes | PASS |
| S05 | note adds the false claim Routes open. | #296 pin: UI/TeachingDiagramPlan.cs in the working tree has exactly the pinned bytes | PASS |
| S22 | confirmed leaks into the draft key row | #296 pin: UI/TeachingDiagramPlan.cs in the working tree has exactly the pinned bytes | PASS |
| S23 | entrance square 11 to 1 design unit | #296 pin: UI/TeachingDiagramPlan.cs in the working tree has exactly the pinned bytes | PASS |
| F1a | opening claim says planning clears the routes | #296 pin: UI/TeachingDiagramPlan.cs in the working tree has exactly the pinned bytes | PASS |
| F1b | one trailing byte on the plan | #296 pin: UI/TeachingDiagramPlan.cs in the working tree has exactly the pinned bytes | PASS |
| F1c | diagram edit committed | #296 pin: UI/TeachingDiagram.cs at HEAD is the pinned git blob; #296 pin: UI/TeachingDiagram.cs in the working tree has exactly the pinned bytes | PASS |
| F1d | plan edit committed | #296 pin: UI/TeachingDiagramPlan.cs at HEAD is the pinned git blob; #296 pin: UI/TeachingDiagramPlan.cs in the working tree has exactly the pinned bytes | PASS |
| F1e | TeachingDiagram.cs deleted | #296 pin: UI/TeachingDiagram.cs in the working tree has exactly the pinned bytes; the checker ran to the end of its mode without an internal error | PASS |
| F1f | plan reverted to the pre-#296 anchor | #296 pin: UI/TeachingDiagramPlan.cs in the working tree has exactly the pinned bytes; the checker ran to the end of its mode without an internal error | PASS |
| F1g | wrong recorded SHA-256 for the diagram | #296 pin: UI/TeachingDiagram.cs at 316429dc has its recorded git blob and SHA-256; #296 pin: UI/TeachingDiagram.cs in the working tree has exactly the pinned bytes | PASS |
| F1h | wrong recorded git blob for the diagram | #296 pin: UI/TeachingDiagram.cs at 316429dc has its recorded git blob and SHA-256; #296 pin: UI/TeachingDiagram.cs at HEAD is the pinned git blob | PASS |
| F1i | diagram replaced by a symlink to identical bytes | #296 pin: UI/TeachingDiagram.cs in the working tree has exactly the pinned bytes; no new Assets file or .meta; only allowed files changed since 1b68ddcd | PASS |
| S10 | #303 test script deleted (partial stage) | stage 3: HEAD contains the #303 stage, so exactly its two verifier scripts and its handoff changed since 1b68ddcd; stage 3: HL/test_stage_inventory_guard.py is byte-identical to pinned #303 commit 6a8b261f | PASS |
| S11 | headland runner reverted to its #297 bytes | stage 3: HEAD contains the #303 stage, so exactly its two verifier scripts and its handoff changed since 1b68ddcd; stage 3: HL/run_headland_check.py is byte-identical to pinned #303 commit 6a8b261f | PASS |
| S12 | #303 runner one-space edit, committed | stage 3: HL/run_headland_check.py is byte-identical to pinned #303 commit 6a8b261f | PASS |
| S13 | #303 handoff deleted | stage 3: H/D072-HEADLAND-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md is byte-identical to pinned #303 commit 69b41605; stage 3: HEAD contains the #303 stage, so exactly its two verifier scripts and its handoff changed since 1b68ddcd | PASS |
| S14 | original-30 guard deleted from the headland runner | stage 3: HL/run_headland_check.py is byte-identical to pinned #303 commit 6a8b261f | PASS |
| S15 | original-92 C# check deleted | only allowed files changed since 1b68ddcd; stage 3: no Stand Map path outside the two pinned #303 verifier scripts changed | PASS |
| S16 | look-alike StandMap2 and standmap paths | only allowed files changed since 1b68ddcd | PASS |
| S20 | #303 integrity test == 39 loosened to >= 30 | stage 3: HL/test_stage_inventory_guard.py is byte-identical to pinned #303 commit 6a8b261f | PASS |
| F2a | wrong recorded SHA-256 for the headland runner | stage 3: HL/run_headland_check.py is byte-identical to pinned #303 commit 6a8b261f; stage 3: pinned #303 blob HL/run_headland_check.py at 6a8b261f matches its recorded SHA-256 | PASS |
| F2b | #303 test script deleted and committed | stage 3: HEAD contains the #303 stage, so exactly its two verifier scripts and its handoff changed since 1b68ddcd; stage 3: HL/test_stage_inventory_guard.py is byte-identical to pinned #303 commit 6a8b261f | PASS |
| F2c | pre-#303 HEAD carrying one #303 file | stage 3: HEAD predates #303, so no #303 verifier path may be changed | PASS |
| S17 | unrelated binary in the five-rack folder | only allowed files changed since 1b68ddcd; the five-rack folder holds exactly its six named files (tracked, untracked or ignored) | PASS |
| S18 | structural C# check deleted | #296 verifier pin: FR/FiveRackDiagramCheck.cs in the working tree has exactly the pinned bytes | PASS |
| S19 | painter assertion deleted | #296 verifier pin: FR/PainterProbe/PainterProbe.cs in the working tree has exactly the pinned bytes | PASS |
| S21 | integrity test accepts any exit code | the integrity test matches its recorded SHA-256 (its checker pin line blanked) | PASS |
| F3a | new unrelated test file in the folder | only allowed files changed since 1b68ddcd; the five-rack folder holds exactly its six named files (tracked, untracked or ignored) | PASS |
| F3b | author test replaced by an unrelated one | the integrity test matches its recorded SHA-256 (its checker pin line blanked) | PASS |
| F3c | author test deleted | the five-rack folder holds exactly its six named files (tracked, untracked or ignored); the integrity test matches its recorded SHA-256 (its checker pin line blanked) | PASS |
| F3d | the inventory edited | the expected assertion inventory matches its recorded SHA-256; the mutation table keeps every original name, anchor, replacement and expected text; the source assertions match the pinned inventory for this mode by name and order | PASS |
| F3e | inventory deleted | the expected assertion inventory matches its recorded SHA-256; the five-rack folder holds exactly its six named files (tracked, untracked or ignored); the mutation table keeps every original name, anchor, replacement and expected text; the source assertions match the pinned inventory for this mode by name and order | PASS |
| F3f | recording stubs edited | #296 verifier pin: FR/PainterProbe/RecordingUnityStubs.cs in the working tree has exactly the pinned bytes | PASS |
| F3g | unrelated NextActionGuidance check_sources.py edited | only allowed files changed since 1b68ddcd | PASS |
| F3h | ignored binary hidden in the five-rack folder | the five-rack folder holds exactly its six named files (tracked, untracked or ignored) | PASS |
| F3i | #306 handoff altered by one newline | handoff pin: H/D072-FIVE-RACK-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md in the working tree has exactly the pinned bytes | PASS |
| F3j | #296 repair handoff altered | handoff pin: H/D072-FIVE-RACK-DIAGRAM-BOUNDED-REPAIR-HANDOFF.md in the working tree has exactly the pinned bytes | PASS |
| F3k | #296 implementation handoff altered | handoff pin: H/OBJECTIVE-GUIDANCE-TEACHING-MAP-IMPLEMENTATION-HANDOFF.md in the working tree has exactly the pinned bytes; the original #296 implementation handoff is unchanged since the reviewed state e7bda1f2 | PASS |
| F3l | binary at the #308 handoff path | the #308 handoff, when present, is a regular UTF-8 text file at its one exact path | PASS |
| F3m | near-name #308 handoff | only allowed files changed since 1b68ddcd | PASS |
| F3n | production file deleted through a rename into the handoff path | no new Assets file or .meta; only allowed files changed since 1b68ddcd | PASS |
| F3o | rack edit committed, then the working tree put back | no new Assets file or .meta; only allowed files changed since 1b68ddcd | PASS |
| F3p | runner edited without re-sealing its self-pin | this runner matches its own recorded SHA-256 (pin line blanked) | PASS |
| S08 | untracked script behind a nested .gitignore under Assets | no ignored untracked path under Assets, Packages or ProjectSettings | PASS |
| S09 | rack edit hidden with assume-unchanged | the git index holds no assume-unchanged, skip-worktree or other non-normal entry (git ls-files -v) | PASS |
| F4a | ignored file under Packages | no ignored untracked path under Assets, Packages or ProjectSettings | PASS |
| F4b | ignored file under ProjectSettings | no ignored untracked path under Assets, Packages or ProjectSettings | PASS |
| F4c | script hidden through .git/info/exclude | no ignored untracked path under Assets, Packages or ProjectSettings | PASS |
| F4d | file under Assets matching an existing root ignore rule | no ignored untracked path under Assets, Packages or ProjectSettings | PASS |
| F4e | rack edit hidden with skip-worktree | the git index holds no assume-unchanged, skip-worktree or other non-normal entry (git ls-files -v) | PASS |
| F4f | diagram edit hidden with assume-unchanged | #296 pin: UI/TeachingDiagram.cs in the working tree has exactly the pinned bytes; the git index holds no assume-unchanged, skip-worktree or other non-normal entry (git ls-files -v) | PASS |
| S06 | selection seam also fires on the introduction | #296 pin: UI/TeachingDiagramPlan.cs in the working tree has exactly the pinned bytes; selection seam: only lesson 0 with the caller's Rack V2 flag shows the figure | PASS |
| S07 | untracked png and .meta under UI/Resources | no new Assets file or .meta; only allowed files changed since 1b68ddcd | PASS |
| S24 | draft swatch fill made opaque | #296 pin: UI/TeachingDiagram.cs in the working tree has exactly the pinned bytes | PASS |
| S25 | map corridors drawn half width | #296 pin: UI/TeachingDiagramPlan.cs in the working tree has exactly the pinned bytes | PASS |
| S26 | shared USS edited | no new Assets file or .meta; only allowed files changed since 1b68ddcd; shared stylesheet (T2-protected bytes) unchanged: the figure's title/notes reuse existing diagram classes | PASS |
| S27 | no-scrolling audit removed | only allowed files changed since 1b68ddcd | PASS |
| S28 | onboarding fit assertion removed | only allowed files changed since 1b68ddcd | PASS |
| S29 | .meta GUID of TeachingDiagram.cs changed | no new Assets file or .meta; only allowed files changed since 1b68ddcd | PASS |
| S30 | near-name production file committed | no new Assets file or .meta; only allowed files changed since 1b68ddcd | PASS |
| S31 | StandMapTransform.cs edited | no new Assets file or .meta; only allowed files changed since 1b68ddcd | PASS |
| S32 | real caption gains a headland claim | no new Assets file or .meta; only allowed files changed since 1b68ddcd | PASS |
| S01 | second Objectives line edited | #296 pin: UI/LearningObjectivesView.cs in the working tree has exactly the pinned bytes; LearningObjectivesView.cs differs from base only at the Objective 1 diagram call, which now passes rackV2Objective | PASS |
| S02 | call site passes !rackV2Objective | #296 pin: UI/LearningObjectivesView.cs in the working tree has exactly the pinned bytes; LearningObjectivesView.cs differs from base only at the Objective 1 diagram call, which now passes rackV2Objective; the passed flag is the view's existing rackV2Objective, computed before the call and unchanged | PASS |

**Benign controls, all accepted with no FAIL line and the expected non-pass marker:** B01 unmodified candidate; B02 ignored `Library/`, `Temp/`, `obj/`, `Logs/`, `UserSettings/`, `.csproj` and `.sln` outside the production folders; B03 an extra newline in this one allowed handoff; B04 archive redirected to an absent path, which gives `FIVE_RACK_SUITE_INCOMPLETE`.

## Behaviour changes a reviewer should know

- **`--scope-only` and `--no-mutations` now exit 2, not 0.** A script that treated exit 0 as success now sees a non-pass, which is intended.
- **The #306 handoff is now pinned.** #307's benign B03 (a newline there) was allowed and is now rejected (F3i). The new benign B03 uses this handoff instead.
- **#307's positive control on a clean `335ee6b3` no longer passes.** With this runner it now fails closed: exit 1, `FIVE_RACK_SUITE_FAIL source_checks=102 failed=7` (`logs/pre303-positive.log`), because the #306 handoff, the test and the inventory do not exist there. The HEAD-aware negative still holds (F2c).
- **A sparse checkout fails by design**, because it uses skip-worktree. The test therefore uses a full clone.
- **A non-default `--base` can never print the PASS marker.**

## Problems I hit and fixed in my own work

Recorded so the reviewer can look at the same places.

- `update-index` applies only the first of `--no-assume-unchanged --no-skip-worktree`, so flags leaked between cases until the test cleared them in two calls.
- A partial run printed the PASS marker text inside a table line. The strict check caught this in the dry run, and the line now prints only the result.
- My first complete run at `73704fa1` passed all four suites, but the test aborted when a case could not be applied to the reviewer's old-runner copies, hiding later cases. `0f7fe269` records `FAIL SETUP` and carries on. That run is kept under `code-head/`.

## Remaining limitations

- **Self-reference.** A coordinated rewrite of the runner, test and inventory together can update every pin, and no in-repository check can stop that. The defence is external: a reviewer compares the SHA-256 values in the `[HANDED-OFF]` comment and reads the diff.
- **The runner alone cannot detect an edit that also re-seals its self-pin** (A17, L5). Treat `FIVE_RACK_SUITE_PASS` as proof only together with the integrity test or a byte comparison.
- **Filesystem and Git-level manipulation beyond index flags is not closed.** Examples are repository config such as `core.fsmonitor` or `core.checkStat` with reset timestamps, filters through `.git/info/attributes`, `git replace`, object alternates and `GIT_*` environment variables. The nine pinned files, the three #303 files and the checker's own files are read as bytes and compared with fixed values, so they are less exposed; the rest of the production tree depends on `git diff`. `git replace` could still affect what `git show` returns, but it cannot match the recorded hashes without a collision.
- **The ignored-path check is narrow:** Assets, Packages, ProjectSettings and the five-rack folder. Ignored files elsewhere are not reported, by design, so that Unity caches stay legal.
- **A mode-only change** (`chmod`) to a pinned file does not fail, because the bytes and blob stay the same. Such a path must already be in the allowed inventory. Not tested.
- **This handoff is unpinned.** It must be UTF-8 text at its exact path, but its content can change, and deleting it is not detected.
- **Shallow clones fail by design**, because the pinned commits must be present.
- **Full mode with an absent archive was not run under the repaired runner.** B04 ran it in scope-only mode and the verdict table covers the combination.
- **Cases run in `--scope-only` mode.** The substantive layer is exercised by L1 to L4 and the composite tree, and the full mode by clean runs, L5 and the composite tree.

## Still red and not claimed green

T2 `Tools/Verification/NextActionGuidance/check_sources.py` (`T2_SOURCE_TALLY PASS=38 FAIL=1`) and the design match (`DIAGRAM_DESIGN_MATCH_FAIL lines=762`) were not rerun and stay red, as historically recorded. No green is inferred.

## Forward note (F-8)

The five-rack inventory covers every changed path. **Any later commit on this branch outside its list, and any merge of `main`, fails it by design.** A future integrated candidate needs a new, separately authorised pinned stage in this checker, and gets no inherited full PASS from this result. Changing the runner or test also requires updating the pins in dependency order: inventory, test with its pin line blanked, runner self-pin, runner full hash.

## NOT RUN

- Unity 6000.6.0f1 import or compile, Editor, Play Mode, rendering, screenshots, layout, fonts, input, performance.
- Rendered 1280×720 and 1920×1080 card checks, grayscale, reviewer comprehension.
- Real saves, Legacy40 and Reference Future v1 at runtime.
- Player and Windows builds, `main` merge, integration.
- T2 and the design match.
- Independent review and Manager verdict: **pending**.

The painter probe and `.NET` runs are source and stand-in evidence only. Nothing here releases #287.

## Evidence

`/media/jer/Files/CCF/Verification/D072-FiveRackCheckerF1F3-20261106/`:

- `preflight.log`, `running-comment.md`
- `logs/baseline-d531f230.log`, `logs/baseline-335ee6b3.log`, `logs/pre303-positive.log`
- `code-head/` (first complete run at `73704fa1`), `code-head-2/` (results above), `final-head/` (rerun on the final HEAD)
- Author tools, not committed: `scripts/` (`gen_inventory.py`, `seal.py`, `name_diff.py`, `composite.py`, `verify_all.sh`, `gen_handoff.py`)
- Reviewer inputs, read-only: `/media/jer/Files/CCF/Verification/D072-Combined-ReReview-20261106/weak/`, `scripts/`
