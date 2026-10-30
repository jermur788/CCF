# PARITY-09 — Browser dependency/readiness audit

**Parent:** #91  
**Browser lane:** #162  
**Audit claim:** #177  
**Date:** 2026-10-30  
**Audited integration ref:** `integration/scenario-one-player-test-ready-v1`  
**Exact integration SHA:** `8fa83bff22ea1b4943689bee1074eed8eae65d2f`  
**Verdict:** **READY — PARITY-09 LOCAL QUALIFICATION MAY START FROM THE EXACT SHA ABOVE**

## Method

This audit does not trust issue titles or stale task-status prose. It reconciles exact pushed handoffs, Manager reviews, Unity qualification evidence, explicit user/Manager dispositions, integration handoffs and Git ancestry.

No production file, local worktree, Unity project, runtime state, rendered output or Windows build was touched by this audit.

## Prerequisite reconciliation

| Slice | Durable evidence | Integration disposition | Audit result |
|---|---|---|---|
| PARITY-03 — mixed density | `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Unity/MANAGER-REVIEW.md`: **APPROVED — MIXED DENSITY PARITY REQUALIFIED**; Legacy40 and Enlarged80 34/34, protected density fingerprints and save/load continuation green. Independent static review found no production patch; Q11 documentation repair is complete. | Exact qualified production lineage is already contained by the current integration. | **PASS**. Q10 strengthening remains separate, non-blocking evidence debt; it is not permission to weaken the PARITY-09 density gate. |
| PARITY-04 — adult Oak/Beech | `.../Implementation-04-Unity/MANAGER-REVIEW.md`: **APPROVED — ADULT SPECIES PARITY REQUALIFIED** on exact tested `81a037e35126bbc256fcda076019af47e8e0954f`; Legacy40 + Enlarged80 30/30, deterministic replay/save-load and protected Sitka comparator green. | Git compare proves `81a037e...` is an ancestor of `8fa83bff...` (`behind_by=0`). | **PASS**. Accepted Beech height-band miss remains diagnostic/calibration, not a blocker. |
| PARITY-05 — early-life/browse | Production/docs patch integrated at `1d7445e302f31641183549a3e18a8c7282f3c36a`. Unity reconciliation reproduced the corrected browse hashes on that exact PARITY-05 base; user explicitly authorised `92A0DA50D6F5D3D9` / `A3541E0EDF86DE66`. Canonical juvenile hashes remain `BFC55473C1506067` / `3485B6630C9EA448`. | Git compare proves `1d7445e...` is an ancestor of `8fa83bff...`. Authorised anchor record is durably published under `.../Implementation-05-Unity/ANCHOR-UPDATE-AUTHORISED.md`. | **PASS**. PARITY-09 must use the authorised corrected browse anchors, not the superseded packet values. |
| PARITY-06 — timber/economy/pruning | Exact Unity-qualified candidate `6abfb59e81d394e2667694446ac7e333e3488594`; independent review PASS; 14/14 Unity configurations reported PASS, including timber, residual species rules, growth/density, geometry, Reference, completion and economy. | Git compare proves `6abfb59e...` is an ancestor of `8fa83bff...`. | **PASS**. |
| PARITY-07 — player-facing truth | OpenAI review findings were fixed; final rendered handoff `Tools/Verification/Parity07Unity/HANDOFF.md` records exact candidate `5cbde15d2fd1e72dcb87ccfd25101d4e29a7bf9d`, Unity exit 0, 1031 checks, 36 nonblank captures at 1280×720 and 1920×1080, world hash restored. #91 Manager disposition accepted the slice as integrated/verified after that return. | Git compare proves exact candidate `5cbde15...` is now an ancestor of `8fa83bff...`; this closes the historical publication-only uncertainty. | **PASS**. Native OS input and non-16:9 layouts were not part of the accepted PARITY-07 gate and remain outside this prerequisite. |
| PARITY-08 — save/deterministic compatibility | `.../Implementation-08/INDEPENDENT-REVIEW.md`: **APPROVED** on exact candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`, including unknown-species pre-mutation rejection, v24 save/provenance, model-1/model-2, frozen Reference Future v1, protected density and Sitka timber anchors. | `.../Implementation-08/INTEGRATION-HANDOFF.md`: **APPROVED / INTEGRATED**, merge `7d2049e862df5c9311748167423b981e713d1fa9`; current integration `8fa83bff...` is its docs-only successor. | **PASS**. The model-0 completion diagnostic mismatch was proven byte-identical on the pre-PARITY-08 base and remains recorded D-047/D-048 verification debt; historical anchor is not rebaselined. |

## Current integration pin

Immediately before publication, the remote integration branch still resolved to:

`8fa83bff22ea1b4943689bee1074eed8eae65d2f`

The local PARITY-09 qualification must start from that exact SHA. If the branch advances before the worker starts, the worker must **not silently repin**. Return to the Manager/browser lane for a dependency/ancestry check and a packet amendment.

## PARITY-09 required scope

The approved parity task list requires one integrated Tier-2 pass over:

1. pure-Sitka and mixed Sitka/Oak/Beech density behaviour on both supported geometries;
2. adult growth, juvenile growth/survival, browsing and pruning parity;
3. Oak/Beech/Sitka timber and economy behaviour;
4. save/load and deterministic continuation, including the PARITY-08 unknown-species hard stop;
5. player-facing species truth/copy and the accepted rendered states;
6. current-model completion and solvency witnesses;
7. Multi-Scale Forestry Management V1's **Sitka-only quantitative guard**: spacing/proposal/release logic must not auto-treat Oak/Beech as Sitka or automatically remove broadleaves under a Sitka prescription;
8. all protected pure-Sitka, frozen Reference Future v1, historical save and accepted hash/anchor contracts.

The MFM species guard is an explicit accepted product constraint in `specs/multiscale-forestry-management-v1/spec.md` §9. It is part of PARITY-09 qualification, not evidence that MFM-08 itself is already qualified or integrated.

## Required review and stop rules

- Verification class: **Tier 2 / feature qualification**, not final Tier 3.
- Local execution must use the machine-wide Unity lock and the live ownership record.
- No production changes are authorised by the qualification packet. A genuine production defect is a stop/report condition; do not patch or retune inside the qualification claim.
- Do not weaken assertions or rebaseline protected values to obtain green.
- Preserve frozen Reference Future v1 exactly.
- Preserve the authorised PARITY-05 corrected browse anchors.
- Record pre-existing/non-candidate debt separately from regressions using exact base-vs-candidate evidence.
- After the local handoff, require an **independent reviewer** before marking the parity P0 spec VERIFIED. Reviewer must be independent of the local qualification executor/provider where practical.
- No Windows build.

## Release decision

All PARITY-03 through PARITY-08 prerequisites are durably accepted and are contained in the current integration lineage. No prerequisite gap blocks PARITY-09 local qualification.

The exact local packet is published alongside this audit at:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/TASK-PACKET.md`

A separate substantive local claim is required and must be pinned to `8fa83bff22ea1b4943689bee1074eed8eae65d2f`.

**Return token:** `PARITY-09 READY — EXACT LOCAL PACKET PUBLISHED`

**Dashboard sync:** GitHub handoff/claim state is the authoritative sync target for this browser task; external/generated dashboard ingestion is not asserted by this audit.
