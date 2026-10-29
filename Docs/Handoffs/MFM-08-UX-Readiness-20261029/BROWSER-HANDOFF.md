# MFM-08 Map / 3D Review UX Readiness

**Label: BROWSER-CAPABLE**  
**Claim #153**

Parent: #91 / #106. Approved authority: `specs/multiscale-forestry-management-v1/` MFM-08.

## Purpose

Perform browser/GitHub-only readiness, API, ownership and implementation-packet reconciliation for MFM-08 while Unity execution is blocked by the machine SSD safety floor. Prepare the exact player-facing Map/3D review contract without implementing or qualifying production UI.

## Current dependency state

- MFM-07 is integrated/verified at `c8cfae8c4025cec6e960a050edf4bfde2a981de5`.
- MFM-08A #129 is implementing the authoritative coverage-world adapter and remains local/unreviewed until its corrected candidate is pushed and qualified.
- #129 Unity qualification is currently blocked by the machine storage floor; that does not block this browser-only readiness task.
- Production MFM-08 implementation must remain blocked until an exact #129 candidate is Manager-approved and integrated/repinned.

## Browser task

1. Refresh live `Docs/Project/coordination/active-tasks.md` and claims #91, #106, #121, #125, #129, #147, #151 and #153.
2. Inspect the approved MFM-08 requirements and current published implementation APIs for:
   - `ScenarioOneCoverage` output and reasons;
   - candidate state/navigation/confirmation from MFM-05;
   - proposal state and approval boundary from MFM-07;
   - current Stand Map layers and interaction conventions;
   - current 3D navigation/inspection hooks;
   - Red Fell, Blue Crop Tree and Orange Rack mark presentation.
3. Define the smallest faithful production surface for:
   - source-sample display;
   - Covered / Not Covered / Excluded / Manual coverage states and reasons;
   - spacing-only candidate display and navigation;
   - proposal display and reasons;
   - required extrapolated-unit spot check;
   - clear non-commitment before prescription approval and later Work Plan approval.
4. Pin player-facing wording to comparable-area / demonstrated-sample-treatment language. Do not present `operator selection` as formal Irish terminology.
5. Define visual/semantic separation from Red Fell, Blue Crop Tree and Orange Rack marks. If accepted authority does not specify final colour/art treatment, leave that as bounded implementation design rather than inventing a product decision.
6. Reconcile production ownership with S1-C/S1-D presentation work and MFM-09. MFM-08 must not absorb Annual Review history, Work Plan teaching, save/schema or curriculum ownership.
7. Identify exact read-only APIs MFM-08 may consume from #129. If a required API is absent, specify the smallest accessor requirement only; do not edit production code.
8. Prepare `Docs/Handoffs/MFM-08-Map-3D-Review-UX/TASK-PACKET.md` and a child implementation claim if the surface is clear, but mark that implementation **BLOCKED ON #129 MANAGER-APPROVED / INTEGRATED LINEAGE**.
9. Publish `Docs/Handoffs/MFM-08-UX-Readiness-20261029/BROWSER-RESULT.md` and reconcile relevant claims/coordination.

## Hard boundaries

- Browser/GitHub only.
- No production C#/Unity.
- No save/schema/calibration changes.
- No MFM-09 teaching sequence or Work Plan mutation.
- No S1-C/S1-D history edits.
- No assumption that the local #129 candidate has passed qualification.
- No Windows build.

## Difficulty / model

Difficulty: **3/5** — browser-only but coupled to multiple UI and semantic boundaries.

Best-suited model/provider: **OpenAI GPT-5.6 Sol browser/GitHub lane** or equivalent strong browser reviewer.

## Expected result

`MFM-08 UX CONTRACT READY — IMPLEMENTATION BLOCKED ON APPROVED #129 LINEAGE`

## User action

Send this packet to a new browser chat. No local Unity action is required for this browser task.
