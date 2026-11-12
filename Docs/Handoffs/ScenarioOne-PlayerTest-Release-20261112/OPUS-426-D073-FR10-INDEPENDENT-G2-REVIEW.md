# #426 — D-073 FR-10 visible-provenance repair: independent G2 source review

**Date:** 2026-11-12. **Verdict: APPROVED (G2 source only).** **#419 B-1 is CLOSED at source level.** Findings: **0 Blocker, 0 Major, 2 Minor, 4 Note.** None of them blocks the focused Tier-2 Unity qualification.

**NOT UNITY-VERIFIED.** I did not run Unity, build a Player, or render the UI. No offline fixture in the repository builds `AnnualReviewView`'s visual children, so how the card actually looks at 1280×720 and 1920×1080 is still unverified. The rendered-label oracle and Unity run belong to #427 and are not part of this review.

| Item | Value |
|---|---|
| Child claim / parent | [#426](https://github.com/jermur788/CCF/issues/426) / [#412](https://github.com/jermur788/CCF/issues/412); defect source [#419](https://github.com/jermur788/CCF/issues/419) B-1; writer [#422](https://github.com/jermur788/CCF/issues/422) |
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`), Anthropic, Claude A account, T3 Code delegate. I am not the writer (#422, Codex) and not the parent (#412, Fable). Work done directly, no subagents. |
| Reviewed source commit | `339517ed2d49bb3d5716fc9cf029d9ef0eaeee72`, tree `96ded36c897dbd92c44f7008a33838a1be63bcf4`, parent `77a56a4461f8ab2d313082af50442ce24dc54471` |
| Branch tip | `55863f1b7719011bba16b3f3ab377e379c0b988a` on `feature/scenario-one-d073-fr10-visible-provenance-20261112-v1`. The tip adds only the #422 handoff. |
| Authority read | Manager #412 one-go packet (Phase 1 G2 bullet) and #419 disposition at `daeef0b0`; #419 result at `origin/docs/d073-t06-independent-browser-review-20261112-v1`; approved spec at `origin/docs/manager-d073-approved-scope-20261112-v1` (`53dec8db`), FR-9/FR-10 lines 100–110 |
| Local evidence | `/media/jer/Files/CCF/Verification/D073-FR10-426-20261112/`: `tree/` and `base/` are `git archive` extracts of `339517ed` and `77a56a44`, and `gates/` holds all logs. Nothing was run inside any worktree. |

## A. B-1 closure / FR-10

**Source-level answer: yes.** In the D-073 card, each Year A and latest Year B observation is now its own value, with a chip taken from the mapper's `ValueProvenance`. The difference is a third value, with a chip taken from `DifferenceProvenance`. The view no longer draws a `row.Text` line with a single label for any stand or species measure row.

Code path, `Assets/ForestPrototype/UI/AnnualReviewView.cs` @ `339517ed`:

- `:229-246` handles `StandMeasure` and `SpeciesMeasure` rows. It draws:
  - the measure name (`:233-234`), keeping the species-name prefix on the first row of a species;
  - a wrapping `row row-wrap` box (`:235-238`);
  - three calls to `AddYearComparisonValue`, then `continue` (`:239-245`). Year A and Year B both pass `row.ValueProvenance` (`:240`, `:242`). The difference passes `row.DifferenceProvenance` (`:244`).
- `AddYearComparisonValue` (`:254-273`) draws one column as:
  - a caption taken from the TableHeader (`:264`);
  - its own `annual-text-source` box holding a chip from the existing `Tag` helper (`:265-270`, `Tag` at `:402-412`), which keeps the `prov` / `prov-recorded` / `prov-derived` classes;
  - the value text exactly as the mapper wrote it, units included (`:271`).
- The mapper sets `ValueProvenance = Recorded` and `DifferenceProvenance = Derived` for every compared stand and species measure (`ScenarioOneYearComparisonText.cs:295-296`, `:340-341`). So the chips read RECORDED, RECORDED, DERIVED, and nothing is hard-coded in the view.

**Every row kind the mapper can produce** (mapper `:85-170`), sources ON:

| Row kind / state | What the view renders now | FR-10 |
|---|---|---|
| `Heading` | Plain heading, no chip (no value) | OK, unchanged |
| Work lines (`work-outcomes`, `work-failed-cancelled`, `work-none`) | One `DERIVED` chip on the line (`:247-250`). Every number on these lines is an event-derived count. | OK, unchanged |
| `work-ambiguous`, `work-unresolved`, `work-event-only` | One `NOT RECORDED` chip plus the full warning sentence | OK, unchanged (FR-5) |
| `missing-selected`, `missing-latest`, `cannot-compare` | One `NOT RECORDED` chip plus the full sentence | OK, unchanged (FR-9) |
| `snapshots` / `snapshot-selected-only` | One `RECORDED` chip | OK as the spec draws it. See N-1 about "{B−A} year(s) later". |
| `TableHeader` | Still one plain untagged line (`Provenance` is null; mapper `:244-257`). Its fields now also act as the captions of every value column. | No value shown. See m-1. |
| `StandMeasure`, Compared | Measure name, then **End of Year A [RECORDED] value**, **End of Year B [RECORDED] value**, **Difference [DERIVED] value** | **Fixed (B-1)** |
| `StandMeasure`, LatestSnapshotMissing | Measure name, then **End of Year A [RECORDED] value** only. Year B and Difference are empty, so `:258` draws no column and no chip. The `missing-latest` `NOT RECORDED` line follows (mapper `:144-146`). | OK (FR-6, FR-9) |
| `SpeciesHeading` | Same foldout as before, same name and same `openDetails` key (`:196`, `:203-210`) | Unchanged |
| `SpeciesMeasure`, first row of a species | "Sessile oak: standing trees", then the same three tagged columns | **Fixed (B-1)** |
| `SpeciesMeasure`, later rows | Inside the existing 10 px indented box (`:217-223`), then the same three tagged columns | **Fixed (B-1)** |
| `SpeciesNotRecorded` | One `NOT RECORDED` chip plus the full sentence naming the missing year, with no numbers (`:247-250`) | OK, unchanged (FR-8) |
| `Caveat`, `GuidancePointer`, latest-year hint | Plain muted text, no chip. The pointer is still skipped when no guidance card exists (`:201-202`). | Unchanged |

So no displayed number still sits under one mixed label, apart from the spec-drawn elapsed-years phrase (N-1).

**Sources OFF.** The writer's reading is **correct**. FR-10 (spec line 110) says the card "MUST respect the existing Show data sources toggle in the same way as the rest of the review". The rest of the review hides chips only, through `.hide-data-sources .prov { display: none; }`. `ScenarioOneUi.uss:594-595` comments that rule as "disclosure affects chips only; plain uncertainty remains visible", and the toggle wiring at `:70-73` and `:132` is unchanged.

With sources OFF:

- the RECORDED and DERIVED chips are hidden;
- dated captions, values, units and every NOT RECORDED or event-only warning sentence stay visible;
- `Tag` also adds the plain qualifier text "calculated from records" before every DERIVED chip (`ScenarioOneTeachingSequence.cs:11-21`). That text is not `.prov`, so it stays visible. The Difference column therefore still reads as calculated even with sources OFF. This is exactly what every other DERIVED line in the review does.

Keeping chips visible with sources OFF would itself break the "same way as the rest of the review" clause.

## B. Correctness risks examined

- **`tableHeader` null dereference (`:195`, `:227-228`, `:239-244`).** Not reachable with the frozen mapper. Both branches that emit measure rows call `AddTableHeader` first (mapper `:144-145`, `:153-154`), and species rows exist only in the Compared branch, after `AddSpeciesRows` has added the `SpeciesHeading` (mapper `:306-307`). I checked this against **real mapper output** with a reviewer probe (section F):
  - 43 cards, 206 measure rows and 598 value columns;
  - zero measure rows before a header;
  - zero species rows before the foldout heading.

  A mutant mapper with the header moved after the stand rows makes the probe report the would-be NullReferenceException, and the existing 132 fixture also fails on it. The dependency is unguarded, though (N-2).
- **Wrong column captions.** Species rows reuse the stand TableHeader. That is correct, because both compare the same two years. Whenever a measure row has a Year B value, the header also has a Year B caption (probe check).
- **Early return on empty value (`:258`).** A column is empty only in the LatestSnapshotMissing stand rows (Year B and Difference). FR-9 requires the Year A observation, then the `[NOT RECORDED] … latest year` sentence, and the mapper emits that sentence as its own tagged row. No NOT RECORDED marker is lost, no zero is invented, and no column draws a RECORDED chip over a missing value. In every column that is drawn, the provenance is non-null and correct (probe check).
- **`row.Text` no longer drawn for measure rows.** Nothing is lost. For every one of the 206 measure rows, the old `row.Text` rebuilds exactly from `Label`, `SelectedValue`, `LatestValue`, `Difference` and the species prefix (probe check). Units sit inside the value strings. Only the `→` and parentheses are gone.
- **Element names.** The measure-name Label keeps the old name `annual-year-comparison-<Key>`; only its text is now the bare measure name. I searched for consumers of measure-row names or text:
  - repository at `339517ed`: no hit outside this file and the #416 handoff;
  - #417 qualifier `D073Verification.cs`: it looks up only the card, `-species`, `-latest-year`, `-caveat` and the gap rows (`cannot-compare`, `missing-*`), which are all unchanged;
  - its species-absence check (`:388`) reads `card.Q<Foldout>()` and a `not recorded` Label, both unchanged;
  - nothing reads `annual-year-comparison-stand-*` or `-species-*-<measure>` text.

  The probe found no duplicate element names within any card. Keys are unique because the helper returns CannotCompare on duplicate species identity.
- **Muted styling.** Measure rows are never muted (probe). Captions use `muted`; values use the row's `body` class.
- **Layout at 1280×720 (not verified).** Each column is `flex-grow 1`, `flex-basis 0`, `min-width 180` inside a `row-wrap` box, so three columns need about 540 px plus margins. That fits easily in the 96%-wide compact modal, and narrower widths wrap rather than overflow. Each measure now takes about four short lines instead of two, so an expanded species section roughly doubles in height inside the existing ScrollView (N-3).
- **Existing source audits that read this file**, each run on both baseline `77a56a44` and repair `339517ed`:

  | Audit | Baseline | Repair |
  |---|---|---|
  | S1C history query | PASS, 55 checks | PASS, 55 checks |
  | S1C T-03 drill-down | PASS, 72 checks | PASS, 72 checks |
  | S1C T-06 guidance | PASS, 53 checks | PASS, 53 checks |
  | S1C T-04 provenance view audit | **FAIL**, 1 of 56 checks | **FAIL**, 1 of 56 checks |

  The T-04 failure on the baseline already lists 3 untagged-line offenders. The repair adds a 4th: the caption line `:264` (m-2).

## C. Preservation

The source diff is **additive only: +42 / −0**. Every pre-existing line is byte-identical. Specifically:

- `BuildYearComparison` is still called once (`:175`) on the change-key-guarded `Refresh` path (`:115-116`), with one `ScenarioOneManagementFollowonReadout.Build` (`:187`).
- `AcknowledgeAnnualReview` appears exactly once in the file: the original footer callback, guarded (`:84`).
- Unchanged: footer predicate (`:133`), ScrollView creation and ownership (`:78-80`, card added to `scroll` at `:193`), species foldout name and key (`:196`, `:205-207`), data-source toggle (`:70-73`, `:132`), year buttons and title (`:143-147`), `ShowLatest` / `TryShowYear` (`:91-104`), and the latest hint row.
- Nothing is added beyond captions copied from existing mapper header fields and existing mapper values: no new rows, money, causal text, tree identity, manager/history read or state write.

## D. Scope

- `git diff --stat 77a56a44..55863f1b`: `AnnualReviewView.cs` +42, plus the #422 handoff. Nothing else.
- `git diff --stat 77a56a44 339517ed`: one file, +42.
- Tree and blob ids are **identical** at both commits for:

  | Path | Id |
  |---|---|
  | Mapper `ScenarioOneYearComparisonText.cs` | `9ca159a0` |
  | Helper `ScenarioOneManagementFollowonReadout.cs` (the #414 blob) | `2c7bdb3c` |
  | `UiKit.cs` | `e7b3c48f` |
  | `ScenarioOneUi.uss` | `b5d01630` |
  | `AnnualReviewView.cs.meta` | `cca264f7` |
  | `Tools/` | `cb63a41b` |
  | `Tools/Verification/YearComparison` | `d9e22281` |
  | `ProjectSettings` | `aa08457b` |
  | `Packages` | `b45a8758` |

- No change to UiRoot, any test, any `.meta`, or save/hash code. `git diff --check` exits 0.

## E. Ancestry (verified with git after `fetch origin`)

- `origin/integration/scenario-one-personal-playtest-candidate-20261111-v1` = `b62446ca`. It is an ancestor of `77a56a44`, with `rev-list --left-right --count` = **0 behind / 20 ahead**.
- `77a56a44` = `origin/feature/scenario-one-d073-annual-review-v1-20261112`.
- First-parent chain: `77a56a44 → 339517ed → 55863f1b`, linear, with **0 merge commits** in `b62446ca..55863f1b`. The tip is 0 behind / 22 ahead of the #382 source.

## F. Offline gates re-run by me on exact `339517ed`

All runs used a `git archive` extract outside every worktree, with `UseSharedCompilation=false`, `MSBUILDDISABLENODEREUSE=1` and `DOTNET_CLI_USE_MSBUILD_SERVER=0`. That means a non-shared `csc`, the `bincore/csc` invocation recorded in the log. dotnet SDK 10.0.114.

| Gate | Result |
|---|---|
| Production syntax/type binding: `ScenarioOneNonUi/compile_runtime_sources.py`, the #416/#422 method against `/home/jer/ccf-verify-player/CCFVerify_Data/Managed` | **PASS:** wrapper `sources=192 refs=116 exit=0 errors=0`. I rebuilt the generated project myself at `-v n --no-incremental`: `Build succeeded`, **0 Warning(s), 0 Error(s)**, 0 `error CS` lines, exit 0 (`gates/typecheck.full.log`, sha256 `bcf84d9e…c6c8`). `AnnualReviewView.cs`, the mapper and `UiKit.cs` are all in the compile list. |
| Typecheck negative control | **Detects errors:** an `AnnualReviewView` copy with `tableHeader.SelectedValueMUTANT` gives exit 1 and `error CS1061` at the mutated line (239). The gate really compiles this file. |
| Helper fixture `ManagementFollowonReadout/run_offline.py` | **PASS 53** focused assertions, `dotnet run` exit 0 |
| Mapper fixture `YearComparison/run_offline.py` | **PASS 132** assertions, 43 cards. Build exit 0 with 0 warnings / 0 errors (`TreatWarningsAsErrors=true`); fixture run exit 0. |
| Old fixtures unedited | **Confirmed:** the `Tools/` tree is identical at `77a56a44` and `339517ed` (`cb63a41b`) |
| Reviewer probe (not a repository file) | **PASS:** the unchanged fixture runs, then I read its 43 real mapper cards and transliterate the view's `:199-251` measure branch into an element plan. States covered: Compared 24, SelectedSnapshotMissing 5, LatestSnapshotMissing 2, CannotCompare 4, LatestYear 4, NoCompletedYear 2, InvalidInput 2. Result: 206 measure rows, 598 value columns, **5213 checks, 0 failures**. Checked: header before measures, foldout before species rows, provenance per column, non-empty captions, unique names, `row.Text` rebuildable, no muted measures. `gates/reviewer-probe/ReviewerProbe.cs`, sha256 `1232db80…908d`. |
| Probe negative control | Mapper mutant with header after stand rows: 5 probe failures of the "measure before TableHeader (NullReferenceException)" kind, and the existing fixture fails too (exit 1) |

**Honest limit:** the probe is a transliteration of the view's logic over real mapper output. It does not run UI Toolkit, USS, `Tag`, or the real `VisualElement` tree. No existing offline fixture builds `AnnualReviewView`'s rendered children.

## Findings

| ID | Severity | Location | Finding / failing scenario | Smallest fix inside `AnnualReviewView.cs` |
|---|---|---|---|---|
| m-1 | **Minor** (presentation) | `:227-228` with `:247-250`; mapper `:256` | The TableHeader is still drawn as one plain untagged line, "End of Year 3 · End of Year 9 · Difference", above columns that each repeat those captions. On an earlier year with both snapshots, the player sees an orphan header line that is not attached to any column, with no `[DERIVED]` beside "Difference" as the spec layout draws it. No value is mislabelled. | Optional: after `tableHeader = row;` add `continue;`, so the header supplies captions only. If kept, Tier-2 screenshots should confirm it reads as a heading and not as data. |
| m-2 | **Minor** (tooling) | `:264` | The S1C T-04 view audit (`run_t04_offline_check.py`) **already fails on the baseline** (3 untagged offenders). The new caption `UiKit.Add(entry, heading, "muted", …)` becomes a 4th, because it has a `"muted"` literal and is not on the allowlist. The pass/fail state does not change, but the audit's offender list grows. The caption is a column title directly above its own chip, not a history value. | None needed in the view. Do **not** disguise the literal to dodge the audit. A test-only allowlist entry belongs to a separate test owner (#427 or later). Record it in the parent result. |
| N-1 | Note (spec-sanctioned) | mapper `:151-152`; view `:247-250` | "…, 4 year(s) later." shows `YearsBetween`, an arithmetic B−A, under the line's RECORDED chip. The approved spec layout (line 67) draws exactly this as `[RECORDED]`, and #419 did not raise it. The text is mapper-owned and outside correction A. | None in this scope. Manager may note it for a later spec pass. |
| N-2 | Note (robustness) | `:239-244` | `tableHeader` is dereferenced without a null check. That is safe only while the mapper keeps header-before-measures, which the 132 fixture asserts. The existing `species` foldout has the same pre-existing pattern (`:216`). | Optional hardening: if `tableHeader == null`, fall back to the old `AddTagged(parent, row.Provenance.Value, row.Text, textClass)` path. |
| N-3 | Note (G3 check) | `:235-271` | Each measure now takes about four short lines (name, caption, chip row, value) instead of two. Expanded species roughly doubles in height inside the ScrollView. With DERIVED, "calculated from records" plus the chip may wrap inside a 180 px column. Not a defect at source level. | None. #427 Tier-2 should capture the stand and expanded-species sections at 1280×720 and 1920×1080, sources ON and OFF. |
| N-4 | Note | `:233-234` | The measure-name Label keeps the old element name but now holds only the measure name. No consumer depends on the old text (B). Future oracles should read `<P>-year-a-value` and the related column names, as the #422 handoff documents. | None |

## Not checked by me

- Unity import or compile, rendered UI, USS resolution, pixel fit at 720p or 1080p, native input, runtime save/hash/event non-mutation, old-save, Legacy40 or frozen Reference compatibility.
- The #427 oracle or its evidence; the #417 raw artefacts; the physical Ops board or Unity lock.
- Dashboard or Drive sync.
- I did not edit any source, test, `main`, the repair branch, #382, or any checkout. The only write is this file, on new branch `docs/d073-fr10-independent-g2-review-20261112-v1`.

**Return to #412:** APPROVED for G2 source conformance. #419 B-1 is closed at source level. Proceed to the #427 focused Tier-2 on exact `339517ed`, or on a candidate with an identical production tree. m-1 is optional and should be decided before qualification so that the qualified source is not changed afterwards. Release readiness remains the Overall Manager's call.
