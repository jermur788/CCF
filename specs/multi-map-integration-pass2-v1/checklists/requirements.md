# MULTI Map Integration Pass 2 — Requirements / Acceptance Checklist

- **Spec ID:** SPEC-multi-map-integration-pass2-V1
- **Spec status:** IMPLEMENTING
- **Checklist owner:** Independent Reviewer / Overall Manager (every box starts unchecked `[ ]`; only an independent reviewer or Manager ticks rows after inspecting evidence)
- **Implementation under test:** `feat/multi-map-integration-pass2-20261124-v1` (base `1f13182659769f19e7c91159dd7ad475667e0d62`)
- **Date:** 2026-11-24

> **Rule:** Every row below starts unchecked `[ ]`. The Primary Implementer records observed numbers in the durable handoff (`Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass2/MULTI-MAP-INTEGRATION-PASS2-NEON-IMPLEMENTATION-20261124.md`) and leaves all checkboxes unticked for independent verification.

## Acceptance rows

- [ ] **MI-P2-01 — Six fenced write endpoints, strict guards & exact Core refusal surfacing:**
  - All six endpoints registered under `/s/<capability>/` behind unchanged `PlayerApi.HandleAsync` guards (`Host`, capability, method, `Origin`, `Content-Type: application/json`, `16 KiB` cap, strict camelCase, unknown/duplicate/null member rejection):
    - `POST api/multi/clear/add` (`areaId`, `column`, `row` → `AddClearUndergrowth`)
    - `POST api/multi/clear/remove` (`areaId`, `column`, `row` → `RemoveClearUndergrowth`)
    - `POST api/multi/plant/add` (`areaId`, `speciesId`, `xM`, `zM` → `AddPlant`)
    - `POST api/multi/shelter/add` (`areaId`, `xM`, `zM` → `AddShelter`)
    - `POST api/multi/spot-clear/add` (`areaId`, `xM`, `zM` → `AddSpotClear`)
    - `POST api/multi/planting/remove` (`orderId` → `RemovePlanting`)
  - Every endpoint enforces `fence: { sessionId, source, planStateRevision }` and returns `MultiCommandPayload` (`view`, `scenes`, `refusal`, `execution`, `annual`, `stage4`, `ecology`).
  - Exact status semantics: `200` committed; `400` malformed/unknown/duplicate/wrong-type JSON; `409` typed Core refusal with fresh `view + stage4 + ecology` and zero mutation; `500` `OutcomeUnconfirmed` locking commands until re-read.
  - Exact Core refusal codes surfaced verbatim with zero client-side forestry maths: `Phase2CoordinatesInvalid`, `Phase2PointOutsideArea`, `Phase2PointWithoutGroundCell`, `Phase2DuplicateOrder`, `Phase2SpeciesUnknown`, `PlantSpacingTooClose`, `PlantTooCloseToAdult`, `ShelterNoTarget`, `ShelterAlreadyPresent`, `PlayerPlanStateStale`.
  - Evidence:

- [ ] **MI-P2-02 — Single commitment gate (`Pending → Approve → Execute`):**
  - Queueing any order creates `Pending` work only with zero ecology mutation.
  - `Approve` promotes `Pending → Approved` across all six order types with zero ecology mutation.
  - `Execute` commits atomically in engine order (`SpotClear → Plant → Shelter`, plus `ClearUndergrowth`, `Fell`, `Prune`) with all-or-nothing revalidation and zero calendar advance.
  - No single map click ever mutates ecology or queues an order automatically.
  - Evidence:

- [ ] **MI-P2-03 — Four explicit modes, provisional previews, sequential planting queue & Escape order:**
  - `Select Tree` (default): crown-wins, 5 competitor rows + `Add` above the fold at `1280×720`, species-aware Prune readout, and `Queue 8-year Shelter` (`api/multi/shelter/add`) when inspecting a living `PJ` sapling without an active shelter.
  - `Inspect Cell`: swaps inspector body; shows `Clear undergrowth (spares seedlings)` (`api/multi/clear/add`) and `Remove pending clear` (`api/multi/clear/remove`) when pending.
  - `Plant`: click places provisional `Draft` local preview (dashed slate outline, snap OFF, zero engine order) at exact world `(X, Z)` and swaps inspector to Planting Drawer (`Sessile Oak` / `European Beech`, `[ ] Add 8-year shelter`, `[ ] Spot-clear ~1 m²` previewing `0.564 m` ring, `Queue to Work Plan`).
  - Sequential fence-chained queueing (`plant/add` → `shelter/add` → `spot-clear/add`): chains fresh fences; on step 2/3 `409`, stops immediately, shows exact Core refusal, and discloses earlier pending orders with one-click remove.
  - `Spot Clear`: click places provisional seedling-free `0.564 m` circular preview at exact `(X, Z)` and opens drawer with `Queue Spot Clear (~1 m²)`.
  - `Escape` order: modal → exit `Plant`/`Spot Clear`/`Inspect Cell` preview/mode → clear selection; never discards draft Fell competitor ticks and never mutates the Work Plan.
  - Evidence:

- [ ] **MI-P2-04 — Work Plan panel, dialogs & SVG map visuals:**
  - Pending/Approved Work Plan panel lists every `Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, and `SpotClear` order with targeted remove buttons before `Approve`; Execute confirmation and Year+1 consent dialogs cover all six order types.
  - Pass 1 SVG grammar preserved (`Pending` dashed gold, `Approved` pale-blue filled, `Executed` only in `Completed · History Log`, spot-clear ring beneath `PJ` saplings, stump `X` kept per N-6, zero `Expired Shelter` buttons).
  - Post-Execute map live-renders the new `PJ` sapling, active 8-year shelter, and exact world-scale `0.564 m` spot-clear patch.
  - Evidence:

- [ ] **MI-P2-05 — Real Edge browser matrix (`1920×1080` & `1280×720`, `ma-1` & `ma-1-mixed`) & payload budgets:**
  - Full lifecycle verified live in headless Edge across all 4 matrix cells: `Plant + Shelter + SpotClear + ClearUndergrowth + broadleaf Prune → Approve → Execute → Year +1`.
  - Commitment gate verified live (zero ecology change on queue and approve; atomic commit with zero year advance on execute); live-rendered `PJ` sapling, active 8-year shelter, and `0.564 m` spot-clear ring verified.
  - Refusal paths visibly `409` with exact Core codes (including sequential partial disclosure + one-click remove); modes, `Escape` order, draft Fell ticks preserved, and R3 5-row fold at `1280×720` verified.
  - Zero horizontal scroll, zero console/JS/CSP/5xx errors, zero `data:image/webp`, crowns strictly circular (`w = h = 2r`).
  - Hard raw-byte payload budgets met with Pass 2 orders pending and after Execute on both experiments at Year 0 and Year +5 (`GET api/multi/view` ≤ 524,288 B; `ecology` ≤ 153,600 B).
  - Evidence:

- [ ] **MI-P2-06 — Clean builds, tests, restored N-3/N-4 assertions, hygiene & durable handoff:**
  - Clean builds (`--no-incremental --disable-build-servers`, external `TMPDIR`): `0 Warning(s) / 0 Error(s)` for both `MultiRackSitkaP1Lab.sln` and `SitkaP1.Ui.csproj`.
  - Full Core suite: `≥ 251` existing + all new Pass 2 tests passing (`0` failures), identical PASS set on two consecutive runs.
  - `ui-smoke-multiarea.sh` updated and passing with restored Pass 1 `N-3`/`N-4` assertions.
  - Ephemeral servers stopped; protected listeners `127.0.0.1:33767`, `127.0.0.1:43649`, `127.0.0.1:46593` still listening (PID 5564).
  - Durable handoff committed; `git status --short` clean; task branch pushed only (no PR, merge, force push, or tag).
  - Evidence:

- [ ] **MI-REG — Frozen anchors and scope integrity:**
  - All 9 frozen anchors re-observed unchanged (`7e3a2a86…1ece`, `a692a6d5…233d`, `814e8165…6287`, `8a926776…87fb`, `c4b670ed…dc35`, `c67edc6b…53d8`, `776a68df…e22d`, `9a99f781…34b3`, `93e9abda…251f`).
  - **Zero edits to `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/**`** and zero edits to forbidden scopes (`Assets/**`, `Packages/**`, `ProjectSettings/**`, saves, economy, frozen references, Pass 1 specs, `main`).
  - Evidence:

- [ ] **MI-P2-REVIEW — Independent review and verdict:**
  - Independent reviewer verification complete; no self-review accepted.
  - Evidence:

## Verdict

- **Spec conformance:** NOT VERIFIED (pending independent review)
- **Implementation correctness:** NOT VERIFIED (pending independent review)
- **State:** `IMPLEMENTING` (at implementation completion: `IMPLEMENTED ON TASK BRANCH — READY FOR INDEPENDENT REVIEW — NOT INTEGRATED, NOT ACCEPTED`)
