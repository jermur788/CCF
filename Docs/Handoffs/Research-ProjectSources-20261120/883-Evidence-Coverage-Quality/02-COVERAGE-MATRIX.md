# 02 — Coverage matrix: the six-paper extraction against the four composites

**Status:** `RESEARCH PRODUCED`. Supporting research, not decision authority.
**Machine-readable:** `01-source-inventory.tsv` (inventory) and `02-coverage-matrix.tsv` (this crosswalk, same rows plus the status legend).

## 1. Question this stage had to answer

#860 recorded that the six-paper extraction had never been assessed. #883 asks whether that was a **reconciliation
failure** (the papers were assessed but not carried across) or a **coverage failure** (relevant evidence genuinely
missing). It also had to settle whether the "five academic papers" named in #860 were a separate unassessed set.

## 2. Method

1. Read the 24-page extraction in full to enumerate its topic-level claims and its own P1–P6 page references.
2. Enumerate the four composites: heading map, source manifests, then a **concept-token sweep** over all four files
   for each extraction topic. The sweep is the evidence for a "no coverage" verdict, and its token list is printed
   below so a reviewer can rerun or challenge it.
3. Classify each row into one of five statuses, and separately record coverage strength.
4. Primary-check the consequential figures at the 111-page original (`03-PRIMARY-VERIFICATION-LOG.md`).

The concept sweep, per composite:

| Topic family | Tokens searched | Ecology | Browsing | Growth | Practice |
|---|---|---|---|---|---|
| frame/crop tree | frame tree, crop tree, final crop, target tree, elite tree | 0 | 0 | 2 | 3 |
| competitor/release | competitor, crown release, local release | 3 | 0 | 0 | 7 |
| marking agreement | marteloscope, marking agreement, kappa, rater | 0 | 0 | 0 | 1 |
| structural indices | mingling, diameter differentiation, Clark-Evans, contagion, Pielou, pair correlation | 0 | 0 | 0 | 0 |
| crown geometry/plasticity | crown plastic, crown cent, crown diameter, superorganism | 0 | 0 | 0 | 0 |
| Bayesian/uncertainty | Bayesian, credible interval, posterior, relascope, ingrowth, uncertainty | 0 | 1 | 1 | 0 |
| regeneration taxonomy | shelterwood, group system, strip system, plenter, two-storey, regeneration method, silvicultural system | 0 | 0 | 0 | 0 |
| h/d ratio | h/d, height-diameter | 0 | 0 | 3 | 2 |

Those counts are why three topic families are reported as zero coverage rather than "possibly thin": the
vocabulary does not appear at all in 130,756 bytes of composite text.

## 3. Status definitions used

| Status | Meaning |
|---|---|
| `COVERED` | The claim is already held, at adequate depth, in an existing composite |
| `CROSS-REFERENCED-INTENTIONALLY` | Topically adjacent material exists, but it is a different source or a different claim; the right action is a pointer, not duplication |
| `MATERIAL GAP` | Relevant, source-supported evidence with no adequate home in the composite set |
| `DISPUTED` | The figure or its attribution is contested, mis-stated or secondhand and needs resolution before reuse |
| `OUT OF SCOPE` | Not a CCF-relevant increment, or belongs to another programme/decision |

Coverage strength is recorded separately (`NONE` / `ADJACENT` / `PARTIAL` / `DIRECT`) because "adjacent" and
"missing" are different failures and should not be merged.

## 4. Result summary

| Paper | Rows | MATERIAL GAP | CROSS-REF | COVERED | DISPUTED |
|---|---|---|---|---|---|
| P1 Individual-based management | 11 | 7 | 4 | 0 | 0 |
| P2 Marking agreement | 9 | 5 | 2 | 0 | 2 |
| P3 Structural indices | 8 | 8 | 0 | 0 | 0 |
| P4 CanopyShotNoise | 7 | 5 | 1 | 0 | 1 |
| P5 Bayesian dynamics | 14 | 11 | 3 | 0 | 0 |
| P6 Regeneration taxonomy | 11 | 6 | 3 | 2 | 0 |
| Other sources | 7 | 1 | 0 | 4 | 1 |
| **Total** | **67** | **43** | **13** | **6** | **4** |

Reading that table honestly: the six-paper set is **not** largely covered. The composites are built from a
different evidence family — Irish ecology, Irish browsing, Irish/GROWFOR growth and density, Irish practice and
economics — and the six papers are a *forest-science method and human-judgement* family. The two families barely
overlap. That is the answer to the owner's question.

## 5. The "five academic papers" question — resolved

#860 left open whether five academic papers remained unassessed. They do not. The five named topics map onto
P1, P3, P4, P5 and P6:

| #860 topic | Paper |
|---|---|
| individual-based management | P1 |
| silvicultural systems | P6 |
| Bayesian projection | P5 |
| structure indices | P3 |
| CanopyShotNoise | P4 |

The sixth paper, P2 (`Rating experiments in forestry`), is the one the #860 addendum half-cites secondhand. There
is **no separate missing source set**. What was missing was the reconciliation, exactly as the task suspected.

## 6. Judgement calls a reviewer should test

1. **P1-01 / P1-04 / P2-08 / P2-09 scored CROSS-REFERENCED, not MATERIAL GAP.** If the reviewer judges that the
   composites' practitioner-level statements are too thin to bear the weight, these become material gaps and the
   proposed addition grows. This is the most consequential soft judgement in the packet.
2. **P6-08 and P6-09 scored COVERED from other sources.** Enrichment planting and the rack/regeneration-strip
   distinction are genuinely already held (Practice §3; research-index). Adding P6 material there would duplicate.
3. **P3-08 and P6-11 scored gaps even though they are "negative" claims.** A limitation is still evidence the
   project needs, because it prevents unsupported inference. They are cheap to add and expensive to omit.
4. **P5-09 is scored MATERIAL GAP although it supports the status quo.** "Several competition indices performed
   similarly" is the single best published defence for not replacing the existing Hegyi treatment, so its absence
   is a real gap in the project's ability to justify a decision it has already made.
5. **No row was created for gameplay calibration.** Nothing in these six papers authorises a coefficient, and no
   row claims otherwise.
