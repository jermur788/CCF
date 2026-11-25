# CCF Multi 2D — Phase 1 Step 2 (Time Pacing, Decision→Outcome History & Woodland Structure UI/HTTP + NB-2): Implementation and Verification Handoff

**Date:** 2026-11-25 (desktop-local lane, WSL2 Ubuntu + Windows Microsoft Edge headless CDP).
**Role & sole writer:** Primary Implementer — Gemini 4 Neon (sole writer on the dedicated Step 2 worktree and branch; zero subagents spawned).
**Specification:** [`specs/multi-2d-phase1-time-history-ui-v1/spec.md`](../../../../specs/multi-2d-phase1-time-history-ui-v1/spec.md) (`SPEC-multi-2d-phase1-time-history-ui-V1`).
**Review & repair history:**
- Initial review R1 (`origin/review/multi-2d-phase1-time-history-ui-20261125-v1` on `513355c4`): `NEEDS CHANGES` (`F-1`..`F-6`) → Repair #1 (`adea7af220259a1808346cd5381babed3e9052fa`) under binding **Owner G5 Year 30 horizon**.
- Re-review R2 (`origin/review/multi-2d-phase1-time-history-ui-20261125-v1-r2` on `adea7af2`): closed `F-1` (Year 0..30 budget + never silent), `F-3`, `F-4` flags/shape, `F-6`, `N-1`/`N-3`/`N-4`, and raised 3 bounded findings **`R2-1`**, **`R2-2`**, **`R2-3`** → **Authorized Bounded Repair #2** (this HEAD).

> **EXPLICIT STATE:** This work is **IMPLEMENTED ON TASK BRANCH** only. It is **NOT REVIEWED**, **NOT INTEGRATED**, and **NOT ACCEPTED**. Every checkbox in `specs/multi-2d-phase1-time-history-ui-v1/checklists/requirements.md` remains unchecked `[ ]` pending independent R3 re-review of Repair #2.

| Label | State |
|---|---|
| IMPLEMENTED | **YES — IMPLEMENTED ON TASK BRANCH** (Initial Step 2 implementation + Repair #1 closing `F-1`..`F-6` + Authorized Bounded Repair #2 closing `R2-1`, `R2-2`, `R2-3` and adjacent nit `N-R2-3`) |
| REVIEWED | **NO — NOT REVIEWED** (independent R3 re-review of Repair #2 pending; all 12 checklist boxes remain `[ ]`) |
| INTEGRATED | **NO — NOT INTEGRATED** (task branch only; zero merge to `main`, zero PR, zero tags) |
| VERIFIED (author gates) | **YES for Repair #2 author gates** (`0/0` warnings/errors on `.sln` & `SitkaP1.Ui`; Core `277/277`; Save `50/50`; `ui-smoke-multiarea.sh` `363/363`; `phase1-time-history-ui/run-step2-edge.sh` `67/67` including Year `0..30` sweeps on both `MA-1` & `MA-1-MIXED`, R2-1 Year-0-after-Execute check, and R2-2 cross-tier dead-tree checks; all 9 frozen anchors & `.p0ref` hash exact; note `phase1-save-ui/run-edge-e2e.sh` was NOT re-run in Repair #2 and last passed `63/63` on `adea7af2`) |
| ACCEPTED | **NO — NOT ACCEPTED** |

---

## 1. Lane, Base, Toolchain & Commits

- **Repository root:** `/home/jer/Development/CCF-HP`
- **Worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-20261125`
- **Branch:** `feat/multi-2d-phase1-time-history-ui-20261125-v1`
- **Base commit (Step 1 Core):** `5fe2fabe9073d7b72711f4ab48553c785351d7bc` (independently APPROVED at `6c83d16e`)
- **R2 reviewed HEAD (base for Repair #2):** `adea7af220259a1808346cd5381babed3e9052fa`
- **Repair #2 commit:** linear commit immediately above `adea7af220259a1808346cd5381babed3e9052fa` (`fix(ui): close R2-1/R2-2/R2-3 Step 2 honesty and tier findings`), no amend and no force-push
- **Toolchain:** `~/.dotnet/dotnet` SDK `8.0.134` (`global.json` pins `8.0.133` + `latestPatch`); Linux Node `v26.13.1` (`/home/jer/.local/share/vite-plus/bin/node`); Windows Node `v26.13.1` (`/mnt/c/Program Files/nodejs/node.exe`); installed Windows Microsoft Edge (`C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe`)
- **External `TMPDIR`:** `/home/jer/ccf-p1ui-tmp` (outside repository)
- **External evidence archive (mode `700`, outside repository):** `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125` (zero capability tokens/URLs committed to Git)

### Commit list (oldest → newest above `5fe2fabe…`)

| # | SHA | Message |
|---|---|---|
| 1 | `f16c86db34ade7411d705c8757b414fa5e36fa63` | `spec: add multi-2d phase1 time-history UI quartet` |
| 2 | `d24a89df675d8dc1d719c5353a91db498a257b75` | `feat(ui): add review preview, skip, history and structure endpoints with bounded view payload` |
| 3 | `176ab838c7bae993a6c493379a517d0edbc70267` | `feat(ui): add Skip, History drawer, Structure panel and NB-2 Plant-drawer polish` |
| 4 | `a241c9a055071b628048e8894b5cdf28fe5003a2` | `fix(ui): clean Skip summary null, pin History Load-older footer and fit Structure table at 1280x720` |
| 5 | `6d4c406fa37dbba0e5a66cdf44c30bc30bc217f3` | `test(ui): add Step 2 API, payload, smoke and real-Edge gates` |
| 6 | `513355c4bcef881413d517aa62feed4676234319` | `docs(phase1-2d): record Step 2 implementation and verification handoff` |
| 7 | `adea7af220259a1808346cd5381babed3e9052fa` | `fix(ui): close Step 2 review findings for Year 30 payload and history safety` |
| 8 | *(Repair #2 commit)* | `fix(ui): close R2-1/R2-2/R2-3 Step 2 honesty and tier findings` |

---

## 2. Authorized Bounded Repair #2 Dispositions (`R2-1`, `R2-2`, `R2-3` + `N-R2-3`)

| ID | Severity | Finding Summary | Disposition | Verification Evidence |
|---|---|---|---|---|
| **R2-1** | SHOULD (D-030) | Results pane displayed `"Compact view omits annual history rows to stay within payload budget..."` at Year 0 immediately after Execute because the branch gated on `totalRecordedYears > 0`. | **FIXED**: `renderHistory()` now gates `"Compact view omits annual history rows to stay within payload budget..."` strictly on `state.historyTruncated && Boolean(view.source && view.source.ecologicalYear > 0)` (NEVER on `totalRecordedYears > 0`), with a non-budget defensive fallback if `ecologicalYear > 0 && !state.historyTruncated`, and displays `"No ecological year has been advanced in this session."` whenever `ecologicalYear === 0` (including Year 0 after Execute). | Smoke `R2a` (`PASS`, `repair2-smoke/ui-smoke-multiarea.txt`) + real headless Edge `R2-1` in `runMixedPlaySave` (`PASS`, `repair2-edge-step2/run.log`: `latest="No ecological year has been advanced in this session."`, `truncHidden=true`, `hasWorkLog=true`) + `S2j` (`PASS` for genuinely truncated case). |
| **R2-2** | SHOULD (§5 / D-020 / D-030) | Under projection, dead-tree inspector replaced Core's `BiologicallyDead` with browser-invented `DeadFallen`/`DeadStanding`, appended a suffix to `FelledStump`, and dropped `Pruning lifts` inconsistently across tiers. | **FIXED**: Added Core-authoritative `LifeState` (`LifeStates.Of(tr)`) to `MultiProjectedDeathDto` (zero browser model derivation; `FelledStump` with no suffix and `BiologicallyDead · <cause>, year <y>`, identical to `MultiAreaContinuation.cs:439-441`) and applied the pre-decided rule in `renderInspector()` (`if (tree.isLiving && ps)`) omitting `Pruning lifts` for dead trees identically in BOTH unprojected (Tier 0) and projected (Tier 1/2/3) tiers, documented explicitly in `spec.md` §5. All Years `0..30` remain strictly `<= 524,288 B` on both experiments without restoring full ecology entries. | Real headless Edge cross-tier checks on BOTH experiments (`repair2-edge-step2/run.log`): `MA-1` Tier 0 (Yr 28) → Tier 1 (Yr 29) (`R26-MA-A-9-6`: `BiologicallyDead · DensitySuppression, year 27`; stump `MA-A-001`: `FelledStump`; `Pruning lifts` absent in both tiers; `ma1-dead-tree-projected-1280x720.png`) and `MA-1-MIXED` Tier 0 (Yr 16) → Tier 1 (Yr 17) (`MA-A-043`: `BiologicallyDead · DensitySuppression, year 1`; stump `MA-A-001`: `FelledStump`; `Pruning lifts` absent in both tiers; `mixed-dead-tree-projected-yr17-1280x720.png`). |
| **R2-3** | SHOULD (spec accuracy / retention) | Spec §5 & plan §1.2(6) tier order did not match code, and `SerializeBoundedMultiView` retained an undocumented zero-history emergency branch (`TailRaw(0, 0)` with `exceeded: false`). | **FIXED**: Deleted the undocumented zero-history emergency branch completely from `SerializeBoundedMultiView` (`PlayerMultiApi.cs`) so every tier preserves `>= 1` latest event & `>= 1` latest annual record whenever present and any post-horizon overflow goes straight to Tier 3 (`payloadBudgetExceeded: true` + stderr log). Rewrote `spec.md` §5 and `plan.md` §1.2(6) to match the implemented code **Tier 0 → Tier 1 → Tier 2 → Tier 3** order exactly 1:1. | Direct code/spec 1:1 inspection + all Repair #2 builds, smoke (`363/363`), and Edge/sweep gates (`67/67`). |
| **N-R2-3** | NIT (adjacent, zero-risk) | Unused `var bounded = ...; _ = bounded;` in `ReadMultiViewAsync`. | **FIXED**: Removed unused local/discard in `ReadMultiViewAsync`. | Clean 0W/0E UI build + all Repair #2 gates. |

Prior R1 findings (`F-1`..`F-6`, `N-1`, `N-3`, `N-4`) remain closed as verified in R2; optional R1 nits `N-2` and `N-5` and optional R2 nits `N-R2-1` and `N-R2-2` remain deferred/out-of-scope of this bounded pass.

---

## 3. Files Changed & Delta Stat

### Repair #2 code/spec/test delta (`git diff --stat adea7af2`, excluding this handoff)

```text
 Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs |  52 ++++-----
 .../tests/phase1-time-history-ui/drive-step2.mjs   | 127 +++++++++++++++++++++
 .../tests/ui-smoke-multiarea.sh                    |  12 ++
 specs/multi-2d-phase1-time-history-ui-v1/plan.md   |   6 +-
 specs/multi-2d-phase1-time-history-ui-v1/spec.md   |  18 +--
 5 files changed, 175 insertions(+), 40 deletions(-)
```

Plus this updated handoff (`Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP2-TIME-HISTORY-UI-NEON-IMPLEMENTATION-20261125.md`) — **6 files total in Repair #2**, strictly inside the Repair #2 writable allowlist.

### Cumulative branch files vs base (`git diff --name-only 5fe2fabe..HEAD`)

Exactly **9 files** total (all inside the Step 2 allowlist):
- `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP2-TIME-HISTORY-UI-NEON-IMPLEMENTATION-20261125.md`
- `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`
- `Tools/MultiRackSitkaP1Lab/tests/phase1-time-history-ui/drive-step2.mjs`
- `Tools/MultiRackSitkaP1Lab/tests/phase1-time-history-ui/run-step2-edge.sh`
- `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh`
- `specs/multi-2d-phase1-time-history-ui-v1/{checklists/requirements.md,plan.md,spec.md,tasks.md}`

**Forbidden paths verified 100% untouched:** `src/Core/**` (including `src/Core/Phase1/**`), `src/Ui/wwwroot/**`, `.p0ref/**`, `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/coordination/**`, `.vscode/**`, `main`.

**Hygiene verified on Repair #2:**
- `data:image/webp` in `src/Ui/PlayerMultiApi.cs`: **0**
- Capability tokens (`/s/[0-9a-f]{64}`) in `git diff 5fe2fabe..HEAD` and in `repair2-smoke`/`repair2-edge-step2` logs: **0**
- Checked boxes (`[x]`) in `specs/multi-2d-phase1-time-history-ui-v1/checklists/requirements.md`: **0** (all **12** remain `[ ]`)

---

## 4. Measured Year 0..30 `GET api/multi/view` & `ecology` Payload Tables (Repair #2, Owner G5 Horizon)

Hard limits at every Year `0..30`: raw `GET api/multi/view <= 524,288 B` (512 KiB), `ecology <= 153,600 B` (150 KiB), `>= 1` latest work event & `>= 1` latest annual record retained whenever present, `payloadBudgetExceeded = false`, empty server stderr.

Measured directly over live loopback HTTP during Repair #2 (`repair2-edge-step2/step2-sweep-ma-1.json` and `repair2-edge-step2/step2-sweep-ma-1-mixed.json`, WITH Core `death.lifeState` included on projected dead trees):

- **`MA-1` Year 0..30 summary:** max raw **524,279 B** (Year 28, Tier 0; Y29 flips cleanly to Tier 1 at `406,036 B`, Y30 `413,142 B` with `ev=4, an=30, trunc=false, proj=true, s4c=false, over=false`), max ecology **43,320 B** (Year 0). **31/31 PASS**.
- **`MA-1-MIXED` Year 0..30 summary:** max raw **523,375 B** (Year 29, Tier 1; Y17 flips cleanly to Tier 1 at `409,378 B`; Y27..Y30 monotonically trim `an=18 -> 12 -> 8 -> 2`, Y30 `520,582 B` with `ev=2, an=2, trunc=true, proj=true, s4c=false, over=false`), max ecology **49,685 B** (Year 19). **31/31 PASS**.
- `stage4Compacted` (`s4c`) remained **`false`** at every single year `0..30` on both experiments!

| Year | `MA-1` Raw (B) | `MA-1` Eco (B) | `MA-1` `ev`/`an` | `MA-1` `trunc`/`proj`/`s4c`/`over` | `MA-1-MIXED` Raw (B) | `MA-1-MIXED` Eco (B) | `MIXED` `ev`/`an` | `MIXED` `trunc`/`proj`/`s4c`/`over` |
|---:|---:|---:|:---:|:---:|---:|---:|:---:|:---:|
| 0 | 391,058 | 43,320 | 1/0 | F/F/F/F | 393,452 | 45,045 | 1/0 | F/F/F/F |
| 1 | 397,695 | 42,744 | 1/1 | F/F/F/F | 400,092 | 44,487 | 1/1 | F/F/F/F |
| 2 | 401,585 | 41,571 | 1/2 | F/F/F/F | 404,236 | 43,209 | 1/2 | F/F/F/F |
| 3 | 403,734 | 41,080 | 1/3 | F/F/F/F | 406,621 | 42,618 | 1/3 | F/F/F/F |
| 4 | 405,354 | 40,807 | 1/4 | F/F/F/F | 408,894 | 42,258 | 1/4 | F/F/F/F |
| 5 | 408,715 | 39,874 | 1/5 | F/F/F/F | 412,591 | 41,018 | 1/5 | F/F/F/F |
| 6 | 411,022 | 39,426 | 1/6 | F/F/F/F | 415,211 | 40,471 | 1/6 | F/F/F/F |
| 7 | 414,337 | 38,587 | 1/7 | F/F/F/F | 418,897 | 39,386 | 1/7 | F/F/F/F |
| 8 | 416,825 | 38,010 | 1/8 | F/F/F/F | 421,935 | 38,584 | 1/8 | F/F/F/F |
| 9 | 419,410 | 37,400 | 1/9 | F/F/F/F | 425,122 | 37,770 | 1/9 | F/F/F/F |
| 10 | 422,341 | 37,073 | 2/10 | F/F/F/F | 429,561 | 37,955 | 2/10 | F/F/F/F |
| 11 | 425,212 | 36,335 | 2/11 | F/F/F/F | 435,963 | 37,994 | 2/11 | F/F/F/F |
| 12 | 427,587 | 35,901 | 2/12 | F/F/F/F | 457,675 | 41,736 | 2/12 | F/F/F/F |
| 13 | 437,445 | 37,291 | 2/13 | F/F/F/F | 475,601 | 44,496 | 2/13 | F/F/F/F |
| 14 | 445,400 | 37,815 | 2/14 | F/F/F/F | 486,253 | 45,141 | 2/14 | F/F/F/F |
| 15 | 447,337 | 36,953 | 2/15 | F/F/F/F | 501,355 | 46,008 | 2/15 | F/F/F/F |
| 16 | 452,940 | 36,618 | 2/16 | F/F/F/F | 512,449 | 44,917 | 2/16 | F/F/F/F |
| 17 | 457,005 | 36,255 | 2/17 | F/F/F/F | 409,378 | 46,439 | 2/17 | F/**T**/F/F |
| 18 | 464,663 | 35,885 | 2/18 | F/F/F/F | 424,738 | 48,378 | 2/18 | F/**T**/F/F |
| 19 | 473,135 | 35,578 | 2/19 | F/F/F/F | 440,197 | 49,685 | 2/19 | F/**T**/F/F |
| 20 | 476,298 | 34,214 | 3/20 | F/F/F/F | 451,237 | 49,228 | 3/20 | F/**T**/F/F |
| 21 | 486,057 | 35,317 | 3/21 | F/F/F/F | 459,513 | 48,112 | 3/21 | F/**T**/F/F |
| 22 | 490,756 | 34,282 | 3/22 | F/F/F/F | 466,592 | 45,898 | 3/22 | F/**T**/F/F |
| 23 | 495,290 | 33,672 | 3/23 | F/F/F/F | 479,252 | 46,379 | 3/23 | F/**T**/F/F |
| 24 | 499,271 | 33,319 | 3/24 | F/F/F/F | 493,828 | 46,895 | 3/24 | F/**T**/F/F |
| 25 | 504,081 | 32,822 | 3/25 | F/F/F/F | 506,865 | 47,034 | 3/25 | F/**T**/F/F |
| 26 | 512,286 | 33,195 | 3/26 | F/F/F/F | 519,667 | 46,825 | 3/26 | F/**T**/F/F |
| 27 | 516,961 | 32,112 | 3/27 | F/F/F/F | 522,866 | 47,475 | 3/18 | **T**/**T**/F/F |
| 28 | **524,279** | 32,749 | 3/28 | F/F/F/F | 522,457 | 46,028 | 3/12 | **T**/**T**/F/F |
| 29 | 406,036 | 32,108 | 3/29 | F/**T**/F/F | **523,375** | 46,109 | 3/8 | **T**/**T**/F/F |
| 30 | 413,142 | 32,650 | 4/30 | F/**T**/F/F | 520,582 | 48,441 | 2/2 | **T**/**T**/F/F |

Interactive Edge workflows also independently verified Year 0..30:
- `MA-1` (`ma1-full` Yr 9..30): `max_raw = 521,312 B <= 524,288`, `max_eco = 37,807 B <= 153,600`, full history intact (`totalRecordedYears = 31`).
- `MA-1-MIXED` (`mixed-play-save` Yr 16..30): `max_raw = 523,572 B <= 524,288`, `max_eco = 48,425 B <= 153,600`, Year 30 `522,504 B` (`ev=3, an=4, trunc=true, proj=true, s4c=false, over=false`), Year 0 detail intact at Year 30.

---

## 5. Exact Build, Test, Smoke & Real-Edge Verification Results (Repair #2)

Environment: WSL2 Ubuntu, `PATH="$HOME/.dotnet:$HOME/.local/share/vite-plus/bin:$PATH"` (`dotnet 8.0.134`, Linux `node v26.13.1`), Windows `node.exe v26.13.1`, Windows Microsoft Edge (`--headless=new`), `TMPDIR=/home/jer/ccf-p1ui-tmp`.

| Gate | Exact Command | Result (Repair #2) |
|---|---|---|
| G1. Solution build | `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| G2. Core tests | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **277/277 passed, 0 failed** (`repair2-core-tests.log`; 8/8 Core anchors exact) |
| G3. Save tests | `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | **50/50 passed, 0 failed** (`repair2-save-tests.log`) |
| G4. UI build | `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | **Build succeeded — 0 Warning(s), 0 Error(s)** |
| G5. Multi-area HTTP + DOM smoke | `TMPDIR="$TMPDIR" tests/ui-smoke-multiarea.sh <evidence>/repair2-smoke` | **OVERALL: PASS (363 passed, 0 failed)** (`repair2-smoke/ui-smoke-multiarea.txt`; includes new `R2a` Year-0-after-Execute check; empty stderr; clean SIGINT; lab tree & git status unchanged) |
| G6. Existing Save/Load real-Edge E2E | *(not re-run in Repair #2 per authorized command list; last verified on `adea7af2`)* | **NOT RE-RUN IN REPAIR #2** (last observed PASS `63/63` in `repair-edge-save/run.log` on `adea7af2`) |
| G7. Step 2 real-Edge + Year 0..30 sweeps | `tests/phase1-time-history-ui/run-step2-edge.sh <evidence>/repair2-edge-step2` | **OVERALL: PASS (67 passed, 0 failed)** (`repair2-edge-step2/run.log`: `ma1-full` 29/29 incl. R2-2 MA-1 Yr 28→29 dead-tree check, `mixed-play-save` 20/20 incl. R2-1 Year-0-after-Execute check + R2-2 MA-1-MIXED Yr 16→17 dead-tree check, `mixed-restart-load` 6/6, `sweep-ma-1` 1/1 [31/31 yrs], `sweep-ma-1-mixed` 1/1 [31/31 yrs], plus 10 SIGINT/stderr checks across all 5 server processes) |

---

## 6. Nine Frozen Anchors, `.p0ref` Pin & Protected Listeners

All 9 frozen anchors were re-observed exact in `repair2-core-tests.log` and `sha256sum`:

| # | Anchor | Observed SHA-256 | Match |
|---|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | **EXACT** |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | **EXACT** |
| 3 | Multi-area `0/0/1/0` (Crown C2) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | **EXACT** |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | **EXACT** |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | **EXACT** |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | **EXACT** |
| 7 | Phase 2 Q6 (`1/1/1/1/1` vs `1/1/1/1/0`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | **EXACT** |
| 8 | P0 Core binary pin (`.p0ref/SitkaLab.Core.dll`) | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | **EXACT** |
| 9 | MA-1-MIXED 30-year fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | **EXACT** |

- **Protected listeners (verified alive before and after, untouched):**
  - `127.0.0.1:33767` (`LISTEN`, PID `1750123`)
  - `127.0.0.1:43649` (`LISTEN`, PID `1806361`)
  - `127.0.0.1:46593` (`LISTEN`, PID `839989`)
- **Ephemeral cleanup:** Zero leftover `sitkap1-ui.dll` processes from this lane and zero lingering `ccf-step2-edge`/`ccf-edge-` `msedge.exe` processes.

---

## 7. Deviations & Residual Risks (Disclosed Transparently)

### Deviations
1. **Optional nits deferred:** R1 optional nits `N-2` (adjacent read-only locks) and `N-5` (History drawer focus trap) and R2 optional nits `N-R2-1` (forcing Tier 3 over-budget branch inside automated gates) and `N-R2-2` (dead-tree 3-decimal projection vs 2-decimal display rounding) were left out of this strictly bounded R2-1/R2-2/R2-3 repair pass.
2. **Pre-decided dead-tree `Pruning lifts` omission rule:** Documented explicitly in `spec.md` §5 and implemented identically across both unprojected (Tier 0) and projected (Tier 1/2/3) tiers.
3. **Tier 2 (`stage4Compacted`) & Tier 3 (`payloadBudgetExceeded`):** Neither activates anywhere in Years `0..30` on `MA-1` or `MA-1-MIXED` (`s4c = false`, `over = false` across all 31 years); both remain active for post-horizon runs with no zero-history drop.

### Residual Risks
1. **Horizons beyond Year 30:** Under the binding Owner G5 Year 30 horizon, both experiments hold `<= 524,288 B` raw and `<= 49,685 B` ecology at every year `0..30` (`MA-1-MIXED` Year 30 keeps `ev=2, an=2` at `520,582 B`). Post-horizon dense mixed stands will transition cleanly to Tier 2 (`stage4Compacted=true`) and, if still over budget, Tier 3 (`payloadBudgetExceeded=true` + stderr log) while preserving `>= 1` event & `>= 1` annual record.
2. **Pending independent R3 re-review:** All 12 acceptance checklist boxes remain `[ ]` until independent re-review verifies Repair #2.

---

## 8. Evidence Archive Inventory (Outside Repository)

Path: `/home/jer/Development/CCF-Multi-Phase1-Step2-Evidence-20261125` (mode `700`, zero capability tokens in Git):
- **Repair #2 artifacts:**
  - `repair2-core-tests.log` (Core `277/277`, 8/8 Core anchors)
  - `repair2-save-tests.log` (Save `50/50`)
  - `repair2-smoke/ui-smoke-multiarea.txt` (`363/363`, capability-redacted)
  - `repair2-edge-step2/run.log` + `step2-sweep-ma-1.json` + `step2-sweep-ma-1-mixed.json` + `step2-pre-save.json` + 5 capability-redacted server logs + 19 PNG screenshots (including `ma1-dead-tree-projected-1280x720.png` and `mixed-dead-tree-projected-yr17-1280x720.png`) (`67/67`)
- **Earlier pass artifacts (preserved untouched):** `core-tests.log`, `save-tests.log`, `smoke/`, `edge-save/`, `edge-step2/`, `repair-core-tests.log`, `repair-save-tests.log`, `repair-smoke/`, `repair-edge-save/`, `repair-edge-step2/`
