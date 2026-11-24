# Multi-Area Map Integration — Pass 1 Independent Review

**State (D-030):** `INDEPENDENTLY REVIEWED ON TASK BRANCH — APPROVED FOR INTEGRATION GATES — NOT INTEGRATED, NOT MERGED`

---

## 1. Identity and provenance

| Field | Value |
|---|---|
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness, full-access, default effort |
| Role | Independent reviewer. I did not write, plan or repair any part of this change. I did not trust the implementer's claims: every PASS below is something I ran or read myself. |
| Delegating T3 thread | `c00d4799-5f15-4a7d-9231-0f133246a64c` |
| Start UTC | `2026-11-24T12:55:50Z` |
| End UTC | `2026-11-24T13:14Z` (file finalised; the commit timestamp is in Git) |
| Reviewed feature SHA | `1f13182659769f19e7c91159dd7ad475667e0d62` (`feat/multi-map-integration-pass1-20261124-v1`; the same SHA on `origin`) |
| Base | `cf6fe7a52aa096e824827c568f43fe05724dcce9` |
| Stage 4 Owner UX reference | `0e7774557dbb2947b29993cf032c084aa3b7227f` |
| Review branch / worktree | `review/multi-map-integration-pass1-20261124-v1` at `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-MapIntegration-Pass1Review-20261124` |
| Review-branch parent of this commit | `1f13182659769f19e7c91159dd7ad475667e0d62`. The new review HEAD SHA is reported in the delegation result, because a file cannot contain its own commit SHA. |
| Git author note | The worktree's inherited Git identity is `DeepSeek V4.1 Flash orchestrator`. The review itself was executed by Claude Opus 5.5. |
| Tools actually used | PowerShell → `wsl -d Ubuntu` bash scripts; `/home/jer/.dotnet/dotnet` SDK `8.0.134` (`TMPDIR=/home/jer/tmp`); `git`; `python3 -I` (`http.client`); WSL `node` v26.13.1 (smoke-test DOM stand-in); Windows `node.exe` v26.13.1 driving real headless Microsoft Edge over CDP; the Read/Grep tools for code and PNG review |
| Not used | Unity Editor, Blender, `build/build-p0-core-ref.sh` (never run), any package install |

**Build isolation.** I built and ran everything in a `git clone --shared` copy outside the repository (`/home/jer/tmp/opus-p1rev/build`), checked out detached at `1f131826`. Its tree object `3717defa6ee1ba5199aca625cc8a6383354c6159` is identical to the review worktree's. I copied the pinned `.p0ref` from the feature worktree and verified it before use (`SitkaLab.Core.dll` sha256 `9a99f781…34b3`). The feature worktree and the root checkout were only read. The only write to this review worktree is this file.

---

## 2. Decision

```
INTEGRATION DECISION: APPROVED
Gate G2: PASS
Gate G3: PASS
Spec conformance: PASS
Repair cycles used: 0
```

There are **no blocking findings.** The non-blocking findings in §6 are documentation-precision items, test-coverage gaps or cosmetic/owner-judgement notes. None of them is a false verification claim, a scope breach, a biology change or a spec violation that would stop integration.

---

## 3. What I personally ran (observed evidence)

Raw logs are outside Git (not durable `/home/jer/tmp` storage): `/home/jer/tmp/opus-p1rev/{build.log,test.log,test2.log,buildui.log,smoke.log,probes.log,cli.log,prune.log}` and `/home/jer/tmp/opus-p1rev/browser-out/` (driver source, 4 JSON result files, 28 screenshots). No capability URL is written in any of them in the clear; the drivers redact `/s/<64 hex>/`.

| # | Check | Command / method | Observed |
|---|---|---|---|
| E1 | Clean solution build | `dotnet build MultiRackSitkaP1Lab.sln -nologo -v quiet --disable-build-servers --no-incremental` | `Build succeeded. 0 Warning(s) 0 Error(s)`, exit 0 |
| E2 | Clean UI build (the `.sln` does **not** contain `SitkaP1.Ui.csproj`) | `dotnet build src/Ui/SitkaP1.Ui.csproj … --no-incremental` | `0 Warning(s) 0 Error(s)`, exit 0 |
| E3 | Core suite, run 1 | `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | `251/251 passed, 0 failed`, exit 0 |
| E4 | Core suite, run 2 + PASS-set diff | same, then `diff` of the `^(PASS|FAIL)` lines | `251/251`; PASS set **identical** (251 lines) |
| E5 | Test inventory vs base | `T.Add` names at `cf6fe7a5` vs `1f131826` | +6 names (`MI-P1 C-01..C-06`); one rename (`UI U-02`); **zero removed** |
| E6 | `ui-smoke-multiarea.sh` at `1f131826` | `NODE=$(command -v node) DOTNET=… tests/ui-smoke-multiarea.sh <evidence>` | `OVERALL: PASS (170 passed, 0 failed)`, exit 0 (M 13, W 95, J 23, R 19, F 8, S 4, Z 8; Z03/Z04: no lab file changed, git status unchanged) |
| E7 | Live HTTP probe, default launch (no flag), `ma-1`, `ma-1-mixed` | `probe_http.py` (own `http.client` driver against the built process) | 52/52, 52/52, 54/54 PASS |
| E8 | CLI rejection probes | 9 bad invocations of `sitkap1-ui.dll` | 9/9: exit 64, one usage line, no URL printed |
| E9 | Live species-aware Prune/Crop/Approve/Cancel/Execute/Year+1 over HTTP on `ma-1-mixed` | `probe_prune.py` | 11/11 PASS |
| E10 | Real headless Edge matrix (own CDP driver `review-cdp.mjs`) | `ma-1` and `ma-1-mixed` × 1280×720 and 1920×1080, a fresh server per cell | **27/27 PASS in each of the 4 cells** |
| E11 | Scope | `git diff --name-only cf6fe7a5..1f131826`, `git grep image/webp`, route diff | see MI-REG |
| E12 | Hygiene | `/proc` cmdline-checked SIGTERM/SIGINT of my own servers only; `netstat -ano` | every server of mine ended "Session ended; nothing was saved." with empty stderr; protected listeners `127.0.0.1:33767/43649/46593` (PID 5564) still LISTENING before and after |

### 3.1 Frozen anchors (observed in my E3 run; every pinned-fingerprint test PASSed)

| Anchor | Expected | Observed (my run) | Result |
|---|---|---|---|
| Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` (model0 F-01) | MATCH |
| Multi `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` (model0 F-02) | MATCH |
| Multi `0/0/1/0` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | MATCH |
| Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | `8a926776…87fb` (S4 run and S5-01 F-01) | MATCH |
| Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | `c4b670ed…dc35` (S5 full run and P2-01 `1/1/1/1/0`) | MATCH |
| Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` (S5-07A) | MATCH |
| Phase 2 Q6 | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | `1/1/1/1/0 776a68df…e22d` \| `1/1/1/1/1 776a68df…e22d` (P2-03 M-08) | MATCH (both sides equal) |
| P0 Core binary pin | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | `sha256sum` of `.p0ref/SitkaLab.Core.dll` before the build, plus the P2-01 runtime check: `9a99f781…34b3` | MATCH |
| MA-1-MIXED 30-y | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` (P2-10) | MATCH |

The `ma-1` manifest content hash was also re-observed live: the body SHA-256 equals `contentHash`, which equals the pinned `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95`.

### 3.2 Payload budgets (my live raw bytes)

`ecology` was measured as the exact raw byte span of the top-level `"ecology"` value in the HTTP body, not as a re-serialisation. It is the last member of `MultiViewPayload`, and the span was re-parsed to confirm it is one complete JSON value.

| Launch | Year | `GET api/multi/view` raw bytes (≤ 524,288) | `ecology` raw bytes (≤ 153,600) |
|---|---:|---:|---:|
| default / `ma-1` | 0 | 387,472 | 43,438 |
| default / `ma-1` | 5 | 410,538 | 45,385 |
| `ma-1-mixed` | 0 | 388,736 | 44,393 |
| `ma-1-mixed` | 5 | 414,571 | 46,961 |

These equal the Core test `MI-P1 C-06` figures byte for byte. Over 383 cells × 2 years × 3 launches, every `stage4.ground` cell has exactly one `cellsCompact` row. The compact `light` value agrees with the full-precision `stage4` light index to within 1e-4, and **no positive value was floored to 0**. One Year-0 entry per launch lies in (0, 0.001) and is preserved.

### 3.3 Real-browser matrix (my CDP driver; Stage 4 R3 fold gate re-typed from `0e777455` `scenario-owner-ux.mjs` `LOADED_FOLD`/`loadedFoldOk`)

| Cell | Result | Horizontal scroll | R3 loaded fold: 4 blocked + 1 ticked | R3 loaded fold: 2 blocked + 3 ticked | Crowns (A / B) | Console / exceptions / CSP / 5xx |
|---|---|---|---|---|---|---|
| `ma-1` 1280×720 | 27/27 | `1280 ≤ 1280` | last row 590 ≤ dock 594; Add bottom 644 ≤ 720; scrollTop 0; rows 38 px | same geometry | 171 / 73, all `w=h=2r` | 0 / 0 / 0 / 0 |
| `ma-1` 1920×1080 | 27/27 | `1920 ≤ 1920` | 818 ≤ 829; Add 915 ≤ 1080; rows 50 px | same | 171 / 73 | 0 / 0 / 0 / 0 |
| `ma-1-mixed` 1280×720 | 27/27 | `1280 ≤ 1280` | 590 ≤ 594; Add 644 ≤ 720 | same | 171 / 73 (B: 25 `#ccf-sitka`, 24 `#ccf-oak`, 24 `#ccf-beech`) | 0 / 0 / 0 / 0 |
| `ma-1-mixed` 1920×1080 | 27/27 | `1920 ≤ 1920` | 837 ≤ 848; Add 934 ≤ 1080 | same | as above | 0 / 0 / 0 / 0 |

The only network log entry was the deliberate 409 from my stale-fence probe. The other checks in every cell are listed in the result JSON. They cover: the truthful badge, all 21 ids, zero WebP, zero external URLs, zero Pass 2 buttons, no Expired Shelter in the Work Plan, visible PAR and schematic caveats, the default modes, Tab reaching the skip link, a real crown click selecting the tree, the Inspect Cell swap, a real click producing `POST api/multi/cell` 200 with zero revision change, a live 409 with a fresh view, a real Escape keeping draft ticks, the reference and the overlay, the fold still holding afterwards, and all three overlays.

---

## 4. Per-criterion verdict

| Row | Verdict | Evidence I observed |
|---|---|---|
| **MI-P1-01** | **PASS** | **Default Phase 2.** A default launch with no flag serves `models` 1/1/1/1/1 in the manifest and `ecology.models` (E7). `OpenSynthetic` → `OpenExperiment(..., models ?? Phase2)` (`MultiAreaWorkPlanAdapter.cs:32-58`). The Core default constructor stays 0/0/0/0/0 (C2-01 and S4 V-01 PASS). The single-area player has no file in the diff and its anchor holds. **CLI.** 9/9 rejection probes, plus smoke M12/M13 (E8, E6). **MA-1-MIXED factory.** In `src/Core/MultiArea/MultiAreaEcology.cs:243-289`, with no test-assembly reference. Live: MA-B ordinal round-robin has 0 mismatches over 73 trees (25/24/24) and MA-A is 100 % Sitka (E7). C-01 covers determinism. **Badges.** Exact strings in the manifest and the visible header in all 4 browser cells. **Atomic snapshot.** `ReadCompositeUiSnapshot()` is one `lock (sync)` around `CompositeSnapshotOf(root)` (`MultiAreaContinuation.cs:281-284, 381-401`). `GET api/multi/view` uses only that path (`PlayerMultiApi.cs:99-104`). Live sessionId/source/planStateRevision parity across view/stage4/ecology at Y0 and Y5 on 3 launches; C-02 PASS. |
| **MI-P1-02** | **PASS** | `view`/`scenes`/`labels`/`stage4` keep their names and shapes (`PlayerMultiApi.cs:238-254`); `ecology` is additive. It is `null` for Species 0 (C-03 checks Legacy, Stage 3, Stage 4 and Stage 5). Budgets: §3.2. The non-zero floor and light alignment were verified live (§3.2). The `stage4` prune readout equals `ecology.pruneStatusByTreeId` for **every** tree (E7). Oak is 2 / 3-yr / 3.5 m. `POST api/multi/cell` (E7, ×3 launches): 200 with every FR-5.3 member, its `lightIndex` equal to that cell's `stage4` value. 409 for stale planStateRevision, stale source, foreign session, out-of-bounds, negative, unknown area and wrong area (A cell under MA-B), each with a fresh view/stage4/ecology. 400 for PascalCase, missing, null, string, fractional and int-overflow coordinates, an unknown top-level or nested member, a duplicate member, malformed JSON, an array, whitespace in `areaId` and a 65-char `areaId`. 413 at > 16 KiB, 415, 403 for both Origin and Host, 404 without the capability, 405 for GET. **`GET api/multi/view` was byte-identical before and after all of these probes.** |
| **MI-P1-03** | **PASS** | All 21 ids are in `#map <defs>` (live, 4 cells; smoke M09). `git grep image/webp` in `src/` finds nothing, and there is zero `data:image/webp` in the live DOM, `multi.js` or `multi.css`. `PlayerApi.cs` (the CSP owner) is unchanged and smoke M02 passes. Live, 244 crowns per cell were drawn as `<use>` with `x=cx-r`, `y=cy-r`, `width=height=2r`, `data-radius-m = r`, `rotate()` only, and **zero `scale(` transforms** anywhere in `#map`. The transparent true-radius `circle.crown` hit disc is kept (`PlayerMultiApi.cs` JS `crownDecoration`/`renderMap`). The browser script has no DBH→radius, basal-area or crown maths; its only DBH uses are display formatting. Rules 1-5: ClearUndergrowth highlights the whole cell polygon, never a crown badge. SpotClear uses a world-scale `use` box of `2·radiusM`, in its own layer under `planted-layer`, with no seedling in the symbol (see N-7). Pending is dashed `#edc774` + ◷ + text, Approved is solid `#88c5e8` + ✓ + text, Completed is a dark-slate "☑ Completed · History Log" pill (see N-6). Shelters use solid cyan `#ccf-shelter-active` and dashed grey + X `#ccf-shelter-expired`. **Expired Shelter is legend/map only:** the Work Plan read-only list uses only `ccf-action-{fell,prune,clear,plant,shelter,spot}`, and the live check found zero in `#pane-plan`. The crop ring is a double gold ring plus `C`. I compared screenshots with `Standard_SVG_Asset_Sheet_v2.png` (sha256 per spec): the families, palette and markers agree. |
| **MI-P1-04** | **PASS** | Two modes. Inspect Cell swaps `#cell-panel` in and hides `#tree-panel` (live: no stacking). A real Escape returns to Select Tree with the draft ticks ("Add selected 3"), the reference and the overlay kept (live, 4 cells). There is one overlay `<select>` with Off/Light/Undergrowth/Seedlings; Undergrowth draws both hatches, and Seedlings is cell-level with the "not a literal stem count" caveat. Zero Pass 2 write controls (live and code). The two caveats are visible. The deadwood tooltip gives the engine species plus "direction schematic, not simulated" (`renderDeadwoodLayer`). Species-aware Prune works end to end over HTTP (E9, 11/11): Oak 3.5 m → 3-yr wait → 6.0 m accepted at year 3; Sitka 2.5 m → 5-yr wait, with year-3 add refused 409 `PruneIntervalTooShort`. The readout always matched the engine guard. Fell, Crop, Approve, Cancel, Execute and Year+1 pass via smoke W/J (E6) and E9. |
| **MI-P1-05** | **PASS** | My own real Edge matrix: §3.3, 4 × 27/27. **The implementer's matrix numbers were not adopted**; these rows are my observations. |
| **MI-P1-06** | **PASS** | E1/E2 both 0/0. E3/E4: 251/251 twice with identical PASS sets, which is 245 base + 6 new by name inventory (E5). E6 smoke 170/170. E12 servers and listeners. The handoff is committed at `1f131826` and `origin/feat/...` = `1f131826`. No tag points at it, no PR exists (`gh pr list` → `[]`), and it is not on `origin/main`. `git status --short` of the feature worktree was empty when I read it. |
| **MI-REG** | **PASS** | All 9 anchors observed (§3.1). `git diff --name-only cf6fe7a5..1f131826` lists only `Tools/MultiRackSitkaP1Lab/{src/Core/MultiArea,src/Ui,tests}`, `specs/multi-map-integration-pass1-v1/**` and the handoff. That means zero `Assets/`, `Packages/`, `ProjectSettings/`, `Tools/MultiRackSitkaLab/`, save, economy or `Stage5GridStudy.cs` paths. The route diff shows one new route, `POST api/multi/cell`, which is read-only, plus the manifest handler; there is no Pass 2 write route. No capability token appears in the diff. |
| **MI-P1-REVIEW** | **PASS** | This document: an independent reviewer with no implementation role, no repairs made, and every claim re-observed or marked otherwise. |

---

## 5. Attack points from the brief, with conclusions

1. **Stage 4 UX preservation.** Commit `5f74f8a7` makes `PlayerMultiApi.cs` and `ui-smoke-multiarea.sh` **byte-identical** to `0e777455` (`git diff --quiet`). The Pass 1 rewrite on top of it keeps every Stage 4 polish item I could test:
   - crown-wins (a live real click);
   - the R3 5-row + Add fold with blocked rows at both viewports (my re-typed `loadedFoldOk` gate);
   - Escape precedence: dialog first (F-05), then search box, cell mode, inspection (`onEscape`);
   - the scale bar (`renderScale`), Fit (`zoom-fit`), the legend hide/show toggle (`setLegendHidden`), the narrow navigation dock (`#map-dock`), and the compact notice line with Details;
   - the F-2 unpaired-readout guard (`readoutState() === 'unavailable'` → commands locked).

   Removed: the `light-toggle`/`ground-mode` controls. Pass 1 deliberately replaced them with the overlay `<select>` and Inspect Cell (FR-7.1/7.3).
2. **Test integrity.** `MultiAreaTests.cs` MA-W12 stays an **exact** ordered-equality assertion and only adds the 4 new adapter members, so it is not weakened. `Stage4UiCatchupTests.cs` U-02 follows the FR-1.1 contract change. It still asserts (1,1,1) Stage 4 models on the default, adds an `ecology.Models == (1,1,1,1,1)` assertion, still checks that default trees equal the Stage 4 genesis, and still asserts the Stage 4 area-model token on an explicit Stage 4 open and the Legacy path. This is a legitimate contract update, not a weakening. Smoke-test changes are covered in N-3/N-4.
3. **Zero biology drift.** `MultiAreaContinuation.cs`: the new fields and methods are read-only, and `ReadoutOf` was split into `EnsureGroundCache` with the same cache key and the same computation. `MultiAreaUiReadout.cs`: `TreeOf` now calls `MultiAreaSpeciesPruneHelper`. For Sitka it is identical to `PruningRules.ScheduledRefusalCode` (`Phase2Pruning.RefusalCode(..., null)` delegates to `PruningRules.RefusalCode` with `NextLiftTargetM(lifts)`), and for broadleaves it uses the same `Phase2Pruning` functions the engine's `RequirePrunable` uses. `MultiAreaWorkPlanAdapter.cs` changes only the default selection and adds factories and reads. `MultiAreaExperiment.cs` has a `DisclosureOverride` that is null for MA-1, so the manifest bytes and hash are unchanged. No rule, constant, RNG stream or ordering changed, and all anchors hold.
4. **Frozen anchors.** §3.1: 9/9 MATCH.
5. **Atomic snapshot.** Confirmed single-lock (point 3 and MI-P1-01). Command, hit and competition responses pair via `CompositeFor(result.View)`, which re-checks `(source, planStateRevision)` under the lock and returns null as a whole. That is exactly FR-3.2. No HTTP route still uses the legacy `ReadSnapshot`/`ReadoutFor` pair.
6. **`POST api/multi/cell`.** Confirmed: strict camelCase (`RequireMember` exact paths plus `UnmappedMemberHandling.Disallow`), the 16 KiB cap, and 400/409 mapping (`StatusOf`: refusal → 409; guard failures → 4xx before Core is reached). Read-only, with byte-identical views before and after (E7).
7. **Payload budgets.** §3.2, measured myself; there is no rounding to 0.
8. **SVG system.** MI-P1-03; the Expired Shelter conflict rule is honoured.
9. **Scope.** MI-REG.
10. **Browser claims.** Re-run independently (§3.3) rather than accepted.

---

## 6. Findings

### 6.1 Blocking findings

**None.**

### 6.2 Non-blocking findings

| ID | File:line | Finding | Impact |
|---|---|---|---|
| N-1 | `Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass1/MULTI-MAP-INTEGRATION-PASS1-NEON-IMPLEMENTATION-20261124.md:92` | The handoff lists labels `MultiLightNotPar` and `MultiExpiredShelterRule`, which **do not exist**. The code adds `MultiLightCaveatShort`, `MultiDeadwoodSchematic`, `MultiClearUndergrowthRule`, `MultiNaturalSeedlingsNote` and `MultiNotModelledPhase2` (`MultiAreaContracts.cs:179-185`). Line 73 cites "brief §7.6 rule 4"; the spec section is §5.6 rule 4. The code wins. | Doc precision only |
| N-2 | same handoff `:182-189`, `:198-200` | The live table's "`ecology` bytes" (43,428 / 45,375 / 44,371 / 46,940) are not raw-byte spans. The raw spans I measured are 43,438 / 45,385 / 44,393 / 46,961 (§3.2), a gap of 10-22 B. "R3 … `650 <= 720`" does not match the Add-button bottom I measured, 644 (probably a different element). Both are well within budget. | Measurement wording; no false PASS |
| N-3 | `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh:776-779, 1114` | The J-section realignment loosened three text checks: the `(not a Crop Tree)` check was dropped, the release-reason exact-equality check became `includes('retained Crop Tree ' + aTree)`, and the rank-count check became self-referential with an `=== 5`. These J selectors (`reason-choice`, `[data-competitor]`, `Total pending:`) were already absent from the `0e777455` page, so the realignment is legitimate. Restoring exact-text checks in a later lane is recommended. | Minor coverage loss |
| N-4 | `ui-smoke-multiarea.sh:880-909` | The four `0e777455` R2(1) checks for the removed Inspect-ground panel became 2 Inspect Cell checks. The old "entering the mode keeps focus on the pressed toggle and does not scroll the inspector" assertion has no Pass 1 equivalent. My browser run covers the fold after leaving cell mode, but not focus-on-entry. | Minor coverage gap |
| N-5 | `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs:1704` | The JS fallback `pruneStatusOf` hard-codes `minIntervalYears: 5`, a pruning-rule literal in browser JS (D-020 spirit). It is only reachable when `ecology` is absent, which means Species-0 sessions, which are all Sitka, so the value is currently correct. A server-provided field would be cleaner. | Nit |
| N-6 | `PlayerMultiApi.cs:2777, 3074` | Felled stumps still carry a map `X` badge (`b-executed`), and the legend says "Executed work (History Log)". FR-6.4 rule 3 says Executed is "removed from active map badges". I read the stump X as an inherited Stage 4 *tree-state* marker rather than a Work Plan state badge, because no Pending/Approved badge remains after Execute. **Owner to confirm** the intended reading. | Owner-judgement note |
| N-7 | `PlayerMultiApi.cs:582` | The `#ccf-spot-clear` dashed ring is drawn at r = 93 in a 100-unit symbol box. The box is exactly world scale (`2·radiusM`), but the ring centreline sits at about 0.525 m rather than 0.564 m, with its outer stroke edge at about 0.55 m. | Cosmetic |
| N-8 | `PlayerMultiApi.cs:1676` | `speciesOf()` falls back to `sitka-spruce` when `ecology` is absent. On an unpaired frame in `ma-1-mixed` (only possible when the session moves concurrently), Oak/Beech crowns would briefly draw as Sitka until the next read. Commands are locked in that state. | Edge case |
| N-9 | `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs:31` | A comment still says `img-src data:` exists for "embedded spruce crown sprites". It is stale after WebP removal; the file is correctly left unchanged because the CSP must not change. | Nit |
| N-10 | `PlayerMultiApi.cs:86` | `GET api/multi/manifest` builds a full composite snapshot only to read the model tuple. | Efficiency nit |
| N-11 | (visual) `browser-out/*-02-areaB.png` | At map scale, `#ccf-oak` and `#ccf-beech` are both rounded clustered greens and only moderately distinct. An owner visual sign-off on species legibility is recommended. | Owner visual review |
| N-12 | `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaEcology.cs:474` | `isEffective` uses `IsEffective(Math.Max(year, InstalledYear))`, so a shelter in the window between Execute and its install year shows as active. This matches the spec's stated definition ("strict < expiryYear and not failed"), but it is not the raw engine value for that window. | Note only |

### 6.3 Bounded repair list

**Not required: the decision is APPROVED.** If the Manager wants polish before integration, the optional items are N-1 and N-2 (handoff text corrections) and N-3/N-4 (restore exact-text and focus assertions in the smoke test). None needs a spec, biology or scope change.

---

## 7. What I could not verify, and why

- **Unity Editor / Play Mode / Unity C# compile:** out of scope for this standalone lab; not available and not run.
- **Non-Edge browsers, screen readers, a real interactive desktop or touch:** only headless Edge over CDP was available.
- **The exhaustive every-tree loaded fold sweep (`fold-sweep-owner-ux.mjs`):** not run. I checked two loaded references per cell (4 blocked, and 2 blocked: no reference had exactly 3 blocked in my setup) at both viewports, plus the clean state after leaving cell mode.
- **Live rendering of planted juveniles, shelters, SpotClear patches and ClearUndergrowth orders in the browser:** Pass 1 has no write path that creates them, so I could not exercise them live. Their DTO is covered by Core `MI-P1 C-03`, and their render code was read, not observed.
- **Browser timing (fetch/parse/paint ms) and Year +5 payloads after an executed Work Plan:** not re-measured. My Year +5 measurement is after five plain years.
- **Human/owner visual acceptance of the SVG art against the four boards:** I compared screenshots with `Standard_SVG_Asset_Sheet_v2.png` myself, but owner sign-off is a separate judgement (N-6, N-11).
- **The implementer's evidence archive:** I read it (driver sources, smoke log header showing `73f9a389`) but relied on none of its numbers. `73f9a389..1f131826` differs only by the handoff file, so my runs at `1f131826` cover the same code.

---

## 8. Recommended checklist ticks (I have not edited the checklist)

Recommended for `specs/multi-map-integration-pass1-v1/checklists/requirements.md`:

- `[x] MI-P1-01` — Evidence: this review §4 (E7 default/mixed manifests, CLI 9/9, live round-robin 0 mismatches, single-lock code + live parity, C-01/C-02).
- `[x] MI-P1-02` — Evidence: §3.2 raw budgets; live floor/alignment; cell endpoint 200/400/409/413/415/403/404/405 with byte-identical views.
- `[x] MI-P1-03` — Evidence: 21 ids, zero WebP, unchanged CSP, live circular-crown DOM audit (4 × 244 crowns), rule review, sheet comparison; owner visual notes N-6/N-11.
- `[x] MI-P1-04` — Evidence: live mode swap/Escape/overlays/zero Pass 2 controls; HTTP Prune 11/11; smoke W/J.
- `[x] MI-P1-05` — Evidence: reviewer's own Edge matrix, 4 × 27/27 (§3.3).
- `[x] MI-P1-06` — Evidence: 0/0 builds ×2, 251/251 ×2 identical, smoke 170/170, hygiene, branch-only push.
- `[x] MI-REG` — Evidence: 9/9 anchors (§3.1), zero forbidden-scope paths.
- `[x] MI-P1-REVIEW` — Evidence: this file.

Recommended verdict lines: **Spec conformance: PASS. Implementation correctness: PASS (independent review, Claude Opus 5.5, `1f131826`).** State remains `NOT INTEGRATED` until the integration gates run.
