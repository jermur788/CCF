# Multi — Playable 2D Stand Map: bounded tasks and exact writable paths (DRAFT)

- **Parent:** [spec.md](spec.md) (DRAFT) · [plan.md](plan.md).
- **Status:** PM-T00 (Phase I) **DONE in this commit**. PM-T01…PM-T08 are **NOT STARTED and NOT AUTHORISED** until owner G5 and #730 acceptance.
- **Base blobs** at `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f` are listed so a reviewer can diff exactly.

## Exact writable production path list (proposed for #730 admission)

All paths are relative to `Tools/MultiRackSitkaP1Lab/`. "New" means the path does not exist at the base. Nothing outside this list is written, apart from GitHub handoff Markdown under `Docs/Handoffs/MultiRack-Simulation-20261116/` and corrections to this spec folder.

| # | Path | Kind | Base blob | Task | Why it is needed |
|---|---|---|---|---|---|
| 1 | `src/Core/AnnualContinuation.cs` | **modify** | `87903271b2246311ee0d356ad6979b77c6b1b43a` | PM-T01 | The contract requires the view and the fences under the **same** `sync` lock and `WorkRoot`, which only this class owns. Edits are limited to: new `WorkRoot` members; session ID; the publish lines of six existing methods; and new internal members. Legacy validation order and refusal codes are unchanged |
| 2 | `src/Core/PlayerSessionView.cs` | new | — | PM-T02 | Public immutable view, fence and result DTOs, plus `PlayerLabels` |
| 3 | `src/Core/PlayerWorkPlanAdapter.cs` | new | — | PM-T02 | Public facade (the name the owner handoff uses) over the internal continuation; no state of its own |
| 4 | `src/Core/PlayerMapScene.cs` | new | — | PM-T03 | Pure geometry, paint order and identity hit-test. In Core so the existing runner tests it with **no csproj edit**; contains no colours or drawing |
| 5 | `tests/Core.Tests/PlayerSessionTests.cs` | new | — | PM-T04 | PS-01…PS-19 |
| 6 | `tests/Core.Tests/PlayerMapSceneTests.cs` | new | — | PM-T04 | MS-01…MS-06 |
| 7 | `tests/Core.Tests/Program.cs` | **modify, +1 line** | `86952c504dcb64d3bf0da971eaea6b4b557c2a71` | PM-T04 | The runner has no discovery; insert `PlayerSessionTests.Register(manifestPath);` after line 441. No other change |
| 8 | `src/Ui/SitkaP1.Ui.csproj` | new | — | PM-T05 | New UI project (UI-A: shared-framework reference, embedded `wwwroot`). Not added to the `.sln`, so the qualified solution build graph is unchanged |
| 9 | `src/Ui/Program.cs` | new | — | PM-T05 | Process entry, loopback bind, capability, shutdown |
| 10 | `src/Ui/PlayerApi.cs` | new | — | PM-T05 | Guards and JSON endpoints (thin calls into the adapter) |
| 11 | `src/Ui/wwwroot/index.html` | new | — | PM-T06 | Page structure and accessible regions |
| 12 | `src/Ui/wwwroot/app.css` | new | — | PM-T06 | Survey-sheet styling, responsive layout, focus styles |
| 13 | `src/Ui/wwwroot/app.js` | new | — | PM-T06 | SVG map, table, inspector, Work Plan, dialogs; renders only from server views |
| 14 | `tests/ui-smoke.sh` | new | — | PM-T07 | UI process guard and flow witness (bash + curl) |
| 15 | `README.md` | **modify** | `e3b312b2cfd48081298e43dcbb5c3c4400d6ec50` | PM-T08 | Real launch path and honesty boundaries; replaces the stale "Host / UI … Not implemented" row |

Rows 8–14 assume **UI-A**. If the owner chooses **UI-B**, rows 10–14 are replaced by `src/Ui/Gtk.cs` and `src/Ui/MainWindow.cs` plus a manual operation note, and the list is re-submitted to #730 before writing.

### Explicitly NOT written (and why no change is needed)

| Path | Base blob | Reason it stays untouched |
|---|---|---|
| `src/Core/WorkContract.cs` | `79cf27e56d5a289cc6b1598761d88562f8dea9e1` | Existing public records are reused; new types go in new files |
| `src/Core/AnnualEngine.cs` | `625a3b44a9b1e8c9276ff5b0ee8c7b57c10f8738` | `ContinueInMemory()` is internal and reachable from the new Core facade; no `InternalsVisibleTo` change needed |
| `src/Core/AnnualRules.cs`, `DerivedManifest.cs`, `ManifestGenerator.cs`, `FixtureAccess.cs`, `TrustTables.cs`, `CcfSourcePins.cs`, other Core files | — | Biology, fixture and trust are protected |
| `src/Core/SitkaP1.Core.csproj` | `ee8504726e13a5a5fe129728b9594a5cafe6b96b` | SDK globbing compiles new `src/Core/*.cs` automatically |
| `tests/Core.Tests/SitkaP1.Core.Tests.csproj` | `9b66edd692ea4fc0fcfb06556c2b3f6542508496` | New tests are in Core's test project and are globbed |
| `MultiRackSitkaP1Lab.sln` | `c2bc5c47e4ed8e942886622fddc528e2e7a207e9` | The UI is built by explicit project path |
| `src/Cli/**` | — | No CLI route is added |
| `tests/Core.Tests/{AnnualTests,ContinuationTests,WorkContractTests,Runner}.cs` | — | Base 66 tests are never weakened or edited |
| `fixtures/**`, `.p0ref/**`, `build/**`, `global.json`, `Directory.Build.props`, `.gitignore`, existing `tests/*.sh`, `tests/l4_analyze.py` | — | Protected or unrelated |
| `Tools/MultiRackSitkaLab/**`, `Assets/**`, `ProjectSettings/**`, any Scenario One file | — | Hard prohibition |

## Tasks

### PM-T00 — Phase I architecture proof and spec delta — **DONE (docs only)**

Phase I note, this four-file delta and the environment proof. No production source.

### PM-T01 — Same-lock session root extension (`AnnualContinuation.cs`)

- **Gate:** #730 acceptance of spec §6 and Q-2, Q-5, Q-6.
- **Do:** spec §6.2–6.5. Fenced internal commands call the legacy methods reentrantly inside one `sync`.
- **Checks:** base 66 unchanged plus PS-01…PS-17; ten digests identical.

### PM-T02 — Public view DTOs and adapter (`PlayerSessionView.cs`, `PlayerWorkPlanAdapter.cs`)

- **Gate:** PM-T01.
- **Do:** spec §6.1, §6.6–6.7, §7.2 labels.
- **Checks:** PS-05, PS-18, PS-19.

### PM-T03 — Pure map scene and hit-test (`PlayerMapScene.cs`)

- **Gate:** PM-T02.
- **Do:** spec §4.3–4.4, §5.
- **Checks:** MS-01…MS-06.

### PM-T04 — Core tests and the one-line registration

- **Gate:** PM-T01…T03.
- **Do:** plan P2 step 5.
- **Exit:** push, then a Tier-1 clone run of the pushed SHA.

### PM-T05 — UI process (G5 option; UI-A shown)

- **Gate:** **owner G5** plus PM-T04 Tier-1 green.
- **Do:** spec §3.2, §3.6; plan P3 steps 1–3.
- **Checks:** offline restore; build with 0 warnings; the `644 B-01` scan still passes.

### PM-T06 — Page: map, table, inspector, Work Plan, dialogs, alerts

- **Gate:** PM-T05.
- **Do:** spec §4, §5, §7, §8.
- **Checks:** browser evidence per plan P3, keyboard-only pass.

### PM-T07 — `tests/ui-smoke.sh`

- **Gate:** PM-T05.
- **Do:** plan P3 step 5.
- **Checks:** AC-PM-11.

### PM-T08 — Lab README launch section, author Tier-1 and handoff

- **Gate:** PM-T05…T07.
- **Do:** plan P3 step 6, P4 author part, P5. Then hand to the independent reviewer.

### PM-T09 — Independent Tier-2 (GPT-6.1 Sol; not this author)

- **Gate:** PM-T08 pushed SHA.
- **Do:** plan P4. Writes only its own review artifact.
