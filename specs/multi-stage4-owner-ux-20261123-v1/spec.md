# CCF Multi Stage 4 — Owner UX Repair and Light Guide Presentation
**Spec ID:** SPEC-MULTI-STAGE4-OWNER-UX-20261123-V1  
**Status:** APPROVED (Phase A only) — owner approved 2026-11-23 after publication at `b4dd96a2`; originally owner-supplied DRAFT, published verbatim. Phase B (UX-10) remains UNAPPROVED and requires its own G5 model/history decision and approval gate.  
**Date:** 23 November 2026  
**Coordinator:** DeepSeek orchestration agent  
**Primary implementer:** Claude Opus 5.5  
**Independent reviewer:** GPT-6.1 Sol, separate thread/lane from Opus  
**Task class:** SPEC REQUIRED — multi-part player workflow and substantial UI change; optional second phase requires cross-time comparison design  
**Product:** Standalone browser `/multi` two-area MA-1 Sitka simulator, *not* Unity 3D  
**Reviewed starting candidate:** `feat/multi-stage4-ui-catchup-20261123-v1` at immutable reviewed `e7c1a57da519ed1ee03ccfbad240196a66df0905`  
**Coordinator's detailed existing reference:** `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/MULTI-STAGE4-UI-CATCHUP-CODEX-CONTINUATION-HANDOFF-20261123.md` at `db24a5422e9b0dffc9f2ff5bcd26406243941029`, on `docs/multi-900-codex-continuation-20261123-v1`  
**Important ID collision:** This is the Stage 4 UI catch-up owner UX follow-on. It is NOT issue #900's separate DRAFT forestry canopy/light-engine specification. Do not conflate scopes or approvals.

## 0.1 Approval record

| Field | Value |
|---|---|
| Approved by | Project owner (direct instruction, 2026-11-23) |
| Approved scope | Phase A, UX-01–UX-09, plus bounded F-2 truthful `stage4:null` handling inside `PlayerMultiApi.cs` |
| Not approved | Phase B / UX-10; any `src/Core` edit; `PlayerApi.cs`; `Program.cs`; Unity `Assets/**`; saves; `main` merge |
| Published quartet | `b4dd96a2` on `docs/multi-stage4-owner-ux-spec-20261123-v1` |
| Publication is verbatim | Owner bundle preserved byte-identical, sha256 `849f89899c61d35eafc713ccc488e23b0a460a8f4332744703964fe17b3d08c4` |

### 0.2 Owner clarifications after the R2 independent review (2026-11-23)

| Question | Owner decision |
|---|---|
| Minimum supported desktop width (UX-01 requires one to be documented) | **1280x720 is the documented minimum supported desktop width.** The five competitor rows and Add must hold above the fold there, including rows whose competitor is blocked (queued, approved, Crop or felled). 1920x1080 holds today; below 1280x720 no fold requirement applies. |
| F-03 — may an open-ground read scroll the primary block out of view? | **Yes, keep the current behaviour.** The reading scrolls into view; the reference summary and `Close inspection` may leave the screen until the next selection. Escape and `Stop` remain available. |
| AC22 names "Independent GPT-6.1 Sol review". Sol's review run ended on a provider usage limit and published nothing. | **Satisfied by substitution, owner 2026-11-23.** The owner designated Claude Sonnet 5.5 as the independent reviewer; AC22 is treated as met by that review of the exact candidate. GPT-6.1 Sol did not review this change. |

These clarify open parameters in UX-01 and UX-07. They do not change any other requirement, acceptance criterion or owner decision.

## 1. Intent and definition of done
The owner personally exercised the approved-for-playtest Stage 4 `/multi` candidate and identified player-interaction problems. Prioritise the actual playable flow: click any living tree; inspect its five ranked competitors; select via map ranks 1–5 or inspector; draft Fell; close inspection with × or Escape and restore the normal map; navigate both areas without a bulky panel; inspect an unobtrusive, continuously graduated ground-light visual while retaining tree selection.

The owner selected **soft coral/peach** for increasing light, rejected green and the previous amber design. The first phase is a **bounded presentation-only repair**: do not change the forest engine's underlying light equation, measured data, crown geometry, competition ranking, actual tree positions, Work Plan semantics or numerical provenance.

**Honest visual contract:** Light of 0% draws no highlight. Any positive model light draws some positive, potentially imperceptibly subtle indicator; e.g., 1% light / 99% shade is exceptionally faint. Scale continuously, not in thresholded visible bands. At 100% light/fully unshaded ground, use a bright, readable pale peach. No invented bright gaps; real current MA-A may remain dark. Keep tree crowns visible, avoid glows, circles, band numbers, or displacing artwork. Ensure 1% value is not rounded to 0 in the display engine.

## 2. Owner-locked functional UI changes (Phase A)
### UX-01 — Five competitor choices above the fold
After selecting any living tree at 1280×720 with map legend visible, the inspector immediately shows the **five competitor rows 1–5 and an actionable 'Add selected to Fell plan' button without scrolling the inspector**. Use a compact reference tree summary (ID, DBH, crown/height only if room). Move/collapse secondary info—pruning, Crop designation, reason, technical details—below this primary flow. An empty or fewer-than-five ranked list must be represented truthfully, not padded. If narrow viewport makes this impossible, apply a responsive stack and verify a documented minimum supported desktop width; 1280×720 remains mandatory.

### UX-02 — Map ranks 1–5 are real selection controls
Click/tap the **numbered rank markers** 1–5 on the displayed competitor map to toggle their *local draft Fell selection*. Clicking again clears; allow multi-select; immediately update corresponding inspector checkbox, styling and selected count. The existing inspector controls must toggle map marker state symmetrically. Use stable **engine tree IDs**, not visual crown positions, order-only indices or scaled sprites, for the mapping. Pointer targets must be usable and accessible by keyboard, with visible focus, ARIA label using competitor number/tree ID. Clicking ranks must not accidentally select a new reference, click the ground map, submit a Work Plan or Fell a tree. Preserve the existing actual tree positions and forest state. No new API route needed.

### UX-03 — Clearly exit tree inspection
Prominent `× Close inspection` near selected reference ID; Escape invokes exactly the same inspect-exit action whenever no higher-priority modal/dialog is open. If a dialog, modal or other dismissible overlay is open, Escape dismisses that first (do not exit inspect in same event). Closing clears active **reference/temporary competitor draft choices and inspection enlargement** and restores tree map to default normal rendering; preserves current zoom, pan, active area, Crop designations, queued/approved plans, engine/year state and other committed data. Do not confuse exiting inspection with revoking a Crop Tree designation. Specify precedence when the Light Guide is active, and preserve its visible overlay unless the user turns it off.

### UX-04 — Compact synthetic information
Replace the long duplicated yellow uppercase warning banner with **one unobtrusive line** such as `Synthetic Sitka experiment · 2 areas · Site unknown` and an expandable `Details` disclosure. The full exact existing caveats remain available on demand: authored synthetic not surveyed, areas not verified comparable, site unknown, connected woodland where growth/crowns use both areas, independent per-area mortality/work plans/summaries, no treatment transfer, no Unity parity. Do not erase the source metadata or misstate area independence.

### UX-05 — Narrow navigation layout
Make map navigation panel narrower, reducing its intrusion on the stand. Compass in 3×3 grid: `N` centre top; `W` left middle and `E` right middle; `S` centre bottom. Immediately below **one horizontal `− | Fit | +` row**. Below it **`Area A | Area B` on one horizontal row**. Use minimum pointer target sizes, preserve hover/focus, north-up orientation, original pan/zoom limits, Fit behavior and area-switch semantics. No hand-drawn inaccurate compass.

### UX-06 — Light Guide appearance; continuous coral-peach
When enabled, replace existing numerous near-black circular numbered band markers and heavy violet cell fills with a **lightweight semi-transparent soft coral-to-peach illumination layer**. Use actual existing authoritative *modelled light value* `L∈[0,1]` for each clipped ground sample:
- `L=0`: invisible.
- `0<L≈0.01`: extremely faint indicator (≈99% shade).
- Gradually increase visibility, brightness and/or tint strength across *the entire continuous range*.
- `L≈1`: bright pale peach, clearly distinct from evergreen crowns without fluorescent glow.
- Suggested starting palette only: low `#7F3B4A` muted rose, medium `#DA796D` coral, high `#FFD9BD` peach; **Opus to check actual visual quality** at both screen sizes and against existing greens. Palette not an empirical ecology rule. Do not present shade as measured sunlight or silently flip cover fraction into light index. Use CSS/SVG linear continuous scale from normalized underlying model field, not thresholded 1–5 bands.
- Ground heatmap should read as light *on forest floor*, behind/under crowns where architecture permits (or delicately composited) and clipped to the area polygon, not floating globes over the trees.
- Keep Light Guide OFF by default unless separately directed. All core tree-selection controls remain functional while Light Guide is ON.
- Never fake bright patches to match earlier illustrative mockups; Area A can be nearly entirely zero light.

### UX-07 — Compact ground inspector
On explicit ground inspection, show a concise light summary: friendly light level (e.g., *Very shaded / Low / Moderate / Bright*), exact numeric model light index or percent-equivalent only with correct units, geometric canopy cover from engine where present (separate physical quantity) and count of contributing trees. No large equation or list of nine IDs in the top visible portion. Expand `Contributing trees` and `Technical details` for exact IDs, cell coordinate, grid area, Unity k10a10 source/model provenance and calibration status. Technical data must not disappear; it is moved, not falsified. Do not substitute a linear canopy-to-light identity if the 2D model supplies distinct fields.

### UX-08 — Independent tree and ground interactions
With Light Guide ON, clicking a tree's visible crown/marker must **still select a tree**. Provide explicit `Inspect ground` mode/action or separate ground-point inspect affordance; ground hit tests must not steal events intended for tree ranks or tree crowns. Define click precedence: modal > rank toggle > tree crown selection > ground inspection (only if ground mode is active). Ground mode should have obvious ON/OFF state and be keyboard accessible. Returning from ground inspection restores normal tree interaction without changing Light Guide visibility. If a chosen ground sample doesn't exist, report no result rather than synthesize a value.

### UX-09 — Remove repeated visible disclaimers
The visible map and legend should say simply `Light Guide` with a short, readable continuous colour ramp or low-to-bright endpoints (not 5 cluttered band labels). Remove repeating on-map `modelled light — not measured sunlight` and long calibration qualification from routine play, as the owner explicitly requested. Retain factual source/version/uncertainty in optional technical details and project documentation. Do not delete server provenance or change meaning of displayed numbers. Accessibility: colour alone must not convey the selected light level; the inspector provides text and a number.

## 3. Second phase — temporal before/after comparison (UX-10)
The owner explicitly requested implementing **all** earlier suggestions, including the proposed future enhancement. Therefore UX-10 is included, but **as a separately gated Phase B** because it adds cross-time state/view comparisons and may touch history/persistence policy. It must not become a stealth change to ecology or save formats.

Desired player flow: record an explicit same-area **Before** baseline from authoritative current session state; after the *actual* approved Fell Work Plan executes, present **After** current forest-floor light and optionally **Change (+/−)** by grid cell, while preserving true work timeline. Advance one or three ecological years and observe how the *same area's actual computed* light evolves with crown growth. No pending/approved-only effect. Distinguish **before/after actual intervention trajectory** from a *counterfactual no-work copy*: do not claim to show an untreated same-start control unless that independent baseline simulation really exists, is supported and is authorised. Area B is not a substitute control. Do not invent historical data if snapshots were not captured.

Proposed minimal UX-10 boundaries requiring DeepSeek/owner approval before implementation:
- Option B1 **session-local snapshots** of light grid at pre-Execute and immediately after successful Execute; optional selected year snapshots +1/+3, with no new engine or saves and no guaranteed no-work control.
- Option B2 **matched counterfactual** only after authorisation of cloning a deterministic complete simulator state and running identical year schedules; broader Core contract, separate approved spec and review.
- API/snapshot size, eviction/capture points, failure behavior on 409, area mapping, stable cell coordinate alignment, multiple interventions, model-version invalidation and redraw must be specified explicitly before code.
- The UI for phase B is `Before | After | Change` with coral/peach for absolute light and a **distinct neutral diverging palette and signed numbers** for change (do not use a misleading single light ramp for negative differences). Comparator must use actual engine numeric outputs. Preserve the separate phase A results if B cannot be approved.

**Phase A can be authored and reviewed independently after formal approval. Phase B cannot start until its exact model/history policy and writer scope receive explicit G5/spec approval.** Owner's direction to include it here establishes desired product scope, not blanket permission to change persistence/model contracts.

## 4. Existing evidence and implementation reality
- Existing reviewed Stage 4 candidate `e7c1a57d` includes 1/1/1 session default in `/multi`, Stage 4 `stage4` readout, 2.5m ground cells, actual pruning and canopy light summary, with G2 independent review APPROVED for owner playtest, *not integrated to main*.
- The `/multi` UI is embedded **Html/Css/Js raw string literals** in `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`, `internal static class MultiAreaPage`; it is not the single-area `wwwroot` page.
- Physical tree/cell data already come from 2D Core, e.g. `src/Core/MultiArea/MultiAreaUiReadout.cs` and `src/Core/CanopyLightEvaluator.cs`. The **artistic enlarged/displaced inspected crowns must never feed ecology**.
- Before this task: coordinator reported solutions builds 0 warnings/errors; Core 184/184; headless Edge 73/73 observed by worker/reviewer; reviewer G2 APPROVED. `ui-smoke-multiarea` 110 passing/6 failing previously triaged selector drift; single-area smoke 250/4 from worker. These are **prior reported evidence**, not new verification.
- Known F-2: `stage4:null` erroneously shown as legacy/model-0, can offer empty Fell confirmation despite stale-fence Execute refusal. **Include the narrowly scoped truthful unavailable-state handling in the implementation when inside granted `PlayerMultiApi.cs` scope**, test it; otherwise stop and report conflict.
- F-3 halo around light-band numbers should disappear when the old numbered marker layer is removed; explicitly test the new symbols at 1280/1920. F-7 stale docs is separate maintenance, do not rewrite unrelated historical documents without a path grant.
- The owner already confirmed Stage 4 prune lifts **2.5/5/6.5m**, natural dead standing `D` crown **plus** fallen log, and screenshot AC-9 **≤0.9MB/image, ≤5MB/10-image set**. These are locked, not open for re-design.

## 5. Source/write boundaries and project protections
DeepSeek must create the required **APPROVED** quartet (`spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`) on a canonical source branch and secure live exclusive D-091 ownership and actual physical lease before Opus writes. This draft pack itself is **not** an admission or writer grant. Check current `Docs/Project/coordination/active-tasks.md` from remote/live `origin/main` and D-091 reservations, locks/listeners; local `/home/jer/Development/CCF-HP` main was reported 1864 ahead / 50 behind origin/main and MUST NOT be reset or treated as remote main.

**Proposed Phase A write allowlist:** `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` only plus narrowly necessary new/revised **UI tests and UI smoke harness** in `Tools/MultiRackSitkaP1Lab/tests/`, and new agent-specific Markdown handoff evidence. Read-only Core inputs are allowed only if explicitly granted in the task packet; the current candidate includes compile-linked files but linkage is not source write authority. If UI cannot be implemented inside this scope, STOP and request a different approval. Do not edit `src/Ui/PlayerApi.cs`, `src/Ui/Program.cs`, `src/Core/**`, Unity `Assets/**`, fixtures, projects, P0 pins, saves, `main`, frozen models, protected packages or actor branches. Phase B requires separate allowlist when approved.

Use fresh branch/worktree off exact reviewed candidate `e7c1a57d`; do not move/overwrite the reviewed branch or branch from untrusted local main. No G4 integration, no Unity/Windows builds, no shared heavy Editor lock without separate approval; physical Ops and active-worktree locks govern.

## 6. Non-functional expectations
- Performance: light overlay should not generate hundreds of independent text circles or excessive hit targets. Measure before optimizing any 342 KB Stage 4 response; this task does not license payload refactoring.
- Accessibility: rank marker focus/toggle, Esc prioritization, visible selected state beyond colour, screen-reader announcement of selection counts, buttons rather than div-click semantics.
- Default visual layout retains current owner-selected tree artwork and 2D map grid; no full redesign. Smaller navigation, compact disclaimer, inspector layout and light overlay are the intended presentation changes.
- Keep Year+1 behavior and Work Plan Pending→Approve→Execute fencing intact. Selected ranks are **draft only** until Add; no change to commitments.
- Preserve stand-A/B independence for actions and per-rack summaries, while current connected-woodland growth/crowns may use both areas.
- Avoid implementing new forest-floor ecological law, new crown model, measured PAR display or regeneration response.

## 7. Owner approval and classification
- **Owner-selected decisions already captured:** all UX-01 to UX-09 behaviors, soft coral/peach continuously graduated light from almost invisible at 1% to brightest at 100%, no repeated “this is a simulation” warnings; UX-10 desired separately.
- **Unresolved implementation gates:** canonical APPROVED quartet, live D-091 writer scope, exact target HEAD/locks and Phase B state/history policy.
- **Review statuses:** SPEC DRAFT until formally approved. The existing Stage 4 G2 APPPROVED verdict belongs to `e7c1a57d`, not to the new repair.
