# 02 — Coverage matrix: the six-paper extraction against the four composites

**Status:** `RESEARCH PRODUCED`. Supporting research, not decision authority.
**Machine-readable:** `02-coverage-matrix.tsv` (68 rows, 11 fields) and `01-source-inventory.tsv` (source files).

## 1. Question this stage had to answer

#860 recorded that the six-paper extraction had never been assessed. #883 asks whether that was a **reconciliation
failure** (the papers were assessed but not carried across) or a **coverage failure** (relevant evidence genuinely
missing). It also had to settle whether the "five academic papers" named in #860 were a separate unassessed set.

## 2. Method

1. Read the 24-page extraction in full to enumerate its topic-level claims and its P1–P6 page references.
2. Enumerate the four composites: heading map, source manifests, then a **concept-token sweep** over all four files
   for each extraction topic. The sweep is the evidence for a "no coverage" verdict, and its token list is printed
   below so a reviewer can rerun or challenge it.
3. Classify each row; record coverage strength separately.
4. Primary-check the consequential figures (`03-PRIMARY-VERIFICATION-LOG.md`).

The four Project attachment copies total **132,717 bytes (131,136 characters)**. Including the canonical Git Growth
composite (35,555 B instead of the stale 33,058 B attachment) the set is **135,214 bytes**. The token sweep in the
table below was run over the attachment copies.

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
vocabulary does not appear at all. **#896 ran an independent, broader sweep** (165 alternate terms and 65 exact
figures over five files) and reached the same bulk verdict: no composite covers marking agreement, structural
indices, CanopyShotNoise, the Bayesian spatial model, or the regeneration-system taxonomy under other wording.
The sweep reduces but does not eliminate the false-negative risk (`05-UNRESOLVED-EVIDENCE.md` U-8).

## 3. Status definitions used

| Status | Meaning |
|---|---|
| `COVERED` | The claim is already held, at adequate depth, in an existing composite |
| `CROSS-REFERENCED-INTENTIONALLY` | Topically adjacent material exists, but it is a different source or a different claim; the right action is a pointer, not duplication |
| `MATERIAL GAP` | Relevant, source-supported evidence with no adequate home in the composite set |
| `DISPUTED` | The figure or its attribution is contested, mis-stated or secondhand and needs resolution before reuse |
| `OUT OF SCOPE` | Not a CCF-relevant increment, or belongs to another programme/decision |
| `DESIGN NOTE` | A producer inference or design safeguard, **not a source finding**; excluded from evidence counts |

Coverage strength is recorded separately (`NONE` / `ADJACENT` / `PARTIAL` / `DIRECT`) because "adjacent" and
"missing" are different failures and should not be merged.

`DESIGN NOTE` was added after #896: two rows (P2-09, P6-09) were producer inferences dressed as evidence and had to
leave the evidence tally without being silently deleted.

## 4. Result summary

| Paper | Rows | MATERIAL GAP | CROSS-REF | COVERED | DESIGN NOTE | DISPUTED | OUT OF SCOPE |
|---|---|---|---|---|---|---|---|
| P1 Individual-based management | 11 | 7 | 4 | 0 | 0 | 0 | 0 |
| P2 Marking agreement | 9 | 7 | 1 | 0 | 1 | 0 | 0 |
| P3 Structural indices | 8 | 8 | 0 | 0 | 0 | 0 | 0 |
| P4 CanopyShotNoise | 7 | 6 | 1 | 0 | 0 | 0 | 0 |
| P5 Bayesian dynamics | 14 | 10 | 4 | 0 | 0 | 0 | 0 |
| P6 Regeneration taxonomy | 12 | 7 | 3 | 1 | 1 | 0 | 0 |
| Other sources | 7 | 1 | 0 | 4 | 0 | 1 | 1 |
| **Total** | **68** | **46** | **13** | **5** | **2** | **1** | **1** |

Row arithmetic: 46 + 13 + 5 + 2 + 1 + 1 = 68.

Changes since the 67-row version: `P2-01` and `P2-04` moved `DISPUTED` → `MATERIAL GAP` (primary-verified, so
nothing is in dispute); `P5-09` moved `MATERIAL GAP` → `CROSS-REFERENCED`; `P2-09` and `P6-09` moved to
`DESIGN NOTE`; candidate row `P6-12` was added.

**Read this table as a measure of how finely topics were split, not of importance.** P5 alone is 14 rows. The
honest summary is the producer's own: six topic clusters, mostly method descriptions and caveats.

The six papers are **not** largely covered. The composites are built from a different evidence family — Irish
ecology, Irish browsing, Irish/GROWFOR growth and density, Irish practice and economics — while the six papers are a
*forest-science method and human-judgement* family. The two families barely overlap. That is the answer to the
owner's question.

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
(#896 confirms the file-name and topic match. The optional one-clause clarification in the addendum remains
unapproved and is left to #425.)

## 6. Judgement calls — post-#896

1. **P1-01 scored `CROSS-REFERENCED`, strength now `PARTIAL`.** Practice §3 already states the retained-tree /
   competitor-only logic and carries the Brownhill trial; P1 is *supporting literature* for that framing, not its
   evidential basis. Downgrading the wording was #896's correction and is accepted.
2. **P2-09 scored `DESIGN NOTE`.** P2 says nothing about automation or delegation (P2:2 only notes that machine
   operators select trees in some countries); D-065 is not a composite and holds no P2 evidence. The delegation
   boundary is a producer inference and is now labelled as one, not counted as a cross-reference.
3. **P6-09 scored `DESIGN NOTE`, not `COVERED`.** No Doc, composite or git-tracked file holds a
   regeneration-strip/extraction-rack distinction; `git grep` finds none. P6's own figures draw extraction racks
   *inside* strip/wedge/group systems, which makes the distinction the producer's, not the paper's.
4. **P5-09 is `CROSS-REFERENCED`, low priority.** The source reports that Hegyi variants and Gaussian kernels
   "showed a performance similar to the selected RBA with very little difference" — a qualitative one-sentence
   statement with no numbers, in a Finnish selection-forest fit. It says nothing about CCF's Hegyi treatment, and
   the earlier "strongest justification for not replacing Hegyi" wording was unsupported and is deleted.
5. **P3-08 stays a gap but is a caveat to P3-01…07, not an independent finding.** It only qualifies the indices; the
   paper's own wording ("randomness as a reference", "no substitute for naturalness") is preserved rather than
   paraphrased into a slogan.
6. **P6-11 is labelled `CROSS-REFERENCED` in both TSV and narrative** (it had been called a "gap" in prose while
   the TSV said cross-reference). P6 does mention Sitka qualitatively — Irish clearcut natural regeneration and a
   Welsh group-strip shelterwood example — and those now sit in candidate row `P6-12`, tagged qualitative with no
   Irish rate.
7. **No row was created for gameplay calibration.** Nothing in these six papers authorises a coefficient.

## 7. Row schema, page conventions and completeness

- `paper_page` is the page of that paper **as supplied**. Collection page = `paper_page` + offset:
  **P1 +2, P2 +14, P3 +34, P4 +54, P5 +84, P6 +98.** P4 Table 1 happens to be p. 12 in both. #896 found the
  earlier column name `original_pdf_page` misleading and it is renamed.
- **The 68 rows are not exhaustive.** #896 identified Sitka material in P6 that had no row (added as `P6-12`) and
  several unexamined rows (`P1-02`, `P2-03`, `P3-06`, `P4-05`, `P5-06`, `P6-01`…`P6-03`, `P6-06`, `O-03`) that rest
  on topic/number sweeps only.
- `DESIGN NOTE` rows are excluded from the material-gap and cross-reference counts by definition; they are kept so
  the design reasoning stays visible and reviewable rather than being deleted.
- 19 of the original 67 rows were not re-adjudicated by #896 and 8 of the proposed composite's 77 assertions were
  not checked. Neither blocks consideration; both limit any claim of completeness.
