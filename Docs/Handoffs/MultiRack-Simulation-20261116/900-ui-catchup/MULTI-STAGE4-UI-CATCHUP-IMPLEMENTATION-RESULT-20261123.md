# MULTI Stage 1–4 `/multi` UI catch-up — implementation result

**IMPLEMENTED: yes. VERIFIED (author, Tier 2, real Edge): yes, with the exceptions listed. REVIEWED: no. OWNER ACCEPTED: no.**

| Field | Value |
|---|---|
| Task | `multi-stage4-ui-catchup-20261123` |
| Worker | Claude Opus 5.5 (Claude Code), implementation sub-agent |
| Role | Sole implementation writer for the D-091 lane (no subagents, no delegation) |
| Routing ids | decision `d-900-ui-catchup-impl-1`, attempt `att-20261123-02`, admission commit `d3a9f20f` |
| Lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4UiCatchup-20261123` |
| Branch | `feat/multi-stage4-ui-catchup-20261123-v1` (pushed to `origin`; no merge to `main`, no force push) |
| Code base | `500a50ae` (Stage 4 engine) |
| Admission | `d3a9f20f` (D-091 lane admission doc) |
| Candidate code HEAD | `541bb898` (all source, tests and harness); this handoff and the evidence are in the next commit on the branch |
| GitHub handoff path | `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/MULTI-STAGE4-UI-CATCHUP-IMPLEMENTATION-RESULT-20261123.md` on the branch above |
| Authority | Owner packet 2026-11-23 (four locked decisions), D-095, D-091; spec quartet `specs/multi-stage4-ui-catchup-v1/` (Status APPROVED, committed first as `18518b97`) |
| Tools actually used | Bash (Git Bash → `wsl.exe`), file read/write/edit, `/home/jer/.dotnet/dotnet` 8.0.134, WSL node v26.13.1 (smoke), Windows node v26.13.1 + headless **Microsoft Edge 157.0.4341.60** over CDP (real browser). **No Unity, no Editor, no Windows build.** |

## 1. Commits (base `d3a9f20f` → candidate)

```text
18518b97 spec(multi-stage4-ui): APPROVED quartet for the /multi Stage 1-4 UI catch-up
68d566cc feat(multi-stage4-ui): one-root UI read-out, scheduled Prune, 1/1/1 /multi default; atomic-refusal test
f9f54f1d feat(multi-stage4-ui): /multi payload carries the one-root Stage 4 read-out; fenced Prune routes
f23ffb59 feat(multi-stage4-ui): /multi page shows the Stage 4 read-out, Prune flow, Light Guide and fallen deadwood
527a4c02 test(ui-smoke-multiarea): stand-in cloneNode (J01 root cause); disclosure check follows the 1/1/1 default
541bb898 fix(multi-stage4-ui): browser-review fixes - band numbers at 1280x720, ground inspector in view, prune-only status
(next)   docs(multi-stage4-ui): implementation result handoff, evidence screenshots, browser driver
```

The spec quartet is its own commit and precedes every `src/**` edit.

## 2. What changed

### A. Core test — execute-time Prune revalidation (Stage 4 handoff §9 item 12)

`UI U-01` builds an approved mixed plan (one Fell in Area A, two Prunes in Area B, every guard valid at Approve), then drives one Prune guard false and Executes. **Finding:** through public commands the guard cannot turn false after Approve. Every change to a tree, its pruning history or the year moves the source token, so Execute refuses `WorkPlanStaleSource` first. Year+1 also cancels approved work. The test therefore injects the drift straight into the private `root` by reflection (the suite already reads `root` that way), keeping token and plan identical. It covers three guards: `PruneMaxLiftsReached`, `PruneIntervalTooShort` and `PruneTreeNotLiving`. For each it asserts:

- the stable refusal code, again on retry;
- the root reference is unchanged (nothing published);
- same token, plan-state revision and year 0;
- no work event;
- the plan still approved with the same PlanId and key, and the Fell order still listed;
- every tree bit-identical, including every crown radius;
- the approved Fell did not happen;
- no prune record; both Prune orders still approved; the valid Prune was not applied either.

A control run of the same plan without drift executes: Fell done, both radii × 0.92, year unchanged. **No production defect was found**, so the revalidation code is unchanged.

### B. Engine read-out and API (`src/Core/**`, `src/Ui/PlayerMultiApi.cs` C# part)

- `MultiAreaWorkPlanAdapter.OpenSynthetic(models = Stage4)`. The read-only `Program.cs` calls it with no argument, so **/multi runs 1/1/1**. `OpenSynthetic(MultiAreaModelSelection.Legacy)` gives the frozen 0/0/0 session (test U-02). `new MultiAreaContinuation(e)` stays model 0.
- New `MultiAreaUiReadout` (internal records + builder). It is built under one engine lock from the same root as the view, and cached with the canopy cache (per AnnualRevision/WorkRevision). It contains:
  - models: `crownModelVersion` / `adultModelVersion` / `mortalityModelVersion` (ints), `lightModelVersion = "unity-k10a10-decoupled-v1"`, light label, `calibratedPar: false`, the not-calibrated-PAR caveat, the synthetic-stand disclaimer, Stage 4 evidence labels, 2.5 m cells, b = 2.5 m;
  - per tree: `lifeState`, `deathCause`, `deathYear`, `pruningLifts`, `maxPruningLifts`, `crownBaseHeightM`, `lastPruningYear`, `pruneEligible`, `pruneRefusalCode`, `pruneRefusalReason`, `nextPruneTargetM`, `nextPruneEligibleYear`, `prunePlanState` (`PendingPrune` / `ApprovedPrune`);
  - per rack canopy summary: `geometricCoverFraction`, `geometricCoveredM2`, `geometricUncoveredM2`, `polygonAreaM2`, `openingCount`, `largestOpeningAreaM2`, `largestOpeningWidthM`, `relativeDensity`, annual and cumulative natural deaths, `fallenDeadwoodCount`, mean/min/max light index, and `fallenDeadwood[]` with engine X/Z and map U/V;
  - per rack `ground[]`: column/row, centre X/Z and U/V, clipped area, polygon-clipped cell vertices (X/Z and U/V), `lightIndex`, `geometricCoveredFraction`, `crossBorderTrees`, `contributingTreeIds`.
- `CanopyLightEvaluator`: three internal per-cell helpers repeat existing rules exactly: contributors are trees with influence > 0 in the same float kernel; the clipped square uses the same Sutherland–Hodgman clip; per-cell cover uses the same 0.25 m micro-grid, inside test and clipped weighting as Layer B. `Evaluate` is unchanged.
- `PruningRules.SitkaLiftTargetHeightsM = {2.5, 5, 6.5}` and `NextLiftTargetM` come from the verified Unity schedule (`ScenarioOne.asset` `pruningLiftTargetHeightsM`, read via `ScenarioOneDefinition.NextPruningTargetHeightM`). `MultiAreaContinuation.AddScheduledPrune` passes the engine's target into the unchanged `AddPrune`. The client never sends or computes a target.
- API: every `/multi` view, command, competition and hit payload gains an additive `stage4` member; no existing member renamed or removed. GET view reads view + read-out under one lock. A command's read-out is attached only if the root is still the command view's root; otherwise it is `null` and the page says so.
- New `POST api/multi/prune/add` and `api/multi/prune/remove` follow the Fell pattern:
  - unchanged `PlayerApi` guards; strict required members (full fence + `areaId` + `treeId`); duplicate, unknown and null members refused; length checks;
  - the engine fence; 409 with a fresh view on refusal;
  - Approve, Cancel, Execute and Year+1 are the existing shared routes.
- New Core labels (additive) for the 1/1/1 page: Stage 4 disclosure, connected-woodland statement and chip, Light Guide label, prune and deadwood notes. The manifest disclosure and content hash are unchanged (smoke M07 pinned hash PASS).

### C. Page (inside the `MultiAreaPage` constants)

- **Rack cards:** Canopy cover % · open m², Stand density RD, Natural deaths N this year · M logs total, and a model badge (`Model 1/1/1`, `Synthetic stand`, `Light: not PAR`). The badges show the full engine text as tooltips.
- **Tree inspector:** shows Pruning lifts X / 3 and the engine's eligibility or refusal reason, with the next eligible year where the 5-year guard applies. **✂ Add to Prune plan** is disabled while ineligible, with that engine reason shown. Remove-pending-Prune is available.
- **Prune states everywhere:** the Work Plan lists **✂ Pending Prune** / **✂ Approved Prune**. Map marks use a scissors tile plus a dashed line (pending) or double line (approved), with matching legend rows. The plan summary, area tabs and pane counts include Prunes.
- **Execute confirmation** freezes and re-checks the approved Prunes and names them. Its button reads "Confirm felling", "Confirm pruning" or "Confirm felling and pruning". The receipt shows the engine prune record (radius before → after, lifts), and the consent dialog lists Prunes.
- **Light Guide:** toggle in the map header, Off by default.
  - When On, it draws the polygon-clipped 2.5 m cells in five colour bands, with the band number written in every cell.
  - A legend row under the map explains the bands under the exact label "Modelled Light Guide (Unity k10a10 proxy — not calibrated PAR)".
  - A cell click opens the ground inspector: Modelled Light Index (0–1), overhead cover fraction, contributing tree IDs, cross-border trees, cell area and the engine caveat. It never touches the selection or unsent Fell drafts.
- **Fallen deadwood:** each record is a fixed-size log symbol (stem plus end-grain disc) at its engine map position, with a legend row "Fallen deadwood · log" beside Naturally dead · D and Felled tree · X.
- **Honesty text:** for a 1/1/1 session the disclosure, chip, map note, history line and Details pane say "connected woodland". The model 0 "edge neighbours excluded" statements would be false under decision 1. The Details pane lists models, the light caveat, the deadwood note, the evidence labels and the light uncertainties.
- **D-020:** the page computes no forestry, canopy, light, RD or eligibility value. It only formats engine values: a fraction × 100 for "%", banding the engine light index into five display bands, and rounding. The 2× competitor overlay is untouched.

## 3. Files changed (`d3a9f20f..541bb898`, plus the handoff commit)

```text
specs/multi-stage4-ui-catchup-v1/{spec,plan,tasks}.md, checklists/requirements.md          new (quartet)
Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaUiReadout.cs                         new
Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs                      ReadUiSnapshot/ReadUiReadout, AddScheduledPrune
Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaWorkPlanAdapter.cs                   OpenSynthetic(models), ReadSnapshot, ReadoutFor, AddPrune, RemovePrune
Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContracts.cs                         additive labels only
Tools/MultiRackSitkaP1Lab/src/Core/CanopyLightEvaluator.cs                                 internal per-cell helpers only
Tools/MultiRackSitkaP1Lab/src/Core/Stage4Records.cs                                        Unity Sitka lift schedule
Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs                                         payloads, prune routes, page constants
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Stage4UiCatchupTests.cs                         new (U-01..U-04)
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs                                      one registration line
Tools/MultiRackSitkaP1Lab/tests/Core.Tests/MultiAreaTests.cs                               MA-W12 exact adapter member list + 4 new names
Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh                                      stand-in cloneNode; disclosure check for 1/1/1 (mode 100755 kept)
Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/<this file>, evidence/*.png, browser-driver/*   new
```

Not touched:

- `src/Ui/PlayerApi.cs`, `src/Ui/Program.cs`, `src/Ui/wwwroot/**`;
- `Assets/**`, fixtures, `.sln`, `global.json`, `Directory.Build.props`, build scripts, `.p0ref`;
- `Tools/MultiRackSitkaLab/`, `active-tasks.md`, `main`, any other worktree.

`MA-W12` pins the exact adapter member list. I extended it with the four new names; the exact-match assertion, the no-delegate-seam check and the internal-types check are all unchanged.

## 4. Serialized Unity files / simulation and save impact / assets

- Serialized Unity files: **none**.
- Simulation/save impact: **none**. `/multi` is in-memory, nothing is saved, and the Core default ctor and every frozen path are unchanged (hashes below). `/multi` now *selects* the existing Stage 4 engine, per decision 1.
- Assets: **no new images were created.** The #885 owner-supplied crown sprites (inline WebP) and their provenance comment are unchanged. The log symbol, Prune tile and light cells are SVG drawn in code.

## 5. Must-not-change hashes (observed in the final gate on `541bb898`, 2026-11-23T10:17Z)

| Anchor | Observed | Result |
|---|---|---|
| Model 0 single-area F-01 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | unchanged |
| Model 0 MA-1 F-02 | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | unchanged |
| Stage 1–3 0/0/1 MA-1 (S4 V-02) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | unchanged |
| Stage 4 ten-year script (S4 R-02) | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | unchanged |
| P0 pin `.p0ref/SitkaLab.Core.dll` sha256 (before **and** after) | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | unchanged |
| /multi manifest content hash (smoke M07) | `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95` | unchanged |

## 6. Verification run (this lane only; `TMPDIR=/tmp/ui900/tmp`; `build-p0-core-ref.sh` not run)

| # | Command | Result |
|---|---|---|
| 0 | Baseline on `d3a9f20f` (same gate script) | sln 0/0, Core 180/180, 4 anchors + P0 present, Ui 0/0, ui-smoke-multiarea 107 pass / J01+J99 fail — matches the orchestrator's baseline |
| 1 | `/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` | **0 Warning(s), 0 Error(s)** |
| 2 | `/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` | **184/184 passed, 0 failed** (+4: UI U-01..U-04) |
| 3 | Anchors + `sha256sum .p0ref/SitkaLab.Core.dll` before/after | all unchanged (§5) |
| 4 | `/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` | **0 Warning(s), 0 Error(s)** |
| 5 | `DOTNET=/home/jer/.dotnet/dotnet bash tests/ui-smoke-multiarea.sh` | **110 passed, 6 failed** (J04, J05, J09, J10, J11, J99), triage in §8. All M (headers, CSP, `node --check`, pinned manifest), G (guards), W (86-check JSON workflow on the 1/1/1 default) and Z (clean SIGINT, no stderr, no file written, git status unchanged) checks PASS |
| 5b | `DOTNET=… bash tests/ui-smoke.sh` (single-area non-regression, extra) | 250 passed, 4 failed (L01, L02, L06, J31). No file it reads differs from base: `git diff 500a50ae HEAD -- src/Ui/wwwroot src/Ui/PlayerApi.cs src/Ui/Program.cs src/Ui/SitkaP1.Ui.csproj tests/ui-smoke.sh` is empty. J31 is the stand-in lacking `getBoundingClientRect` for `app.js` since #885 `b3a62953`. **Pre-existing; base result not re-observed** (no base checkout allowed) |
| 6 | Real server + real browser (§7) | **73/73 checks PASS** in the final run against the `541bb898` sources |

Ad-hoc API probe on a scratch server:

- `prune/add` 200 with an engine target of 2.5 m;
- a stale fence → 409 `PlayerPlanStateStale`;
- an unknown member → 400 `BadRequest`;
- `prune/remove` 200;
- Approve with zero Fell orders → 200;
- Execute → r 3.389092 → 3.117965 (ratio 0.920000), year 0, then `PruneIntervalTooShort` with next year 5;
- server stderr empty.

Protected listeners `:33767` (pid 1750123), `:43649` (pid 1806361) and `:46593` (pid 839989) were listening before and after every gate and server run and were never touched. Every scratch server bound an OS-assigned port, was stopped by its own PID (SIGTERM after a `/proc/<pid>/cmdline` check) and printed "Session ended; nothing was saved." with empty stderr.

## 7. Real-browser verification (headless Edge 157.0.4341.60, CDP, exact viewports)

- **Setup:** the scratch Ui host ran from this lane (`/tmp/ui900/run`, fixture copy). Windows node drove Edge over CDP with no packages, using a fresh temporary profile that was removed afterwards. Driver: `browser-driver/cdp.mjs` + `scenario.mjs`; the run record (`browser-results-20261123.json`, `browser-run-20261123.txt`) is token-redacted (0 capability matches).
- **Input:** clicks are real CDP mouse events at element centres. Engine numbers were cross-read in the page context with `fetch('api/multi/view')`.

| Check | Observation |
|---|---|
| Loads 1/1/1 | models `1,1,1,unity-k10a10-decoupled-v1, calibratedPar false`; C2 radii A 0.97–2.64 m (mean 1.73), B 1.82–3.97 m (mean 2.94) |
| Console / CSP / 5xx | 0 console errors, 0 exceptions, 0 `securitypolicyviolation`, 0 CSP log lines, 100 responses all 200. The only error-level browser log line in the whole run is the network line for my **deliberate** final 409 probe, recorded separately |
| No sideways scroll + stand clear of dock at Fit | PASS in all 8 states {1920×1080, 1280×720} × {legend shown, hidden} × {Light Guide off, on}, checked at the start and again after 6 years (16/16). `scrollWidth == innerWidth` (1920 / 1280); stand outline never intersects the dock and stays inside the map pane |
| Non-colour cues | every light cell carries its band number (144/144 cells at both sizes after the fix); 5-row numeric band legend; legend rows "Naturally dead · D", "Felled tree · X", "Fallen deadwood · log", "Pending Prune · ✂", "Approved Prune · ✂" |
| Light Guide + ground inspector | cell 11,3 (Area A): label exact; light 0.004, band 1, cover 100 %, 11 contributing trees; caveat "NOT measured or calibrated PAR". **Two unsent ticked Fell drafts and the reference were kept** across the cell click |
| Prune gate | MA-B-001: "Pruning lifts 0 / 3 · Eligible now … 2.5 m". Add → Pending: trees, cover and light bit-identical. Approve → no ecology change. Execute → confirmation "Prunes exactly 1 named tree in Year 0 …", "Confirm pruning"; r 3.3890924 → 3.1179652 (× 0.92), year 0; B cover 0.87872 → 0.87763, B mean light 0.14718 → 0.14755; receipt lists the prune record; inspector then "1 / 3 · Not eligible: a tree needs at least five years between pruning lifts (next lift possible in year 5)" with Add disabled and the engine reason |
| Ineligible refused | browser `POST prune/add` for felled stump MA-A-011 → 409 `PruneTreeNotLiving` (the code the read-out showed), plan-state revision unchanged |
| Fell 10+ in Area A | 16 trees queued (ticked multi-tree flow + "Fell this tree instead"); Work Plan "Area A · 16 pending / Area B · 0 pending"; Execute "Fells exactly 16 named trees …". Area A cover 96 % → 89 %, open 33 → 99 m², RD 0.80 → 0.71; local cell 11,3 light 0.004 → 0.606 |
| Cross-rack | Area B border cell 15,3 was shaded by A trees MA-A-044/059/060/076. Felling them in A raised its light 0.302 → 0.823. B's own summary stayed per area (0 deaths, its own RD) and the Work Plan listed the A orders under Area A only |
| Years 1–6 | felled-gap cell 11,3: cover 0 → 0.01, light 0.583 → 0.462 (falling every year); mean crown radius of the 8–9 living trees within 6 m 1.581 → 1.794 m. Natural deaths in A 0/4/3/2/5/3 → fallen logs 0/4/7/9/14/17; **drawn log symbols = deadwood records every year**; rack card "N this year · M logs total" matched every year |

The gap re-closes **locally**. **Area A total cover did not rise** (0.8951 → 0.8907 over years 1–6): natural deaths kept opening the canopy faster than crown growth closed it. This is the engine's behaviour, reported as observed and not hidden.

## 8. Screenshots

Captured in that final run at exact viewports, deviceScaleFactor 1. No capability token is visible: the page shows no URL and headless Edge has no address bar.

| State | 1280×720 (committed, `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/evidence/`) | 1920×1080 (committed, same directory) |
|---|---|---|
| (a) baseline map | `a-baseline-1280x720.png` 306,591 B | `a-baseline-1920x1080.png` 837,166 B |
| (b) Light Guide On + ground inspector | `b-light-guide-ground-1280x720.png` 231,626 B | `b-light-guide-ground-1920x1080.png` 553,371 B |
| (c) Prune Work Plan — Pending | `c0-prune-pending-1280x720.png` 224,676 B | `c0-prune-pending-1920x1080.png` 538,097 B |
| (c) Prune Work Plan — Approved | `c-prune-workplan-1280x720.png` 223,893 B | `c-prune-workplan-1920x1080.png` 537,973 B |
| (d) post-thinning gaps + fallen deadwood (year 6) | `d-thinning-gaps-deadwood-1280x720.png` 297,061 B | `d-thinning-gaps-deadwood-1920x1080.png` 844,777 B |

- **Committed set:** 10 PNGs, 4,595,231 B total (1280×720 1,283,847 B; 1920×1080 3,311,384 B). A plan cannot hold Pending and Approved Prunes at the same time, so state (c) has one image per state and five states exist per resolution.
- **Both resolutions are in the repository.** AC-9 was reconciled with the owner on 2026-11-23: the earlier ≤ 400 KB per image / ≤ 3 MB per set bound was an orchestration size cap, not a product requirement, and the tested UI does not meet it without changing the rendering. AC-9 now reads ≤ 0.9 MB each, ≤ 5 MB per set, both resolutions committed. The scratch copies under `/tmp/ui900/evidence/` are no longer the evidence of record; `browser-driver/scenario.mjs` can regenerate any of the images.

## 9. J01 / J99 triage (`tests/ui-smoke-multiarea.sh`)

- **J01 root cause (base):** the J witness's DOM stand-in had no `Node.cloneNode`. The #885 page's `drawGrid` calls `line.cloneNode(false)`, so the first `renderAll` threw before the tree table was filled, and J01 timed out. **Fixed** by adding `cloneNode` to the stand-in only (harness alignment; no assertion touched). J01 now PASSES.
- **Disclosure check** (base J04, now J03): updated to the Stage 4 equivalents ("CONNECTED WOODLAND" / "Connected woodland"). It now also asserts that the model 0 "EDGE NEIGHBOURS EXCLUDED" text is absent. Its other phrases are unchanged.
- **Remaining J04, J05, J09, J10, J11, J99 — pre-existing #885 drift, not fixed.** The witness was last edited in `7be3ff54`, before the #885 UI commits `3e6a6add`/`1e4e0221`. It still drives controls the base page (`500a50ae`) no longer has:
  - `[data-competitor]` buttons (0 occurrences in base and HEAD; #885 uses `data-draft` checkboxes) → J04, J05, J10, J11;
  - `insp-area` containing "MA-B" (#885 shows the label "Area B") → J09;
  - the removed `#reason-choice` select → the crash that makes J99 fail (`TypeError … reason-choice`).

  These were masked on the base by the J01 crash. Fixing them means re-authoring the J flow for the #885 controls, which goes beyond harness alignment, so I stopped there. **Never claimed as PASS.**

## 10. Spec conformance

| Requirement | Verdict | Evidence |
|---|---|---|
| FR-1 1/1/1 default; 0/0/0 reachable and tested | PASS | U-02; browser models; F-01/F-02/V-01 |
| FR-2 additive `stage4` payload | PASS | probe; browser; U-03 oracles |
| FR-3 engine prune target (Unity schedule) | PASS | U-04; API probe target 2.5 |
| FR-4 prune routes, Fell pattern, unchanged guards | PASS | probe (409/400); smoke G/W; `PlayerApi.cs` untouched |
| FR-5 no arithmetic in JS | PASS (author review) | §2 C; reviewer to confirm |
| FR-6 fallen-log symbols + legend | PASS | browser years table; screenshot d |
| FR-7 non-colour cues | PASS | band numbers 144/144; ✂ text; letters |
| FR-8 layouts, no sideways scroll, dock clear | PASS | 8 states (2 sizes × legend × Light Guide), checked at start and after 6 years: 16/16 |
| FR-9 atomic-refusal test | PASS | U-01 |
| AC-4 smoke no regression | PASS with exception: 110/6 vs base 107/2; the 6 are pre-existing #885 drift exposed by the J01 fix (§9) |
| AC-9 ten screenshots, both resolutions, ≤ 0.9 MB each, ≤ 5 MB set | PASS: 10 committed PNGs at 1920×1080 and 1280×720, 4,595,231 B total, largest 844,777 B; owner-corrected bound recorded in the spec |

## 11. Known limitations and UNKNOWNs (reviewer attention)

1. **Revalidation reachability.** The execute-time Prune guard is defence in depth that public commands cannot reach (§2 A). The test reaches it by reflection on the private root.
2. **Prune target rule.** The engine uses the Unity Sitka lift schedule 2.5 / 5 / 6.5 m. The owner packet did not name a target rule; this is the verified Unity source. I recorded it rather than inventing one, but the owner may want another.
3. **Stage 4 death drawing.** A natural death keeps its standing-dead crown and D badge (engine LifeState BiologicallyDead) and also gets the fallen-log symbol at the same position. Unity hides the tree and shows only a log. Recorded in spec "Open product decisions".
4. **Light Guide clicks.** With the Light Guide on, map clicks inspect ground cells. Trees stay selectable from the Trees table and the exact-ID box; crown clicks resume when the guide is off.
5. **Area-level re-closure.** After a 16-tree thinning, Area A total cover kept falling slowly for 6 years because natural deaths continued (§7). Local gap re-closure is real.
6. **Refusal wording.** For code `PruneTreeNotLiving` the read-out reason ("only living trees can be pruned", from `PruningRules.Message`) differs from the command refusal text ("only a living exact tree ID of this area can be pruned", from the existing Stage 4 guard). The codes match; I did not reword the existing engine message.
7. **Competition diagnostic.** It remains the frozen per-area Hegyi helper while Stage 4 growth uses the connected woodland. The page now says so in the competition details.
8. **Payload size.** `GET api/multi/view` is about 342 KB (ground cells and contributor lists for 383 cells), and every command answer carries the same read-out. That is fine on loopback; latency was not measured beyond functional use.
9. **Light bands.** The five display bands (0.2 steps) are formatting choices, not engine categories.
10. **1280×720 legibility.** The stand at 1280×720 is small (#885 baseline layout), band digits shrink to 6–10 px, and the map-status pill can sit over the lower stand edge.
11. **Not run:**
    - `tests/p0-isolation.sh`: its cold rebuild could rebuild `.p0ref`.
    - `build-output-safety.sh`, `cli-safety.sh`.
    - Unity, import, Windows build, any rendered review by the owner.
12. **Pre-existing failures.** The single-area `ui-smoke.sh` 4 failures and the multi J04+ drift are pre-existing harness drift (§6, §9).

## 12. Unrelated work preserved

The lane was clean at `d3a9f20f` and stayed clean apart from this task's files. No git command was run in the main checkout or any other worktree; no reset, clean, stash, checkout of other branches or deletion. Scratch files live only under `/tmp/ui900` and `C:\Users\hp\AppData\Local\Temp\ui900`.

## 13. Decisions needed (owner)

1. Accept or replace the Unity 2.5 / 5 / 6.5 m lift schedule as the /multi Prune target rule (limitation 2).
2. Stage 4 death drawing: keep the standing-dead crown + D with the log, or show only the log as Unity does (limitation 3).
3. Whether to authorise re-authoring the stale `ui-smoke-multiarea.sh` J flow for the #885 controls (and the single-area `ui-smoke.sh` stand-in) as a separate bounded task.

## 14. Assessment and next step

**Ready for independent review: yes, conditionally.** The conditions are owner decisions 1–2 above and acceptance that the remaining smoke J failures are pre-existing drift.

**Next step:** independent read-only review (per the admission, `opencode / gpt-6.1-sol`, separate context) against the quartet, this handoff and the five-anchor gate. Then owner acceptance. No merge to `main` by this lane.
