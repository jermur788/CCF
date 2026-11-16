# Sen-1 Ground Graphics Parity — DRAFT Implementation Plan

**DRAFT — NOT APPROVED; no production or Unity authority.** Subordinate to [spec.md](spec.md).

## Phases
**G-00: audit acceptance and decisions (browser).** Verify authored graphics audit and one-file branch. Gather Q1 forestry-surface visual references (COFORD Irish road manual; UK Forest Research access classifications; licence-safe location imagery), confirm scope with owner, record Q1 track identity, Q3 lighting direction, Q4 texture budget/asset policy; keep Q5 exterior scenery outside this first slice. Q2 drafting rule is within-cell visual relocation then suppression, Q6 E80-only, Q7 SPEC REQUIRED. Require a four-file spec independent quality review and explicit spec approval before source.

**G-03: genuine before baseline (local physical Unity).** Check branch/candidate/main ownership, physical Unity machine-wide lease and current CCF-Ops board, source conflicts with #544 save and #443 Objective One, machine disk and protected prefs/save. Use six source-audit camera poses C1–C6 plus an oblique on actual E80 Year 0 at 1080p and 720p. Record same-machine C1/C6 frame median, render settings and exact tree/path/Map/coverage/historical anchors. No edit to production before baseline witness. No use of the different 30m research screenshots as E80 proof.

**G-04: lighting and fallback (one local source writer).** Proposed E80-only colour/sky/fog alternatives, lit fallback vegetation, visual contrast and frame measurements. Keep shader/material provenance and scope; do not alter global URP default profile or shared scene without a new gate.

**G-05: vegetation display mask.** Use existing path/headland/clearing definitions to find intersections. Zero on-surface instances; relocate only same-cell same-state display anchor and suppress if no safe anchor; preserve strength ordering, gameplay/ecology counts and verifiers. Record rendering-only exclusion/suppression totals. No simulated vegetation change.

**G-06: continuous route material and edge visuals.** Ground/track texture diversity and normals only from approved repo/licensed source; continuous world UV at joins, soft edges/low-cost dressing. Preserve 52 actual track GameObjects, names, transforms/active state, runtime approach/headland geometry and all readers, including map, coverage and starting-tree clearances. Do not edit any real saved world or alter new-game generation.

**G-07: focused native qualification.** Exact candidate Tier-2 Unity, both resolutions, C1–C6 before/after pairs, frame +15% threshold, no plant overlap, same tree/path/coverage/history/save/Reference/Legacy40, no skipped/red tests. Confirm current save/Prefs segregation and release physical lease afterwards.

**G-08: independent conformance and user review.** Nonauthor source/visual verifier assesses exact-head evidence; Manager issues integration verdict independently from source author. User reviews actual before/after E80 screenshots as final art acceptance. No broad rollout until explicit acceptance.

**G-09: second rollout (new tasks).** Extend accepted appearance to remaining stand and later natural clusters, Sitka needle litter and boundary dressing. Ground relief separate new spec. Gate all new assets against D-032, licences and physical performance.

## Work coordination
All physical work SERIALISED under the one Unity lock; only the assigned single C#/assets writer may edit defined source set. Separate docs/research can run in parallel, but no competing scripts touching save code, #443 files, scene files or shared source. Write exact GitHub handoffs for source, qualification, independent review and integration, with changed file inventory, commits/branches, test and rendering evidence, machine profile, hash comparisons, assets/licensing, ownership/lease release and dashboard/Drive sync status. Missing positive baseline/owner policy stops actual build, not the documentation preparation.
