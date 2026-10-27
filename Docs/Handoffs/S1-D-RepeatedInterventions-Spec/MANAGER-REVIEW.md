# MANAGER REVIEW — S1-D Scenario One Second and Subsequent Interventions

**Date:** 2026-10-27  
**Reviewer:** Overall Manager (ChatGPT Game Dev)  
**Reviewed branch:** `spec/scenario-one-repeated-interventions-v1`  
**Reviewed worker HEAD:** `666cb67cfc3e17fc22df5607ac4043c8fbd7ee7d`  
**Packet base:** `3b499aa459b92212f7bb06897836bae5914b5230`  
**Implementation lineage inspected by worker:** `c6342f79a3e89c040fadd232017a937132779385`  
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`

## Verdict

**NEEDS CHANGES — NARROW MANAGER AMENDMENT ONLY**

The draft is accepted in substance. Its core diagnosis is sound: the candidate Scenario One lineage already contains substantial partial S1-C/S1-D machinery, but that does not make the canonical S1-C/S1-D milestone items complete. The code is an implementation baseline to preserve and re-verify, not proof that the features are integrated, verified or accepted.

No new product-design cycle is required. The branch must be amended to record the Manager decisions below, reconcile the S1-C interface to the actual S1-C draft/Manager review, and convert the package to `APPROVED` only after those amendments are exact.

## Manager decisions on OPEN-1…OPEN-11

### OPEN-1 — candidate-lineage baseline

**DECISION:** Accept the following from the `c6342f7` Scenario One lineage as the **partial implementation baseline** that S1-D extends, subject to T-01 re-verification on the actual integrated implementation base:

- derived management/intervention history architecture;
- the protected reviewed-two-cycle completion rule;
- completion guidance;
- the derived sequential lesson/guide architecture.

This does **not** mean S1-C or S1-D are complete. Canonical milestone wording that they remain pre-demo work is still correct until their approved specs are implemented, integrated and feature-qualified. The older statement “no new S1-C/S1-D development needed” is not accepted as product authority.

The protected two-cycle completion rule and its normalised-source hash remain unchanged unless a future explicit Manager decision opens them.

### OPEN-2 — later-cycle management mode

**DECISION:** **Bounded hybrid.** The first guided learning sequence establishes the minimum teaching path. After that, later interventions are ordinary player-directed management with advisory guidance and the existing global scenario objectives/completion conditions. Do not create a new prescriptive objective for every cycle and do not imply a single correct treatment.

### OPEN-3 — recurring Annual Review acknowledgement

**DECISION:** **Advisory, not a recurring hard gate.** The game should surface when the latest intervention has not been reviewed, and the protected completion chronology may continue to require reviewed cycles, but later Work Plan approval/advance must not be blocked merely because a review acknowledgement is outstanding.

### OPEN-4 — “ready for next intervention”

**DECISION:** Readiness is an **advisory derived state**, never a gate and never a timetable.

It should report components/reasons, not a single prescriptive yes/no. At minimum distinguish:

- approved work awaiting execution;
- pending work, including carried-over work;
- stale/invalid orders requiring attention;
- whether the latest resolved intervention has been reviewed;
- years since the latest resolved intervention as descriptive context only.

No minimum observation interval is introduced. “Wait and observe” remains a valid choice indefinitely while the scenario itself remains manageable.

### OPEN-5 — 40% teaching-footprint safeguard

**DECISION:** The teaching-footprint 40% cumulative trunk-area safeguard must **not remain a lifetime hard block on ordinary later management**.

During the lesson/sample activity that owns the safeguard, retain the existing hard teaching constraint. After that teaching activity is complete, exceeding the historical footprint threshold may be shown as an advisory warning for manual later work but must not disable Approve Work Plan or Advance.

This does not weaken a separately approved Multi-Scale Forestry Management prescription ceiling. Prescription-specific limits remain owned by that feature.

### OPEN-6 — previous versus new provenance

**DECISION:** V1 uses **derived provenance from existing orders/events/history only**. Do not add Crop Tree designation-year persistence and do not add a dated past-work map layer under S1-D V1.

Where designation time is absent, say nothing that implies a date. Spatial review may reuse S1-C history/waypoint surfaces where recorded locations already exist.

### OPEN-7 — lesson/guide progress authority

**DECISION:** Keep the guide **derived from persisted history; add no new save progress field for S1-D V1**.

Make the learning sequence monotone using historical “ever satisfied” facts / a frozen teaching-sample completion condition. Session-only `[J]/[K]` inspection state and device-wide `PlayerPrefs` must not be progression authority after the associated lesson has historically been completed.

A durable Annual Review/history fact may be used as the latch where it faithfully establishes that the release/review lesson has been completed. Do not persist `[J]/[K]` decisions merely to prevent guide regression. If later Multi-Scale Forestry Management needs those decisions as prescription inputs, that persistence remains owned by the Multi-Scale spec and must not be smuggled into S1-D.

### OPEN-8 — completion and post-completion management

**DECISION:** Keep the existing protected minimum of **two reviewed successful intervention cycles in distinct years** as the completion chronology requirement. Do not increase it under S1-D V1.

Continued management after `Completed` is accepted until the existing scenario horizon/terminal conditions. Completion is an achievement state, not an automatic lockout.

### OPEN-9 — racks in later cycles

**DECISION:** S1-D does not decide rack reopening/additional-rack authoring. Defer to the Rack Planning & Field Marking V2 authority. S1-D only requires that Objective 1 rack teaching does **not automatically repeat** in every management cycle.

### OPEN-10 — Legacy40 scope

**DECISION:** Geometry-independent derived presentation/re-entry features may work for Legacy40 where the required historical records exist. Enlarged80-only rack/sample/curriculum features must not be migrated into Legacy40. Reference Future v1 remains frozen.

### OPEN-11 — second Multi-Scale prescription

**DECISION:** A second Multi-Scale Forestry Management prescription is **not part of S1-D V1**. S1-D treats any Multi-Scale-generated Work Plan orders as ordinary orders. Later prescription re-derivation belongs to a Multi-Scale amendment.

## Additional Manager decision — ambiguous management-event IDs

Approve the T-06 hardening requirement in principle, with this implementation policy:

- reject a loaded save with duplicate or non-positive management-event IDs **before mutation**;
- provide a clear validation error;
- do not silently repair, renumber, or reinterpret historical event identity;
- if a shipped/accepted fixture is discovered with ambiguous IDs, STOP and return to Manager before changing compatibility policy.

This is a save-history integrity gate and requires the normal load-path verification, but no schema field.

## S1-C reconciliation required

The S1-D draft was written before the actual S1-C draft existed. Reconcile §9 / EI-1…EI-7 to the S1-C specification and its Manager review.

Accepted S1-C direction relevant to S1-D:

- one read-only history capability over existing authoritative records; no competing history store;
- stable IDs and append-only/read-only history surfaces are the compatibility contract;
- recorded fact, current observed state, derived comparison and teaching must remain distinguishable;
- intervention detail is aggregate-first with drill-down;
- session-only browse/DBH facts remain session-only;
- pre-feature objective dates may be labelled-derived only where exact reconstruction from persisted year-tagged records is evidence-complete; otherwise “not recorded”;
- full mark/retain decision provenance is not required for S1-C V1.

S1-D may define a small read-only cycle-state adapter over that contract, but must not create a second history authority or duplicate S1-C UI.

## Sequencing / implementation base

1. Finish and integrate the active Rack V2 core work first.
2. Complete the S1-C narrow spec amendment so its contract is `APPROVED`.
3. Amend/approve S1-D using this review.
4. Run S1-D T-01 against the **actual integrated Rack V2 + approved S1-C contract** base before production coding.
5. If T-01 passes, T-02 (derived cycle state), T-03 (teaching-cap decoupling), and T-06 (event-ID validation) may be prepared as separate non-overlapping implementation lanes subject to live ownership.
6. T-04 guide monotonicity should land before the later Multi-Scale curriculum insertion if both touch the same guide/current-state files.
7. T-05 previous-vs-proposed UI must coordinate with the S1-C UI owner and reuse its history surface rather than duplicate it.
8. T-07 conditional persistence is **CANCELLED for S1-D V1** by the decisions above unless a later explicit decision reopens it.

## Required amendments before approval

Update `spec.md`, `plan.md`, `tasks.md`, and `checklists/requirements.md` so that:

1. OPEN-1…OPEN-11 become a resolved decision register or are replaced by the decisions above;
2. status becomes `APPROVED` only after every amendment is applied;
3. the baseline is described as partial implementation evidence, not canonical completion;
4. T-01 is the mandatory re-verification gate on the eventual integrated base;
5. T-02 defines advisory component/reason state with no intervention timetable;
6. T-03 scopes the 40% cap as hard only during its owning teaching activity and advisory/non-blocking later;
7. T-04 uses persisted-history/frozen-sample facts, not new S1-D save state or `PlayerPrefs` authority;
8. T-05 has no designation-year field and no S1-D dated map layer;
9. T-06 rejects ambiguous event IDs pre-mutation; no silent repair;
10. T-07 is cancelled for V1;
11. the protected two-cycle completion rule remains unchanged and continued management after completion is accepted;
12. §9/EI-1…EI-7 are reconciled to the S1-C contract above;
13. all references to a fixed future save version remain conditional on the actual integration base; S1-D V1 itself adds no schema field;
14. the original handoff receives an addendum recording this Manager review and the amended/approved HEAD.

## Review verification

Manager compared `3b499aa...` → `666cb67`: the branch is two commits ahead and contains only the task packet, S1-D spec package and handoff. No production source, Unity asset, scene, prefab, save, package or settings change is present.

The worker correctly labelled its implementation findings as static inspection of `c6342f7`; no Unity/runtime verification is inferred from the draft.

## Implementation authority

**NOT YET.** The branch remains DRAFT until the amendment packet is executed exactly. If the amendments match this review and introduce no new ambiguity, the Manager may approve/integrate the spec without another product-design round.

## Dashboard

`DASHBOARD SYNC: NOT AVAILABLE`  
`Published dashboard reflects this review: NO`
