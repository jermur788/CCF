# 10 — Corrective handoff: Manager intake `NEEDS CHANGES` (B1–B3)

**Status:** `RESEARCH PRODUCED` — still **NOT INDEPENDENTLY REVIEWED**, **NOT CANONICALLY APPROVED**.
**Lane:** `research/883-condensed-evidence-coverage-review-20261120-v1`
**Prior head:** `7cbfefbe56bf648981e55f6e80d5892239af172d`
**Repair commit:** `79b87611` — all corrections and both Stage C drafts in one commit, documents only.
**Manager intake:** `883-MANAGER-INTAKE-NEEDS-CHANGES-AND-STAGE-C-REPAIR-20261120.md` (branch
`docs/883-manager-intake-needs-changes-20261120-v1`).
**Write scope observed:** `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/**` only.

## B1 — V-4 withdrawn (accepted in full)

The Manager rendered compendium p. 66 and extraction p. 12 and found both P4 Table 1 renderings correctly grouped
and mutually consistent. **The producer's "extraction error / shifted by one group" claim was a false positive**
and is withdrawn from every document. No correction to the source extraction is needed or permitted.

| Requirement | Done |
|---|---|
| `00-README.md` | Finding replaced with an explicit withdrawal paragraph |
| `02-COVERAGE-MATRIX.md` | Summary retallied; `P4-04` reclassification noted |
| `02-coverage-matrix.tsv` | `P4-04` `DISPUTED` → `MATERIAL GAP`, action rewritten |
| `03-PRIMARY-VERIFICATION-LOG.md` | V-4 row struck through and marked `CLAIM WITHDRAWN`; method limit rewritten; "Why V-4 matters" replaced by a retraction section |
| `04-PROPOSED-INCORPORATION.md` | "with defect V-4 corrected" removed; success test rewritten |
| `05-UNRESOLVED-EVIDENCE.md` | `U-3` closed, not deleted |
| `06-WHAT-CHANGED.md` | V-4 listed as WITHDRAWN |
| `07-REVIEW-PACKET.md` | Attack item inverted: the reviewer now tests the *retraction* |
| `08-…` draft | Table 1 reproduced in the article's own grouping with a correction note |

**Why it happened, recorded rather than hidden:** the producer verified tables from plain-text extraction. Two
text layers — the source's and the extraction's — were both column-interleaved, and comparing two corrupted
renderings produced a confident wrong answer. Table/parameter claims now require a rendered page. This host has no
PDF renderer (no poppler, mutool, Ghostscript, PyMuPDF), which is itself a limitation the packet now states.

**Retally after reclassification:** 67 rows = **44** MATERIAL GAP + **13** CROSS-REFERENCED + **6** COVERED +
**3** DISPUTED + **1** OUT OF SCOPE. `P4` now 6 gaps / 1 cross-reference / 0 disputed.

## B2 — Stage C drafts now exist

| File | What it is |
|---|---|
| `08-PROPOSED-COMPOSITE-Forest-Science-Methods-and-Human-Judgement.md` | The actual proposed composite: 11 sections, source manifest with DOIs and licences, per-claim original page references, units, evidence classes, transfer limits, quantitative tables, cross-reference map to the four existing composites, and a closing "what this does not authorise" |
| `09-PROPOSED-860-ADDENDUM-P2-CHANGE.md` | Exact current text, exact proposed text, a before/after table of what changes in substance, and the page pointers (P2 pp. 11, 16-17; table p. 9) |

Both are marked **PROPOSED — NOT CANONICAL, NOT ATTACHED, NOT A DECISION** in their own headers. Both live under the
#883 handoff subtree, not in `Docs/Research/**`, not on the #860 branch, not in any Project attachment.

The composite is built from the already-completed 24-page extraction. **No six-paper re-research was performed.**
Numbers not previously verified are either verified here (V-1, V-2, V-5–V-8) or flagged (V-9's "18 plots", V-10's
~49 trees/ha/yr). One deliberate omission: only the verified Ae row of P2 Table 2 is tabulated; the remaining rows
are pointed at the rendered table rather than transcribed from text.

## B3 — Row arithmetic fixed

Both the Markdown summary and the TSV now account for all 67 rows, with an explicit `OUT OF SCOPE` column and the
`O-06` row identified as the one it carries. Markdown and TSV were cross-checked programmatically (field count
per row, status tally, per-paper tally).

## What did not change

- The four composites, the #860 addendum and its branch, `Docs/Research/**`, `Docs/Project/**`, `research-index.md`,
  the #194 audit: untouched.
- No Project attachment, Drive object, Unity project, C# file, scene or save file was read or written.
- No PR, no main merge, no roster decision, no attachment slot arithmetic (still unverifiable from this host).
- The 44 gap calls, 13 cross-references and overall completeness remain **unadjudicated**; that is Stage D.

## Residual UNKNOWNs for the reviewer

1. The V-4 retraction was not re-confirmed by a local render (no renderer on this host). It rests on the Manager's
   rendered inspection plus the article's own row descriptions, which agree. One rendered look closes it.
2. P5's "18 analysed plots" (V-9) remains unverified; the other P5 counts are confirmed.
3. P5's ~49 trees/ha/year (V-10) is partially verified; the 3,360-tree figure is confirmed, the per-hectare rate
   is not recomputed.
4. Coverage verdicts remain topic-level, from heading maps, manifests and published token sweeps (U-8). This is the
   packet's main exposure and the reviewer is asked to attack it directly.
5. The proposed composite has been checked against the extraction but not against the rendered original pages for
   every one of its ~60 claims. Its own header says so.

## Next gate

Stage D: a fresh, genuinely independent non-author researcher/thread on its own docs-only branch and a **disjoint
new review path**, following the repaired `07-REVIEW-PACKET.md`. The producer must not review its own work, and the
Manager's intake explicitly was not that review. #425 retains all publication, attachment, roster and mirror
decisions.
