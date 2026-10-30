# S1-C T-08 — Review / Management History Feature Qualification

**Parent:** #91 / local umbrella #106 / browser prep #186  
**Spec:** `specs/scenario-one-review-history-v1/` T-08  
**Status:** **DEPENDENCY-CLEARED ON T-07; NOT RELEASED — WAITING AUTHORITATIVE UNITY LOCK + #156 COMPLETION / LANE RELEASE**  
**Preparation reference:** current Scenario One integration `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`  
**Purpose:** bounded Tier-2 Unity qualification of the complete S1-C history/review slice followed by browser/Manager spec-conformance review.

## Difficulty and routing

- **Difficulty:** High — cross-cutting save/history/UI/rendered qualification with genuine-old-save and grouped-money honesty requirements.
- **Preferred executor after release:** SOL/CODEX high-reasoning Unity verification lane or Claude Sonnet-class Code lane.
- **Independent conformance:** Overall Manager/browser after the pushed handoff; cross-provider code/save review if material ambiguity appears.

## Current dependency state

- #151 S1-D T-05 is APPROVED / INTEGRATED / CLOSED; it no longer owns Unity.
- #187 S1-C T-07 is **MANAGER APPROVED / CLOSED**. Manager review: `Docs/Handoffs/S1-C-Implementation-07/MANAGER-REVIEW-20261030.md` at main `950a620da0933f1ca92b294e763410ec1d6166b2`.
- Accepted T-07 evidence fixes the genuine old-save source for T-08: v20-y5 SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`; do not substitute the rejected synthetic `v20-records` case.
- #156 MFM-08 retains first priority on the serialized Unity lane. Its fresh preflight is currently blocked on OPS lock-governance repair #189; this is not an MFM-08 product failure.
- S1-D T-08 is also released but serialized. Because T-07 is now accepted before the post-#156 slot opens, current coordination prefers **S1-C T-08 immediately after #156** so S1-D T-08 can reuse final history evidence.

No statement in this packet proves the Unity lock is free. Do not start while #189 is unresolved or #156 owns/awaits the first restored Unity slot.

## Start gate / candidate

Do **not** start until:

1. OPS #189 restores the authoritative machine-wide Unity lock mechanism and the normal lock tool can establish holder/free state;
2. #156 completes its Tier-2 work and releases the serialized Unity lane, or Overall Manager explicitly reprioritises the lane;
3. current integration is refreshed and one exact candidate is pinned containing accepted S1-C T-01…T-07;
4. live `active-tasks.md` grants T-08 the next slot without overlap;
5. no history/Annual Review writer conflicts with qualification;
6. a substantive T-08 execution claim is created/marked `[RUNNING]` only at actual start with model/provider/branch/worktree/start SHA and lock receipt.

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
10. genuine supported old-save load using the exact v20-y5 source accepted by T-07 (SHA-256 `af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00`);
11. accepted T-07 identity/data-contract checks remain green;
12. Annual Review acknowledgement semantics unchanged;
13. browsing/querying is read-only and does not mutate current world or historical records;
14. complete S1-C requirements/checklist mapping.

Do not substitute the previously corrected synthetic `v20-records` fixture merely because of its label.

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

`Run S1-C T-08 from Docs/Handoffs/S1-C-Implementation-08/TASK-PACKET.md only after OPS #189 has restored authoritative Unity lock governance, #156 has completed/released the first restored Unity slot (unless Overall Manager explicitly reprioritises), an exact current integration candidate is repinned, and live coordination grants T-08 the next serialized slot. Tier-2 qualification only: execute the complete multi-year/year-browser/drill-down/grouped-harvest/no-work/provenance/guidance/objective-history/current-save/genuine-supported-v20-y5-old-save/rendered matrix at 1280×720 and 1920×1080. Reuse the accepted v20-y5 fixture SHA af448bb297491d96bb18457ab2173694b2a0eafc2b8674d4438f25c9101eff00; do not invent/substitute one. Do not fix production under the qualification claim. Publish HANDOFF.md and return S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED or the exact blocker. No Windows build.`
