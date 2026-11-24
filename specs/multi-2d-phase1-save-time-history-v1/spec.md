# 2D Phase 1 — Save/Load, Time Pacing, and Decision History

**Status: DRAFT — NOT APPROVED — NO PRODUCTION SOURCE AUTHORITY.**  
**Classification:** SPEC REQUIRED (persistence and player workflows).  
**Product direction:** owner wants Phase 1 before simple forest maker / finance and wants the non-map subset to proceed alongside active map integration (2026-11-24).  
**Inspection baselines:** Phase 2 engine `cf6fe7a52aa096e824827c568f43fe05724dcce9`; Map Pass 1 `feat/multi-map-integration-pass1-20261124-v1` compared against Phase 2, 5 commits ahead at inspection; `main` coordination read 2026-11-24. The map branch is NOT asserted integrated or accepted.  
**Authority:** latest owner direction, D-030/D-073/D-074/D-075/D-077, specs/README.md. This is a planning proposal; D-077 authorises Scenario One calibration, NOT by itself a 2D code port.

## 1. Player promise

A player can continue the same independent 2D Sitka/Oak/Beech woodland after restarting, step one real ecological year at a time or advance toward a *suggested review point* without missing decisions, and inspect recorded work and observed stand changes across years. The player—not a timer or skipped simulation—remains responsible for Work Plan approval.

This is 2D-only, in `Tools/MultiRackSitkaP1Lab/`. It must not alter the Unity 3D save schema, Compact30, historical world snapshots, frozen reference archives, or 3D builds. No invented costs, labour, revenue, forestry cause-and-effect, or extra forest creator belongs to this phase.

## 2. Parallel-work boundary, explicitly binding for proposed implementation packets

Map Pass 1 already changed `src/Core/MultiArea/MultiAreaContinuation.cs`, `MultiAreaContracts.cs`, `MultiAreaEcology.cs`, `MultiAreaExperiment.cs`, `MultiAreaUiReadout.cs`, `MultiAreaWorkPlanAdapter.cs`, `src/Ui/PlayerMultiApi.cs`, `src/Ui/Program.cs`, `tests/Core.Tests/Program.cs`, existing test files, and the map smoke script. Do NOT claim an exclusive engine lane merely because work is not visual.

**Safe parallel tranche (1A0), once spec and lane explicitly approved:** one owner, fresh exclusive branch/worktree, NEW files only under `src/Core/Phase1/` and NEW dedicated tests/evidence with separate filenames; read existing Core as required. No edits to existing `MultiArea*.cs`, `Phase2*.cs`, `Stage5*.cs`, `Player*.cs`, `Program.cs`, existing test registration, map assets, fixtures, `Assets/**`, `.sln`, or coordination except Manager-administered updates. No automatic compilation or test success claim from source addition; verify via an authorised worker. If a new file would compile-link into the map's active build, keep it in the separate branch and do not integrate until tested against the settled map candidate.

**Deferred join (1A1/1B):** root capture/restore, real multi-tick transition and HTTP/UI controls require the shared `MultiAreaContinuation` / adapters / `PlayerMultiApi` / `Program` and must wait for a fresh integration grant following map review and collision check. Avoid reconstructing another ecology engine or changing existing `Advance` for convenience.

No source implementation while this quartet is DRAFT. This spec branch is docs only.

## 3. Persistence v1 — independent 2D files

**Proposed default, OWNER DECISION REQUIRED:** server-managed named 2D save slots, restricted to a dedicated app data directory. No arbitrary browser-supplied paths or remote upload in v1. Provide a visible save label, ecological year, experiment/genesis identity, model version vector, saved UTC and validation status. Saving/loading must not silently overwrite another slot; replace requires explicit selection/confirmation.

Define one versioned 2D envelope with format marker, schema version, model-selection vector (adult/mortality/crown/regeneration/species), immutable experiment/genesis identity/content hash and deterministic inputs, save generation/checksum, and **complete mutable root**. Source fields include the common year, all area rosters (trees incl dead and exact values), marks, all revisions, pending and approved work, executed idempotency keys and event ledger, full AnnualHistory, Stage4 pruning/deadwood/order/log/next IDs, Stage5 cell and seed/mast/credit/clear state and accounts, and Phase2 adult species identities, natural broadleaf bands, exact planted juveniles, shelters, spot patches, order state, sequence numbers and annual accounts. Maintain all specific model options/seeds that affect continuation. No economy/labour fields exist yet; do not fabricate them.

Build serialization from an authoritative capture under one engine root lock in 1A1. Do NOT save the presentational `/multi` JSON view as the source of truth, and do NOT save canopy/ground cached readouts or transient session ID. Use strict, deterministic field encoding (including float bits or lossless round-trip), ordinal ordering and explicit type tags for polymorphic orders, bounded size and recursion, duplicate-key rejection, checksum and full consistency checks before activation.

**Proposed default, OWNER DECISION REQUIRED:** refuse unknown schema/model/genesis combinations with a clear error. No auto-migration, alternate forest identity substitution or 3D save import. Preserve the previously running forest on any failure. Use temporary write, flush, atomic replacement where supported, optional prior-good backup and recovery, and never claim the file is committed before durability succeeds. If the underlying platform cannot perform an atomic replace, fail safely rather than implement an unverified copy-over-original fallback. A load creates a new runtime session fence and invalidates any old UI source tokens; any open plan must be handled by a specified safe reapproval or cancellation policy rather than silently considered approved. Which policy is an open owner decision.

## 4. One-year and multi-year advance

Keep the existing `MultiAreaContinuation.Advance` as the **single annual ecological oracle**; every skipped year must be an actual consecutive year step with unchanged ecological models and recorded accounts/history. Never calculate a later year directly. The approved D-077 four-year suggested review gap and at most five ticks per press is a **proposed 2D gameplay-calibration default only**, subject to owner acceptance for this distinct product surface. It is not a mandatory thinning schedule or model of real harvesting.

`Advance 1 Year`: explicit, unchanged annual command and existing pending-work refusal/consent semantics.

`Skip to Suggested Review`: preview from current state with visible proposed target and maximum years, no speculative mutations; refuse if pending/approved or unresolved work, unread required review, unsaved conflicting plan state, preview/reference mode, or required first-lesson decision. Never use existing `consentToCancelWork=true` as a shortcut. On confirmed advance, lock or otherwise serialize commands and execute at most five real annual ticks, stopping *before* any step needing new player consent, and stopping immediately after any genuine new objective, outcome, storm, failure, milestone, teaching-state change, year 25 or year 100 if those events are actually implemented in the 2D runtime. **Unimplemented 3D tutorial/goal/storm events must not be invented in 2D.** Return actual completed years and the honest stop reason, with intermediate annual history retained. Cancellation can only take effect between complete ticks; partial annual commits may never occur.

The D-077 Fell anchor is for navigation only; never infer an automatic silvicultural commitment, nor require a four-year wait before a player may manage. Implement no false annual-review acknowledgements.

## 5. Decision history

Represent four distinct truth levels per D-073: **L1 Recorded** plan/work/event/account facts with years, **L2 Current** actual present-year state, **L3 Derived** differences between available recorded snapshots/current readings with no causal attribution, **L4 Teaching** contextual interpretation clearly labelled as such. Historical data absent from old or partial records must be displayed as unavailable, never backfilled as if recorded.

History should use the existing `MultiAreaRoot.Events`, `AnnualHistory`, Stage4/Stage5/Phase2 work logs and accounts wherever they support statements. Define a lightweight yearly summary at actual annual commits for convenient comparison, not a second biology authority. Year listing must be paged/lazy, because Pass 1's `GET /multi/view` is already near its output-size budget. Proposed endpoints after the map join: a list of years, selected-year detail, and an optional derived comparison. Never append the whole multi-decade history to the map's main view.

Read-only structural measures, if retained in Phase 1 scope, are supplementary, labelled derived teaching indicators, computed from genuine tree geometry/species/DBH with explicit neighbour count and edge handling, not an asserted biodiversity score or causal ecology change.

## 6. Test and safety expectations

Acceptance `P1-01` … `P1-10` in `checklists/requirements.md`. Test save/load equivalence including planted juvenile→promotion, mortality, marks, orders/approval, executed-key idempotency and replay. Test stable multi-year hashes and comparison to repeated one-year steps; invalid input and partial/failed writes must not corrupt in-memory or previous good save. Verify strict stale-token refusal after load, pagination / output-size bounds, and honest provenance. Negative cases are essential. Preserve frozen MA-1 and Phase2 equivalence anchors; do not alter simulation rules or protected 3D data.

## 7. Open owner decisions — all defaults PROPOSED, NOT APPROVED

1. **Save interface:** A (recommended) restricted server-managed named slots; B browser export/import.
2. **Compatibility:** A (recommended) exact schema/genesis/model match, explicit failure; B automated migration (requires separately specified migration contract).
3. **2D skip cadence:** A (recommended) D-077-like advisory 4-year anchor/max 5 ticks; B 4 years from current year. Neither auto-approves work.
4. **History retention:** A (recommended) every real event/account plus compact annual summaries and lazy views; B every complete historical tree/cell root (large, slow).
5. **Delivery:** A (recommended) 1A isolated Core then 1B map UI after merge/review; B combined branch (must wait for map closure).
6. **Reloaded approved plans:** A (recommended) invalidate approval, keep as visible draft requiring a new explicit approval; B refuse load until plan is cleared (safe but inconvenient).

Approve the full quartet and these choices explicitly before production work. A user question about whether parallel work is possible is NOT itself approval of these technical choices.
