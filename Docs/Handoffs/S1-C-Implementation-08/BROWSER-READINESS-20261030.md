# S1-C T-08 — Review / Management History Final Qualification — Browser Readiness

**Date:** 2026-10-30  
**Browser prep claim:** #186  
**Authority:** approved `specs/scenario-one-review-history-v1/` T-08 / complete checklist  
**Status:** PREPARED — BLOCKED ON S1-C T-07 ACCEPTANCE AND UNITY AVAILABILITY

## Dependency reconciliation

Published programme evidence supports S1-C T-01 through T-06 on the current integration lineage:

- T-01 read-only history query is present in production and consumed by later accepted S1-C slices;
- T-02 Annual Review year browsing is integrated and later S1-C/S1-D work relies on it;
- T-03 intervention drill-down/rendered qualification was completed/integrated before the later slices;
- T-04 provenance labelling is Manager-approved and integrated (issue #123);
- T-05 objective/progression history v22 is present in the accepted lineage and later save versions preserve it;
- T-06 prior-treatment guidance is Manager-approved/integrated (issue #147; exact integration production merge `625e0adf5e7fd71184cb074e85e1901b417ec02d`, docs successor `cfdc93bf...`).

T-07 is the remaining semantic prerequisite. No dedicated T-07 qualification was found, so it has now been released separately as verification-only claim #187 on exact published integration base `8fa83bff22ea1b4943689bee1074eed8eae65d2f`.

Therefore **do not release T-08 until T-07 returns green, is Manager-accepted, and the exact then-current integration candidate is repinned.**

Heavy Unity is also currently occupied by #151 S1-D T-05. T-08 must not bypass the serialized machine lane even if T-07 finishes first.

## Current schema / compatibility context

The original T-08 packet predated later accepted save work and referred to MFM v23. Current published integration contains save schema v24 and accepted S1-C v22 / MFM v23 / S1-D v24 lineages. T-08 must test the **actual current accepted schema on its final pinned candidate**, plus the spec-required genuine v20 old-save path. Do not regress or fabricate intermediate state.

## Required qualification

The approved T-08 gate must exercise one exact candidate and prove:

1. multi-year session with at least three completed years;
2. browse earlier year(s) and return to latest without mutating history;
3. multiple intervention types with aggregate-first drill-down to recorded targets;
4. grouped-harvest year with one job/year settlement and no invented per-order money;
5. no-work year;
6. provenance classes used honestly: recorded / current / derived / reconstructed / session-only / not recorded;
7. prior-treatment guidance using only recorded history, including faithful recorded-location waypoint where available and no invented waypoint when unavailable;
8. objective-achievement history, including old-save exact-derivation versus `not recorded` behavior;
9. current-schema save/load round trip;
10. genuine v20 old-save load and safe forward behavior;
11. accepted T-07 identity/data-contract stability;
12. Annual Review acknowledgement semantics unchanged;
13. no mutation from year browsing/querying;
14. rendered review at 1280×720 and 1920×1080 for all material states without blocking overflow/clipping;
15. complete `specs/scenario-one-review-history-v1/checklists/requirements.md` evidence matrix.

## Protected / cross-feature gates

- frozen Reference Future v1 unchanged;
- Legacy40/pre-feature compatibility required by the accepted save path preserved;
- current MFM prescription/provenance state is not lost, relabelled or fabricated by S1-C browsing;
- accepted S1-D management-event ID validation remains authoritative;
- protected hashes/anchors are not silently rebaselined;
- no second history UI/store/query authority;
- #151 S1-D generic Work Plan presentation is not absorbed into S1-C qualification;
- no production repair under the qualification claim.

If the gate finds a production defect, stop and publish the exact failing witness for a separate bounded repair assignment.

## Required handoff / review

Local handoff:
`Docs/Handoffs/S1-C-Implementation-08/HANDOFF.md`

The handoff must name the exact candidate, ancestry, claim/model/provider, branch/worktree, Unity lock receipt, commands/counts, old/current save fixtures, capture manifest, complete requirements matrix, protected compatibility and clean return state.

After the local handoff, Overall Manager/browser issues one of:

- `APPROVED`
- `NEEDS CHANGES`
- `REJECTED`

Only APPROVED may be integrated/marked accepted for S1-C feature scope.

Local success token:
`S1-C REVIEW / MANAGEMENT HISTORY V1 — FEATURE QUALIFIED`

No Windows build.
