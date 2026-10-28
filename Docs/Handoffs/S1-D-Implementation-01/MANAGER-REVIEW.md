# S1-D T-01 — Repeated-intervention baseline re-verification — Manager Review

**Task:** `S1-D-T01 — re-verify the repeated-intervention baseline before later cycle work`  
**Claim:** #13 (`[HANDED-OFF]`, closed)  
**Worker handoff:** `Docs/Handoffs/S1-D-Implementation-01/HANDOFF.md`  
**Inspected candidate:** `49db765f1dd883310ae56c523d0f317a70d27353`  
**Worker branch tip:** `860f9dc89fea2b2078d74e44ebca4448ed1ee852`  
**Handoff integration:** PR #71 / merge `4d6864515c32c87445bdff42819fb358905a10a0`  
**Manager verdict:** **APPROVED — S1-D BASELINE RE-VERIFIED; T-02..T-06 UNLOCKED SUBJECT TO LIVE OWNERSHIP**

## Accepted evidence

- The review ran read-only against an actual `main` descendant of the integrated Rack V2 core and approved/integrated S1-C T-01/T-02 base.
- F-1 through F-11 and R-1 through R-5 remain materially unchanged for the approved S1-D assumptions.
- F-12 is **changed but spec-compatible**: Rack V2 reopening exists and the teaching/sample geometry is now derived from the third confirmed rack. Those changes are owned by accepted Rack V2/MFM authority rather than S1-D.
- The repository still has one derived-history architecture: `ScenarioInterventionHistory` plus the S1-C per-year query layer. No second history store, duplicated history presenter or competing identity scheme was found.
- The normalised `ScenarioOneCompletionRules` source remains pinned; S1-D did not change the completion rules.
- Offline checks reported PASS for N3 (88), S1-C query (55), scenario history (74), rack-bay (87) and rack-sample (75).
- The single UX failure is accepted as a pre-existing stale assertion after the onboarding change that made the walking-HUD first-use summary null; it is not evidence of S1-D baseline drift.
- The four reported type-check errors are `Painter2D` reference-assembly limitations in files outside S1-D scope; they are not S1-D code errors.
- No Unity run or Windows build occurred or is inferred.

## Manager interpretation of the three risk notes

### 1. Teaching-footprint 40% cap can become a global trap

Accepted as a real **static/reachable-by-reading risk**, not a verified runtime failure. Mortality/storm can shrink the living denominator while completed felling remains in the cumulative removed term, so the teaching footprint may cross 40% after the player's intervention. If that leaves `RackPlanProblem` non-empty, Approve and Advance can both be disabled even though the player cannot undo executed removals.

This does **not** invalidate T-01. It strengthens the already-authorised S1-D T-03 verification requirement: the offline decision table must include the mortality/storm-only crossover case, and T-03 must stop for Manager review if the retained in-activity rule can leave the player permanently unable to complete the activity.

### 2. Reopening a Rack V2 plan can move the guide

Accepted as a T-04 regression case. When Rack V2 planning is reopened and the derived teaching bay becomes unavailable, the guide may fall onto no-bay branches. T-04 must include this state in its monotonicity matrix without changing Rack V2 reopening/locking semantics.

### 3. T-02 history precision and ownership

Accepted. T-02 must choose and document one exact definition of "latest resolved intervention" and one derivation of per-year acknowledgement instead of adding an uncoordinated third interpretation. It must stay a read-only derived adapter with no persisted state.

S1-C T-03 claim #19 is currently dispatched but unstarted. Because it may touch the same history-query files as S1-D T-02, Manager sequencing is: **S1-D T-02 owns the history-query file set first; S1-C T-03 waits until T-02 is integrated or a narrower non-overlapping file boundary is explicitly agreed.**

## Dispatch consequence

The approved S1-D spec now authorises T-02, T-03, T-04 and T-06 after T-01. T-05 remains dependent on T-02 plus S1-C UI ownership. T-08 remains blocked on T-02..T-06.

The immediate non-Unity lane is **S1-D T-02 — derive advisory cycle/readiness state from existing persisted records**. It is intentionally code-only/offline and does not require the machine-wide Unity lock.

**Verdict:** `APPROVED — S1-D BASELINE RE-VERIFIED; T-02..T-06 UNLOCKED SUBJECT TO LIVE OWNERSHIP`

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
