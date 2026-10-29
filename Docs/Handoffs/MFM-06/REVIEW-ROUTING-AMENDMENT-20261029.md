# MFM-06 — Independent Review Routing Amendment

**Date:** 2026-10-29  
**Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`  
**Implementation:** #111 `MFM-06 — save/provenance foundation`  
**Independent review:** #112  
**Integration gate:** #113

## Purpose

This amendment supersedes **only** the model/provider routing for the MFM-06 independent review in `Docs/Handoffs/MFM-06/TASK-PACKET.md`. All technical scope, compatibility rules, verification requirements, stop conditions and integration gates in the original packet remain unchanged.

## Why the route changed

The packet was prepared on the assumption that MFM-06 implementation/qualification would be performed by OpenAI 6.1 Sol and the independent review by Claude Opus 5.5 Low.

At actual task start, #111 recorded a different executor:

- **implementation provider/model:** Anthropic / Claude Sonnet 5.5 via the T3 local lane;
- **branch:** `feature/mfm-v1-save-provenance-v23`;
- **worktree:** `/media/jer/Videos/CCF-MFM-06`;
- **reported starting HEAD:** `ac876a64969752a3864034521f42559671264b30`, explicitly local/unpushed;
- **reported local schema check:** `ForestSaveData.CurrentVersion == 22`, therefore MFM-06 owns v23 on that exact local lineage.

The browser/Manager lane does **not** independently verify the unpushed starting SHA or local schema assertion. They remain T3 execution evidence until published.

Because the implementation provider is now Anthropic, using another Claude model for the mandatory high-risk save/reference/determinism review would not satisfy the programme's cross-provider-independence requirement.

## Correct independent-review route

**#112 reviewer: OpenAI 6.1 Sol.**

Start #112 only after #111 has published:

1. an exact GitHub-resolvable candidate SHA/ref;
2. `Docs/Handoffs/MFM-06/HANDOFF.md` with the required schema/compatibility/negative-test/Unity/save evidence;
3. the exact v22 → v23 ownership proof for the implemented lineage;
4. evidence that malformed MFM save data is rejected before mutation;
5. Legacy40, pre-feature Enlarged80 and frozen Reference Future compatibility evidence;
6. protected hash/comparator evidence without silent rebaselining.

If OpenAI 6.1 Sol remains unavailable when #111 is ready, **#112 stays WAITING**. Do not silently substitute Claude Sonnet or Claude Opus for this mandatory independent gate.

## Review scope

OpenAI 6.1 Sol reviews the exact published #111 candidate against the approved MFM-06 authority, including:

- next-version ownership and migration semantics;
- stable prescription IDs;
- rule/version/source-sample identity;
- accepted coverage decisions;
- approval/year fields only to the extent authorised by MFM-06;
- work-order provenance and history linkage;
- missing fields = no prescription;
- no migration of existing marks/orders into fabricated prescriptions;
- deterministic validation/loading behaviour;
- malformed-new-data rejection before world mutation;
- v22 S1-C objective/progression-history preservation;
- Rack V2 / Legacy40 / pre-feature Enlarged80 compatibility;
- frozen Reference Future integrity;
- whether local/Unity/save evidence was run on the exact candidate presented for review.

Verdict: `APPROVED`, `NEEDS CHANGES`, or `REJECTED` in:

`Docs/Handoffs/MFM-06/INDEPENDENT-REVIEW.md`

The reviewer does not modify production or integrate under #112.

## Integration gate

#113 remains blocked until all original integration conditions are satisfied **and** #112 returns `APPROVED` under the corrected cross-provider route.

MFM-07 must not start before accepted MFM-06 integration.

No Windows build.

## Copy/paste review dispatch line

`Run claim #112 as an independent OpenAI 6.1 Sol review of the exact published #111 MFM-06 candidate. Use Docs/Handoffs/MFM-06/TASK-PACKET.md plus Docs/Handoffs/MFM-06/REVIEW-ROUTING-AMENDMENT-20261029.md. Review save/schema ownership, v22→v23 compatibility, provenance persistence, malformed-data pre-mutation rejection, Legacy40/pre-feature/Reference preservation and exact-candidate Unity/save evidence. Do not edit or integrate production. Publish Docs/Handoffs/MFM-06/INDEPENDENT-REVIEW.md with APPROVED / NEEDS CHANGES / REJECTED. No Windows build.`

**User action required now:** none. The review remains waiting for #111 evidence and OpenAI 6.1 Sol availability.
