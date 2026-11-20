# #883 — Evidence coverage audit, targeted incorporation and review packet

**Status:** `RESEARCH PRODUCED` — **NOT INDEPENDENTLY REVIEWED**, **NOT CANONICALLY APPROVED**, **NOT PUBLISHED**.
**Authority:** supporting research only. Nothing here is a decision, a specification, a mechanic, an
attachment instruction or a replacement of any Project source. The Decision Log and #425 remain authoritative.
**Task:** https://github.com/jermur788/CCF/issues/883 · **Workspace:** `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/**` (new files only)
**Branch:** `research/883-condensed-evidence-coverage-review-20261120-v1`, base `main` @ `1511df3b6f2e7cdb8cf953c21d530947286ac280`
**Produced:** 2026-11-20. **Producer:** DeepSeek V4.1 Flash (OpenCode/T3), docs-only, no Unity/Drive/Project writes.

## What this packet answers

The owner asked why previously extracted research was not carried into the project-facing composites, and
wanted only genuine, source-supported gaps incorporated, with page traceability, followed by independent review.

Short answer, evidenced in `02-COVERAGE-MATRIX.md`:

1. The 11 November six-paper extraction was **never incorporated** into any of the four composites. This is not
   a case of "the papers were never assessed" — they were assessed well — nor a case of silent duplication.
   At topic level, **three of the six papers have zero coverage** in the composite set (P3 structural indices,
   P4 crown geometry/plasticity, P6 regeneration-method taxonomy), and two more have only adjacent
   practitioner-level coverage (P1 individual-tree management, P2 marking agreement). P5 has no coverage at all.
2. The "five academic papers" referred to in #860 are **five of the same six** (P1, P3, P4, P5, P6). There is no
   separate unassessed source set to chase. `perrin2011` and `davies2008` are **already covered** in the
   Ecology and Growth composites. `Rating experiments in forestry.pdf` (P2) is the sixth paper and is on disk.
3. Two genuine evidence defects were found and are logged, not silently fixed:
   - **Version skew:** the *Project attachment copy* of the Growth composite is a **stale pre-V1B version**
     (33,058 B) while the Git/Drive canonical is 35,555 B and carries the 23 October V1B update. Three of the
     four composites have **no Git counterpart at all**. Details in `01-SOURCE-AND-VERSION-INVENTORY.md`.
   - **Extraction mis-grouping:** the 24-page extraction's P4 parameter table is **shifted by one group**
     relative to the original Table 1. `a`/`b` are total-height parameters (not mortality), `c0`/`c1`/`d` are
     mortality-threshold parameters (not recruitment), `B`/`f` are birth intensity/rate (not crown movement).
     Verified against the original paper pages; see `03-PRIMARY-VERIFICATION-LOG.md` item V-4.
4. The #860 addendum's one P2 figure ("slight-to-fair agreement, 36 experiments, 12 sites", explicitly marked
   *not verified against the paper*) is now **primary-verified** and can be promoted from secondhand to cited.

## Reading order

| File | Purpose |
|---|---|
| `01-SOURCE-AND-VERSION-INVENTORY.md` + `.tsv` | Every source read, its bytes/SHA-256, location, and Git-vs-attachment version skew |
| `02-COVERAGE-MATRIX.md` + `.tsv` | The claim/topic crosswalk: each P1–P6 topic and each other unincorporated source, classified |
| `03-PRIMARY-VERIFICATION-LOG.md` | High-impact figures checked at the 111-page original, with quoted evidence and outcomes |
| `04-PROPOSED-INCORPORATION.md` | Minimal proposed additions (one new composite, not four rewrites) and what deliberately stays out |
| `05-UNRESOLVED-EVIDENCE.md` | Open conflicts, unverified secondhand material, and what would close each item |
| `06-WHAT-CHANGED.md` | Change log against the four composites and the #860 addendum |
| `07-REVIEW-PACKET.md` | Brief for the independent non-author reviewer (Stage D) |

## Honest limits of this pass

- The 24-page extraction was read **in full** (1,017 lines). The 111-page volume was read **selectively** at the
  pages the extraction cites (targeted verification, per the task's "do not reread all papers" instruction).
- The four composites were read at **heading, manifest and targeted-concept-sweep level**, not line-by-line
  end-to-end. Coverage verdicts are therefore *topic-level*, and each one names the concept tokens searched so
  a reviewer can reproduce or overturn it. This is the weakest part of the packet and the reviewer should attack it.
- `perrin2011.pdf` and `davies2008.pdf` were confirmed present and their composite coverage located; the
  original papers were **not** re-read for this pass.
- No Project attachment was opened, replaced or re-uploaded. No Drive file was moved. No Unity, C#, save or
  scene file was touched.
- **Stage D is not satisfied.** This producer cannot supply its own independent reviewer. See `07-REVIEW-PACKET.md`.
