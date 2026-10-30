# PARITY-09 — Browser dependency/readiness audit

**Parent:** #91  
**Browser lane:** #162  
**Audit claim:** #177  
**Date:** 2026-10-30  
**Audited integration ref:** `integration/scenario-one-player-test-ready-v1`  
**Exact integration SHA:** `8fa83bff22ea1b4943689bee1074eed8eae65d2f`  
**Verdict:** **NOT READY — PARITY-03 THROUGH PARITY-08 ARE DURABLY ACCEPTED/INTEGRATED, BUT MFM-10 `SPECIES GUARD QUALIFIED` IS AN EXPLICIT UNSATISFIED PARITY-09 START GATE**

## Correction note

An initial browser draft incorrectly treated the MFM Sitka-only species guard as something PARITY-09 could prove inside its own qualification. The already-published authoritative PARITY-09 packet says otherwise: `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/TASK-PACKET.md` explicitly requires **MFM-10 Sitka-only species guard qualified before PARITY-09 starts** and says to create the local claim only when all start gates are true. This audit therefore corrects the release decision rather than weakening that prerequisite.

No production file, local worktree, Unity project, runtime state, rendered output, integration or Windows build was touched by this audit.

## PARITY-03 through PARITY-08 reconciliation

| Slice | Durable evidence | Integration disposition | Audit result |
|---|---|---|---|
| PARITY-03 — mixed density | `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Unity/MANAGER-REVIEW.md`: **APPROVED — MIXED DENSITY PARITY REQUALIFIED**; Legacy40 and Enlarged80 34/34, protected density fingerprints and save/load continuation green. Q11 documentation repair is complete. | Qualified lineage is contained by current integration. | **PASS**. Q10 remains separate, explicitly non-blocking evidence strengthening. |
| PARITY-04 — adult Oak/Beech | `.../Implementation-04-Unity/MANAGER-REVIEW.md`: **APPROVED — ADULT SPECIES PARITY REQUALIFIED** on exact tested `81a037e35126bbc256fcda076019af47e8e0954f`; Legacy40 + Enlarged80 30/30, deterministic replay/save-load and protected Sitka comparator green. | Git ancestry proves `81a037e...` is contained by `8fa83bff...`. | **PASS**. Accepted Beech height-band miss remains diagnostic/calibration, not a blocker. |
| PARITY-05 — early-life/browse | Production/docs patch integrated at `1d7445e302f31641183549a3e18a8c7282f3c36a`. Unity reconciliation reproduced corrected browse hashes on that exact base; user explicitly authorised `92A0DA50D6F5D3D9` / `A3541E0EDF86DE66`. Canonical juvenile hashes remain `BFC55473C1506067` / `3485B6630C9EA448`. | Git ancestry proves `1d7445e...` is contained by `8fa83bff...`; durable anchor authorisation is published under `.../Implementation-05-Unity/ANCHOR-UPDATE-AUTHORISED.md`. | **PASS**. |
| PARITY-06 — timber/economy/pruning | Exact Unity-qualified candidate `6abfb59e81d394e2667694446ac7e333e3488594`; independent review PASS; 14/14 Unity configurations reported PASS, including timber, residual species rules, growth/density, geometry, Reference, completion and economy. | Exact candidate is an ancestor of current integration. | **PASS**. |
| PARITY-07 — player-facing truth | Final rendered handoff `Tools/Verification/Parity07Unity/HANDOFF.md` records exact candidate `5cbde15d2fd1e72dcb87ccfd25101d4e29a7bf9d`, Unity exit 0, 1031 checks, 36 nonblank captures at 1280×720 and 1920×1080, world restored. | Git ancestry now proves exact candidate `5cbde15...` is contained by `8fa83bff...`, closing the historical local-publication uncertainty. | **PASS**. |
| PARITY-08 — save/deterministic compatibility | `.../Implementation-08/INDEPENDENT-REVIEW.md`: **APPROVED** on exact candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`; unknown-species pre-mutation rejection, v24 save/provenance, model-1/model-2, frozen Reference Future v1, protected density and Sitka timber anchors accepted. | `.../Implementation-08/INTEGRATION-HANDOFF.md`: **APPROVED / INTEGRATED**, merge `7d2049e862df5c9311748167423b981e713d1fa9`; current `8fa83bff...` is its docs-only successor. | **PASS**. Model-0 completion diagnostic mismatch remains proven pre-existing D-047/D-048 verification debt; no historical anchor rebaseline. |

## Unsatisfied prerequisite — MFM-10

The authoritative PARITY-09 packet has this start gate:

1. PARITY-03…07 accepted on the active lineage;
2. PARITY-08 compatibility pass + independent APPROVED review;
3. **MFM-10 Sitka-only species guard qualified**;
4. exact current integration SHA published and no parity/MFM species writer active.

The approved MFM task list defines **MFM-10 — Species-parity integration guard** as a separate SOL/CODEX qualification with required proof that:

- Sitka proposal behaviour is unchanged unless explicitly revised;
- Oak/Beech are not accidentally processed by Sitka-only rules;
- exclusions/reasons are accurate;
- any desired multi-species treatment stops for a spec amendment rather than silently generalising.

Its gate token is `SPECIES GUARD QUALIFIED`.

No pushed MFM-10 implementation/qualification claim, handoff or accepted `SPECIES GUARD QUALIFIED` result was found. The current programme sequence also still has MFM-08 at offline/static-candidate stage and MFM-09 downstream of the MFM-08 / S1-D T-05 integration boundary. Therefore MFM-10 is not yet executable as a completed prerequisite, and PARITY-09 must remain blocked.

## Smallest next actions

1. Finish #151 S1-D T-05 Unity qualification, final Manager/browser review and integration if green.
2. Finish #156 MFM-08 Tier-2 Unity/rendered qualification, final Manager review and integration if green.
3. Execute/qualify MFM-09 teaching-sequence / Work Plan integration on the accepted combined lineage.
4. Create and run a substantive MFM-10 local qualification claim on that exact integrated lineage; require `SPECIES GUARD QUALIFIED` with Oak/Beech exclusion/reason evidence and unchanged Sitka proposal behaviour.
5. Re-run this PARITY-09 browser readiness audit against the then-current integration SHA. Only then release/update the PARITY-09 local packet and create its substantive local claim.

The existing prepared PARITY-09 packet remains useful but **NOT RELEASED**. Its stale PARITY-08 blocker should be read as satisfied; the material outstanding start gate is MFM-10.

## Claim hygiene

A browser-created #179 PARITY-09 local claim was opened before the existing packet's MFM-10 start gate was noticed. It must not be treated as a valid dispatch. It is withdrawn/closed until MFM-10 is durably qualified and PARITY-09 is repinned.

## Current integration pin

At audit time, the remote integration branch resolves to:

`8fa83bff22ea1b4943689bee1074eed8eae65d2f`

That SHA is the current evidence baseline only. It is **not** a released PARITY-09 local candidate while MFM-10 remains unsatisfied.

**Return token:** `PARITY-09 NOT READY — EXACT PREREQUISITE GAPS PUBLISHED`

**User action required:** none.

**Dashboard sync:** GitHub handoff/claim state is updated by this browser task; external/generated dashboard ingestion is not asserted here.
