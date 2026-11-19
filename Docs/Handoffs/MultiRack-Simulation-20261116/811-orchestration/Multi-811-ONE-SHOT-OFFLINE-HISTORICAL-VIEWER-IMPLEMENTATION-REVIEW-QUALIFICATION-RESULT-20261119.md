# Multi #811 — one-shot offline read-only historical Sitka viewer: implementation, independent review and qualification result

**19 November 2026. Local prototype result. Not integrated, not released, no main merge, no L4 waiver.**

| | |
|---|---|
| IMPLEMENTED | **yes** — new additive offline read-only viewer + source-bound read adapter |
| SUPPORTED YEARS | **Year 0 (Genesis) and genuine sequential Year 1–10 committed states** |
| LOCALLY QUALIFIED | **yes** — 49 focused tests, real-browser smoke, protected hashes, deterministic offline replay |
| INDEPENDENTLY REVIEWED | **yes** — Claude Opus 5.5, nonauthor changed-lines review and focused re-check (§8) |
| INTEGRATED | **no** — local branch only; no code push, main merge or PR |
| ACCEPTED / RELEASED | **no** |
| UNITY / WINDOWS PLAYER | **not run / not built** |
| .NET L4 FAIL38 | **UNWAIVED** |

## 1. The result

A usable, offline, read-only 2D history viewer for the trusted Year 0 Sitka forest and the genuine later committed years, with year navigation and tree/stand inspection, source-bound admission, one independent nonauthor review and focused source/refusal/bit/hash/two-viewport browser checks.

Runnable file:

```
/home/jer/CCF-Dev/Multi-20261119/_811/history/Tools/MultiRackSitkaHistoryViewer/dist/year0-10-history-viewer.html
```

Windows view: `\\wsl.localhost\Ubuntu\home\jer\CCF-Dev\Multi-20261119\_811\history\Tools\MultiRackSitkaHistoryViewer\dist\year0-10-history-viewer.html`.

Open it directly (`file://`). No server, install or network is needed. It is one self-contained HTML file, 1,143,774 B, SHA-256 `8e35dd6df00c97b879ae02dc36453e878a4bb85a8490b304ff2c02a628b1d27f`.

## 2. Actors and independence

| Role | Actual identity |
| --- | --- |
| Orchestrator / source author | DeepSeek V4.1 Flash 1017, `opencode-go/deepseek-v4.1-flash`, native T3 Code + OpenCode Code Mode, `jer-laptop` WSL2 Ubuntu, GitHub `jermur788` |
| Independent nonauthor reviewer | Claude Opus 5.5, `claudeAgent/claude-opus-5-5`, distinct provider and session; changed-lines review and focused fix re-check |
| Manager record | [#730](https://github.com/jermur788/CCF/issues/730) |

No GPT-6.2 was used. The reviewer did not write the implementation. The review and re-check records are in `evidence/independent-review.md` and §8.

## 3. Gates, ownership and worktree

- **RUNNING registration:** [#811 comment 7008296576](https://github.com/jermur788/CCF/issues/811#issuecomment-7008296576), posted before the first source edit.
- **New isolated ext4 worktree:** `/home/jer/CCF-Dev/Multi-20261119/_811/history`, branch `feature/multi-811-offline-history-viewer-20261119-v1`, created from the restored Core head `c4124b63252fb563dc7d84c9fc7fc4e3e0d8d2d1`.
- **Candidate head:** `896132373542023ccbd0e4ed9d96826202ac4e81` (three additive commits: implementation, review fixes, evidence refresh).
- **Source heads inspected, not edited:** Core `c4124b63`, Viewer `f3537fe3`, Year2–10 engine `6f9d2e99`.
- **Local physical ownership:** user `jer`; no foreign worktree lock and no competing Unity/dotnet/browser process observed. The external ZX20/Files Ops board was **not mounted** in this WSL session, so the external physical lease was not re-read; no external Ops write was attempted.
- **Write ceiling:** all Git-tracked writes are new files under `Tools/MultiRackSitkaHistoryViewer/**`. No protected source, Asset, ProjectSettings, package, save, Host or main file was edited.
- **V-01 prospective inputs:** exactly the two public compile-linked blobs `SitkaGrowthModel.cs` (`e4c73402…`, SHA-256 `29160104…`) and `SimulationRandom.cs` (`bb83d7f8…`, SHA-256 `d1b52a60…`) were copied task-locally; the originals were unchanged. This is not a retrospective #789 waiver and not a V-02 main claim.

## 4. Implemented files

All under `Tools/MultiRackSitkaHistoryViewer/`:

| Path | Purpose |
| --- | --- |
| `src/Exporter/Program.cs`, `SitkaHistoryExporter.csproj` | Task-local read adapter. Compiles unmodified #789 Core mirrors + the two V-01 inputs into one assembly, opens the fixture through the engine's own admission path, performs the genuine sequential commits, verifies Year1/digests/roster/bits/death freeze, and emits a detached bit-word package. The project file is tracked. |
| `web/history-admission.js` | Source-bound admission of the detached package; typed refusals; no network. |
| `web/history-map.js` | North-up Canvas math/renderer; one nearest-centre hit test at 8 CSS px, ordinal tie-break, explicit MISS; CSS/DPR separation; fitted-scale zoom limits. |
| `web/history-ui.js`, `history-boot.js`, `shell.template.html`, `history.css` | Year navigation, committed/diagnostic stages, inspection, case-sensitive exact-first ID search, roster, keyboard ID route, dead-stem X marks, model-derived stand strip and H0 summary. |
| `build/export-history.sh`, `assemble-viewer.py` | Offline build and single-file packaging with restrictive CSP. |
| `tests/run-tests.js` | 49 focused admission/bit/refusal/map/UI/static checks, including the package embedded in the shipped HTML. |
| `tests/verify-offline-and-protected.sh` | Forced rebuild from committed sources, deterministic `unshare --net` replay and protected-baseline hash checks. |
| `README.md`, `SOURCE-MAP.md`, `build-inputs/*` | Run recipe, field map and pinned mirror provenance. |
| `dist/year0-10-history-viewer.html` | The runnable deliverable. |
| `evidence/*` | Test output, protected-hash log, browser smoke, screenshots, independent review. |

The exporter does **not** modify the #789 engine. The engine Core files are copied unmodified into this task's `build-inputs/engine/` mirror and compiled into the exporter assembly.

## 5. Supported years and actual source proof

| Year | Stage | Admission proof |
| --- | --- | --- |
| 0 | Genesis | 32,567 B fixture, SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`, content hash `b7dee8a3…`; 100 living `P30-001…P30-100`; exact source f32 `unityX`, `unityZ`, `dbhCm`, `heightM`, `crownRadiusM`; f32 area `3db851ec`. Annual stand/hazard **NotEvaluated**. |
| 1 | Committed `AfterMortality(1)` | The engine's genuine `Advance(0,0)` commit; the recreated original #725 CLI envelope is **byte-identical** to the preserved 45,980 B stdout, SHA-256 `574d8746…`; 0 deaths, 100 ages 30→31, reported BA 55.872909 → 58.339545 m²/ha, Dq 25.303288 → 25.855792 cm, RD 0.609796 → 0.637584. |
| 2–10 | Committed `AfterMortality(y)` | Nine further sequential `AnnualContinuation.Advance(y-1, y-1)` commits in one experiment; every per-year digest reproduces the #789 recorded anchor; revision 1–10; no jump, no fast-forward, no fresh Year1 rerun. |

Per-year recorded digests reproduced exactly:

```
1  df2d44fd…  0 deaths
2  23a1d889…  0 deaths
3  953fc7ef…  1 death  P30-044
4  6fc220c3…  0 deaths
5  cbd51e38…  0 deaths
6  01cfb656…  1 death  P30-079
7  c6baab55…  2 deaths P30-026, P30-053
8  61879c1e…  4 deaths P30-064, P30-067, P30-073, P30-093
9  faaea476…  5 deaths P30-016, P30-018, P30-022, P30-035, P30-038
10 10d31913…  0 deaths
```

Total 13 natural `self-thinning/hazard` deaths; 87 living at Year 10. Every archived row is frozen bit-for-bit after death and carries its death year/cause/mechanism. No Year 2–10 frame comes from a new Year1 process, a forecast, a fabricated snapshot or a missing year.

## 6. Read behaviour

- Default view: **H0 Genesis Year 0 → committed Year 1 After mortality**, with the exact stand comparison and the model-derived “all 100 ages 30 → 31” statement.
- Year strip: Year 0–10. Dead-year badges appear on Years 3, 6, 7, 8 and 9.
- Stage tabs: the committed active state plus `Before` and `Post-growth` diagnostics of the same completed transaction. A victim that dies in `AfterMortality` is visible alive in `PostGrowth` for explanation only.
- Inspection: ID, status, age, DBH, height, crown, source XY-mm, centred X/Z, f32 bit words, death metadata, per-year deltas and the transition's CI/neighbour/suppression/hazard/roll.
- Map: north-up 30 m source plane, crown circles from the source crown radius, centre dots, dead-stem X marks, 8 CSS px nearest-centre hit test, ordinal tie-break, explicit MISS, zoom/pan/fit, CSS-pixel DPR independence and fitted-scale zoom limits.
- Keyboard: map arrow keys pan, `+`/`-` zoom, `0` fits; the ID search/previous/next route is the accessible non-pointer path. Exact ID search is case-sensitive and wins even when the Living/Dead filters hide that row.
- Offline: one CSP `default-src 'none'` file, `connect-src 'none'`, no external `<script>`, `<link>`, `<img>` or fetch/XHR/WebSocket/persistence API.

## 7. Admission and fail-closed refusals

The viewer admits the package before showing any UI. Refusal order:

1. `SOURCE_REFUSED` — schema, domain, fixture id/bytes/SHA/content hash, disclosure, area f32 `3db851ec`, seed/model/algorithm/site-productivity, transform, engine commit/branch/mirror digest, Year1 pin, digest list, V-01 compile-input provenance, P0 DLL.
2. `BINDING_OR_STAGE_REFUSED` — wrong stage, year, revision, non-contiguous successor, more than ten transitions, or a diagnostic promoted to active.
3. `FRAME_INCOMPLETE_OR_INVALID` — roster, bit kind/length/finiteness, transform mismatch, missing mandatory field.
4. `FRAME_CONTRADICTS_SOURCE` — digest, life/death, moved source position, PostGrowth/After geometry disagreement, archived freeze, resurrection, death accounting, hazard/roll rule, negative aggregate.
5. `YEAR_UNAVAILABLE` — a shorter proven prefix is accepted with later years explicitly labelled unavailable; the shipped package supports 0–10, so none are unavailable.

A refusal shows no map, roster or stand measures. The field-level map is in `Tools/MultiRackSitkaHistoryViewer/SOURCE-MAP.md`.

## 8. Independent nonauthor review and fix cycle

### 8.1 Initial review — NEEDS CHANGES

Claude Opus 5.5, a different provider and session from the author, reviewed the first candidate `f4318e73` changed line by line and independently confirmed the history itself: Year 1 matched the preserved report field for field, all ten digests and 13 deaths were correct, and no protected file or path outside the tool was touched. It returned **NEEDS CHANGES** with:

- **B1 blocking:** the exporter `.csproj` was hidden by the root `*.csproj` ignore, so a fresh checkout could not build.
- **M1 major:** browser admission did not check every frame's source position, PostGrowth/After geometry or archived PostGrowth freeze; the last After frame and PostGrowth frames could be altered.
- **M2 major:** refusal order/codes did not match the documented order (digest before stage; revision coded as contradiction; eleven transitions coded as unavailable).
- **M3 major:** tests admitted the gitignored build output rather than the shipped HTML package, and several named refusal branches were not exercised.
- **Minors m1–m8:** content hash/mirror digest unpinned; case-insensitive search contradicted the case-sensitive ID claim; hard-coded commit/death claims; death rule always required a roll; exporter engine identity self-declared; evidence not tied to the commit; relative zoom limits; `innerHTML`.

The full review is preserved verbatim in `Tools/MultiRackSitkaHistoryViewer/evidence/independent-review.md`.

### 8.2 Bounded fixes — same run

All findings were fixed in the same run (commit `667744a5`), then evidence was refreshed (commit `89613237`):

- The `.csproj` is tracked and `git ls-files` includes it.
- Admission pins fixture content hash, engine branch and compiled mirror digest; checks every frame against the Genesis position, enforces PostGrowth age/geometry against After, freezes archived rows in every frame including the last year, and applies the documented refusal order/codes (stage/year/revision/eleven-transitions are binding; digest after stage).
- The exporter pins engine commit, branch, mirror digest and fixture content hash instead of trusting free CLI arguments.
- Tests now admit the package embedded in the shipped HTML and cover pin, position, PostGrowth, last-year freeze, resurrection, uppercase hex, duplicate death, boundary death, roll-vs-p, unevaluated/archived hazard, Genesis stand, schema, transform, site, seed, disclosure and Year1 mutations (49 tests). The tautological archived-hazard assertion was replaced by raw-record checks.
- The ID route is case-sensitive and exact-first, and ignores filters for an exact ID.
- The footer and H0 age claim are derived from the admitted model; `innerHTML` was removed; zoom is clamped to the fitted scale.
- Verification forces a rebuild from committed sources; the browser smoke records the exact HTML SHA-256 and checks Year 0 and the exact dead-ID route.
- SOURCE-MAP terminal/absent-Dq wording now matches the adapter; the package-integrity limitation is documented.

### 8.3 Focused re-check — APPROVED

The same Claude Opus 5.5 reviewer re-checked the fix commit and returned **APPROVED: all 12 findings fixed, no new blocking or major defect.**

It independently confirmed: the exporter `.csproj` is tracked and the `.gitignore` negation resolves; admission now refuses moved positions, PostGrowth/After geometry changes, archived PostGrowth changes and an age skip; a bad digest plus promoted stage, a bad revision and eleven transitions all return `BINDING_OR_STAGE_REFUSED`; the tests admit the embedded dist package and reach the named paths; content hash/engine branch/mirror are pinned; the exact dead-ID route ignores filters; the displayed claims are model-derived and `innerHTML` is gone; archived rows cannot carry a hazard; hazard deaths satisfy `roll < p` and boundary deaths need no roll; the exporter pins its engine identity; the evidence was refreshed after the fix and the browser JSON records the exact HTML SHA; the forced rebuild is in the verify script; SOURCE-MAP matches the adapter; and zoom is clamped to the fitted scale. It ran the 49-test suite and 22 independent in-memory mutations, and confirmed the rebuilt dist contains the fixed web sources and the recorded hashes match. It did not itself run the build, exporter or browser; those rest on the committed evidence.

Two non-blocking test-quality notes and the package-integrity limitation are disclosed in §12. No finding was reopened.

## 9. Qualification evidence

### 9.1 Focused JS suite — 49/49 PASS

```bash
cd /home/jer/CCF-Dev/Multi-20261119/_811/history/Tools/MultiRackSitkaHistoryViewer
/home/jer/.local/share/vite-plus/bin/node tests/run-tests.js
```

Covers: H0 anchors, 100-ID roster, 13-death chronology/freeze, digest anchors, exact f32/f64 kinds, area `3db851ec` versus an f64 `0.09`, all source pins, missing/duplicate/out-of-order IDs, non-finite/uppercase bits, moved positions, PostGrowth/After disagreement, archived freeze in the final frame, resurrection, diagnostic promotion, revision and eleven-transition binding errors, death hazard/roll rules, a genuine boundary death without a roll, a shorter prefix, 8 CSS px hit test/ordinal/MISS, DPR independence, zoom anchor, UI helpers, the embedded shipped package and the static offline/CSP checks. Output: `evidence/unit-tests.txt`.

### 9.2 Deterministic offline replay and protected hashes

```bash
bash tests/verify-offline-and-protected.sh
```

Results (full log: `evidence/verify-protected.txt`):

- a **forced rebuild** from the committed sources succeeds with 0 warnings / 0 errors;
- the normal export and the `unshare --user --map-current-user --net` export are byte-identical, SHA-256 `1c5cf76856197e4c2e9d14b82d30f8c13285bece5f0e662d782a3446a65dfc68`;
- the packaged frames equal the replay;
- #784 Year0 Core/Viewer/fixture and the frozen 127,831 B packaged HTML hashes unchanged;
- #725 45,980 B Golden `574d8746…` unchanged;
- all twelve #789 engine Core source hashes unchanged;
- the P0 reference DLL `9a99f781…` unchanged;
- both V-01 host Assets unchanged.

### 9.3 Real-browser offline smoke

Windows Edge (Chromium) headless through the locally installed `playwright-core`, opening the built file from `file://`:

- requests: the `file://` document only; **zero external requests**;
- CSP blocked a deliberate external connect probe;
- zero page and console errors;
- H0 `Living 100 → 100`, 0 deaths, BA/Dq/RD replayed;
- Year 0 Genesis: stand and annual diagnostics explicitly NotEvaluated;
- Year 8 shows **4 new natural deaths**;
- Year 3 diagnostic: `P30-044` living at PostGrowth and dead at After, with evaluated hazard;
- exact dead-ID route selected `P30-044` with the Dead filter off; keyboard route then selected `P30-053`;
- map hit selected `P30-001` at 8 CSS px and an outside click produced explicit MISS;
- 390×844: no horizontal overflow and next-year navigation works;
- the raw result records the exact opened HTML SHA-256 `8e35dd6d…`;
- screenshots: `evidence/browser-wide-1440x900.png`, `evidence/browser-narrow-390x844.png`; raw result: `evidence/browser-smoke-results.json`.

### 9.4 Artifact hashes

```
8e35dd6df00c97b879ae02dc36453e878a4bb85a8490b304ff2c02a628b1d27f  1143774  Tools/MultiRackSitkaHistoryViewer/dist/year0-10-history-viewer.html
1c5cf76856197e4c2e9d14b82d30f8c13285bece5f0e662d782a3446a65dfc68  2581679  build/out/admitted-frames.json (detached package)
04313d726ff1e7cae848eae2daea4a3c48ab26739e5c4230a5730ef5db532fc8   295335  evidence/browser-wide-1440x900.png
363a794456371f27851ca1195060897f17c8ee19d28e4124b3e3799bff87c437   142870  evidence/browser-narrow-390x844.png
022d3b680503a414ae8825ecce4ce199fdc7e0eb3eacf1e57893a90070e273b9     1357  evidence/browser-smoke-results.json
```

### 9.5 Build environment

- WSL2 Ubuntu 26.04 x64, kernel 6.18.58.1, 7.7 GiB RAM.
- Local .NET SDK 8.0.134 / runtime 8.0.34 (`global.json` pins 8.0.133 with `rollForward: latestPatch`); the exact 8.0.133 SDK was not installed. This deviation is disclosed; the Year1 byte-equality and all ten digest checks passed under 8.0.134.
- Python 3 for packaging; Node 26.13.1 for tests; Windows Edge for the browser smoke.
- No remote NuGet source; `NuGet.config` clears all sources; the export was re-run under `unshare --net`.

## 10. Protected baselines

| Baseline | Verification |
| --- | --- |
| #784 Year0 `core/year0-core.js` | `01e7e936…` unchanged |
| #784 Year0 viewer HTML/CSS/map/UI/boot | `3bc38102…`, `f91cb171…`, `fbb80216…`, `17b951ff…`, `aa409eca…` unchanged |
| #784 fixture | `bcaaa5c3…`, 32,567 B unchanged |
| #784 frozen packaged HTML | `0b69cc1d…`, 127,831 B unchanged |
| #725 Year1 stdout | `574d8746…`, 45,980 B unchanged |
| #789 engine source (12 files) | all hashes unchanged |
| #789 P0 reference DLL | `9a99f781…` unchanged |
| CCF-HP checkout | not entered or changed |
| Restored Core/Viewer/Engine worktrees | clean; only the pre-existing untracked restored fixture directory in Core |

## 11. Scope and prohibition attestation

No candidate marking, selection-for-fell, undo, Work/Fell/Save/Host, simulated intervention, economy, durable event history or O-02 implementation. No fabricated year, forecast, implied persistence, missing metric or ten fresh Year1 runs. No edit to the protected #784 private F3/five-member global/CSP/fixture/HTML, #725 golden, #789 engine, Unity project, ProjectSettings, packages, saves or main. No public code push, main merge, release, public API, new open port, Windows Player build or .NET L4 waiver.

## 12. Limitations / not run

- No full screen-reader, touch, 200 % zoom or complete accessibility programme; only the focused keyboard/ID route and the two browser widths.
- No independent desktop gameplay programme, Unity Editor run or Windows Player build.
- The external ZX20/Files Ops board was not mounted; the external physical lease was not re-read.
- The exact 8.0.133 SDK was not installed; 8.0.134 (latestPatch roll-forward) was used and all byte/digest gates passed.
- The viewer is an integrity/consistency gate, not a cryptographic signature. A deliberate editor of the single HTML file who also rewrites every embedded value and its pins could in principle construct a different coherent package; the C# adapter and recorded hashes are the source of truth.
- Terminal zero-live state was not reached and is not claimed. The current stand DTO requires all four f64 words, so a terminal frame with absent Dq/height would be refused rather than guessed.
- **Non-blocking test-quality notes from the re-check:** (1) `tests/run-tests.js` still loads the gitignored `build/out/admitted-frames.json` as its primary `raw` input, so a fresh checkout must run `build/export-history.sh` first; the suite separately admits the package embedded in the shipped HTML. (2) One test's raw-row lookup uses `.id` where the raw object has `.treeId`, so it resolves to the first (living) row; the assertion remains valid but does not target the intended stem. Neither affects the viewer, the source proof or any refusal.
- **Evidence-head note:** `verify-protected.txt` records the protected Core/Viewer/Engine heads and the forced rebuild/replay, not the candidate head; the candidate head is recorded here and in the commit history.
- This is a local prototype, not the Unity Scenario One game, a public reader, a save format or a main integration.

## 13. Handoff to Manager #730

**One outcome:** a locally runnable, source-bound, offline read-only Year 0–10 viewer and its independent review/qualification record. The candidate branch is local-only:

- worktree `/home/jer/CCF-Dev/Multi-20261119/_811/history`
- branch `feature/multi-811-offline-history-viewer-20261119-v1`
- base `c4124b63252fb563dc7d84c9fc7fc4e3e0d8d2d1`
- candidate head `896132373542023ccbd0e4ed9d96826202ac4e81`
- runnable file `Tools/MultiRackSitkaHistoryViewer/dist/year0-10-history-viewer.html`

Manager decision requested: **APPROVE the local viewer candidate** or return a specific bounded correction. No new broad design review, no main integration and no release are implied. Candidate marking remains a separate future stage.
