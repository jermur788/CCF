# MFM IMPLEMENTATION 01 — PREREQUISITE / LATEST-LINEAGE QUALIFICATION

**Task ID:** `MFM-01`  
**Feature:** Multi-Scale Forestry Management V1  
**Spec:** `specs/multiscale-forestry-management-v1/` — **APPROVED FOR IMPLEMENTATION PLANNING**  
**Task type:** READ-ONLY BASE QUALIFICATION  
**Overall Manager:** ChatGPT Game Dev  
**Status:** **READY / DISPATCHED**

## Authority

MFM-00 coordination authority is already present on canonical `main`:

- Decision Log D-065 exists;
- Current Milestone marks MFM V1 required before beginner/external Scenario One testing;
- Research Index contains the delegated-treatment / sample-to-comparable-area evidence audit and wording boundaries.

Rack V2 core prerequisite is now satisfied by canonical integration merge:

`f23549b18b17b3542ec8f633d7309b11020990d4`

The exact qualified Rack repair production HEAD was:

`7b557fd43ca9468a1be22c161c24064eec9f955a`

Post-merge comparison showed the only additions relative to the qualified Rack branch were management/coordination documentation; no production file changed.

## Goal

Confirm the exact implementation base for MFM before any production implementation starts.

Return whether the integrated Scenario One lineage satisfies the MFM-01 prerequisite contract and provide the exact inputs for MFM-02 and MFM-03.

## Required preflight

1. Refresh `Docs/Project/coordination/active-tasks.md`.
2. Record exact current `main` SHA; it must descend from `f23549b18b17b3542ec8f633d7309b11020990d4`.
3. Work in a clean read-only worktree/lane.
4. Confirm no conflicting writer owns files you inspect; inspection itself does not claim ownership.
5. Do not acquire the Unity lock for this packet. If a fact cannot be established without Unity, record it as `LIVE UNITY CONFIRMATION REQUIRED` for MFM-02/03 rather than running Unity here.

## Verify

### Rack V2 prerequisite

- persisted authoritative confirmed-rack objects exist on the integrated lineage;
- stable rack IDs and geometry survive save/load;
- world identity / old-save policy is explicit;
- Legacy40 does not enter Rack V2;
- Reference Future v1 remains frozen/compatible;
- current headland/access geometry and confirmed-rack APIs can support a representative rack-served sample;
- identify the exact authoritative API/data needed to order confirmed racks along their common normal and select the third/central rack.

### Current Scenario One lineage

- current save schema version and relevant rack/prescription-free save fields;
- current teaching sequence and `ScenarioOneRackBay` / lesson-2–5 implementation;
- current rack-bay footprint derivation and historical fixed middle authored-line dependency;
- current map ownership / Stand Map source model;
- current Work Plan ownership and approval boundary;
- current Crop Tree / Fell / Rack-mark states and conflict semantics;
- current completion / review chronology relevant to inserting `Extend your treatment` after Lesson 5.

### MFM geometry inputs

Reconcile the approved MFM-02 contract against integrated code:

- representative teaching rack = third confirmed rack ordered along the common normal;
- sample anchor = closest point on that rack centreline to stand centre, clamped for ±12 m length;
- fixed teaching footprint half-width = 9.0 m;
- 8 m teaching Crop Tree spacing remains separate from 8 m Hegyi neighbourhood;
- rack corridor exclusion comes from actual confirmed Rack V2 geometry;
- rack-served coverage boundaries are derived separately from the fixed teaching footprint;
- outer remainders must be explicit.

Report which of these can be proven statically and which require live Unity measurement.

## Important Rack teaching-bay compatibility decision

The Rack V2 Manager review established that Lessons 2–5 cannot remain anchored to an unrelated historical authored line once player-authored V2 racks become the real plan.

For V2, the teaching area must use the deterministic central confirmed rack; model-0/Legacy40 retain their historical middle authored rack. This matches MFM-02's already-approved `third confirmed rack ordered along the common normal` rule.

Do not implement this in MFM-01. Report the exact later patch surface and tests required so MFM-02 can implement it once the base is qualified.

## Output

Publish:

`Docs/Handoffs/MFM-Implementation-01/HANDOFF.md`

Include:

- exact inspected `main` SHA;
- exact branch/worktree;
- prerequisite table with `PASS`, `LIVE UNITY CONFIRMATION REQUIRED`, or `BLOCKER`;
- current save-schema identity;
- authoritative rack API/data map;
- current lesson/rack-bay geometry map;
- current map/Work Plan ownership map;
- exact MFM-02 patch surface prediction;
- exact live measurements still required by MFM-02/03;
- ownership/conflict findings;
- verdict.

## Verdict

Use exactly one:

- `MFM IMPLEMENTATION BASE QUALIFIED`
- `MFM BASE QUALIFIED WITH LIVE UNITY CONFIRMATIONS DEFERRED TO MFM-02/03`
- `BLOCKED — RACK V2 PREREQUISITE`
- `BLOCKED — IMPLEMENTATION BASE / OWNERSHIP CONFLICT`

## Exclusions

No production edits.  
No prescription logic.  
No proposal state.  
No Crop Tree candidate generation.  
No save/schema changes.  
No map/UI edits.  
No Rack lifecycle edits.  
No S1-C/S1-D edits.  
No Windows build.
