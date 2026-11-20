# Multi — Playable standalone 2D Stand Map and player session facade (V1)

- **Spec ID:** MULTI-STANDMAP-PLAYABLE-2D-V1
- **Status:** **DRAFT — architecture delta awaiting (1) owner G5 frontend-runtime decision and (2) Multi Manager #730 acceptance. NO PRODUCTION SOURCE WRITE IS AUTHORISED BY THIS FILE.**
- **Date:** 20 November 2026
- **Author:** Claude Opus 5.5, primary implementer/integrator for task *Multi-StandMap-Playable-2D* (model identity is worker-reported; see the Phase I note §0). Independent reviewer reserved: GPT-6.1 Sol (non-author).
- **Phase I evidence:** [`Docs/Handoffs/MultiRack-Simulation-20261116/Multi-StandMap-Playable-2D-PhaseI-Architecture-Proof-20261120.md`](../../Docs/Handoffs/MultiRack-Simulation-20261116/Multi-StandMap-Playable-2D-PhaseI-Architecture-Proof-20261120.md) (same commit as this file).
- **Base source:** `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f` (A-1, accepted by #730 as scoped); branch `feat/multi-standmap-playable2d-20261120-v1`.

## Authority and parent documents (read, not amended)

| Document | Ref read | Role here |
|---|---|---|
| Owner one-shot handoff | `docs/Multi-StandMap-Playable-2D-OneShot-Orchestration-20261120-v1` @ `7ec4a7a1` | Task scope, gates, prohibitions |
| A-1 Manager acceptance | `docs/Multi-A1-Manager-Acceptance-and-Next-UI-Gate-20261120-v1` @ `1dee048b` | Exact base; A-2 still open; "no connected UI exists yet" |
| **APPROVED Stage 5 four-file design** | `docs/multi-730-837-g2-contract-decision-draft-20261119-v1` @ `6245755c`, `specs/multirack-sitka-2d-intervention-workflow-v1/` | Gameplay authority. Its 26 R + 24 X stay authoritative and unchecked |
| Player Work Plan UX preflight | `Multi-stage5-player-workplan-ux-preflight-20261119-v1` @ `58cbba76` | Screens S0–S7, refusal table, accessibility |
| Source-fenced adapter contract | `docs/multi-player-session-adapter-contract-20261119-v1` blob `75593a12` | Same-lock view, PlanStateRevision, A-2 replay rule |
| Manager review of that contract | `docs/Multi-Player-Adapter-Manager-Contract-Verdict-20261119-v1` @ `f9e77874` | "APPROVED as contract direction; HELD production code" |
| Stand Map reuse assessment | `docs/Multi-StandMap-Reuse-Assessment-20261120-v1` @ `ec044b4d` | Reuse conventions; do not reuse `StandMapSource`/Unity UI |

Order of precedence follows `Docs/Project/AgentWorkflow.md` §Spec authority. Where this delta must qualify an approved statement, it says so in §0.3 and that qualification is **not effective until #730 accepts it**.

## 0. Relationship to the approved Stage 5 design

### 0.1 What this delta adds

The approved Stage 5 design covers the one-area manual Work Plan loop but leaves five architecture items open. This delta specifies only those:

1. the standalone frontend technology, packaging and real user launch path (§3, **owner G5**);
2. the map presentation path: Scenario One reuse versus local layer (§4, decision **B: local layer**);
3. exact tree selection identity (§5);
4. the concrete player session facade ABI that realises spec.md §8 G2-5 and the adapter contract (§6);
5. the test plan for the above (§9 and `plan.md`).

### 0.2 What this delta does not change

- Gameplay: one undivided 30 × 30 m, 100-Sitka management area; manual per-ID Fell; PendingWork → explicit whole-plan approval (zero Fell) → separate Execute (one atomic typed Fell event) → genuine Year+1 from survivors; warn/consent/stale-cancel on advance with unexecuted work; D-093 (170 mm informational; all-live batch refused).
- Gates: G0 RESOLVED (no rack), G1 **DISABLED — Manual/NotCovered**, G3 no save/persistence, G4 no economics (**UNAVAILABLE**, never €0).
- Protected: the 100 original `TreeId`s; the ten no-work Year0–10 digests; `ContinuationSnapshot` and `AnnualReport` shapes; `AnnualRules.cs`; fixtures; goldens; `.p0ref`/P0; `Tools/MultiRackSitkaLab/**`; `Assets/**`; Scenario One sources.
- The approved 26 R + 24 X checklist remains the gameplay acceptance authority. This delta's own checklist covers only the new architecture and points back to R/X ids.

### 0.3 Explicit qualifications of approved statements (each needs #730 acceptance)

| Id | Approved statement | Qualification proposed here | Why |
|---|---|---|---|
| Q-1 | Stage 5 spec FR-16 and §7 G3 "no … Host/H2"; checklist R-23 "no Host/H2"; dispatch §5 "no Host/server/network architecture" | **Only if owner G5 selects UI-A (§3.2):** permit one single-session, loopback-only, in-memory UI *presentation* process. It is not a source authority, not a persistence Host, not H2, not a service and not multi-user. All R-23 no-save / no-cross-process / no-localStorage-authority rules stay | A browser UI needs a channel to the in-process engine; the approved texts forbid a Host without defining one. This must be decided, not inferred |
| Q-2 | Adapter-contract review item 2: "whether executing work also increments the plan-state revision is a required engineering consistency decision" | **Decided: yes.** A successful Execute increments `PlanStateRevision` by exactly one (it clears the approval); a replay does not | Closes the open engineering item as the Manager recommended |
| Q-3 | Stage 5 §3 step 2, FR-02, R-06, X-05: optional representative local sample | **Not delivered by this slice.** Manual per-ID selection only. X-05 evidence from this slice is limited to the manual fallback and the G1-disabled label. The requirement itself is not removed | Keeps the first playable slice minimal; a sample needs its own evidence design |
| Q-4 | UX preflight S0: "a 2D map is an optional coordinate-backed companion to a searchable semantic table" | The map becomes the **primary visual surface** (owner handoff). The table stays the **authoritative keyboard and screen-reader path** for ID precision | Owner asked for a map-first UI; accessibility authority unchanged |
| Q-5 | Stage 5 spec §8 G2-5 "one immutable, bounded public current-source view" (PROPOSED) | Realised as public `PlayerSessionView` plus `PlayerWorkPlanAdapter` in `SitkaP1.Core` (§6). This **adds public ABI to the qualified Core assembly** | A G4-class protected shared-source change; needs #730 |
| Q-6 | FR-12 "previous-versus-current-versus-proposed" history | Session-local history = existing `WorkEvent` ledger + per-tree `AnnualDeath` + new immutable `AnnualYearRecord` per committed year. No "proposed" column, because G1 is disabled | Minimal truthful history without invented proposals |

## 1. Scope

**In:** one standalone UI process connected in-process to the existing `AnnualContinuation`; a north-up 30 m map that draws real positions and crown radii; exact per-tree selection; an inspector with measurements and a written reason; Add/Remove pending; review and approve the whole Work Plan (zero Fell); Execute (one real typed Fell event); fresh re-read; Advance Year+1 with consent handling; truthful labels; refusals and "outcome unconfirmed" handling; accessibility as specified in §8.

**Out:** racks, corridors, entrances, five-rack layout, headland, field marking; G1 comparator, proposals or samples; economics; save/load or any persistence; multiple sessions or users; Unity, Scenario One edits or Scenario One code linking; P0 changes; a second biology engine; new NuGet packages; Windows builds; main merge.

## 2. Architecture overview

```text
fixtures/derived (trusted, catalog-checked, no-follow read)
   │ AnnualEngine.Open(DerivedP1ThirtyMetreExperimentV1, "p1d-30m-sitka-adult-v1", dir).ContinueInMemory()
   ▼
AnnualContinuation  ── one lock (sync), one immutable WorkRoot ──  the ONLY state and the ONLY engine
   │ same-lock atomic read + fenced commands (new internal members, §6)
   ▼
PlayerWorkPlanAdapter (new, public, in SitkaP1.Core; holds no state of its own)
   │ PlayerSessionView (immutable DTO)  ──►  PlayerMapScene (pure geometry + identity hit-test, no colours)
   ▼
SitkaP1.Ui process (new, G5)  ── JSON ──  browser page: SVG map + tree table + inspector + Work Plan
```

The UI never mutates engine state directly, never deletes a tree visually ahead of a committed result, keeps no copy of "felled" trees, counters, RNG or clocks, and derives every displayed actual value from the latest `PlayerSessionView`. A local click selection is a UI-only draft, distinct from engine-side PendingWork.

## 3. Frontend technology, packaging and launch path — **OWNER G5 REQUIRED**

### 3.1 Constraints that decide the choice

The UI must offer clickable rendering and keyboard control (no output-only console map, no mock), testing at a 360 CSS-px equivalent viewport and 200 % scale, a keyboard-only path and screen-reader support. It must also respect: .NET 8 (SDK 8.0.134 present), no new NuGet packages, no Windows build, no Unity, no persistence, and dispatch §5 "no Host/server/network architecture". Everything available that satisfies the accessibility requirements needs either a local channel (network) or a native GUI binding, so this is a genuine runtime decision. It is not self-granted here.

### 3.2 UI-A — RECOMMENDED: single-session loopback browser UI process

- **Project:** new `src/Ui/SitkaP1.Ui.csproj` (assembly `sitkap1-ui`, `net8.0` Exe). It references `../Core/SitkaP1.Core.csproj` and `FrameworkReference Microsoft.AspNetCore.App`, using the shared framework **already installed** (`~/.dotnet/shared/Microsoft.AspNetCore.App/8.0.34`, targeting pack `packs/Microsoft.AspNetCore.App.Ref/8.0.34`). It needs **no NuGet download**. The sibling P0 lab used the same framework for its owner-qualified loopback UI.
- **Server:** `WebApplication.CreateSlimBuilder` and Kestrel `Listen(IPAddress.Loopback, 0)`. The OS assigns an ephemeral port, never 5187 and never privileged, and the process prints it. No HTTPS, cookies, antiforgery, authentication or data-protection features are used, so no key ring is created.
- **Assets:** `index.html`, `app.css` and `app.js` are compiled into the assembly as embedded resources and served from memory. The UI makes **no file-system read or write** at runtime apart from the existing trusted fixture read in Core, and complies with the existing `644 B-01` banned-file-API scan of `src/**/*.cs`.
- **Session:** one `PlayerWorkPlanAdapter` per process. Process exit ends the session; nothing is saved.
- **Fallback inside UI-A (same envelope):** BCL `System.Net.HttpListener` instead of Kestrel, if the owner prefers zero additional shared framework.

### 3.3 UI-B — ALTERNATIVE: native GTK 3 window, no network

`src/Ui` would call the system `libgtk-3.so.0` through hand-written `DllImport` (present in this WSL; shown through WSLg, `DISPLAY=:0`). The map would be drawn with cairo in a `GtkDrawingArea`, with GTK widgets for the table, inspector and Work Plan. No socket and no NuGet. The costs:

- roughly several hundred lines of interop with callback lifetime management, not learner-friendly;
- Linux/WSLg only;
- the custom-drawn map has no per-tree accessible objects;
- screen-reader testing would be Orca inside WSL. Whether Windows NVDA can read WSLg windows is **expected not, NOT VERIFIED**;
- "360 px / 200 %" can only be approximated with `GDK_SCALE`;
- no automated UI witness is available.

### 3.4 Rejected (not offered)

| Option | Reason |
|---|---|
| WinForms / WPF | Windows Desktop SDK not present; would be a Windows build |
| Avalonia, Eto, GtkSharp, Photino, WebView2, Electron | NuGet/external packages |
| Blazor WebAssembly / `browser-wasm` | requires the `wasm-tools` workload download |
| Terminal UI | cannot draw true-radius crowns legibly; fails "no output-only console map"; weak accessibility |
| Static HTML export | cannot drive the live engine; would be a mock |

### 3.5 Real user launch path (UI-A)

```bash
# Ubuntu (WSL2), from the repository root
cd Tools/MultiRackSitkaP1Lab
build/build-p0-core-ref.sh            # only if .p0ref/ is absent; or copy a pinned copy and verify its SHA (see plan.md)
dotnet build src/Ui/SitkaP1.Ui.csproj
dotnet src/Ui/bin/Debug/net8.0/sitkap1-ui.dll fixtures/derived
#   prints:  Open http://127.0.0.1:<port>/s/<capability>/   — Ctrl+C ends the session; nothing is saved
```

Open the printed URL in Microsoft Edge on Windows. WSL2 NAT localhost forwarding was re-verified for a dynamic high port on 20 November 2026. Use the printed `127.0.0.1` address exactly; other host names are refused (§3.6).

### 3.6 Security envelope for UI-A (part of the G5 request)

1. Bind `127.0.0.1` only. A test proves every listening socket for the process is `0100007F` in `/proc/net/tcp`.
2. Per-launch 256-bit random capability in the URL path (`/s/<capability>/…`), from `RandomNumberGenerator`. A request without it gets 404 and no body.
3. `Host` must equal `127.0.0.1:<port>`, which blocks DNS rebinding. Every POST must carry `Origin: http://127.0.0.1:<port>` and `Content-Type: application/json`, with bodies of at most 16 KiB.
4. No CORS headers. No outbound network. No static file serving from disk. No directory listing.
5. All engine state stays behind the single `AnnualContinuation` lock, so concurrent requests are safe and nothing is cached outside the engine.
6. The process does not open a browser, register a service, write files, or persist anything.

## 4. Map presentation path — decision **B: minimal local layer**

### 4.1 Why not A (direct compile-link of Scenario One map files)

Full evidence is in the Phase I note §4. In summary, the exact transitive closure of the six requested pure files on `main` @ `7758e17c` is **eight** files (2,030 lines). All are Unity-free and all are in the global namespace.

1. **No grant:** V-01 covers only `SitkaGrowthModel.cs` and `SimulationRandom.cs`. Linking eight more Scenario One files needs a prospective #844 + product-owner read/compile grant, which this author cannot give.
2. **Rack domain in the closure:** `StandMapModel.cs`, `StandMapTransform.cs` and `StandMapSymbols.cs` depend on `RackPoint`/`RackGeometry` in `Assets/ForestPrototype/Racks/RackGeometry.cs`. That file carries the 439-line rack-planning domain (`RackPlanRack`, `RackOrderLifecycle`, `RackPlanOperationResult`, corridor geometry). Linking even the transform compiles rack planning into the Multi assembly, contrary to G0.
3. **No exact identity:** `MapTree` has no `TreeId`; `Seed` is derived from it. Exact selection would need a Scenario One edit (forbidden) or a fragile parallel index.
4. **State vocabulary mismatch:** `MapTreeMark {None, Fell, Crop}` and the rack-worded legend cannot express pending ≠ approved ≠ executed.
5. **Raster versus identity:** `StandMapCanopyRaster` produces an RGBA bitmap, which has no per-tree element for exact hit-testing or assistive technology. Its sprite-caching advantage targets ~1,300 trees; this stand has 100.

Not reasons: Unity coupling (none in the closure) and size (modest). Even with a grant, items 2–5 would still make B the better choice.

### 4.2 What B is

New, small, locally written code inside `Tools/MultiRackSitkaP1Lab` (`src/Core/PlayerMapScene.cs` plus the page's SVG renderer). **No Scenario One code is copied, linked or edited, and `StandMapSource.cs` is not reused.** It adopts these inspected *conventions* as design reference only:

- north-up; X east to the right; Z north up; map fraction measured from the west and north edges;
- every tree at its true position with its true crown radius; taller crowns drawn over shorter ones;
- visible dead distinction: a felled tree becomes a stump marker and its canopy gap opens; natural death is drawn as a muted standing-dead crown;
- property boundary as a dashed light line, a local metre grid (east/north from the south-west corner), scale bar and north arrow;
- a dark survey-sheet palette with restrained conifer green, chosen locally;
- a legend that names only what is present, with colour never the only cue.

**No pixel or visual-match claim is made**; no reference render was captured.

### 4.3 Coordinates (exact)

From the admitted fixture rule `p1-square30-corner-to-centred-xz-v1` (`DerivedManifest.SideMm = 30000`, parser-enforced trees in `[0, 30000)` mm): `AnnualTree.XM`, `ZM` ∈ [−15, 15) m. Map metres: `U = XM − (−15)` (east of the west edge), `V = 15 − ZM` (south of the north edge). The SVG `viewBox="0 0 30 30"` uses U/V directly, so 1 user unit = 1 m. Crown circle radius = `CrownRadiusM`. All arithmetic is f64 on the source f32 values; display only, never authority.

### 4.4 Draw items and status vocabulary

| `PlayerMapStatus` | Source rule | Drawn as (non-colour cue in brackets) |
|---|---|---|
| `Living` | `IsLiving`, not pending/approved | crown disc |
| `PendingFell` | living ID in `Pending` | crown + dashed ring [dashed; "P" in table] |
| `ApprovedFell` | living ID in `Approved.Orders` | crown + solid double ring [solid double; "A"] |
| `Felled` | `Death.Cause == "fell"` | stump marker, radius 0.35 m presentation constant [✕ glyph; "Felled Y<n>"] |
| `NaturalDead` | `Death` with any other cause | muted crown [hatched; "Dead (natural) Y<n>"] |

Paint order: all crowns (Living, PendingFell, ApprovedFell, NaturalDead) sorted by `HeightM` ascending, then `TreeId` ordinal; then stumps by `TreeId` ordinal. `PaintOrder` is the index, and the last item is on top.

## 5. Exact selection identity

- **S-1** Every drawn item carries the exact `TreeId` string from the current `CommittedFrame` (`data-tree-id`). Identity is never derived from a seed, index, colour or nearest centre.
- **S-2** On a pointer click, the page converts the point to map metres (U, V) using the SVG's own inverse screen transform and calls the read-only hit-test with its displayed fence (session, source token, plan-state revision).
- **S-3** The server runs `PlayerMapScene.HitTest` on the **current** view. It returns every item whose disc contains the point (inclusive), topmost first. If the fence is stale, it returns a stale refusal plus the fresh view; the page redraws and selects nothing.
- **S-4** No candidates means no selection; clicking bare ground never picks the nearest tree. One candidate selects it. Several candidates select the topmost and list the others as one-step alternatives ("3 crowns overlap here"), each showing exact ID, height and DBH.
- **S-5** Cross-check: the browser's own event target (topmost painted element) must equal `candidates[0]`. On mismatch the page selects nothing and announces "Selection ambiguous — choose from the list". This is a runtime witness that drawn and hit-tested identity agree.
- **S-6** Keyboard and screen-reader path: the tree table lists all 100 IDs (filterable by status, exact case-sensitive ID search). Selecting a row selects that exact ID. The map is not a keyboard trap.
- **S-7** The inspector always shows the selected exact ID in full. Commands send that exact string. The engine re-validates it at Add, Approve and Execute; display selection is never authority.
- **S-8** Any new view (after any command or refresh) re-resolves the selection by exact ID. If the tree is no longer living, the inspector shows its actual new state and offers no Add.

## 6. Player session facade contract — PROPOSED ABI for #730 (G4-class)

### 6.1 Public types (new files `src/Core/PlayerSessionView.cs`, `src/Core/PlayerWorkPlanAdapter.cs`, `src/Core/PlayerMapScene.cs`)

```csharp
public sealed record PlayerFence(string SessionId, WorkSourceToken Source, int PlanStateRevision);
public sealed record PlayerApprovedPlan(string PlanId, string ExecutionKey, WorkSourceToken Source, IReadOnlyList<WorkOrder> Orders);
public sealed record AnnualYearRecord(int YearBefore, int YearAfter, int AnnualRevisionAfter, int WorkRevision,
    AnnualStand Before, AnnualStand After, IReadOnlyList<AnnualDeath> NaturalDeaths, bool CancelledUnexecutedWork);
public sealed record PlayerMapExtent(double MinXM, double MinZM, double SideM, float AreaHa);
public sealed record PlayerSessionView(string SessionId, int PlanStateRevision, WorkSourceToken Source, CommittedFrame Frame,
    IReadOnlyList<WorkOrder> Pending, PlayerApprovedPlan? Approved, IReadOnlyList<AnnualYearRecord> AnnualHistory,
    PlayerMapExtent Extent);
public sealed record PlayerCommandResult(PlayerSessionView View, WorkExecutionResult? Execution, AnnualReport? Annual,
    string? RefusalCode, string? RefusalMessage);
public static class PlayerLabels { /* fixed truthful strings, §7.2 */ }
public sealed class PlayerWorkPlanAdapter { /* §6.4; no state of its own */ }
public enum PlayerMapStatus { Living, PendingFell, ApprovedFell, Felled, NaturalDead }
public sealed record PlayerMapItem(string TreeId, int PaintOrder, double U, double V, double RadiusM, float HeightM,
    float DbhCm, PlayerMapStatus Status, int? DeathYear, string? DeathCause);
public static class PlayerMapScene { /* Build(view), HitTest(scene, u, v) */ }
```

Exact member spelling may be refined during Phase II only if the semantics here are preserved, with any change listed in the Phase II handoff. `WorkContract.cs` and `AnnualEngine.cs` are **not modified**. Existing public types are reused unchanged.

### 6.2 Same-lock atomic read

New internal `AnnualContinuation.ReadPlayerView()` takes `sync` once and builds the whole view from the **same** `root` reference: the session ID, `root.PlanStateRevision`, `Token(root)`, the frame (identical construction to `Frame`), copies of `Pending`, the approved plan (with its minted IDs), `AnnualHistory` and the extent. Every exposed collection is a new read-only array wrapper. Reading never mutates.

### 6.3 Session identity and plan-state revision

- `SessionId`: 128-bit `RandomNumberGenerator` hex, created once per `AnnualContinuation`. It is not serialised into snapshots or reports.
- `WorkRoot` gains `int PlanStateRevision` (starts at 0) and `IReadOnlyList<AnnualYearRecord> AnnualHistory`. `WorkRoot` is internal and not part of any digest. `ContinuationSnapshot` and `AnnualReport` are unchanged.

| Transition (successful only) | `PlanStateRevision` | `WorkRevision` | Token changes? |
|---|---|---|---|
| Add pending / Remove pending | +1 | — | no |
| Approve / Cancel approved | +1 | — | no |
| Execute (new key) | **+1** (Q-2) | +1 | yes (work revision) |
| Execute replay (same key, verified) | — | — | no |
| Advance with consented cancellation | +1 | — | yes (year, annual revision) |
| Advance with no unexecuted work | — | — | yes (year, annual revision) |
| Any refusal or fault | — | — | no |

These increments live inside the existing mutation methods (`AddPendingFell`, `RemovePendingFell`, `ApprovePlan`, `CancelApprovedPlan`, `Execute`, `AdvanceLocked`), at their single `root = …` publish. A legacy unfenced call therefore also invalidates fenced callers. Validation order and refusal codes of those methods are unchanged.

### 6.4 Fenced commands (internal on `AnnualContinuation`; mirrored publicly by the adapter)

Every command takes `sync` once, checks the fence **before** any mutation, performs the transition, then builds the resulting view **under the same lock**:

1. `SessionId` mismatch → `PlayerSessionMismatch`.
2. `Source != Token(root)` (full record equality: domain, fixture, file SHA, content hash, scope, year, annual revision, work revision, model lineage, threshold) → `PlayerSourceStale`.
3. `PlanStateRevision != root.PlanStateRevision` → `PlayerPlanStateStale`.

| Command | After the fence | Notes |
|---|---|---|
| `AddPending(fence, treeId, reason)` | legacy `AddPendingFell` (reentrant `sync`) | exact live ID, required reason |
| `RemovePending(fence, treeId)` | legacy `RemovePendingFell` | |
| `Approve(fence)` | mint `PlanId = "{SessionId}:plan:{n}"`, where n = plan-state revision after approval; legacy `ApprovePlan(PlanId)` | **callers cannot supply a PlanId**; zero Fell |
| `CancelApproved(fence)` | legacy `CancelApprovedPlan` | approved orders are not returned to pending; the player re-adds |
| `Execute(fence, planId, executionKey)` | see §6.5 | the engine resolves the approved plan itself; no caller plan object |
| `Advance(fence, consentToCancelWork)` | legacy 4-argument `Advance(year, revision, workRevision, consent)` using the fence token's clocks | the legacy 3-argument advance stays fail-closed after a Fell (A-1) |
| `HitTest(fence, u, v)` (read-only) | `PlayerMapScene.HitTest` on the current view | |

`ExecutionKey` shown in `PlayerApprovedPlan` is minted as `"{PlanId}:execute"`. The page always echoes the minted key, so a retry of the same intent reuses it and never invents a new key.

### 6.5 Execute and the A-2 replay rule

Order under one lock:

1. Session check.
2. **Replay path:** if `executionKey` is already executed, return `Replayed = true` with zero mutation **only if** the recorded `Event.PlanId == planId`, the recorded source equals `fence.Source` (the original pre-execution token), and the session matches. Otherwise `WorkExecutionKeyConflict`. This runs before the staleness check on purpose: a genuine retry after an unconfirmed success must get its receipt, not a second Fell.
3. Source and plan-state fence (§6.4).
4. An approved plan must exist with `PlanId == planId`, otherwise `WorkPlanNotApproved`.
5. Legacy `Execute(approved, executionKey)` with its whole-batch validation, D-093 all-live refusal, single-root publish and no seam exposed.

PlanIds are unique within a session (monotonic plan-state revision), so PlanId plus session identifies exactly one immutable order payload. The legacy `Execute` replay comparison is left byte-identical; the PlanId guard is added only on the fenced path, as the Manager directed.

### 6.6 Refusal codes

Existing codes are reused unchanged (`UnknownTreeId`, `WorkReasonRequired`, `WorkDuplicateTreeId`, `WorkPlanAlreadyApproved`, `UnknownWorkOrder`, `WorkEmptyPlan`, `WorkPlanNotApproved`, `WorkExecutionKeyConflict`, `WorkTreeNotLive`, `WorkWouldLeaveNoLivingStems`, `WorkPendingConsentRequired`, `ContinuationYearMismatch`, `ContinuationRevisionMismatch`, `ContinuationWorkRevisionMismatch`, `ContinuationNoLivingStems`). New: `PlayerSessionMismatch`, `PlayerSourceStale`, `PlayerPlanStateStale`. The adapter converts `P1Refusal` into `PlayerCommandResult.RefusalCode/Message` plus a freshly read current view. Non-refusal exceptions propagate (fail closed) and the page shows "outcome unconfirmed — re-read".

### 6.7 Not exposed

The `beforePublish` and `beforeCommit` test seams; the legacy 3-argument `Advance`; any way to pass a caller-built `WorkPlan`; any proposal, sample or comparator member; anything returning mutable engine collections.

## 7. Minimum human-usable UI

### 7.1 Regions (map-first; one column at 360 CSS px, two or three at desktop width)

1. **Status bar (S0):** the title "Synthetic 30 m Sitka experiment" and the full disclosure text; domain and fixture ID; fixture SHA-256 (short form plus an expandable full value); RNG v1, growth model v2, algorithm `p1-f0-adult-sitka-30m-v1`; ecological year, annual revision, work revision, plan-state revision and session ID (short form, expandable); live count, stems/ha, basal area m²/ha and Dq, **all from `Frame.Stand`**; "One undivided management area — MS-P30-UNDIVIDED-30M-V1 (not a rack)"; "Comparison extension: DISABLED — Manual/NotCovered"; "Costs, revenue, budget: UNAVAILABLE (not €0)"; "Session only — nothing is saved".
2. **Map:** SVG per §4, with legend, scale bar, north arrow and grid. Overlap alternatives appear beside it (§5 S-4).
3. **Tree table:** 100 rows with ID, status text, DBH, height, crown radius and X/Z; exact-ID search and status filter.
4. **Inspector (S1):** exact ID; status; X/Z; age, DBH, height and crown radius; death year and cause when dead; "170 mm is fixture metadata only; it does not restrict Fell (D-093)"; required reason field; **Add to Work Plan — no tree is felled**.
5. **Work Plan (S2–S4):** the pending list with reasons and Remove; **Approve whole Work Plan — 0 trees felled**; the approved plan with its PlanId and **Cancel approved plan**; **Execute approved plan**, with a confirmation dialog stating "fells exactly n named trees in Year y, or refuses all"; a client-side pre-warning when the batch would leave no living tree (the engine still refuses authoritatively).
6. **Result and year (S5–S7):** the executed receipt (key, PlanId, work revision w→w+1, felled IDs with cause, before/after count and BA from the `WorkEvent`, "Already executed — no additional Fell" on replay); **Advance one ecological year**, with the consent dialog "Keep orders — stay in Year y" / "Cancel unexecuted work and advance one year"; and the history of `AnnualYearRecord`s and work events (previous actual versus current actual).
7. **Alerts:** `role="alert"` for refusals (what was refused, code, "nothing changed", current year, one recovery action); `role="status"` for committed transitions; and an "Outcome unconfirmed — re-read current state" state after any transport failure.

### 7.2 Label rules

All labels come from `PlayerLabels` or the view, never invented client-side. Money is never displayed as a number. No rack, corridor, entrance, marking, proposal, sample or "equivalent area" control or text appears anywhere.

## 8. Accessibility (testable requirements)

- **A-1:** Every action in §7 can be reached and operated by keyboard in logical order, with visible focus and Enter/Space activation. Escape closes dialogs. There are no focus traps.
- **A-2:** The table is the authoritative screen-reader path. Each row exposes exact ID, status text and measurements with units. The map has an accessible name and summary and is skipped by Tab.
- **A-3:** Status never relies on colour alone (§4.4 patterns, glyphs and text). Text contrast is at least 4.5:1.
- **A-4:** At 360 CSS px width and at 200 % zoom: single column, no horizontal scrolling of core actions, long hashes wrap or collapse, targets are at least 44 × 44 CSS px.
- **A-5:** Refusals go to the alert region and move focus there (or to the failing field). Committed transitions are announced politely. Approve and Execute are separate, focus-distinct operations and never a double-click shortcut.
- **A-6:** Touch: no hover-only information and no gesture that approves, executes or bulk-selects.
- **A-7:** A real screen-reader PASS is **not** claimed unless NVDA or another reader is actually run; otherwise it is reported NOT RUN.

## 9. Acceptance criteria for this delta (all NOT RUN)

- **AC-PM-01** The base 66 tests pass unchanged and the ten no-work digests are character-identical on the candidate. Fixture, goldens, `.p0ref` pin, `AnnualRules.cs`, `WorkContract.cs`, `AnnualEngine.cs`, CLI, `.sln` and both csproj files are unchanged.
- **AC-PM-02** The view is atomic: under concurrent Execute and Advance, every observed view satisfies `Frame.WorkRevision == Source.WorkRevision`, `Frame.EcologicalYear == Source.EcologicalYear`, events and felled trees agree, and pending/approved belong to the same root.
- **AC-PM-03** The plan-state rules in §6.3 hold exactly, including Add/Remove ABA staleness and Execute increment.
- **AC-PM-04** All six commands refuse wrong-session, stale-source and stale-plan-state fences with zero mutation of the full view.
- **AC-PM-05** Add, Remove, Approve and Cancel make zero Fell and leave the token unchanged. Execute fells exactly the approved IDs once with cause `fell/manual-work-plan`; year and annual revision are unchanged and work revision is +1.
- **AC-PM-06** Replay works: same key, PlanId, source and session gives `Replayed = true` and zero mutation. A different PlanId under the same key gives `WorkExecutionKeyConflict` with zero mutation. A different session gives `PlayerSessionMismatch`.
- **AC-PM-07** An all-live batch is refused (`WorkWouldLeaveNoLivingStems`) with zero mutation; injected-fault rollback leaves zero mutation, including plan-state.
- **AC-PM-08** Advance with unexecuted work and no consent is refused with zero mutation. With consent, the orders are cancelled at zero Fell and one genuine year runs. After a Fell, Year+1 survivor biology equals `AnnualRules.Calculate` on the post-Fell state (same function, no second engine), and the felled tree stays dead with its Fell cause.
- **AC-PM-09** The scene has one item per frame tree with exact IDs, the §4.3 geometry and the §4.4 paint order. Hit-test returns candidates topmost-first, inclusive at the boundary, and empty on bare ground. A stale fence is refused.
- **AC-PM-10** Only the §6 public members exist. There is no proposal, comparator, sample, seam or plan-object entry point.
- **AC-PM-11** (UI-A only) The process binds only 127.0.0.1 on a non-5187 ephemeral port. Capability, Host, Origin, content-type and size guards refuse correctly. No file is created in the lab tree or `~/.aspnet`.
- **AC-PM-12** A real human-operable run is witnessed: select by map click and by table, overlap alternatives, add with reason, approve (0 Fell), execute (one event, canopy gap and count/BA change), refusal cases, consent dialog, Year+1. It is captured as real browser screenshots or recording with provenance at 1280 px, 360 px and 200 %, with a keyboard-only pass. Screen reader is reported RUN or NOT RUN truthfully.
- **AC-PM-13** Independent GPT-6.1 Sol Tier-2 review of the exact pushed candidate, on a fresh clone, returns APPROVED with no CRITICAL or MAJOR findings.

## 10. Gates and stop conditions

- **G5 (owner):** UI-A versus UI-B (§3). **No `src/Ui/**` file is written before this decision.**
- **#730 Manager acceptance** of this delta (including Q-1…Q-6 and the §6 ABI) **before any production write**, including the Core facade.
- **No new Scenario One grant is requested.** Option B needs none. If the owner nevertheless wants A, the request must name all eight files and blobs (Phase I note §4.3), and items 2–5 of §4.1 must be resolved first.
- **STOP** if implementation would need changes to `WorkContract.cs`, `AnnualEngine.cs`, CLI, `.sln`, csproj files other than the new UI project, `AnnualRules.cs`, fixtures, goldens or P0. Stop also if any digest or base test would change, or if a G1, economy, persistence or rack element is requested.
