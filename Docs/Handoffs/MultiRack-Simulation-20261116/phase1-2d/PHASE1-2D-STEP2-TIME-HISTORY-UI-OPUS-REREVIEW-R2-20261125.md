# CCF Multi 2D — Phase 1 Step 2 (Time History UI) Repair Delta: Independent Re-Review R2

**Verdict: NEEDS CHANGES**

> **NOT MERGED. NOT INTEGRATED. NOT OWNER-ACCEPTED.** This is an independent review record only. No merge, PR, tag, force-push, amend, `main` write or feature-branch write was made. No requirements checkbox was ticked by this review.

| Item | Value |
|---|---|
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`), independent non-author reviewer, T3 Code / Claude Code harness |
| Implementer | Neon (`antigravity_2` / `gemini-4-neon`), a different provider and thread |
| Separation | No implementation role. No source, test, spec or implementation-handoff edits. No subagents. All probes and mutations ran outside the review worktree (scratch under `/home/jer/ccf-p1ui-r2-tmp`, since removed). The only repository write is this file on the R2 review branch |
| Freshness | Same reviewer identity as the first review (`ea6961ae`), new session. This is a delta re-review: the prior review document, the repair diff and the code were the inputs. The implementer's reasoning was not an input. I re-derived every measured number below |
| Review worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-ReReview-R2-20261125` |
| Review branch | `review/multi-2d-phase1-time-history-ui-20261125-v1-r2` |
| Repaired feature HEAD (reviewed) | `adea7af220259a1808346cd5381babed3e9052fa` (`feat/multi-2d-phase1-time-history-ui-20261125-v1`) |
| Previously reviewed HEAD | `513355c4bcef881413d517aa62feed4676234319` |
| Base | `5fe2fabe9073d7b72711f4ab48553c785351d7bc` |
| Prior review | `ea6961aebf0d0a49a2265613140a7622f3522221` (NEEDS CHANGES, F-1..F-6) |
| Binding owner decision | G5: P1-UI-04 horizon = **Year 30**, projection-only reductions, keep field names and UI behaviour, never return over-budget bytes silently |
| Date | 2026-11-25 |

## 1. Summary

The repair closes the payload blocker within the owner's horizon:

- **Budget.** `GET api/multi/view` stays at or under 524,288 B, and `ecology` at or under 153,600 B, at **every year 0–30 on both experiments**. This holds in my rerun of the author's sweep and in my own probe with a different workload. Every tier checks the cap itself before returning.
- **Never silent.** Past the horizon the server flags `payloadBudgetExceeded` and logs every over-budget response. I observed this at MA-1-MIXED Years 36–50.
- **F-3, F-6 and the flag-honesty part of F-4 are fixed.** Each new test I mutated fails when its fix is reverted (§6).

Three new defects block approval. Two are honesty regressions that the repair itself introduced, and both reproduce in real Edge:

- **R2-1 (SHOULD).** The F-2 Results-pane fix shows a false statement in the most common first turn. Execute any work at Year 0 and the Results pane says "Compact view omits annual history rows to stay within payload budget". In fact nothing was omitted and no year has been advanced (`historyTruncated=false`).
- **R2-2 (SHOULD).** Under the new projection, the tree inspector's "Life state" for dead trees changes from Core's `BiologicallyDead` to a browser-invented `DeadFallen`. This starts at MA-1-MIXED Year 17 and MA-1 Year 28/29, inside the horizon. The "Pruning lifts" row also disappears. Spec §5 promises that inspector behaviour is preserved.
- **R2-3 (SHOULD, spec accuracy).** The spec quartet's tier list does not match the code. The code also keeps an undocumented emergency tier that drops **all** history without flagging it, which breaks the spec's own retention rule. It is latent: neither workload reached it.

All 7 gates pass on `adea7af2`: G1 build 0 warnings / 0 errors; G2 Core **277/277**; G3 Save **50/50**; G4 UI build 0/0; G5 smoke **362/362**; G6 save Edge **63/63**; G7 Step 2 Edge **64/64**.

## 2. Start-of-review checks (exact results)

| # | Check | Result |
|---|---|---|
| 1 | `git rev-parse HEAD`; `git status --short` | `adea7af220259a1808346cd5381babed3e9052fa`; status **empty** at start, after every gate, and before writing this file |
| 2 | `git diff --name-only 513355c4..adea7af2` | **Exactly the 9 claimed files**: the Neon handoff, `src/Ui/PlayerMultiApi.cs`, `tests/phase1-time-history-ui/drive-step2.mjs`, `tests/phase1-time-history-ui/run-step2-edge.sh`, `tests/ui-smoke-multiarea.sh`, and the four `specs/multi-2d-phase1-time-history-ui-v1/` files. Stat: 9 files, +1134/−409. `git diff --summary`: no mode changes (`run-step2-edge.sh` and `ui-smoke-multiarea.sh` remain 100755) |
| 3 | `git diff --name-only 5fe2fabe..HEAD` against `src/Core/**` (including Phase1), `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/coordination/**` | **Zero** paths (the forbidden-path grep returned rc=1). The base..HEAD list is the same 9 files. `wwwroot` is untouched |
| 4 | `git ls-remote origin` | Feature `adea7af220259a1808346cd5381babed3e9052fa` = reviewed HEAD. `main` = `cc2757a730ad9451e3d6e3680dffd123171a5df5` (unchanged). `.p0ref/SitkaLab.Core.dll` sha256 = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (exact, read-only, not rebuilt). Listeners `127.0.0.1:33767` (pid 1750123), `:43649` (pid 1806361) and `:46593` (pid 839989) were all LISTEN |
| 5 | Ephemeral lane processes | At start: 13 `sitkap1-ui.dll` processes, all from other lanes (cwd outside this lane). None of them, and no headless `msedge`, belonged to this lane. The only headless Edge running was another task's (`opencode\ccf-v2-task` profile), which I left alone. End state: §12 |

## 3. Gates rerun by the reviewer (on `adea7af2`)

Environment: WSL2 Ubuntu, `PATH="$HOME/.dotnet:$PATH"` (dotnet 8.0.134), `TMPDIR=/home/jer/ccf-p1ui-r2-tmp` (outside the repo), node v26.13.1, Windows Microsoft Edge `--headless=new`. Commands were exactly as briefed, run from `Tools/MultiRackSitkaP1Lab`.

| Gate | Command | Exact result |
|---|---|---|
| G1 | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | Build succeeded, **0 Warning(s), 0 Error(s)** (11 s) |
| G2 | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **277/277 passed, 0 failed**. All 8 test-printed anchors are present (`7e3a2a86`, `a692a6d5`, `814e8165`, `8a926776`, `c4b670ed`, `c67edc6b`, `776a68df`, `93e9abda`); with the `.p0ref` pin that makes 9/9 |
| G3 | `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | **50/50 passed, 0 failed**; no warning or error lines |
| G4 | `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | Build succeeded, **0 Warning(s), 0 Error(s)** |
| G5 | `TMPDIR="$TMPDIR" tests/ui-smoke-multiarea.sh …/r2-smoke` | **OVERALL: PASS (362 passed, 0 failed)**; Z03/Z04 lab tree and git status unchanged |
| G6 | `tests/phase1-save-ui/run-edge-e2e.sh …/r2-edge-save` | **OVERALL: PASS (63 passed, 0 failed)** |
| G7 | `tests/phase1-time-history-ui/run-step2-edge.sh …/r2-edge-step2` | **OVERALL: PASS (64 passed, 0 failed)**, including both Year 0..30 sweeps and all six Z01/Z02 SIGINT/empty-stderr checks |

The counts match the handoff exactly. G7's printed sweep values are byte-identical to the handoff tables (for example MA-1 Year 28 = 524,279 B and MA-1-MIXED Year 27 = 524,032 B). Gate logs are at `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125/r2-gates/`.

## 4. Delta scope reviewed

I read the whole `PlayerMultiApi.cs` diff (server and page script), both harness diffs, the smoke diff, the four spec diffs and the handoff. I also read the Core contracts the projection replaces: `MultiAreaContracts.cs`, `MultiAreaUiReadout.cs`, `AnnualEngine.cs`, `PlayerMapScene.cs`, `Stage4Models.cs` (`LifeStates.Of`), `MultiAreaHistoryQuery.cs` (`ReadIndex`, `ReadYear`) and the Stage 4 deadwood creation in `MultiAreaContinuation.cs:1049-1060`.

## 5. Finding-by-finding verification (F-1..F-6)

### F-1 (BLOCKER): view budget. **CLOSED within the Year 30 horizon**, with R2-2 as a side effect

**Code** (`PlayerMultiApi.cs:418-628`):

- Tier 0 serializes the full payload and returns it only if raw ≤ 524,288 and `ecology` ≤ 153,600 (`:565-576`).
- The projection tiers (`TryFitTier`, `:578-601`) return only candidates whose serialized length passed `<= MaxViewPayloadBytes`. They return nothing when `ecoBytes > MaxEcologyPayloadBytes`.
- The emergency tier (`:603-617`) also checks the cap.
- The final branch (`:619-627`) sets `payloadBudgetExceeded: true` both in the bytes and in the record used by the composite endpoints, and writes `PAYLOAD BUDGET EXCEEDED GET api/multi/view (year=…): raw=… B (limit 524288), ecology=… B (limit 153600)` to stderr.

So no path returns over-budget bytes without the flag and the log line. The near-cap values are not luck: every returned candidate passed the cap check first.

**Projection-only.** Every reduction builds new DTOs or `with` copies (`ProjectViewForBudget :633`, `ProjectScenesForBudget :665`, `ProjectStage4ForBudget`, `ProjectEcologyForView :418`). Core records are immutable, and the root is never touched. Empirical checks on both experiments:

- at Year 30 the `history/index` keyset walk returned every year 30..0 exactly once;
- `history/year 0` was fully readable;
- repeated `GET api/multi/view` was byte-identical at every year;
- G7's Save → SIGINT → restart → Load matched Preview, History and Structure exactly.

**Measured** (§7 table):

- **Author workload (G7 rerun).** MA-1 max 524,279 B (Year 28, Tier 0, a 9 B margin); MA-1-MIXED max 524,032 B (Year 27, 256 B margin). Max `ecology` 43,320 B and 49,685 B.
- **Reviewer workload.** I wrote an independent Python `http.client` probe, `r2_probe.py`. It differs from the author's driver:
  - it fells the middle living MA-A tree;
  - on MA-1-MIXED it also queues Prune, ClearUndergrowth, a beech Plant, Shelter and SpotClear;
  - work runs at Years 0, 10, 20, 30 and 40;
  - each year is advanced by `POST api/multi/year/skip` to year+1, so the Step 2 skip path runs every year.

  Results: MA-1 max 518,728 B (Year 27); MA-1-MIXED max 523,992 B (Year 28, 296 B margin). Max `ecology` 43,438 B (MA-1, fresh Year 0 before work) and 50,679 B (MIXED Year 19). **Every year 0..30 is within both budgets on both experiments**, with `payloadBudgetExceeded=false` and empty stderr through Year 30.
- **Beyond the horizon** (informational; not a budget claim):
  - MA-1 stayed within budget to Year 50 (514,802 B).
  - MA-1-MIXED was flagged `payloadBudgetExceeded=true` from Year 36 to Year 50 (533,782 → 751,084 B). It kept 1 event and 1 annual record, and wrote 69 `PAYLOAD BUDGET EXCEEDED` stderr lines across GET and composite responses. The flag was set **if and only if** raw or ecology exceeded its cap.

**Fields the map, table and UI need.** I diffed the JSON key sets of Tier 0 against the projected tiers. The projection drops:

- `death` (null) on living view trees;
- `death.treeId`;
- the scene fields `heightM`, `dbhCm` and `deathCause`, plus null `deathYear` and false `isCrop`;
- null optional `stage4` tree fields;
- **every dead `stage4` tree with `pruningLifts == 0`**;
- dead unpruned entries in `ecology.pruneStatusByTreeId`. This one is always on, including at Tier 0.

The page script reads none of the dropped scene fields. Stump and dead-stem positions, `status`, `deathYear`, radii and fallen deadwood are retained. The exception is the inspector (R2-2).

### F-2 (MAJOR): zero-history fallback and false Results text. **PARTLY CLOSED**

- **Retention.** Fixed within the horizon. The floor is `minEvents/minAnnual = 1 whenever present` (`:464-465`), and `pruneLog` is trimmed (caps full → 10 → 1 → 0) before the tail goes below that floor.
  - In both sweeps every year 1..30 kept at least 1 event and at least 1 annual record. The smallest MIXED tail was 4 events and 6 annual records at Year 30.
  - Beyond the horizon my MIXED run showed the order: `pruneLog` stayed 4/4 while history was trimmed at Years 31–35, then went to 0/4 with history at the 1/1 floor at Years 36+.
- **Results text for the truncated case.** Fixed: the S2j DOM witness passes, and mutant M4 is killed (§6).
- **New false text (R2-1).** The repaired branch fires whenever `annualHistory` is empty and `totalRecordedYears > 0`. That includes the ordinary Year 0 after any executed work, where no history is truncated.
- **Residual zero-history path (R2-3).** The code still contains a branch that returns zero events and zero annual records without flagging it.

### F-3 (MINOR): out-of-range history year returns 500. **CLOSED**

`ReadMultiHistoryYearAsync` (`:279-343`) checks the fence first, then `year < 0` → `HistoryYearInvalid`, then `year > currentView.Source.EcologicalYear` → `HistoryYearNotRecorded`, all before calling Core.

Reviewer HTTP probe results:

- **Fresh Year 0, on both experiments.** Each of these returned **409** with the exact code, a fresh fence equal to the current fence, and a non-null view:
  - `int.MaxValue` and `2147483646` → `HistoryYearNotRecorded`;
  - `1` (future) and `0` (not yet recorded) → `HistoryYearNotRecorded`;
  - `-1` and `int.MinValue` → `HistoryYearInvalid`.
- **Stale fences.** Session, source and plan-state stale fences combined with `year = 0` and `year = int.MaxValue` returned `PlayerSessionMismatch`, `PlayerSourceStale` and `PlayerPlanStateStale` respectively.
- **At Year 30.** `31` and `int.MaxValue` → `HistoryYearNotRecorded`; `-7` → `HistoryYearInvalid`.
- **No side effects.** The view stayed byte-identical and server stderr stayed **empty**.

Negative years return `HistoryYearInvalid`, not `HistoryYearNotRecorded`, as spec §4.4 requires. There is no overflow path left: a year that passes the guard is at most the current ecological year.

### F-4 (MINOR): unflagged compact `stage4`. **CLOSED for flags and shape**; spec drift is in R2-3

The flags were truthful at every measured year on both experiments. I checked each against the payload content:

- `historyTruncated` equals (events kept < executed) or (annual kept < year) or (`pruneLog` kept < executed prunes);
- `projectionCompacted` equals "living trees lack `death`" and "scenes lack `heightM`";
- `stage4Compacted` equals "ground cells lack `contributingTreeIds`".

When `stage4Compacted` is true (MIXED Year 33+ only), `contributingTreeIds` is **omitted**, never `[]`, and every vertex keeps `{xm, zm, u, v}`. `stage4Compacted` stays **false at every year 0..30 on both experiments** (author and reviewer workloads). The spec quartet records the Year 30 horizon, the per-year budget, the flags and the never-silent rule (spec §5, plan §1.2(6), checklist P1-UI-04, tasks T-06). The tier order is wrong, though (R2-3).

### F-5 (MINOR): test gaps. **MOSTLY CLOSED**; two assertions pass trivially (NIT N-R2-1)

The new tests are present:

- the per-year sweep 0..30 on both experiments, with no stop at the first truncation (`run-step2-edge.sh`), plus `drive-step2.mjs` per-year loops;
- a check for at least 1 event and at least 1 annual record;
- the zero-history Results text (S2j);
- `history/year` `PlayerSourceStale`, `PlayerPlanStateStale`, `int.MinValue` and `int.MaxValue`;
- Load over a live session with History and Structure open;
- NB-2 order 2 at 1280×720 and 1920×1080;
- the F-6 stale-response witnesses (S2k, S2l, S2m).

Mutation results are in §6. The F-3, F-6 and F-2-text tests are strong. The retention-floor and never-silent assertions cannot fail inside Year 30.

### F-6 (MINOR): stale structure and history responses stored as current. **CLOSED**

- **Structure.** `applyPayload` bumps `structureRequest` and clears `structureLoading` when the tree tag changes (`:3315-3319`). `refreshStructure` captures the tag at request time and discards a response whose counter or tag no longer matches (`:7280-7293`).
- **History.** `loadHistoryIndex(…, append=false)` bumps `historyDetailRequest` (`:6994-6996`). `loadHistoryYear` captures the session and guards on return (`:7027-7037`). Load-confirmed reset bumps both counters (`:3577-3585`).
- **Tests.** Reverting either fix is caught (M2 and M3 are killed by S2k, S2l and S2m).

### Nits from the first review

- **N-1 (closed).** `isOverdue` is now computed on the server from Core's `OverdueReviewPeriodLabel` (`:180`, `:366`), and smoke checks it.
- **N-3 (closed).** Composite responses now carry the bounding flags.
- **N-4 (closed).** The provenance pills come from the server.
- **N-2 and N-5.** Deferred as disclosed.

The UI does not apply the aux `view` from preview-review or history/year (it re-reads on refusal), so projecting that view when there are more than 50 dead trees (`:778`) is harmless.

## 6. Mutation check of the new tests (disposable clone only)

Method: a sparse `git clone --shared` of the repository at `adea7af2`, outside the worktree, with `.p0ref` copied and its pin verified. For each mutant I made an exact single-site replacement in `PlayerMultiApi.cs`, rebuilt the UI, ran the named suite(s) and restored the file. Mutation logs are archived in `r2-opus-probes/mutation/`.

| Mutant | Change | Suites | Result |
|---|---|---|---|
| M0 control | none | smoke | **PASS 362/362** |
| M1 | drop the F-3 `year > current` guard | smoke | **KILLED**: W253 `int.MaxValue` → HTTP 500; W999 and Z02-api stderr |
| M2 | revert F-6 structure (no counter bump, no tag check) | smoke | **KILLED**: S2k stale structure stored as current |
| M3 | revert F-6 history detail (no bump, no session guard) | smoke | **KILLED**: S2l and S2m |
| M4 | revert F-2 Results text to "No ecological year…" | smoke | **KILLED**: S2j |
| M5 | retention floor set to 0 (`minEvents`/`minAnnual`) | smoke + G7 | **SURVIVED**: smoke 362/362 and G7 64/64. No in-horizon state reaches the floor (N-R2-1) |
| M6 | over-budget branch made silent (flag false, no stderr) | smoke + G7 | **SURVIVED**: smoke 362/362 and G7 64/64. No test reaches the over-budget branch (N-R2-1); verified only by my beyond-horizon probe |
| M7 | ecology projection disabled | smoke + G7 | **KILLED** by G7 (60/64; smoke still 362/362): the MA-1-MIXED sweep breached the cap at 534,673 B, with ecology at 118,881 B, and the flag and stderr fired. The ecology projection is therefore load-bearing for the Year 30 budget (relevant to fixing R2-2) |

The clone's `git status` was clean after the run.

## 7. Year 0..30 payload table (P1-UI-04, owner G5 horizon)

Raw UTF-8 bytes of `GET api/multi/view` and of its `ecology` slice. I measured the slice from the raw text and checked it parses to the same object.

- "rerun" = the author's sweep (`run-step2-edge.sh`) re-executed by me in G7.
- "reviewer" = my independent `r2_probe.py`.
- `ev/an` = events / annual records kept in the view.
- flags = `T` historyTruncated, `P` projectionCompacted, `S` stage4Compacted, `X` payloadBudgetExceeded.

| Year | MA-1 rerun raw / eco | ev/an | flags | MA-1 reviewer raw / eco | ev/an | flags | MIXED rerun raw / eco | ev/an | flags | MIXED reviewer raw / eco | ev/an | flags |
|---:|---:|:-:|:-:|---:|:-:|:-:|---:|:-:|:-:|---:|:-:|:-:|
| 0 | 391,058 / 43,320 | 1/0 | `----` | 391,004 / 43,322 | 1/0 | `----` | 393,452 / 45,045 | 1/0 | `----` | 393,392 / 45,041 | 1/0 | `----` |
| 1 | 397,695 / 42,744 | 1/1 | `----` | 397,626 / 42,743 | 1/1 | `----` | 400,092 / 44,487 | 1/1 | `----` | 400,019 / 44,481 | 1/1 | `----` |
| 2 | 401,585 / 41,571 | 1/2 | `----` | 401,270 / 41,689 | 1/2 | `----` | 404,236 / 43,209 | 1/2 | `----` | 403,903 / 43,320 | 1/2 | `----` |
| 3 | 403,734 / 41,080 | 1/3 | `----` | 403,662 / 41,076 | 1/3 | `----` | 406,621 / 42,618 | 1/3 | `----` | 406,549 / 42,606 | 1/3 | `----` |
| 4 | 405,354 / 40,807 | 1/4 | `----` | 405,260 / 40,814 | 1/4 | `----` | 408,894 / 42,258 | 1/4 | `----` | 408,795 / 42,246 | 1/4 | `----` |
| 5 | 408,715 / 39,874 | 1/5 | `----` | 408,356 / 39,995 | 1/5 | `----` | 412,591 / 41,018 | 1/5 | `----` | 412,216 / 41,128 | 1/5 | `----` |
| 6 | 411,022 / 39,426 | 1/6 | `----` | 410,716 / 39,543 | 1/6 | `----` | 415,211 / 40,471 | 1/6 | `----` | 414,897 / 40,583 | 1/6 | `----` |
| 7 | 414,337 / 38,587 | 1/7 | `----` | 414,245 / 38,585 | 1/7 | `----` | 418,897 / 39,386 | 1/7 | `----` | 418,807 / 39,378 | 1/7 | `----` |
| 8 | 416,825 / 38,010 | 1/8 | `----` | 416,769 / 38,019 | 1/8 | `----` | 421,935 / 38,584 | 1/8 | `----` | 421,868 / 38,584 | 1/8 | `----` |
| 9 | 419,410 / 37,400 | 1/9 | `----` | 419,322 / 37,397 | 1/9 | `----` | 425,122 / 37,770 | 1/9 | `----` | 426,089 / 38,046 | 1/9 | `----` |
| 10 | 422,341 / 37,073 | 2/10 | `----` | 422,226 / 37,075 | 2/10 | `----` | 429,561 / 37,955 | 2/10 | `----` | 430,410 / 38,103 | 2/10 | `----` |
| 11 | 425,212 / 36,335 | 2/11 | `----` | 425,107 / 36,338 | 2/11 | `----` | 435,963 / 37,994 | 2/11 | `----` | 435,925 / 37,740 | 2/11 | `----` |
| 12 | 427,587 / 35,901 | 2/12 | `----` | 427,221 / 36,029 | 2/12 | `----` | 457,675 / 41,736 | 2/12 | `----` | 457,356 / 41,609 | 2/12 | `----` |
| 13 | 437,445 / 37,291 | 2/13 | `----` | 437,314 / 37,275 | 2/13 | `----` | 475,601 / 44,496 | 2/13 | `----` | 475,538 / 44,216 | 2/13 | `----` |
| 14 | 445,400 / 37,815 | 2/14 | `----` | 445,226 / 37,803 | 2/14 | `----` | 486,253 / 45,141 | 2/14 | `----` | 485,025 / 44,564 | 2/14 | `----` |
| 15 | 447,337 / 36,953 | 2/15 | `----` | 446,093 / 36,712 | 2/15 | `----` | 501,355 / 46,008 | 2/15 | `----` | 499,174 / 45,233 | 2/15 | `----` |
| 16 | 452,940 / 36,618 | 2/16 | `----` | 451,716 / 36,363 | 2/16 | `----` | 512,449 / 44,917 | 2/16 | `----` | 511,549 / 44,483 | 2/16 | `----` |
| 17 | 457,005 / 36,255 | 2/17 | `----` | 455,752 / 35,996 | 2/17 | `----` | 405,637 / 46,439 | 2/17 | `-P--` | 404,675 / 46,104 | 2/17 | `-P--` |
| 18 | 464,663 / 35,885 | 2/18 | `----` | 463,087 / 35,992 | 2/18 | `----` | 420,563 / 48,378 | 2/18 | `-P--` | 418,452 / 48,022 | 2/18 | `-P--` |
| 19 | 473,135 / 35,578 | 2/19 | `----` | 469,493 / 35,195 | 2/19 | `----` | 435,402 / 49,685 | 2/19 | `-P--` | 436,149 / 50,679 | 2/19 | `-P--` |
| 20 | 476,298 / 34,214 | 3/20 | `----` | 473,607 / 34,333 | 3/20 | `----` | 445,827 / 49,228 | 3/20 | `-P--` | 444,657 / 49,432 | 3/20 | `-P--` |
| 21 | 486,057 / 35,317 | 3/21 | `----` | 483,739 / 35,333 | 3/21 | `----` | 453,638 / 48,112 | 3/21 | `-P--` | 453,163 / 48,425 | 3/21 | `-P--` |
| 22 | 490,756 / 34,282 | 3/22 | `----` | 489,468 / 34,528 | 3/22 | `----` | 460,035 / 45,898 | 3/22 | `-P--` | 460,438 / 46,206 | 3/22 | `-P--` |
| 23 | 495,290 / 33,672 | 3/23 | `----` | 493,970 / 33,907 | 3/23 | `----` | 472,137 / 46,379 | 3/23 | `-P--` | 470,262 / 45,970 | 3/23 | `-P--` |
| 24 | 499,271 / 33,319 | 3/24 | `----` | 497,241 / 32,939 | 3/24 | `----` | 486,062 / 46,895 | 3/24 | `-P--` | 481,523 / 45,510 | 3/24 | `-P--` |
| 25 | 504,081 / 32,822 | 3/25 | `----` | 504,564 / 32,856 | 3/25 | `----` | 498,510 / 47,034 | 3/25 | `-P--` | 497,270 / 46,934 | 3/25 | `-P--` |
| 26 | 512,286 / 33,195 | 3/26 | `----` | 512,423 / 33,308 | 3/26 | `----` | 510,723 / 46,825 | 3/26 | `-P--` | 512,328 / 47,613 | 3/26 | `-P--` |
| 27 | 516,961 / 32,112 | 3/27 | `----` | 518,728 / 32,971 | 3/27 | `----` | 524,032 / 47,475 | 3/25 | `TP--` | 523,665 / 48,308 | 3/24 | `TP--` |
| 28 | 524,279 / 32,749 | 3/28 | `----` | 398,520 / 33,595 | 3/28 | `-P--` | 522,957 / 46,028 | 3/19 | `TP--` | 523,992 / 46,835 | 3/19 | `TP--` |
| 29 | 400,967 / 32,108 | 3/29 | `-P--` | 402,235 / 32,365 | 3/29 | `-P--` | 523,112 / 46,109 | 3/12 | `TP--` | 523,737 / 47,190 | 3/11 | `TP--` |
| 30 | 407,861 / 32,650 | 4/30 | `-P--` | 406,935 / 31,152 | 4/30 | `-P--` | 523,968 / 48,441 | 4/6 | `TP--` | 522,224 / 47,571 | 4/6 | `TP--` |

Year 0 is measured after the Year-0 executed work. Every one of the 124 measurements is ≤ 524,288 B raw and ≤ 153,600 B ecology, with `stage4Compacted = false` and `payloadBudgetExceeded = false`.

The 9 B margin at MA-1 Year 28 (author workload) is a deterministic Tier-0 payload. It reproduced byte-for-byte in my rerun, and my own workload switched to the projection tier at that same year. The MIXED values of about 523–524 KB at Years 27–30 come from the binary search filling up to the cap. They are not a measurement artifact: I took the raw `Content-Length`-equivalent byte count of each response body.

## 8. New findings (R2)

### R2-1 — SHOULD (D-030 state honesty, new regression): the Results pane claims rows were "omitted for payload budget" at Year 0 after any executed work

- **Where:** `PlayerMultiApi.cs:5934-5939` (`renderHistory`). The branch condition is `state.totalRecordedYears > 0 || state.historyTruncated || ecologicalYear > 0`.
- **Why it fires:** Core counts a year with executed work as a recorded year (`MultiAreaHistoryQuery.cs:403-411, 440`). So after the first Execute at Year 0 the state is `annualHistory = []`, `events = 1`, `totalRecordedYears = 1`, `historyTruncated = false`.
- **Observed in real Edge** (MA-1, `r2_edge.mjs ma1y0`):
  - Before any work, Results correctly says "No ecological year has been advanced in this session."
  - After one Year-0 Fell → Approve → Execute, then a real Re-read click, Results says: **"Compact view omits annual history rows to stay within payload budget; open History in the header to inspect all 1 recorded year."**
  - The API state at that moment was `historyTruncated=false`, `projectionCompacted=false`, view `annualHistory=0`, `events=1`.
  - The execute command's own response carries the same `totalRecordedYears=1`, so the UI's own Execute path produces the same text.
  - Screenshot: `r2-opus-probes/edge/r2-ma1-year0-after-fell-results-1280x720.png`.
- **Also measured:** my HTTP probe reached the same state on **both** experiments. The handoff's own MA-1 table row "Year 0: ev 1, an 0, totalRecYrs 1" is this state.
- **Reach:** the normal first turn (any Fell, Prune, Clear, Plant, Shelter or SpotClear executed before the first advance), and Load of such a save. Nothing was omitted, so the statement is false; the old text was true in this state.
- **Minimal correction:** decide on truncation, not on the recorded-year count. For example, show the "Compact view omits …" text only when `state.historyTruncated` is true, keep `ecologicalYear > 0` as a defensive fallback, and otherwise keep "No ecological year has been advanced in this session." Add a smoke or Edge assertion at Year 0 after Execute.

### R2-2 — SHOULD (spec §5 "inspector behaviour preserved"; D-020/D-030): the projection replaces Core's life state with browser-invented values and drops the "Pruning lifts" row for dead trees

- **Where:**
  - `ProjectStage4ForBudget` drops every dead `stage4` tree with `pruningLifts == 0` (`PlayerMultiApi.cs:712`).
  - `ProjectEcologyForView` drops their `pruneStatusByTreeId` entries (`:418-438`, always on).
  - The new inspector fallback (`:5313-5316`) shows `tree.death.cause === 'fell' ? 'FelledStump' : dw ? 'DeadFallen' : 'DeadStanding'`.
- **Core's actual values:** Core's `LifeState` has only `Living`, `FelledStump` and `BiologicallyDead` (`Stage4Models.cs:34-39`, `LifeStates.Of :113-115`). The readout gives a death cause and year only for `BiologicallyDead` (`MultiAreaContinuation.cs:439-441`).
- **Observed in real Edge** (MA-1-MIXED, `r2_edge.mjs mixedlife`, no extra work after a Year-0 Fell):

  | Tree | Year 3, Tier 0 | Year 18, `projectionCompacted=true` |
  |---|---|---|
  | `MA-A-018` (natural death, Year 2) | Life state "BiologicallyDead · DensitySuppression, year 2"; Pruning lifts "0 / 3 (5-yr wait)" | Life state "**DeadFallen** · DensitySuppression, year 2"; **no Pruning lifts row** |
  | `MA-A-058` (stump) | "FelledStump" | "FelledStump · fell, year 0" |

  On the same Year-18 page, the map marks `MA-A-018` with the legend's "Standing dead · D" marker. So the page now contradicts itself about the same stem.
  - Screenshots: `r2-mixed-tier0-natural-dead-inspector-1280x720.png` and `r2-mixed-projected-natural-dead-inspector-1280x720.png`.
- **Reach:** inside the horizon, from MA-1-MIXED Year 17 and MA-1 Year 28/29 on both workloads, whenever any dead tree is inspected.
- **Why it matters:**
  - `DeadFallen` and `DeadStanding` are not engine states.
  - Spec §5 states that "map/table/inspector UI behaviours remain preserved" and that the UI "falls back honestly".
  - The handoff calls the ecology projection "lossless for UI".
- **Minimal correction:** have the fallback show exactly what Core would show (`FelledStump` with no suffix; otherwise `BiologicallyDead · cause, year`). Better, keep a tiny server-side dead-tree entry so the browser derives no model state. Either restore the "Pruning lifts" row for dead trees, or record its removal as an accepted, spec-documented projection effect (a Manager/owner call). Mutant M7 shows the ecology projection is needed to hold the Year 30 budget. So do not send the full dead-tree `ecology`/`stage4` entries back: either fix the browser fallback to show the exact engine values, or add a minimal server-side field (life state only) and re-measure Years 0..30. Add an Edge check that compares the inspector text across the tier boundary.

### R2-3 — SHOULD (spec accuracy, plus a latent breach of the §5 retention rule): the spec's tier list does not match the code, and the code has an unflagged zero-history emergency tier

**Spec vs code order.**

- **Spec and plan** (spec §5 lines 182-186; plan §1.2(6)):
  1. Tier 1: trim history on **full, unprojected** `stage4`;
  2. Tier 2: projection;
  3. Tier 3: ground compaction;
  4. Tier 4: flag and log.
- **Code** (`:565-627`):
  1. Tier 0: full;
  2. Tier 1: projection **plus** history trim;
  3. Tier 2: ground compaction;
  4. Tier 3: an **emergency `TailRaw(0, 0)`** that returns zero events and zero annual records with `payloadBudgetExceeded: false`;
  5. finally: flag and log.

  Observed: at MIXED Year 17 the view went straight to `projectionCompacted=true` with full history (17/17). That is the code's order, not the spec's.

**Handoff vs code.** The handoff says it "Documented exact monotonic Tiers 0–3 in spec.md/plan.md" (§2 F-4), and §7 residual risk 1 says "Tier 3 explicitly sets payloadBudgetExceeded". Neither matches the code.

**Reach.** The emergency tier is reached only when one event plus one annual record is exactly the overflow margin. Neither workload hit it: my MIXED run went from 2/2 at Year 35 straight to a flagged 1/1 at Year 36. So this is latent and past the horizon. If reached, it reproduces the original F-2 projection (all history dropped). The Results text would then be honest, and `historyTruncated=true`.

**Minimal correction:** delete the emergency branch so that the flagged branch keeping 1 event and 1 annual record handles it, and rewrite spec §5 and plan §1.2(6) in the code's real order. Alternatively, keep the branch but flag it and document it.

### NITs

- **N-R2-1 (test strength).** Mutants M5 (retention floor 0) and M6 (silent over-budget) survived both smoke (362/362) and G7 (64/64) (§6). Within Year 30 no state reaches the floor or the over-budget branch: the smallest in-horizon tail is 4 events and 6 annual records. So "≥1 event / ≥1 annual" and "never silent" pass trivially within the horizon. I verified both behaviours only by probing beyond the horizon (§5 F-1, F-2). A forced check would pin them, for example a smoke fault mode or test hook with a lowered cap, or an informational MIXED run to Year 36 that expects `payloadBudgetExceeded=true` plus the stderr line.
- **N-R2-2.** Dead-tree floats are rounded to 3 decimals and then shown with 2 (`fixed(…, 2)` in the inspector and table). In rare cases the displayed hundredth can change across the tier boundary. Display only.
- **N-R2-3.** `ReadMultiViewAsync` keeps an unused `var bounded …; _ = bounded;` (`:162-163`).
- **Pre-existing, out of scope:** `tests/phase1-save-ui/run-edge-e2e.sh` (unchanged here) still writes unredacted capability URLs to its evidence `run.log` and `server-*.out`. I redacted my R2 copies; the tokens were dead.

## 9. Per-P1-UI verdicts (R2)

| Item | Verdict | Basis |
|---|---|---|
| P1-UI-01 Preview / Skip | **PASS** | G5/G7, plus the server-computed `isOverdue` (N-1 closed); skip ran every year in my probe |
| P1-UI-02 History | **PASS** | F-3 closed (§5); paging, provenance pills (N-4) and Escape behaviour in G5/G7 |
| P1-UI-03 Structure | **PASS** | G7: MA-1 `M_i=0.00`, MIXED `M_i=0.3790`; F-6 races closed |
| P1-UI-04 Payload budget | **FAIL (honesty)**; the budget itself PASSES | Budget, flag truthfulness, never-silent and projection-only all hold for Years 0..30 on both experiments (§5, §7). But the Results-pane honesty clause in the P1-UI-04 text fails at Year 0 after work (R2-1). The projection also does not preserve inspector behaviour (R2-2), and the spec's tier rule misdescribes the code (R2-3) |
| P1-UI-05 Continuity | **PASS** | G7 Save → SIGINT → restart → Load matched; Load over a live session with History and Structure open passed (F-5) |
| P1-UI-06 NB-2 | **PASS** | G7: orders 2 and 3 at 1280×720 and 1920×1080 |
| P1-UI-07 Edge matrix | **PASS** | G7: zero horizontal scroll, single-row topbar, R3 fold, Escape, zero console errors, CSP violations or 5xx. My own Edge runs also had zero console errors |
| P1-UI-08 Hygiene | **PASS** | Delta: 0 capability URLs, 0 64-hex strings, 0 `data:image/webp`, 0 inline `<script>`, 0 `style=`, 0 `innerHTML`/`eval`, 0 csproj, props, package or `PackageReference` changes, 0 `wwwroot` changes, 0 checklist boxes ticked. Exec bits are preserved |
| P1-UI-REG | **PASS** | All 7 gates (§3) |
| Gate G2 ACCEPTANCE | **FAIL** (R2-1, R2-2) | |
| Gate G3 MEASUREMENT INTEGRITY | **PASS for the budget numbers** | Reproduced byte-identically, and confirmed independently with a different workload. The handoff's spec/code-alignment claims are inaccurate (R2-3) |

## 10. Residual risks

- Composite endpoints (commands, skip, load, cell, hit, competition) now run the bounded serializer. That is 1 serialization at Tier 0, and up to several dozen in the worst case (binary search × `pruneLog` caps × two tiers). I did not measure latency. It was not noticeable in the gates.
- Beyond Year 30, MA-1-MIXED exceeds the view budget from Year 36 (flagged and logged). That is allowed under G5 but is a known limit for any longer horizon.
- The single-Fell and six-family workloads leave the history tail at no fewer than 4 events and 6 annual records by Year 30. Heavier per-year work could tighten that; the retention floor would still hold.

## 11. NOT TESTED

- The code's zero-history emergency tier (R2-3): inspection only; neither workload reached it.
- The over-budget notice text in the real UI ("exceeded the 512 KiB / 150 KiB transport budget"): the flag and stderr were verified over HTTP beyond Year 30, but the page was not opened there.
- The F-6 races in a real browser: covered by the smoke DOM witness and mutants M2/M3 only.
- Latency of the composite endpoints under the bounded serializer.
- `CancelledBetweenYears`, `AnnualStepRefused` and `StandCannotAdvance` skip outcomes over HTTP (unchanged by the delta).
- Unity: not applicable. This is a standalone lab with no `Assets`, `Packages` or `ProjectSettings` change.

## 12. End-of-review state

- **Worktree and branches:**
  - HEAD is still `adea7af2`.
  - Before this commit, `git status --short` showed only this new file.
  - Remote feature is `adea7af2` and `main` is `cc2757a7`, both unchanged.
  - `.p0ref` pin exact.
- **Processes:**
  - Listeners `:33767` (1750123), `:43649` (1806361) and `:46593` (839989) are LISTEN.
  - The same 13 `sitkap1-ui.dll` PIDs as at start, all from other lanes.
  - Zero processes reference this lane, the worktree or the scratch.
  - Zero `msedge` processes and zero temp profiles from this lane (`opus-r2-edge*`).
  - Every server I started stopped on SIGINT with rc 0.
- **Scratch:** the disposable mutation clone and `TMPDIR` (`/home/jer/ccf-p1ui-r2-tmp`) were removed.
- **Evidence,** outside Git, mode `go-rwx`, under `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125/`:
  - `r2-gates/`, `r2-smoke/`, `r2-edge-save/`, `r2-edge-step2/`;
  - `r2-opus-probes/{http,edge,mutation,scripts}/`.
  - Capability URLs were redacted; 0 remain.

## 13. Verdict and integration decision

```text
INTEGRATION DECISION
Status: NEEDS CHANGES
Gate: G2 ACCEPTANCE (FAIL — P1-UI-04 honesty clause R2-1; spec §5 inspector-preservation R2-2); G3 PASS for budget evidence; G4 not reached
Reviewed branch: feat/multi-2d-phase1-time-history-ui-20261125-v1
Reviewed HEAD: adea7af220259a1808346cd5381babed3e9052fa (delta from 513355c4bcef881413d517aa62feed4676234319)
Context commit: 5fe2fabe9073d7b72711f4ab48553c785351d7bc
SPEC: SPEC-multi-2d-phase1-time-history-ui-V1 / IMPLEMENTING — Conformance: FAIL (P1-UI-04 Results honesty; §5 inspector preservation; §5 tier list vs code)
Prior findings: F-1 CLOSED (Year 0..30, both experiments, never silent); F-2 PARTLY CLOSED (retention + truncated-case text fixed; R2-1 new false text; R2-3 latent zero-history tier); F-3 CLOSED; F-4 CLOSED (flags/shape), spec order drift in R2-3; F-5 MOSTLY CLOSED (N-R2-1); F-6 CLOSED
Gates on adea7af2: G1 0W/0E; G2 Core 277/277; G3 Save 50/50; G4 UI 0W/0E; G5 smoke 362/362; G6 save Edge 63/63; G7 Step 2 Edge 64/64
Required fixes:
- R2-1: Results-pane condition must key on truncation, not totalRecordedYears; add Year-0-after-Execute assertion.
- R2-2: inspector Life state must show engine values under projection (or keep a minimal server-side dead-tree entry); decide/document the dead-tree Pruning-lifts row; add a cross-tier Edge check.
- R2-3: remove (or flag + document) the zero-history emergency tier; make spec §5 / plan §1.2(6) match the real tier order.
Next: narrow re-review (R3) of the fix delta: rerun G5 + G7, the Year 0..30 sweeps, and the Edge checks above.
Integration target: none (NOT MERGED, NOT INTEGRATED, NOT OWNER-ACCEPTED)
```

**NOT MERGED. NOT INTEGRATED. NOT OWNER-ACCEPTED.**
