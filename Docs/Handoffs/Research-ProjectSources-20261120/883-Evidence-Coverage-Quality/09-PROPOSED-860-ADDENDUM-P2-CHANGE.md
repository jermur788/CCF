# 09 — Proposed #860 addendum change (P2 provenance promotion)

**STATUS: PROPOSAL ONLY.** Not applied. The #860 addendum lives on
`docs/860-desktop-drive-consolidation-addendum-20261120-v1` and is outside this task's write scope. This file
records the exact text change for a future editor acting under #425.

## Why one change, not a rewrite

The addendum is `DRAFT / SECONDHAND FIGURES / PRIMARY SOURCE NOT VERIFIED / CONFLICTS OPEN`. Almost all of that
label is still accurate — its other sources (Teagasc, GBIF, Zenodo, Forest Research) were not opened by its author
and were not opened here either. Exactly **one** claim can be promoted with page traceability: the P2 marking-
agreement sentence, because P2 is a complete paper on disk and its figures were checked at source.

## Current text (section D, fourth bullet)

```markdown
- Human marking agreement is only slight-to-fair (36 experiments, 12 sites, British study, Pommerening et al.
  2018, in `Rating experiments in forestry.pdf`, which is in the archive and can be checked), so expert marks are
  not "correct answers". MART pp.2, 10. I have **not** verified this against the paper.
```

## Proposed text

```markdown
- Human marking agreement is only slight-to-fair (36 experiments, 12 sites, British study; Pommerening et al.
  2018, `Rating experiments in forestry.pdf`), so expert marks are not "correct answers". **PRIMARY-VERIFIED**
  against the paper: 36 experiments at 12 sites, 0.1 ha marteloscopes, 19 rater groups of 9-20; agreement
  poor-to-fair in crown thinning and fair-to-moderate in low thinning, overall mostly slight-to-fair, 4 of 36
  experiments poor, and no experiment reaching substantial or almost perfect (P2 pp. 11, 16-17; agreement
  categories P2 Table 3, p. 9; reported p < 0.001 for low-vs-crown differences). MART pp.2, 10 remains the
  secondhand source for the *dataset-rights* discussion only.
```

## What changes in substance

| Item | Before | After |
|---|---|---|
| Provenance label for this claim | secondhand, "can be checked" | primary-verified with page pointers |
| Precision of the agreement result | "slight-to-fair" | poor-to-fair / fair-to-moderate / overall slight-to-fair, 4 of 36 poor, none substantial |
| Study description | 36 experiments, 12 sites | adds 0.1 ha plots, 19 rater groups of 9-20 |
| MART citation | supports the claim | retained, but only for dataset-rights discussion |

Nothing else in the addendum changes. In particular the conflicts in `05-UNRESOLVED-EVIDENCE.md` U-4 (Prelenfrey
access, country/site counts, species availability, Dowd DBH/height, the 40% BA ceiling, the "80-100" figure) stay
open, and the section A/E/C/F figures remain secondhand.

## Optional adjacent clarification (not proposed for action)

The addendum's "Not covered" section lists "the five academic papers (individual-based management, silvicultural
systems, Bayesian projection, structure indices, CanopyShotNoise)". Those five are **five of the same six papers**
P1, P6, P5, P3 and P4; the sixth is P2, which the addendum already half-cites. If an editor is touching this file
anyway, adding "(five of the six papers in the 11 Nov extraction; the sixth is P2)" would remove a misleading
implication that a separate unassessed set exists. This is recorded here only because it is the same file and the
same subject — it is not part of the requested Stage C repair.

## Verification behind this proposal

`03-PRIMARY-VERIFICATION-LOG.md` V-6 (36 experiments / 12 sites), V-5 (Table 2 Ae inventory), V-11 (this claim).
The page figure V-11 relies on is P2 pp. 11 and 16-17 for results and p. 9 for the category table; a reviewer with
the rendered paper should confirm the three page pointers before the text is applied.
