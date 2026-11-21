# 07 — Review packet (delta gate after the #896 Stage D review)

**State of the gate:** Stage D was performed by #896 (Claude Sonnet 5.5, independent thread) and returned
**NEEDS CHANGES** with 28 corrections, 14 blocking. This producer has answered R-1…R-28
(`11-R1-R28-DISPOSITION-AND-CORRECTION-HANDOFF.md`). **This file is now the brief for the delta review**, not a
first review.

| Label | State |
|---|---|
| RESEARCH PRODUCED | yes |
| INDEPENDENTLY REVIEWED (first pass) | yes — #896, `NEEDS CHANGES` |
| CORRECTIONS APPLIED | yes — see `11` |
| DELTA REVIEW CLOSED | **no** |
| CANONICALLY APPROVED / PUBLISHED | **no** |

## 1. Scope of the delta review

**Only R-1…R-28.** Do not re-audit the unchanged rows or the unchanged parts of `08`: #896 already did that and
explicitly said a delta review need not re-audit wholesale. The producer's disposition map lists, for each R-ID, the
file and the exact change, so the review is a check of that list.

Suggested diff: the previous producer commit `053686b5` to the corrected commit named in `11`.

## 2. What the reviewer must decide

| Dimension | Question |
|---|---|
| (i) Source traceability | Does each R-1…R-28 change carry the page/paper evidence #896 asked for — including λ_B, the Eq. 10 scope, the P2 page pointers, and the 18-vs-20 and 17-plot ambiguities kept visible rather than resolved? |
| (ii) Scientific quantities | Are the corrected numbers right (49 ha⁻¹ yr⁻¹, >half of death causes unknown, κ_F vs c̄ direction, Ae 0.133 ha, VES01 context)? |
| (iii) Coverage completeness | Are the reclassifications defensible — P2-01/P2-04 → gap, P5-09 → cross-reference, P2-09/P6-09 → design note, P1-01 strength, P6-12 added — and does the 68-row tally (46/13/5/2/1/1) add up? |
| (iv) Practical suitability | Is `08` still usable as a proposed attachment, now that the Handling column, the training list and the recruitment claim have been removed or relabelled, and that the licensing hold is stated? |

## 3. Required checks

1. **Tell-tale consistency sweep** for the values that changed: `λ_B`, `0.133`, `1,321`, `20 plots`, `49`,
   `more than half`, `κ_F`, `c̄`, `VES01`, `bare land`, `P6-12`, `DESIGN NOTE`.
2. **Arithmetic:** 68 rows, 11 fields each, one row per ID, status tally 46/13/5/2/1/1, per-paper table consistent
   with the TSV.
3. **No new unapproved claims:** read `08` §9 and §10 as a licence/boundary check, not as science.
4. **No smuggled design advice:** confirm `08` §5 has no Handling column, §4's training list is labelled a producer
   inference, and the P3 illustration makes no recruitment claim.
5. **Scope:** the diff touches only `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/**`.
6. **Honesty of attribution:** no statement in the packet may claim the producer rendered a page. Where rendered
   readings are used they must be attributed to #896.

## 4. Verdict format

```
DELTA VERDICT: APPROVED | NEEDS CHANGES | REJECTED
R-IDs closed: <list>
R-IDs still open: <list with reason>
(i)  Source traceability:    <verdict> — <evidence>
(ii) Scientific quantities:  <verdict> — <evidence>
(iii) Coverage completeness: <verdict> — <evidence>
(iv) Practical suitability:  <verdict> — <evidence>
NEW FINDINGS (if any): <numbered>
RESIDUAL RISK: <what remains unverified>
```

A delta APPROVED means the corrected research is suitable for **#425 consideration**. It does not approve
publication, attachment changes, licensing or adoption.

## 5. What the reviewer must not do

- Do not edit producer files; keep any review note under
  `Docs/Handoffs/Research-ProjectSources-20261120/883-Independent-StageD-Review/` on a review branch.
- Do not approve #860 edits, Project attachments, Drive state or a merge.
- Do not treat the corpus as evidence for a gameplay mechanism.

## 6. Live unresolved items the reviewer should not treat as defects

Recorded in `05`: U-1/U-2 (composite Git counterparts, stale Growth attachment), U-4 (#860 conflicts), U-5
(2025 Yearbook), U-6/U-6b (18 vs 20; 17-plot ambiguity — deliberately preserved), U-8 (topic-level sweep), U-10
(CC BY-NC-ND licensing hold), U-11 (partial verification coverage), U-12 (attachment bytes unconfirmed).
