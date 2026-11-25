# CCF Multi 2D — Phase 1 Step 2 (Time Pacing, History & Structure UI/HTTP + NB-2): Independent Review

**Verdict: NEEDS CHANGES**

> **NOT MERGED. NOT INTEGRATED. NOT OWNER-ACCEPTED.** This is an independent review record only. No merge, PR, tag, force-push, `main` write or feature-branch write was made. The requirements checklist was not ticked by this review; the Manager owns any checklist update.

| Item | Value |
|---|---|
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`), independent non-author reviewer, T3 Code / Claude Code harness |
| Implementer | Gemini 4 Neon (`antigravity_2`), a different provider and thread |
| Separation | No implementation role. No source, test, spec or implementation-handoff edits. No subagents. The only repository write is this new Markdown file on the review branch |
| Review worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-Review-20261125` |
| Review branch | `review/multi-2d-phase1-time-history-ui-20261125-v1` |
| Feature branch / HEAD reviewed | `feat/multi-2d-phase1-time-history-ui-20261125-v1` @ `513355c4bcef881413d517aa62feed4676234319` |
| Base | `5fe2fabe9073d7b72711f4ab48553c785351d7bc` (Step 1 Core, independently APPROVED at `6c83d16e`) |
| Spec | `specs/multi-2d-phase1-time-history-ui-v1/` (`SPEC-multi-2d-phase1-time-history-ui-V1`, status `IMPLEMENTING`) |
| Date | 2026-11-25 |

## 1. Summary

Most of the Step 2 work is sound, and I reproduced the author's gate results exactly. The five endpoints, the guard chain, stale-fence handling (zero mutation), the `WorkPlanOpen` refusal (no cancellation), the Skip UI, the History drawer, the Structure section, the NB-2 fix and Save → quit → restart → Load continuity all held under my own probes.

**P1-UI-04 fails.** The 512 KiB `GET api/multi/view` budget and the 150 KiB `ecology` budget do not hold over multi-decade runs:

- **MA-1-MIXED:** `GET api/multi/view` exceeds 524,288 B from **Year 24** and reaches 1,295,498 B at Year 50. `ecology` exceeds 153,600 B from **Year 38**.
- **MA-1:** exceeds the view budget from **Year 42**.
- **Silent over-budget return:** `SerializeBoundedMultiView` returns the over-budget bytes with no signal.
- **History dropped first:** just before the breach, the serializer drops **every** event and annual record, against spec §5. The Results pane then falsely tells the player "No ecological year has been advanced in this session." I confirmed this in real Edge at Year 23.

The author's evidence stopped at Year 20 (MA-1-MIXED) and Year 26 (MA-1). Their Edge driver stops at the first truncation, so it never observed the breach.

## 2. Start-of-review checks (exact results)

| # | Check | Result |
|---|---|---|
| 1 | `git rev-parse HEAD`; `git status --short` | `513355c4bcef881413d517aa62feed4676234319`; status **empty** (checked at start and again before writing this file) |
| 2 | `sha256sum Tools/MultiRackSitkaP1Lab/.p0ref/SitkaLab.Core.dll` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`: **exact**. Not rebuilt |
| 3 | `git diff --name-only 5fe2fabe..HEAD` | **Exactly the 9 expected paths**: the Neon implementation handoff, `src/Ui/PlayerMultiApi.cs`, `tests/phase1-time-history-ui/drive-step2.mjs`, `tests/phase1-time-history-ui/run-step2-edge.sh` (new, mode 100755), `tests/ui-smoke-multiarea.sh`, and the four `specs/multi-2d-phase1-time-history-ui-v1/` files. No other path |
| 4 | Forbidden paths (`src/Core/**` incl. Phase1, `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/coordination/`), plus `src/Ui/wwwroot/` | **Zero** diff paths |
| 5 | `git ls-remote` | Feature `513355c4bcef881413d517aa62feed4676234319` = reviewed HEAD. `main` = `cc2757a730ad9451e3d6e3680dffd123171a5df5` (unchanged) |
| 6 | Protected listeners | `127.0.0.1:33767` (pid 1750123), `:43649` (pid 1806361), `:46593` (pid 839989) all **LISTEN** at start and at end |
| 7 | No ephemeral server or headless Edge from this lane | At start, 13 `sitkap1-ui.dll` processes from other lanes (including the three protected ones); none from this lane. At end, the **identical 13 PIDs**, zero lane `msedge.exe`, and no leftover scratch directories. Every server I started stopped on SIGINT with rc 0 |

## 3. Gates rerun by the reviewer

Environment: WSL2 Ubuntu, `PATH="$HOME/.dotnet:$PATH"` (`dotnet 8.0.134`; system dotnet 10 not used), `TMPDIR=/home/jer/ccf-p1ui-review-tmp` (outside the repo), Linux node v26.13.1, Windows node v26.13.1, Windows Microsoft Edge `--headless=new`.

| Gate | Command (from `Tools/MultiRackSitkaP1Lab`) | Exact result |
|---|---|---|
| G-1 | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | Build succeeded, **0 Warning(s), 0 Error(s)** |
| G-2 | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **277/277 passed, 0 failed**. All 8 test-printed anchors present in the log (`7e3a2a86…`, `a692a6d5…`, `814e8165…`, `8a926776…`, `c4b670ed…`, `c67edc6b…`, `776a68df…`, `93e9abda…`), plus the `.p0ref` pin exact = **9/9 anchors** |
| G-3 | `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | **50/50 passed, 0 failed**. No warning or error lines in the build/run output |
| G-4 | `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | Build succeeded, **0 Warning(s), 0 Error(s)** |
| G-5 | `TMPDIR="$TMPDIR" tests/ui-smoke-multiarea.sh …/review-smoke` | **OVERALL: PASS (352 passed, 0 failed)**. Z03/Z04: lab tree and git status unchanged |
| G-6 | `tests/phase1-save-ui/run-edge-e2e.sh …/review-edge-save` | **OVERALL: PASS (63 passed, 0 failed)** |
| G-7 | `tests/phase1-time-history-ui/run-step2-edge.sh …/review-edge-step2` | **OVERALL: PASS (59 passed, 0 failed)**: `ma1-full` 29/29, `mixed-play-save` 19/19, `mixed-restart-load` 5/5, plus 6 SIGINT/stderr checks |

These counts match the author's handoff exactly. `git status --short` was empty after every gate.

## 4. Independent probes written by the reviewer (not committed)

I wrote these outside the repository. Scripts and outputs are archived (mode 700, outside Git) at `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125/review-opus-probes/`, in subfolders `scripts/`, `http/`, `edge/` and `gates/`.

- **`probe_http.py`**: a Python HTTP probe against fresh servers on each experiment. It covers:
  - guard edge cases;
  - stale fences on all three fenced endpoints, with byte-identical view before and after;
  - `WorkPlanOpen` for non-Fell families;
  - `InvalidTargetYear`;
  - a Year 0–50 run with executed work, measuring the budget every year from Year 15;
  - Save → SIGINT quit → restart → Load continuity at Year 50, with byte comparison.
- **`probe_edge.mjs`**: a dependency-free Edge/CDP probe. Clicks are real mouse events, refused unless `elementFromPoint` hits the target. It covers:
  - the topbar at its longest Skip labels;
  - NB-2 at order 3 with the pane pre-scrolled, and at order 2 (Shelter);
  - Load over a live session with History and Structure open;
  - HTML injection;
  - Escape;
  - the Results pane when the view carries no history.
- **`probe_composition.py`**: a byte breakdown of the view at Years 0, 24 and 50.

My probe workload: on MA-1-MIXED, the six action families (Fell, Prune, ClearUndergrowth, Plant, Shelter, SpotClear) were executed at Years 0, 10, 20, 30 and 40. On MA-1, a Fell was executed at the same years.

## 5. Measured payloads (P1-UI-04)

### 5.1 Author checkpoints, reproduced

My G-7 rerun printed byte-identical values to the handoff:

| Point | MA-1 view / ecology (B) | MA-1-MIXED view / ecology (B) |
|---|---|---|
| Year 0 | 389,762 / 43,428 | 391,026 / 44,371 |
| Year 5 | 414,220 / 45,444 | 419,173 / 47,668 |
| Year 15 | 458,842 / 50,576 | 516,691 / 63,723 |
| Year 20 (MIXED, compact `stage4`) | — | 511,193 / 80,223 |
| Year 26 (MA-1, `historyTruncated: true`) | 522,939 / 59,835 | — |

### 5.2 Reviewer per-year measurement past Year 15

Raw UTF-8 bytes of `GET api/multi/view`. The "view keeps annual/events" column shows how many records the bounded view returned, against the totals in the Core root.

**MA-1-MIXED:**

| Year | Raw view (B) | ecology (B) | `historyTruncated` | View keeps annual/events | Compact `stage4` | Within budget |
|---|---|---|---|---|---|---|
| 15 | 518,007 | 64,133 | false | 15/2 of 15/2 | no | yes |
| 16 | 523,096 | 65,255 | **true** | 10/2 of 16/2 | no | yes |
| 17 | 443,052 | 68,565 | **false** | 17/2 of 17/2 | **yes** | yes |
| 20 | 512,317 | 81,197 | false | 20/3 of 20/3 | yes | yes |
| 22 | 522,626 | 85,060 | true | 5/3 of 22/3 | yes | yes |
| **23** | 522,792 | 88,184 | true | **0/0** of 23/3 | yes | yes (no history kept) |
| **24** | **542,396** | 91,821 | true | 0/0 | yes | **NO** |
| 30 | 686,204 | 118,368 | true | 0/0 | yes | **NO** |
| **38** | 900,897 | **156,635** | true | 0/0 | yes | **NO (view and ecology)** |
| 40 | 970,692 | 170,280 | true | 0/0 | yes | **NO** |
| 50 | **1,295,498** | **226,776** | true | 0/0 | yes | **NO** |

**MA-1:**

| Year | Raw view (B) | ecology (B) | `historyTruncated` | View keeps annual/events | Compact `stage4` | Within budget |
|---|---|---|---|---|---|---|
| 24 | 521,831 | 57,598 | false | 24/3 of 24/3 | no | yes |
| 25 | 523,328 | 58,842 | true | 20/3 of 25/3 | no | yes |
| 30 | 471,250 | 64,339 | false | 30/4 of 30/4 | yes | yes |
| 41 | 524,258 | 81,236 | true | 2/2 of 41/5 | yes | yes |
| **42** | **529,089** | 82,244 | true | **0/0** | yes | **NO** |
| 50 | **602,575** | 94,754 | true | 0/0 | yes | **NO** |

I reproduced the zero-history point independently in Edge with a minimal workload (a single Year-0 Fell, MA-1-MIXED). The view had 0 annual records and 0 events at Year 23, at 522,331 B.

Other observations from this run:

- The flag was truthful (it matched the view counts) and `totalRecordedYears` equalled `history/index` at every measured year.
- The keyset walk over `history/index` returned every year 50..0 exactly once.
- `history/year` for Year 0 stayed fully readable at Year 50. The truncation is projection-only, as specified.

### 5.3 Where the bytes are

`probe_composition.py`, MA-1-MIXED with a single Year-0 Fell:

| Year | Total | `view.areas[].trees` | `scenes` | `stage4` (trees / ground / rest) | `ecology` (of which `pruneStatusByTreeId`) | `events` + `annualHistory` | Tree records (living) |
|---|---|---|---|---|---|---|---|
| 0 | 392,395 | 34,715 | 50,852 | 248,654 (73,742 / 171,316 / ~3.6 K) | 44,439 (28,669) | 1,175 | 244 (243) |
| 24 | 545,865 | 90,592 | 98,384 | 253,141 (81,082 / 110,805 compact / ~61 K) | 91,175 (71,746) | **4** | 427 (175) |
| 50 | 1,306,803 | 264,801 | 254,968 | 549,268 (216,735 / 110,578 compact / ~222 K) | 225,206 (194,702) | **4** | 1,072 (138) |

The growth comes from per-tree records for trees that are no longer living, plus recruited broadleaves. It does not come from the history lists the spec's tail rule trims. So the bound cannot hold past Year 23 (MA-1-MIXED) or Year 41 (MA-1) under the spec's own rule.

### 5.4 Unbudgeted responses (informational)

At MA-1-MIXED Year 50:

| Response | Bytes |
|---|---|
| `POST api/multi/year/skip` | 1,290,576 |
| `POST api/multi/year/preview-review` | 339,696 (the page fetches this after every mutation) |
| `POST api/multi/load` | 1,681,085 |
| `POST api/multi/history/year` | 7.5–13.7 KB |

## 6. Real Edge matrix (P1-UI-07)

- **Author suite (G-7), rerun by me.** All four cells passed: 1920×1080 and 1280×720 on both MA-1 and MA-1-MIXED.
  - Horizontal scroll: zero (`1920<=1920`, `1280<=1280`).
  - Topbar: single row (h=78/53 px, delta 7/3 px).
  - R3 fold at 1280×720: `scrollTop=0`, 5 rows, `addBottom=632<=712`.
  - Escape preserved one draft Fell tick.
  - Console errors, exceptions, CSP violations and 5xx: zero.
- **Reviewer topbar probe** at the longest labels, at both sizes:
  - `Skip → Yr 4 (Obs.)` / `Quiet obs. · Work plan open · GAMEPLAY CALIBRATION` (Year 0, Pending work);
  - `Skip → Yr 14 (Obs.)` with the same sub-label (Year 10).

  Result: `scrollW=1280<=1280`, `maxRight=1264`, h=53 px, delta 6 px, no clipped control. At 1920: 1896 px, h=78 px.
- **Skip click at Year 10 with Pending Fell.** I captured network events. The page showed an inline `WorkPlanOpen` refusal with "never cancels open work" and `GAMEPLAY CALIBRATION`. **No `year/skip` request was sent**, and the Pending order was intact.
- **Console.** Every reviewer Edge run had zero console errors, exceptions, CSP violations or 5xx.

## 7. NB-2 proof (P1-UI-06)

- **Author suite, rerun.** At 1280×720 and 1920×1080: `scrollTop=0`, `Phase2DuplicateOrder` and `Already queued` shown, and both Remove buttons inside the pane and viewport. Real clicks removed both orders.
- **Reviewer probe, a different area and state.** MA-1-MIXED, area **MA-B**, at Year 10. Every element was hit-tested with `elementFromPoint`.
  - **Order 3 (SpotClear), pane pre-scrolled first** (`#pane-tree.scrollTop=93` at 1280×720). After the partial refusal, `scrollTop=0`. The `Phase2DuplicateOrder` code, the `Already queued` label and both Remove buttons were the top-most hit targets at **1280×720 and 1920×1080**. Real clicks removed **exactly** the two partial orders on the server; the pre-queued SpotClear was kept.
  - **Order 2 (Shelter).** Setup: I queued Plant+Shelter at P, then removed the Plant, leaving a pending Shelter at P. Re-queueing Plant+Shelter+SpotClear at the same screen point stopped at **Shelter** with `Phase2DuplicateOrder` and one queued Plant. The code, `Already queued` and the Remove button were top-most hit targets with `scrollTop=0` at **both** sizes, and Remove worked. Screenshots: `edge/nb2-o2-1280x720.png` and `edge/nb2-o2-1920x1080.png`.

**P1-UI-06: PASS** (independently reproduced at orders 2 and 3, at both sizes).

## 8. Save / restart / Load continuity (P1-UI-05)

- **Author suite (G-7).** Save at MA-1-MIXED Year 15, SIGINT, restart a fresh process, then Load through the UI. The restored preview, history index and Year 0 detail, and structure (`M_i=0.3790`, `T_i=0.2594`, n=197) all matched.
- **Reviewer HTTP probe**, both experiments at **Year 50**, with history truncation active.
  - **Sessions and fences.** The restarted process came up at Year 0 under a new `sessionId`. Load restored Year 50 under another new `sessionId`. The pre-load fences, from both the fresh process and the pre-save session, were refused with 409 `PlayerSessionMismatch`.
  - **Byte-identical before and after the restart:**
    - `history/index?pageSize=50`;
    - `history/year` for years 0, 1, 10, 25, 49 and 50;
    - `structure`;
    - `preview-review`, apart from its `fence` and `view` fields.
- **Reviewer Edge probe: Load over a live session** (MA-1).
  - **Setup.** The Year-0 Fell's reason contained HTML. I skipped to Year 4 and saved `opus-early`, then skipped to Year 8 (the summary was shown) with Structure open in Details. I opened the History drawer at 9 years and selected Year 0. Then I loaded `opus-early` with the drawer open.
  - **History drawer.** The index refreshed to `[4,3,2,1,0]` and showed "5 recorded years". The stale Year-8 detail was cleared.
  - **Skip.** The summary was cleared, and the label refreshed to `Skip → Yr 8` with `Review (overdue)`.
  - **Structure.** It re-read for the restored stand: n 222 against 198, and T_i 0.1726 against 0.1605.
  - **Fence.** No request after the Load carried the old `sessionId`.
  - **Escape.** It closed the drawer and returned focus to `#history-open`.

**P1-UI-05: PASS.**

## 9. Endpoints, guards and fences (P1-UI-01/02/03), reviewer HTTP probe

- **Guards:**
  - POST without `Origin` → 403. Foreign `Origin` (another loopback port), and a foreign `Origin` on a GET, → 403. Wrong `Host` → 403.
  - Wrong or near-miss capability → 404.
  - A body of exactly 16,384 B is accepted by preview-review, history/year and skip. A body of 16,385 B → **413** on all three.
  - Unknown nested `fence.source.extra`, a duplicate nested key, a missing `fence.source.workRevision`, or a case-variant duplicate `Fence` → 400 on all three POST routes. Null `year` → 400. `targetYear` beyond int32 → 400.
- **`history/index` query strictness.** Each of these → 400: `+5`, empty value, bare key, overflow value, `PageSize`, `1.5`, `' 5'`, `pageSize&pagesize` duplicate, `-0` for `beforeYear`, and `0x5`. `pageSize=-5` → 200, clamped to 1. `beforeYear=0` → 200. `structure?` (empty query string) → 200.
- **Stale fences, at Year 0 and at Year 50.** I sent 12 probes each time (preview, skip and history/year × session, workRevision, ecologicalYear and planStateRevision). Every one returned 409 with the exact code (`PlayerSessionMismatch` / `PlayerSourceStale` / `PlayerPlanStateStale`) plus a fresh fence and view. The view was **byte-identical before and after**. The read-only fence check on `history/year` (`PlayerMultiApi.cs:275-296`) mirrors Core `RequireFence`. `MultiAreaSourceToken` is scalar-only, so record equality is exact and the check cannot be bypassed.
- **`WorkPlanOpen`.** Pending Prune, Pending ClearUndergrowth, Pending Plant and an Approved plan each gave preview `eligible:false` and skip 409 `WorkPlanOpen`, and the view stayed byte-identical (open work untouched).
- **`InvalidTargetYear`.** `targetYear` = current, current−1, current+6, `int.MinValue` and `int.MaxValue` → 409 with zero mutation.
- **`consentToCancelWork`.** The diff never passes `consentToCancelWork = true`. Skip calls `AdvanceYears`, which hard-codes `consentToCancelWork: false` (Core `MultiAreaTimePacing.cs:317`).
- **CSP.** The header on `/multi` equals the spec string exactly. It is unchanged in `PlayerApi.cs:33` (not in the diff).
- **Defect.** `history/year` with `year = 2147483647` → **500 `OutcomeUnconfirmed`**, and the server writes `OverflowException` to stderr. See F-3.

## 10. Per-item verdicts

| Item | Verdict | Basis |
|---|---|---|
| P1-UI-01 Preview / Skip | **PASS** (nit N-1) | §6, §9 and G-5/G-7. Year 0 `Obs.` +4, post-Fell `Y+4`, overdue fallback, `WorkPlanOpen` inline refusal without a skip request, stale-fence 409, `GAMEPLAY CALIBRATION` and quiet observation labels, stepped-years summary |
| P1-UI-02 History | **PASS except one out-of-range case** (F-3) | Paging over 51 years, WorkYear Y against ObservedYear Y+1, six provenance pills, the three NOT RECORDED disclosures, the exact non-causal note with no weather, zero economy rows, and Escape keeping the drafts. `year=2147483647` → 500, not 409 |
| P1-UI-03 Structure | **PASS** | MA-1 `M_i=0.00`; MA-1-MIXED `M_i=0.3790`. `DERIVED` pill and exact disclaimer shown. Values come from the server; `formatStructureCell` only formats. The live `null → Unavailable` path is NOT TESTED (unreachable in live play); it is covered only by smoke S2f and Core P1-TH-06b |
| P1-UI-04 Payload budget | **FAIL** (F-1, F-2) | §5.2–§5.3 |
| P1-UI-05 Continuity | **PASS** | §8 |
| P1-UI-06 NB-2 | **PASS** | §7 |
| P1-UI-07 Edge matrix | **PASS** | §6 |
| P1-UI-08 Hygiene | **PASS** | Zero capability tokens in the diff (`/s/<64hex>` count 0). The only 64-hex strings are the 9 published anchors and the pin. Zero `data:image/webp`, inline `<script>` or `style=` in source. No `innerHTML` or `eval` added. No dependency or `csproj` change. `wwwroot` and Core untouched. 0 checklist boxes ticked |
| P1-UI-REG | **PASS as rerun** | All counts in §3 match. The suite does not detect F-1, F-2 or F-3 (F-5) |
| Gate G2 ACCEPTANCE | **FAIL** | P1-UI-04 unmet. The spec §5 tail rule is violated (F-2). There is a D-030 state-honesty defect (F-2) |
| Gate G3 MEASUREMENT INTEGRITY | **FAIL for P1-UI-04** | The measurements reported are accurate (reproduced byte-identically) but do not support the claim. The evidence stops at Year 20/26, and `drive-step2.mjs:496` stops at the first truncation. The handoff's residual-risk statement (§8, "runs through Year 20–26 remain comfortably under both caps", where it predicts only ecology growth past Year 30) does not disclose the raw-view breach at MA-1-MIXED Year 24 |
| G3 honesty labels (P1-TH-07 sense) | PASS, except the false Results-pane statement in F-2 | |

## 11. Findings (by severity)

### BLOCKER

**F-1 — The view budget is not bounded over multi-decade runs, and over-budget bytes are returned silently**

- **Where:** `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs:390-530` (`SerializeBoundedMultiView`). The final fallback at `:508-529` returns `emptyBytes` or `trimmedBytes` whether or not they fit.
- **Observed:**
  - MA-1-MIXED raw view 542,396 B at Year 24, rising to 1,295,498 B at Year 50.
  - MA-1-MIXED ecology 156,635 B at Year 38, rising to 226,776 B at Year 50.
  - MA-1 raw view 529,089 B at Year 42, rising to 602,575 B at Year 50.
- **Spec:** §2 ("keeping `GET api/multi/view` strictly bounded over multi-decade runs"); §5 (budgets "at Year 0, Year 5, and Year 15+"); plan §1.2(6) ("until raw UTF-8 bytes <= 524288").
- **Reachability:** five to ten Skip presses.
- **Cause (§5.3):** non-living per-tree records accumulate in `view.areas[].trees`, `scenes`, `stage4.areas[].trees` and `ecology.pruneStatusByTreeId`. At MA-1-MIXED Year 50 there are 1,072 records, 138 of them living, together about 930 KB.
- **Why this needs a decision:** these sections are not covered by the spec's projection rule, and the spec requires "existing field names and UI expectations remain unchanged". Closing the gap may need an owner/Manager decision on the horizon or the projection rules (G5).

### MAJOR

**F-2 — The zero-history fallback breaks spec §5 and makes the UI state something false**

- **Where:**
  - `PlayerMultiApi.cs:508-516`: when even one event plus one annual record with compact `stage4` overflows, the code returns `TailView(0)` before trimming `stage4.pruneLog`.
  - `PlayerMultiApi.cs:5631`: `renderHistory()` then shows "No ecological year has been advanced in this session." next to "Compact view shows the most recent tail only; open History in the header for all 24 recorded years."
- **Observed:** zero annual records and zero events in the view at MA-1-MIXED Year 23 (both the reviewer workload and the single-Fell workload) and at MA-1 Year 42.
- **Edge evidence:** `edge/results-pane-zero-history-y23-1280x720.png`.
- **Spec:** §5 requires "preserving at least the latest work event and latest annual record whenever present", and trims history before `stage4.pruneLog`. The Results statement is a D-030 state-honesty failure.

### MINOR

**F-3 — An out-of-range history year returns 500 instead of 409**

- **Where:** `PlayerMultiApi.cs:299-310` guards only `year < 0`. Core `MultiAreaHistoryQuery.ReadYear` computes `checked(year + 1)`.
- **Observed:** `POST history/year {year: 2147483647}` gives 500 `OutcomeUnconfirmed`, and stderr shows `OUTCOME UNCONFIRMED POST api/multi/history/year: OverflowException`. That stderr line would also fail the suites' own "server wrote nothing to stderr" checks.
- **Spec:** §4.4 and the checklist require 409 for an "unrecorded or out-of-range year".
- **Reach:** the UI never sends such a year, so this affects the API contract only.

**F-4 — The compact `stage4` projection departs from the spec and is not flagged**

- **Where:** `PlayerMultiApi.cs:532-571`.
- **What changes in compact mode:**
  - ground vertices lose `xm`/`zm`;
  - every `contributingTreeIds` becomes `[]` (a false empty list, not an omission);
  - null tree fields are omitted;
  - prune refusal code and reason are nulled;
  - `pruneLog` is capped at 20 while the serialized `historyTruncated` ignores that cap (`:487` uses `isTrunc`).
- **Observed:** at MA-1-MIXED Years 17–20 the view serves compact `stage4` with `historyTruncated:false`. Behaviour is also non-monotonic: Year 16 keeps full `stage4` and trims history to 10/16; Year 17 keeps full history with compact `stage4`.
- **Impact today:** none visible. I checked that the cell inspector uses `POST api/multi/cell`, that prune status prefers `ecology.pruneStatusByTreeId` (which covers every tree, `MultiAreaEcology.cs:351-368`), and that SVG drawing uses only `u`/`v`.
- **Status:** disclosed as handoff Deviation 2, but contrary to spec §5 ("field names … unchanged") and not approved.

**F-5 — Tests that pass without catching the gaps above**

- `drive-step2.mjs:496` stops at the first truncation, and no test measures past Year 20/26.
- No test asserts that at least one event and one annual record are retained, or checks the Results-pane text in the zero-history state.
- `ui-smoke-multiarea.sh:789` covers only `PlayerSessionMismatch` on `history/year`. A mutant that dropped the Source or PlanState checks (`PlayerMultiApi.cs:284-289`) would survive the suites; my probe shows the current code is correct.
- P1-UI-05 loads into a fresh page, so the in-page reset (drawer, structure, Skip summary) on Load over a live session is not exercised by the author's suite. My probe passes it.
- NB-2 order 2 is not covered by the author's suite. My probe passes it.

**F-6 — Stale structure or history responses can be stored as current**

- **Status:** code inspection only, not reproduced.
- **Structure:** `PlayerMultiApi.cs:6978` records `structureRootTag` at **response** time. `applyPayload` (`:3021-3024`) clears `state.structure` on a tree-tag change but does not bump `structureRequest` unless Details is active. A structure response already in flight before a mutation can then be stored as current. `showPane('details')` will not refresh because `state.structure` is non-null.
- **History:** `loadHistoryIndex(null,false)` resets the selected detail but does not bump `historyDetailRequest`, so a late `history/year` answer can repopulate the pane.

### NIT

- **N-1:** `PlayerMultiApi.cs:6521` computes "overdue" in JavaScript (`anchorFellYear + (suggestedGapYears || 4) <= currentYear`). That is pacing math in the browser (D-020). Core already distinguishes `OverdueReviewPeriodLabel` in `label`.
- **N-2:** The comment at `PlayerMultiApi.cs:153-161` still says "One atomic composite read under one Core lock". `totalRecordedYears` and `reviewPreview` are read under separate locks, so a race can mismatch `totalRecordedYears` or turn `reviewPreview` into null. The `history/year` fence check and the `ReadHistoryYear` snapshot are also separate locks; that read is read-only, so the risk is low.
- **N-3:** Command, load and skip payloads do not carry `historyTruncated`. A stale "Compact view …" note can therefore stay visible after a command returns a full view. Command, load, skip and preview responses are unbounded (§5.4), and `preview-review` (≈340 KB at Year 50) is fetched after every mutation.
- **N-4:** The Later Observed Comparison card head adds a `SESSION-ONLY` pill to rows that are CURRENT/DERIVED (`:6918`). The Observed Outcome pill is hard-coded `RECORDED` (`:6855`) rather than read from `annualOutcomeNextYear.provenance`.
- **N-5:** The History drawer is `aria-modal="false"` (`:1833`) with a full-screen backdrop and no focus containment. Keyboard focus can reach controls behind it.
- **Pre-existing, out of scope:** `tests/phase1-save-ui/run-edge-e2e.sh` (unchanged here) writes unredacted capability URLs to its evidence `run.log` and `server-*.out`. The tokens are dead and the folder is outside Git. I redacted my review copies.

## 12. Minimal repair list

1. **(F-1; Manager/owner decision first, G5)** Set the multi-decade horizon P1-UI-04 must hold to. I recommend at least Year 50, which is 10 Skip presses. Then choose one of these:
   - **(a)** Add further projection-only reductions that keep UI behaviour, for example dropping non-living trees from `ecology.pruneStatusByTreeId` and `stage4.areas[].trees`, and keeping fewer fields for dead stems in the per-tree arrays. Then re-measure `GET api/multi/view` and `ecology` every year to the horizon on both experiments, with executed work.
   - **(b)** If (a) is infeasible under "field names/UI expectations unchanged", get a spec amendment (horizon, cap or projection rule) before claiming PASS.

   In every case, `SerializeBoundedMultiView` must never silently return more than 524,288 B. Make the over-budget case explicit, for example an additive `payloadBudgetExceeded` field plus a server log line, and disclose it in the handoff.
2. **(F-2)** In `SerializeBoundedMultiView`, keep the latest work event and the latest annual record whenever they exist, and trim `stage4.pruneLog` before that. In `renderHistory()`, when `annualHistory` is empty but `totalRecordedYears > 0` (or `historyTruncated`), do not say no year was advanced; point to the History drawer instead.
3. **(F-3)** In `ReadMultiHistoryYearAsync`, return 409 `HistoryYearNotRecorded` for years beyond the current ecological year before calling Core, so `year = 2147483647` no longer gives 500 or writes to stderr. No Core edit is needed.
4. **(F-4)** Either get compact `stage4` into the spec, or make it honest: omit `contributingTreeIds` instead of emptying it, keep the vertex field names or signal compaction with an additive flag, and include the `pruneLog` cap in the serialized `historyTruncated`.
5. **(F-5)** Extend the gates:
   - a per-year budget sweep to the agreed horizon on both experiments, not stopping at the first truncation;
   - assertions that at least one event and one annual record are retained;
   - the Results-pane text in the zero-history state;
   - `PlayerSourceStale`, `PlayerPlanStateStale` and `year=int.MaxValue` on `history/year`;
   - Load over a live session with the drawer and Structure open;
   - NB-2 order 2.
6. **(F-6, same pass)** Capture the tree tag at request time in `refreshStructure` and discard mismatched answers, or bump `structureRequest` in `applyPayload` on a tag change. Bump `historyDetailRequest` when `loadHistoryIndex` resets the detail.

Nits N-1 to N-5 are optional.

## 13. Verified by the reviewer vs accepted from the author

- **Verified by the reviewer:**
  - every start check;
  - all 7 gates rerun with exact counts;
  - every number in §5–§9 (my own probes, or my rerun of the author's driver);
  - code inspection of the full `PlayerMultiApi.cs` diff, both test harnesses, and the Step 1 Core APIs (`MultiAreaTimePacing`, `MultiAreaHistoryQuery`, `WoodlandStructureMetrics`, `MultiAreaContinuation.RequireFence`).
- **Accepted from the author:** nothing material. The author's archive (`/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125`, their own subfolders) was not relied on; my outputs are in its `review-*` subfolders. I did not inspect the author's screenshots; I inspected my own.

## 14. NOT TESTED

- The live `null → Unavailable` Structure path: unreachable in normal play; covered only by smoke S2f and Core tests.
- `CancelledBetweenYears`, `AnnualStepRefused` and `StandCannotAdvance` skip outcomes over HTTP: Core-tested, not reachable through normal UI play.
- The F-6 races: inspection only.
- UI responsiveness with view payloads over 1 MB (Year 30+).
- Mutation runs of the author's suites: test-strength findings come from inspection plus my independent probes.
- Unity: not applicable; standalone lab, with no Unity, `Assets`, `Packages` or `ProjectSettings` change.

## 15. Integration decision

```text
INTEGRATION DECISION
Status: NEEDS CHANGES
Gate: G2 ACCEPTANCE (FAIL — P1-UI-04) and G3 MEASUREMENT INTEGRITY (FAIL for P1-UI-04 evidence); G4 not reached
Reviewed branch: feat/multi-2d-phase1-time-history-ui-20261125-v1
Reviewed HEAD: 513355c4bcef881413d517aa62feed4676234319
Context commit: 5fe2fabe9073d7b72711f4ab48553c785351d7bc (base; Step 1 Core approved at 6c83d16e)
SPEC (SPEC REQUIRED only):
- ID / status: SPEC-multi-2d-phase1-time-history-ui-V1 / IMPLEMENTING
- Conformance: FAIL (P1-UI-04; spec §5 tail-retention rule)
Reasons:
- GET api/multi/view exceeds 524,288 B from MA-1-MIXED Year 24 (1,295,498 B at Year 50) and MA-1 Year 42 (602,575 B at Year 50); ecology exceeds 153,600 B from MA-1-MIXED Year 38; the serializer returns over-budget bytes silently (F-1).
- At the edge of the budget the projection drops ALL events and annual records, violating spec §5, and the Results pane falsely states "No ecological year has been advanced in this session." (F-2, Edge-verified at Year 23).
- history/year year=2147483647 returns 500 OutcomeUnconfirmed instead of 409 (F-3); compact stage4 is an unflagged spec deviation (F-4); suite gaps let F-1/F-2/F-3 pass (F-5).
- Everything else re-verified PASS: builds 0W/0E; Core 277/277; Save 50/50; smoke 352/352; save Edge E2E 63/63; Step 2 Edge 59/59; 9/9 anchors; .p0ref exact; guards/fences/zero-mutation; WorkPlanOpen never cancels; NB-2 orders 2 and 3 at 1280x720 and 1920x1080; Save -> quit -> restart -> Load byte-identical at Year 50; Load-over-live reset; hygiene clean.
Required fixes / remaining gates:
- Repair list items 1-6 (§12). Item 1 needs a Manager/owner horizon decision (G5) before implementation.
- Fresh independent re-review of the repair delta, rerunning all 7 gates plus a per-year budget sweep to the agreed horizon on both experiments.
Integration target: none (NOT MERGED, NOT INTEGRATED, NOT OWNER-ACCEPTED)
Integrated HEAD: n/a
Post-integration verification:
- n/a
```

**NOT MERGED. NOT INTEGRATED. NOT OWNER-ACCEPTED.**
