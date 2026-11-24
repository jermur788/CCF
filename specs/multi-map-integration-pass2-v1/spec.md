# MULTI Map Integration Pass 2 — Interactive Fenced Work Plan Actions Specification

- **Spec ID:** SPEC-multi-map-integration-pass2-V1
- **Status:** IMPLEMENTING
- **Owner:** Project owner (product decisions); Primary Implementer custody under D-091
- **Created:** 2026-11-24
- **Last updated:** 2026-11-24
- **Decision log references:** D-020 (presentation authority), D-030 (state honesty), D-061 (mixed density weights), D-091 (single-writer lane custody), D-095 (Stage 4 adult/mortality/prune)
- **Milestone:** MultiRack Sitka 2D standalone simulator (`Tools/MultiRackSitkaP1Lab`) — Pass 2 Map Integration
- **Related evidence:** Base commit `1f13182659769f19e7c91159dd7ad475667e0d62` (independently reviewed and owner-accepted Pass 1)
- **Supersedes / superseded by:** Builds directly on `SPEC-multi-map-integration-pass1-V1` (adds interactive fenced Work Plan actions; does not alter Pass 1 specs or Core biology)

## 1. Product intent

Pass 1 brought `/multi` onto the closed Phase 2 engine (`1/1/1/1/1`) as a read-only ecological display with a pure inline SVG visual system, additive compact `ecology` DTO, `--multi-experiment=ma-1|ma-1-mixed`, and read-only `POST api/multi/cell`. However, players could not yet queue or remove `ClearUndergrowth`, `Plant`, `Shelter`, or `SpotClear` orders from `/multi`.

Pass 2 exposes the biological Work Plan verbs that already exist in `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs` through **six fenced write endpoints** and **interactive map modes**, strictly governed by the single `Pending → Approve → Execute` commitment gate, with **zero edits to Core biology** (`src/Core/MultiArea/**`).

## 2. Goals

- Expose all six existing fenced Core Work Plan operations (`AddClearUndergrowth`, `RemoveClearUndergrowth`, `AddPlant`, `AddShelter`, `AddSpotClear`, `RemovePlanting`) over HTTP under `PlayerApi`'s unchanged security guards (`16 KiB` cap, strict camelCase JSON, unknown/duplicate/null member rejection, `400`/`409`/`500` semantics).
- Enforce the single commitment gate across all six order families (`Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`): queueing and approval produce zero ecology mutation; only `Execute` commits atomically with zero calendar advance.
- Provide four explicit map interaction modes (`Select Tree`, `Inspect Cell`, `Plant`, `Spot Clear`) with provisional local `Draft` previews (snap OFF, zero engine order on click), inspector body swapping (never stacking), and deterministic `Escape` precedence preserving draft Fell competitor ticks.
- Implement the sequential fence-chained planting queue (`plant/add` → `shelter/add` → `spot-clear/add`) with immediate stop and honest partial disclosure (plus one-click remove of earlier queued orders) if step 2 or step 3 returns `409`.
- Extend the Work Plan panel, Execute confirmation dialog, and Year+1 consent dialog to cover every order type with targeted remove buttons before `Approve`, while preserving Pass 1's pure inline SVG grammar, strict circular crowns, R3 5-row fold geometry at `1280×720`, and N-6 stump `X` marker.
- Surface every Core refusal code/message verbatim with zero client-side forestry maths (D-020).
- Restore Pass 1 non-blocking smoke assertions (`N-3` and `N-4`) where clean and verify live post-Execute rendering of a new `PJ` sapling, an active 8-year shelter, and a `0.564 m` spot-clear ring.

## 3. Non-goals and strict scope boundaries

- **Zero edits to `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/**`**: biology, refusal rules, constants, RNG streams, ordering, and `MultiAreaWorkPlanAdapter` remain untouched.
- **Zero edits to forbidden paths**: `Assets/**`, `Packages/**`, `ProjectSettings/**`, Unity saves, economy/cash/prices, frozen Reference Future v1, `Tools/MultiRackSitkaLab/**`, `specs/multi-map-integration-pass1-v1/**`, `.p0ref/**`, `main`, or generated directories (`bin/`, `obj/`, `Library/`, `Temp/`, `Logs/`).
- **Zero CSP relaxation or external dependencies**: CSP stays `default-src 'none'; script-src 'self'; style-src 'self'; img-src data:; connect-src 'self'; base-uri 'none'; form-action 'none'; frame-ancestors 'none'`. Zero `data:image/webp`, zero inline JS/CSS, zero external URLs or packages.
- **Zero client-side forestry maths (D-020)**: browser JS never predicts spacing (`0.5 m` / `0.7 m`), shelter match radius (`0.25 m`), polygon interiority, cell existence, light, or pruning eligibility.
- **No `Expired Shelter` Work Plan button**: `Expired Shelter` stays strictly a map/legend state.
- **Keep N-6 stump `X` as-is**: felled stump `X` marker and legend line remain unchanged pending owner decision.

## 4. Functional contract

### 4.1 Six fenced write endpoints (`MI-P2-01`)

All six routes register under `/s/<capability>/` inside `PlayerApi`, passing the unchanged `HandleAsync` guards in order: `Host`, 256-bit `capability`, `route + method` (`POST` only), `Origin`, `Content-Type: application/json`, `16 KiB` (`16,384` B) body cap, unique-member check, required non-null member check, bounded text check (`1..64` non-whitespace/non-control characters for `areaId`, `speciesId`, `orderId`), and `JsonSerializer` with `JsonNamingPolicy.CamelCase` + `UnmappedMemberHandling.Disallow`.

Every request requires `fence: { sessionId, source, planStateRevision }` with all 12 scalar `source` members non-null (`domain`, `experimentId`, `manifestContentHash`, `areaIds`, `ecologicalYear`, `annualRevision`, `workRevision`, `rngModelVersion`, `growthModelVersion`, `p1AlgorithmVersion`, `areaModelVersion`, `actionEligibleDbhThresholdMm`).

| Route | Required camelCase body members (besides `fence`) | Exact types | Core `MultiAreaContinuation` call |
|---|---|---|---|
| `POST api/multi/clear/add` | `areaId`, `column`, `row` | `string`, `int`, `int` | `AddClearUndergrowth(fence, areaId, column, row)` |
| `POST api/multi/clear/remove` | `areaId`, `column`, `row` | `string`, `int`, `int` | `RemoveClearUndergrowth(fence, areaId, column, row)` |
| `POST api/multi/plant/add` | `areaId`, `speciesId`, `xM`, `zM` | `string`, `string`, JSON number (`double` → `float`), JSON number (`double` → `float`) | `AddPlant(fence, areaId, speciesId, (float)xM, (float)zM)` |
| `POST api/multi/shelter/add` | `areaId`, `xM`, `zM` | `string`, JSON number (`double` → `float`), JSON number (`double` → `float`) | `AddShelter(fence, areaId, (float)xM, (float)zM)` |
| `POST api/multi/spot-clear/add` | `areaId`, `xM`, `zM` | `string`, JSON number (`double` → `float`), JSON number (`double` → `float`) | `AddSpotClear(fence, areaId, (float)xM, (float)zM)` |
| `POST api/multi/planting/remove` | `orderId` | `string` | `RemovePlanting(fence, orderId)` |

**Coordinate transport note:** `xM` and `zM` use exact camelCase names (`"xM"`, `"zM"`, matching the API contract, via `[JsonPropertyName("xM")]` / `[JsonPropertyName("zM")]`). They deserialize as JSON numbers (`double?`) and narrow to `float` when calling Core so that valid JSON numbers overflowing float32 (such as `1e40`) reach Core's `!float.IsFinite` check and return `409 Phase2CoordinatesInvalid`, while non-numeric/null/missing/string/over-double inputs are rejected at the HTTP boundary as `400 BadRequest`.

**Response contract (`MultiCommandPayload`):**
`{ view, scenes, refusal, execution, annual, stage4, ecology }`, where `(stage4, ecology)` are obtained atomically from `CompositeFor(result.View)` (returning `null` together on concurrent move; zero torn frames).

**HTTP status semantics:**
- `200 OK`: order queued or removed; `refusal: null`; updated `view`, `scenes`, `stage4`, `ecology` with incremented `planStateRevision` and unchanged `source.ecologicalYear` / `source.workRevision` / `source.annualRevision`.
- `400 Bad Request`: malformed JSON, wrong JSON type (e.g., string or fractional number for `column`/`row`, string for `xM`/`zM`), missing/null/duplicate/unknown/wrong-case members, empty/whitespace/oversized IDs (`> 64` chars). Does not reach Core; zero state change.
- `403 Forbidden`: wrong `Host` or wrong/missing `Origin`.
- `404 Not Found`: wrong capability.
- `405 Method Not Allowed`: non-`POST` method.
- `413 Payload Too Large`: body `> 16,384` bytes.
- `415 Unsupported Media Type`: non-JSON `Content-Type`.
- `409 Conflict`: typed Core `P1Refusal` with exact `{ refusal: { code, message } }` AND a fresh `view`, `scenes`, `stage4`, `ecology`; zero state change.
- `500 Internal Server Error`: `{ error: { code: "OutcomeUnconfirmed", message } }`; client locks all mutating commands until a fresh `GET api/multi/view` re-read succeeds.

### 4.2 Single commitment gate (`MI-P2-02`)

- **Queueing (`clear/add`, `plant/add`, `shelter/add`, `spot-clear/add`, `pending/add`, `prune/add`)**: creates `Pending` plan entries only; bumps `planStateRevision` by 1; **zero ecology mutation** (no change to `ecologicalYear`, `annualRevision`, `workRevision`, living stems, crowns, bramble/bracken cover, natural bands, `planted[]`, `shelters[]`, or `patches[]`).
- **Removing (`clear/remove`, `planting/remove`, `pending/remove`, `prune/remove`)**: removes one `Pending` entry before approval; bumps `planStateRevision` by 1; **zero ecology mutation**.
- **Approving (`api/multi/plan/approve`)**: promotes all pending orders across every order type (`Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`) to `Approved` under one engine-minted `PlanId` and `ExecutionKey`; **zero ecology mutation**.
- **Cancelling (`api/multi/plan/cancel`)**: discards the approved plan; **zero ecology mutation**.
- **Executing (`api/multi/plan/execute`)**: commits the entire approved mixed batch atomically in Core's fixed resolve order (`SpotClear → Plant → Shelter`, plus `ClearUndergrowth`, `Fell`, `Prune`) with all-or-nothing revalidation, incrementing `workRevision` by 1 and advancing `ecologicalYear` by **0**.
- **No single-click mutation**: clicking the map in any mode never mutates ecology and never queues an order without an explicit drawer action button click.

### 4.3 Four explicit map modes & interaction arbitration (`MI-P2-03`)

The workspace mode group exposes four mutually exclusive modes, each swapping the right-hand inspector body cleanly (never stacking panels):

1. **`Select Tree` (default):**
   - Preserves Stage 4 "crown wins" selection, 5 competitor rows + `Add` above the fold at `1280×720` (including loaded blocked-competitor rows) and `1920×1080`, and engine-authoritative species-aware Prune readouts (Oak/Beech max 2 lifts at `3.5 / 6.0 m`, 3-year wait; Sitka max 3 lifts at `2.5 / 5.0 / 6.5 m`, 5-year wait).
   - **Living `PJ` sapling inspection:** clicking an active `PJ` sapling marker on the map, selecting it from the planted list in the `Trees` tab, or entering its exact `juvenileId` (e.g., `PJ0001`) in `#quick-id` inspects the sapling in `#juvenile-panel` (swapped inside `#pane-tree`, leaving any adult reference tree and its draft Fell ticks intact). When the inspected `PJ` sapling is active (`isActive === true`) and has no active shelter (`!hasEffectiveShelter`):
     - If no `Shelter` order is pending/approved for its point, show **`Queue 8-year Shelter`** (`POST api/multi/shelter/add` with `{ fence, areaId, xM: j.xm, zM: j.zm }`).
     - If a `Shelter` order is already `Pending` for its point, show a Pending pill and **`Remove pending shelter`** (`POST api/multi/planting/remove` with `orderId`).
     - If a `Shelter` order is `Approved`, show an Approved notice.
     - When an active shelter already protects the `PJ` sapling, display the active shelter ID and expiry year with no `Queue 8-year Shelter` button.

2. **`Inspect Cell`:**
   - Swaps the right-hand inspector to `#cell-panel` (hiding `#tree-panel` and all other drawers).
   - Clicking an in-polygon `2.5 m` cell fetches read-only cell detail via `POST api/multi/cell` (zero mutation).
   - For a valid inspected cell:
     - When no `ClearUndergrowth` order is queued for `(areaId, column, row)`: show **`Clear undergrowth (spares seedlings)`** (`POST api/multi/clear/add`).
     - When a `ClearUndergrowth` order is `Pending` for `(areaId, column, row)`: show **`Remove pending clear`** (`POST api/multi/clear/remove`).
     - When `Approved`: show Approved state note (`Whole 2.5 m cell · spares seedlings`).

3. **`Plant`:**
   - Swaps the right-hand inspector to `#plant-panel` (Planting Drawer).
   - Clicking the map places or repositions a **provisional `Draft` local preview only** (dashed slate outline, snap OFF, zero engine order) at the exact world `(X, Z)` (`xM = extent.minXM + u`, `zM = extent.minZM + extent.sideM - v`) without client-side validity filtering.
   - Planting Drawer controls:
     - Species choice: `Sessile Oak` (`sessile-oak`, default) | `European Beech` (`beech`)
     - Checkboxes: `[ ] Add 8-year shelter` and `[ ] Spot-clear ~1 m²` (ticking the latter previews the world-scale `0.564 m` dashed slate ring around the provisional point on the map)
     - Primary button: **`Queue to Work Plan`**
   - **Sequential fence-chained queueing rule:** pressing `Queue to Work Plan` executes sequentially, refreshing the fence from each `200 OK` response before sending the next step, in exact order:
     1. `POST api/multi/plant/add` (`areaId`, `speciesId`, `xM`, `zM`)
     2. If `[x] Add 8-year shelter` is ticked: `POST api/multi/shelter/add` (`areaId`, `xM`, `zM`)
     3. If `[x] Spot-clear ~1 m²` is ticked: `POST api/multi/spot-clear/add` (`areaId`, `xM`, `zM`)
   - **Partial-failure / 409 disclosure rule:** if step 2 or step 3 returns `409` (or a `4xx` guard error), the sequence **stops immediately**, updates the view/ecology from the `409` payload, displays the exact Core refusal code and message, and explicitly lists every earlier pending order that was queued (with its exact `orderId`, kind, and a one-click **`Remove`** button calling `POST api/multi/planting/remove`) plus any unattempted steps. On `500` / transport loss, the sequence stops immediately and engages the fail-closed command lock until re-read.

4. **`Spot Clear`:**
   - Swaps the right-hand inspector to `#spot-panel` (Spot Clear Drawer).
   - Clicking the map places or repositions a provisional **seedling-free** `0.564 m` circular dashed slate preview at the exact world `(X, Z)` (snap OFF, zero engine order).
   - Primary button: **`Queue Spot Clear (~1 m²)`** (`POST api/multi/spot-clear/add` with `{ fence, areaId, xM, zM }`).

5. **`Escape` precedence:**
   - Exact order: (1) open modal `<dialog>` closes itself (leaving any in-flight hit-test alone, F-05) → (2) non-empty `<input type="search">` clears itself → (3) exit `Plant` / `Spot Clear` / `Inspect Cell` provisional preview and mode back to `Select Tree` (focusing `#mode-tree`) → (4) close `PJ` sapling inspection if active (returning to adult `#tree-panel`) → (5) clear adult tree selection (`closeInspection`).
   - Exiting `Plant`, `Spot Clear`, `Inspect Cell`, or `PJ` inspection via `Escape` **never discards pending draft Fell competitor ticks**, never changes the ground overlay, and **never mutates the Work Plan**.

### 4.4 Work Plan panel, dialogs, and map rendering (`MI-P2-04`)

- **Work Plan panel (`#pane-plan`):**
  - Lists every pending and approved order across both areas for all six order types: `Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`.
  - Summary card (`#plan-summary`), Work Plan tab count badge, and Area A / Area B dock badges count all six order types truthfully.
  - Before `Approve`, every pending row provides a targeted remove button (`pending/remove`, `prune/remove`, `clear/remove`, `planting/remove`). After `Approve`, editing is locked until `Cancel approved plan`.
  - `Expired Shelter` never appears as a Work Plan action or button.
- **Execute confirmation dialog & Year+1 consent dialog:**
  - `confirmationOf(view)` and `confirmationStillShown` freeze and verify all approved `Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, and `SpotClear` orders.
  - Execute confirmation dialog lists every approved order per area across all six types and focuses `Back` by default.
  - Year+1 consent dialog triggers whenever any pending or approved order of any type exists and lists all unexecuted orders per area.
- **Map SVG grammar (Pass 1 preserved + Pass 2 pending/approved/draft previews):**
  - Provisional `Draft` preview (`Plant` / `Spot Clear`): dashed slate outline (`#9fb4c8`) in `#draft-preview` layer; zero seedling inside Spot Clear preview.
  - `Pending` orders: dashed gold (`#edc774`) + `◷` pill/marker (`ClearUndergrowth` whole `2.5 m` cell polygon; `SpotClear` world-scale `0.564 m` circle in `#spot-patches` beneath `#planted-layer`; `Plant` / `Shelter` markers with action icon).
  - `Approved` orders: solid pale-blue (`#88c5e8`) + `✓` pill/marker.
  - `Executed` orders: all Pending/Approved badges removed from active map layers; completed execution shown in `Latest receipt` / `Results` under `Completed · History Log`; stump `X` retained as-is (N-6).
  - Post-Execute live rendering: new `PJ` sapling (`#ccf-sapling-oak` / `#ccf-sapling-beech`), active 8-year shelter (`#ccf-shelter-active`), and exact world-scale `0.564 m` spot-clear patch (`#ccf-spot-clear` in `#spot-patches` layer beneath `#planted-layer`).

### 4.5 Exact Core refusal surfacing (`MI-P2-01`, `MI-P2-04`)

Browser JS performs zero client-side validation of ecological/spatial rules and always sends requests to Core and surfaces the exact Core refusal code and message for:
`Phase2CoordinatesInvalid`, `Phase2PointOutsideArea`, `Phase2PointWithoutGroundCell`, `Phase2DuplicateOrder`, `Phase2SpeciesUnknown`, `PlantSpacingTooClose`, `PlantTooCloseToAdult`, `ShelterNoTarget`, `ShelterAlreadyPresent`, `ClearCellNotInArea`, `ClearDuplicateCell`, `UnknownWorkOrder`, `WorkPlanAlreadyApproved`, `PlayerSourceStale`, `PlayerPlanStateStale`, `PlayerSessionMismatch`.

## 5. Frozen anchors (all nine must match exactly)

| # | Anchor | Expected SHA-256 |
|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` |
| 3 | Multi-area `0/0/1/0` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` |
| 7 | Phase 2 Q6 (`1/1/1/1/1` vs `1/1/1/1/0`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` |
| 8 | P0 Core binary pin (`.p0ref/SitkaLab.Core.dll`) | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` |
| 9 | MA-1-MIXED 30-y fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` |

## 6. Hard payload budgets

Measured in raw UTF-8 HTTP response bytes on both `ma-1` and `ma-1-mixed`, at Year 0 and Year +5, **both with Pass 2 orders pending and after Execute**:
- `GET api/multi/view` total body: **≤ 524,288 bytes (512 KiB)**.
- Top-level `"ecology"` raw UTF-8 JSON value span: **≤ 153,600 bytes (150 KiB)**.

## 7. Evidence classification, security, and persistence

- **Persistence:** Session-only in memory. Nothing is written to disk.
- **Ecological rules:** All planting spacing (`0.5 m` / `0.7 m`), 8-year shelter life (`0.25 m` match), `0.564 m` spot clearance, selective undergrowth clearing, and broadleaf pruning remain engine-owned `Gameplay Calibration / Transfer / Simulation Abstraction / New Owner Rule` per `Phase2Evidence`. Planting is not assumed automatically beneficial.
- **Security:** All six new endpoints pass `PlayerApi.HandleAsync` guards (`Host`, capability, method, `Origin`, `Content-Type`, `16 KiB` cap, strict camelCase, no duplicate/unknown/null keys, no CORS, strict CSP). No capability URL is ever stored in Git.

## 8. Acceptance criteria (`MI-P2-01`..`MI-P2-06`, `MI-REG`, `MI-P2-REVIEW`)

- **MI-P2-01:** Six fenced write endpoints implemented behind unchanged guards, strict camelCase, 16 KiB cap, exact `200`/`400`/`409`/`500` semantics with fresh composite `view + stage4 + ecology` on `409`, and all required refusal codes surfaced verbatim.
- **MI-P2-02:** Single commitment gate verified: queueing and approving produce zero ecology mutation; Execute commits atomically (`SpotClear → Plant → Shelter` + `ClearUndergrowth`, `Fell`, `Prune`) with zero calendar advance; no map click mutates state.
- **MI-P2-03:** Four explicit map modes (`Select Tree`, `Inspect Cell`, `Plant`, `Spot Clear`), provisional `Draft` previews with snap OFF, sequential fence-chained `Plant → Shelter → SpotClear` queue with immediate stop & partial disclosure + one-click remove on step 2/3 `409`, `PJ` sapling `Queue 8-year Shelter`, `Inspect Cell` clear/remove buttons, and `Escape` order preserving draft Fell ticks.
- **MI-P2-04:** Work Plan panel, Execute confirmation, and Year+1 consent cover all six order types with targeted remove buttons before `Approve`; Pass 1 SVG grammar preserved (stump `X` kept per N-6; zero `Expired Shelter` buttons); post-Execute live map renders new `PJ` sapling, active 8-year shelter, and world-scale `0.564 m` spot-clear patch beneath `PJ` saplings.
- **MI-P2-05:** Real loopback + real headless Edge browser matrix at `1920×1080` and `1280×720` on both `ma-1` and `ma-1-mixed`: full `Plant + Shelter + SpotClear + ClearUndergrowth + broadleaf Prune → Approve → Execute → Year +1` lifecycle, commitment gate, live post-Execute markers, `409` refusal surfacing + partial disclosure, modes, `Escape`, R3 5-row fold at `1280×720`, zero horizontal scroll, zero console/CSP/5xx, zero WebP, circular crowns, payload budgets met.
- **MI-P2-06:** Clean builds (`0/0`) for `.sln` and `SitkaP1.Ui.csproj`, Core suite (`≥ 251` baseline + new Pass 2 tests, 0 failures, identical across two runs), `ui-smoke-multiarea.sh` updated & passing with restored Pass 1 `N-3`/`N-4` assertions, servers stopped, protected listeners alive, clean `git status --short`, durable handoff committed and branch pushed.
- **MI-REG:** All 9 frozen anchors unchanged; zero edits to `src/Core/MultiArea/**` or any forbidden scope.
- **MI-P2-REVIEW:** Independent reviewer verification and checklist sign-off (every row remains `[ ]` until independent review).

## 9. Stop conditions

Stop immediately and report if:
1. `.p0ref/SitkaLab.Core.dll` is missing or fails sha256 pin `9a99f781…34b3`.
2. Any of the 9 frozen anchors moves.
3. A Core edit in `src/Core/MultiArea/**` appears unavoidable.
4. Payload budgets (`≤ 524,288 B` view or `≤ 153,600 B` `ecology`) are exceeded with Pass 2 orders pending or executed.
5. The real loopback server or real headless Edge cannot be driven.
