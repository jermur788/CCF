# #883 — Manager intake verdict and minimum Stage C repair packet

**Date:** 2026-11-20. **Disposition:** **NEEDS CHANGES — producer evidence packet, before Stage D quality approval.** This is a bounded Manager/source-page intake, **not** the full independent scientific Stage D review and **not** canonical publication.  
**Producer:** DeepSeek V4.1 Flash, branch `research/883-condensed-evidence-coverage-review-20261120-v1`, exact commit `7cbfefbe56bf648981e55f6e80d5892239af172d`.  
**Producer handoff:** https://github.com/jermur788/CCF/blob/research/883-condensed-evidence-coverage-review-20261120-v1/Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/00-README.md  
**Binding original delegation:** https://github.com/jermur788/CCF/blob/docs/883-research-evidence-incorporation-review-delegation-20261120-v1/Docs/Handoffs/Research-ProjectSources-20261120/Sen-1-883-MANAGER-RESEARCH-EVIDENCE-COVERAGE-INCORPORATION-AND-INDEPENDENT-QUALITY-REVIEW-DELEGATION-20261120.md  
**Publication authority:** #425. **Live coordination:** `Docs/Project/coordination/active-tasks.md`, read-only to this intake; only its authorised manager writer may edit it. No producer/reviewer source or Unity lease inferred.

## GitHub-verified intake

Compared base `1511df3b6f2e7cdb8cf953c21d530947286ac280` to the producer branch. Result: **ahead one commit, behind zero**, ten new files in `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/`, **687 additions and zero deletions**. No replacement of canonical research, Project attachments or #860 addendum is shown by this comparison. GitHub files `00`–`07` and the original #883 delegation were read. This proves repository scope; it does not validate the science, the locally reported hash inventory, Project live attachment membership, or Drive bytes.

## Blocking findings

### B1 — V-4 is a false-positive source defect: withdraw the claim

**Directly inspected rendered pages, not OCR/text-line ordering:**

- `CCF_Six_Complete_Forestry_Research_Papers.pdf`, **compendium page 66**, P4 CanopyShotNoise **Table 1** (original article journal page 6847).
- `CCF_Six_Papers_Relevance_Evidence_Extraction.pdf`, **page 12**, **Published parameters — preserve provenance, do not adopt**.

The *existing 24-page extraction table is correctly grouped and correctly labelled*. Both rendered tables agree:

| Parameter | True meaning | Already in extraction p.12 |
|---|---|---|
| `a=0.5697, b=9.1091` | total-height relationship | **Correct: Height relationship** |
| `c0=1.0200, c1=0.0200, d=0.0060` | mortality thresholds | **Correct: Mortality** |
| `λB=0.0390, f=0.0900` | annual birth intensity and birth rate per crown-projection area | **Correct: Recruitment** |
| `R=0.4000 m` | annual crown-centre movement radius | **Correct: Crown movement** |

The producer's V-4 appears to have treated text-layer row-order corruption as evidence that the *rendered extraction* was wrong. It was not. **Do not alter the source extraction's correct table.** Retract `EXTRACTION ERROR — groups shifted by one` from the packet and from any downstream advice. A valid observation remains that plain-text PDF extraction can scramble table alignment.

Required linked changes: `00-README.md` lines 29–32; `02-coverage-matrix.tsv` P4-04 classification/action; `03-PRIMARY-VERIFICATION-LOG.md` V-4, method limit and explanatory section; `04-PROPOSED-INCORPORATION.md` references to fixing V-4; `05-UNRESOLVED-EVIDENCE.md` U-3; `06-WHAT-CHANGED.md` V-4; and `07-REVIEW-PACKET.md` V-4 attack/verification wording. Retally the matrix if P4-04 changes status. Do not mechanically relabel that row without assessing coverage.

### B2 — Stage C's proposed *content files* are absent

Original #883 delegation §3 Stage C and §4 required **minimal proposed revised evidence document(s) and #860 proposed addendum revision**, inside the **new-only task workspace**, before Stage D. The produced `04-PROPOSED-INCORPORATION.md` is an outline; the 10-file Git diff contains no `CCF_Forest_Science_Methods_and_Human_Judgement_Evidence_Composite.md` draft and no proposed addendum draft/patch. Therefore **analysis/plan produced, incorporation candidate not yet produced**.

Required: write an actual **proposed, not canonical** compact research composite for the six-paper family, *if it remains the evidence-backed minimal choice*, with source manifests, P1–P6 original page references, geography/class/transfer limits, quantitative values with units and caveats, and retrieval-ready sections. Base it on the already completed 24-page extraction; do not start a six-paper re-research. Retain the four existing composites unchanged. Provide the **actual proposed addendum text or narrow marked diff** for P2 V-11 with primary page pointers and secondhand-to-primary provenance transition. Both drafts belong under the existing #883 task handoff subtree, **not** under canonical `Docs/Research/**`, live #860 source branch or Project attachments. If source/page facts cannot be proven, mark them unresolved rather than inventing them.

### B3 — Coverage summary omits the out-of-scope row

`02-COVERAGE-MATRIX.md` states 67 rows but shows only 43 MATERIAL GAP + 13 CROSS-REF + 6 COVERED + 4 DISPUTED = **66**. The accompanying `02-coverage-matrix.tsv` contains `O-06` with `OUT OF SCOPE`, which explains the missing row. Add `OUT OF SCOPE = 1` column and retally after B1. This is a reporting correction, not proof that the audit's semantic classifications are accurate.

## Not yet independently adjudicated

The producer's 43 gap calls, 13 cross-references, P2 V-11 claim, local Git/Drive/attachment SHA-256 comparison, V-9/P5 qualifications and overall completeness need a **separate Stage D reviewer**, not this narrow Manager intake. The current topic/token sweep over 130,756 bytes is expressly not a line-by-line content audit. A sample review must challenge its false-negative risk and the `CROSS-REFERENCED` borderline rows. Existing docs are not sufficient to claim an approved Project attachment or science-quality PASS.

## Minimum continuation — same producer, no premature independent gate

**Task class:** PACKET ONLY (research/doc correction). **Difficulty:** medium substantive scientific editing.  
**Primary author:** original #883 producer if available and properly re-registered; else an explicitly handed-off single docs writer. **Proposed lane:** existing `research/883-condensed-evidence-coverage-review-20261120-v1` from exact `7cbfefbe56bf648981e55f6e80d5892239af172d`. Existing producer worktree and dirty-file status require *actual local preflight*, not an inference from GitHub. **Write allowlist:** only `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/**`. Exclude all other paths, including canonical `Docs/Research/**`, `Docs/Project/**`, original PDFs, #860 branch, Unity/C#/save/files and Drive. No PR or main merge without #425.

Checks: (1) render and compare P4 original compendium p.66 and extraction p.12; (2) withdraw every false V-4 occurrence; (3) document actual proposed composite and P2 addendum text; (4) validate proposed rows' original page/table references, evidence classes, units, source limits; (5) match totals across Markdown/TSV; (6) recompare branch to base, no out-of-scope changes; (7) push one GitHub Markdown corrective handoff with exact new SHA and file inventory. Stop only an action blocked by missing original pages, contested source scope or unapproved canonical publication; do not treat science as product authority.

**Only then commission Stage D:** fresh independent nonauthor researcher/thread, not the author or the author's helper, on its own docs-only branch and **disjoint new review file path**. Follow `07-REVIEW-PACKET.md` after it is repaired; source traceability, scientific quantities, coverage completeness, and *actual draft attachment* practicality each need explicit APPROVED / NEEDS CHANGES / REJECTED evidence. Reviewer must inspect P4 rendered Table 1 and original high-risk figures; never accept a text-layer-only verification. #425 alone decides canonical source publication, attachment changes, new slot and mirror/context changes.

## Paste-ready single producer continuation dispatch

> **CCF #883 — REPAIR PRODUCER PACKET BEFORE INDEPENDENT REVIEW.** Difficulty MEDIUM research/doc-only. Provider: original DeepSeek V4.1 Flash if actually available and legally owns its isolated lane, or one separately registered capable docs producer. **DIRECT binding Manager handoff:** https://github.com/jermur788/CCF/blob/docs/883-manager-intake-needs-changes-20261120-v1/Docs/Handoffs/Research-ProjectSources-20261120/883-MANAGER-INTAKE-NEEDS-CHANGES-AND-STAGE-C-REPAIR-20261120.md . Target `research/883-condensed-evidence-coverage-review-20261120-v1` exact prior commit `7cbfefbe56bf648981e55f6e80d5892239af172d`, isolated docs-only worktree to be freshly confirmed/claimed; sole writes inside `Docs/Handoffs/Research-ProjectSources-20261120/883-Evidence-Coverage-Quality/**` after refresh of live `active-tasks.md` and owner/branch/lane checks. **B1:** rendered original P4 Table 1 at 111-page compendium p.66 and rendered existing 24-page extraction p.12 match; the claimed V-4 one-group-shift defect is **false**. Remove it from the README, matrix TSV, verification log, proposal, unresolved log, change log and review packet. **B2:** produce the *actual proposed* compact P1–P6 methods/human-judgement research composite and narrow proposed #860 P2 addendum change, traceable to page/units/evidence class; `04-PROPOSED-INCORPORATION.md` alone is insufficient Stage C. **B3:** show 67-row tally including one OUT OF SCOPE and account for any P4-04 reclassification. Keep existing four composites and #860 authoritative locations unedited; no Project/Drive/Unity/scene/save/main/PR/approved mechanic change. Run focused PDF-rendered verification and branch diff, then publish exact GitHub result and flag residual UNKNOWN. **Do not self-review.** Fresh independent Stage D follows only after correction; #425 retains all publication decisions. If the original author is unavailable, stop claiming that author, reallocate openly, preserve the pushed packet and work only in a disjoint newly assigned lane.
