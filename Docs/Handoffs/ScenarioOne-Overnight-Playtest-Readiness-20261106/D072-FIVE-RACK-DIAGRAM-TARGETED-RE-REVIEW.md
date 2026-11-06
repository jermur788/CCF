# D-072 combined #296/#303/#306: targeted independent re-review

**Reviewer verdict: NEEDS CHANGES, limited to the #306 five-rack checker and its integrity test.** This is a source-only reviewer opinion. It is not Manager approval and it is not a Unity, render or playtest result. Manager #200 issues the final verdict.

**Production source: no defect found.** B-1, M-1, M-2 and M-3 pass at source level, and the #303 headland checker passes all five of its requirements. Every tally the handoffs claim reproduced exactly in a fresh clone. No production file needs to change for this verdict.

**Why it is not a pass:**

1. The binding ruling says the #296 runner must prove the #296 diagram is byte-stable. It does not. An edit to `TeachingDiagram.cs` or `TeachingDiagramPlan.cs` that avoids a named guard gets `FIVE_RACK_SUITE_PASS` (F-1). The #303 checker does reject these edits, so the two checkers together are not false-green on them.
2. The #306 integrity test accepted four weakened checkers I gave it, including one with the SHA-256 pins removed and one that accepts a partial #303 stage (F-2).
3. The five-rack test folder remains a directory allowance, and nothing compares the 109, 165 or 29 tallies to their expected values (F-3).

**Decision for the Manager:** whether the combined coverage satisfies ruling item 1. If it does, F-1 becomes a recorded caveat and only the F-2 and F-3 test hardening remains. I recommend the small repair listed under each finding, because all of it is Python under `Tools/Verification/ScenarioOneFiveRackDiagram/`.

## Reviewer, lanes and independence

| Item | Value |
|---|---|
| Provider / model | Anthropic, Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness in T3 Code |
| Effort | Session reasoning-effort setting **100**. This is the setting I can observe, not a self-rating. |
| **Independence limitation** | **Same provider and model as the #296/#303 author** (observed effort 15, low). This is a separate session that wrote none of this code, not a cross-provider review, so shared blind spots are possible. The #306 author is a different provider (DeepSeek V4.1 Flash). |
| Prior contact | This account's memory holds short notes of the earlier #296 review (NEEDS CHANGES at `e7bda1f2`) and of #305. I read them before starting. I also read the published #296 review for its fit method. |
| Parallel activity | While this review ran, another lane was active on the same candidate: worktree `/media/jer/Files/CCF/Operations/CCF-D072-CombinedReview-20261106`, branch `docs/d072-combined-296-303-306-review-20261106-v1`, evidence under `/media/jer/Files/CCF/Verification/D072-CombinedReview-20261106/`. It has no claim comment on #307. I saw its directory and process names only and did not read its logs or results. Its names mention ignored files; my case S08 was already written before I saw them. |
| Report lane | `docs/d072-five-rack-diagram-targeted-re-review-20261106-v1`, start `c6ef3168` (equal to `origin/main`), `/media/jer/Files/CCF/Operations/CCF-D072-FiveRackReReview-20261106`, clean with `-uall` at start |
| Source lane (read-only) | `feature/scenario-one-next-action-guidance-v1` at `d531f2308a103d022aca3128497b5c3a975228d5`, unchanged at the end of the review. On `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106` I ran only `rev-parse` and `status`; it is clean at the same HEAD. |
| Execution clone | `/media/jer/Files/CCF/Verification/D072-Combined-ReReview-20261106/source`, detached at `d531f230`, clean before and after every run |
| Evidence | `/media/jer/Files/CCF/Verification/D072-Combined-ReReview-20261106/` (`logs/`, `scripts/`, `weak/`, `mut/out/`, `runs/`). The execution clone is kept. The extra throwaway mutation and pre-#303 clones were deleted after the run, and their logs are kept. |
| Preflight | `git diff --name-status e7bda1f2..d531f230` is exactly the 13 named paths. The only Assets paths are `LearningObjectivesView.cs`, `TeachingDiagram.cs` and `TeachingDiagramPlan.cs`. No Packages or ProjectSettings path. **No blocking scope finding.** |

## Result per item

| Item | Result | Basis |
|---|---|---|
| **B-1** figure only in Rack V2 worlds | **PASS (source)** | See below |
| **M-1** no-scroll budget, nothing weakened | **PASS (source)**; real fit NOT RUN | See below |
| **M-2** painter evidence and negative mutations | **PASS** | See below |
| **M-3** confirmed/pending wording, non-colour semantics | **PASS** | See below |
| **#303 req 1** four map files exact, 30 + 92 retained | **PASS** | 29 names unchanged, 1 tightened and renamed; 92/92 same names in same order; fingerprint identical |
| **#303 req 2** stage anchors, one Objectives argument | **PASS** | Stage 1 and 2 re-derived with `git diff`; byte test below |
| **#303 req 3** reject other Assets/Packages/ProjectSettings | **PASS**, with F-4 limit | S07, S26, S29, S30, S31, S32 rejected |
| **#303 req 4** mutations and weakening fail | **PASS** | 20/20 rerun; my W6 and W7 also detected |
| **#303 req 5** honest before/after, reds disclosed | **PASS** | Failure at 11 PASS SOURCE re-derived; T2 38/1 and design match 762 still red |
| **#306** third stage: exact three paths, commit + SHA-256, all-or-nothing, HEAD-aware | **PASS** | Rejected committed edit, each removal, wrong SHA, and a pre-#303 HEAD carrying the files |
| **#306** two original assertions replaced by tighter proof | **PASS** | Before and after re-derived on the same candidate |
| **#306** 29 mutations, 109, 165, snapshot, INCOMPLETE semantics retained at this HEAD | **PASS** | Inventory audit below; archive-absent run exits 2 with `FIVE_RACK_SUITE_INCOMPLETE` |
| **#306 ruling item 1:** the #296 runner proves the #296 diagram is byte-stable | **FAIL** | F-1 |
| **#306 ruling item 4:** negative controls reject a diagram edit and a weakened runner | **FAIL** (partial) | F-1, F-2 |
| **#306 issue:** test files enumerated by exact name, not a wildcard | **FAIL** | F-3 |
| **#306 process:** `[RUNNING]`/`[HANDED-OFF]` on the claim, truthful preflight record | **FAIL** | P-1, P-2 |
| **Scope and protection (G)** | **PASS** | Protected-file audit below |
| Unity, rendering, input, saves, performance, playtest, Windows | **NOT RUN** | Listed at the end |

### B-1: PASS at source level

- **Selection:** `TeachingDiagram.cs:41` picks the figure only through `Plan.ShowsFiveRackFigure(lesson, rackV2World)`, and that seam is `lesson == 0 && rackV2World`. Neither diagram file looks up eligibility.
- **Call site:** there is one production `new TeachingDiagram` call (`LearningObjectivesView.cs:161`), and it passes the existing flag from line 152. I took the `1b68ddcd` file bytes, added only `, rackV2Objective` at the one occurrence and compared: they equal HEAD and the working tree. The delta is exactly 17 bytes. The file is also unchanged from `7f5278bf` to `1b68ddcd`.
- **Old constructor:** `: this(lesson, caption, false)` fails closed. P7 restores the opposite and is caught.
- **Eligibility:** `RackPlanningV2Active` requires rack model V2 and Enlarged80. Legacy40, Reference Future v1 and pre-V2 states therefore pass `false`. This is a source reading; those worlds were not run.
- **Original picture:** my own script, not the author's probe logic, compared the two dumps: **78 comparisons, 0 differences** against the `1b68ddcd` painter, across 9 pictures and 3 widths. The base dump sha256 `cbe08cc8…a6a3` and its 191,904 lines match the handoff.
- **No claims:** the flag-false dump has no "headland", "feeder", "five-rack", "stand map", "80 m", "field-marked", "entrance", "confirmed", "pending" or "work plan". Its picture labels are "Permanent access", "Retained rows", "Strip: access rack" and "Disc: standing tree".
- **Probe limit, closed by reading:** the probe's claim check uses a stand-in caption. The real caption is "Racks provide access while the rows between them remain standing.", which makes no such claim. Editing it to add a headland claim is rejected by both checkers (S32).

### M-1: PASS at source level; real fit is a Unity gate

No no-scroll test was weakened. `git diff` over `ScenarioOneRackUx`, `ScenarioOneUxPedagogy`, `RackV2NoCompetingAuthor`, `SequentialObjectives`, `CloudTypeCheck` and `NextActionGuidance` is **empty** against `7f5278bf`, `1b68ddcd` and `e7bda1f2`. `FigureMinTextSize` (20), `FigureTextSize` (20), `MinLabelSize` (17) and `MinLegendSize` (15) are unchanged. Removing the `NEEDS_NO_SCROLLING` audit or the onboarding fit check is rejected by the five-rack inventory (S27, S28).

I re-estimated the fit with the real font instead of a character count. I measured the shipped `LiberationSerif-Regular.ttf`, and the system Liberation Serif Bold for the bold title and key rows (the repository ships only Regular, so Unity's bold may differ). The base card heights come from the earlier review's capture measurements (745 − 197 and 1,118 − 295).

| | 1280×720 | 1920×1080 |
|---|---|---|
| Frame width | 810 UI units | 788 UI units |
| Title, 900.8 units wide | 2 lines | 2 lines |
| Note, 1,321.6 units wide | 2 lines | 2 lines |
| Widest key line against its box | 235.9 of 325.6 units | 235.9 of 316.8 units |
| Added height with margins | about 81 px | about 121 px |
| Estimated card bottom | about 629 px | about 944 px |
| Scroll viewport bottom | about 701 px | about 1,035 px |
| **Estimated slack** | **about 72 px** | **about 91 px** |

These agree with the handoff's estimates (627 and 941). **This is an estimate from captures of an older candidate, not a measurement.** The mandatory Unity gate is `OBJECTIVE_CARD_FITS_THE_SCREEN` and `OBJECTIVE_CARD_NEEDS_NO_SCROLLING`.

**Loosened or changed constants, flagged as requested:**

| Change | Assessment |
|---|---|
| Key rows `<= 36` characters became `<= 2 lines of <= 27` | Tighter per line, looser in total (longest row is now 53 characters). The measured widths fit, and rows stay inside the 55–57 unit row pitch. Acceptable. |
| Spread check `"(15.6–19.4 m)"` became `"(15.6–19.4 m"` | Needed because the note now says "m here)". Harmless. |
| "Notes name A/B/C and where the feeder joins" became a key-row check | Text was cut under M-1 as the ruling allowed. The words "western", "southern" and "between racks 1 and 2" are no longer on the card. The join position is still checked geometrically. |
| New: one note `<= 175`, title + note `<= 300`, `<= 4` lines | New guard. It counts text only: a 60-unit title margin is not seen by the five-rack runner (S04, F-1). |

### M-2: PASS

- **Real compile:** `probe-current/build/check.csproj` compiles the real `Assets/ForestPrototype/UI/TeachingDiagram.cs` and `TeachingDiagramPlan.cs` from the clone with `RecordingUnityStubs.cs`. The authority constants are generated from parsed sources.
- **Transparency is meaningful:** my reading of the dump at width 810 shows planned fills `182,118,232` at alpha **66** (6 of them: 5 racks and the key swatch), the draft fill at alpha **33**, and opaque harvested fill. P2 (the pre-repair opaque fill) is caught by 33 FAIL lines, and my S24 (opaque draft fill) by 3.
- **Boundary and key:** the boundary is one 5-point stroke; P11 dashes it and is caught. Key rows have distinct signatures with colour ignored; P1 and P12 collapse them and are caught.
- **All 29 author mutations caught** with their intended text.
- **Limit:** the stubs have no layout, fonts, USS, tessellation or blending, so this proves the command stream, not pixels. The probe also does not check everything the painter does. It does not check the title margin (S04) or entrance size (S23). The handoff states the first limit plainly.

### M-3: PASS

The key rows read:

- "Existing: A headland, / B feeder, C track, clearing"
- "Draft rack: dashed, / a proposal only"
- **"Planned = confirmed: solid, / work pending, trees stand"**
- "Entrance: square / on the headland"
- "Field-marked: stripes, / not felled, not a Fell mark"
- "Harvested: open strip / with stumps"
- "Disc: standing tree, / retained, not a tree count"

"Confirmed" and "pending" appear only in the planned row; my S22 leaks "confirmed" into the draft row and is caught. The note starts "Only Work Plan approval commits the work." The title keeps "Planning or confirming does not clear the routes." No shipped text says a route is cleared. The shapes differ without colour at both plan and painter level. Grayscale reading is NOT RUN.

**Gap:** the "cleared, cut or open" scan covers key rows 1–3 only, not the note. A note saying "Routes open." passes the five-rack runner (S05). This is part of F-1, and the headland byte pin rejects it.

## Findings

### F-1 (major, requirement unmet, mitigated): the five-rack runner does not prove the #296 diagram is byte-stable

- **Requirement:** ruling item 1, "The #296 runner must prove the exact chronology: accepted #297 map source is byte-stable, #296 diagram and one authorised Objective 1 call-site are byte-stable". Item 4 also requires rejecting an "unauthorised diagram" edit by its expected assertion.
- **Code:** `run_five_rack_check.py:65` lists both diagram files in `ALLOWED` with no pin. The map files and the Objectives line are pinned; these two are not. The author's case 2a edits the one selection line that already has a named guard, and the handoff does not state this limit.
- **Evidence:** the five-rack runner exits 0 with `FIVE_RACK_SUITE_PASS` for:
  - S03, a comment byte;
  - S04, title margin 6 to 60 UI units, about 43 px more card height at 720p;
  - S05, a note saying "Routes open.";
  - S23, entrance square reduced from 11 to 1 design unit.

  These cases ran with `--no-mutations`, so I also ran a **full-mode** check. I applied S03, S04, S18, S19 and S23 together with the S05 claim moved so that no author mutation anchor is disturbed. The full suite still reports `FIVE_RACK_SUITE_PASS source_checks=140 failed=0`, `FIVE_RACK_PASS checks=108`, `FIVE_RACK_PAINTER_PASS checks=162` and all 29 mutations caught (`logs/full-mode-falsegreen.log`). The headline `source_checks=140` does not move, so a reader who checks only that number would not notice. The headland checker rejects the same tree.
- **Mitigation:** the headland checker rejects all four with `stage 2: present and byte-identical to accepted #296 content`. The five-rack stage pins the headland checker's bytes, so that pin cannot be removed without tripping five-rack. **The combined gate is not false-green on these edits, but the five-rack runner does not call the headland runner, so that holds only when both are run.**
- **Repair:** in `stage_inventory`, pin both files to `316429dc` by bytes and SHA-256, the same way as the #303 paths:
  - `TeachingDiagram.cs`: `5062dd7dd1836cdb9663b257c0e4a4b74daf3b6f183923a27e6f86e35be83874`
  - `TeachingDiagramPlan.cs`: `967b21b8bfd1bedafa3fdb23e8bcda1c90bcbf3696882a918b62a33628514771`

  Add a one-byte case for each to the integrity test.

### F-2 (major): the #306 integrity test accepts weakened checkers

The checker at this HEAD is not weakened: it rejects every matching input directly (S10–S13, a wrong SHA constant). This finding is about what the integrity test can detect, which is the ruling item 4 requirement that it "reject … a weakened/wildcard allowance in the repaired #296 runner".

| Weakened copy (not a repository file) | Integrity test result | What that copy then accepts |
|---|---|---|
| W1: pin follows `HEAD:` instead of the commit, no hash check | **`INTEGRITY_PASS`, 14/14** | A committed edit to `run_headland_check.py` gets `FIVE_RACK_SUITE_PASS`. The real checker rejects it. |
| W2: both SHA-256 comparisons removed, commit pin kept | **`INTEGRITY_PASS`, 14/14** | A wrong hash constant goes unnoticed. The real checker rejects it. |
| W3: all-or-nothing removed, only files still in the delta compared | **`INTEGRITY_PASS`, 14/14** | Deleting the #303 test script passes. The real checker rejects it. |
| W5: one original guard deleted, 4 of 29 mutations deleted, 2 mutation expectations emptied | **`INTEGRITY_PASS`, full run**, `clean-before PASS_SOURCE=131`, `clean-after PASS_SOURCE=81` | Silently keeps 25 of 29 mutations |

**Causes:**
- Cases 303a–c are uncommitted edits, so a pin that follows HEAD still sees them.
- There is no removal case, although the #306 issue's negative test (b) says "changed/removed".
- `test_stage_inventory_guard.py:153-155` uses the floor `source >= 131` against a real 140, and only checks for the words `FIVE_RACK_PASS` and `FIVE_RACK_PAINTER_PASS`. The sister #303 test uses `source == 39`, which is why my W7 was caught there.

**The handoff's heading "Deliberate weakening / wildcard allowance is detected" is true only for its own three weakened copies.** My W4 (directory prefix) was detected at 303d, so simple wildcards are covered.

**Repair:** add committed-edit, removal and wrong-hash cases, and replace the floor with exact tallies: 140 (or the new total), `checks=109`, `checks=165`, 29 caught.

### F-3 (medium, no production reach): the five-rack folder is still a directory allowance, and the tallies are not enforced

- **Requirement:** the #306 issue says "enumerate them by exact name rather than a wildcard exemption". `ALLOWED_PREFIX` (`run_five_rack_check.py:67`) still allows anything under `Tools/Verification/ScenarioOneFiveRackDiagram/`. The prefix predates #306, and the first ruling scoped #296 to that folder, so this is an unmet dispatch line, not a new leak.
- **Evidence, no checker rejects any of these:**
  - S17, an unrelated binary in the folder;
  - S18, a structural check deleted (`FIVE_RACK_PASS checks=108`);
  - S19, a painter assertion deleted (`FIVE_RACK_PAINTER_PASS checks=162`);
  - S21, the #306 integrity test loosened to accept any exit 1.

  The lower counts print, but nothing compares them with 109 or 165.
- **Why this is limited:** a checker cannot fully guard itself, and at this HEAD my inventory audit confirms nothing was removed. The headland side is much stronger, because the five-rack stage pins all of its files (S14, S15, S20 rejected).
- **Repair:** list the five files by name and enforce exact tallies as in F-2.

### Minor findings

| ID | Finding | Evidence | Action |
|---|---|---|---|
| F-4 | "No untracked Assets script" uses `--exclude-standard` in both checkers, so ignored files are invisible. S08 hides a script behind a nested `.gitignore`, which Unity would import; both checkers pass. S09 hides a tracked edit with `--assume-unchanged`; this one is deliberately adversarial. | I checked directly: **0 ignored paths** under Assets, Packages and ProjectSettings in the clone and in the author's lane. The candidate is not affected. | Optional: list production files without excludes. |
| F-5 | `--scope-only` and `--no-mutations` print the same `FIVE_RACK_SUITE_PASS`. With the opaque-fill defect restored, `--scope-only` exits 0 with that token at `source_checks=70`. | `logs/head-aware.log` run G | Use a distinct token, as m-5 did for INCOMPLETE. |
| F-6 | An edited call site makes the runner print the right FAIL line, then crash with `ValueError` at `run_five_rack_check.py:265`. It exits 1 with no suite token. It still fails closed. | S02 | Guard the `index` call. |
| F-7 | The default `--out` of `run_headland_check.py` is #297's evidence directory, and it rewrites files in place. This is the same class as carried m-4. I passed `--out`. | Line 129 | Carry; changing it needs a new pin. |
| F-8 | **Forward note.** The five-rack inventory covers every path. Any later commit on this branch outside its list, including the next Docs handoff, and any merge of `main`, will fail it by design. The headland checker covers production paths only. | Author case 303f | #287 and the integrated candidate need a further stage, or must run this check at a pinned HEAD. |

## Commands and raw tallies

All commands ran from the execution clone with `.NET 10.0.114` and Python 3.12.3, with output outside the repository.

```
R=/media/jer/Files/CCF/Verification/D072-Combined-ReReview-20261106; cd $R/source
python3 -I -B Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py --out $R/runs/five-rack-head
python3 -I -B Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py --out $R/runs/headland-head
python3 -I -B Tools/Verification/StandMap/HeadlandOffline/test_stage_inventory_guard.py --work $R/runs/headland-integrity
python3 -I -B Tools/Verification/ScenarioOneFiveRackDiagram/test_stage_inventory_guard.py --work $R/runs/five-rack-integrity
python3 -I -B $R/scripts/review_mutations.py <worker> <count>      # my S and B cases
python3 -I -B $R/scripts/inventory.py                              # static assertion audit
python3 -I -B $R/scripts/runtime_names.py                          # runtime assertion audit
```

| Run | Exit | Raw result | Expected |
|---|---|---|---|
| Five-rack at `d531f230` | 0 | `FIVE_RACK_SUITE_PASS source_checks=140 failed=0`; `FIVE_RACK_PASS checks=109`; `FIVE_RACK_PAINTER_PASS checks=165`; 29 applied, 29 caught; build warnings `[]` | Match |
| Headland at `d531f230` | 0 | 39 PASS SOURCE; `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0`; fingerprint `1e1bd6388f287525…aa6d1` | Match, identical to #297 |
| #303 integrity | 0 | `MUTATIONS=20 CLEAN_RUNS=2 FAIL=0`, each by its exact assertion | Match |
| #306 integrity | 0 | `MUTATIONS=14 CLEAN_RUNS=2 FAIL=0`; clean-before 140, clean-after 82 | Match |
| **Pre-#303:** headland at `335ee6b3` | 1 | 11 PASS SOURCE, then `only four authorised display files changed…` | Re-derived |
| **Pre-#303:** five-rack at `335ee6b3` | 0 | `source_checks=131 failed=0`; 109; 165; 29 caught | Re-derived |
| **Original #297:** headland at `1b68ddcd` | 0 | 30 PASS SOURCE; 92/92; same fingerprint | Baseline for the audit |
| **Before #306:** the `335ee6b3` runner on `d531f230` | 1 | `source_checks=131 failed=2`: exactly `only allowed files changed since 1b68ddcd` and `no package, project-settings or Stand Map checker change…` | Re-derived |
| HEAD's runner on a clean `335ee6b3`, `--scope-only` | 0 | `source_checks=64 failed=0`, using the `HEAD predates #303` branch | HEAD-aware control |
| Same, plus one #303 file, or all three | 1 | `HEAD predates #303, so no #303 verifier path may be changed` | HEAD-aware negative |
| `--base d531f230` | 1 | Stage 3 all-or-nothing and the Objectives check both FAIL | Base cannot hide the delta |
| Archive redirected to an absent path | 2 | `FIVE_RACK_SUITE_INCOMPLETE` | m-5 holds |
| Non-empty `--out` | 1 | `--out exists and is not empty` | m-4 holds |
| T2 `check_sources.py` | 1 | `T2_SOURCE_TALLY PASS=38 FAIL=1` | **Still red, pre-existing** |
| `run_diagram_check.py --design …/stand.json` | 1 | `DIAGRAM_DESIGN_MATCH_FAIL lines=762` | **Still red, pre-existing** |
| `run_diagram_check.py` | 0 | `DIAGRAM_PLAN_PASS checks=1330` | Match |
| `CloudTypeCheck/run_typecheck.py --mode prod` | 0 | `TYPECHECK_CLEAN`, 6 distinct errors = baseline 6 | Approximates Unity 2021.3 with stubs. **Not a Unity compile.** |
| `StandMap/OfflineLogic/run_standmap_check.py` | 0 | 176 passed, 0 failed | Match |
| `git diff --check e7bda1f2 d531f230` | 0 | No lines | Clean |

**Pins verified:** `git show` of each pinned blob, the HEAD blob and the working-tree file all hash to the recorded values (`73e11149…`, `5ae49817…`, `b146d2fb…`). `git log` confirms `6a8b261f` and `69b41605` are the commits that last touched them.

**#303 stages:**
- Stage 1: `git diff --name-only 7f5278bf 1b68ddcd -- Assets Packages ProjectSettings` is exactly the four map files.
- Stage 2: `1b68ddcd..d531f230` is exactly the two diagram files plus Objectives (numstat 1/1).
- Production since `316429dc`: none. No directory prefix exists in `run_headland_check.py`.

## My mutations (39) and every false green

None of these is in the author suites as written. Each S case applies one change to a throwaway clone, then runs **both real checkers**. Five-rack ran with `--no-mutations` (82 source checks, including the structural and painter stages); the full-mode check is reported in F-1. FR is the five-rack exit code and HL the headland exit code.

| ID | Change | FR | HL | Rejected by / result |
|---|---|---|---|---|
| S01 | Second Objectives line edited (`"What to do"` label) | 1 | 1 | Objectives one-line check in both |
| S02 | Call site passes `!rackV2Objective` | 1 | 1 | Both; FR then crashes (F-6) |
| **S03** | One comment byte in `TeachingDiagram.cs` | **0** | 1 | **FR false green (F-1)**; HL stage 2 |
| **S04** | Title margin 6 to 60 UI units | **0** | 1 | **FR false green (F-1)**; HL stage 2 |
| **S05** | Note adds "Routes open." | **0** | 1 | **FR false green (F-1)**; HL stage 2 |
| S06 | Seam fires on the introduction, `lesson <= 0` | 1 | 1 | Source guard, structural, painter |
| S07 | Untracked `unrelated.png` + `.meta` under Resources | 1 | 1 | Allowed-files, no-new-Assets; stage 2 inventory |
| **S08** | Untracked script behind a nested `.gitignore` | **0** | **0** | **Combined false green (F-4)** |
| **S09** | Rack edit hidden by `--assume-unchanged` (adversarial) | **0** | **0** | **Combined false green (F-4)** |
| S10 | #303 test script deleted (partial stage) | 1 | 0 | Stage 3 all-or-nothing and byte check |
| S11 | Headland runner reverted to #297 bytes | 1 | 1 | Stage 3; old inventory |
| S12 | One-space #303 runner edit, **committed** | 1 | 0 | Stage 3 byte pin |
| S13 | #303 handoff deleted | 1 | 0 | Stage 3 |
| S14 | Original-30 guard deleted (0.25 s cadence) | 1 | 0 | Stage 3 pin. HL alone prints PASS at 38. |
| S15 | Original-92 C# check deleted | 1 | 0 | Stage 3 StandMap path. HL alone prints PASS at 91. |
| S16 | Look-alike `StandMap2/x.py` and `standmap/x.py` | 1 | 0 | Allowed-files |
| **S17** | Unrelated binary in the five-rack folder | **0** | **0** | **Combined false green (F-3)** |
| **S18** | Structural check deleted, `checks=108` | **0** | **0** | **Combined false green (F-3)** |
| **S19** | Painter assertion deleted, `checks=162` | **0** | **0** | **Combined false green (F-3)** |
| S20 | #303 integrity test `== 39` loosened to `>= 30` | 1 | 0 | Stage 3 pin |
| **S21** | #306 integrity test accepts any exit 1 | **0** | **0** | **Combined false green (F-3)** |
| S22 | "confirmed" in the draft key row | 1 | 1 | Planned-row-only check |
| **S23** | Entrance square 11 to 1 design unit | **0** | 1 | **FR false green (F-1)**; HL stage 2 |
| S24 | Draft fill opaque | 1 | 1 | Painter: fainter see-through fill |
| S25 | Map corridors half width | 1 | 1 | Structural rack width |
| S26 | USS `diagram-caption` edited | 1 | 1 | Stylesheet guard; stage 2 inventory |
| S27 | `NEEDS_NO_SCROLLING` audit removed | 1 | 0 | Allowed-files |
| S28 | Onboarding fit check removed | 1 | 0 | Allowed-files |
| S29 | `TeachingDiagram.cs.meta` GUID changed | 1 | 1 | No-new-Assets; stage 2 inventory |
| S30 | Near-name `TeachingDiagramExtras.cs`, committed | 1 | 1 | Same |
| S31 | `StandMapTransform.cs` edited | 1 | 1 | Same |
| S32 | Real caption gains a headland claim | 1 | 1 | Same |

Weakened copies, run through the authors' integrity tests with `--checker`:

| ID | Weakening | Integrity test | Result |
|---|---|---|---|
| **W1** | Five-rack pin follows HEAD, no hash | **PASS 14/14** | **False green (F-2)** |
| **W2** | Five-rack SHA-256 comparisons removed | **PASS 14/14** | **False green (F-2)** |
| **W3** | Five-rack accepts a partial #303 stage | **PASS 14/14** | **False green (F-2)** |
| W4 | Five-rack `HeadlandOffline/` directory allowance | FAIL, `303d` not caught | Detected |
| **W5** | Five-rack guard, 4 mutations and 2 expectations removed | **PASS, clean 131, 14/14, clean 81** | **False green (F-2)** |
| W6 | Headland exemption for `UI/Resources/` | FAIL at `1f` | Detected |
| W7 | Headland original guard deleted | FAIL, clean runs at 38 | Detected |

**Benign controls, all passed both checkers:** B01 unmodified candidate; B02 ignored `Library/`, `Temp/`, `obj/`, `Logs/`, `UserSettings/`, `.csproj`, `.sln`; B03 extra newline in the #306 handoff (allowed and unpinned). HEAD's runner on a clean `335ee6b3` also passed.

**Tally of the 32 S cases:** 26 rejected by at least one checker; **6 combined false greens** (S08, S09, S17, S18, S19, S21); 4 more are false greens for the five-rack runner alone (S03, S04, S05, S23). **No combined false green changes the production code of the candidate as committed.** S08 and S09 need hidden local state, and S17, S18, S19 and S21 are in the test folder. **Of the 7 weakened copies, 4 passed the integrity test.**

The dispatch's examples map as follows:

| Dispatch example | Result |
|---|---|
| Broaden a StandMap allowlist to a directory | W4 detected |
| Remove a pin hash | **W2 not detected by the integrity test.** A wrong hash constant is rejected by the real checker. |
| Accept a partial #303 stage | **W3 not detected by the integrity test.** The real checker rejects the partial stage (S10, S11, S13). |
| Revert one #296 diagram byte | **S03 not rejected by five-rack**; rejected by headland |
| Add an unrelated untracked asset | S07 rejected; **S08 (ignored) not rejected** |
| Edit a second Objectives line | S01 rejected by both |
| Delete an original 30-check guard | S14 rejected by five-rack; W7 detected by the #303 test |
| Weaken a #306 mutation expectation | **S21 and W5 not detected** |

## Assertion-retention audit

I compared named assertions, not totals, at runtime (`logs/`, `scripts/runtime_names.py`) and statically by AST (`logs/assertion-inventory.txt`).

| Inventory | From → to | Removed | Added | Verdict |
|---|---|---|---|---|
| Headland source, runtime | `1b68ddcd` (30) → HEAD (39) | 1 name | 10 names | **Retained.** The removed name is the four-file check; it returns as the stricter "only four authorised display files changed by #297 and only the recorded #296 delta since". The other 9 are new stage assertions. |
| Headland C#, runtime | `1b68ddcd` (92) → HEAD (92) | 0 | 0 | **Same names, same order** |
| Headland `protected` list | | `LearningObjectivesView.cs` only | | Moved to the tighter stage 2 byte check. Every other path stays. |
| Five-rack source, runtime | `335ee6b3` (131) → HEAD (140) | 1 name | 10 names | **Retained.** "no package, project-settings or Stand Map checker change" is split into "no package or project-settings change" and "no Stand Map path outside the two pinned #303 verifier scripts", plus 8 more stage 3 checks. "only allowed files changed" keeps its name. |
| Five-rack structural, runtime | `335ee6b3` (109) → HEAD (109) | 0 | 0 | **Same names, same order** |
| Painter probe, runtime | `335ee6b3` (165) → HEAD (165) | 0 | 0 | **Same names, same order** |
| Mutations | `335ee6b3` (29) → HEAD (29) | 0 | 0 | **Name, old, new and expected text all identical** (AST). All 29 applied and caught. |
| Five-rack C#, static | `e7bda1f2` → HEAD | 4 relocated | 13 | The #296 relocations are stated in its handoff and listed under M-1. "Every symbol explained" was split into both directions, which is stricter. |
| Five-rack Python, static | `e7bda1f2` → HEAD | 2 | 21 | "Lesson 0 draws the figure" became the stricter selection guards. The caught-mutation condition changed from `returncode != 0 and any(...)` to `bool(hit)`, which is equivalent in practice because `hit` needs a FAIL line. |

**Result: no original substantive assertion was silently deleted, downgraded or masked at this HEAD in either checker.** No skip, waiver or expected-failure path exists in either. What is missing is a guard that keeps it that way for the five-rack side (F-2, F-3).

## Protected-file audit

`git diff --name-status 1b68ddcd d531f230 -- ':!Docs' ':!Tools'` lists **only** the three authorised files. So these are all unchanged since `1b68ddcd`: physical geometry, rack layout (`ScenarioOneSuggestedRackLayout.cs`, `Racks/`), saves (`ForestSaveData.cs`), scenes (`ForestTest.unity`), prefabs, USS, the Stand Map source files, `StandMapView`, `WorkPlanView`, `HudMinimapView`, ecology, economy, Reference v1 and the minimap. Since `7f5278bf` the production delta is exactly those three plus the four #297 map files.

- No file was added under Assets, Packages or ProjectSettings, so there is no new script or `.meta`.
- Production bytes have not changed since `316429dc`.
- The ZX20 lane is clean at `d531f230`, with no ignored production paths.
- The report lane contains only this file. I did not touch `active-tasks.md`, `main`, user saves or any user checkout.

## Carried minors from the first review

| ID | Status at this HEAD |
|---|---|
| m-1 key/map completeness, dashed boundary | **Addressed.** Boundary is solid, clearing named, converse check added, P11 caught |
| m-2 tree-on-surface self-test | **Addressed.** Independent check; Q3 caught |
| m-3 one-way approach fidelity | **Addressed.** 110 points, worst 0.01261 m; Q4 caught |
| m-4 default `--out` deleted evidence | **Addressed** for five-rack. Same class remains in the headland runner (F-7) |
| m-5 PASS without the archive | **Addressed.** Exit 2, `FIVE_RACK_SUITE_INCOMPLETE` |
| m-6 lesson purple vs Stand Map amber | **Open, carried** to user clarity |
| m-7 "painter methods unchanged" overstated | **Addressed** by the probe's byte comparison |
| m-8 mesh cost | **Open, carried** to Unity performance. The dump shows 390 canopy fills, each with a stroke. |

Also carried: #297 notes m-1 to m-5, the five-to-two-rack change in lessons 2–8 (the Manager ruled not to change it), grayscale stripe contrast, and one world's seeded positions.

## Process and independence of #306

**P-1. The #306 claim shows no execution record.** The API returns 0 comments on #306, title `[MANAGER PACKET READY / NOT RUNNING]`, last updated 15:07 UTC, before the work began. The ruling requires `[RUNNING]` at genuine start and `[HANDED-OFF]` after evidence, and the handoff says the end SHA "is in the #306 [HANDED-OFF] comment". That comment does not exist.

**P-2. The cited preflight record does not support the claim.** The handoff says "Preflight (fresh, actual): `git status --porcelain=v1 -uall` empty" and cites `preflight.log`. That log is dated 15:47:54, about 30 minutes after `before-original.log` began, and it lists ` M run_five_rack_check.py` and `?? test_stage_inventory_guard.py`. It cannot show a clean start. I found no earlier status record among the evidence files I read. I am not claiming the lane was dirty at start; `before-original` and the HEAD values are consistent with a clean start.

**P-3. The deviation, stated factually.** #306 was implemented directly by the DeepSeek V4.1 Flash orchestrator rather than a separate worker. The commit author is `DeepSeek Orchestrator <deepseek@invalid>`, and the handoff discloses this. It reports effort as "the harness default"; no observed band exists.

- **Does this limit evidence independence?** Yes. The same session routed the work, wrote the code and produced all the #306 evidence, so no second party had looked before this review.
- **Do the code and tests stand on their own when rerun?** Yes, for what they claim. Every #306 tally and the before/after reproduced exactly from a fresh clone, and the stage 3 logic behaved correctly under every direct test I gave it.
- **What rerunning cannot show:** what the suite leaves out. F-1, F-2 and F-3 are there, and the handoff did not disclose them. I cannot say whether a separate worker would have avoided them.

**Low-effort author note.** #296 and #303 were written at observed effort 15. I found no production defect in #296's source and no requirement gap in #303. The #303 integrity test is the stronger of the two.

## NOT RUN

Source review cannot satisfy any of these. They remain required by the Manager ruling before acceptance:

- Unity 6000.6.0f1 import or compile, Editor, Play Mode;
- rendered 1280×720 and 1920×1080 Objective card captures and the no-scroll assertions;
- colour and grayscale review, and two-reviewer comprehension;
- native input;
- real old saves, Legacy40 and Reference Future v1 at runtime;
- real headland, map and 3D checks at runtime;
- performance and mesh cost;
- user playtest;
- Windows and Player builds.

The painter probe, `.NET` runs and `CloudTypeCheck` are source and stand-in evidence only.

## Requests

- **Manager #200:** rule on F-1 (repair, or accept the combined coverage as a recorded caveat), and on F-2, F-3, P-1 and P-2. This report does not approve the candidate and does not release #287.
- **If a repair is released:** F-1, F-2 and F-3 are confined to `run_five_rack_check.py`, its integrity test and a new handoff path, which the runner must then allow by exact name. Nothing pins the five-rack runner's bytes, so this does not disturb the #303 pins. This reviewer should not author it.
- **Two lanes:** the parallel lane named above is reviewing the same candidate. The Manager should decide which report answers #307.
