# Access Spine & Diagonal Rack Network — Draft Plan

**Status: DRAFT / PLANNING ONLY.** References: D-067 (design direction), D-064 Rack V2 (current binding spec), [#246](https://github.com/jermur788/CCF/issues/246).

## Workflow

1. **Read-only source reconnaissance:** T3 Code inspects exact published Scenario One candidate (no checkout or Unity run), mapped central path and rack geometry, existing trees/trails, world-to-map/minimap rendering, marking, movement, work order/persistence. Return line-referenced evidence. Do not infer world meters from screenshot pixels.
2. **Verified scene geometry later only if necessary and safe:** after previous Unity lease release and fresh lock/disk preflight, measure spine endpoint, stand boundaries, row bearings, distances, five-rack feasibility and entrance targets in an isolated lane. **No Unity permitted by this initial packet.**
3. **Product reconciliation:** Manager tests options A (existing spine extended, diagonal racks, default), B (path stays footpath, separate extraction spine), C (edge-headland-only Rack V2 baseline) against actual site geometry and educational outcomes. A is preferred unless evidence gives a concrete failure.
4. **Spec authority:** decide whether spine is existing vs planned vs paid work, draft/confirmation behaviour for disconnected racks, how the five-rack target is preserved, and persistence compatibility. Modify D-064 and Rack V2 spec **only through approved amendment**; preserve accepted implementation as baseline until then.
5. **Implementation after spec approval:** bounded separate ownership of authoritative network, Stand Map, world/minimap rendering, first-person traversal, objective education and verification; merge only after cross-provider review and proportionate affected delta qualification.

## Technical invariants (to validate)

- No independent map/3D rack authoring or duplicate counters.
- Existing confirmed racks and entrance marks remain valid across save/load unless explicitly, compatibly migrated; executed removals never reversed.
- Proposed/access/cleared status differentiated in every projection.
- Connectivity graph and actual painted entrance targets are derived from one authoritative geometry.
- Path traversal slowdown is gameplay calibration, not unsubstantiated physics precision.
- The user may make a poor **proposal** and see why; work approval remains a distinct action.

## Risks and gates

Primary risks are accepted Rack V2 geometry/persistence regressions, MFM coverage area changes, 3D visual-mesh mismatch, forest walkability and unintended world-hash changes. No Windows build; no full accepted Tier-3 rerun solely to explore options. Verify current user playtest is not interrupted.

## DPC orchestration and evidence policy

**Owner:** DPC parent orchestrator for local task routing, with #200 Overall Manager retaining product approval, coordination, integration and release decisions. **Permitted worker models:** Claude Sonnet 5.5 **Medium** and OpenAI GPT-6.1 **Medium**, chosen according to actual measured/observed capacity, task fit and required independence. Exact runtime model/effort and subscription pool must be recorded; no claim of guaranteed availability.

**Mandatory GitHub ledger:** `Docs/Handoffs/ScenarioOne-Access-Spine-20261103/ORCHESTRATION-LEDGER.md`. DPC appends contemporaneous V2 events for dispatch, provider/context/quota preflight, switching/cooldowns, rework, handoffs, verification and acceptance outcomes. Each significant child handoff includes a V2 Orchestration Decision Appendix and links its ledger events. Source label each fact, claim and inference; unknown estimates remain UNKNOWN. Historical 30 October ledger and #241 backfill are separate.

**Continuation:** DPC does not stop for a provider limit if a permitted alternative has adequate capacity or an unrelated already-authorised task can proceed. Preserve claims, branch/worktree ownership, context checkpoint and artifacts before switching. A DRAFT spec, unsafe Unity lease, protected gate, cross-provider review or unapproved product change is **not** bypassable. Do not turn this plan into production authority; AS-01/#246 is currently the only released access-spine step.
