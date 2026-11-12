# #435 Independent review of the #427 FR-10 rendered oracle and Tier-2 Unity evidence

Date: 12 November 2026. Reviewer: **Claude Opus 5.5 (`claude-opus-5-5`), Anthropic, Claude A**, T3 Code on Predator. Child claim #435; parent #412. No subagents. I wrote none of the view, oracle, drivers, fixtures or Unity runs under review.

I worked **read-only**. I started no Unity Editor, edited no source, tests or evidence, and only read the lane `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` and the evidence under `/media/jer/Files/CCF/Verification/D073-FR10-427-20261112/`. This file is the only thing I wrote.

## Verdict

**APPROVED** for this question: *do the #427 oracle and Tier-2 evidence adequately support using source `339517ed2d49bb3d5716fc9cf029d9ef0eaeee72` as a personal Unity Editor playtest candidate?* The approval does not cover an external demo, a Player/Windows build, Tier 3, integration, or user acceptance.

There are no Blockers and no Majors. The published PASS is supported by the raw data. The F-1 disposition ("harness defect, not a regression from the repair") is justified, and is better supported than #427 itself argued: the first-run raw data contain a **second lost click on an unrelated control** in the same 25-second window, plus xdotool timing proof that the OS pointer was being moved during that window (finding M-1). Neither #427 worker noticed this, and the published record misexplains one inherited check count, so a small correction is needed for the record. It does not change the verdict.

## Identities checked

| Item | Value |
|---|---|
| Candidate source | `339517ed2d49bb3d5716fc9cf029d9ef0eaeee72` (parent `77a56a44…`) |
| Candidate tip | `55863f1b7719011bba16b3f3ab377e379c0b988a` |
| Qualification tip / report | `ab92cbdf0f00f4875e6ae2485c22a1dab0fd693f` (first-run report `87b1d157`) |
| Runner commits | `87405a56` (first run), `b219ca70`, `2539782c`, `f0b79863`, `3e281aff`, `27e1b680`, `d9970c06`, `73d7876f`, `e2a91857` |
| Candidate view SHA-256 | `8e51c9eb…cfeac86` (= `git show 339517ed:…/AnnualReviewView.cs`, and = lane file now) |
| Unrepaired view SHA-256 | `174617e6…266b429a` (= `git show 77a56a44:…`) |
| Logs | `candidate1/FR10.log` SHA-256 `dcc6e361…` and `negative1/FR10.log` `d1c9d523…`, both matching the report |

## Q1: Oracle fidelity (`Tools/Verification/YearComparison/Fr10RenderedOracle.cs` @ `ab92cbdf`)

**What it does correctly (verified in code and in the log):**

- It inspects the **live** `AnnualReviewView` tree (`review.Root.Q("annual-year-comparison")`, line 182), not a copy. Elements are looked up by name (`<P>-values`, `<P>-year-a|year-b|difference`, and their `-value`, `-heading` and `-tag` labels; lines 214–225).
- Each entry must be a direct child of `<P>-values` (line 223). The `-values` child count must equal the number of non-empty mapper values (line 215). The tag is looked up **inside** its own entry (line 225). An observation and the difference therefore cannot share one tag: a shared or row-level tag leaves the per-entry tag null, and the "exact chip" check fails (line 230).
- The provenance expected for each column is a **fixed constant** (`Recorded` for columns A/B, `Derived` for the difference; line 229), not a value copied from the mapper. A mapper that mislabelled provenance would fail this check.
- Missing elements are never skipped silently on measure rows. A missing entry records a FAIL and then `continue`s (lines 223–224). An empty mapper value requires the entry to be **absent** (line 222). In the log, `missing-latest` has 40 PASS "absent record has no false value/tag" checks on `year-b`/`difference` across 4 runs.
- Hidden chips are not treated as present. `Displayed()` walks every ancestor's resolved `display` and `visibility` and requires non-zero world bounds (lines 141–147). With sources ON the tag must `Fits()` inside the ScrollView viewport and panel after `ScrollTo`. With sources OFF it must be `!Displayed` (line 233).
- Every NOT RECORDED line must have exactly one `prov` chip, reading `NOT RECORDED`, displayed if and only if sources are ON (lines 203–208).
- Fixture effects are asserted explicitly for `ambiguous` (`CannotCompare` and 0 measures), `event-only` (`work-event-only` row) and `ambiguous-event` (lines 239–241). Read-only state is asserted per rendered case: canonical state, world hash and event count (line 242). I counted 28/28 `exact state/hash/events unchanged` PASS lines and 28 `FR10_WORLD` lines with equal before/after hashes.
- The expected values come from `ScenarioOneYearComparisonText.Describe(...)`, the same mapper the view uses. That is appropriate here: FR-10/B-1 is a presentation defect, and the mapper is covered separately (132 real-type controls, offline mapper suite, #426). So "recomputed the same way" is not a fidelity gap for this question.

**Gaps (none would let the specific B-1 defect pass; see m-2):**

- No geometric non-overlap assertion between value, heading and tag, or between columns. `Fits()` checks only containment inside the viewport and panel. Overlap is covered only by screenshot review.
- Enumeration is driven by the mapper. Rendered elements that are **not** in the mapper output (a stray extra row, an extra row-level chip, stale measure rows in a `CannotCompare` or missing-selected card) are caught only indirectly, by the duplicate-name check and the exact-`row.Text` "no mixed label" check (line 216). A re-formatted mixed label elsewhere in the card would not be caught.
- The DERIVED/RECORDED chips on **non-measure** lines are not asserted. Examples: the two event-outcome count lines and the "Stand recorded at the end of Year N…" line (line 199 checks text and visibility only; chips are checked only for NotRecorded). FR-10 covers "every displayed value"; the screenshots show these chips correctly.
- For `missing-selected`, `missing-latest` and `both-missing`, no oracle line asserts that the fixture actually produced the missing-snapshot row. The log shows those rows rendered with exact text and NOT RECORDED chips (e.g. `missing-selected` exact-line/uncertainty/chip-display checks ×4 runs), and inherited `NegativeViews` covers mapper semantics. The claim "fixture-specific … states are asserted" (report line 47) is therefore only partly true for the missing modes.
- Rows are skipped silently in two places: `SpeciesHeading` rows (line 196), and a missing `GuidancePointer` label (line 198).
- Chip visibility does not consider opacity or colour.

## Q2: Negative control

- It really ran in Unity with the unrepaired view. `run_unity_427.py` lines 41–46 admit a dirty lane only if the sole change is `AnnualReviewView.cs`, byte-equal to `git show 77a56a44:`, and only with `CCF_FR10_NEGATIVE=1`. The follow-up wrapper (`run_followup_427.py`) runs the same guard. The failure pattern itself proves the old view was compiled.
- Recount from `negative1/FR10.log`: **39 PASS / 170 FAIL** FR10_CHECK, 2 cases FAIL, D073_CHECK 2/0, `error CS` 0. The 170 failures break down as 102 × "own value entry" (17 measures × 3 columns × ON/OFF), 34 × "separate entries" and 34 × "no mixed observation/difference label". Those are exactly the B-1 symptoms. The 39 passes are the setup checks plus non-measure lines, card existence, no duplicate names, 17-measure enumeration and read-only checks. It failed for the right reason. Scope: 1280x720 only, as the report states.
- Restoration: `restored-candidate-assets.diff` is 0 bytes, and so are `followup1/base-diagnostic-…` and `base-replica-stress-restored-candidate-assets.diff`. `git diff 339517ed <commit> -- Assets Packages ProjectSettings` lists **0 files** at all ten commits from `87405a56` to `ab92cbdf`. The protected manifests after the negative and both base sessions are identical to `production-before.json`, which records the candidate hash `8e51c9eb…` for `AnnualReviewView.cs`.

## Q3: Old assertions and production tree

- `git diff --name-status 55863f1b ab92cbdf`: **16 files, all `A` (added), 3,319 insertions, 0 deletions**. Nothing pre-existing under `Tools/Verification/**` was modified or deleted.
- The seven "inherited" files (`D073Verification.cs`, `D073BaselineAnnualReviewView.cs`, `D073ReferenceIntegrity.cs`, `Cadence408GameSize.cs`, `RackAuthoringNativeInput.cs`, `d073_native_input.py`, `run_unity_417.py`) are **byte-identical** to `origin/qualify/scenario-one-d073-year-comparison-unity-20261112-v1` (#417, `2b0bbf1f`). `git diff 2b0bbf1f ab92cbdf -- Tools/Verification/YearComparison` shows only the eight new #427 files.
- `87405a56..ab92cbdf` adds only five new test files plus the report. The first-run oracle, inherited files and driver were not touched after the first run.
- The first-run report text is unedited: `87b1d157..ab92cbdf` on the report is 177 insertions and 0 deletions.
- `D073Runner.Check` throws on failure (`D073Verification.cs:54`), so inherited failures surface as case exceptions in both runners. Nothing is masked by the separate counters.

## Q4: F-1 disposition

**What I re-read myself.** I read `candidate1/driver/request-0014…0021.json`, `native-actions.jsonl` (78 lines) and desktop before/after captures for requests 14–21 and 33–39 (pixel diffs, threshold 40). I also read the `candidate1/FR10.log` window from line 15847 to 16094, the follow-up `native-actions.jsonl` timings and the follow-up `click-trace.txt` lock/focus fields.

**Confirmed.** Before 15 the foldout is closed. After 15 it is open, with hover tint. Request 16, at the same point (957,512), changes only a 29×6 px box: hover lost, no toggle. That matches #427.

**What #427 missed (finding M-1).**

1. **The pointer was being displaced, and the timing proves it.** The first-run driver uses `xdotool mousemove --sync`. Whenever the target equals the current pointer position, this blocks for about 15 s. In the first run, every repeated same-point move took 19.2–20.2 s (requests 2, 3, 10, 11, 23, 30, 31, 36, 38, 39). The **only** exceptions are requests **16, 18, 19 and 20** at 1280x720 (4.5, 5.0, 8.3 and 5.2 s). In every `--sync` follow-up session (candidate-diagnostic, base-diagnostic, candidate-faithful, candidate-full), all 46 repeated moves blocked for about 15 s. So between requests 15 and 20 something kept moving the OS pointer away from where the driver had left it. This is direct evidence for #427's "pointer displaced" hypothesis, which #427 described as only inferred.
2. **A second lost click, on a control the repair does not touch.** In the probe's "Later year ▶" loop at 720p, request 17 goes from Year 3 to Year 4. Request **18 leaves Year 4 at Year 4**; the before capture shows the button hovered, and the after capture shows the hover gone and nothing else changed (diff box 808–852 × 456–467, 448 px). Requests 19 and 20 go to Year 5 and Year 6. The loop retried, so `native later returns follow-latest` still passed. As a result 720p needed **4** Later clicks against **3** at 1080p (requests 37–39, all advancing, with normal 19–20 s blocking).
3. **So the published explanation of the 7-versus-6 inherited checks is wrong.** Report lines 59 and 114 attribute the extra 720p check to "the extra opening click … attempted when needed". In fact **both** sizes started closed and sent open plus close (720p requests 15/16; 1080p requests 35/36, the latter a full close diff and a 19.7 s block). The extra check is the retried, lost Later-year click. The follow-up also calls F-1 "the single first-run failure" and "one event"; it was a roughly 25-second episode that affected at least two clicks.

**Assessment.**

- *Harness defect, no regression from the repair:* **justified, and stronger than argued.** The same signature hit a "Later year" button that `339517ed` does not change. The view diff is labels only (`git diff 77a56a44 339517ed`: one file, +42). The anomaly is confined to one window, and the pointer was moved by something other than the driver. Several things push the same way:
  - The unchanged original probe replayed 15/15 (9 candidate: 6 in `candidate-full`, 3 in `candidate-faithful`; 6 base in `base-replica-stress`).
  - Human-paced single clicks passed 16/16 on both the candidate and base sessions.
  - The 400 stress clicks lost none, apart from the 2 caused by the worker's own helper.
  - The Game view in this harness is drawn at about 0.44 scale: the 720p `targetRect` is 564×317 desktop px (`D073_NATIVE` lines). The toggle is therefore only about 10 desktop px tall at 720p (about 15 px at 1080p), so a small pointer displacement is enough to lose a 720p click.
- *What remains unproven:* the source of the displacement. Traced runs show `cursorLock=None appFocused=True` in all 658 trace lines, so no evidence points at the game's cursor-lock or warp paths (`ForestPlayer.cs:336` warps only when the cursor is locked). The Sonnet worker's own counter bug and parker-helper incidents are honestly disclosed. They are not evidence about the first run, and they are not counted as passes.
- *Acceptable residual risk for a personal Editor playtest?* **Yes.** The risk is that an unknown process in the private Xephyr harness displaced the pointer. A real player clicks at native scale on their own desktop, and nothing in the evidence ties the displacement to `AnnualReviewView`. This is **not** a waived failure: the FAIL stays on record (`FR10_CASE 1280x720/native-foldout-close-and-latest FAIL checks=2` in the immutable log, report text unchanged), and the behaviour was re-tested.
- *Is the corrected probe fair?* **Yes, not weaker.** `Fr10Followup427.CorrectedClose` (lines 318–330) tests the same behaviour: native open, then native close of the species foldout at 720p, followed by a native return to follow-latest. It adds an assertion of the closed starting state, a read-only world check, and pointer hygiene (move away, 1.1 s settle, move back). It puts the toggle mid-viewport rather than at the `ScrollTo` edge; the diagnostic phase also passed at viewport fractions 0, 0.5 and 1 on both candidate and base. The exact original sequence is still covered by the 15 replays.
- *Like-for-like caveat (Note):* the six base replays ran with the fast driver (no `--sync`, no captures). The candidate replays ran with the `--sync` driver. The like-for-like base comparison is the `base-diagnostic` human-paced probe (same `--sync` driver), 16/16.

## Q5: F-2

- The four original failures are on record. In `candidate1/FR10.log`: `ambiguous-event-on|off` FAIL at both sizes, with 4 × `D073_CHECK FAIL` from the rejected load. They are also in `candidate1-failures.txt`. The 580-file `followup1/prior-evidence-preservation-before.json` still matches every file byte-for-byte (I rehashed all 580: 0 changed, 0 missing).
- The new fixture is valid and exercises the intended ambiguity. `Fr10Followup427.AmbiguousEvent` (lines 332–354) clones a Year-1 `WorkResolved` event with a new maximum event ID and bumps `nextManagementEventId`. `ForestSaveValidation.ValidateManagementEventIds` (lines 155–167) rejects only non-positive or repeated **event** IDs. `ScenarioOneManagementFollowonReadout.cs:166–178` marks repeated resolved `workOrderId`s as ambiguous, and `ScenarioOneYearComparisonText.cs:219–221` emits the NOT RECORDED `work-ambiguous` line. The log shows, ×4: `distinct event IDs … 319/538 order=1`, `valid loaded world exercises exact ambiguity`, `actual warning retained … "2 work record(s) have ambiguous provenance and are not counted."`, and `FR10_CHECK PASS full uncertainty never RECORDED work-ambiguous | NOT RECORDED`.

## Q6: Published table versus raw data

Recount from anchored line starts only. Diagnostic substrings are not counted.

| Log | FR10_CHECK P/F | D073_CHECK P/F | Cases P/F | `error CS####` | DONE line |
|---|---|---|---|---|---|
| `candidate1/FR10.log` | 5,063 / 5 | 233 / 4 | 42 / 5 (720p 20/3, 1080p 21/2, mapper 1/0) | 0 | `checks=5068 failures=5 casesFailed=5 inheritedChecks=237` |
| `negative1/FR10.log` | 39 / 170 | 2 / 0 | 0 / 2 | 0 | `checks=209 failures=170` |

Rendered ON/OFF is `checks=409` PASS at both sizes.

| Follow-up log | FOLLOWUP P/F | Oracle P/F | D073 P/F | CS |
|---|---|---|---|---|
| candidate-diagnostic | 19 / 0 | 0 / 0 | 2 / 0 | 0 |
| base-diagnostic | 19 / 2 (both the disclosed `IOException` counter bug) | 0 / 0 | 14 / 0 | 0 |
| candidate-full | 35 / 0 | 1,664 / 0 | 122 / 0 | 0 |
| candidate-faithful | 6 / 0 | 824 / 0 | 55 / 0 | 0 |
| candidate-stress | 9 / 2 (both the disclosed parker-induced losses) | 0 / 0 | 2 / 0 | 0 |
| base-replica-stress | 15 / 0 | 12 / 0 | 108 / 0 | 0 |

All match the report.

**Screenshots.** All 184 PNGs (108 `candidate1/evidence`, 76 `followup1/*/evidence`) have exactly the pixel size in their names, and none is near-blank (luminance range ≥ 30). I viewed nine at full size:

- `1280x720-full-on-stand-canopy`
- `1280x720-full-off-species-beech-standing-trees`
- `native-1280x720-expanded-species-caveat`
- `1280x720-missing-selected-off-missing-selected`
- `1280x720-native-extra-closed-latest`
- `1920x1080-full-on-species-sessile-oak-standing-trees`
- `1920x1080-missing-latest-on-missing-latest`
- `1920x1080-event-only-off-work-event-only`
- `1920x1080-species-one-sided-on-species-beech-not-recorded`

I also viewed `followup1/candidate-full/evidence/1280x720-distinct-ID-ambiguous-on.png` and `…-corrected-native-closed.png`. Sources ON shows separate End-of-Year-A RECORDED / End-of-Year-B RECORDED / Difference DERIVED, with the value under each heading. Sources OFF hides the chips and keeps "calculated from records" and the values. NOT RECORDED chips appear on missing, one-sided and ambiguous lines. With the 720p species section open, the rows and the fixed caveat ("These are recorded stand totals for two years…") are readable and reachable by scrolling. The only cut-off text is ordinary ScrollView boundary clipping. Nothing is black, blank or horizontally clipped.

**Cases not re-run after the follow-up** (b, c, e, f, g at 1080p; e, f, g at 720p). Relying on the first-run results is **valid**. Between `87405a56` and the follow-up, the production tree was identical (empty Assets/Packages/ProjectSettings diff at every commit), and so was the code under test for those cases (`Fr10RenderedOracle.cs`, `D073Verification.cs` and the driver are unchanged). The first run's unexpected material drift occurred during that run, came from the Editor itself, and only affects art materials, not UI.

## Q7: Custody

- **Leases.** All eight release receipts (`negative1`, `candidate1` and six follow-up sessions, including the orphaned Codex lease `aadacab8…`) have `"status": "released", "forced": false` and no Editor PIDs. The parent disposition (#427, 18:11 UTC) authorised the follow-up and the ten-path restoration. The successor's takeover and orphan close-out are documented in the 18:26 and 18:38 comments.
- **Manifests, recomputed from the stored files.**
  - `host-before.json` (37 entries) is identical to all ten host-after manifests.
  - `production-before.json` (4,011 entries) is identical to `negative1-production-after`, all six follow-up `*-production-after` files, `final-production-after` and `production-restored`.
  - `candidate1-production-after` and `publication-production-after` differ in exactly the 10 archived drift paths, as reported.
  - The per-session follow-up *before* manifests equal the originals.
- **Lane now.** `ab92cbdf` on the qualify branch. `git status --porcelain -uall` is empty. `AnnualReviewView.cs` SHA-256 is `8e51c9eb…`. No `Fr10*`, `D073*`, `Cadence408*`, `RackAuthoringNative*` or `YearComparisonVerification*` copies remain under `Assets/ForestPrototype/`. `unity-lock status`: `free`, no Editor PIDs. No Unity Editor process is running.
- **Read-only viewing.** This is asserted, not inferred. There are 28 per-case canonical-state, hash and event-count assertions, inherited `NativeSize`/`ReadOnlyAB` nonmutation checks, and the corrected probe's world-equality check.

## Findings

| ID | Severity | Finding | Evidence | Smallest remedy |
|---|---|---|---|---|
| M-1 | **Minor** (record accuracy; supports the verdict) | F-1 was a multi-click pointer-displacement episode, not a single event. A second lost click (request 18, "Later year ▶") has the identical signature. The first-run explanation of the 7-versus-6 inherited checks ("extra opening click") is wrong: both sizes sent open and close, and the extra check is the retried lost Later click. | `candidate1/driver/native-actions.jsonl` timing (requests 16/18/19/20 not blocked against 10 blocked repeats); `desktop-{before,after}-0018.png`; requests 35/36 and 37–39; `FR10.log` lines 15847–16094; report lines 59, 114 and the follow-up "single" wording | Parent records this erratum when dispositioning #427 (no rerun). Optionally keep a `--sync`-timing check as a cheap pointer-displacement detector in future native harnesses. |
| m-2 | Minor | The oracle has no overlap assertion, no assertion on DERIVED/RECORDED chips for non-measure lines, and no check for rendered elements outside the mapper output (stray rows or chips, stale measure rows in CannotCompare or missing cards). The missing-snapshot fixture effect is not asserted explicitly. | `Fr10RenderedOracle.cs:149–156, 194–211, 216, 238–241` | If a future FR-10 change happens, add these to a new oracle file: no `-values`/RECORDED chip when the mapper has 0 measures; per-line chip text for all non-measure rows; pairwise non-overlap of value/heading/tag rects. Not needed for this playtest. |
| n-3 | Note | The unknown pointer-displacement source is a residual harness risk. | traces: `cursorLock=None` ×658 | During the personal playtest at a small Game-view size, watch the By-species foldout and year buttons for missed clicks or pointer jumps. Report one, don't assume. |
| n-4 | Note | The base replicas used the fast driver (no `--sync`); the candidate replicas used `--sync`. | `followup1/base-replica-stress/driver/native-actions.jsonl` (0 s moves) | None. `base-diagnostic` (`--sync`, 16/16) is the like-for-like base comparison. |
| n-5 | Note | Follow-up `faithful-rendered-*` case lines cannot show oracle failures (`checks=0`). The exit code and `oracleFailures` do include them, and both are 0. | `Fr10Followup427.cs:181–182, 203–206` | None needed now. |
| n-6 | Note | The first-run worker added `inspect-*.png` crops into `candidate1/driver/` at 17:39–17:40 while the candidate Editor was still running. This is additive only; the raw captures were unchanged. | file mtimes against `FR10.log` 17:55 | Keep inspection crops outside the live run directory in future. |

## What I did not check

- I ran no Unity or other runtime gate. Every Unity result above is the workers' evidence, recounted and inspected, not reproduced.
- I did not rehash the lane's 4,011 files myself. I compared the stored manifests with each other and checked the lane's `git status`, the view hash and the lock state.
- I viewed 11 of 184 screenshots and none of the contact sheets. Of the 362 follow-up desktop captures I examined none, beyond the timing and trace logs. I did not examine the first-run captures outside requests 14–21 and 33–39.
- I did not re-verify the 1080p cases d–g beyond log recounts. I did not check `helper-offline.log`, `mapper-offline.log`, `typecheck*/compile.log`, Ops board receipts or licence-copy removal receipts.
- I did not re-review the `AnnualReviewView` source (APPROVED in #426), the inherited `D073Runner` method internals beyond `Check`/`Restore`/`NativeClick`, the old #417 99-check or #408 143-case suites, or any Reference biological continuation (NOT RUN by #427, as disclosed).
- I did not identify the cause of the first-run pointer displacement.
