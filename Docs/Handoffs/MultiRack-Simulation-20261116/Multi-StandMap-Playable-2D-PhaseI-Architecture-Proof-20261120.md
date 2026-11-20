# Multi-StandMap-Playable-2D — Phase I architecture proof, map-path decision and environment proof

**20 November 2026 (UTC) | Primary implementer: Claude Opus 5.5 | Parent: DeepSeek V4.1 Flash / OpenCode (administrative only) | Manager: Multi 2D #730**

**Result status: PHASE I COMPLETE — DOCS ONLY. NO PRODUCTION SOURCE WRITTEN. HOLD at two gates: owner G5 (frontend runtime) and #730 acceptance of the spec delta.**

Companion spec delta (same commit): [`specs/multirack-sitka-2d-playable-standmap-v1/`](../../../specs/multirack-sitka-2d-playable-standmap-v1/spec.md) (spec, plan, tasks, checklist).

## 0. Identity and provenance

- Author model identity is **worker-reported** (Claude Opus 5.5 via T3 Code); Git metadata does not prove it (A-1 finding F-1).
- This lane's configured git identity is `DeepSeek V4.1 Flash 1017 <jermur788@users.noreply.github.com>`, which is the **non-author** parent. This commit therefore sets author and committer explicitly per command to Claude Opus 5.5, without changing any git config.
- Independent reviewer reserved: GPT-6.1 Sol. It has not reviewed this note.

## 1. Base verification

| Check | Observed |
|---|---|
| Lane | T3 Windows checkout `C:\Users\hp\.t3\worktrees\CCF-HP\feat-multi-standmap-pla-b40873fbd04c0a28`; gitdir `//wsl.localhost/Ubuntu/home/jer/Development/CCF-HP/.git/worktrees/feat-multi-standmap-pla-b40873fbd04c0a28` |
| HEAD | `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f` "fix(multi-a1): fail closed legacy annual advance after a Fell" ✔ |
| Branch | `feat/multi-standmap-playable2d-20261120-v1` (no upstream before this push) ✔ |
| Tree | **Clean in content.** Windows git shows 563 `M` entries; every one is `mode change 100755 → 100644` (WSL `core.filemode=true` seen through NTFS). With `-c core.filemode=false`, status is empty. None are staged or committed |
| Line endings | Windows system git `core.autocrlf=true`: text files in this lane have CRLF (for example `SimulationRandom.cs` has 67 CR bytes). Blobs in Git are unaffected; the fixture JSON has 0 CR |
| WSL view | WSL git lists this worktree as `prunable` (it cannot resolve `C:/…`). **`git worktree prune` must not be run** |
| Lab tree at HEAD | `Tools/MultiRackSitkaP1Lab` tree `6059b62dcb9b2159d97ce189597998d70fab8dab`; `Tools/MultiRackSitkaLab` tree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` (the P0 qualified tree) |

## 2. Inputs read in full

| Document | Ref |
|---|---|
| Owner one-shot handoff | `origin/docs/Multi-StandMap-Playable-2D-OneShot-Orchestration-20261120-v1` @ `7ec4a7a17014f715a57d3ca2a76a4cdd4bbc2241` (blob `9791d0e1`) |
| A-1 Manager acceptance | `origin/docs/Multi-A1-Manager-Acceptance-and-Next-UI-Gate-20261120-v1` @ `1dee048ba7e38d0004929f4cc9ec11f21d03f007` (blob `d01bb3db`) |
| APPROVED Stage 5 spec/plan/tasks/checklist | `origin/docs/multi-730-837-g2-contract-decision-draft-20261119-v1` @ `6245755c77398d7d61356018280b31b9330a1ad2` |
| UX preflight | `origin/Multi-stage5-player-workplan-ux-preflight-20261119-v1` @ `58cbba76ad3a898dac3852bdd3408d62fa0e4135` |
| Adapter contract (as approved) | `origin/docs/multi-player-session-adapter-contract-20261119-v1` @ `46a04f61`, blob `75593a128ab838d1ca8756a65c85b48c78da635e` |
| Manager review of the contract | `origin/docs/Multi-Player-Adapter-Manager-Contract-Verdict-20261119-v1` @ `f9e77874a04687a7172f9b32ef6f2418643a1c35` |
| Stand Map reuse assessment | `origin/docs/Multi-StandMap-Reuse-Assessment-20261120-v1` @ `ec044b4d16de277350e03abe43275aa6a8f07894` |
| Scenario One map sources (read only) | `origin/main` @ `7758e17c882fad01970d2f2fb8c09586bbcab5c1`, `Assets/ForestPrototype/UI/Map/*` and `Assets/ForestPrototype/Racks/RackGeometry.cs` |

`Docs/Project/coordination/active-tasks.md` was **not** re-fetched live by this worker. Lock and claim registration is the parent's administrative step (dispatch §2).

## 3. Existing lab — inspected facts (`Tools/MultiRackSitkaP1Lab` @ `4bb80973`)

- **Projects:** `src/Core` (`SitkaP1.Core`, library), `src/Cli` (`sitkap1`, Exe), `tests/Core.Tests` (dependency-free runner, Exe). One `.sln` holds those three projects. `Directory.Build.props`: net8.0, nullable, `TreatWarningsAsErrors`, deterministic, invariant globalisation. `global.json` 8.0.133 with `rollForward: latestPatch`. No NuGet packages.
- **Dependencies:** P0 `SitkaLab.Core.dll` is a binary reference from git-ignored `.p0ref/`, SHA-pinned in `SitkaP1.Core.csproj` (`9a99f781…34b3`). The V-01 pair `SitkaGrowthModel.cs` and `SimulationRandom.cs` is compile-linked from `Assets/ForestPrototype/` (read only).
- **Engine authority:** `AnnualEngine.Open(domain, fixtureId, dir)` is public; `ContinueInMemory()` and `AnnualContinuation` are **internal**.
  - `AnnualContinuation` holds one `sync` lock and one immutable `WorkRoot(Snapshot, WorkRevision, Events, Executed, ApprovedUnexecuted, Pending)`.
  - Members: `Frame`, `CurrentToken`, `AddPendingFell`, `RemovePendingFell`, `ApprovePlan(planId)`, `CancelApprovedPlan`, `Execute(plan, key, seam)`, legacy `Advance(year, rev)` (fails closed after a Fell, A-1) and the 4-argument `Advance(year, rev, workRev, consent)`.
- **Gaps against the approved contract:** no PlanStateRevision; no session identity; `Frame` and `CurrentToken` lock separately; pending and approved state are not readable; Add and Approve take no expected token; replay compares source and orders but not PlanId (A-2).
- **Coordinates:** the fixture `xzRule` `UnityX=(xMm−15000)/1000`, `UnityZ=(yMm−15000)/1000`. `AnnualTree.XM`/`ZM` ∈ [−15, 15) m, +X east, +Z north; the parser enforces trees in [0, 30000) mm. `CrownRadiusM` is per tree in the source.
- **UI today:** P1 has **no UI**; its README marks "Host / UI … Not implemented" and "P1 never binds port 5187". Precedent: sibling P0 lab `Tools/MultiRackSitkaLab/src/Host` is an ASP.NET Core minimal API on `127.0.0.1:5187` with Host/Origin guards. Its README records owner operation from Microsoft Edge on Windows through WSL2.
- **Test constraints for any future UI:**
  - the runner registers suites explicitly from `tests/Core.Tests/Program.cs` (no discovery);
  - test `644 B-01` (`Program.cs:428`) bans `File.Write`, `File.Open`, `File.ReadAll`, `FileStream`, `StreamWriter` and similar in every `src/**/*.cs`, so a UI under `src/` must serve embedded resources;
  - no-work digests serialise only `{ContinuationSnapshot, AnnualReport}` (`ContinuationTests.cs`), so internal `WorkRoot` fields can be added without moving them.

## 4. Scenario One Stand Map — dependency proof and decision (deliverable A)

### 4.1 Orientation, raster and selection-relevant fields (read only)

- `StandMapTransform` (blob `73cbe31d`): north-up. `WorldToFraction` = ((x − MinX)/Size, 1 − (z − MinZ)/Size), so Y is measured from the **north** edge. The same transform serves the full map and the minimap; `PixelsToWorld` inverts it.
- `StandMapCanopyRaster` (blob `634c61d1`): an RGBA byte buffer whose first row is the **south** edge. Each tree is drawn at its true position and crown radius, shortest first, with per-size cached sprites, a cosmetic variant from `MapTree.Seed`, light from the north-west and 768/256 px sides. Stumps are small pale discs; standing dead use a muted base colour.
- `StandMapModel` (blob `8fe67ec5`): `MapTree { X, Z, CrownRadiusM, HeightM, DbhCm, Kind, State {Living, StandingDead, Stump}, Mark {None, Fell, Crop}, Seed }`. It has **no `TreeId`**. Trees are sorted by height by the (Unity-bound) source. The model carries rack, path, landmark, juvenile, shelter and player fields.
- `StandMapSymbols` / `StandMapLegend` (blobs `4487eff1` / `bc64262d`):
  - one style table with colour words and meanings, "colour never carries the meaning alone";
  - a legend that lists only categories present;
  - rack-centred vocabulary ("Rack in the Work Plan", "harvested rack"…); the Fell mark is a red ring.

### 4.2 Exact transitive closure of the six named pure files

All files are in the **global namespace**. A code scan with comments stripped found **no** UnityEngine, Mathf, Texture2D, VisualElement or MonoBehaviour usage; the only hits are in comments.

| File | Blob @ main `7758e17c` | Lines | Depends on (outside itself) |
|---|---|---|---|
| `Assets/ForestPrototype/UI/Map/StandMapModel.cs` | `8fe67ec5ce56a868f726da8e7ad01c0a5aaa3ece` | 275 | `RackPoint`, `RackGeometry.DefaultWidthM`, `StandMapTransform` |
| `Assets/ForestPrototype/UI/Map/StandMapTransform.cs` | `73cbe31de1c23a8544935cccdc0491cddf981cfb` | 76 | `RackPoint` |
| `Assets/ForestPrototype/UI/Map/StandMapCanopyRaster.cs` | `634c61d1eecd2ab2bec770c3134544fc70e7e2a3` | 452 | Model, Symbols |
| `Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs` | `0d39e4ec48ebf91ce5a28b13327962edcfcbadcb` | 389 | Model, Transform, Symbols, **`StandMapGridPlan`**, `RackPoint` |
| `Assets/ForestPrototype/UI/Map/StandMapSymbols.cs` | `4487eff168802e8dc1cca6ef5db6509146a42828` | 130 | Model enums, `RackGeometry.DefaultWidthM` |
| `Assets/ForestPrototype/UI/Map/StandMapLegend.cs` | `bc64262d8e77f60edeb1b023a6d230b4b181c6ec` | 139 | Model, Symbols, **`StandMapGridPlan`** |
| `Assets/ForestPrototype/UI/Map/StandMapGridPlan.cs` *(pulled in)* | `5c2b24e3a47d51f8c8cefa7c0b7aad59577cd6e7` | 130 | Model, Transform types |
| `Assets/ForestPrototype/Racks/RackGeometry.cs` *(pulled in)* | `04aca81e6295617a2b00a8e8fd246fd2f4be8834` | 439 | none (self-contained rack-planning domain) |
| **Total** | 8 files | **2,030** | |

`StandMapSource.cs` (blob `c8f9bfbf`) is Unity-bound (`ForestTree` scene scan, `ScenarioOneManager`). It is excluded by the owner and is not reused. `StandMapCanvas`, `StandMapCanopyLayer`, `StandMapSurface` and `StandMapRackEditor` are Unity UI and out of scope.

### 4.3 Decision: **Option B — minimal local, presentation-compatible layer**

Specific reasons A is not available (the generic "too hard" is not among them):

1. **Missing grant.** V-01 covers only `SitkaGrowthModel.cs` (blob `e4c73402`) and `SimulationRandom.cs` (blob `bb83d7f8`). Option A requires a new prospective read/compile grant from Scenario One #844 and the product owner for all eight files above. This author has no authority to grant or assume it.
2. **The closure carries the forbidden rack domain.** Even the transform alone requires `RackPoint`, which lives in `RackGeometry.cs` with the full rack-planning model (`RackPlanRack`, `RackOrderLifecycle`, `RackPlanOperationResult`, `RackRows`, corridor clipping). Linking it compiles Scenario One rack planning into the Multi assembly, contrary to G0 ("no rack mechanism"). Isolating only the pure transform would require a Scenario One refactor, which is owned by Scenario One and not allowed here.
3. **No exact tree identity.** `MapTree` lacks `TreeId`; `Seed` is derived from it. Exact selection would require editing Scenario One source (forbidden) or a parallel ID array kept index-aligned with a height-sorted list. That is the fragile "derived/nearest" identity the handoff warns against.
4. **Work-state vocabulary mismatch.** `MapTreeMark {None, Fell, Crop}` and the rack-worded legend cannot represent pending ≠ approved ≠ executed, so a local overlay and legend would be needed anyway.
5. **Raster has no per-tree identity or accessibility.** The canopy raster produces pixels, with no per-tree element for an exact hit-test or assistive technology, and needs PNG encoding to reach a browser. Its main advantage (sprite caching for ~1,300 trees) is irrelevant at 100 trees.

Honest non-reasons: Unity coupling (none in the closure) and closure size (modest). Items 2–5 would still favour B **even if a grant were given**, so no Scenario One grant is requested.

**What B is:** new, small code written by this author inside `Tools/MultiRackSitkaP1Lab`. `src/Core/PlayerMapScene.cs` holds pure geometry, paint order and identity hit-test; the page renders per-tree SVG.

- **Conventions adopted, design reference only:** north-up; X east, Z north, fraction from the west and north edges; true position and crown radius; taller crowns over shorter; felled → stump with the canopy gap opening; natural death muted; dashed property line, metre grid, scale bar, north arrow; legend of present items only; colour never alone.
- **Not done:** no code copied, no Scenario One file edited or linked, no pixel-match claim, no five racks, corridor, headland or marking. Unity may later consume the same `PlayerSessionView` through its own adaptor (future, separately gated).

## 5. Frontend runtime — the material G5 decision (not self-granted)

Every option that meets the accessibility requirements (clickable map, keyboard, 360 CSS px, 200 %, screen reader) needs either a local network channel or a native GUI binding. Dispatch §5 forbids "Host/server/network architecture", and Stage 5 FR-16/G3/R-23 forbid a "Host". This is therefore a genuine owner G5 boundary. The two smallest real alternatives (spec §3):

| | **UI-A (recommended)** | **UI-B (alternative)** |
|---|---|---|
| What | One single-session, loopback-only UI process: Kestrel minimal API (ASP.NET Core **shared framework already installed**) serving an embedded HTML/CSS/JS page that calls the in-process adapter | Native GTK 3 window via hand-written `DllImport` to system `libgtk-3.so.0`, drawn through WSLg |
| New dependency | None downloaded: `~/.dotnet/shared/Microsoft.AspNetCore.App/8.0.34` and `packs/Microsoft.AspNetCore.App.Ref/8.0.34` present | None downloaded: `libgtk-3.so.0`, `libcairo.so.2` present; `DISPLAY=:0`, `WAYLAND_DISPLAY=wayland-0` |
| Network | Yes — `127.0.0.1` only, ephemeral port (not 5187), capability path + Host/Origin guards (spec §3.6) | None |
| Accessibility testing | Real browser: keyboard, 360 px, 200 %, touch emulation, screen reader (NVDA/Edge on Windows) | Orca inside WSL only. Windows NVDA reading WSLg windows is **expected not, NOT VERIFIED**. No per-tree accessible objects on a custom-drawn map |
| Evidence | Real screenshots/recordings from a Windows browser; automated HTTP guard tests | Manual screenshots only |
| Maintainability | Plain C# endpoints plus one small page; P0 precedent | Several hundred lines of interop and callback lifetime code |

**Recommendation: UI-A**, with the exact security envelope of spec §3.6. If approved, the dispatch's "no server/network" prohibition and Stage 5's "no Host" wording are **narrowly qualified** (spec §0.3 Q-1) for this presentation process only. Saves, persistence, multi-user use, cross-process replay and Host authority all remain forbidden.

**Environment proof for UI-A (no repository writes):** a throwaway `python3 -m http.server` bound to `127.0.0.1:47613` inside WSL for 40 s, in `/tmp` (removed afterwards), was fetched from Windows `curl` at `http://127.0.0.1:47613/probe.txt`, returning `ccf-standmap-loopback-probe` with rc 0; `http://localhost:47613` also returned 200. WSL networking mode: `nat`. No .NET server was run, and **UI rendering was not tested.**

## 6. Minimum human-usable UI (spec §7)

A standalone north-up 30 m top-down map of the one undivided area, drawing each tree at its real position with its real crown radius, alongside a 100-row exact-ID table (the authoritative keyboard and screen-reader path) and an inspector.

- **Selection:** a click resolves overlaps to an exact displayed `TreeId` (topmost, with listed alternatives; bare ground selects nothing; server hit-test on the current frame cross-checked with the browser's event target).
- **Tree states:** live, felled (stump, canopy gap) and natural-dead are distinct; pending (dashed), approved (solid double) and executed are distinct by pattern and text, not colour alone.
- **Inspector:** measurements and a required written reason.
- **Work Plan:** explicit Work Plan approval ("0 trees felled"), separate Execute (one real typed Fell event, with a confirmation naming the exact count and year), then Advance Year+1 (consent dialog when unexecuted work exists).
- **Figures and labels:** counts, stems/ha, BA m²/ha and Dq from the authoritative `CommittedFrame.Stand`; model/fixture/year/annual/work/plan-state revision labels; "Synthetic 30 m Sitka experiment"; "Comparison extension: DISABLED — Manual/NotCovered"; money "UNAVAILABLE (not €0)"; "Session only — nothing is saved".
- **Errors:** visible stale and refused alerts; an "outcome unconfirmed — re-read" state that never re-executes under a new key.
- **Absent by design:** no rack corridor, entrance, five-rack layout, marking action, proposal or sample.

## 7. Spec delta (deliverable B) and what needs acceptance

| File | Content |
|---|---|
| `specs/multirack-sitka-2d-playable-standmap-v1/spec.md` | DRAFT architecture delta: frontend options and launch path (G5); map path B; selection identity S-1…S-8; facade ABI (`PlayerSessionView`, `PlayerFence`, fenced commands, PlanStateRevision table, A-2 replay rule, server-minted PlanId/ExecutionKey, new refusal codes); UI regions and labels; accessibility A-1…A-7; AC-PM-01…13 |
| `…/plan.md` | Phases II–V, Tier-1 evidence lane discipline, the PS-01…19 and MS-01…06 test list, risks |
| `…/tasks.md` | Exact path list with base blobs, not-written list, tasks PM-T00…T09 |
| `…/checklists/requirements.md` | 14 PM-R + 16 PM-X, all unchecked, each traced to Stage 5 R/X ids |

The approved Stage 5 four files are **not edited**. Statements this delta proposes to qualify are listed explicitly in spec §0.3, and each requires **#730 acceptance**:

- **Q-1:** "no Host/server/network" → a loopback presentation process, if G5 chooses UI-A.
- **Q-2:** Execute increments PlanStateRevision (closes the contract's open engineering item).
- **Q-3:** the local sample is not delivered by this slice.
- **Q-4:** the map is the primary visual; the table remains the a11y authority.
- **Q-5:** new public ABI in the qualified `SitkaP1.Core` (G4-class).
- **Q-6:** minimal session history without a "proposed" column (G1 disabled).

## 8. Exact writable production path list (deliverable C)

All paths are relative to `Tools/MultiRackSitkaP1Lab/`. Details and base blobs are in `tasks.md`.

| Path | Kind | Justification for any existing file |
|---|---|---|
| `src/Core/AnnualContinuation.cs` | modify | The same-lock view and fences must live with the only `sync` lock and `WorkRoot`. Edits are limited to new `WorkRoot` members, the session ID, the six existing publish lines (plan-state increment, annual record) and new internal members. Legacy validation order and refusal codes are unchanged |
| `src/Core/PlayerSessionView.cs` | new | |
| `src/Core/PlayerWorkPlanAdapter.cs` | new | |
| `src/Core/PlayerMapScene.cs` | new | |
| `tests/Core.Tests/PlayerSessionTests.cs` | new | |
| `tests/Core.Tests/PlayerMapSceneTests.cs` | new | |
| `tests/Core.Tests/Program.cs` | modify, **+1 line** | The runner has no discovery. One registration line after line 441. A `[ModuleInitializer]` workaround was rejected as non-obvious |
| `src/Ui/SitkaP1.Ui.csproj`, `src/Ui/Program.cs`, `src/Ui/PlayerApi.cs` | new (UI-A) | **Not** added to the `.sln`, so the qualified solution build graph is unchanged |
| `src/Ui/wwwroot/index.html`, `app.css`, `app.js` | new (UI-A) | Embedded resources (complies with test `644 B-01`) |
| `tests/ui-smoke.sh` | new (UI-A) | |
| `README.md` (lab) | modify | Real launch path; replaces the stale "Host / UI not implemented" row |

**Not changed:** `WorkContract.cs` (reuse its public records), `AnnualEngine.cs` (internal `ContinueInMemory` is reachable from the Core facade), `AnnualRules.cs`, fixtures and goldens, `src/Cli/**`, `.sln`, `SitkaP1.Core.csproj` (SDK globbing compiles new files), `SitkaP1.Core.Tests.csproj`, existing test files, `.p0ref/**`, `build/**`, `Tools/MultiRackSitkaLab/**`, `Assets/**`. If UI-B is chosen, the UI rows are re-issued before writing.

## 9. Environment proof (deliverable D)

### 9.1 P0 reference (copied, never rebuilt)

- Source copy: `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-A1-WorkRevision-Fence/Tools/MultiRackSitkaP1Lab/.p0ref/` (A-1 source worktree at the same HEAD `4bb80973`; read only). Seven byte-identical copies exist on this host.
- Verified equal: DLL SHA-256 = csproj pin = `SitkaLab.Core.dll.sha256` record = `provenance.txt` `dllSha256` = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`.
- Provenance: `p0QualifiedCommit=a7f41983…`, `p0QualifiedTree=f212c8f6…`, built with SDK 8.0.133.
- Installed into the pristine clone and into this lane's git-ignored `.p0ref/` (nothing staged).

### 9.2 Build and test results

SDK: WSL `~/.dotnet/dotnet` **8.0.134** (satisfies `global.json` 8.0.133 + latestPatch). The host `/usr/bin/dotnet` 10.0.115 was not used. `TMPDIR=/tmp/ccf-standmap-p1-tmp`.

| Run | Where | Build | Tests | Ten digests |
|---|---|---|---|---|
| **Authoritative baseline** | pristine WSL-native LF sparse clone of `4bb80973` at `~/.ccf-orch/standmap-p1-opus/base-clone-4bb80973` (`git clone --shared --no-checkout`, sparse paths: both labs + the four pinned Assets files) | **0 warnings, 0 errors** | **66/66 passed, 0 failed** | **all identical** to the recorded literals (`df2d44fd…`, …, `10d31913…`); deaths per year 0,0,1,0,0,1,2,4,5,0 |
| Lane, as-is | this checkout via `/mnt/c` | 0 / 0 | 64/66 | identical |
| Lane + `GIT_DIR=<admin dir>` | same | 0 / 0 | 65/66 | identical |

Exact commands (scripts in Appendix A):

```bash
bash make-pristine-clone.sh /home/jer/Development/CCF-HP ~/.ccf-orch/standmap-p1-opus/base-clone-4bb80973 4bb809732fb691f15630e1fbc5ea20dca6dbbe2f
bash p0ref-copy-verify.sh <A-1 worktree>/Tools/MultiRackSitkaP1Lab/.p0ref <clone>/Tools/MultiRackSitkaP1Lab
TMPDIR=/tmp/ccf-standmap-p1-tmp bash build-and-test.sh <clone>/Tools/MultiRackSitkaP1Lab ~/.ccf-orch/standmap-p1-opus/logs-clone
#   inside: dotnet build MultiRackSitkaP1Lab.sln ; dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
```

The clone's `git status --porcelain` was empty after the build. Core DLL SHA-256 in the clone: `e067ceee…798a`. In the lane it is `ceb1f425…8867`; the difference is expected, because deterministic builds embed source checksums and the lane's sources are CRLF.

### 9.3 Environment obstacles (precise)

1. **Lane git access from WSL:** the worktree's `.git` points to a `//wsl.localhost/…` UNC path that WSL git cannot open. Tests `original: trust records match pinned P0 blobs…` (`Program.cs:196/208`) and `source: linked CCF files are byte-identical…` (`Program.cs:337`) fail with `git … failed`. Setting `GIT_DIR` to the admin dir fixes git access without writing anything.
2. **CRLF in the lane:** `autocrlf=true` makes `Assets/ForestPrototype/SimulationRandom.cs` hash `05cd6c90…` versus the pinned `d1b52a60…`, so the `CcfSourcePins` working-tree check fails in the lane only. **Consequence:** author Tier-1 must run in a pristine LF clone of the **pushed** SHA (plan P1). The lane is for editing and fast loops only.
3. **Mode artifacts:** 563 phantom `100755 → 100644` entries. They are never staged; commits stage explicit paths only.
4. **`prunable` worktree in WSL:** do not prune.
5. **Lane git identity** is the non-author DeepSeek parent (see §0).

External evidence (WSL, not in Git), all under `/home/jer/.ccf-orch/standmap-p1-opus/`:

| File | SHA-256 |
|---|---|
| `logs-clone/tests.txt` | `4e6d17005110424eb61969a448e8dff0562a83a6fe83945da32631df772a7b7c` |
| `logs-clone/build.txt` | `c725b025700db11f815d061270125b955cde2ce02d17a314530a44069f54612f` |
| `logs-lane-asis/tests.txt` | `48099f7810c754aafbb5ae239358a5dd78c1bda853309a0f93cab1eca1a1e479` |
| `logs-lane-gitdir/tests.txt` | `c70a059abe740be2c62d7ff23c636380155523173dcebf3125e04976e7f49bd4` |
| `p0ref-clone.log` | `38c569478329377df5a916b97034f002162b49f886bfdbb5e784790a1951e3b7` |

The same directory also holds `clone.log` and `p0ref-lane.log`.

## 10. Blocked, by whom

| Blocked item | Waiting on |
|---|---|
| Any production write (Core facade, tests, UI) | **#730 Manager** acceptance of the spec delta (Q-1…Q-6, §6 ABI, path list) |
| Any `src/Ui/**` write and the choice of path rows 8–14 | **Owner G5:** UI-A or UI-B |
| Lock/claim registration, `active-tasks.md` refresh, reviewer admission | **Parent** (administrative) |
| Scenario One map grant | **Not requested** (Option B) |

## 11. Not verified by this worker

- No production code, UI, rendering or browser run. No screen reader. No 360 px or 200 % test.
- No Kestrel or GTK feasibility program was run; only a Python loopback probe and package/library presence checks.
- Live `active-tasks.md` and the #730 thread were not re-read. Claim and lock state is the parent's responsibility.
- No full `p0-isolation.sh`, `build-output-safety.sh` or `cli-safety.sh` run (not required for Phase I).
- The Phase II/III design is unimplemented, so no Stage 5 R/X item and no PM item is PASS.
- Owner and #730 positions on Q-1…Q-6 are unknown.

## 12. Standard handoff block

```text
IMPLEMENTATION HANDOFF (PHASE I — DOCS/SPEC + ENVIRONMENT PROOF ONLY)
Task: Multi-StandMap-Playable-2D, Phase I
Worker: Claude Opus 5.5 (worker-reported) via T3 Code
Role: primary implementer/integrator (non-reviewer)
Branch: feat/multi-standmap-playable2d-20261120-v1
Base: 4bb809732fb691f15630e1fbc5ea20dca6dbbe2f
Context commit: 4bb809732fb691f15630e1fbc5ea20dca6dbbe2f
HEAD: (this commit; reported to the parent after push)
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/Multi-StandMap-Playable-2D-PhaseI-Architecture-Proof-20261120.md
GITHUB HANDOFF COMMIT: (this commit)
EXTERNAL EVIDENCE ARCHIVE: WSL /home/jer/.ccf-orch/standmap-p1-opus/ (hashes in §9.3)
WHAT CHANGED:
- Added this Phase I note and the DRAFT four-file spec delta specs/multirack-sitka-2d-playable-standmap-v1/
FILES CHANGED:
- 5 new Markdown files; no source, test, project, fixture or Assets file
SERIALIZED UNITY FILES TOUCHED:
- none
PLAYER-FACING RESULT:
- none yet (design only)
SIMULATION / SAVE IMPACT:
- none
ASSETS ADDED OR MODIFIED:
- none
MUST-NOT-CHANGE HASHES / ANCHORS:
- ten no-work digests; P0 pin 9a99f781…34b3; Tools/MultiRackSitkaLab tree f212c8f6; V-01 blobs e4c73402/bb83d7f8; fixture
INTENTIONALLY CHANGED HASHES / ANCHORS:
- none
VERIFICATION RUN:
- pristine-clone build + 66-test run at 4bb80973; lane runs as-is and with GIT_DIR; P0 ref 4-way SHA check; loopback probe
VERIFICATION RESULTS:
- clone 0W/0E, 66/66, digests identical; lane 64/66 (git access) and 65/66 (CRLF), digests identical; P0REF VERIFIED; probe OK
SPEC CONFORMANCE (SPEC REQUIRED only): NOT VERIFIED (no implementation)
REMAINING VERIFICATION GATES:
- owner G5; #730 acceptance; then Phase II/III Tier-1 and independent Sol Tier-2
KNOWN LIMITATIONS:
- §9.3 lane obstacles; §11
UNRELATED WORK PRESERVED:
- 563 mode-only lane artifacts untouched; main checkout, A-1 worktrees and receipt lane read only (.p0ref copied from A-1 fence worktree, read only)
DECISIONS NEEDED:
- Owner G5: UI-A (recommended) or UI-B. #730: accept or amend Q-1…Q-6, the §6 ABI and the path list
WORKER ASSESSMENT:
Ready for review: Conditional (ready for #730 design review; no source to review)
RECOMMENDED NEXT STEP:
- Parent routes the G5 question and this delta to #730/owner; on acceptance, start PM-T01 (Core facade) in this lane
```

## Appendix A — Phase I verification scripts (run from outside the repository)

These scripts were run as written; they are kept here, not as repository files. `make-pristine-clone.sh`:

```bash
#!/usr/bin/env bash
set -euo pipefail
src="$1"; dest="$2"; commit="$3"
[[ ! -e "$dest" ]] || { echo "STOP: $dest already exists" >&2; exit 4; }
git clone --shared --no-checkout --quiet -c core.autocrlf=false "$src" "$dest"
git -C "$dest" config core.autocrlf false
git -C "$dest" sparse-checkout set --no-cone \
  /Tools/MultiRackSitkaP1Lab/ /Tools/MultiRackSitkaLab/ \
  /Assets/ForestPrototype/SitkaGrowthModel.cs /Assets/ForestPrototype/SimulationRandom.cs \
  /Assets/ForestPrototype/Species/SitkaSpruce.asset /Assets/ForestPrototype/TreeSpeciesDefinition.cs
git -C "$dest" -c advice.detachedHead=false checkout --quiet --detach "$commit"
echo "HEAD=$(git -C "$dest" rev-parse HEAD)"
git -C "$dest" status --porcelain
```

`p0ref-copy-verify.sh` (core logic): it copies the three `.p0ref` files with `cat` (refusing symlinks and non-regular files), then requires `sha256(dll)` = the `<P0CoreExpectedSha256>` of the destination csproj = the `.sha256` record = `provenance.txt` `dllSha256=` = `sha256(source dll)`. On any disagreement it exits 3 and prints `STOP`.

`build-and-test.sh` (core logic):

```bash
export DOTNET_ROOT="$HOME/.dotnet" PATH="$HOME/.dotnet:$PATH" DOTNET_CLI_TELEMETRY_OPTOUT=1 DOTNET_NOLOGO=1
cd "$p1"; rm -rf src/Core/{bin,obj} src/Cli/{bin,obj} tests/Core.Tests/{bin,obj}
dotnet build MultiRackSitkaP1Lab.sln --nologo
dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll   # PASS/FAIL lines + "N/M passed"
```
