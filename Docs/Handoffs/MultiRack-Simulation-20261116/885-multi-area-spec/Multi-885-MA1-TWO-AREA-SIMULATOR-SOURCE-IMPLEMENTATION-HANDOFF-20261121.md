# Multi #885 MA-1 — two-area manual forestry simulator: source implementation handoff

Date: 2026-11-21. Author: Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, the **sole source implementer** for this lane, delegated by the DeepSeek V4.1 Flash orchestrator at the owner's direct instruction. **I am the author. Everything below is author verification: not an independent review, not a Manager G4 verdict, not integrated, not merged and not owner accepted.** No subagents or delegates were used.

**Classification: PLAYABLE CANDIDATE — AUTHOR-TESTED, NOT REVIEWED.** The two-area page, its JSON routes and the Core engine are implemented and pass the author gates listed in §3, including an author headless-browser run at 1280×720. An independent nonauthor Tier-2 review is the next step.

## 0. Exact candidate and custody

| Item | Value |
|---|---|
| Branch | `feat/multi-885-ma1-two-area-workplan-20261121-v1` |
| Base | `e11dd5192d7a89abf859613ce92814bdd6338ea2` (the #882 repaired source); worktree clean at start |
| **Final source SHA** | **`58950a9aa74067f22c9fe322b27f42e036ae2e01`**. Review this commit. The later commit that adds this report changes documentation only. |
| Worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-MA1` (only this worktree was written) |
| Machine | `DESKTOP-DL1BQI7`, WSL2 Ubuntu, user `jer`; builds and tests in WSL bash |
| Toolchain | `/home/jer/.dotnet/dotnet` SDK 8.0.134; WSL node v26.13.1; `TMPDIR=/tmp/ccf-ma1-tmp` (outside the repository) |
| Frozen inputs | `.p0ref/SitkaLab.Core.dll` SHA-256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (copied by the orchestrator, never rebuilt); P1 fixture SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`. Both unchanged at the end. |
| Protected processes | PIDs `390578` (#867), `1750123` (#877) and `1806361` (#882 owner playtest) were observed running, read-only, before and after my work. None was signalled, restarted or reused, and their ports were not used. |
| Own processes | Every UI process I started bound an OS-assigned loopback port. The smoke scripts stop their own processes with SIGINT. I stopped five helper-started processes myself with SIGTERM, each by PID after checking its command line: `2985412` (a scratch syntax check whose SIGINT was ignored because it ran as a background job; see §3.5), and the real-browser servers `2989974`, `2990778`, `2991333` and `3008412`. All printed `Session ended; nothing was saved.` or stopped cleanly. My headless Edge instances used fresh temporary profiles and were closed through the DevTools protocol; no Edge of mine and no profile directory remained. **No capability URL is published here.** |

### Stage commits

| Stage | Commit | Content |
|---|---|---|
| S1 | — | Read-only preflight. Baseline at `e11dd519`: Core **108/108**, `tests/ui-smoke.sh` **287/287**. |
| S2 | `904f1986f797019e6ae0cca5fb8e6642b2b88cc7` | Core multi-area engine, experiment, generator, geometry, projection, adapter; 27 new tests |
| S3 | `6e6f3516963645108819503356cd1e0c2a75a083` | Two-area page and `api/multi/*` routes on the same listener; `tests/ui-smoke-multiarea.sh` |
| S4 | `9977d4f915f441a0fdb3b22f7f1d630da6cd29e1` | Single-area G2 repair: read-only competition for any living tree; narrow smoke updates |
| S4b | `7be3ff54091144f37ae3c61e0fc86817bc989294` | 1280×720 layout pass of the two-area page after the author real-browser run |
| S4c | `58950a9aa74067f22c9fe322b27f42e036ae2e01` | Race fix: a competition answer never draws a view while a command is in flight (both pages) |
| S5 | the commit containing this file | This handoff only |

## 1. IMPLEMENTED

All paths below are under `Tools/MultiRackSitkaP1Lab/`.

1. **Two distinct synthetic areas** (`src/Core/MultiArea/MultiAreaGenerator.cs`, `MultiAreaExperiment.cs`, `MultiAreaGeometry.cs`). Experiment `mad-sitka-ma1-two-area-v1`, format `ccf-sitka-multiarea-experiment` v1, domain string `MultiAreaSitkaExperimentV1`. The domain is not a `P1Domain` value, and the `mad-` prefix is refused by the P1 loaders. The experiment is generated in code with integer SplitMix64 (generator seed `8850001`); no file is read.
   - **MA-A "Area A"**: a convex pentagon of exactly 915 m² (0.0915 ha). 171 trees in one 24-year cohort on a 2.24 m jittered grid. 1,868.9 stems/ha, BA 74.02 m²/ha, Dq 22.46 cm, RD 0.802. Simulation seed `885101`.
   - **MA-B "Area B"**: a convex quadrilateral of exactly 1,500 m² (0.15 ha). 73 trees in two cohorts (70 % aged 47, 30 % aged 29) on a 4.3 m jittered grid. 486.7 stems/ha, BA 39.50 m²/ha, Dq 32.15 cm, RD 0.438. Simulation seed `885202`.
   - Hectares come from geometry: the exact integer shoelace sum, then `(float)(twice/2e10)`. Tree IDs are area-prefixed (`MA-A-001` …), globally unique by ordinal and ignore-case comparison, and never `P30-`. No tree belongs to the P1 roster (all P1 trees are aged 30).
   - The two areas **share one boundary segment**: 13 A–B tree pairs lie within the 8 m Hegyi cutoff, so the edge exclusion is real, not hypothetical. Site is `UNKNOWN` in every area, and the engine's constant site value (1.0f) is used unchanged.
   - Identity rules are enforced as `P1Refusal` codes: `ExperimentIdInvalid`, `AreaCountInvalid`, `AreaIdInvalid`, `AreaIdOrder`, `PolygonInvalid` (strictly convex counter-clockwise, exact integers), `AreasOverlap` (exact separating-axis test; touching is allowed), `SeedNotUnique`, `AreaEmpty`, `TreeIdInvalid`, `TreeIdReservedFamily`, `TreeAreaMismatch`, `TreeIdOrder`, `TreeIdNotUnique` (defence in depth; the prefix, area-ID and order rules already make it unreachable, so it is not tested), `TreeIdCaseCollision`, `TreeOutsideArea` (a point on an edge is outside), `DuplicatePosition`, `AgeInvalid`, `DbhOutOfDomain` and `HeightOutOfDomain`.
   - **Provenance:** a canonical manifest (`CanonicalWriter`, strict ASCII JSON, 67,491 bytes). Its content hash is the SHA-256 of the body without `contentHash`: **`c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95`** (pinned in tests). It is served read-only at `api/multi/manifest` and carried in every token. It also records `reviewStatus: PendingIndependentReview`; the author never self-accepts. Its disclosure reads: *SYNTHETIC AUTHORED TWO-AREA SITKA EXPERIMENT. NOT A SURVEYED IRISH FOREST. AREAS NOT ASSESSED COMPARABLE. SITE UNKNOWN. EDGE NEIGHBOURS EXCLUDED BY THIS INITIAL MODEL. NO TREATMENT TRANSFER BETWEEN AREAS. NOT UNITY NUMERICAL OR GEOMETRY PARITY.*
2. **Read-only competition for any living tree in either area** (`MultiAreaContinuation.Competition`, `MultiAreaProjection.cs`). The frozen `PlayerCompetitionDiagnostics.Measure` runs **unchanged** on a transient single-area projection of the area. Only that area's trees and orders are visible to it, so there is no second Hegyi implementation. It returns the top five with share and strength, the current total CI and every neighbour, before any Crop designation and with zero state change. It never marks Crop and never proposes or performs a Fell. Refusals: `CompetitionTreeUnknown`, `CompetitionTargetDead`, `MultiAreaWrongArea`, `MultiAreaUnknownArea`, and the three fence refusals.
3. **Per-area annual ecology** (`MultiAreaAnnual.Step`, `MultiAreaContinuation.Advance`). There is one shared ecological year. Each populated area calls the **unchanged** `AnnualRules.Calculate` once, with its own trees, hectares and seed, and builds its next state exactly as `AnnualContinuation` does. Competition is area-local only. Results are deterministic and independent of evaluation order.
4. **One immutable root, one lock, one publish** (`MultiAreaRoot`). Every area's roster and Crop marks, the shared year, the annual/work/plan-state revisions, pending orders, the approved plan, the event ledger, the executed-key map and the annual history are published by a single reference assignment. This is not two composed `AnnualContinuation` sessions. Every read and command takes the lock once, and every returned collection is an owned read-only copy.
5. **One combined cross-area Work Plan.** `AddPending`, `RemovePending`, `Approve`, `CancelApproved` and `Execute` are fenced by session, the full token (12 scalar fields, value equality) and the plan-state revision.
   - **Approve fells zero.** The engine mints the PlanId `<session>:ma-plan:<n>` and the key `<PlanId>:execute`.
   - **Execute** revalidates the whole batch: area, Fell action, duplicate, living exact ID in its own area, Crop protection and reason. It then builds every area's roster and its before/after `WorkRules.Summarize` (unchanged), and publishes once. It returns **one event and one receipt** with every area's before/after living count and basal area and typed Fell records (`fell` / `manual-work-plan`). Test seams (`afterAreaBuilt`, `beforePublish`) exist only on the internal engine method.
   - **Replay:** the same key with the same PlanId and source returns the recorded receipt with `Replayed=true` and never fells twice. A different PlanId or source with the same key, or an unminted key, is refused with `WorkExecutionKeyConflict`.
   - Refusals for stale year, revisions or plan state, wrong area, duplicate, dead or unknown IDs, and Crop-protected trees leave both areas, the events and every revision byte-identical.
6. **Owner safeguards.** A batch that would leave **any** populated area with no living tree is refused whole (`WorkWouldLeaveNoLivingStems`), and the approval stays. An area emptied by natural mortality is carried forward unchanged (`NoLivingStems=true`; the engine is not run for it) while other areas advance. Year+1 is refused (`ContinuationNoLivingStems`) only when every area is empty. Pending or approved work blocks Year+1 (`WorkPendingConsentRequired`) until the player explicitly consents; consent cancels **all** unexecuted work in both areas at zero Fell, and the year record shows `CancelledUnexecutedWork`. Crop marks on trees that die naturally are cleared in the same publish as the year.
7. **A genuine two-area 2D page** at `/s/<capability>/multi`, served by the same single listener and the same guards:
   - an A/B area switch that remembers each area's selection, and a north-up map of true positions and crown radii (from the frozen `PlayerMapScene`, unchanged);
   - real-click selection through the server's exact-ID hit-test;
   - an inspector showing the area, exact ID, status and a size summary, with **read-only competition shown at once** (ranked top five, numbered rings on the map, total CI);
   - Crop mark and unmark; add and remove Fell with a truthful reason: either "Competition release for retained Crop Tree <id> (operator intent)", offered only for a listed competitor of an inspected Crop, or a typed "Other" reason;
   - one combined Work Plan with A and B sections and per-area plus total counts; Approve; Cancel; Execute through a frozen modal confirmation whose focus starts on Back;
   - one receipt table per area, and Year+1 with per-area results and history;
   - a fail-closed lock after an unconfirmed outcome; floating zoom +/−/Fit and N/W/E/S pan controls on the map; an inset showing both polygons and their shared edge;
   - a visible disclosure line plus chips for "Site UNKNOWN", "Edge neighbours excluded" and "No treatment transfer" (each chip carries its full statement as a tooltip, and the Details pane lists the full statements).
   Exact tree IDs are readable; the session ID, PlanId, key and hash are collapsed behind expandable details.
8. **Single-area G2 repair (bounded).** `api/competition` now answers for any living tree; the `CompetitionCropRequired` branch is removed. Fences, the strict request shape, dead/unknown refusals and the read-only behaviour are unchanged. In `app.js`, selecting any living tree by table, exact ID or map reads its competitors at once, and Inspect is enabled for any living tree. The report says "not a Crop Tree" when that is true, and Crop mark/unmark keeps the tree inspected. The release reason stays gated to a listed competitor of a retained Crop Tree.

### 1.1 Deviations from the written envelope, stated plainly

- **No `src/Ui/wwwroot/multi.*` files were created.** `src/Ui/SitkaP1.Ui.csproj` embeds exactly three named page files (`index.html`, `app.css`, `app.js`) and removes everything else under `wwwroot/`, and both csproj files are outside the envelope. New `wwwroot` files would therefore never be served (and serving files from disk would break the no-file-read rule). The two-area HTML, CSS and JS are compiled from constants in the authorised `src/Ui/PlayerMultiApi.cs` (`MultiAreaPage`) and served from memory under the same strict CSP, with no inline script or style. **Recommended follow-up:** with an explicit grant, add three `EmbeddedResource` lines to the UI csproj and move these constants into `wwwroot/multi.*` unchanged.
- **`PlayerApi` became `partial`.** The multi-area handlers in `PlayerMultiApi.cs` reuse PlayerApi's private guards and helpers instead of copying them. The other `PlayerApi.cs` edits are one constructor parameter, one field assignment, one `AddMultiAreaRoutes()` call and the G2 handler change. `HandleAsync` and every existing guard and route are unchanged.
- **`app.js`: one added race guard** (`state.busy` in `refreshCompetition`, S4c). It belongs to the any-tree change: without it, the newly frequent diagnostic reads made `tests/ui-smoke.sh` intermittently fail (§3.5).
- **`tests/ui-smoke.sh`: three edits, check count unchanged at 287, mode `100755` kept.** K29 now expects 200, a report, no Crop and an unchanged view for a living non-Crop tree (it previously expected 409 `CompetitionCropRequired`). One `until` line was added before the Crop click in the page witness, requiring competitors of the non-Crop tree first. J24 now ignores read-only `api/competition` requests when it checks that Approve was the last command.
- `Program.cs` prints one extra line after the unchanged launch lines: `Two-area MA-1 simulator: http://127.0.0.1:<port>/s/<capability>/multi`.

## 2. Files changed (15; `git diff --name-status e11dd519..58950a9a`)

| Path (under `Tools/MultiRackSitkaP1Lab/`) | Status | Stage |
|---|---|---|
| `src/Core/MultiArea/MultiAreaContinuation.cs` | new | S2 |
| `src/Core/MultiArea/MultiAreaContracts.cs` | new | S2, S4b (chip labels) |
| `src/Core/MultiArea/MultiAreaExperiment.cs` | new | S2 |
| `src/Core/MultiArea/MultiAreaGenerator.cs` | new | S2 |
| `src/Core/MultiArea/MultiAreaGeometry.cs` | new | S2 |
| `src/Core/MultiArea/MultiAreaProjection.cs` | new | S2 |
| `src/Core/MultiArea/MultiAreaWorkPlanAdapter.cs` | new | S2 |
| `tests/Core.Tests/MultiAreaTests.cs` | new | S2 |
| `tests/Core.Tests/Program.cs` | +1 registration line | S2 |
| `src/Ui/PlayerMultiApi.cs` | new | S3, S4b, S4c |
| `src/Ui/PlayerApi.cs` | modified | S3, S4 |
| `src/Ui/Program.cs` | modified | S3 |
| `tests/ui-smoke-multiarea.sh` | new, `100755` | S3, S4b |
| `src/Ui/wwwroot/app.js` | modified | S4, S4c |
| `tests/ui-smoke.sh` | modified | S4 |

None of the protected files changed: no `AnnualRules`, `AnnualEngine`, `WorkContract`, `Domain`, `FixtureAccess`, `DerivedManifest`, `TrustTables`, `NoFollowRead`, `ManifestGenerator`, `PlayerSessionView`, `PlayerWorkPlanAdapter`, `PlayerMapScene` or `PlayerCompetitionDiagnostics`, no csproj, `.p0ref`, fixture, `src/Cli`, `index.html`, `app.css`, `Assets/` or Unity, and nothing on `main`. All multi-area types are `internal`, so the public #849 player surface and PS-19's exact public type list are unchanged.

## 3. TESTED (observed by the author; exact commands)

```bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-MA1/Tools/MultiRackSitkaP1Lab
export TMPDIR=/tmp/ccf-ma1-tmp
~/.dotnet/dotnet build MultiRackSitkaP1Lab.sln            # 0 Warning(s), 0 Error(s)
~/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj           # 0 Warning(s), 0 Error(s)
~/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
tests/ui-smoke.sh
tests/ui-smoke-multiarea.sh
```

### 3.1 Results at the final source SHA `58950a9a` (clean tree)

| Gate | Result |
|---|---|
| Core tests | **135/135** (108 existing, unchanged and passing, + 27 new MA tests) |
| `tests/ui-smoke.sh` | **287/287** (log header `HEAD 58950a9a…`). The same code passed 8 consecutive runs at 287/287 before it was committed. |
| `tests/ui-smoke-multiarea.sh` | **127/127** (log header `HEAD 58950a9a…`), plus 2 earlier runs of the same code at 127/127 |
| Author headless-browser run (§3.4) | **17/17** |

### 3.2 The 27 new Core tests (`tests/Core.Tests/MultiAreaTests.cs`)

- **Identity, MA-F01…F05:** deterministic generation and the pinned manifest hash, recomputable from the published file alone; distinct shape, size, stocking, age/DBH structure, competition and seeds; unique area-prefixed IDs, no P30 and no P1 tree, and refusal of the family by `P1DomainWire.Parse` and `FixtureAccess.ReadDerived`; 23 mutation refusals, each for its own code; exact geometry (shared edge and corner touch are not overlap; containment and identity are).
- **Ecology, MA-E01…E06:**
  - E01: the per-area step reproduces the **ten frozen P1 no-work digests**, a one-area equivalence oracle against `ContinuationTests`.
  - E02: Year+1 per area is byte-equal to a standalone `AnnualRules.Calculate` of that area with its own seed and hectares. The other area's seed would change A's year, and evaluation order does not matter.
  - E03: ten deterministic years across two sessions.
  - E04: **an isolation proof on the 13 real cross-boundary pairs**. CI equals the A-only annual CI and would be larger if B's trees counted.
  - E05: a naturally emptied area is carried forward while the other advances; all-empty Year+1 is refused; the Crop mark of a dead tree is cleared. This uses a test-only experiment whose seed is found deterministically under the real rules, with no seam.
  - E06: a fault after area A, or before the commit, publishes nothing; the retry commits once.
- **Competition, MA-C01…C02:** for **all 244 living trees**, CI bits and neighbour count equal the annual calculation; read-only; never auto-Crop or auto-Fell; refusals for unknown, dead, wrong-area, unknown-area and stale requests leave everything unchanged.
- **Work Plan, MA-W01…W14:**
  - W01–W03: Crop and Fell in both areas, Approve fells zero, Execute with one event and receipt per area, and Year+1 on real survivors.
  - W04: replay, altered replay and unminted key.
  - W05: 14 stale-fence variants × 8 commands, plus hit-test and competition.
  - W06: wrong area, unknown, dead, duplicate and Crop protection.
  - W07: the zero-survivor safeguard.
  - W08: a fault after A's roster and before B's leaves both areas unchanged.
  - W09: consent.
  - W10: concurrent readers never see a torn view, and a racing Execute and Year+1 commit exactly one.
  - W11: read-only, immutable views.
  - W12: token JSON round trip; the facade has no seam; every MA type is internal.
  - W13: map hit-tests.
  - W14: a one-area plan and a no-action year.

### 3.3 `tests/ui-smoke-multiarea.sh` (127 checks; two fresh sessions, each its own process stopped with SIGINT)

- **M01–M07:** the page, style and script are served with the strict headers (CSP self only, no-store, nosniff, no-referrer, DENY, no CORS, no Server); `node --check` passes on the served `multi.js`; the manifest body hash equals `contentHash`, the token, the experiment record and the pinned value.
- **W001–W086** (Python over HTTP):
  - the guards on the new routes: Origin 403, Host 403, capability 404, method 405, 415, 413; 400 for malformed, null, missing-token-member (each of 12), unknown, case-shifted, duplicate, string-in-number, empty or whitespace IDs, an over-long reason and missing hit or consent members. Every refusal leaves the view unchanged.
  - competition for non-Crop trees in A and B, with no change;
  - Crop; a release Fell in A and a typed Fell in B; stale, wrong-area, Crop-protected, duplicate, unknown-area and blank-reason refusals;
  - Approve 0; an unminted key refused; Execute with A 171→170 and B 73→72 in one receipt; replay once; altered replay refused; a felled ID refused;
  - Year+1 refused with the pre-Execute fence, then committed (A 170→165 with 5 natural deaths, B 72→72);
  - consent; hit-tests; and **72 HTTP adds of every living B tree, approved and then refused whole by `WorkWouldLeaveNoLivingStems`**, with the plan still approved.
- **J01–J22** (the served `multi.js` in the DOM stand-in copied from `ui-smoke.sh`):
  - any-tree competitors before Crop, with no server change;
  - the area switch, and a B map click hit-tested in B (the topmost overlapping crown `MA-B-020` was selected for a click at `MA-B-021`'s centre, as designed);
  - a typed reason in B; a release reason in A, offered only for a listed competitor of the inspected Crop;
  - combined counts; Approve 0; the confirmation text and focus on Back; Execute sends the confirmed PlanId and key; the receipt shows both areas; Year+1 results per area;
  - a lost Add locks every command until a re-read; no script errors.
- **Z01–Z04:** clean SIGINT exits, no server stderr, no file written in the lab tree, git status unchanged.

### 3.4 Author real-browser run (headless Microsoft Edge, 1280×720)

A scratch driver **outside the repository** (`C:\Users\hp\AppData\Local\Temp\ccf-ma1-browser\drive.mjs`, Windows node v26.13.1) started its own headless Edge (`--headless=new`, fresh temporary profile, OS-chosen DevTools port). It drove my own WSL UI process through the DevTools protocol with **synthetic** mouse, key and text events. The browser did the real layout, hit-testing and event dispatch; no human input was involved. The final run, at `58950a9a`, passed 17/17:

- B01–B04: the page fits 1280 px with no horizontal scroll, in two columns. The map pane is **795×419 px** and the side panel 435×546 px. The zoom and pan controls float on the map, and Year+1 and the disclosure are on screen.
- B05–B06: a real click on a living A crown (`MA-A-145`) showed ranked competitors at once, "not a Crop Tree", with total Hegyi CI 6.0692, numbered rings, and no Crop or Fell on the server.
- B07–B11: Crop; the release reason chosen with the keyboard (ArrowDown); an A Fell; the B switch, a crown click and a typed reason; combined counts 2 (A 1 · B 1); Approve 0; the modal confirmation "Fells exactly 2 named trees across 2 areas in Year 0, or refuses all." with focus on Back; Execute felling exactly `MA-A-146` and `MA-B-058`; Year+1 with a row per area.
- B12: Tab order from the title: `advance`, `reread`, `MA-A`, `MA-B`, Legend, zoom +/−/Fit, pan N/W/E/S, then the side tabs.
- B13: zoom, pan and Fit change and restore the map view.
- B14–B15: 360 and 640 CSS px (640 is 200 % zoom of 1280) have no horizontal page scroll.
- B16: **single-area page:** a real click on non-Crop `P30-044` shows its competitors; no Crop is created.
- B17: no script or console errors.

Screenshots and a JSON summary are **local, ephemeral scratch evidence** in WSL `/tmp/ccf-ma1-tmp/browser-evidence/` (`01-multi-initial…` to `07-single-area-any-tree-competition…`). They are not committed (outside the envelope) and not copied to shared verification storage.

### 3.5 Failures and attempts preserved

1. **First multi-area smoke attempt (S3): 107 passed, 15 failed**, all in the page witness. Cause: a test-design error. The witness reused the session the HTTP workflow had already changed (a Crop was already marked; `MA-B-021` was already felled). Fix: the script now starts a fresh process (a fresh session) for the page witness.
2. **First author browser run: 17/17 PASS, but its screenshot showed a real defect.** At 1280×720 the two-area map pane was 795×339 px, about 180 px of drawing, and the competitor list was below the fold. The DOM stand-in cannot see layout. Fixed in S4b (map pane 795×419; competition directly under the selected ID).
3. **`tests/ui-smoke.sh` at `7be3ff54`: 278/287 (FAIL J17, J23–J27), intermittent.** Two earlier runs of the same `app.js` had passed. Cause: competition is now read for any selected tree, so the page witness's lost-Add step had a competition read in flight. The server processed the Add first and answered the read with 409 stale plus a fresh view, and the page drew that view before the lost Add was handled. Fixed in S4c with the `state.busy` guard; afterwards 8 consecutive runs passed at 287/287, and the run at `58950a9a` passed 287/287.
4. **A scratch syntax-check helper hung on `kill -INT`.** A background job in a non-interactive shell ignores SIGINT; the smoke scripts avoid this with `set -m`. I verified PID `2985412` was my own process by its command line, then stopped it with SIGTERM.
5. **The S2 commit message was first truncated** by shell quoting at an apostrophe. I corrected the message of my own unpushed commit, with no content change, before any push.

## 4. NOT VERIFIED

- **No independent nonauthor review, no Manager G4 and no owner G2.** Nothing here is integrated, merged or accepted.
- **Native human input:** physical mouse, keyboard or touch by a person; real touch devices; screen readers; OS high-contrast or forced-colours mode; real 200 % browser zoom on a physical display (only a 640 CSS px emulation was measured); browsers other than headless Edge.
- **Science.** The experiment is synthetic. Its numbers (for example A's BA of 74 m²/ha and RD of 0.80) are teaching choices, not field values or calibration. The areas are not assessed comparable, and there is no MA-2, G1 comparator or treatment transfer. Site is UNKNOWN. Cross-boundary competition is deliberately excluded.
- **Unity, save/load, durability and multi-process behaviour.** The session is in memory; process exit ends it.
- **The single-area page's own map layout at 1280×720.** The pre-existing #868 design is unchanged: `index.html` and `app.css` are outside the envelope. Its small map pane is visible in local screenshot 07, and the owner's secondary map-controls request for that page is **not** done.
- Performance beyond 244 trees; long sessions beyond the 10–12 years exercised.

## 5. BLOCKED

Nothing in the authorised scope is blocked. Two items need a separate grant if wanted:

1. A three-line `SitkaP1.Ui.csproj` change so the two-area page can live in `wwwroot/multi.*` (§1.1).
2. A grant covering `index.html` and `app.css` for the single-area map-control overlay (the owner's secondary #882 request).

## 6. How to play it (observable behaviour)

1. Build as in §3, then run `~/.dotnet/dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived` from `Tools/MultiRackSitkaP1Lab`. Open the printed `Two-area MA-1 simulator:` address exactly (the single-area page stays at the `Open` address). Ctrl+C ends the session; nothing is saved.
2. **Expect:** two area buttons (Area A · MA-A, 171 living, 0.0915 ha; Area B · MA-B, 73 living, 0.1500 ha), the yellow disclosure, and chips for "Site UNKNOWN", "Edge neighbours excluded" and "No treatment transfer".
3. Click any crown in A. The Tree pane shows the exact ID and **its ranked competitors immediately**: top five with share and strength, and the total Hegyi CI. Numbered rings appear on the map. **Nothing is Cropped or felled.**
4. Choose *Retain as Crop Tree*, then click competitor 1. The reason list now offers "Competition release for retained Crop Tree <id> (operator intent)". Add it; the Work Plan shows it under Area A.
5. Switch to Area B, select a tree, choose *Other* and type a reason, then add it. The Work Plan reads "Total pending: 2 (Area A 1 · Area B 1)".
6. *Approve whole two-area Work Plan — 0 trees felled.* Living counts are unchanged.
7. *Execute approved two-area plan.* The confirmation reads "Fells exactly 2 named trees across 2 areas in Year 0, or refuses all." Confirm, and one receipt shows A 171 → 170 and B 73 → 72 with basal area before → after, and stumps appear on both maps.
8. *Advance one ecological year (both areas).* Results show each area separately; in the reference run A lost 5 trees to natural thinning and B none.
9. Optional refusals: add a pending order, then Year+1 asks for consent (Keep or Cancel ALL); a plan felling every living tree of an area is refused whole.

## 7. Next actions (not started by me)

1. An independent nonauthor **Tier-2 review** at source SHA `58950a9aa74067f22c9fe322b27f42e036ae2e01`: re-run §3, read §1.1 and §3.5, and do a real-browser and assistive-technology check.
2. A Manager #885 **G4** integration verdict.
3. Owner **G2** personal play test.
4. Optional grants listed in §5.

Branch: `feat/multi-885-ma1-two-area-workplan-20261121-v1`. Do not merge from this handoff.
