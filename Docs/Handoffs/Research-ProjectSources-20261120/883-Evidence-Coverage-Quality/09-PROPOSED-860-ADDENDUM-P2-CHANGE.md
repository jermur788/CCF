# 09 — Proposed #860 addendum change (P2 provenance promotion)

**STATUS: PROPOSAL ONLY — NOT APPLIED.** The #860 addendum lives on
`docs/860-desktop-drive-consolidation-addendum-20261120-v1` and is outside this task's write scope. This file
records the exact text change for a future editor acting under #425.

## Why one change, not a rewrite

The addendum is `DRAFT / SECONDHAND FIGURES / PRIMARY SOURCE NOT VERIFIED / CONFLICTS OPEN`. Almost all of that
label is still accurate — its other sources (Teagasc, GBIF, Zenodo, Forest Research) were not opened by its author
and were not opened here either. Exactly **one** claim can be promoted with page traceability: the P2 marking-
agreement sentence, because P2 is a complete paper on disk and its figures were checked.

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
  against the paper by the #883 producer (claims V-6/V-11) and independently by the #896 reviewer: 36 experiments
  at 12 sites, marteloscopes 0.1 ha each (Ae 0.133 ha per **P1 Table 1, p. 7**; P2 Table 2 gives 0.1 ha), 19 rater
  groups of 9-20; low-thinning kappa_F
  generally 0.35-0.51 (fair to moderate) and crown thinning 0.01-0.28 (poor to slight, the single exception being
  Loch Ard 2015 at 0.44), overall only slight-to-fair (abstract), 4 of 36 experiments poor and none substantial or
  almost perfect (P2 pp. 1, 11, 14, 16, 17; agreement categories Table 3, p. 9; low-vs-crown p < 0.001, p. 11).
  MART pp.2, 10 remains the secondhand report of this same agreement finding (MART PDF p.10); the rights tiers are
  a separate bullet (MART pp.6-7, 15, 19). Note that this addendum's author still did not read the complete
  six-paper PDFs; the verification above is cited to those two artefacts, not to this document.
```

## What changes in substance

| Item | Before | After |
|---|---|---|
| Provenance label for this claim | secondhand, "can be checked" | primary-verified, with named verifiers and page pointers |
| Precision of the agreement result | "slight-to-fair" | low thinning fair-to-moderate; crown thinning poor-to-slight with one fair (Loch Ard 2015); overall slight-to-fair; 4 of 36 poor; none substantial |
| Study description | 36 experiments, 12 sites | adds 0.1 ha plots (Ae 0.133 ha per **P1 Table 1, p. 7**; P2 Table 2 gives 0.1 ha), 19 rater groups of 9-20 |
| MART citation | supports the claim | retained, but as the secondhand report of the same finding |
| Authorship of the verification | unstated | named, with the addendum's own blind spot preserved |

## Deliberate non-changes

- The addendum's "Not covered" line stating the six-paper PDFs were "not read" **stays true of its author** and is
  not removed. The proposed sentence acknowledges this instead of papering over it.
- The other eight reports and every unresolved conflict stay exactly as they are: Prelenfrey access, country/site
  counts, species availability, Dowd DBH/height, the 40% BA ceiling, the "80-100" figure
  (`05-UNRESOLVED-EVIDENCE.md` U-4).
- The optional "five of the six papers" clarification is **left out** of this edit. #896 judged it accurate but
  beyond the single-bullet scope; it is #425's call.

## Verification behind this proposal

Producer: V-6 (36 experiments / 12 sites), V-5 (Table 2 Ae inventory), V-11 (this claim) in
`03-PRIMARY-VERIFICATION-LOG.md` — text-layer checks. Independent: #896 rendered P2 pages 1, 11, 14, 16, 17 and
Table 3 on p. 9, corrected the page pointers, and supplied the Loch Ard exception and the Fig. 7 counts. Neither
party read all six papers as a set.
