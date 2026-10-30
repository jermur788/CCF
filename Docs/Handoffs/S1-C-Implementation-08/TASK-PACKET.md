# S1-C T-08 — Review / Management History Feature Qualification

**Parent:** #91 / local umbrella #106 / browser prep #186  
**Spec:** `specs/scenario-one-review-history-v1/` T-08  
**Status:** **PREPARED / NOT RELEASED — BLOCKED ON #187 MANAGER ACCEPTANCE + SERIALIZED UNITY AVAILABILITY**  
**Preparation reference:** current Scenario One integration `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`  
**Purpose:** bounded Tier-2 Unity qualification of the complete S1-C history/review slice followed by browser/Manager spec-conformance review.

## Difficulty and routing

- **Difficulty:** High — cross-cutting save/history/UI/rendered qualification with genuine-old-save and grouped-money honesty requirements.
- **Preferred executor after release:** SOL/CODEX high-reasoning Unity verification lane or Claude Sonnet-class Code lane.
- **Independent conformance:** Overall Manager/browser after the pushed handoff; cross-provider code/save review if material ambiguity appears.

## Current dependency state

- #151 S1-D T-05 is APPROVED / INTEGRATED / CLOSED; it no longer owns Unity.
- #187 S1-C T-07 is repinned to `ce673714...` and is READY/OFFLINE but **not running and not yet accepted**.
- #156 MFM-08 currently has first priority on the released serialized Unity lane after fresh preflight.
- S1-D T-08 is also released but serialized; live coordination decides the post-#156 order. If #187 is accepted before the next Unity slot, current coordination prefers S1-C T-08 first so later S1-D T-08 can reuse final history evidence.

No statement in this packet proves the Unity lock is free.

## Start gate / candidate

Do **not** start until:

1. #187 returns `S1-C DATA CONTRACT STABILITY QUALIFIED — READY FOR MANAGER REVIEW` and receives Manager `APPROVED`;
2. current integration is refreshed and one exact candidate is pinned containing accepted S1-C T-01…T-07;
3. live `active-tasks.md` shows the machine-wide Unity lane available and grants T-08 the next slot without overlap;
4. no history/Annual Review writer conflicts with qualification;
5. a substantive T-08 execution claim is created/marked `[RUNNING]` only at actual start with model/provider/branch/worktree/start SHA.

Suggested branch/worktree after release: `qual/s1-c-review-history-v1` / `/media/jer/Videos/CCF-S1C-T08`.

At release, inspect and record the **actual accepted save schema**. Current integration is v24; never hard-code a stale version merely because an old packet did.

## Required qualification matrix

On the exact candidate, exercise and evidence:

1. multi-year session with at least three completed years;
2. earlier-year navigation and return to latest year;
3. at least two distinct intervention types with aggregate-first drill-down;
4. grouped-harvest year with job/year-level settlement and no invented per-order money;
5. no-work year;
6. recorded / current / derived / reconstructed / session-only / not-recorded provenance states used honestly;
7. prior-treatment guidance on at least two distinct histories, with a faithful recorded-location waypoint where available and no invented waypoint when unavailable;
8. objective-achievement history;
9. current-schema save/load round trip;
10. genuine supported old-save load required by the approved S1-C contract, using the exact source/identity established by accepted #187 rather than assuming a synthetic fixture name proves provenance;
11. accepted T-07 identity/data-contract checks remain green;
12. Annual Review acknowledgement semantics unchanged;
13. browsing/querying is read-only and does not mutate current world or historical records;
14. complete S1-C requirements/checklist mapping.

If accepted #187 identifies a genuine supported v20 source, use and name that exact source. Do not substitute the previously corrected synthetic `v20-records` fixture merely because of its label.

Rendered evidence at **1280×720** and **1920×1080** must include material year-browser, drill-down, provenance, guidance, old-save and no-work states without blocking overflow/clipping.

## Protected / cross-feature gates

- frozen Reference Future v1 unchanged;
- Legacy40/pre-feature semantics preserved under current save support;
- current MFM prescription/work-order provenance remains present and honestly labelled where history encounters it;
- accepted S1-D management-event identity hardening remains authoritative;
- no protected hash/anchor silently rebaselined;
- no second history UI/store/query authority;
- no S1-D generic Work Plan or MFM teaching implementation under T-08;
- no production repair inside the qualification claim.

If a production defect appears, stop with exact evidence and request separate bounded repair ownership.

## Unity discipline

Tier-2 uses the machine-wide Unity lock. Fresh-preflight exact candidate, live coordination, free space, Editor processes and lock immediately before acquisition. Restore protected save/world state and leave the lane clean. No Windows build.

## Handoff / final review

Publish:
`Docs/Handoffs/S1-C-Implementation-08/HANDOFF.md`

Record exact candidate/ancestry, claim/model/provider, branch/worktree, changed files (expected verification/docs only), commands/counts, current/old-save identities, rendered capture manifest, requirements matrix, protected compatibility, lock/cleanup receipt and deviations.

Local success token:

`S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED`

Then browser/Overall Manager inspects exact pushed evidence and issues `APPROVED / NEEDS CHANGES / REJECTED`. Only APPROVED qualifies S1-C feature scope for later Tier-3.

## Copy/paste dispatch line — use only after Manager release

`Run S1-C T-08 from Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md only after #187 T-07 is Manager-accepted, an exact current integration candidate is repinned, and live coordination grants a free serialized Unity slot. Tier-2 qualification only: execute the complete multi-year/year-browser/drill-down/grouped-harvest/no-work/provenance/guidance/objective-history/current-save/genuine-supported-old-save/rendered matrix at 1280×720 and 1920×1080. Reuse the exact genuine old-save identity accepted by #187; do not invent one. Do not fix production under the qualification claim. Publish HANDOFF.md and return S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED or the exact blocker. No Windows build.`
