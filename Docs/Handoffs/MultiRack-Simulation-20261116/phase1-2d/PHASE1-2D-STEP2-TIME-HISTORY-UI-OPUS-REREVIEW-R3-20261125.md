# CCF Multi 2D — Phase 1 Step 2 (Time History UI) Repair #2 Delta: Independent Re-Review R3

**Verdict: APPROVED**

> **IMPLEMENTED ON TASK BRANCH — INDEPENDENTLY REVIEWED APPROVED — READY FOR COMBINED PHASE 1 OWNER PLAYTEST — NOT INTEGRATED, NOT ACCEPTED.**
>
> **NOT MERGED. NOT INTEGRATED. NOT OWNER-ACCEPTED.** This is an independent review record only. No merge, PR, tag, force-push, amend, `main` write or feature-branch write was made. No requirements checkbox was ticked by this review.

| Item | Value |
|---|---|
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`), independent non-author reviewer, T3 Code / Claude Code harness |
| Implementer | Neon (`antigravity_2` / `gemini-4-neon`), a different provider and thread |
| Separation | No implementation role. No source, test, spec or implementation-handoff edits. No subagents. Probes and mutations ran outside the review worktree (scripts under `/home/jer/ccf-p1ui-r3-scripts`, scratch under `/home/jer/ccf-p1ui-r3-tmp`, since removed). The only repository write is this file on the R3 review branch |
| Freshness | Same reviewer identity as R1 (`ea6961ae`) and R2 (`45061941`), new session. Inputs were the R2 review, the repair diff and the code. The implementer's handoff was read only as a list of claims to check. I re-derived every number below |
| Review worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-ReReview-R3-20261125` |
| Review branch | `review/multi-2d-phase1-time-history-ui-20261125-v1-r3` |
| Repaired feature HEAD (reviewed) | `47b27a8d3873b32726f315ce1182118da38c50b1` (`feat/multi-2d-phase1-time-history-ui-20261125-v1`) |
| Previously reviewed HEAD | `adea7af220259a1808346cd5381babed3e9052fa` |
| Base | `5fe2fabe9073d7b72711f4ab48553c785351d7bc` |
| R2 review | `45061941e45c4f5631127975bf87386e0c405847` (NEEDS CHANGES: R2-1, R2-2, R2-3) |
| Binding owner decisions | G5: P1-UI-04 horizon = **Year 30**, projection-only reductions, never return over-budget bytes silently. Repair #2 authorized **only** for R2-1/R2-2/R2-3, then this one R3. Pre-decided R2-2 rule: omit the dead-tree "Pruning lifts" row identically in all tiers, documented in spec §5 |
| Date | 2026-11-25 |

## 1. Summary

All three R2 findings are closed. I checked each fix in the code and reproduced it in real Edge, then confirmed with mutation tests that the new tests catch a revert:

- **R2-1 (CLOSED).** The "Compact view omits annual history rows…" text now shows only when `historyTruncated` is true **and** the ecological year is above 0. In real Edge, Year 0 after an executed Fell (`totalRecordedYears = 1`, `historyTruncated = false`) shows exactly "No ecological year has been advanced in this session." This holds through both the Re-read path and the page's own Execute button. A genuinely truncated MIXED year still shows the truncation notice. Reverting the condition is caught by smoke `R2a` and by the G7 `R2-1` check (mutants MR1 and MR1b).
- **R2-2 (CLOSED).** The server now sends Core's exact `LifeStates.Of(tree)` as `death.lifeState` on projected dead trees. The inspector no longer derives `DeadFallen` or `DeadStanding`. The "Pruning lifts" row is removed for every dead tree in every tier, and spec §5 documents that rule. My HTTP probe recomputed the inspector's Life-state text for **every dead tree at every year 0..50** on both experiments. Each matched Core's rule and stayed constant across the Tier 0 → Tier 1 (→ 2 → 3) changes. Real-Edge checks passed on the very trees R2 flagged (`MA-A-018`, `MA-A-058`) and on a new case, a pruned stump. Mutants MR2, MR3 and MR4 are all caught by G7.
- **R2-3 (CLOSED).** The zero-history emergency tier is gone. The only over-budget branch keeps at least 1 event and 1 annual record, sets `payloadBudgetExceeded`, and logs to stderr. Spec §5 and plan §1.2(6) now describe the code's Tier 0 → 1 → 2 → 3 order exactly. Beyond the horizon, MA-1-MIXED is flagged and logged at Years 35–50 (74 stderr lines) and keeps 1/1 history at every flagged year.

**Budget (G5, Year 30).** Year 0..30 is within both budgets on both experiments, under two workloads:

- **Author sweep, rerun:** byte-identical to the handoff. MA-1 max **524,279 B** (Year 28); MA-1-MIXED max **523,375 B** (Year 29); `ecology` max **43,320 B** and **49,685 B**.
- **My workload:** MA-1 max 518,728 B; MA-1-MIXED max 523,849 B; `ecology` max 43,322 B and 51,129 B.

**Gates on `47b27a8d`:**

| Gate | Result |
|---|---|
| G1 solution build | 0 warnings / 0 errors |
| G2 Core | **277/277**, all 8 printed anchors present; with the `.p0ref` pin, **9/9** |
| G3 Save | **50/50** |
| G4 UI build | 0 warnings / 0 errors |
| G5 smoke | **363/363** |
| G6 Save Edge | **63/63** |
| G7 Step 2 Edge | **67/67** |

**Findings.** No new defect. Four doc-level or informational nits (§9), none blocking.

## 2. Start-of-review checks (exact results)

| # | Check | Result |
|---|---|---|
| 1 | `git rev-parse HEAD`; `git status --short` | `47b27a8d3873b32726f315ce1182118da38c50b1`; status **empty**: at start, after every gate, after every probe, and before writing this file |
| 2 | `git diff --name-only adea7af2..HEAD` | **Exactly the 6 claimed files.** These are: the Neon handoff, `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`, `tests/phase1-time-history-ui/drive-step2.mjs`, `tests/ui-smoke-multiarea.sh`, `specs/…/plan.md` and `specs/…/spec.md`. Stat: 6 files, +293/−210. One commit (`47b27a8d`, parent `adea7af2`). `git diff --summary`: no mode changes. `ui-smoke-multiarea.sh` and `run-step2-edge.sh` stay 100755. 0 CR bytes in any changed file |
| 3 | `git diff --name-only 5fe2fabe..HEAD` against `src/Core/**` (including `src/Core/Phase1/**`), `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/coordination/**` | **Zero** paths (forbidden count = 0). The base..HEAD list is the same 9 files R2 saw. No `wwwroot`, csproj, props, sln or package changes |
| 4 | Remote and pins | `origin/feat/multi-2d-phase1-time-history-ui-20261125-v1` = `47b27a8d…` (reviewed HEAD). `origin/main` = `cc2757a730ad9451e3d6e3680dffd123171a5df5` (unchanged). `.p0ref/SitkaLab.Core.dll` sha256 = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (exact, read-only, not rebuilt). Listeners `127.0.0.1:33767` (pid 1750123), `:43649` (pid 1806361) and `:46593` (pid 839989) all LISTEN |
| 5 | Ephemeral lane processes | At start: 13 `sitkap1-ui.dll` processes, all from other lanes (their cwds are outside this lane). None referenced this worktree. The only headless `msedge` was another task's (`opencode\ccf-v2-task` profile), which I left alone. End state: §12 |

## 3. Gates rerun by the reviewer (on `47b27a8d`)

Environment:

- WSL2 Ubuntu, `PATH="$HOME/.dotnet:$PATH"` (dotnet 8.0.134), `TMPDIR=/home/jer/ccf-p1ui-r3-tmp` (outside the repo);
- node v26.13.1;
- Windows Microsoft Edge `--headless=new`.

Commands were exactly as briefed, run from `Tools/MultiRackSitkaP1Lab`. I also reran G6, which was cheap.

| Gate | Command | Exact result |
|---|---|---|
| G1 | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | Build succeeded, **0 Warning(s), 0 Error(s)** (11 s) |
| G2 | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **277/277 passed, 0 failed** (50 s). All 8 printed anchors are present (`7e3a2a86`, `a692a6d5`, `814e8165`, `8a926776`, `c4b670ed`, `c67edc6b`, `776a68df`, `93e9abda`); with the `.p0ref` pin, **9/9** |
| G3 | `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | **50/50 passed, 0 failed**; 0 warning or error lines |
| G4 | `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | Build succeeded, **0 Warning(s), 0 Error(s)** |
| G5 | `TMPDIR="$TMPDIR" tests/ui-smoke-multiarea.sh …/r3-smoke` | **OVERALL: PASS (363 passed, 0 failed)**. Includes the new `R2a` (Year 0 after Execute) and the F-2 witness `S2j`. Z01/Z02 SIGINT and empty stderr ×3; Z03 lab tree unchanged; Z04 git status unchanged |
| G6 | `tests/phase1-save-ui/run-edge-e2e.sh …/r3-edge-save` | **OVERALL: PASS (63 passed, 0 failed)** (13 + 12 + 18 + 6 + 6) |
| G7 | `tests/phase1-time-history-ui/run-step2-edge.sh …/r3-edge-step2` | **OVERALL: PASS (67 passed, 0 failed)**: `ma1-full` 29/29, `mixed-play-save` 20/20, `mixed-restart-load` 6/6, `sweep-ma-1` 1/1 (31/31 years), `sweep-ma-1-mixed` 1/1 (31/31 years), plus 10 Z01/Z02 SIGINT and empty-stderr checks over the 5 server processes |

The count changes against R2 are only the new checks: smoke 362 → 363 (`R2a`); G7 64 → 67 (`R2-1` and the two `R2-2` cross-tier checks). The test delta contains only added lines; no existing assertion was removed or weakened. G7's printed sweep values are byte-identical to the handoff's §4 table. Gate logs are in `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125/r3-gates/`.

## 4. Delta scope reviewed

I read the following in full:

- the `PlayerMultiApi.cs` diff (server and page script);
- the smoke and `drive-step2.mjs` diffs;
- the spec and plan diffs;
- the updated handoff.

I re-read the code around each change, with current line numbers:

- `SerializeBoundedMultiView` (`PlayerMultiApi.cs:451-616`);
- `ProjectViewForBudget` (`:621-649`);
- `ProjectStage4ForBudget` (`:674-739`);
- `MultiProjectedDeathDto` (`:1151`);
- `renderInspector` (`:5258-5332`);
- `pruneStatusOf` (`:3126-3140`);
- `renderHistory` (`:5907-5938`).

I checked the Core contracts the fix relies on, all read-only:

- `LifeState` (`src/Core/Stage4Models.cs:34-39`);
- `LifeStates.Of` (`:113-115`);
- `FellCause.Harvest = "fell"` (`src/Core/WorkContract.cs:52`);
- the readout's `DeathCause`/`DeathYear`, which are set only for `BiologicallyDead` (`src/Core/MultiArea/MultiAreaContinuation.cs:439-441`);
- `PlayerMapStatus` (`src/Core/PlayerMapScene.cs:4-11`).

## 5. Finding-by-finding verification

### R2-1 (SHOULD, D-030): false "omitted for payload budget" text at Year 0 after work. **CLOSED**

**Code** (`PlayerMultiApi.cs:5924-5938`):

- `const hasAdvancedYear = Boolean(view.source && view.source.ecologicalYear > 0);`
- The "Compact view omits annual history rows to stay within payload budget…" text shows only when `state.historyTruncated && hasAdvancedYear`.
- A second, non-budget branch ("Annual history rows are not included in this view…") applies only when the year is above 0 and nothing was truncated. Otherwise the pane shows "No ecological year has been advanced in this session."
- `totalRecordedYears` is now used only to word the year count (`:5927`, `:5932`), never to decide which text shows.
- The separate header note `#history-truncated-note` (`:5911-5922`) is unchanged. It is still keyed on `historyTruncated || payloadBudgetExceeded`.

**Reproduced in real Edge** (my own CDP driver `r3_edge.mjs ma1y0`, MA-1, 1280×720):

| Step | API state | Results pane (`#year-latest`) | `#history-truncated-note` |
|---|---|---|---|
| Fresh Year 0 | — | "No ecological year has been advanced in this session." | hidden |
| Year-0 Fell → Approve → Execute (out of band), then a real Re-read click | `ecologicalYear=0`, `totalRecordedYears=1`, `historyTruncated=false`, `annualHistory=0`, `events=1` (this is exactly the R2-1 state) | **"No ecological year has been advanced in this session."** | hidden |
| Second Year-0 Fell through the page's own buttons (Trees row → "Fell this tree instead" → View plan → Approve → Execute → Confirm), **no** Re-read | `totalRecordedYears=1`, `events=2`, `historyTruncated=false` | **"No ecological year has been advanced in this session."** | hidden |
| Control: skip to Year 1 | — | "Latest year 0 → 1 …" | hidden |

Zero console errors; server stderr 0 bytes; SIGINT rc 0. Screenshots: `r3-opus-probes/edge-ma1y0/r3-ma1-year0-after-fell-reread-results-1280x720.png` and `…/r3-ma1-year0-after-page-execute-results-1280x720.png`.

**The truncated case still shows its notice.** In real Edge (`r3_edge.mjs mixedlife`, MA-1-MIXED, no work after Year 2), the first truncated year was Year 28: `historyTruncated=true`, 26 of 28 annual records kept. There the note read "Compact view shows the most recent tail only; open History in the header for all 29 recorded years.", and `#year-latest` showed the latest-year table. In the G7 sweeps the author's workload is truncated at MIXED Years 27–30, and my HTTP workload at the same years. The empty-annual-history form of the "Compact view omits…" text cannot occur in a real payload, because the retention floor keeps at least 1 annual record once any exists. It is covered by the injected-fault smoke witness `S2j`, which still passes.

**The tests catch a revert** (§6):

- **MR1** restores the R2 condition `totalRecordedYears > 0 || historyTruncated || hasAdvancedYear`.
- **MR1b** keys on `totalRecordedYears > 0` alone.

Both are **KILLED** by smoke `R2a` (`tests/ui-smoke-multiarea.sh:1657-1667`) and by G7 `R2-1` (`drive-step2.mjs:728-745`). Each failure prints the exact false text "Compact view omits annual history rows to stay within payload budget; open History in the header to inspect all 1 recorded year."

### R2-2 (SHOULD, spec §5 / D-020 / D-030): browser-invented life state; inconsistent Pruning-lifts row. **CLOSED**

**Server.**

- `ProjectViewForBudget` builds each dead tree's `MultiProjectedDeathDto(d.EcologicalYear, d.Cause, d.Mechanism, LifeStates.Of(tr))` (`:641`, record `:1151`).
- `LifeState` is the Core enum, serialized as a string by the shared `JsonStringEnumConverter` (`PlayerApi.cs:59`). The values are exactly `FelledStump` or `BiologicallyDead`.
- Tier-0 payloads are unchanged: the death keys are `{cause, ecologicalYear, mechanism, treeId}`. Projected payloads have `{cause, ecologicalYear, lifeState, mechanism}`, measured on both experiments.

**Inspector** (`:5299-5310`):

1. **Tier 0, and pruned dead trees in projected tiers.** These keep their Core `stage4` entry, so the inspector renders `s4.lifeState` plus the cause/year suffix only when Core gives a `deathCause`. Core gives a cause only for `BiologicallyDead`.
2. **Unpruned dead trees in projected tiers.** These have no `stage4` entry. The inspector uses `tree.death.lifeState` and renders `FelledStump` with no suffix, or `BiologicallyDead · <cause>, year <y>`. That is the same text as path 1.
3. **No invented states.** `DeadFallen` and `DeadStanding` occur nowhere in `src/` or the spec quartet, and in no payload I captured.
4. **Fallback.** If `lifeState` were missing, the inspector would fall back to `item.status === 'Felled' ? 'FelledStump' : 'BiologicallyDead'`. That comes from Core's own map status, not from deadwood presence. It was never used in any measured payload (see N-R3-3).

**Pruning-lifts rule.**

- The row now requires `tree.isLiving && ps` (`:5307`), so it is absent for every dead tree in every tier. This is the pre-decided rule.
- Spec §5 records it in the "Dead-tree inspector contract" bullet (`spec.md:182`). Plan §1.2(6) repeats it (`plan.md:44`).
- Why the guard matters: in Tier 0, `pruneStatusOf` falls back to the `stage4` entry, so **every** Tier-0 dead tree has a prune status. My probe measured 150 of 150 (MA-1, Year 27) and 111 of 111 (MIXED, Year 16). A pruned stump also keeps its `ecology` prune status in projected tiers. The guard is the only thing that hides the row in these cases.

**Every dead tree, every year (HTTP).** My independent probe `r3_probe.py` re-implements the inspector's Life-state and Pruning-lifts logic from each payload. For every dead tree at every year 0..50 on both experiments it checked that:

- the rendered string equals Core's rule (`cause == "fell"` → `FelledStump`, else `BiologicallyDead · cause, year y`);
- `death.lifeState` equals the Core enum;
- no Pruning-lifts row would render;
- the payload contains no `DeadFallen` or `DeadStanding`.

Results: **PASS on both**, and **every dead tree's rendered string stayed constant across all years and tiers.** Coverage:

- **MA-1:** Tier 0 → Tier 1 at Year 27 → 28. At Year 27, 150 dead trees, all rendered through `stage4`. At Year 28, 152 dead trees, all rendered through `death.lifeState`.
- **MA-1-MIXED:** Tier 0 → 1 → 2 → 3 (Years 16 → 17 → 31 → 35). At Year 17, 120 dead trees: 119 through `death.lifeState`, plus 1 pruned stump (`MA-B-001`) still rendered through `stage4`. My workload added the pruned stumps on purpose.
- **Fallback:** never used (0 hits at any year).

**Cross-tier, real Edge.**

- **Author checks (G7).** MA-1 Year 28 (Tier 0) → Year 29 (Tier 1): natural `R26-MA-A-9-6` "BiologicallyDead · DensitySuppression, year 27"; stump `MA-A-001` "FelledStump"; Pruning lifts absent in both tiers. MA-1-MIXED Year 16 → Year 17: natural `MA-A-043` "BiologicallyDead · DensitySuppression, year 1"; stump `MA-A-001` "FelledStump"; absent in both. Both PASS.
- **My own check (`r3_edge.mjs mixedlife`):** MA-1-MIXED Year 3 (Tier 0) → Year 17 (first projected year). I used the **same two trees R2 flagged**, plus a pruned stump (`MA-B-001`, pruned at Year 0 and felled at Year 2):

  | Tree | Tier 0 (Year 3) Life state / Pruning lifts | Projected (Year 17) Life state / Pruning lifts | R2 observed (projected) |
  |---|---|---|---|
  | `MA-A-018` natural death, Year 2 | "BiologicallyDead · DensitySuppression, year 2" / absent | **"BiologicallyDead · DensitySuppression, year 2" / absent** | "DeadFallen · DensitySuppression, year 2" / absent (Tier 0 had "0 / 3 (5-yr wait)") |
  | `MA-A-058` stump | "FelledStump" / absent | **"FelledStump" / absent** | "FelledStump · fell, year 0" |
  | `MA-B-001` pruned stump (`lifts > 0`, kept in projected `stage4`) | "FelledStump" / absent | **"FelledStump" / absent** | not tested in R2 |

  The projected payload had `death.lifeState` = `BiologicallyDead` / `FelledStump` / `FelledStump`. The unpruned natural tree was dropped from `stage4`, and the pruned stump was kept. Death year, cause, species, age, fallen deadwood, recorded reason and chip were identical across tiers. Zero console errors. Screenshots: `r3-opus-probes/edge/r3-mixed-{tier0,projected}-natural-dead-inspector-1280x720.png` and `…/r3-mixed-{tier0,projected}-pruned-stump-inspector-1280x720.png`.

**Budget with the new field.** Year 0..30 holds on both experiments (§7). The full dead-tree `ecology`/`stage4` entries were **not** restored. The projection still drops unpruned dead `stage4` trees and their `pruneStatusByTreeId` entries; the only addition is the `lifeState` string. The Tier-0 years are byte-identical to R2's. The projected years grew by about 4–9 KB, rising with the dead-tree count (for example MIXED Year 17: 405,637 → 409,378 B; Year 26: 510,723 → 519,667 B).

**The tests catch a revert** (§6): MR2 (guard removed), MR3 (R2 browser-derived `DeadFallen`/`DeadStanding` restored) and MR4 (server sends `BiologicallyDead` for every projected dead tree) are each **KILLED** by both G7 `R2-2` checks. MR2 passes smoke (363/363), so only G7 covers the Pruning-lifts rule.

### R2-3 (SHOULD, spec accuracy plus latent retention breach): undocumented zero-history tier; tier list ≠ code. **CLOSED**

**Code** (`PlayerMultiApi.cs:564-615`). The four tiers are:

1. **Tier 0** (`:564-575`): full payload, returned only if raw ≤ 524,288 and `ecology` ≤ 153,600.
2. **Tier 1** (`:577-589`): projection with full untouched ground. Full history first; then `pruneLog` caps (full → 10 → 1 → 0); a binary search of the history tail never goes below `minEvents`/`minAnnual`, which are 1 whenever present.
3. **Tier 2** (`:591-600`): the same, plus ground compaction (`contributingTreeIds` omitted, never `[]`).
4. **Tier 3** (`:602-615`): `TailRaw(minEvents, minAnnual)`, `pruneLog` 0, `payloadBudgetExceeded: true` in both the bytes and the bounded record, and the `PAYLOAD BUDGET EXCEEDED …` stderr line.

What I confirmed:

- `grep` finds no `TailRaw(0, 0)`, `emergency`, `zeroView` or `zeroBytes` left.
- `exceeded: true` / `PayloadBudgetExceeded: true` occur only in Tier 3 (`:609`, `:615`).
- `TryFitTier` returns only candidates that passed the `<= MaxViewPayloadBytes` check, and returns nothing when `ecology` is over its cap.

So no path can return over-budget bytes without the flag and the log line, and no path drops history below 1/1.

**Spec and plan.**

- Spec §5 (`spec.md:180-187`) and plan §1.2(6) (`plan.md:44-46`) now list exactly Tier 0 → Tier 1 → Tier 2 → Tier 3 with the same content as the code: what each tier projects, the `pruneLog` cap order, the ≥1/≥1 retention in every tier, the flags each tier sets, and "no zero-history drop".
- No tier exists in the code that the documents leave out, and none in the documents that the code lacks.
- The Results-pane rule in the same section matches R2-1's code.

**Year 0..30 without the emergency tier.** Both G7 sweeps and both of my workloads are within budget at every year, with `payloadBudgetExceeded=false`, `stage4Compacted=false` and empty stderr through Year 30. In Years 1..30 the smallest history kept is 1 event and 1 annual record.

**Beyond the horizon** (informational; not a budget claim):

- **MA-1-MIXED** (my workload, which adds a pruned-stump Fell at Years 10–40):
  - Tier 2 starts at Year 31, and the tail reaches the 1/1 floor at Year 34.
  - It is flagged `payloadBudgetExceeded=true` at **every** year 35–50 (531,999 → 780,817 B).
  - It keeps **1 event / 1 annual record** at every flagged year.
  - There are **74** `PAYLOAD BUDGET EXCEEDED` stderr lines across GET and composite responses.
  - The flag was set if and only if raw or `ecology` was over its cap.
- **MA-1** stayed in Tier 1 to Year 50 (522,944 B; history first truncated at Year 50).

## 6. Mutation check of the new tests (disposable clone only)

Method:

- I made a sparse `git clone --shared` of the repository at `47b27a8d` under the external `TMPDIR`, copied `.p0ref` and verified its pin.
- For each mutant I made one exact single-site replacement in `PlayerMultiApi.cs` (asserting it occurs once), rebuilt the UI, ran the named suites and restored the file.
- The clone's `git status` was clean afterwards, and the clone was then deleted.
- Logs: `r3-opus-probes/mutation/`.

| Mutant | Change | Suites | Result |
|---|---|---|---|
| M0 control | none | smoke + G7 | **PASS** 363/363 and 67/67 |
| MR1 | R2-1 gate reverted to `totalRecordedYears > 0 \|\| historyTruncated \|\| hasAdvancedYear` | smoke + G7 | **KILLED**: smoke 361/363 (`R2a`, plus the J99 page-witness exit); G7 66/67 (`R2-1`) |
| MR1b | R2-1 gate = `totalRecordedYears > 0` only | smoke + G7 | **KILLED**: smoke `R2a`; G7 `R2-1` |
| MR2 | Pruning-lifts guard `tree.isLiving && ps` → `ps` | smoke + G7 | **KILLED** by G7 65/67 (both `R2-2` checks). Smoke survives 363/363 |
| MR3 | R2 inspector fallback restored (`DeadFallen`/`DeadStanding` plus suffix on stumps) | G7 | **KILLED** 65/67. The failure text shows "DeadFallen · DensitySuppression, year 27" and "FelledStump · fell, year 4" |
| MR4 | server `LifeStates.Of(tr)` → `LifeState.BiologicallyDead` | G7 | **KILLED** 65/67. The stump renders "BiologicallyDead · fell, year 4" |

R2's carried-over test-strength nit N-R2-1 still stands, and the handoff marks it deferred: the retention-floor and silent-over-budget mutants cannot fail inside Year 30. I checked those two behaviours only past the horizon (§5 R2-3).

## 7. Year 0..30 payload table (P1-UI-04, owner G5 horizon)

The values are the raw UTF-8 bytes of `GET api/multi/view` and of its `ecology` slice. I measured the slice from the raw text and checked it parses to the same object.

- **rerun:** the author's sweep (`run-step2-edge.sh`), re-executed by me in G7.
- **reviewer:** my independent `r3_probe.py`. It differs from the author's driver in these ways:
  - it fells the middle living MA-A tree;
  - on MIXED it also queues Prune, ClearUndergrowth, a beech Plant, Shelter and SpotClear;
  - from Year 10 on MIXED it also fells a previously pruned MA-B tree;
  - work runs at Years 0, 10, 20, 30 and 40;
  - each year is advanced by `POST api/multi/year/skip`.
- **ev/an:** events / annual records kept in the view.
- **flags:** `T` historyTruncated, `P` projectionCompacted, `S` stage4Compacted, `X` payloadBudgetExceeded.

Year 0 is measured after the Year-0 executed work.

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
| 10 | 422,341 / 37,073 | 2/10 | `----` | 422,226 / 37,075 | 2/10 | `----` | 429,561 / 37,955 | 2/10 | `----` | 430,724 / 38,180 | 2/10 | `----` |
| 11 | 425,212 / 36,335 | 2/11 | `----` | 425,107 / 36,338 | 2/11 | `----` | 435,963 / 37,994 | 2/11 | `----` | 436,251 / 37,815 | 2/11 | `----` |
| 12 | 427,587 / 35,901 | 2/12 | `----` | 427,221 / 36,029 | 2/12 | `----` | 457,675 / 41,736 | 2/12 | `----` | 457,746 / 41,672 | 2/12 | `----` |
| 13 | 437,445 / 37,291 | 2/13 | `----` | 437,314 / 37,275 | 2/13 | `----` | 475,601 / 44,496 | 2/13 | `----` | 475,761 / 44,142 | 2/13 | `----` |
| 14 | 445,400 / 37,815 | 2/14 | `----` | 445,226 / 37,803 | 2/14 | `----` | 486,253 / 45,141 | 2/14 | `----` | 485,223 / 44,487 | 2/14 | `----` |
| 15 | 447,337 / 36,953 | 2/15 | `----` | 446,093 / 36,712 | 2/15 | `----` | 501,355 / 46,008 | 2/15 | `----` | 500,534 / 45,794 | 2/15 | `----` |
| 16 | 452,940 / 36,618 | 2/16 | `----` | 451,716 / 36,363 | 2/16 | `----` | 512,449 / 44,917 | 2/16 | `----` | 515,069 / 45,819 | 2/16 | `----` |
| 17 | 457,005 / 36,255 | 2/17 | `----` | 455,752 / 35,996 | 2/17 | `----` | 409,378 / 46,439 | 2/17 | `-P--` | 411,594 / 47,343 | 2/17 | `-P--` |
| 18 | 464,663 / 35,885 | 2/18 | `----` | 463,087 / 35,992 | 2/18 | `----` | 424,738 / 48,378 | 2/18 | `-P--` | 424,235 / 48,751 | 2/18 | `-P--` |
| 19 | 473,135 / 35,578 | 2/19 | `----` | 469,493 / 35,195 | 2/19 | `----` | 440,197 / 49,685 | 2/19 | `-P--` | 441,788 / 51,129 | 2/19 | `-P--` |
| 20 | 476,298 / 34,214 | 3/20 | `----` | 473,607 / 34,333 | 3/20 | `----` | 451,237 / 49,228 | 3/20 | `-P--` | 451,829 / 50,126 | 3/20 | `-P--` |
| 21 | 486,057 / 35,317 | 3/21 | `----` | 483,739 / 35,333 | 3/21 | `----` | 459,513 / 48,112 | 3/21 | `-P--` | 460,896 / 49,605 | 3/21 | `-P--` |
| 22 | 490,756 / 34,282 | 3/22 | `----` | 489,468 / 34,528 | 3/22 | `----` | 466,592 / 45,898 | 3/22 | `-P--` | 469,930 / 48,015 | 3/22 | `-P--` |
| 23 | 495,290 / 33,672 | 3/23 | `----` | 493,970 / 33,907 | 3/23 | `----` | 479,252 / 46,379 | 3/23 | `-P--` | 477,866 / 46,843 | 3/23 | `-P--` |
| 24 | 499,271 / 33,319 | 3/24 | `----` | 497,241 / 32,939 | 3/24 | `----` | 493,828 / 46,895 | 3/24 | `-P--` | 492,052 / 47,419 | 3/24 | `-P--` |
| 25 | 504,081 / 32,822 | 3/25 | `----` | 504,564 / 32,856 | 3/25 | `----` | 506,865 / 47,034 | 3/25 | `-P--` | 508,174 / 48,633 | 3/25 | `-P--` |
| 26 | 512,286 / 33,195 | 3/26 | `----` | 512,423 / 33,308 | 3/26 | `----` | 519,667 / 46,825 | 3/26 | `-P--` | 520,657 / 48,207 | 3/26 | `-P--` |
| 27 | 516,961 / 32,112 | 3/27 | `----` | 518,728 / 32,971 | 3/27 | `----` | 522,866 / 47,475 | 3/18 | `TP--` | 522,971 / 48,627 | 3/19 | `TP--` |
| 28 | **524,279** / 32,749 | 3/28 | `----` | 403,217 / 33,595 | 3/28 | `-P--` | 522,457 / 46,028 | 3/12 | `TP--` | **523,849** / 48,183 | 3/12 | `TP--` |
| 29 | 406,036 / 32,108 | 3/29 | `-P--` | 407,273 / 32,365 | 3/29 | `-P--` | **523,375** / 46,109 | 3/8 | `TP--` | 522,046 / 48,829 | 3/6 | `TP--` |
| 30 | 413,142 / 32,650 | 4/30 | `-P--` | 412,402 / 31,152 | 4/30 | `-P--` | 520,582 / 48,441 | 2/2 | `TP--` | 521,180 / 49,202 | 2/2 | `TP--` |

All 124 measurements are within 524,288 B raw and 153,600 B `ecology`, with `stage4Compacted = false` and `payloadBudgetExceeded = false`.

**Maxima (Years 0..30):**

| Experiment | Workload | Max raw | Max `ecology` |
|---|---|---|---|
| MA-1 | rerun | **524,279 B** (Year 28, Tier 0, 9 B margin) | 43,320 B |
| MA-1 | reviewer | 518,728 B (Year 27) | 43,322 B |
| MA-1-MIXED | rerun | **523,375 B** (Year 29) | 49,685 B |
| MA-1-MIXED | reviewer | 523,849 B (Year 28) | 51,129 B |

The rerun maxima equal the reported values exactly.

**Other observations:**

- **Tier-0 rows are unchanged from R2.** MA-1 rerun Years 0–28 and MIXED rerun Years 0–16 are byte-identical to R2's table, because the delta adds bytes only to projected payloads.
- **The 9 B margin at MA-1 Year 28 is deterministic.** It is the Tier-0 cap check, which reproduced byte-for-byte.
- **The MIXED values of about 523 KB at Years 27–30 are the binary search filling up to the cap.**

The interactive Edge flows in G7 also logged within-budget values: MA-1 Year 30 410,490 B, and MIXED Year 30 522,504 B with history 3/4 and `T P`.

## 8. Handoff claim check (measurement integrity)

The handoff's §2 dispositions, §3 file lists and §4 Year 0..30 table all match what I measured:

- the 6-file delta;
- the 9-file cumulative list;
- 0 forbidden paths;
- every sweep row byte-identical;
- the maxima and the tier changes (MA-1 Tier 1 from Year 29; MIXED projection from Year 17; truncation from Year 27);
- the Year 30 tails (MA-1 4/30; MIXED 2/2).

The handoff says G6 was not rerun in repair #2. I reran it: **63/63**. The handoff's R2-2 evidence names the same trees G7 printed in my rerun. The only budget claim in the handoff, spec and plan is the owner's **Year 30** horizon. Plan §1.2(6) says "`ecology <= 153600` B across Years 0..30+". My runs agree up to Year 50 (max 56,875 B), but I treat this as an observation, not a guarantee.

## 9. New findings (R3)

**No new defect.** The repair introduced no regression I could find: all 7 gates pass, the probes in §5 pass, and a hygiene scan of the delta found:

- 0 capability URLs and 0 64-hex strings;
- 0 `data:image/webp` (the one textual hit is the handoff's own "0 occurrences" line);
- 0 inline `<script>`, 0 `style=`, 0 `innerHTML`/`eval`, 0 `http(s)://` additions;
- 0 new imports, packages or `PackageReference`s;
- 0 checkbox ticks.

**Nits (non-blocking; doc or informational only):**

- **N-R3-1 (doc drift, outside the R3 allowlist).** `specs/multi-2d-phase1-time-history-ui-v1/tasks.md:39` (T-06, the repair #1 record) still describes the superseded Results-pane condition: "…when `annualHistory` is empty while `totalRecordedYears > 0` or `historyTruncated`". Spec §5 and plan §1.2(6) carry the correct rule, and they govern. `tasks.md` was not in repair #2's writable allowlist. Fix it in a later docs pass.
- **N-R3-2 (wording).** The spec §5 dead-tree contract says Core's LifeState is preserved "identically across all tiers via `tree.death.lifeState`". In Tier 0, and for pruned dead trees in projected tiers, the inspector actually reads Core's `stage4.lifeState`, and Tier-0 view deaths carry no `lifeState`. The rendered text is identical; I verified it for every dead tree. Only the wording is imprecise.
- **N-R3-3 (informational).** The inspector keeps a fallback for a missing `death.lifeState`: `item.status === 'Felled' ? 'FelledStump' : 'BiologicallyDead'`. It uses Core's map status, so it does not invent a state, and it was never used in any measured payload. A server regression that sent a wrong value is caught (MR4). One that dropped the field would be hidden in the inspector by this fallback.
- **N-R3-4 (informational).** The new non-budget branch "Annual history rows are not included in this view…" (year above 0, not truncated, empty annual history) cannot occur in any measured state. Tier-0 annual history has *y* records at year *y*, and truncation keeps at least 1. No test covers it. Its text would be true if it were ever reached.
- **Carried over and deferred, as the handoff discloses:** N-R2-1 (retention-floor and over-budget mutants cannot fail inside Year 30) and N-R2-2 (dead-tree floats are rounded to 3 decimals but shown with 2). The R1 nits N-2 and N-5 are also deferred.
- **Pre-existing, out of scope:** `tests/phase1-save-ui/run-edge-e2e.sh` (unchanged) still writes unredacted capability URLs to its evidence logs. I redacted my R3 copies (6 files); 0 tokens remain.

## 10. Per-P1-UI verdicts (R3)

| Item | Verdict | Basis |
|---|---|---|
| P1-UI-01 Preview / Skip | **PASS** | G5/G7; `year/skip` exercised every year in my probe |
| P1-UI-02 History | **PASS** | G5/G7 paging, provenance pills and Escape. F-3 stays closed (G5 W-series) |
| P1-UI-03 Structure | **PASS** | G7: MA-1 `M_i=0.00`; MIXED `M_i>0`, DERIVED |
| P1-UI-04 Payload budget | **PASS** | Year 0..30 within both budgets on both experiments (two workloads). Flags are truthful. Never silent past the horizon. Projection-only. The Results-pane honesty clause holds (R2-1). Inspector behaviour is preserved under the documented dead-tree rule (R2-2). The spec tier list matches the code (R2-3) |
| P1-UI-05 Continuity | **PASS** | G7 Save → SIGINT → restart → Load matched; G6 63/63 |
| P1-UI-06 NB-2 | **PASS** | G7: orders 2 and 3 at 1280×720 and 1920×1080 |
| P1-UI-07 Edge matrix | **PASS** | G7: zero horizontal scroll, single-row topbar, R3 fold, Escape, zero console errors, CSP violations or 5xx. My own Edge runs: zero console errors |
| P1-UI-08 Hygiene | **PASS** | §9 scan; exec bits preserved; no forbidden paths |
| P1-UI-REG | **PASS** | All 7 gates (§3) |
| Gate G2 ACCEPTANCE | **PASS** | R2-1, R2-2 and R2-3 closed with real-Edge and mutation evidence |
| Gate G3 MEASUREMENT INTEGRITY | **PASS** | Budget numbers reproduced byte-identically and confirmed with a second workload. The handoff's spec/code-alignment claims are now accurate |

## 11. Residual risks

- **Thin margin at the horizon.**
  - At Year 30 the MIXED view keeps only **2 events / 2 annual records** under both workloads (R2 measured 4/6). The `lifeState` field costs about 4–9 KB of projected payload.
  - On my workload, Tier 2 starts at Year 31 and Tier 3 at Year 35 (R2 measured Year 36).
  - Heavier per-year work than either workload could reach Tier 2 or 3 inside 30 years. It would be flagged and logged, and the 1/1 floor would hold.
- **Latency.** The bounded serializer can run several dozen serializations in the worst case. I did not measure latency, and it was not noticeable in the gates.
- **Pruning history of dead trees.** The pre-decided rule hides a dead tree's Pruning-lifts history in the inspector, including a pruned stump's lifts and crown base. The data stays in the payload; the History drawer and `pruneLog` are unaffected. This is an owner-accepted UX choice, recorded here for the playtest.

## 12. NOT TESTED

- The empty-annual-history form of the "Compact view omits…" text with real data. It cannot occur under the retention floor, so only the injected-fault smoke witness `S2j` covers it.
- The over-budget notice text in the real UI past the horizon ("exceeded the 512 KiB / 150 KiB transport budget"). I verified the flag and stderr over HTTP only.
- The new non-budget Results branch (N-R3-4): no reachable state.
- Composite-endpoint latency.
- Unity: not applicable. This is a standalone lab with no `Assets`, `Packages` or `ProjectSettings` change.

## 13. End-of-review state

- **Worktree and branches.**
  - HEAD stays `47b27a8d` until this commit.
  - Before this commit, `git status --short` showed only this new file.
  - Remote feature `47b27a8d` and `main` `cc2757a7` are unchanged.
  - The `.p0ref` pin is exact.
  - The gates built `bin/`/`obj/` inside the worktree as briefed. These are git-ignored, and `git status` stayed empty.
- **Processes.**
  - Listeners `:33767` (1750123), `:43649` (1806361) and `:46593` (839989) are LISTEN.
  - The same 13 `sitkap1-ui.dll` PIDs as at start remain, all from other lanes.
  - Zero processes reference this lane, its scratch or its scripts. Zero `msedge` processes or temp profiles from this lane (`opus-r3-edge*`) remain.
  - Every server I started stopped on SIGINT with rc 0 and 0 bytes of stderr (except the deliberate beyond-horizon `PAYLOAD BUDGET EXCEEDED` lines).
- **Scratch.** The disposable mutation clone and `TMPDIR` (`/home/jer/ccf-p1ui-r3-tmp`) were removed.
- **Evidence.** Stored outside Git with mode `go-rwx`, under `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125/`:
  - `r3-gates/`, `r3-smoke/`, `r3-edge-save/`, `r3-edge-step2/`;
  - `r3-opus-probes/{http,edge,edge-ma1y0,mutation,scripts}/`.

  Capability URLs were redacted, and 0 remain. `edge/run-attempt1.log` keeps my first `ma1y0` attempt, which failed on my own driver's selector (`#approve` is reached via `#view-plan`), not on the app. The corrected run is in `edge-ma1y0/`.

## 14. Verdict and integration decision

```text
INTEGRATION DECISION
Status: APPROVED — IMPLEMENTED ON TASK BRANCH — INDEPENDENTLY REVIEWED APPROVED — READY FOR COMBINED PHASE 1 OWNER PLAYTEST — NOT INTEGRATED, NOT ACCEPTED
Gate: G2 ACCEPTANCE PASS; G3 MEASUREMENT INTEGRITY PASS; G4 (integration) not started
Reviewed branch: feat/multi-2d-phase1-time-history-ui-20261125-v1
Reviewed HEAD: 47b27a8d3873b32726f315ce1182118da38c50b1 (delta from adea7af220259a1808346cd5381babed3e9052fa)
Context commit: 5fe2fabe9073d7b72711f4ab48553c785351d7bc
SPEC: SPEC-multi-2d-phase1-time-history-ui-V1 / IMPLEMENTING — Conformance: PASS (P1-UI-01..08, REG)
R2 findings: R2-1 CLOSED; R2-2 CLOSED (Core LifeState across tiers; dead-tree Pruning-lifts row omitted in all tiers per pre-decided rule, documented spec §5); R2-3 CLOSED (zero-history tier removed; spec §5 / plan §1.2(6) = code Tier 0→1→2→3); N-R2-3 CLOSED
Gates on 47b27a8d: G1 0W/0E; G2 Core 277/277 (9/9 anchors with pin); G3 Save 50/50; G4 UI 0W/0E; G5 smoke 363/363; G6 save Edge 63/63; G7 Step 2 Edge 67/67
Year 0..30 maxima: MA-1 524,279 B (Y28) / ecology 43,320 B; MA-1-MIXED 523,375 B (Y29) / ecology 49,685 B (reviewer workload: 518,728 / 43,322 and 523,849 / 51,129)
Mutation: MR1, MR1b, MR2, MR3, MR4 all KILLED; control PASS
New findings: none blocking; nits N-R3-1..4 (doc/informational)
Next: combined Phase 1 owner playtest; integration only after Manager/owner decision
Integration target: none (NOT MERGED, NOT INTEGRATED, NOT OWNER-ACCEPTED)
```

**NOT MERGED. NOT INTEGRATED. NOT OWNER-ACCEPTED.**
