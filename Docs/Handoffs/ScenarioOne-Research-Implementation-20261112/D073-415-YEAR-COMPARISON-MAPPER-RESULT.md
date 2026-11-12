# D-073 #415 — year-comparison text mapper (T-03) result

Child of [#412](https://github.com/jermur788/CCF/issues/412); worker claim [#415](https://github.com/jermur788/CCF/issues/415). Task T-03 of `specs/scenario-one-multiyear-decision-outcome-feedback-v2` only.

**Implemented and pushed on the assigned branch; all offline gates passed. Unity NOT RUN. Nothing is wired to a screen.** No integration, acceptance or Manager verdict is claimed.

## Worker and exact state

- Actual worker: Claude Fable 5.2 (`claude-fable-5-2`), Claude Code harness in T3 Code, account **Claude B**. Work done directly; no subagents.
- Sole writable worktree: `/media/jer/Files/CCF/Worktrees/CCF-D073-AnnualReview-V1`.
- Branch: `feature/scenario-one-d073-annual-review-v1-20261112`.
- Start HEAD: `abb5d653b21668ac7efe4312dddba145f7049abb` (worktree clean at start).
- Tested implementation commit: `ed000dafd5c288d6d6e70752c82f9e638f10dd57` (tree `450684ff146a821824c452073ff2e9c84934a177`, sole parent = start HEAD). Every gate below ran on this committed, clean tree.
- Final HEAD: the handoff-only child of `ed000daf` that adds this file. A committed report cannot contain its own commit hash; the exact final SHA is in the #415 result comment and in `publication.json` under the evidence root.
- Binding spec read from `origin/docs/manager-d073-approved-scope-20261112-v1` at `53dec8db19a9bdde20176b2f3abf508402c7f0af` (spec, plan, tasks, checklist). The spec files are not on this branch and were not copied onto it.
- Helper on this branch is unchanged: blob `2c7bdb3c6b6f13790a325ebabc187c497b335b52`.
- Evidence root (outside the repository): `/media/jer/Files/CCF/Verification/D073-YearComparisonMapper-415-20261112/`.

## Files added (nothing else changed)

| File | Git blob |
|---|---|
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneYearComparisonText.cs` | `9ca159a09a4a47b18bdce2dd77a0f973cd394281` |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneYearComparisonText.cs.meta` | `a66ede481db863aea5c384ff95c713f415c21e60` |
| `Tools/Verification/YearComparison/YearComparisonVerification.cs` | `e052825c718919e020e7979913beefc34f9f6fa7` |
| `Tools/Verification/YearComparison/OfflineProvenanceTypes.cs` | `461edb4bc32786f96b89076a7a479502f7cd8d85` |
| `Tools/Verification/YearComparison/run_offline.py` | `9d37048209963de50de4b367c82e1565aaa02f9d` |
| `Tools/Verification/YearComparison/run_mutations.py` | `ab9191f12e9783429fa60082ea25c7830d8bc75c` |
| `Tools/Verification/YearComparison/README.md` | `6bfd8a5de9cbd37aea2ac08f958f83b16b81e23a` |
| this handoff | resolved by `git rev-parse HEAD:<path>` |

`.meta`: two lines, same format as `ScenarioOneManagementFollowonReadout.cs.meta`; GUID `c00b53f99960439f8e1be41e85582399` (fresh UUID4; `git grep` found no match before creation and exactly one match under `Assets` after the commit).

Not touched: the helper, `AnnualReviewView.cs`, `WorkPlanView.cs`, `ScenarioOneUiRoot.cs`, every save/event/hash/objective source, the existing `Tools/Verification/ManagementFollowonReadout/` files, scenes, prefabs, ProjectSettings, packages, `main`, Primary, other worktrees, the Ops board and `active-tasks.md`.

## Public API (what the T-04 view worker calls)

```csharp
ScenarioOneYearComparisonText.Card card = ScenarioOneYearComparisonText.Describe(
    ScenarioOneManagementFollowonReadout.Build(manager.HistoryYears, year.Year),   // one Build result
    year.Year,                                                                     // the year on screen (not -1)
    ScenarioOneUiFacts.SpeciesName);                                               // Func<string, string>
```

Signature: `public static Card Describe(ScenarioOneManagementFollowonReadout.Result result, int viewedYear, Func<string, string> speciesName)`.

- `Card`: `Visible` (false → add nothing), `State` (`NoCompletedYear`, `InvalidInput`, `LatestYear`, `CannotCompare`, `SelectedSnapshotMissing`, `LatestSnapshotMissing`, `Compared`), `ViewedYear`, `LatestYear`, and `Rows` in display order.
- `Row`: `Kind` (`Heading`, `Line`, `TableHeader`, `StandMeasure`, `SpeciesHeading`, `SpeciesMeasure`, `SpeciesNotRecorded`, `Caveat`, `GuidancePointer`), `Key` (stable name such as `stand-canopy`, for element names and checks; never shown), `Provenance` (`HistoryProvenance?`; null = no label), `Text` (the whole row as plain text, without the label), `Muted`, and for table/species rows `Label`, `SelectedValue`, `LatestValue`, `Difference`, `ValueProvenance`, `DifferenceProvenance`, `SpeciesId`, `SpeciesName`, `FirstRowOfSpecies`, `MissingYear`.
- The mapper does not call `Build`. It is a pure static class: no `UnityEngine` reference, no state, no I/O, no mutation of its input.
- `ScenarioOneUiFacts.SpeciesName` cannot be called from pure code (it uses `Object.FindFirstObjectByType<ForestTreeSpawner>()`), so names come through the delegate, as the plan says. With `null` the species id is shown.

Rendering notes for T-04 (not done here):

- The spec writes `[DERIVED]`, `[RECORDED]`, `[NOT RECORDED]` in front of lines. Those are the data-source chips. They are **not** part of `Row.Text`; pass `Row.Provenance.Value` to the existing `AddTagged`/`Tag`, which already obeys the **Show data sources** toggle (`.hide-data-sources .prov`). Rows with `Provenance == null` go through `UiKit.Add`.
- A row holding two recorded values and their difference carries `Provenance = Derived`, the convention `AnnualReviewView` already uses for `Standing trees {before} → {now}`. For a real three-column table use `ValueProvenance` (Recorded) and `DifferenceProvenance` (Derived) instead.
- `SpeciesHeading` ("By species") is the foldout title; every following `SpeciesMeasure`/`SpeciesNotRecorded` row belongs inside it, closed by default. Rows with `FirstRowOfSpecies == false` sit indented under the species name.

## Requirement coverage

Fixture = `Tools/Verification/YearComparison/YearComparisonVerification.cs`, 132 assertions in 13 groups. Assertion labels are quoted.

| Requirement | Offline fixture coverage |
|---|---|
| FR-1 placement | needs Unity/UI (T-04/T-05): not covered here |
| FR-2 navigation, acknowledgement | needs Unity/UI: not covered here. The mapper has no access to the UI or manager |
| FR-3 single data source | by construction: the only inputs are one `Result`, an `int` and the name delegate. Call-site part is T-04 |
| FR-4 selected-year outcomes | group `yearfilter`: "only viewed-year rows are counted", "Year 5 counts only Year 5 rows", "created and approved orders never appear on the card"; group `full`: exact card text; mutation `year-filter` |
| FR-5 fail closed | group `ambiguous`: "ambiguous rows are excluded from every count", "ambiguous exact sentence", "unresolved exact sentence", "a row that is ambiguous and unresolved is reported in the ambiguous group", "…is not counted again as unresolved", "one row with both conditions appears once, in the ambiguous group only"; group `linkage`: "MissingOrder carries the linkage warning", "InconsistentOrder carries the linkage warning", "the linkage warning counts succeeded and failed event-only outcomes", "a cancellation alone raises no warning"; mutations `ambiguous-exclusion`, `ambiguous-before-unresolved`, `event-only-warning` |
| FR-6 exact years only | group `missing`: "selected snapshot missing shows no stand number from another year", "a snapshot dated for another year is not shown as Year 3", "latest snapshot missing shows no latest value and no difference"; group `nocard`: "a result built for another year shows no card" |
| FR-7 differences and units | group `full` (columns, negative differences, m²/ha, m³); group `units`: "canopy fractions are shown as percent", "canopy change is shown in percentage points with its sign", rising/falling/unchanged signs, "elapsed years are shown", "the helper's stored canopy values stay fractions"; mutation `canopy-points` |
| FR-8 species rows | group `species`: "species missing on the selected side names Year A", "species missing on the latest side names Year B", "a one-sided species shows no value, no zero and no difference", "four separate measure rows per species", "four species measures exact text", "species measures are never summed" |
| FR-9 state wording and precedence | groups `nocard`, `latest` ("latest year line exact wording"), `missing` (selected, latest, both), `precedence` ("CannotCompare beats a missing snapshot", "CannotCompare: work lines then the exact line"), `nowork` ("no work rows exact line", "no work rows is supplementary…"); mutation `cannot-compare-precedence` |
| FR-10 provenance words | group `wording`: "every row carries the right data-source label" over all 43 cards; per-row checks in `full`, `nowork`, `ambiguous`, `linkage`, `species`. The **Show data sources** toggle itself needs Unity/UI |
| FR-11 caveat, no causal wording | group `wording`: "fixed caveat and guidance pointer sentences", "no causal wording" (caused / because / due to / led to / result of; the fixed caveat is the one exempt row), "caveat only with a comparison…" |
| FR-12 no money | group `wording`: "no currency or money wording" on cards built from rows with `GroupedHarvestMoney == true`; "no out-of-scope wording or undisplayed helper field" (coverage unit id, candidate years, rack, prescription, survival, hectare, score, interval, "thinned") |
| FR-13 same answer after either advance | offline part only — group `stable`: "a repeat call gives identical output", "the same history gives the same card", "the order of supplied years does not change the card". One-year versus long advance on a real save needs Unity (T-05) |
| FR-14 read-only | group `stable`: "the mapper does not change its input", "the input is still unchanged after many calls", "other states leave their input unchanged". The change-key-guard call site is T-04 |
| AC-1 | group `nocard` (card part) |
| AC-2 | group `latest` (card part); footer behaviour needs Unity |
| AC-3 | group `full`: "full comparison card exact text and order" |
| AC-4 | groups `missing`, `precedence`, `nowork` |
| AC-5 | groups `ambiguous`, `linkage` |
| AC-6 | groups `full`, `species` (Sitka spruce, sessile oak, beech) |
| AC-7, AC-8, AC-9, AC-10 | need Unity/UI: not covered here |
| AC-11 | group `wording` |
| AC-12 | offline part — `full`: "clean history: card counts equal event counts made the Work-column way"; `ambiguous`: "malformed history: the mismatch with event counts is explained by the warning". Comparison with the real Work column on real saves is T-05 |

## Commands and results

All run from the worktree root on `ed000daf`, with `TMPDIR` under the evidence root. Each has `commands/<label>.{json,stdout.log,stderr.log}` holding argv, cwd and the real exit.

| # | Command | Exit | Result |
|---|---|---:|---|
| a | `python3 -I -B Tools/Verification/YearComparison/run_offline.py --out <evidence>/a-offline` | 0 | **PASS** — `dotnet build exit=0` (0 warnings, 0 errors, warnings treated as errors), `fixture run exit=0`, `YearComparison: PASS 132 assertions (group: all, cards checked: 43).` |
| a′ | `python3 -I -B Tools/Verification/YearComparison/run_offline.py` (exact packet command) | 0 | **PASS** — same three lines |
| m | `python3 -I -B Tools/Verification/YearComparison/run_mutations.py --out <evidence>/mutations` | 0 | **PASS** — 6 of 6 mutants compiled (build exit 0) and were killed (fixture exit 1) on the expected assertion |
| b | `python3 -I -B Tools/Verification/ManagementFollowonReadout/run_offline.py` | 0 | **PASS** — `ManagementFollowonReadout: PASS 53 focused assertions`; underlying explicit `dotnet build` exit 0 (0 warnings, 0 errors) and `dotnet run` exit 0, in `b-helper-dotnet/` |
| c | `python3 -I -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src <worktree> --out <evidence>/c-typecheck-production` | 0 | **PASS** — `sources=192 refs=116 exit=0 errors=0`; underlying `dotnet build -nologo -v q -c Release -clp:NoSummary;ErrorsOnly` exit **0**, in `c-typecheck-dotnet/dotnet-build.json` |
| c2 | same script with `--extra <evidence>/call-site-probe/IntendedCallSiteProbe.cs` | 0 | **PASS** — `sources=193 refs=116 exit=0 errors=0`; underlying build exit 0. The intended T-04 call (above) binds to the real `ScenarioOneManager.HistoryYears`, `ScenarioOneUiFacts.SpeciesName` and `HistoryProvenanceText.Label` |
| c3 | same script with `--extra …/NegativeControlProbe.cs` (a deliberately wrong call) | 0 | **Expected failure seen** — `exit=1 errors=1` (CS7036); underlying build exit **1**. Shows the gate reports errors; the script's own exit stays 0 |
| d | `git diff --name-status abb5d653 HEAD`; `git status --porcelain` | 0 | **PASS** on `ed000daf` — exactly the seven `A` lines in the table above; status empty. After the handoff-only commit the same commands were recorded again (`commands/final-scope.*`, `commands/final-status.*`): those seven plus this file, status empty |
| — | Unity import / compile / play, Player build | — | **NOT RUN** (not allowed in this task) |
| — | Fixture compiled against real production history types instead of the doubles | — | **NOT RUN** |

Mutations (each a copy of the mapper outside the repository; diff, build log and run log kept under `mutations/<name>/`):

| Mutation | Rule removed | Killed by |
|---|---|---|
| `year-filter` | exact-year filter on `RecordedWork` | "only viewed-year rows are counted" |
| `ambiguous-exclusion` | `AmbiguousIdentity` rows excluded from counts | "ambiguous rows are excluded from every count" |
| `ambiguous-before-unresolved` | ambiguous checked before unresolved | "a row that is ambiguous and unresolved is reported in the ambiguous group" |
| `cannot-compare-precedence` | CannotCompare ahead of missing snapshots | "CannotCompare beats a missing snapshot" |
| `canopy-points` | `MeanCanopyChange × 100` | "canopy change is shown in percentage points with its sign" |
| `event-only-warning` | MissingOrder/InconsistentOrder warning | "expected exactly one row 'work-event-only', found 0" |

Gate c is a .NET type check of the real runtime sources against the managed assemblies of an already-built Player at `/home/jer/ccf-verify-player/CCFVerify_Data/Managed` (the script's default, the same set #414 used). **It is not a Unity compile.** The offline fixture (a, m) uses test doubles: `OfflineContractTypes.cs` (existing, referenced unchanged) and the new `OfflineProvenanceTypes.cs`, a copy of the `HistoryProvenance` enum, needed because the production file that declares it also holds text classes that need the whole history model. SDK: .NET 10.0.114, net8.0 runtime present; fixture targets net8.0 / C# 9, type check targets netstandard2.1 / C# 9.

To record real subprocess exits without editing existing tooling, gates b and c ran with an external `audit-bin/dotnet` on `PATH` that forwards to `/usr/bin/dotnet` unchanged and keeps argv, cwd, exit and output (same method as #414). The new launcher needs no wrapper: it runs `dotnet build` and the fixture as two processes and prints both exits.

## The card, as produced

For the clean fixture history (Year 3 viewed, latest Year 9) the fixture asserts exactly this, tags shown as the chips would be:

```text
YEAR 3 COMPARED WITH LATEST YEAR 9
[DERIVED] Recorded outcomes in Year 3: 2 tree(s) felled · 1 planted · 1 crop tree(s) pruned · 1 windthrow stem(s) salvaged · 1 young-tree removal job(s)
[DERIVED] 1 task(s) failed · 1 order(s) cancelled
[RECORDED] Stand recorded at the end of Year 3 and at the end of Year 9, 6 year(s) later.
End of Year 3 · End of Year 9 · Difference
[DERIVED] Standing trees: 120 → 112 (-8)
[DERIVED] Basal area: 14.2 m²/ha → 16.7 m²/ha (+2.5 m²/ha)
[DERIVED] Mean canopy cover: 62 % → 66 % (+4.0 points)
[DERIVED] Regeneration map squares (modelled): 7 → 12 (+5)
[DERIVED] Fallen deadwood: 3.50 m³ → 2.25 m³ (-1.25 m³)
By species
[DERIVED] Sessile oak: standing trees 30 → 30 (0)
[DERIVED] natural regeneration map squares (modelled) 2 → 2 (0)
[DERIVED] planted regeneration map squares (modelled) 0 → 0 (0)
[DERIVED] planted young trees (counted) 5 → 5 (0)
[DERIVED] Sitka spruce: standing trees 90 → 82 (-8)
[DERIVED] natural regeneration map squares (modelled) 4 → 6 (+2)
[DERIVED] planted regeneration map squares (modelled) 1 → 3 (+2)
[DERIVED] planted young trees (counted) 10 → 8 (-2)
These are recorded stand totals for two years. They do not show that Year 3's work caused the difference.
For what to inspect before deciding again, see the prior-treatment guidance in this review and walk the stand.
```

FR-5 and FR-9 sentences are reproduced character for character. The numbers are fixture values, not forest data.

## Design decisions and spec readings — for Manager review

No STOP condition was hit: every rule is expressible from the `Result` as it stands, with no helper change and no saved state. The points below are where the spec left a choice; each is the fail-closed reading and is one small edit to reverse.

1. **Wording I had to write (the spec gives none).**
   - FR-5 linkage warning: `{n} of the outcome(s) counted above are recorded event outcomes only; their original order or target linkage cannot be verified.` (NOT RECORDED).
   - Year A observation when only Year B's snapshot is missing: `Stand recorded at the end of Year {A}.` (the sketch sentence, cut at the first year) followed by the five measures with one column.
   - Species missing on one side: the sketch sentence exactly, then `Missing for Year {X}; recorded for Year {Y}.` FR-8 asks for the year to be identified; the sketch sentence alone does not. If the result cannot say which side, only the sketch sentence is shown.
   - `· {n} other task(s)` is appended to the outcome line only for a task type outside the five known ones. Unreachable with today's `ScenarioWorkType`.
2. **Both snapshots missing.** The card shows the Year A line and then the Year B line. The precedence sentence ("selected missing, then latest missing") can also be read as "first match only"; I named both because the spec's goal is "every missing … record named as such" and FR-9's Year B row says "Year A observation *if recorded*". `State` is `SelectedSnapshotMissing`.
3. **Latest-year line with duplicate latest-year records.** When the viewed year is the latest completed year the single muted line is shown even if the helper reports `CannotCompare` (two records for that year), following "for latest-year selection the one-line latest-year state takes precedence". No number is shown either way.
4. **Caveat and guidance pointer only in the `Compared` state.** "These are recorded stand totals for two years" would be untrue on a card that compares nothing, and FR-9's rows for the gap states end at the gap line. The pointer is its own row kind because the prior-treatment guidance card it points to is only built for years with a recorded treatment; the mapper cannot know that from the `Result`, so T-04 decides whether to show it.
5. **"No recorded work-resolution or cancellation events" is labelled DERIVED**, like the existing "No work was resolved this year." It is a count of zero made from the events; the spec's table gives it no `[NOT RECORDED]` tag, unlike the three gap lines.
6. **The latest-year hint, heading, table header, species heading, caveat and pointer carry no label.** They state no stand or work value.
7. **Number formats.** Canopy `0` % and `+0.0;-0.0;0.0` points, as `AnnualReviewView` does today; deadwood `0.00` m³ as the review headline does; basal area `0.0` m²/ha as `ManagementHistoryText.Completion` does; whole numbers `+0;-0;0`. The minus is the plain hyphen the existing canopy line uses (money elsewhere uses `−`). All formatting is culture-invariant; the existing canopy change line is not, so on a comma-decimal system the two could differ until that line is changed (not in scope here).
8. **All five outcome counts are always shown, zeros included**, to match the sketch exactly. The two count lines are replaced by the single "no events" line only when the year has no row at all; a year whose rows are all ambiguous shows zeros plus the warning.
9. **Cancelled rows never raise the linkage warning.** The helper gives every cancellation `MissingOrder` because a cancelled order is deleted; FR-5 names succeeded/failed outcomes only.
10. **`InvalidInput`** (null result, or `result.SelectedYear != viewedYear`) returns a hidden, empty card rather than throwing, so a wiring mistake can never show one year's records under another year's name. T-04 may want to log it.
11. **Species names start with a capital** (`sessile oak` → `Sessile oak`) because they begin a line. `Row.SpeciesId` keeps the id.
12. **Year A's species are not listed when Year B's snapshot is missing.** FR-8 defines species rows for the comparison only.
13. **Felling is "felled"**, per Manager decision 3; "thinned" appears nowhere (asserted).

Ecology: no ecological rule, coefficient or claim is added. Regeneration figures stay labelled "modelled" map squares; planted young trees are labelled "counted".

## Not verified

- Unity import of the new script and its GUID, Unity/Assembly-CSharp compilation, and any Editor or runtime behaviour. Gate c is not a Unity compile.
- Everything visual: placement, the chips and the **Show data sources** toggle, the foldout, fit and scroll at 1280×720 and 1920×1080, footer and year buttons (FR-1, FR-2, AC-2 footer part, AC-9).
- Real saves: a current save, a genuine older save, Legacy40, frozen Reference Future v1 (AC-10); event/hash equality with and without the card (AC-7); one-year versus long advance (AC-8, FR-13); comparison with the real Work column (AC-12). The fixture's histories are synthetic.
- Real display names from `ForestTreeSpawner`; the fixture uses the fallback names in `ScenarioOneUiFacts.SpeciesName`.
- The fixture against real production history types. Only the mapper, the helper and the intended call were type-checked against them.
- No independent review has happened. The wording in decision 1 has been seen by no player and no reviewer.

## Next steps (not started)

T-04 (wire into `AnnualReviewView`, inside the change-key guard) and T-05 (Unity qualification by a different worker) remain separate tasks. Expected first Unity check: import this exact head in Unity 6000.6.0f1, confirm the script imports with GUID `c00b53f9…` and that Assembly-CSharp compiles.
