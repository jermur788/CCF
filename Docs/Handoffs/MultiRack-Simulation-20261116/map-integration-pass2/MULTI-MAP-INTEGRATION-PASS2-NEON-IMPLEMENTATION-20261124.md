# Multi-Area Map Integration — Pass 2 Implementation Handoff

**State (D-030):** `IMPLEMENTED ON TASK BRANCH — READY FOR INDEPENDENT REVIEW — NOT INTEGRATED, NOT ACCEPTED`

---

## 1. Identity, Session Metadata, and Git Provenance

| Field | Value |
|---|---|
| Implementer | Gemini 4 Neon (Antigravity ACP, full-access, effort `XHigh` / `1.00`) |
| Configured Git author note | Worktree Git config is `DeepSeek V4.1 Flash orchestrator <jermur788@users.noreply.github.com>` (inherited from repo config); actual implementation, verification, and browser testing were executed by Gemini 4 Neon. |
| Delegating T3 thread | `c00d4799-5f15-4a7d-9231-0f133246a64c` |
| Implementation commits UTC (initial run before quota interruption) | `2026-11-24T13:49:56Z` (`db4361fe`) → `2026-11-24T14:22:29Z` (`9aaebeba`) |
| Resumed verification & handoff session UTC | Start `2026-11-24T17:50:06Z` — End `2026-11-24T18:06:00Z` |
| Worktree (sole writable checkout) | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-MapIntegration-Pass2-20261124` |
| Root checkout | `/home/jer/Development/CCF-HP` (read-only; verified clean and untouched) |
| Task branch | `feat/multi-map-integration-pass2-20261124-v1` |
| Base SHA | `1f13182659769f19e7c91159dd7ad475667e0d62` (independently reviewed and owner-accepted Pass 1) |
| Implementation HEAD before handoff (Commit 4) | `9aaebeba9a178c6d41034b09bbd3b41949c25623` |
| Final HEAD SHA | Commit 5 (this handoff commit; exact SHA reported in the Final Report after commit and push) |
| External evidence archive (outside Git, mode `700`) | `/home/jer/Development/CCF-Multi-MapIntegration-Pass2-Evidence-20261124` |
| Tools used | `read_file`, `write_file`, `edit_file`, `run_command` (Windows PowerShell → `wsl -d Ubuntu`), `/home/jer/.dotnet/dotnet` SDK `8.0.134` (`TMPDIR=/home/jer/tmp`), `python3`, `curl`, WSL `node` `v26.13.1`, Windows `node.exe` `v26.13.1`, real headless Microsoft Edge `157.0.4341.60` (`C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe`) over CDP |

### Commit sequence on `feat/multi-map-integration-pass2-20261124-v1`

1. `db4361fed41085d0f07fbac306ebc9594397c552` — `docs(multi-map-pass2): add Pass 2 interactive Work Plan spec quartet`
2. `700807a66bf75b1427eb44ef1ee30ee0bfb9f958` — `feat(multi-map-pass2): add six fenced Pass 2 Work Plan HTTP endpoints`
3. `37a898a050428c4fbc44f158b3cc60e0cfcf3b15` — `feat(multi-map-pass2): add interactive Pass 2 modes, drawers, Work Plan panel and SVG previews`
4. `9aaebeba9a178c6d41034b09bbd3b41949c25623` — `test(multi-map-pass2): add Pass 2 commitment-gate, refusal, payload and smoke tests`
5. Commit 5 (this file) — `docs(multi-map-pass2): add Pass 2 Map Integration durable implementation handoff`

### `git diff --stat 1f13182659769f19e7c91159dd7ad475667e0d62..9aaebeba9a178c6d41034b09bbd3b41949c25623`

```text
 Tools/MultiRackSitkaP1Lab/src/Ui/PlayerApi.cs      |    4 +-
 Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs | 1139 +++++++++++++++++---
 Tools/MultiRackSitkaP1Lab/src/Ui/Program.cs        |   23 +-
 .../tests/Core.Tests/MapIntegrationPass2Tests.cs   |  389 +++++++
 .../tests/Core.Tests/Program.cs                    |    1 +
 .../tests/ui-smoke-multiarea.sh                    |  428 +++++++-
 .../checklists/requirements.md                     |   77 ++
 specs/multi-map-integration-pass2-v1/plan.md       |   68 ++
 specs/multi-map-integration-pass2-v1/spec.md       |  189 ++++
 specs/multi-map-integration-pass2-v1/tasks.md      |   31 +
 10 files changed, 2207 insertions(+), 142 deletions(-)
```

*(Commit 5 adds this single handoff file under `Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass2/`, bringing total files changed vs base to 11. `git status --short` after Commit 5 is empty.)*

---

## 2. File-by-File Summary of Changes (Plain English)

### Spec quartet (`specs/multi-map-integration-pass2-v1/`)
- `spec.md`: Complete, self-contained Pass 2 specification defining all six fenced HTTP write routes, strict camelCase/16 KiB/status rules, single commitment gate, four explicit map modes, provisional `Draft` previews, sequential `Plant → Shelter → SpotClear` fence-chained queueing and `409` partial-failure disclosure, `Escape` precedence, Work Plan panel/dialog coverage, SVG map visuals, all 9 frozen anchors, hard payload budgets, and acceptance criteria (`MI-P2-01`..`MI-P2-06`, `MI-REG`, `MI-P2-REVIEW`). Zero `TBD`.
- `plan.md`: Architecture documenting how Pass 2 calls the six existing `MultiAreaContinuation` commands from `src/Ui/` via the shared session instance with **zero edits to `src/Core/MultiArea/**`**, plus the 5-commit structure.
- `tasks.md`: Bounded task breakdown (`T-01`..`T-05`).
- `checklists/requirements.md`: Requirement verification matrix (`MI-P2-01`..`MI-P2-06`, `MI-REG`, `MI-P2-REVIEW`). Every checkbox remains unticked `[ ]` for independent review.

### UI server & client (`Tools/MultiRackSitkaP1Lab/src/Ui/`)
- `Program.cs`: Added `OpenMultiExperiment(string experimentKey)` returning `(MultiAreaWorkPlanAdapter, MultiAreaContinuation)` backed by the single shared `MultiAreaContinuation` session for both `ma-1` and `ma-1-mixed`, passing both into `PlayerApi` without changing any Core class or `MA-W12` adapter surface.
- `PlayerApi.cs`: Accepted `MultiAreaContinuation multiSession` in the `PlayerApi` constructor so `PlayerMultiApi.cs` can invoke the six existing fenced Core Work Plan methods directly behind the unchanged `HandleAsync` security guards.
- `PlayerMultiApi.cs`:
  - **Six fenced HTTP write routes** registered under `/s/<capability>/` behind `HandleAsync` (`Host`, capability, `POST`, `Origin`, `Content-Type: application/json`, `16 KiB` cap, strict camelCase, unique/non-null/bounded member validation, `UnmappedMemberHandling.Disallow`) and atomic `CompositeFor(result.View)` response pairing:
    - `POST api/multi/clear/add` (`areaId`, `column`, `row` → `AddClearUndergrowth`)
    - `POST api/multi/clear/remove` (`areaId`, `column`, `row` → `RemoveClearUndergrowth`)
    - `POST api/multi/plant/add` (`areaId`, `speciesId`, `xM`, `zM` → `AddPlant`)
    - `POST api/multi/shelter/add` (`areaId`, `xM`, `zM` → `AddShelter`)
    - `POST api/multi/spot-clear/add` (`areaId`, `xM`, `zM` → `AddSpotClear`)
    - `POST api/multi/planting/remove` (`orderId` → `RemovePlanting`)
  - Coordinates `xM` and `zM` deserialize as JSON numbers (`double?`) via `[JsonPropertyName("xM")]` / `[JsonPropertyName("zM")]` and narrow to `float` when calling Core so float32 overflow (e.g. `1e40`) reaches Core's `Phase2CoordinatesInvalid` (`409`) check while non-numeric/null/wrong-case inputs fail as `400 BadRequest`.
  - **Four explicit map modes** (`Select Tree`, `Inspect Cell`, `Plant`, `Spot Clear`) swapping drawers inside `#pane-tree` (never stacking):
    - `Select Tree` (default): preserves Stage 4 crown-wins selection, R3 5-row fold at `1280×720`, species-aware Prune readouts, and adds `#juvenile-panel` when clicking a living `PJ` sapling (or choosing it in `#planted-section` or typing its exact `PJ` ID in `#quick-id`), exposing **`Queue 8-year Shelter`** (`api/multi/shelter/add`) and **`Remove pending shelter`** (`api/multi/planting/remove`) when unsheltered.
    - `Inspect Cell`: swaps in `#cell-panel`; exposes **`Clear undergrowth (spares seedlings)`** (`api/multi/clear/add`) and **`Remove pending clear`** (`api/multi/clear/remove`) for valid in-polygon cells.
    - `Plant`: clicking the map places a provisional dashed-slate `Draft` preview only (`snap OFF`, zero HTTP call) at exact `(X, Z)` and swaps in `#plant-panel` (`Sessile Oak` | `European Beech`, `[ ] Add 8-year shelter`, `[ ] Spot-clear ~1 m²` with `0.564 m` ring preview, **`Queue to Work Plan`**). Pressing `Queue to Work Plan` runs the sequential fence-chained queue `plant/add` → `shelter/add` → `spot-clear/add`; on step 2/3 `409`, it stops immediately, displays the exact Core refusal code/message, and discloses every earlier queued order with one-click **`Remove`** buttons.
    - `Spot Clear`: clicking the map places a provisional seedling-free `0.564 m` circular dashed-slate preview at exact `(X, Z)` (`snap OFF`, zero HTTP call) and opens `#spot-panel` with **`Queue Spot Clear (~1 m²)`**.
  - **`Escape` precedence**: open `<dialog>` → non-empty search box → exit `Plant`/`Spot Clear`/`Inspect Cell` preview & mode (focusing `#mode-tree`) → close `PJ` inspection → clear adult tree selection. Never discards pending draft Fell competitor ticks, never changes the ground overlay, and never mutates the Work Plan. Mode switches preserve focus on the clicked mode toggle and reset `#pane-tree` scrollTop to `0` (Pass 1 `N-4`).
  - **Work Plan panel, dialogs & map SVG**: lists all 6 order types (`Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`) with targeted remove buttons before `Approve`; updates Execute confirmation, Latest receipt (`Completed · History Log`), and Year+1 consent dialog across all 6 families; preserves Pass 1 SVG grammar (`Pending` dashed gold, `Approved` solid pale-blue, `SpotClear` patches in `#spot-patches` beneath `#planted-layer`, circular crowns, N-6 stump `X` retained, zero `Expired Shelter` buttons, zero `data:image/webp`, zero client-side forestry maths).

### Tests (`Tools/MultiRackSitkaP1Lab/tests/`)
- `Core.Tests/MapIntegrationPass2Tests.cs` *(new)*: Four comprehensive Pass 2 Core tests (`MI-P2 C-01..C-04`) verifying the single commitment gate across all 6 order families, 29 refusal assertions across 15 distinct Core refusal codes with zero state mutation, sequential `Plant → Shelter → SpotClear` step-3 partial-failure recovery and post-Execute unsheltered `PJ` shelter installation, and raw UTF-8 JSON payload budgets on `ma-1` and `ma-1-mixed` at Year 0 and Year +5 (both Pending and Executed).
- `Core.Tests/Program.cs`: Registered `MapIntegrationPass2Tests.Register()`.
- `ui-smoke-multiarea.sh`: Added 55 Pass 2 HTTP guard/refusal/commitment-gate/payload checks (`W095..W149`), 14 Pass 2 FakeDOM page-witness checks (`P2a..P2n`), and restored Pass 1 `N-3` (non-Crop status, disabled release option, rank-vs-row ID cross-check, exact release label/preview text) and `N-4` (focus-on-entry + `scrollTop === 0` assertions) checks. Total assertions increased from `170` (Pass 1) to `240` (`240/240 PASS`).

---

## 3. Verification Commands and Real Observed Results

All commands were executed against `9aaebeba9a178c6d41034b09bbd3b41949c25623` in `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-MapIntegration-Pass2-20261124` using `/home/jer/.dotnet/dotnet` (SDK `8.0.134`) and `TMPDIR=/home/jer/tmp`. Raw redacted logs and 28 PNG screenshots are stored outside Git in `/home/jer/Development/CCF-Multi-MapIntegration-Pass2-Evidence-20261124` (mode `700`).

### 3.1 Clean builds (`--no-incremental --disable-build-servers`)

```bash
export TMPDIR=/home/jer/tmp
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-MapIntegration-Pass2-20261124/Tools/MultiRackSitkaP1Lab
/home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln --no-restore --no-incremental --disable-build-servers
/home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --no-restore --no-incremental --disable-build-servers
```

- `MultiRackSitkaP1Lab.sln`: **Build succeeded. `0 Warning(s)`, `0 Error(s)`** (`00:00:09.05`; log `build-sln.txt`)
- `src/Ui/SitkaP1.Ui.csproj`: **Build succeeded. `0 Warning(s)`, `0 Error(s)`** (`00:00:06.06`; log `build-ui.txt`)

### 3.2 Full Core test suite (2 consecutive runs)

```bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-MapIntegration-Pass2-20261124/Tools/MultiRackSitkaP1Lab
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll > "$EVD/core-run1.txt"
/home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll > "$EVD/core-run2.txt"
diff -u <(grep -E '^(PASS|FAIL) ' "$EVD/core-run1.txt") <(grep -E '^(PASS|FAIL) ' "$EVD/core-run2.txt")
```

- **Run 1:** `255/255 passed, 0 failed` (251 Pass 1 baseline + 4 new `MI-P2 C-01..C-04`)
- **Run 2:** `255/255 passed, 0 failed`
- **`diff -u` of Run 1 vs Run 2 `^(PASS|FAIL) ` verdict sets:** empty (`IDENTICAL`, 255 lines; saved in `core-run1.txt`, `core-run2.txt`, `core-run1-verdicts.txt`, `core-run2-verdicts.txt`).

### 3.3 All 9 Frozen Anchors (Expected vs Observed)

Verified from `anchors.txt` (captured from `core-run1.txt` and `sha256sum Tools/MultiRackSitkaP1Lab/.p0ref/SitkaLab.Core.dll`, permissions `-r--r--r--`):

| # | Frozen Anchor | Expected SHA-256 | Observed SHA-256 | Status |
|---|---|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | MATCH |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | MATCH |
| 3 | Multi-area `0/0/1/0` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | MATCH |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | MATCH |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | MATCH |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | MATCH |
| 7 | Phase 2 Q6 (`1/1/1/1/0` vs `1/1/1/1/1`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` (both `1/1/1/1/0` and `1/1/1/1/1`) | MATCH |
| 8 | P0 Core binary pin (`.p0ref/SitkaLab.Core.dll`) | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (`chmod 444`) | MATCH |
| 9 | MA-1-MIXED 30-y fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | MATCH |

### 3.4 `ui-smoke-multiarea.sh` end-to-end witness

```bash
export TMPDIR=/home/jer/tmp PATH="/home/jer/.local/share/vite-plus/bin:$PATH"
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-MapIntegration-Pass2-20261124/Tools/MultiRackSitkaP1Lab
bash tests/ui-smoke-multiarea.sh /home/jer/Development/CCF-Multi-MapIntegration-Pass2-Evidence-20261124
```

- Result: **`OVERALL: PASS (240 passed, 0 failed)`**, exit `0` (saved in `ui-smoke-multiarea.txt`).
- Covers: `S01..S02`, `M01..M13` (strict CSP, `node --check multi.js`, M07 canonical hash, zero WebP, 21 SVG IDs, truthful badges, `--multi-experiment=ma-1-mixed`, exit 64 invalid CLI), `W001..W149` + `W999` (149 HTTP workflow checks including 55 new Pass 2 checks `W095..W149`), `F2a..F2h`, `R2a..R2i` (restored N-4 focus-on-entry + `scrollTop === 0`), `R3a..R3j`, `J01..J23` (restored N-3 checks) + `P2a..P2n` (14 Pass 2 page-witness checks) + `J99`, `Z01..Z04` (clean `SIGINT` shutdown with exit `0`, empty stderr, zero repo mutations).

### 3.5 Raw UTF-8 Payload Byte Budgets (Pending & Executed, Year 0 & Year +5)

Hard budgets: `GET api/multi/view` raw UTF-8 JSON **≤ 524,288 B (512 KiB)**; top-level `"ecology"` raw UTF-8 byte span **≤ 153,600 B (150 KiB)**.

#### Core unit test (`MI-P2 C-04`)

| Experiment | Year | State | Total raw UTF-8 B | Total KiB | `ecology` raw UTF-8 B | `ecology` KiB | Budget |
|---|---:|---|---:|---:|---:|---:|---|
| `ma-1` | 0 | Pending (`ClearUndergrowth` + `Plant` + `Shelter` + `SpotClear`) | `387,834` | `378.74` | `43,800` | `42.77` | PASS |
| `ma-1` | 0 | Executed | `389,036` | `379.92` | `44,087` | `43.05` | PASS |
| `ma-1` | 5 | Pending (second Pass 2 batch) | `412,513` | `402.84` | `46,443` | `45.35` | PASS |
| `ma-1` | 5 | Executed | `413,724` | `404.03` | `46,736` | `45.64` | PASS |
| `ma-1-mixed` | 0 | Pending (`ClearUndergrowth` + `Plant` + `Shelter` + `SpotClear`) | `389,098` | `379.98` | `44,755` | `43.71` | PASS |
| `ma-1-mixed` | 0 | Executed | `390,300` | `381.15` | `45,042` | `43.99` | PASS |
| `ma-1-mixed` | 5 | Pending (second Pass 2 batch) | `416,547` | `406.78` | `48,020` | `46.89` | PASS |
| `ma-1-mixed` | 5 | Executed | `417,758` | `407.97` | `48,311` | `47.18` | PASS |

#### Live HTTP smoke witness (`ui-smoke-multiarea.sh`, Year 1 after prior Fells/Prune)
- `W137` Pass 2 Pending: `view = 402,342 B` (≤ `524,288 B`), `ecology = 44,907 B` (≤ `153,600 B`) — **PASS**
- `W149` Pass 2 Executed: `view = 403,556 B` (≤ `524,288 B`), `ecology = 45,199 B` (≤ `153,600 B`) — **PASS**

#### Live loopback HTTP + Real Headless Microsoft Edge (exact raw UTF-8 wire byte span of `"ecology"`, full 6-order Year 0 batch + second unsheltered Beech PJ + Year 5 Pass 2 batch)

| Matrix Cell | Stage | Total raw HTTP B | Raw `"ecology"` B | `fetch` ms | `JSON.parse` ms | `rAF` paint ms | Budget |
|---|---|---:|---:|---:|---:|---:|---|
| `ma-1 @ 1920x1080` | Year 0 Initial | `387,472` | `43,438` | `27.9` | `2.2` | `19.9` | PASS |
| `ma-1 @ 1920x1080` | Year 0 Pending (all 6 order families) | `388,401` | `43,918` | `7.5` | `1.3` | `7.1` | PASS |
| `ma-1 @ 1920x1080` | Year 0 Executed | `391,201` | `44,579` | `7.1` | `1.3` | `29.7` | PASS |
| `ma-1 @ 1920x1080` | Year 5 Pending | `413,868` | `46,966` | `14.5` | `2.6` | `14.3` | PASS |
| `ma-1 @ 1920x1080` | Year 5 Executed | `415,077` | `47,257` | `13.2` | `2.5` | `3.1` | PASS |
| `ma-1 @ 1280x720` | Year 0 Initial | `387,472` | `43,438` | `17.1` | `1.3` | `100.2` | PASS |
| `ma-1 @ 1280x720` | Year 0 Pending (all 6 order families) | `388,395` | `43,912` | `7.5` | `1.3` | `2.0` | PASS |
| `ma-1 @ 1280x720` | Year 0 Executed | `391,195` | `44,573` | `13.3` | `2.0` | `18.3` | PASS |
| `ma-1 @ 1280x720` | Year 5 Pending | `413,862` | `46,960` | `8.2` | `1.4` | `16.1` | PASS |
| `ma-1 @ 1280x720` | Year 5 Executed | `415,071` | `47,251` | `8.0` | `1.4` | `13.5` | PASS |
| `ma-1-mixed @ 1920x1080` | Year 0 Initial | `388,736` | `44,393` | `19.3` | `1.3` | `56.8` | PASS |
| `ma-1-mixed @ 1920x1080` | Year 0 Pending (all 6 order families) | `389,662` | `44,870` | `10.2` | `1.5` | `7.2` | PASS |
| `ma-1-mixed @ 1920x1080` | Year 0 Executed | `392,464` | `45,538` | `7.5` | `1.2` | `16.4` | PASS |
| `ma-1-mixed @ 1920x1080` | Year 5 Pending | `417,938` | `48,546` | `28.4` | `5.1` | `6.1` | PASS |
| `ma-1-mixed @ 1920x1080` | Year 5 Executed | `419,149` | `48,837` | `13.0` | `2.5` | `15.9` | PASS |
| `ma-1-mixed @ 1280x720` | Year 0 Initial | `388,736` | `44,393` | `17.1` | `1.4` | `56.5` | PASS |
| `ma-1-mixed @ 1280x720` | Year 0 Pending (all 6 order families) | `389,659` | `44,867` | `7.4` | `1.3` | `14.9` | PASS |
| `ma-1-mixed @ 1280x720` | Year 0 Executed | `392,461` | `45,535` | `8.1` | `1.5` | `14.9` | PASS |
| `ma-1-mixed @ 1280x720` | Year 5 Pending | `417,933` | `48,541` | `20.8` | `2.4` | `14.1` | PASS |
| `ma-1-mixed @ 1280x720` | Year 5 Executed | `419,144` | `48,832` | `25.5` | `3.1` | `8.3` | PASS |

Peak observed raw payload across all runs: **`419,149 B` (`409.33 KiB`)** vs `524,288 B` cap (`105,139 B` headroom), and peak raw `"ecology"` span: **`48,837 B` (`47.69 KiB`)** vs `153,600 B` cap (`104,763 B` headroom).

### 3.6 Real Loopback + Real Headless Microsoft Edge 4-Cell Matrix

Driver: external dependency-free Windows Node `v26.13.1` CDP driver (`browser/pass2-verify.mjs`, `browser/cdp-ux.mjs`, `browser/run-cell.sh` in `/home/jer/Development/CCF-Multi-MapIntegration-Pass2-Evidence-20261124/browser/`), launching a **fresh loopback `sitkap1-ui.dll` process per cell** and stopping it cleanly via `SIGINT` (`SERVER_EXIT: 0` on all 4 cells). 28 capability-redacted PNG screenshots (`7` per cell) and 4 JSON reports (`results-*.json`) captured; key screenshots visually inspected.

| Matrix Cell | Result | Zero horiz. scroll | R3 5-row fold (clean / 3 blocked+card / 4 blocked+Add) | Modes, Draft preview (0 POST) & `Escape` keeps draft Fell ticks | Live 409 refusals & step-3 partial disclosure + 1-click remove | Commitment gate (Queue/Approve 0 eco; atomic Execute yr +0) + live PJ / active shelter / `0.564 m` ring + Year +1 | Circular crowns & zero WebP | Console / JS / CSP / 5xx |
|---|---|---|---|---|---|---|---|---|
| `ma-1 @ 1920x1080` | `35/35 PASS` (`SERVER_EXIT: 0`) | PASS (`1920 <= 1920`) | PASS (`818 <= 829`, `addBottom 915 <= 1080`, `wsHeadH 38px`) | PASS (4 modes, N-4 focus, `Escape` keeps 2 ticks + overlay) | PASS (`Phase2PointOutsideArea`, `PlantTooCloseToAdult`, `Phase2DuplicateOrder` + 2 remove btns) | PASS (4 Fell + Sitka Prune `MA-B-001` + Clear + Oak Plant + 8-yr Shelter + `0.564 m` SpotClear + unsheltered Beech PJ + Year +1 → +5) | PASS (`171 A` + `73 B` Sitka, `w=h=2r`, 0 WebP) | `0 / 0 / 0 / 0` (`200`: 83, `409`: 3) |
| `ma-1 @ 1280x720` | `35/35 PASS` (`SERVER_EXIT: 0`) | PASS (`1280 <= 1280`) | PASS (`590 <= 594`, `addBottom 644 <= 720`, `dockBottom 650 <= 720`, `wsHeadH 36px`) | PASS (4 modes, N-4 focus, `Escape` keeps 2 ticks + overlay) | PASS (`Phase2PointOutsideArea`, `PlantTooCloseToAdult`, `Phase2DuplicateOrder` + 2 remove btns) | PASS (4 Fell + Sitka Prune `MA-B-001` + Clear + Oak Plant + 8-yr Shelter + `0.564 m` SpotClear + unsheltered Beech PJ + Year +1 → +5) | PASS (`171 A` + `73 B` Sitka, `w=h=2r`, 0 WebP) | `0 / 0 / 0 / 0` (`200`: 83, `409`: 3) |
| `ma-1-mixed @ 1920x1080` | `35/35 PASS` (`SERVER_EXIT: 0`) | PASS (`1920 <= 1920`) | PASS (`837 <= 848`, `addBottom 934 <= 1080`, `wsHeadH 38px`) | PASS (4 modes, N-4 focus, `Escape` keeps 2 ticks + overlay) | PASS (`Phase2PointOutsideArea`, `PlantTooCloseToAdult`, `Phase2DuplicateOrder` + 2 remove btns) | PASS (4 Fell + Oak broadleaf Prune `MA-B-002` + Clear + Oak Plant + 8-yr Shelter + `0.564 m` SpotClear + unsheltered Beech PJ + Year +1 → +5) | PASS (`171 A` Sitka; `B`: 25 S / 24 O / 24 B, `w=h=2r`, 0 WebP) | `0 / 0 / 0 / 0` (`200`: 83, `409`: 3) |
| `ma-1-mixed @ 1280x720` | `35/35 PASS` (`SERVER_EXIT: 0`) | PASS (`1280 <= 1280`) | PASS (`590 <= 594`, `addBottom 644 <= 720`, `dockBottom 650 <= 720`, `wsHeadH 36px`) | PASS (4 modes, N-4 focus, `Escape` keeps 2 ticks + overlay) | PASS (`Phase2PointOutsideArea`, `PlantTooCloseToAdult`, `Phase2DuplicateOrder` + 2 remove btns) | PASS (4 Fell + Oak broadleaf Prune `MA-B-002` + Clear + Oak Plant + 8-yr Shelter + `0.564 m` SpotClear + unsheltered Beech PJ + Year +1 → +5) | PASS (`171 A` Sitka; `B`: 25 S / 24 O / 24 B, `w=h=2r`, 0 WebP) | `0 / 0 / 0 / 0` (`200`: 83, `409`: 3) |

**Visual inspection notes (screenshots directly inspected via `read_file`):**
- `01-r3-loaded-fold-ma-1-mixed-1280x720.png`: 4 blocked competitor rows (`Pending Fell`) + 1 free row + `Add selected to Fell plan` + `Clear` sit cleanly above the fold (`wsHeadH 36px`, zero horizontal scroll, zero clipped labels).
- `02-plant-partial-409-ma-1-mixed-1280x720.png`: Exact `Phase2DuplicateOrder — two orders of one kind name the same point: P2W0004` refusal alert surfaced verbatim; Work Plan badge reflects `7` (`4 Fell · 1 Plant · 1 Shelter · 1 SpotClear pending`).
- `03-plant-draft-preview-ma-1-mixed-1280x720.png`: Provisional dashed-slate `Draft Plant + Shelter + Spot` marker + `0.564 m` ring preview rendered on map with Planting Drawer (`Sessile Oak`, both checkboxes ticked, `Queue to Work Plan`).
- `04-workplan-approved-ma-1-mixed-1280x720.png`: Approved Work Plan shows `4 Fell · 1 Prune · 1 Clear · 1 Plant · 1 Shelter · 1 SpotClear approved · nothing executed yet` with stand counts untouched (`171` / `73`).
- `05-post-execute-pj-shelter-spot-ma-1-mixed-1920x1080.png` & `1280x720.png`: Live post-Execute rendering confirms Oak `PJ0001` sapling (`#ccf-sapling-oak`, transparent hit disc), active cyan-blue 8-year shelter (`#ccf-shelter-active`), and circular `0.564 m` SpotClear patch (`#ccf-spot-clear`, `width=height=1.128`, in `#spot-patches` strictly below `#planted-layer`) at `YEAR 0`, `work revision 0 → 1`, with `Completed · History Log` receipt.
- `06-pj-sapling-inspector-ma-1-mixed-1280x720.png`: Clicking unsheltered Beech `PJ0002` opens `#juvenile-panel` with dashed white selection ring on the map, `European beech (beech)`, `Height 0.60 m`, `Browse shelter: Unsheltered`, and `Queue 8-year Shelter`.
- `07-year5-executed-ma-1-mixed-1280x720.png`: Year 5 renders cleanly with standing dead `D`, schematic fallen logs, `Results 5`, and zero overflow.

### 3.7 Commitment-Gate and Refusal Test Counts

| Layer | Commitment-gate proofs | Refusal checks | Distinct refusal codes verified |
|---|---|---|---|
| Core unit suite (`MapIntegrationPass2Tests.cs`, `MI-P2 C-01..C-04`) | `2` end-to-end test cases (`C-01` full 6-verb Queue/Remove/Approve/Execute/Cancel gate; `C-03` sequential partial-failure recovery + post-Execute unsheltered `PJ` shelter commit) | `29` `T.Refuses` assertions (each verifying zero `planStateRevision` & `source` change) | `15` Core refusal codes (`Phase2CoordinatesInvalid`, `Phase2PointOutsideArea`, `Phase2PointWithoutGroundCell`, `Phase2DuplicateOrder`, `Phase2SpeciesUnknown`, `PlantSpacingTooClose`, `PlantTooCloseToAdult`, `ShelterNoTarget`, `ShelterAlreadyPresent`, `ClearCellNotInArea`, `MultiAreaWrongArea`, `ClearDuplicateCell`, `UnknownWorkOrder`, `WorkPlanAlreadyApproved`, `PlayerPlanStateStale`) |
| HTTP smoke witness (`ui-smoke-multiarea.sh`, `W095..W149`) | `17` HTTP lifecycle/gate/budget checks (`W120`, `W123..W126`, `W129`, `W131`, `W133..W139`, `W146..W147`, `W149`) | `36` HTTP refusal checks: `4` transport/header guards (`403`/`405`/`415`/`413`, `W096..W099`), `9` schema `400 BadRequest` (`W100..W108`), `23` typed Core `409 Conflict` (`W109`, `W111..W119`, `W121..W122`, `W127..W128`, `W130`, `W132`, `W140..W145`, `W148`) + `W110` fresh `stage4+ecology` check | `4` guard codes + `BadRequest` + all `15` Core refusal codes above |
| FakeDOM page witness (`ui-smoke-multiarea.sh`, `P2a..P2n`) | `14` UI mode/draft/gate/live-SVG/Escape checks (including `P2c`/`P2f` 0-POST Draft previews, `P2j`/`P2k` Pending & Approve 0-ecology proofs, `P2m` post-Execute live markers) | `1` live sequential step-3 `409 Phase2DuplicateOrder` partial disclosure + 1-click remove (`P2h`/`P2i`) | `Phase2DuplicateOrder` |
| Real Headless Edge 4-cell matrix (`pass2-verify.mjs`, `35 × 4 = 140 PASS`) | `4` full 6-order-family lifecycles + `4` unsheltered `PJ` shelter lifecycles + `4` Year +1/+5 progressions (`0` POSTs on Draft previews; `0` ecology change on Queue & Approve; atomic commit at Year +0 on Execute) | `12` live browser `409` refusals (`3` per cell × `4` cells) | `Phase2PointOutsideArea`, `PlantTooCloseToAdult`, `Phase2DuplicateOrder` (step-3 sequential partial disclosure + 1-click remove) |

### 3.8 Host Hygiene and Protected Listeners

- Ephemeral `sitkap1-ui.dll` processes from Pass 2 smoke and browser matrix: **all stopped cleanly** (`SERVER_EXIT: 0`, zero lingering `CCF-Multi-MapIntegration-Pass2` processes via `ps`).
- Protected loopback listeners confirmed still listening:
  - Windows `netstat -ano`: `127.0.0.1:33767` (`LISTENING`, PID `5564`), `127.0.0.1:43649` (`LISTENING`, PID `5564`), `127.0.0.1:46593` (`LISTENING`, PID `5564`).
  - WSL `ss -tlnp`: `:33767` (`pid=1750123`), `:43649` (`pid=1806361`), `:46593` (`pid=839989`).
- Evidence directory permissions: `700` (`/home/jer/Development/CCF-Multi-MapIntegration-Pass2-Evidence-20261124`).
- Root checkout `/home/jer/Development/CCF-HP`: `git status --short` empty (untouched).

---

## 4. Explicit Scope Confirmations

- **Zero edits to `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/**`:** Confirmed via `git diff --name-only 1f131826..HEAD` (zero files under `src/Core/`).
- **Zero edits to Unity / `Assets/**` / `Packages/**` / `ProjectSettings/**` / Pass 1 specs / `.p0ref/**` / `main`:** Confirmed via `git diff --name-only 1f131826..HEAD`.
- **Zero save-system or economy/cash/price edits:** Confirmed (session remains memory-only).
- **Zero `data:image/webp` remaining:** Confirmed by `grep -rn "data:image/webp" Tools/MultiRackSitkaP1Lab/src/`, `ui-smoke-multiarea.sh` `M08`, and live Edge HTML/CSS/JS/DOM audit across all 4 cells.
- **Zero client-side forestry maths (D-020):** Confirmed; all spacing (`0.5 m` / `0.7 m`), shelter matching (`0.25 m`), polygon interiority, cell existence, and pruning eligibility remain engine-authoritative.
- **Zero ticked checklist rows:** Confirmed that every row in `specs/multi-map-integration-pass2-v1/checklists/requirements.md` remains `[ ]`.
- **Zero capability URLs leaked:** Confirmed via `grep -rnE '/s/[0-9a-f]{64}/'` across the entire worktree (`specs/`, `src/`, `tests/`, `Docs/Handoffs/...`) and `/home/jer/Development/CCF-Multi-MapIntegration-Pass2-Evidence-20261124` (`ZERO_CAPABILITY_LEAKS`).

---

## 5. Explicitly NOT Verified / NOT Done

- **Independent review (`MI-P2-REVIEW`):** Not performed; every checkbox in `specs/multi-map-integration-pass2-v1/checklists/requirements.md` is intentionally left unticked `[ ]` for an independent reviewer and Overall Manager.
- **PR creation, merge to `main`, force push, or tagging:** Not performed per binding brief (task branch `feat/multi-map-integration-pass2-20261124-v1` pushed only).
- **Unity Editor / Play Mode / Unity C# builds:** Out of scope for the standalone `/multi` lab Pass 2 and not executed.
- **Interactive human desktop visual review / screen readers (NVDA/JAWS) / non-Edge browsers (Firefox/Safari/Chrome):** Verified in real headless Microsoft Edge `157.0.4341.60` over CDP on Windows + WSL2 loopback; interactive human accessibility and cross-browser testing have not been run.
- **Live browser `500 OutcomeUnconfirmed` network-fault simulation inside real Edge:** The fail-closed `OutcomeUnconfirmed` command lock and recovery via re-read are verified in `ui-smoke-multiarea.sh` (`J20..J22`) and wired identically in `onPlantQueue` / `onSpotQueue` / `run()`, but real Edge CDP network interception of a mid-flight `POST api/multi/plant/add` `500`/drop was not separately injected in `pass2-verify.mjs`.
- **Live browser rendering of an *expired* 8+ year shelter (`#ccf-shelter-expired`):** The browser matrix advances through Year +5 (confirming active shelter rendering at Year 0..5); advancing 8+ years in Edge to visually observe the active→expired transition (`#ccf-shelter-expired`) was not included in the 5-year browser matrix (though `#ccf-shelter-expired` exists in SVG `<defs>`, `renderPlantedLayer` switches on `!sh.isEffective`, and Core `Phase2Tests` tests 8-year expiry).
- **Live browser test of Year+1 consent dialog specifically triggered by unexecuted Pass 2 orders (`ClearUndergrowth`/`Plant`/`Shelter`/`SpotClear`):** Covered in `ui-smoke-multiarea.sh` HTTP/JS code paths and verified visually for code review in `openConsentDialog` (`PlayerMultiApi.cs:5098`), but the 4-cell Edge script executed all Year 0 work prior to advancing Year 0 → 1.
- **Exhaustive 244-tree loaded fold sweep:** R3 fold geometry (clean, 3 blocked + post-Add card, 4 blocked + Add form) was verified on a representative 5-competitor reference tree (`MA-A-001`) across all 4 matrix cells (`1920x1080` and `1280x720`, `ma-1` and `ma-1-mixed`), not exhaustively across all 244 trees.
