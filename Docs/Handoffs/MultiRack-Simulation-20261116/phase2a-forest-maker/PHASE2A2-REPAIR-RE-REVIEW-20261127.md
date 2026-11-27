# CCF Multi 2D — Phase 2A-2 Repair — Narrow Re-Review

- **Reviewer:** Claude Opus 5.5 (independent review sub-agent; same model family as the original review `7f139c64`)
- **Date:** 2026-11-27
- **Mode:** read-only narrow re-review of the repair only. This branch adds one Markdown file and nothing else.
- **Candidate:** `origin/integration/2d-sim` @ `83f8f402a125b8008154bc8bb7cb8212ce55872a`. This is a merge of `fix/multi-2d-phase2a2-review-conditions-20261127-v1` @ `4825f9c0` (code head `6bd2e675`) onto `bfff6b3f`.
- **Previous review:** `review/multi-2d-phase2a2-forest-maker-ui-v3-20261127-v1` @ `7f139c64` (APPROVED WITH CONDITIONS)
- **Worktree:** fresh detached worktree `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2A2-ReReview-20261127` at `83f8f402`. I set `core.autocrlf=false` and re-checked out with LF before reading.
- **Runs:** none. I ran no builds, tests, gate, selftest or browser suite. Every claim below is settled from code and from the existing evidence under `/home/jer/Development/CCF-Multi-2D-Phase2A2-Evidence-20261127/{repair,repair2,combine2-fast}`.

The item numbers follow the re-review brief. They do not follow the condition numbering in `7f139c64` §6.

---

## 0. Topology and scope (checked)

- `83f8f402` has parents `bfff6b3f` and `4825f9c0`. `git diff --stat 4825f9c0 83f8f402` is empty, so the integrated tree **is** the repair tip.
- `6bd2e675..4825f9c0` changes only the handoff (+47/−1). The code reviewed here is `6bd2e675`.
- `bfff6b3f..83f8f402` touches 20 files: 6 Ui `.cs`, 11 Ui `multi/*.js`, the 2 Phase 2A Edge test files, and the handoff.

---

## 1. Item-by-item verdicts

### Item 1 — Condition 5: ≥3-area page renders from C-FENCE lane reads — **PASS in-session; FAIL at page bootstrap (Minor-R1)**

What holds:

- **Composer.** `fetchMultiStandCFence` (`multi/api.js:108–161`) reads `GET api/multi/stand` (overview). It then reads `GET api/multi/stand?lanes=…` with the overview's fence as query parameters, so the server refuses a stale fence (`MultiApi.Stand.cs:198–212`). It accepts the pair only when `JSON.stringify(overview.fence) === JSON.stringify(lanesPayload.fence)` (`:153–154`). On a mismatch it discards both responses and retries, up to 3 times. `applyComposedStandFrame` checks the fence equality again before applying (`:164–192`). Superseded requests are dropped through `state.laneRequest` (`:195–199`).
- **Lane choice.** `visibleLaneIds(overview.areas, active, singleLaneMode)` gives 2 lanes at the outer lanes, 3 in the interior, and 1 when the toggle is on. The audit saw `MA-A,MA-B | MA-B,MA-C,MA-D | MA-D,MA-E | MA-E | MA-C,MA-D,MA-E`.
- **Every in-session ≥3-area path re-reads lanes and never applies the full view body.** This covers the generic `run` (`api.js:289–307`), queue, spot, Money switch and re-read (`commands.js:67–77,141–152,300–312,506–544`), lane switch and the 1-lane toggle (`main.js:263–311,610–636`), hit (`main.js:462–484`), cell (`modes/interactive.js:71–88`), Fell draft and competition (`panels/fell-draft.js:34–36,239–246`), skip (`panels/time-history.js:161–177`), load (`dialogs/saves.js:186–193`) and woodland/new (`dialogs/woodland.js:243–249`).
- **Two-area stands are unchanged.** Each of those branches falls through to `applyPayload(outcome.payload)` when `isMultiLaneStand()` is false. The audit asserts that the MA-1 re-read uses `/view` (`viewGets=1`).
- **Network audit** (`combine2-fast/edge-1920/run.log` and `edge-1280/run.log`, identical in `repair2`): `/view GETs=0`, 9 overviews (max 78,617 B), 9 lane reads (1-lane max 166,877 B, 2-lane max 330,854 B, 3-lane max 495,027 B). The restored v3 5-lane load also shows `viewGets=0`.

What does not hold:

- **Page bootstrap still GETs the whole `/view`.** `main.js:862` calls `reread(false)` while `state.view` is still `null`. The lane branch in `reread` is guarded by `if (state.view && isMultiLaneStand())` (`commands.js:509`), so a first load falls through to `call('api/multi/view')` (`commands.js:546`). Only **after** the full body arrives does it see ≥3 areas, discard the body and fetch lanes (`:551–553`). Any browser reload or fresh tab on a live ≥3-area stand therefore downloads the whole stand first: about 1.6 MB for a preset, and 2,352,243 B (Y0) or 2,426,490 B (Y15) for the near-cap stand.
- **The audit cannot see this.** The tracker is reset **after** bootstrap, while the page is still on MA-1 (`drive-phase2a.mjs:246–248`, `:443–444`). The near-cap "page load 523 ms / 441 ms" figure is a `Page.navigate` against the live 5-lane stand (`repair2/measure-nearcap.mjs:133`, `drive-phase2a.mjs:564`), so that timing **includes** a full `/view` fetch.
- **Effect.** If a ≥3-area stand's whole `/view` goes over its budget while its lane reads still fit, a reload shows the 503 banner and has no view to keep (`commands.js:569–570`). None of today's measured stands reach this. The near-cap stand at Y15 has 195 KB of headroom.

### Item 2 — Condition 1 (Major): typed 503 on every view-returning route — **PASS**

- A new helper, `WriteBoundedJsonAsync` (`MultiApi.View.cs:298–305`), serialises the **whole response** to `byte[]`. If `alreadyExceeded` is set or `bytes.Length > budget.MaxViewBytes`, it writes `WritePayloadBudgetExceededAsync` (HTTP 503 `{"code":"MultiViewPayloadBudgetExceeded","bytes","budget"}`, `:31–37`). Otherwise it writes the bytes through `WriteRawJsonAsync` (`:453–459`). Nothing reaches the response stream before the decision, so the refusal genuinely happens before send.
- Per-route wiring (both the 200 and the 409 branches use the helper):

| Route | Location |
|---|---|
| command (approve/execute/advance …) | `MultiApi.Commands.cs:33–50` (composite and non-composite) |
| economy/session (Money switch) | `MultiApi.Commands.cs:148–155` |
| economy/quote (aux view) | `MultiApi.Commands.cs:176–191` |
| preview-review (aux view) | `MultiApi.History.cs:32–47` |
| skip / advance-years (refusal and result) | `MultiApi.History.cs:81–113` |
| history/year (all 409s and the 200) | `MultiApi.History.cs:170–213` |
| economy/history/year | `MultiApi.History.cs:233–273` |
| load | `MultiApi.Saves.cs:218–226` |
| cell | `MultiApi.View.cs:487–494` |
| competition (composite and fallback) | `MultiApi.View.cs:517–533` |
| hit (composite and fallback) | `MultiApi.View.cs:556–572` |
| woodland/new | `MultiApi.Woodland.cs:177–186` |
| `GET /view`, `GET /stand`, `/stand?lanes=` | unchanged 503 branches: `MultiApi.View.cs:64–69`, `MultiApi.Stand.cs:159–164,239–245` |

- The remaining raw `WriteJsonAsync` calls carry no view: recover, the slot list, save, the history index, structure, woodland options/preview, the economy executor refusal, and stand 409 refusals (only `FenceOf`).
- Edge evidence at both viewports, under a 400,000 B override, returns 503 on `woodland/new`, `GET /view`, the 3-lane `/stand`, `year/advance`, `skip`, `hit`, `cell`, `competition` and `economy/session`.
- Prior Minor-2 still stands as a known non-condition. On mutating routes the 503 is sent **after** the commit: "pre-send" here means before any byte is written, not before the mutation.

### Item 3 — Condition 4: `CCF_TEST_MAX_VIEW_BYTES` can only lower — **PASS (code); the "raise-clamp proof" test proves nothing (Minor-R2)**

- `MultiViewBudget.ForStand` computes the normal budget first and then returns `min(computed, override)` for both the view and ecology caps (`MultiViewBudget.cs:40–61`). `ForLaneFiltered` delegates to it (`:54–55`). `ReadTestOverrideFromEnvironment` accepts only positive integers (`:61–68`). In code, the override cannot raise any budget.
- The lowering direction is proved by the 400,000 B overflow mode: 3-lane `bytes=495026, budget=400000` gives 503.
- The **raising** direction is not proved. `runClampRaise` (`drive-phase2a.mjs:534–566`, server launched with `9999999`) asserts only that near-cap bodies of 2.35 MB or less return 200 within 2,621,440 B. The old unclamped code would pass every one of those assertions, because an override of 9,999,999 also admits them. A one-line unit assertion would close the gap, for example `ForStand(2, 0, 9_999_999).MaxViewBytes == 524_288`, or a two-area body over 512 KiB returning 503 under the override.

### Item 4 — Condition 3: v3 load requires confirmation — **PASS**

- `MultiApi.Saves.cs:194` changed `== false` to `!= true`. The refusal is thrown inside the lock, **before** `RestoreFromV3`, `WrapRestoredV3` and `SwapLocked` (`:195–197`), so a refused load changes nothing. The 409 response carries the unchanged live view.
- Edge restart mode: a load with the field omitted and a load with `false` both return 409 `WoodlandUnsavedProgress`, with the session id and Year 1 unchanged (`drive-phase2a.mjs:419–428`, PASS at both viewports). The v2 branch (`:179–186`) is unchanged, as the handoff says.

### Item 5 — Condition 2: near-cap custom woodland measured — **PASS; numbers match**

`repair2/nearcap-report.json` against handoff §8.6 (recipe 120 × 120 m, 2,000/ha, 5 lanes, PureSitka, seed 2,880,101, 2,880 living stems at Y0 and 2,710 at Y15):

| Read | Y0 evidence | Y15 evidence | Handoff | Status |
|---|---|---|---|---|
| overview | 149,563 B | 186,727 B | same | 200 |
| 1-lane | 356,864 | 410,031 | same | 200 |
| 2-lane | 702,933 | 761,309 | same | 200 |
| 3-lane | 1,049,710 | 1,113,193 | same | 200 |
| whole `/view` | 2,352,243 | 2,426,490 | same | 200 (Tier-2 compacted) |
| woodland/new | 2,352,370 | — | (Edge clamp log) | 200 |
| page render 2-lane / 3-lane | 523 / 721 ms, no alert | 441 / 709 ms, no alert | same | — |
| skips to Y15 | 5,793 / 5,607 / ~5.4 s | — | same | 200 |

All of these are inside the scaled 2,621,440 B budget, and the page rendered without the overflow banner (Edge clamp mode, both viewports). The speed targets are partly missed: 3-lane first paint is 702–799 ms against the 500 ms target. §8.6 reports these numbers but does not call out the miss.

### Item 6 — Regression risk from `ForLaneFiltered → ForStand` — **PASS on frozen values; one effective lane-budget raise to disclose (Minor-R3)**

- **Two-area stands are unchanged.** `areaCount <= 2` still gives `(524,288, 153,600)` (`MultiViewBudget.cs:41–44`). Before the repair, two-area `ForLaneFiltered` returned 512 KiB for ≤3 lanes and `ForStand(2,…)` = 512 KiB otherwise, so the two-area results are byte-for-byte identical. The constants `DefaultViewBytes 524_288`, `DefaultEcologyBytes 153_600`, `CeilingViewBytes 2_621_440`, `CeilingEcologyBytes 768_000` and `StemsPerBudgetUnit 300` are unchanged (`:20–25`). The ≥3-area formula is the same code, moved.
- **The preset 3-lane read is still under 512 KiB:** 490,909 B GET, 495,026 B POST, 495,027 B player maximum. The Edge suite still asserts `<= 524,288`.
- **One effective raise.** For **≥3-area** stands the server-enforced lane-read budget rose from the shipped 512 KiB to the scaled stand budget. For the 1,344-stem presets that is `ceil(1344/300) = 5` units, which equals 2,621,440 B, a 5× increase. Spec §8 (`specs/multi-2d-phase2a-forest-maker-v1/spec.md:192–194`) sets no separate lane cap, so this does **not** breach the spec's "never raised" rule. That rule covers the scaled ceiling, and the ceiling did not move. But 512 KiB for preset lane reads is now only a test assertion, not a server guarantee, and the handoff does not say so plainly (see Item 8).

### Item 7 — No gate/config change; fences; biology/economy; anchors and golden — **PASS**

- `git diff --name-only bfff6b3f 83f8f402` lists no file under `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/`, `specs/`, `Tools/MultiRackSitkaP1Lab/build/`, `src/Core/`, `.p0ref/`, `tests/Core.Tests/`, `tests/Phase1.Save.Tests/` or `src/Ui/wwwroot/` (count 0).
- The only test-harness changes are the Phase 2A Edge driver and runner. `build/2d-gate*` is untouched.
- No biology or economy math changed: Core has zero diff, and the Ui changes are serialisation, budget and presentation only.
- `combine2-fast` on the integrated tree: 3 clean builds with 0 warnings and 0 errors, `Core.Tests` 312/312 (anchors 10–11 pinned, frozen-anchor tests in the unchanged suite), and `Phase1.Save.Tests` 50/50, including the golden SHA and `T.Eq(24703, …)` at `tests/Phase1.Save.Tests/Program.cs:735`. The 9/9 frozen anchors are inferred from the unchanged suite passing in full. The log does not print a "9/9" line.

### Item 8 — Handoff honesty (§8 / §8.6) — **Mostly accurate; four overclaims or omissions (Minor-R4)**

- (a) §8.1, row `23593916`, says "≥3-area stands render **exclusively** from C-FENCE … `/stand`". That is false at bootstrap (Item 1).
- (b) The §8.2 table lists "Active budget 524,288 B" for every lane read. After §8.6 the active budget for those preset reads is 2,621,440 B. §8.2 is not marked as superseded, although §8.5 item 1 does redirect the §8.3 table.
- (c) §8.6 says "No ceiling was raised and no new constants were added". That is literally true. It does not say that the ≥3-area lane-read budget rose from 512 KiB to the scaled budget (5× for presets). It also does not say that the original handoff's "the lane-filtered read is what brings it under 512 KiB" no longer describes a server limit.
- (d) §8.4 calls the clamp-raise mode a "raise-clamp proof". As shown under Item 3, it would pass on unclamped code.
- (e) Missing: the handoff does not record the combine verification on `83f8f402` (`combine2-fast`, which passed at 60/0 per viewport). §8.6 does disclose that the near-cap 3-lane render misses the 500 ms target only by listing the numbers.
- Accurate as stated: the 503 route list, the v3-load change, the clamp code, the near-cap numbers, the zero forbidden-path diff, and the deferral of selftest/G4 and the decision-log entry (§8.5 item 2).

---

## 2. Findings

### Critical
None.

### Major
None. Prior Major-1 is closed.

### Minor

- **Minor-R1. Page bootstrap on a ≥3-area stand still depends on whole `/view`.** `main.js:862` and `commands.js:509,546–553`. Every in-session path is lane-only, but the first load and every reload download the full stand. This is invisible to the audit (`drive-phase2a.mjs:246–248,443–444`). *Fix (small):* when `state.view` is `null`, call `refreshMultiStandFrame(null, false, 'Read')` first, and fall back to `/view` only when it returns `two-area`. Extend the audit to a `Page.navigate` on a live 5-lane stand with `viewGets === 0`.
- **Minor-R2. The "raise-clamp" Edge mode does not test raising.** `drive-phase2a.mjs:534–566`. The clamp code itself is correct (`MultiViewBudget.cs:56–61`). *Fix:* add one assertion that the override cannot exceed the computed budget.
- **Minor-R3. The ≥3-area lane-read budget was effectively raised from 512 KiB to the scaled budget.** `MultiViewBudget.cs:51–55`. The spec permits this. It is a deliberate product trade-off that the owner should acknowledge, because the 512 KiB preset figure is now only a test assertion.
- **Minor-R4. Handoff overclaims and omissions.** See Item 8 (a)–(e).
- **Minor-R5 (residual, not a condition). The whole-stand body is still the real capacity limit for ≥3-area stands.** Every mutating and aux route still serialises the **whole-stand** bounded view (for example woodland/new at 2,352,370 B near-cap), and the client discards it to re-read lanes. A stand whose whole view exceeds 2.5 MiB would get 503 after commit on advance, skip and similar routes, even though its lane reads fit. In those 503 branches the client shows the banner with a Re-read button but does not refresh lanes automatically (for example `commands.js:79–81`). So the lane reads do not yet extend the size of woodland the page can handle. A future slimmer command response (fence plus notice only, for ≥3 areas) would fix this.

### Nit

- **Nit-R1.** `WriteBoundedJsonAsync` checks the whole response, but tier trimming in `SerializeBoundedMultiView` sizes only the `MultiViewPayload` (`MultiApi.View.cs:151–155` vs `:298–305`). A response near the edge could return 503 where trimming more history would have fit. This errs on the safe side.
- **Nit-R2.** The overview budget uses `view.Areas.Sum(a => a.Trees.Count)` rather than `GenesisStemCount` (`MultiApi.Stand.cs:160`), which differs from every other call site. It can only lower the budget, and the overview is small.
- **Nit-R3.** `_ = requestedLaneCount;` (`MultiViewBudget.cs:54`) is a dead parameter, kept for signature stability. Either remove it or document it.

---

## 3. Status of the original conditions (`7f139c64` §6)

| Original condition | Status at `83f8f402` |
|---|---|
| 1. Major-1: 503 on skip/cell/competition/hit/economy-session + overflow tests | **Closed** (Item 2) |
| 2. Minor-1: v3 load with confirmation omitted refused | **Closed** (Item 4) |
| 3. Minor-8: clamp the test budget override | **Closed in code**; test gap Minor-R2 |
| 4. Complete `selftest.sh` + G4 mutation cases | **Open**: deferred and disclosed (§8.5 item 2) |
| 5. Record C-V3 vs spec §6.2 in the decision log; handoff addendum for Minor-6/Minor-3 | Addendum written. **Decision-log entry still open** (disclosed). The Minor-3 correction was done by implementing lane reads, with the bootstrap gap in Minor-R1. |
| 6. Measure the max-cap custom woodland | **Closed** (Item 5) |

---

## 4. Conditions (follow-up on `integration/2d-sim` before Playable Milestone 1 closeout)

1. **Fix Minor-R1**: bootstrap through C-FENCE for ≥3-area stands, plus a reload audit with `viewGets === 0`. Alternatively, correct the handoff claim to "in-session only" and accept the bootstrap `/view` dependency explicitly.
2. **Correct the handoff by addendum** for Minor-R4 (a)–(e), including an explicit statement of the ≥3-area lane-budget change in Minor-R3.
3. **Carry forward** the original conditions 4 (selftest + G4 mutations) and 5 (decision-log entry). These are unchanged by this repair and were disclosed as deferred.

Minor-R2 is recommended. Minor-R5 and the Nits are not conditions.

## 5. Not verified by this reviewer

I ran no builds, tests, Edge suites, gate or selftest. Results come from logs in `repair`, `repair2` and `combine2-fast`, cross-checked against source at `83f8f402`. I did not exercise a browser reload on a live 5-lane stand. Minor-R1 is established from code (`main.js:862`, `commands.js:509,546`) and from the measurement script's `Page.navigate` (`measure-nearcap.mjs:133`). I did no Unity, Windows or 3D checks.

Finding counts: Critical 0 · Major 0 · Minor 5 (one residual) · Nit 3.

INTEGRATION DECISION: APPROVED WITH CONDITIONS
